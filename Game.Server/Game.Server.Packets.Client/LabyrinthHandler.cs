using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(131, "场景用户离开")]
	public class LabyrinthHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			UserLabyrinthInfo userLabyrinthInfo = client.Player.LoadLabyrinth();
			int iD = client.Player.PlayerCharacter.ID;
			int num2 = num;
			switch (num2)
			{
			case 1:
				{
					bool flag = packet.ReadBoolean();
					if (userLabyrinthInfo == null)
					{
						return 0;
					}
					ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, 11916);
					if (itemByTemplateID == null)
					{
						userLabyrinthInfo.isDoubleAward = false;
					}
					if (flag && itemByTemplateID != null && !userLabyrinthInfo.isDoubleAward && client.Player.RemoveTemplate(11916, 1))
					{
						userLabyrinthInfo.isDoubleAward = true;
					}
					client.Player.Out.SendLabyrinthUpdataInfo(iD, userLabyrinthInfo);
					client.Player.Labyrinth.isDoubleAward = userLabyrinthInfo.isDoubleAward;
					break;
				}

			case 2:
				if (userLabyrinthInfo.isValidDate())
				{
					userLabyrinthInfo.completeChallenge = true;
					userLabyrinthInfo.accumulateExp = 0;
					userLabyrinthInfo.isInGame = false;
					userLabyrinthInfo.currentFloor = 1;
					userLabyrinthInfo.LastDate = DateTime.Now;
				}
				client.Player.Out.SendLabyrinthUpdataInfo(iD, userLabyrinthInfo);
				client.Player.Labyrinth = userLabyrinthInfo;
				break;

			default:
				if (num2 != 6)
				{
					if (num2 != 9)
					{
						Console.WriteLine("???labyrinth_cmd: " + (LabyrinthPackageType)num);
					}
					else
					{
						bool flag2 = packet.ReadBoolean();
						packet.ReadBoolean();
						if (!flag2)
						{
							client.Player.SendMessage("Tái lập thành công!");
						}
						client.Player.Labyrinth.tryAgainComplete = false;
					}
				}
				else
				{
					client.Player.SendMessage("Hiện tại không thể tái lập!");
				}
				break;
			}
			return 0;
		}
	}
}
