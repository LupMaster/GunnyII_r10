using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(92, "场景用户离开")]
    public class OpenVipHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string text = packet.ReadString();
            int num = packet.ReadInt();
            int num1 = 1190;
            int num4 = 3570;
            int num6 = 5999;
            int num3 = 6999;
            client.Player.SendMessage("Aberto com sucesso!");
            int num2 = num;
            if (num6 != 30)
            {
                if (num6 != 90)
                {
                    if (num6 == 180)
                    {
                        num2 = num6;
                    }
                }
                else
                {
                    num2 = num4;
                }
            }
            else
            {
                num2 = num3;
            }
            GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
            if (num2 <= client.Player.PlayerCharacter.Money)
            {
                DateTime now = DateTime.Now;
                using (PlayerBussiness playerBussiness = new PlayerBussiness())
                {
                    playerBussiness.VIPRenewal(text, num, ref now);
                    if (clientByPlayerNickName == null)
                    {
                        client.Player.SendMessage("O jogador {" + clientByPlayerNickName + "} renovou com sucesso no teu VIP por {" + DateTime.Now + "} dia(s)");
                    }
                    else
                    {
                        if (client.Player.PlayerCharacter.NickName == text)
                        {
                            if (client.Player.PlayerCharacter.typeVIP == 0)
                            {
                                client.Player.OpenVIP(now);
                            }
                            else
                            {
                                client.Player.ContinousVIP(now);
                                client.Player.SendMessage("Renovação sucedida!");
                            }
                            client.Out.SendOpenVIP(client.Player.PlayerCharacter);
                        }
                        else
                        {
                            string message2;
                            if (clientByPlayerNickName.PlayerCharacter.typeVIP == 0)
                            {
                                clientByPlayerNickName.OpenVIP(now);
                                client.Player.SendMessage("O jogador {" + clientByPlayerNickName + "} iniciou com sucesso no teu VIP por {" + DateTime.Now + "} dia(s)");
                            }
                            else
                            {
                                clientByPlayerNickName.ContinousVIP(now);
                            }
                            clientByPlayerNickName.Out.SendOpenVIP(clientByPlayerNickName.PlayerCharacter);
                        }
                    }
                    client.Player.AddExpVip(num2);
                    client.Player.RemoveMoney(num2);
                    return 0;
                }
            }
            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Cupons insuficientes.", new object[0]));
            return 0;
        }
    }
}
