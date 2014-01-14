using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(44, "购买物品")]
	public class UserBuyItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			eMessageType type = eMessageType.Normal;
			string translateId = "UserBuyItemHandler.Success";
			List<ItemInfo> list = new List<ItemInfo>();
			List<bool> list2 = new List<bool>();
			List<int> list3 = new List<int>();
			StringBuilder stringBuilder2 = new StringBuilder();
			Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
			bool flag = false;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			int num6 = packet.ReadInt();
			for (int i = 0; i < num6; i++)
			{
				packet.ReadInt();
				int iD = packet.ReadInt();
				int num7 = packet.ReadInt();
				string text = packet.ReadString();
				bool item = packet.ReadBoolean();
				string text2 = packet.ReadString();
				int item2 = packet.ReadInt();
				packet.ReadBoolean();
				ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
				if (shopItemInfoById == null)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
					return 1;
				}
				if (!ShopMgr.CanBuy(shopItemInfoById.ShopID, (consortiaInfo == null) ? 1 : consortiaInfo.ShopLevel, ref flag, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
					return 1;
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				if (shopItemInfoById.BuyType == 0)
				{
					if (1 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.AUnit;
					}
					if (2 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.BUnit;
					}
					if (3 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.CUnit;
					}
				}
				else
				{
					if (1 == num7)
					{
						itemInfo.Count = shopItemInfoById.AUnit;
					}
					if (2 == num7)
					{
						itemInfo.Count = shopItemInfoById.BUnit;
					}
					if (3 == num7)
					{
						itemInfo.Count = shopItemInfoById.CUnit;
					}
				}
				if (itemInfo != null || shopItemInfoById != null)
				{
					itemInfo.Color = ((text == null) ? "" : text);
					itemInfo.Skin = ((text2 == null) ? "" : text2);
					if (flag)
					{
						itemInfo.IsBinds = true;
					}
					else
					{
						itemInfo.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
					}
					stringBuilder2.Append(num7);
					stringBuilder2.Append(",");
					if (!dictionary.Keys.Contains(itemInfo.TemplateID))
					{
						dictionary.Add(itemInfo.TemplateID, itemInfo);
					}
					else
					{
						dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
					}
					list2.Add(item);
					list3.Add(item2);
					ItemInfo.SetItemType(shopItemInfoById, num7, ref num, ref num2, ref num3, ref num4, ref num5);
				}
			}
			int num8 = packet.ReadInt();
			if (dictionary.Values.Count == 0)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			if (num <= client.Player.PlayerCharacter.Gold && num2 <= client.Player.PlayerCharacter.Money && num3 <= client.Player.PlayerCharacter.Offer && num4 <= client.Player.PlayerCharacter.GiftToken)
			{
				client.Player.AddExpVip(num2);
				client.Player.RemoveMoney(num2);
				client.Player.RemoveGold(num);
				client.Player.RemoveOffer(num3);
				client.Player.RemoveGiftToken(num4);
				string text3 = "";
				foreach (ItemInfo current in dictionary.Values)
				{
					text3 += ((text3 == "") ? current.TemplateID.ToString() : ("," + current.TemplateID.ToString()));
					switch (num8)
					{
					case 1:
					case 2:
						if (!UserBuyItemHandler.AddItemsToStoreBag(client, current))
						{
							list.Add(current);
						}
						break;

					default:
						if (!client.Player.AddTemplate(current, (eBageType)current.GetBagType, current.Count, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView))
						{
							list.Add(current);
						}
						break;
					}
				}
				bool flag2 = false;
				if (list.Count > 0)
				{
					using (new PlayerBussiness())
					{
						flag2 = client.Player.SendItemsToMail(list, LanguageMgr.GetTranslation("UserBuyItemHandler.Mail", new object[0]), LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]), eMailType.BuyItem);
						translateId = "UserBuyItemHandler.Mail";
					}
				}
				if (flag2)
				{
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Player.OnPaid(num2, num, num3, num4, num5, stringBuilder.ToString());
				LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, num, num4, num3, num5, "牌子编号", text3, stringBuilder2.ToString());
			}
			else
			{
				if (num2 > client.Player.PlayerCharacter.Money)
				{
					translateId = "UserBuyItemHandler.NoMoney";
				}
				if (num > client.Player.PlayerCharacter.Gold)
				{
					translateId = "UserBuyItemHandler.NoGold";
				}
				if (num3 > client.Player.PlayerCharacter.Offer)
				{
					translateId = "UserBuyItemHandler.NoOffer";
				}
				if (num4 > client.Player.PlayerCharacter.GiftToken)
				{
					translateId = "UserBuyItemHandler.GiftToken";
				}
				if (num5 > client.Player.PlayerCharacter.medal)
				{
					translateId = "UserBuyItemHandler.Medal";
				}
				type = eMessageType.ERROR;
			}
			client.Player.MainBag.SaveToDatabase();
			client.Player.PropBag.SaveToDatabase();
			client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
		private static bool AddItemsToStoreBag(GameClient client, ItemInfo item)
		{
			int num = 2;
			if (item.TemplateID == 11018 || item.TemplateID == 11025)
			{
				num = 0;
			}
			ItemInfo itemAt = client.Player.StoreBag.GetItemAt(num);
			if (itemAt != null && itemAt.Count < 999 && itemAt.CanStackedTo(item))
			{
				return client.Player.StoreBag.AddTemplateAt(item, item.Count, num);
			}
			if (itemAt == null)
			{
				return client.Player.StoreBag.AddItemTo(item, num);
			}
			return client.Player.AddTemplate(item, (eBageType)item.GetBagType, item.Count, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView);
		}
	}
}
