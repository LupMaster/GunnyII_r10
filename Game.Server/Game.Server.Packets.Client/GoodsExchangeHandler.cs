using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(31, "场景用户离开")]
	public class GoodsExchangeHandler : IPacketHandler
	{
		private int GetGoodsAward(int index)
		{
			switch (index)
			{
			case 1:
				return 3;

			case 2:
				return 5;

			case 3:
				return 7;

			default:
				return 1;
			}
		}
		private List<ActiveConvertItemInfo> GetActiveConvertItem(ActiveBussiness db, int id, int index, int lengh)
		{
			ActiveConvertItemInfo[] singleActiveConvertItems = db.GetSingleActiveConvertItems(id);
			List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
			ActiveConvertItemInfo[] array = singleActiveConvertItems;
			for (int i = 0; i < array.Length; i++)
			{
				ActiveConvertItemInfo activeConvertItemInfo = array[i];
				if (activeConvertItemInfo.ItemType == this.GetGoodsAward(index))
				{
					for (int j = 0; j < lengh; j++)
					{
						list.Add(activeConvertItemInfo);
					}
				}
			}
			return list;
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			using (ActiveBussiness activeBussiness = new ActiveBussiness())
			{
				int num = packet.ReadInt();
				packet.ReadInt();
				int num2 = packet.ReadInt();
				int i = 0;
				ActiveInfo singleActives = activeBussiness.GetSingleActives(num);
				int num3 = Convert.ToInt32(singleActives.GoodsExchangeNum);
				ItemInfo item = null;
				PlayerInventory playerInventory = null;
				while (i < num2)
				{
					int templateId = packet.ReadInt();
					packet.ReadInt();
					int bageType = packet.ReadInt();
					playerInventory = client.Player.GetInventory((eBageType)bageType);
					item = playerInventory.GetItemByTemplateID(0, templateId);
					int itemCount = playerInventory.GetItemCount(templateId);
					if (itemCount < num3 || itemCount < 0)
					{
						client.Out.SendMessage(eMessageType.Normal, "vật phẩm không đủ!");
						break;
					}
					i++;
				}
				int index = packet.ReadInt();
				StringBuilder stringBuilder = new StringBuilder();
				List<ActiveConvertItemInfo> activeConvertItem = this.GetActiveConvertItem(activeBussiness, num, index, num2);
				foreach (ActiveConvertItemInfo current in activeConvertItem)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(current.TemplateID);
					if (itemTemplateInfo == null)
					{
						client.Out.SendMessage(eMessageType.Normal, "Lổi phần thưởng liên hệ admin!");
						break;
					}
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
					itemInfo.IsBinds = current.IsBind;
					client.Player.AddTemplate(itemInfo, itemInfo.Template.BagType, current.ItemCount, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView);
					stringBuilder.Append(itemTemplateInfo.Name + " x" + current.ItemCount.ToString() + ". ");
				}
				playerInventory.RemoveCountFromStack(item, num3 * num2);
				if (stringBuilder.Length > 0)
				{
					client.Out.SendMessage(eMessageType.Normal, stringBuilder.ToString());
				}
			}
			return 0;
		}
	}
}
