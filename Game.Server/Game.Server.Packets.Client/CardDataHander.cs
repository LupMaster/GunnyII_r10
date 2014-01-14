using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.Packets.Client
{
	[PacketHandler(216, "防沉迷系统开关")]
	internal class CardDataHander : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			PlayerInfo arg_12_0 = client.Player.PlayerCharacter;
			CardInventory cardBag = client.Player.CardBag;
			List<ItemInfo> list = new List<ItemInfo>();
			string text = "";
			switch (num)
			{
			case 0:
				{
					int num2 = packet.ReadInt();
					int num3 = packet.ReadInt();
					UsersCardInfo itemAt = cardBag.GetItemAt(num2);
					if (itemAt == null)
					{
						client.Out.SendMessage(eMessageType.Normal, "Không tìm thấy thẻ này, thử lại sau.");
						return 0;
					}
					if (cardBag.FindEquipCard(itemAt.TemplateID) && num2 != num3)
					{
						text = "Thẻ này đã trang bị!";
					}
					else
					{
						if (num2 != num3)
						{
							text = "Trang bị thành công!";
							client.Player.MainBag.UpdatePlayerProperties();
						}
						cardBag.MoveItem(num2, num3);
					}
					if (text != "")
					{
						client.Out.SendMessage(eMessageType.Normal, text);
						goto IL_2F8;
					}
					goto IL_2F8;
				}

			case 1:
			case 4:
				{
					int slot = packet.ReadInt();
					int num4 = packet.ReadInt();
					ItemInfo itemAt2 = client.Player.MainBag.GetItemAt(slot);
					if (num == 4)
					{
						itemAt2 = client.Player.PropBag.GetItemAt(slot);
					}
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					int num8 = 0;
					int num9 = 0;
					bool flag = false;
					if (ItemBoxMgr.CreateItemBox(itemAt2.TemplateID, list, ref num6, ref num5, ref num7, ref num8, ref num9))
					{
						int index = CardDataHander.random.Next(list.Count);
						ItemInfo itemInfo = list[index];
						flag = this.TakeCard(client, itemInfo.Template.Property5);
					}
					if (!flag)
					{
						int num10 = CardDataHander.random.Next(5, 25);
						client.Player.AddCardSoul(num10);
						client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, true, num10);
					}
					if (num == 4)
					{
						client.Player.PropBag.RemoveCountFromStack(itemAt2, num4);
						goto IL_2F8;
					}
					client.Player.MainBag.RemoveCountFromStack(itemAt2, num4);
					goto IL_2F8;
				}

			case 2:
				{
					int num4 = packet.ReadInt();
					List<UsersCardInfo> cards = cardBag.GetCards(5, cardBag.Capalility);
					if (num4 == cards.Count)
					{
						cardBag.BeginChanges();
						try
						{
							try
							{
								UsersCardInfo[] rawSpaces = cardBag.GetRawSpaces();
								cardBag.ClearBag();
								for (int i = 0; i < num4; i++)
								{
									int num11 = packet.ReadInt();
									int num12 = packet.ReadInt();
									UsersCardInfo card = rawSpaces[num11];
									if (!cardBag.AddCardTo(card, num12))
									{
										CardDataHander.log.Warn(string.Format("move card error: old place:{0} new place:{1}", num11, num12));
									}
								}
							}
							catch (Exception ex)
							{
								CardDataHander.log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message);
							}
							goto IL_2F8;
						}
						finally
						{
							cardBag.CommitChanges();
						}
					}
					CardDataHander.log.Warn(string.Format("Count: {0} recoverCards.Count:{1} ", num4, cards.Count));
					goto IL_2F8;
				}
			}
			Console.WriteLine("??????????????????????????????????????????????cmdCard: " + num);
			IL_2F8:
			cardBag.SaveToDatabase();
			return 0;
		}
		private bool TakeCard(GameClient client, int templateId)
		{
			bool result = false;
			int place = client.Player.CardBag.FindFirstEmptySlot(5);
			CardTemplateInfo card = CardMgr.GetCard(templateId);
			if (card != null)
			{
				int num = client.Player.CardBag.FindPlaceByTamplateId(5, templateId);
				UsersCardInfo usersCardInfo;
				if (num == -1)
				{
					usersCardInfo = new UsersCardInfo();
					usersCardInfo.CardType = card.CardType;
					usersCardInfo.UserID = client.Player.PlayerCharacter.ID;
					usersCardInfo.Place = place;
					usersCardInfo.TemplateID = card.CardID;
					usersCardInfo.isFirstGet = true;
					usersCardInfo.Attack = 0;
					usersCardInfo.Agility = 0;
					usersCardInfo.Defence = 0;
					usersCardInfo.Luck = 0;
					usersCardInfo.Damage = 0;
					usersCardInfo.Guard = 0;
				}
				else
				{
					usersCardInfo = client.Player.CardBag.GetItemAt(num);
					if (usersCardInfo.CardType < card.CardType)
					{
						usersCardInfo.isFirstGet = true;
						usersCardInfo.CardType = card.CardType;
					}
					else
					{
						usersCardInfo = null;
					}
				}
				if (usersCardInfo != null)
				{
					if (num == -1)
					{
						result = client.Player.CardBag.AddCardTo(usersCardInfo, place);
					}
					client.Out.SendGetCard(client.Player.PlayerCharacter, usersCardInfo);
				}
			}
			return result;
		}
	}
}
