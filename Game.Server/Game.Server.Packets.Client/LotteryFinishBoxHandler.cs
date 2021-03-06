using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(28, "打开物品")]
	public class LotteryFinishBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			client.SendTCP(pkg);
			client.Player.ClearCaddyBag();
			client.Lottery = -1;
			return 1;
		}
	}
}
