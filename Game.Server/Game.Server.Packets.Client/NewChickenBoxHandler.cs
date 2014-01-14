using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.NEWCHICKENBOX_SYS, "")]
    public class NewChickenBoxHandler : IPacketHandler
    {
        GSPacketIn gSPacketIn = new GSPacketIn(87);
        private void EnterNewChickenBox(GamePlayer player, GSPacketIn packet)
        {
            gSPacketIn.WriteInt(7024);//_loc_3.TemplateID = _loc_2.readInt();
            gSPacketIn.WriteInt(12);//_loc_3.StrengthenLevel = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.Count = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.ValidDate = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.AttackCompose = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.DefendCompose = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.AgilityCompose = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.LuckCompose = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//_loc_3.Position = _loc_2.readInt();
            gSPacketIn.WriteBoolean(true);//_loc_3.IsSelected = _loc_2.readBoolean();
            gSPacketIn.WriteBoolean(true);//_loc_3.IsSeeded = _loc_2.readBoolean();
            gSPacketIn.WriteBoolean(true);//_loc_3.IsBinds = _loc_2.readBoolean();
            player.Out.SendTCP(gSPacketIn);
        }

        private void OpenEye(GamePlayer player, GSPacketIn packet)
        {
            gSPacketIn.WriteInt(7024);
            gSPacketIn.WriteInt(12);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteInt(10);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteBoolean(true);
            Console.WriteLine("Run __  EnterDice");
            player.Out.SendTCP(gSPacketIn);
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            switch (packet.ReadByte())
            {
                case 0:
                    this.EnterNewChickenBox(client.Player, packet);
                    break;
            }

            return 0;
        }
    }
}
