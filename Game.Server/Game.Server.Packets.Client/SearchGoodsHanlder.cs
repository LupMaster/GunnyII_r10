using Game.Base.Packets;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(98, "场景用户离开")]
	public class SearchGoodsHanlder : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
            Console.WriteLine("SearchGoodsHanlder: " + b);
			return 0;
		}
	}
}
