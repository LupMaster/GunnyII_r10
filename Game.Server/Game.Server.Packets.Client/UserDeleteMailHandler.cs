using Bussiness;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(112, "删除邮件")]
	public class UserDeleteMailHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(112, client.Player.PlayerCharacter.ID);
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			int num = packet.ReadInt();
			gSPacketIn.WriteInt(num);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				int playerID;
				if (playerBussiness.DeleteMail(client.Player.PlayerCharacter.ID, num, out playerID))
				{
					client.Out.SendMailResponse(playerID, eMailRespose.Receiver);
					gSPacketIn.WriteBoolean(true);
				}
				else
				{
					gSPacketIn.WriteBoolean(false);
				}
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
