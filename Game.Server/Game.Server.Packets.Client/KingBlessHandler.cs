using System;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler(142, "场景用户离开")]
    public class KingBlessHandler : IPacketHandler
    {
        private string message;

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            var b = packet.ReadByte();
            Console.WriteLine("KingBlessHandler: " + b);
            int money1 = 475;
            int money2 = 1425;
            int money3 = 2500;
            GSPacketIn gSPacketIn = new GSPacketIn(142);
           
            switch (b)
            {
                case 1:
                    
                    if (money1 <= client.Player.PlayerCharacter.Money)
                    {
                        message = "Tiếp phí thàng công!";
                        client.Player.RemoveMoney(money1);
                        client.Out.SendMessage(eMessageType.Normal, message);
                    }
                    else
                    {
                        message = "Tiếp phí lỗi!"; 
                        return 0;
                    }
                    break;
                case 2:
                    if (money2 <= client.Player.PlayerCharacter.Money)
                    {
                        message = "Tiếp phí thàng công!";
                        client.Player.RemoveMoney(money2);
                        client.Out.SendMessage(eMessageType.Normal, message);
                    }
                    else
                    {
                        message = "Tiếp phí lỗi!"; 
                        return 0;
                    }
                    break;

                case 3:

                    if (money3 <= client.Player.PlayerCharacter.Money)
                    {
                        message = "Tiếp phí thàng công!";
                        client.Player.RemoveMoney(money3);
                        client.Out.SendMessage(eMessageType.Normal, message);
                    }
                    else
                    {
                        message = "Tiếp phí lỗi!"; 
                        return 0;
                    }
                    break;

            }

            return 0;
        }
    }
}
