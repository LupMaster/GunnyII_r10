using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(68, "添加好友")]
	public class PetHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			string text = "Xu không đủ!";
			PetInventory petBag = client.Player.PetBag;
			switch (b)
			{
			case 1:
				this.UpdatePetHandle(client, packet.ReadInt());
				return 0;

			case 2:
				{
					int place = packet.ReadInt();
					int bagType = packet.ReadInt();
					int iD = client.Player.PlayerCharacter.ID;
					int num = petBag.FindFirstEmptySlot();
					if (num == -1)
					{
						client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
						return 0;
					}
					ItemInfo itemAt = client.Player.GetItemAt((eBageType)bagType, place);
					PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(itemAt.Template.Property5);
					if (petTemplateInfo == null)
					{
						client.Player.SendMessage("Xảy ra lổi liên hệ admin.");
						return 0;
					}
					UsersPetinfo usersPetinfo = PetMgr.CreatePet(petTemplateInfo, iD, num);
					usersPetinfo.IsExit = true;
					petBag.AddPetTo(usersPetinfo, num);
					client.Player.MainBag.RemoveCountFromStack(itemAt, 1);
					if (petTemplateInfo.StarLevel > 3)
					{
                        string msg = string.Format("Chúc Mừng [{0}] Nhận Đuợc {1} {2} sao.", client.Player.PlayerCharacter.NickName, petTemplateInfo.Name, petTemplateInfo.StarLevel);
						GSPacketIn packet2 = WorldMgr.SendSysNotice(msg);
						GameServer.Instance.LoginServer.SendPacket(packet2);
					}
					petBag.SaveToDatabase(false);
					return 0;
				}

			case 4:
				{
					int place = packet.ReadInt();
					int bagType = packet.ReadInt();
					int num2 = packet.ReadInt();
					bool flag = false;
					ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)bagType, place);
					if (itemAt2 == null)
					{
						client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển kênh và thử lại.");
						return 0;
					}
					int num3 = Convert.ToInt32(PetMgr.FindConfig("MaxHunger").Value);
					int num4 = Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);
					UsersPetinfo petAt = petBag.GetPetAt(num2);
					int num5 = itemAt2.Count;
					int property = itemAt2.Template.Property2;
					int property2 = itemAt2.Template.Property3;
					int num6 = num5 * property2;
					int num7 = num6 + petAt.Hunger;
					int num8 = num5 * property;
					text = "";
					if (itemAt2.TemplateID == 334100)
					{
						num8 = itemAt2.DefendCompose;
					}
					if (petAt.Level > 11 && itemAt2.TemplateID == 334100)
					{
						text = "Pet level 10 trở xuống mới dùng đuợc " + itemAt2.Template.Name;
					}
					else
					{
						if (petAt.Level < num4)
						{
							num8 += petAt.GP;
							int level = petAt.Level;
							int level2 = PetMgr.GetLevel(num8);
							int gP = PetMgr.GetGP(level2 + 1);
							int gP2 = PetMgr.GetGP(num4);
							int num9 = num8;
							if (num8 > gP2)
							{
								num8 -= gP2;
								if (num8 >= property && property != 0)
								{
									num5 = num8 / property;
								}
							}
							petAt.GP = ((num9 >= gP2) ? gP2 : num9);
							petAt.Level = level2;
							petAt.MaxGP = ((gP == 0) ? gP2 : gP);
							petAt.Hunger = ((num7 > num3) ? num3 : num7);
							flag = petBag.UpGracePet(petAt, num2, true, level, level2, ref text);
							if (itemAt2.TemplateID == 334100)
							{
								client.Player.StoreBag.RemoveItem(itemAt2);
							}
							else
							{
								client.Player.StoreBag.RemoveCountFromStack(itemAt2, num5);
								client.Player.OnUsingItem(itemAt2.TemplateID);
							}
						}
						else
						{
							int hunger = petAt.Hunger;
							int num10 = num3 - hunger;
							if (num7 >= num3 && num7 >= property2)
							{
								num5 = num7 / property2;
							}
							num7 = hunger + num10;
							petAt.Hunger = num7;
							if (hunger < num3)
							{
								client.Player.StoreBag.RemoveCountFromStack(itemAt2, num5);
								flag = petBag.UpGracePet(petAt, num2, false, 0, 0, ref text);
								text = "Ðộ vui vẻ tang thêm " + num10;
							}
							else
							{
								text = "Ðộ vui vui đã đạt mức tối da";
							}
						}
					}
					if (flag)
					{
						petBag.SaveToDatabase(false);
					}
					if (!string.IsNullOrEmpty(text))
					{
						client.Player.SendMessage(text);
						return 0;
					}
					return 0;
				}

			case 5:
				{
					bool refreshBtn = packet.ReadBoolean();
					this.RefreshPetHandle(client, refreshBtn, text);
					return 0;
				}

			case 6:
				{
					int num2 = packet.ReadInt();
					int num11 = petBag.FindFirstEmptySlot();
					if (num11 == -1)
					{
						client.Out.SendRefreshPet(client.Player, petBag.GetAdoptPet(), null, false);
						client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
						return 0;
					}
					UsersPetinfo adoptPetAt = petBag.GetAdoptPetAt(num2);
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						if (adoptPetAt.ID > 0)
						{
							playerBussiness.RemoveUserAdoptPet(adoptPetAt.ID);
							adoptPetAt.ID = 0;
						}
					}
					petBag.RemoveAdoptPet(adoptPetAt);
					if (petBag.AddPetTo(adoptPetAt, num11))
					{
						PetTemplateInfo petTemplateInfo2 = PetMgr.FindPetTemplate(adoptPetAt.TemplateID);
						if (petTemplateInfo2.StarLevel > 3)
						{
                            string msg2 = string.Format("Chúc Mừng [{0}] Nhận Đuợc {1} {2} sao.", client.Player.PlayerCharacter.NickName, petTemplateInfo2.Name, petTemplateInfo2.StarLevel);
							GSPacketIn packet3 = WorldMgr.SendSysNotice(msg2);
							GameServer.Instance.LoginServer.SendPacket(packet3);
						}
						client.Player.OnAdoptPetEvent();
					}
					petBag.SaveToDatabase(false);
					return 0;
				}

			case 7:
				{
					int num2 = packet.ReadInt();
					int killId = packet.ReadInt();
					int killindex = packet.ReadInt();
					if (!petBag.EquipSkillPet(num2, killId, killindex))
					{
						client.Player.SendMessage("Skill này đã trang bị!");
						return 0;
					}
					return 0;
				}

			case 8:
				{
					int num2 = packet.ReadInt();
					UsersPetinfo petAt2 = petBag.GetPetAt(num2);
					if (petBag.RemovePet(petAt2))
					{
						using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
						{
							playerBussiness2.UpdateUserAdoptPet(petAt2.ID);
						}
					}
					client.Player.SendMessage("Thả pet thành công!");
					petBag.SaveToDatabase(false);
					return 0;
				}

			case 9:
				{
					int num2 = packet.ReadInt();
					string name = packet.ReadString();
					int num12 = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
					if (client.Player.PlayerCharacter.Money >= num12)
					{
						if (petBag.RenamePet(num2, name))
						{
							text = "Đổi tên thành công!";
						}
						client.Player.RemoveMoney(num12);
					}
					client.Player.SendMessage(text);
					return 0;
				}

			case 17:
				{
					int num2 = packet.ReadInt();
					bool isEquip = packet.ReadBoolean();
					if (petBag.EquipPet(num2, isEquip))
					{
						client.Player.MainBag.UpdatePlayerProperties();
						return 0;
					}
					return 0;
				}

			case 18:
				{
					int num2 = packet.ReadInt();
					this.RevertPetHandle(client, num2, text);
					petBag.SaveToDatabase(false);
					return 0;
				}

			case 19:
				{
					bool flag2 = packet.ReadBoolean();
					if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
						return 0;
					}
					UserFarmInfo currentFarm = client.Player.Farm.CurrentFarm;
					int buyExpRemainNum = currentFarm.buyExpRemainNum;
					PetExpItemPriceInfo petExpItemPriceInfo = PetMgr.FindPetExpItemPrice(this.RealMoney(buyExpRemainNum));
					if (petExpItemPriceInfo == null || buyExpRemainNum < 1)
					{
						return 0;
					}
					bool flag3 = false;
					int money = petExpItemPriceInfo.Money;
					if (flag2)
					{
						if (money <= client.Player.PlayerCharacter.Gold)
						{
							client.Player.AddExpVip(money);
							client.Player.RemoveMoney(money);
							flag3 = true;
						}
					}
					else
					{
						if (money <= client.Player.PlayerCharacter.GiftToken)
						{
							client.Player.RemoveGiftToken(money);
							flag3 = true;
						}
					}
					if (!flag3)
					{
						client.Player.SendMessage("Tiền không đủ. Nạp thêm.");
						return 0;
					}
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334102);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, petExpItemPriceInfo.ItemCount, 102);
					itemInfo.IsBinds = true;
					client.Player.AddTemplate(itemInfo, itemInfo.Template.BagType, petExpItemPriceInfo.ItemCount, eItemNotice.NoneTypeView, eItemNotice.GoodsTipTypeView);
					currentFarm.buyExpRemainNum--;
					GSPacketIn gSPacketIn = new GSPacketIn(68);
					gSPacketIn.WriteByte(19);
					gSPacketIn.WriteInt(currentFarm.buyExpRemainNum);
					client.SendTCP(gSPacketIn);
					client.Player.Farm.UpdateFarm(currentFarm);
					return 0;
				}

			case 20:
				{
					int bagType2 = packet.ReadInt();
					int num2 = packet.ReadInt();
					int num13 = packet.ReadInt();
					if (petBag.GetPetAt(num13) == null)
					{
						return 0;
					}
					ItemInfo itemAt3 = client.Player.GetItemAt((eBageType)bagType2, num2);
					if (itemAt3 == null || !itemAt3.IsEquipPet())
					{
						return 0;
					}
					if (petBag.MoveEqFromBag(num13, itemAt3.eqType(), itemAt3))
					{
						client.Player.RemoveAt((eBageType)bagType2, num2);
						client.Player.MainBag.UpdatePlayerProperties();
						return 0;
					}
					client.Player.SendMessage("Cấp không đủ, trang bị thất bại!");
					return 0;
				}

			case 21:
				{
					int num14 = packet.ReadInt();
					int num2 = packet.ReadInt();
					UsersPetinfo petAt3 = petBag.GetPetAt(num14);
					if (petAt3 == null)
					{
						return 0;
					}
					if (num2 > petAt3.EquipList.Count)
					{
						return 0;
					}
					PetEquipDataInfo petEquipDataInfo = petBag.GetPetAt(num14).EquipList[num2];
					petBag.MoveEqToBag(petEquipDataInfo);
					petEquipDataInfo.eqTemplateID = 0;
					petEquipDataInfo.ValidDate = 0;
					petEquipDataInfo = petEquipDataInfo.addTempalte(null);
					petBag.UpdateQPet(num14, num2, petEquipDataInfo);
					client.Player.MainBag.UpdatePlayerProperties();
					return 0;
				}
			}
			Console.WriteLine("pet_cmd: " + (ePetType)b);
			return 0;
		}
		private void RevertPetHandle(GameClient client, int place, string msg)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("RecycleCost").Value);
			if (client.Player.PlayerCharacter.Money >= num)
			{
				UsersPetinfo petAt = client.Player.PetBag.GetPetAt(place);
				if (petAt == null)
				{
					return;
				}
				UsersPetinfo usersPetinfo = new UsersPetinfo();
				int iD = petAt.ID;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					usersPetinfo = playerBussiness.GetAdoptPetSingle(iD);
				}
				if (usersPetinfo == null)
				{
					client.Player.SendMessage("Phục hồi thất bại!");
					return;
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334100);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				itemInfo.IsBinds = true;
				itemInfo.DefendCompose = petAt.GP;
				itemInfo.AgilityCompose = petAt.MaxGP;
				if (!client.Player.PropBag.AddTemplate(itemInfo, 1))
				{
					client.Player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				petAt.Blood = usersPetinfo.Blood;
				petAt.Attack = usersPetinfo.Attack;
				petAt.Defence = usersPetinfo.Defence;
				petAt.Agility = usersPetinfo.Agility;
				petAt.Luck = usersPetinfo.Luck;
				int arg_171_0 = client.Player.PlayerCharacter.ID;
				int templateID = usersPetinfo.TemplateID;
				petAt.TemplateID = templateID;
				petAt.Skill = usersPetinfo.Skill;
				petAt.SkillEquip = usersPetinfo.SkillEquip;
				petAt.GP = 0;
				petAt.Level = 1;
				petAt.MaxGP = 55;
				Dictionary<int, PetEquipDataInfo> equip = petAt.GetEquip();
				client.Player.PetBag.MoveEqAllToBag(equip);
				petAt.EquipList = client.Player.PetBag.RemoveEq(equip);
				bool flag = client.Player.PetBag.UpGracePet(petAt, place, false, 0, 0, ref msg);
				if (flag)
				{
					client.Player.SendMessage("Phục hồi thành công!");
					client.Player.RemoveMoney(num);
					return;
				}
			}
			else
			{
				client.Player.SendMessage(msg);
			}
		}
		private void RefreshPetHandle(GameClient client, bool refreshBtn, string msg)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
			int templateId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (refreshBtn)
			{
				if (num > client.Player.PlayerCharacter.Money)
				{
					client.Player.SendMessage(msg);
					return;
				}
				if (itemByTemplateID != null)
				{
					client.Player.PropBag.RemoveTemplate(templateId, 1);
				}
				else
				{
					client.Player.RemoveMoney(num);
					client.Player.AddPetScore(num / 10);
				}
				List<UsersPetinfo> list = PetMgr.CreateAdoptList(client.Player.PlayerCharacter.ID);
				client.Player.PetBag.ClearAdoptPets();
				foreach (UsersPetinfo current in list)
				{
					client.Player.PetBag.AddAdoptPetTo(current, current.Place);
				}
			}
			client.Player.Out.SendRefreshPet(client.Player, client.Player.PetBag.GetAdoptPet(), null, refreshBtn);
		}
		private void UpdatePetHandle(GameClient client, int ID)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(ID);
			PlayerInfo playerInfo;
			UsersPetinfo[] array;
			if (playerById != null)
			{
				playerInfo = playerById.PlayerCharacter;
				array = playerById.PetBag.GetPets();
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerInfo = playerBussiness.GetUserSingleByUserID(ID);
					array = playerBussiness.GetUserPetSingles(ID);
					PetEquipDataInfo[] eqPetSingles = playerBussiness.GetEqPetSingles(ID);
					for (int i = 0; i < array.Length; i++)
					{
						array[i].EquipList = this.GetPetEquip(array[i].ID, eqPetSingles);
					}
				}
			}
			if (array != null && playerInfo != null)
			{
				client.Out.SendPetInfo(playerInfo, array);
			}
		}
		private Dictionary<int, PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
		{
			Dictionary<int, PetEquipDataInfo> dictionary = new Dictionary<int, PetEquipDataInfo>();
			for (int i = 0; i < eqs.Length; i++)
			{
				PetEquipDataInfo petEquipDataInfo = eqs[i];
				if (petID == petEquipDataInfo.PetID)
				{
					dictionary.Add(petEquipDataInfo.eqType, petEquipDataInfo);
				}
			}
			return dictionary;
		}
		private int RealMoney(int timebuy)
		{
			switch (timebuy)
			{
			case 1:
				return 20;

			case 2:
				return 19;

			case 3:
				return 18;

			case 4:
				return 17;

			case 5:
				return 16;

			case 6:
				return 15;

			case 7:
				return 14;

			case 8:
				return 13;

			case 9:
				return 12;

			case 10:
				return 11;

			case 11:
				return 10;

			case 12:
				return 9;

			case 13:
				return 8;

			case 14:
				return 7;

			case 15:
				return 6;

			case 16:
				return 5;

			case 17:
				return 4;

			case 18:
				return 3;

			case 19:
				return 2;

			default:
				return 1;
			}
		}
	}
}
