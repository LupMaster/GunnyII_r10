using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(171, "场景用户离开")]
	public class UseReworkNameHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte bageType = packet.ReadByte();
			int slot = packet.ReadInt();
			string newNickName = packet.ReadString().Trim();
			string text = "";
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			ItemInfo itemAt = inventory.GetItemAt(slot);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (playerBussiness.RenameNick(client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.NickName, newNickName))
				{
					inventory.RemoveCountFromStack(itemAt, 1);
				}
				else
				{
					text = "Thay đổi Nickname thất bại.";
				}
			}
			if (text != "")
			{
				client.Player.SendMessage(text);
			}
			return 0;
		}
	}
}
