using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(100, "场景用户离开")]
    public class DragonBoatHanler : IPacketHandler
    {
        //public int HandlePacket(GameClient client, GSPacketIn packet)
        //{
        //byte b = packet.ReadByte();
        //Console.WriteLine("//DragonBoatHanler: " + b);
        //return 0;
        /*public static const START_OR_CLOSE:int = 1;
         public static const BUILD_DECORATE:int = 2;
         public static const REFRESH_BOAT_STATUS:int = 3;
         public static const EXCHANGE:int = 4;
         public static const REFRESH_RANK:int = 16;
         public static const REFRESH_RANK_OTHER:int = 17;*/

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            switch (packet.ReadByte())
            {
                case 17:
                    this.updateOtherRank(client.Player, packet);
                    break;
                case 16:
                    this.updateSelfRank(client.Player, packet);
                    break;
            }
            return 0;
        }

        GSPacketIn gSPacketIn = new GSPacketIn(100);
        private void updateOtherRank(GamePlayer player, GSPacketIn packet)
        {
            gSPacketIn.WriteInt(6);//_loc_2:* = param1.readInt();
            gSPacketIn.WriteInt(7);//_loc_8.rank = param1.readInt();
            gSPacketIn.WriteInt(8);//_loc_8.score = param1.readInt();
            gSPacketIn.WriteString("MrPhuongPc");//_loc_8.name = param1.readUTF();
            gSPacketIn.WriteString("MrPhuongPc");//_loc_8.zone = param1.readUTF();
            gSPacketIn.WriteInt(9);
            gSPacketIn.WriteInt(10);
            player.Out.SendTCP(gSPacketIn);
            Console.WriteLine("//updateOtherRank: ");
        }
        private void updateSelfRank(GamePlayer player, GSPacketIn packet)
        {
            gSPacketIn.WriteInt(1);//var _loc_2:* = param1.readInt();
            gSPacketIn.WriteInt(2);//_loc_8.rank = param1.readInt();
            gSPacketIn.WriteInt(3);//_loc_8.score = param1.readInt();
            gSPacketIn.WriteString("MrPhuongPc");//_loc_8.name = param1.readUTF();
            gSPacketIn.WriteInt(4);
            gSPacketIn.WriteInt(5);
            player.Out.SendTCP(gSPacketIn);
            Console.WriteLine("//updateSelfRank: ");
        }

    }
}