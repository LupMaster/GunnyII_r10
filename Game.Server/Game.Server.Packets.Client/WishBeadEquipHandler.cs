using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(106, "场景用户离开")]
	public class WishBeadEquipHandler : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int bageType = packet.ReadInt();
			int templateId = packet.ReadInt();
			int place = packet.ReadInt();
			int bagType = packet.ReadInt();
			int templateId2 = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			ItemInfo itemInfo = inventory.GetItemAt(num);
			client.Player.GetItemAt((eBageType)bagType, place);
			double num2 = 5.0;
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipNewTemplate(templateId);
			GSPacketIn gSPacketIn = new GSPacketIn(106, client.Player.PlayerCharacter.ID);
			if (goldEquipTemplateLoadInfo == null && itemInfo.Template.CategoryID == 7)
			{
				gSPacketIn.WriteInt(5);
			}
			else
			{
				if (!itemInfo.IsGold)
				{
					if (num2 > (double)WishBeadEquipHandler.random.Next(100))
					{
						itemInfo.StrengthenLevel++;
						itemInfo.IsGold = true;
						itemInfo.goldBeginTime = DateTime.Now;
						itemInfo.goldValidDate = 30;
						itemInfo.IsBinds = true;
						if (goldEquipTemplateLoadInfo != null && itemInfo.Template.CategoryID == 7)
						{
							ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo.NewTemplateId);
							if (itemTemplateInfo != null)
							{
								ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
								inventory.RemoveItemAt(num);
								inventory.AddItemTo(itemInfo2, num);
								itemInfo = itemInfo2;
							}
						}
						inventory.UpdateItem(itemInfo);
						gSPacketIn.WriteInt(0);
						inventory.SaveToDatabase();
					}
					else
					{
						gSPacketIn.WriteInt(1);
					}
					client.Player.RemoveTemplate(templateId2, 1);
				}
				else
				{
					gSPacketIn.WriteInt(6);
				}
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
