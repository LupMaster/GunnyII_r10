using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(172, "场景用户离开")]
	public class SaveToDB : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			client.Player.MainBag.SaveToDatabase();
			client.Player.PropBag.SaveToDatabase();
			return 0;
		}
	}
}
