using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(63, "打开物品")]
	public class OpenUpArkHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int bageType = (int)packet.ReadByte();
			int slot = packet.ReadInt();
			int num = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			ItemInfo itemAt = inventory.GetItemAt(slot);
			string text = "";
			if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel)
			{
				if (num < 1 || num > itemAt.Count)
				{
					num = itemAt.Count;
				}
				string str = "";
				int num2 = 0;
				string arg = "";
				StringBuilder stringBuilder = new StringBuilder();
				List<ItemInfo> list = new List<ItemInfo>();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (!inventory.RemoveCountFromStack(itemAt, num))
				{
					return 0;
				}
				stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
				for (int i = 0; i < num; i++)
				{
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					List<ItemInfo> list2 = new List<ItemInfo>();
					ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list2, ref num4, ref num3, ref num5, ref num6, ref num7);
					if (num3 != 0)
					{
						num2 += num3;
						arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]);
						client.Player.AddMoney(num3);
						LogMgr.LogMoneyAdd(LogMoneyType.Box, LogMoneyType.Box_Open, client.Player.PlayerCharacter.ID, num3, client.Player.PlayerCharacter.Money, num4, 0, 0, 0, "", "", "");
					}
					if (num4 != 0)
					{
						num2 += num4;
						arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]);
						client.Player.AddGold(num4);
					}
					if (num5 != 0)
					{
						num2 += num5;
						arg = LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]);
						client.Player.AddGiftToken(num5);
					}
					if (num6 != 0)
					{
						num2 += num6;
						arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]);
						client.Player.AddMedal(num6);
					}
					if (num7 != 0)
					{
						num2 += num7;
						arg = LanguageMgr.GetTranslation("OpenUpArkHandler.Exp", new object[0]);
						client.Player.AddGP(num7);
					}
					foreach (ItemInfo current in list2)
					{
						stringBuilder.Append(current.Template.Name + "x" + current.Count.ToString() + ",");
						if (current.IsTips)
						{
							client.Player.SendItemNotice(current, 3, 3);
						}
						str = current.Template.Name;
						if (!client.Player.AddTemplate(current, current.Template.BagType, current.Count, eItemNotice.OpenTypeView, eItemNotice.GoodsTipTypeView))
						{
							list.Add(current);
						}
					}
				}
				if (num2 > 0)
				{
					stringBuilder2.Append(num2 + arg);
				}
				if (list.Count > 0)
				{
					client.Player.SendItemsToMail(list, LanguageMgr.GetTranslation("OpenUpArkHandler.Content1", new object[0]) + str + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2", new object[0]), LanguageMgr.GetTranslation("OpenUpArkHandler.Title", new object[0]) + str + "]", eMailType.Common);
					text = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail", new object[0]);
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					string[] array = stringBuilder.ToString().Split(new char[]
					{
						','
					});
					for (int j = 0; j < array.Length; j++)
					{
						int num8 = 1;
						for (int k = j + 1; k < array.Length; k++)
						{
							if (array[j].Contains(array[k]) && array[k].Length == array[j].Length)
							{
								num8++;
								array[k] = k.ToString();
							}
						}
						if (num8 > 1)
						{
							array[j] = array[j].Remove(array[j].Length - 1, 1);
							array[j] += num8.ToString();
						}
						if (array[j] != j.ToString())
						{
							array[j] += ",";
							stringBuilder2.Append(array[j]);
						}
					}
				}
				stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
				stringBuilder2.Append(".");
				client.Out.SendMessage(eMessageType.Normal, text + stringBuilder2.ToString());
				if (!string.IsNullOrEmpty(text))
				{
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			return 1;
		}
	}
}
