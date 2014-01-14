using Game.Base.Packets;
using Game.Server.Quests;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(179, "任务完成")]
	public class QuestFinishHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int selectedItem = packet.ReadInt();
			BaseQuest baseQuest = client.Player.QuestInventory.FindQuest(num);
			int num2 = 0;
			if (baseQuest != null)
			{
				num2 = (client.Player.QuestInventory.Finish(baseQuest, selectedItem) ? 1 : 0);
			}
			if (num2 == 1)
			{
				packet.WriteInt(num);
				client.Out.SendTCP(packet);
			}
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemAt = client.Player.MainBag.GetItemAt(i);
				if (itemAt != null)
				{
					client.Player.OnItemStrengthen(itemAt.Template.CategoryID, itemAt.StrengthenLevel);
				}
			}
			return num2;
		}
	}
}
