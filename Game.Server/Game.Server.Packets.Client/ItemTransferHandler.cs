using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(61, "物品转移")]
	public class ItemTransferHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(61, client.Player.PlayerCharacter.ID);
			new StringBuilder();
			int num = 40000;
			bool tranHole = packet.ReadBoolean();
			bool tranHoleFivSix = packet.ReadBoolean();
			ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(0);
			ItemInfo itemInfo2 = client.Player.StoreBag.GetItemAt(1);
			if (itemInfo != null && itemInfo2 != null && itemInfo.Template.CategoryID == itemInfo2.Template.CategoryID && itemInfo2.Count == 1 && itemInfo.Count == 1 && itemInfo.IsValidItem() && itemInfo2.IsValidItem())
			{
				if (client.Player.PlayerCharacter.Gold < num)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold", new object[0]));
					return 1;
				}
				client.Player.RemoveGold(num);
				if (itemInfo.Template.CategoryID == 7 || itemInfo2.Template.CategoryID == 7)
				{
					int templateID = itemInfo.TemplateID;
					int templateID2 = itemInfo2.TemplateID;
					this.GetWeaponID(ref templateID, ref templateID2);
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateID);
					ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(templateID2);
					if (itemTemplateInfo == null || itemTemplateInfo2 == null)
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition", new object[0]) + " code 61");
						return 0;
					}
					ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
					itemInfo = itemInfo3;
					ItemInfo itemInfo4 = ItemInfo.CloneFromTemplate(itemTemplateInfo2, itemInfo2);
					itemInfo2 = itemInfo4;
				}
				StrengthenMgr.InheritTransferProperty(ref itemInfo, ref itemInfo2, tranHole, tranHoleFivSix);
				client.Player.StoreBag.ClearBag();
				client.Player.StoreBag.AddItemTo(itemInfo, 0);
				client.Player.StoreBag.AddItemTo(itemInfo2, 1);
				client.Player.SaveIntoDatabase();
				gSPacketIn.WriteByte(0);
				client.Out.SendTCP(gSPacketIn);
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition", new object[0]));
			}
			return 0;
		}
		private void GetWeaponID(ref int idZero, ref int idOne)
		{
			string text = idZero.ToString().Substring(0, 4);
			string text2 = idOne.ToString().Substring(0, 4);
			text += idOne.ToString().Substring(4);
			text2 += idZero.ToString().Substring(4);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(int.Parse(text));
			if (itemTemplateInfo == null || itemTemplateInfo.CategoryID == 27)
			{
				if (this.IsSpecialTemplate(text))
				{
					text = text.Substring(0, 4) + "8";
				}
				else
				{
					text = text.Substring(0, 4) + "4";
				}
			}
			itemTemplateInfo = ItemMgr.FindItemTemplate(int.Parse(text2));
			if (itemTemplateInfo == null || itemTemplateInfo.CategoryID == 27)
			{
				if (this.IsSpecialTemplate(text2))
				{
					text2 = text2.Substring(0, 4) + "8";
				}
				else
				{
					text2 = text2.Substring(0, 4) + "4";
				}
			}
			idZero = int.Parse(text);
			idOne = int.Parse(text2);
		}
		private bool IsSpecialTemplate(string id)
		{
			return id != null && (id == "70244" || id == "70264" || id == "70274");
		}
	}
}
