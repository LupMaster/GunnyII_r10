using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server;
using Game.Server.GameUtils;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
    [PacketHandler(121, "物品镶嵌")]
    public class BeadHandle : IPacketHandler
    {
        public static ThreadSafeRandom randomExp = new ThreadSafeRandom();

        static BeadHandle()
        {
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num1 = ((PacketIn)packet).ReadByte();
            PlayerInventory inventory1 = client.Player.GetInventory((eBageType)21);
            string message = "";
            if (client.Player.PlayerCharacter.get_HasBagPassword() && client.Player.PlayerCharacter.get_IsLocked())
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            else if (DateTime.Compare(client.Player.LastDrillUpTime.AddSeconds(2.0), DateTime.Now) > 0)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Quá nhiều thao tác!", new object[0]));
                return 1;
            }
            else
            {
                switch (num1)
                {
                    case (byte)1:
                        int num2 = ((PacketIn)packet).ReadInt();
                        int num3 = ((PacketIn)packet).ReadInt();
                        int needLv = 10;
                        if (num3 == -1)
                            num3 = inventory1.FindFirstEmptySlot();
                        if (num3 <= 12 && num3 >= 4 && !BeadHandle.canEquip(num3, client.Player.PlayerCharacter.get_Grade(), ref needLv))
                        {
                            client.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} mở", (object)needLv));
                            return 0;
                        }
                        else
                        {
                            ItemInfo itemAt1 = inventory1.GetItemAt(num2);
                            ItemInfo itemAt2 = inventory1.GetItemAt(num3);
                            if (itemAt1 == null)
                            {
                                client.Out.SendMessage(eMessageType.Normal, "Sảy ra lổi, chuyển kênh và thử lại!");
                                return 0;
                            }
                            else
                            {
                                if (num2 <= 18 && num2 >= 13)
                                {
                                    bool flag = false;
                                    int drillLevel = client.Player.GetDrillLevel(num2);
                                    if (!BeadHandle.JudgeLevel(itemAt1.get_Hole1(), drillLevel))
                                        flag = true;
                                    if (itemAt2 != null && !BeadHandle.JudgeLevel(itemAt2.get_Hole1(), drillLevel))
                                        flag = true;
                                    if (flag)
                                    {
                                        inventory1.UpdateChangedPlaces();
                                        client.Out.SendMessage(eMessageType.Normal, "Cấp châu báu và cấp lỗ không khớp.");
                                        return 0;
                                    }
                                }
                                if (num3 > 31 && itemAt2 != null)
                                {
                                    RuneTemplateInfo runeTemplateId = RuneMgr.FindRuneTemplateID(itemAt2.get_TemplateID());
                                    bool flag = true;
                                    if (num2 == 1 && !runeTemplateId.IsAttack())
                                        flag = false;
                                    if ((num2 == 2 || num2 == 3) && !runeTemplateId.IsDefend())
                                        flag = false;
                                    if (num2 > 3 && num2 < 30 && !runeTemplateId.IsProp())
                                        flag = false;
                                    if (!flag)
                                    {
                                        client.Out.SendMessage(eMessageType.Normal, "Trang bị không phù hợp.");
                                        return 0;
                                    }
                                }
                                if (!inventory1.MoveItem(num2, num3, 1))
                                    client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể di chuyển!");
                                client.Player.MainBag.UpdatePlayerProperties();
                                break;
                            }
                        }
                    case (byte)2:
                        List<int> places = new List<int>();
                        ItemInfo itemAt3 = inventory1.GetItemAt(31);
                        if (itemAt3 == null)
                            return 0;
                        itemAt3.get_Hole1();
                        int num4 = ((PacketIn)packet).ReadInt();
                        int num5 = RuneMgr.MaxLv();
                        for (int index = 0; index < num4; ++index)
                        {
                            int slot = ((PacketIn)packet).ReadInt();
                            ItemInfo itemAt1 = inventory1.GetItemAt(slot);
                            RuneTemplateInfo runeTemplateId1 = RuneMgr.FindRuneTemplateID(itemAt3.get_TemplateID());
                            if (itemAt1 == null)
                            {
                                inventory1.RemoveAllItem(places);
                                client.Player.SendMessage("Không tìm thấy châu báu hảy thử lại sau.");
                                return 0;
                            }
                            else if (itemAt1.get_Hole1() < itemAt3.get_Hole1() && !itemAt1.get_IsUsed())
                            {
                                int hole2_1 = itemAt1.get_Hole2();
                                int hole2_2 = itemAt3.get_Hole2();
                                int hole1 = itemAt3.get_Hole1();
                                int exp = hole2_1 + hole2_2;
                                places.Add(slot);
                                if (BeadHandle.CanUpLv(exp, hole1))
                                {
                                    ItemInfo itemInfo1 = itemAt3;
                                    int num6 = itemInfo1.get_Hole2() + hole2_1;
                                    itemInfo1.set_Hole2(num6);
                                    ItemInfo itemInfo2 = itemAt3;
                                    int num7 = itemInfo2.get_Hole1() + 1;
                                    itemInfo2.set_Hole1(num7);
                                }
                                else
                                {
                                    ItemInfo itemInfo = itemAt3;
                                    int num6 = itemInfo.get_Hole2() + hole2_1;
                                    itemInfo.set_Hole2(num6);
                                }
                                int nextTemplateId = runeTemplateId1.get_NextTemplateID();
                                RuneTemplateInfo runeTemplateId2 = RuneMgr.FindRuneTemplateID(nextTemplateId);
                                if (runeTemplateId2 != null && itemAt3.get_Hole1() == runeTemplateId2.get_BaseLevel())
                                {
                                    ItemInfo itemInfo = new ItemInfo(ItemMgr.FindItemTemplate(nextTemplateId));
                                    itemAt3.set_TemplateID(nextTemplateId);
                                    itemInfo.Copy(itemAt3);
                                    inventory1.RemoveItemAt(31);
                                    inventory1.AddItemTo(itemInfo, 31);
                                }
                                if (itemAt3.get_Hole1() == num5 + 1)
                                    break;
                            }
                        }
                        inventory1.UpdateItem(itemAt3);
                        inventory1.RemoveAllItem(places);
                        inventory1.SaveToDatabase();
                        break;
                    case (byte)3:
                        string[] strArray = ((string)GameProperties.OpenRunePackageMoney).Split(new char[1]
            {
              '|'
            });
                        int index1 = ((PacketIn)packet).ReadInt();
                        ((PacketIn)packet).ReadBoolean();
                        int rand = 0;
                        bool flag1 = true;
                        int num8 = Convert.ToInt32(strArray[index1]);
                        if (client.Player.PlayerCharacter.get_Money() < num8)
                        {
                            client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Xu không đủ.", new object[0]));
                            flag1 = false;
                        }
                        if (inventory1.FindFirstEmptySlot() == -1)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
                            flag1 = false;
                        }
                        if (flag1)
                        {
                            List<ItemInfo> list;
                            switch (index1)
                            {
                                case 1:
                                    list = RuneMgr.OpenPackageLv2();
                                    break;
                                case 2:
                                    list = RuneMgr.OpenPackageLv3();
                                    break;
                                case 3:
                                    list = RuneMgr.OpenPackageLv4();
                                    break;
                                default:
                                    list = RuneMgr.OpenPackageLv1();
                                    break;
                            }
                            if (list == null)
                            {
                                client.Player.SendMessage("Sảy ra lổi, chuyển kênh và thử lại.");
                                return 0;
                            }
                            else
                            {
                                int index2 = ThreadSafeRandom.NextStatic(list.Count);
                                ItemInfo itemInfo = list[index2];
                                itemInfo.set_Count(1);
                                inventory1.AddItem(itemInfo);
                                RuneTemplateInfo runeTemplateId = RuneMgr.FindRuneTemplateID(itemInfo.get_TemplateID());
                                client.Player.SendMessage(string.Format("Bạn nhận được {0} lv{1}.", (object)runeTemplateId.get_Name(), (object)runeTemplateId.get_BaseLevel()));
                                client.Player.RemoveMoney(num8);
                                rand = BeadHandle.NextBeadIndex(client, index1);
                                this.BeadIndexUpdate(client, index1);
                            }
                        }
                        client.Out.SendRuneOpenPackage(client.Player, rand);
                        break;
                    case (byte)4:
                        int slot1 = ((PacketIn)packet).ReadInt();
                        ItemInfo itemAt4 = inventory1.GetItemAt(slot1);
                        if (itemAt4 == null)
                        {
                            client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển kênh và thử lại.");
                            return 0;
                        }
                        else
                        {
                            if (itemAt4.get_IsUsed())
                                itemAt4.set_IsUsed(false);
                            else
                                itemAt4.set_IsUsed(true);
                            inventory1.UpdateItem(itemAt4);
                            break;
                        }
                    case (byte)5:
                        int index3 = ((PacketIn)packet).ReadInt();
                        int templateId = ((PacketIn)packet).ReadInt();
                        PlayerInventory inventory2 = client.Player.GetInventory((eBageType)1);
                        inventory2.GetItemByTemplateID(0, templateId);
                        if (inventory2.GetItemCount(templateId) <= 0)
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Mủi khoan không đủ!", new object[0]));
                        }
                        else
                        {
                            int num6 = BeadHandle.randomExp.Next(2, 6);
                            message = LanguageMgr.GetTranslation("OpenHoleHandler.GetExp", new object[1]
              {
                (object) num6
              });
                            UserDrillInfo drill = client.Player.UserDrills[index3];
                            UserDrillInfo userDrillInfo1 = drill;
                            int num7 = userDrillInfo1.get_HoleExp() + num6;
                            userDrillInfo1.set_HoleExp(num7);
                            if (drill.get_HoleExp() >= GameProperties.HoleLevelUpExp(0) && drill.get_HoleLv() == 0 || drill.get_HoleExp() >= GameProperties.HoleLevelUpExp(1) && drill.get_HoleLv() == 1 || (drill.get_HoleExp() >= GameProperties.HoleLevelUpExp(2) && drill.get_HoleLv() == 2 || drill.get_HoleExp() >= GameProperties.HoleLevelUpExp(3) && drill.get_HoleLv() == 3) || drill.get_HoleExp() >= GameProperties.HoleLevelUpExp(4) && drill.get_HoleLv() == 4)
                            {
                                UserDrillInfo userDrillInfo2 = drill;
                                int num9 = userDrillInfo2.get_HoleLv() + 1;
                                userDrillInfo2.set_HoleLv(num9);
                                drill.set_HoleExp(0);
                            }
                            client.Player.UpdateDrill(index3, drill);
                        }
                        if (message != "")
                            client.Out.SendMessage(eMessageType.Normal, message);
                        client.Player.Out.SendPlayerDrill(client.Player.PlayerCharacter.get_ID(), client.Player.UserDrills);
                        inventory2.RemoveTemplate(templateId, 1);
                        break;
                }
                return 0;
            }
        }

        private static bool JudgeLevel(int beadLv, int drillLv)
        {
            switch (drillLv)
            {
                case 1:
                    if (beadLv >= 1 && beadLv <= 4)
                        return true;
                    else
                        break;
                case 2:
                    if (beadLv >= 1 && beadLv <= 8)
                        return true;
                    else
                        break;
                case 3:
                    if (beadLv >= 1 && beadLv <= 12)
                        return true;
                    else
                        break;
                case 4:
                    if (beadLv >= 1 && beadLv <= 16)
                        return true;
                    else
                        break;
                case 5:
                    return true;
            }
            return false;
        }

        private static bool canEquip(int place, int grade, ref int needLv)
        {
            bool flag = true;
            switch (place)
            {
                case 6:
                    needLv = 15;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 7:
                    needLv = 18;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 8:
                    needLv = 21;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 9:
                    needLv = 24;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 10:
                    needLv = 27;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 11:
                    needLv = 30;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
                case 12:
                    needLv = 33;
                    if (grade < needLv)
                    {
                        flag = false;
                        break;
                    }
                    else
                        break;
            }
            return flag;
        }

        private static bool CanUpLv(int exp, int lv)
        {
            return exp >= GameProperties.RuneExp()[lv];
        }

        public static int NextBeadIndex(GameClient client, int index)
        {
            if (client.beadRequestBtn1 == 10 && index == 0)
                return 1;
            if (client.beadRequestBtn2 == 5 && index == 1)
                return 2;
            return client.beadRequestBtn3 == 5 && index == 2 ? 4 : 0;
        }

        public void BeadIndexUpdate(GameClient client, int index)
        {
            if (index == 0)
            {
                if (client.beadRequestBtn1 > 10)
                    client.beadRequestBtn1 = 0;
                ++client.beadRequestBtn1;
            }
            if (index == 1)
            {
                ++client.beadRequestBtn2;
                client.beadRequestBtn1 = 0;
            }
            if (index == 2)
            {
                ++client.beadRequestBtn3;
                client.beadRequestBtn2 = 0;
            }
            if (index != 3)
                return;
            client.beadRequestBtn3 = 0;
        }
    }
}
