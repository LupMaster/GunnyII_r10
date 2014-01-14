using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(126, "场景用户离开")]
	public class QuickBuyGoldBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadBoolean();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(1123301);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			int num2 = num * shopItemInfoById.AValue1;
			if (client.Player.PlayerCharacter.Money > num2)
			{
				client.Player.RemoveMoney(num2);
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				List<ItemInfo> itemInfos = new List<ItemInfo>();
				ItemBoxMgr.CreateItemBox(itemTemplateInfo.TemplateID, itemInfos, ref num4, ref num3, ref num5, ref num6, ref num7);
				int num8 = num * num4;
				client.Player.AddGold(num8);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bạn nhận được " + num8 + " vàng.", new object[0]));
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney", new object[0]));
			}
			return 0;
		}
	}
}
