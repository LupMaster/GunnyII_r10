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
	[PacketHandler(26, "打开物品")]
	public class LotteryOpenBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			new ProduceBussiness();
			if (client.Lottery != -1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đang hoạt động!");
				return 1;
			}
			int bageType = (int)packet.ReadByte();
			int slot = packet.ReadInt();
			int num = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			inventory.GetItemAt(slot);
			if (inventory.FindFirstEmptySlot() == -1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
				return 1;
			}
			PlayerInventory propBag = client.Player.PropBag;
			ItemInfo itemByTemplateID = propBag.GetItemByTemplateID(0, 11456);
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			if (!ItemBoxMgr.CreateItemBox(num, list, ref num3, ref num2, ref num4, ref num5, ref num6))
			{
				client.Player.SendMessage("Sảy ra lổi hảy thử lại sau.");
				return 0;
			}
			if (num2 != 0)
			{
				stringBuilder.Append(num2 + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
				client.Player.AddMoney(num2);
			}
			if (num3 != 0)
			{
				stringBuilder.Append(num3 + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
				client.Player.AddGold(num3);
			}
			if (num4 != 0)
			{
				stringBuilder.Append(num4 + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
				client.Player.AddGiftToken(num4);
			}
			if (num5 != 0)
			{
				stringBuilder.Append(num5 + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]));
				client.Player.AddMedal(num5);
			}
			if (num6 != 0)
			{
				stringBuilder.Append(num6 + LanguageMgr.GetTranslation("OpenUpArkHandler.Exp", new object[0]));
				client.Player.AddGP(num6);
			}
			int index = ThreadSafeRandom.NextStatic(list.Count);
			ItemInfo itemInfo = list[index];
			bool val = true;
			int num7 = num;
			string str;
			if (num7 != 112047)
			{
				switch (num7)
				{
				case 112100:
				case 112101:
					break;

				default:
					str = itemInfo.Template.Name;
					goto IL_270;
				}
			}
			str = client.Player.PlayerCharacter.NickName;
			if (itemByTemplateID.Count > 4)
			{
				itemByTemplateID.Count -= 4;
				propBag.UpdateItem(itemByTemplateID);
			}
			else
			{
				propBag.RemoveItem(itemByTemplateID);
			}
			IL_270:
			GSPacketIn gSPacketIn = new GSPacketIn(245, client.Player.PlayerId);
			gSPacketIn.WriteBoolean(val);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteString(str);
			gSPacketIn.WriteInt(itemInfo.TemplateID);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteBoolean(false);
			client.Out.SendTCP(gSPacketIn);
			inventory.AddItem(itemInfo);
			stringBuilder.Append(itemInfo.Template.Name);
			ItemInfo itemByTemplateID2 = client.Player.PropBag.GetItemByTemplateID(0, num);
			if (itemByTemplateID2 != null)
			{
				if (itemByTemplateID2.Count > 1)
				{
					itemByTemplateID2.Count--;
					client.Player.PropBag.UpdateItem(itemByTemplateID2);
				}
				else
				{
					client.Player.PropBag.RemoveItem(itemByTemplateID2);
				}
			}
			client.Lottery = -1;
			if (stringBuilder != null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Bạn nhận được " + stringBuilder.ToString());
			}
			return 1;
		}
	}
}
