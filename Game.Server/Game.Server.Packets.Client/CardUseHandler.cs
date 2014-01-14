using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(183, "卡片使用")]
	public class CardUseHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			string translateId = null;
			ItemInfo itemInfo = null;
			ShopItemInfo shopItemInfo = new ShopItemInfo();
			List<ItemInfo> list = new List<ItemInfo>();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			if (num == -1 && num2 == -1)
			{
				int num3 = packet.ReadInt();
				int templatID = packet.ReadInt();
				int num4 = packet.ReadInt();
				int num5 = 0;
				int num6 = 0;
				for (int i = 0; i < num3; i++)
				{
					shopItemInfo = ShopMgr.FindShopbyTemplateID(templatID);
					if (shopItemInfo != null)
					{
						ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfo.TemplateID);
						itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
						num6 = shopItemInfo.AValue1;
						itemInfo.ValidDate = shopItemInfo.AUnit;
					}
					if (itemInfo != null)
					{
						if (num5 <= client.Player.PlayerCharacter.Gold && num6 <= client.Player.PlayerCharacter.Money)
						{
							client.Player.RemoveMoney(num6);
							client.Player.RemoveGold(num5);
							LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Card, client.Player.PlayerCharacter.ID, num6, client.Player.PlayerCharacter.Money, num5, 0, 0, 0, "牌子编号", itemInfo.TemplateID.ToString(), num4.ToString());
							translateId = "CardUseHandler.Success";
						}
						list.Add(itemInfo);
					}
				}
			}
			else
			{
				PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
				itemInfo = inventory.GetItemAt(num2);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
				translateId = "CardUseHandler.Success";
			}
			if (list.Count > 0)
			{
				string translateId2 = string.Empty;
				using (List<ItemInfo>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ItemInfo current = enumerator.Current;
						if (current.Template.Property1 != 21)
						{
							AbstractBuffer abstractBuffer = BufferList.CreateBuffer(current.Template, current.ValidDate);
							if (abstractBuffer != null)
							{
								abstractBuffer.Start(client.Player);
								if (num2 != -1 && num != -1)
								{
									PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
									inventory.RemoveCountFromStack(current, 1);
								}
							}
							client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
						}
						else
						{
							if (current.IsValidItem())
							{
                                client.Player.PlayerCharacter.GP += client.Player.AddGP(itemInfo.Template.Property2 * itemInfo.Count);//fix add nuoc kinh nghiem //client.Player.AddGP(current.Template.Property2 * current.Count);
								if (current.Template.CanDelete)
								{
									client.Player.RemoveAt((eBageType)num, num2);
									translateId2 = "GPDanUser.Success";
								}
							}
							client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId2, new object[]
							{
								itemInfo.Template.Property2 * itemInfo.Count
							}));
						}
					}
					return 0;
				}
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail", new object[0]));
			return 0;
		}
	}
}
