using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(222, "TinhLuyen")]
    public class EquipRetrieveHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
            PlayerInventory arg_19_0 = client.Player.PropBag;
            int num = 0;
            bool isBinds = true;
            for (int i = 1; i < 5; i++)
            {
                ItemInfo itemAt = inventory.GetItemAt(i);
                if (itemAt != null)
                {
                    inventory.RemoveItemAt(i);
                }
                if (itemAt.IsBinds)
                {
                    isBinds = true;
                }
                num += itemAt.Template.Quality;
            }
            int[] array = new int[]
			{
				7015,
				7016,
				7017,
				7018,
                15043,
				15046,
				15047,
				15052,
				15053,
				15054,
				15055,
				15056,
				15058,
				15061,
				15070,
				15071,
                15073,
				15074,
				15076,
				15077,
				1851,
				1852,
				1854,
				1601,
				1587,
				1517,
				2408,
				2409,
                2311,
				2312,
				2220,
				3437,
				3438,
				3439,
				3418,
				3337,
				3306,
				3318,
				3194,
				4284,
                6285,
				70461,
				70491,
				70501,
				70541,
				70551,
				70581,
				70611,
				70751,
				8314,
				9222,
                9223,
				13601,
				13602,
				13603,
				13604,
				13605,
				13606,
				13607,
				13608,
                13609,
				13610,
				13611,
				13562,
				13576,
				13537,
				13529,
				13530,
				13496,
				13497,
				13498,
				13499,
                13500,
				13427,
				13399,
				13375,
				13362,
				13364,
				13365,
				13366,
				13349,
				13204,
                17002,
				11904,
				11903,
				11905,
				100100,
				11903,
				11904,
				11905,
                7024,
				7026,
				7027,
				7031,
				7032,
				7041,
				7048,
				7051,
				7069,
				5528,
				5529,
				5628,
                5629,
				5683,
				5571,
				11101,
				11102,
				11109,
				11110,
				11111,
				11112,
				11113,
				11905,
				7019,
				7021,
				7022,
				7023,
				7048,
				11041,
				15019,
				16015
			};
            Random random = new Random();
            int num2 = random.Next(0, array.Length - 1);
            int templateId = array[num2];
            ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 105);
            itemInfo.IsBinds = isBinds;
            itemInfo.BeginDate = DateTime.Now;
            if (itemInfo.Template.CategoryID != 11)
            {
                itemInfo.ValidDate = 7;
            }
            itemInfo.RemoveDate = DateTime.Now.AddDays(7.0);
            inventory.AddItemTo(itemInfo, 0);
            return 1;
        }
    }
}
