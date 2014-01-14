using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(166, "场景用户离开")]
	public class LittleGameHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			int iD = client.Player.PlayerCharacter.ID;
			GSPacketIn gSPacketIn = new GSPacketIn(166, iD);
			byte b2 = b;

			if (b2 == 2)
			{
                gSPacketIn.WriteInt(1);
                //gSPacketIn.WriteInt(2);
				//gSPacketIn.WriteInt(1);
				//gSPacketIn.WriteInt(1);
				//gSPacketIn.WriteString("bogu4,bogu5,bogu6,bogu7,bogu8");
				//gSPacketIn.WriteString("2001");
				client.Player.Out.SendTCP(gSPacketIn);
			}
			else
			{
				Console.WriteLine("//LittleGame_cmd: " + (LittleGamePackageOut)b);
			}
			return 0;
		}
	}
}
