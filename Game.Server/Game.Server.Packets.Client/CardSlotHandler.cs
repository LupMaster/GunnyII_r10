using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(170, "场景用户离开")]
	public class CardSlotHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			string text = "";
			List<UsersCardInfo> cards = client.Player.CardBag.GetCards(0, 5);
			switch (num)
			{
			case 0:
				if (num3 > 0 && num3 <= client.Player.PlayerCharacter.CardSoul)
				{
					int type = cards[num2].Type;
					int gP = cards[num2].CardGP + num3;
					int level = CardMgr.GetLevel(gP, type);
					int num4 = CardMgr.GetGP(level, type) - cards[num2].CardGP;
					if (level == 40)
					{
						num3 = num4;
					}
					client.Player.CardBag.UpGraceSlot(num3, level, num2);
					client.Player.RemoveCardSoul(num3);
					client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, cards[num2]);
					client.Player.MainBag.UpdatePlayerProperties();
				}
				else
				{
					text = "Thẻ hổn không đủ!";
				}
				break;

			case 1:
				if (client.Player.PlayerCharacter.Money >= 300)
				{
					int num5 = 0;
					for (int i = 0; i < cards.Count; i++)
					{
						num5 += cards[i].CardGP;
					}
					client.Player.CardBag.ResetCardSoul();
					client.Player.AddCardSoul(num5);
					text = LanguageMgr.GetTranslation("UpdateSLOT.ResetComplete", new object[]
					{
						num5
					});
					client.Player.RemoveMoney(300);
					client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, cards);
					client.Player.MainBag.UpdatePlayerProperties();
				}
				else
				{
					text = "Xu không đủ!";
				}
				break;
			}
			if (text != "")
			{
				client.Out.SendMessage(eMessageType.Normal, text);
			}
			client.Player.CardBag.SaveToDatabase();
			return 0;
		}
	}
}
