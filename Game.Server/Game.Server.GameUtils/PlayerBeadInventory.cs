using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.GameUtils
{
	public class PlayerBeadInventory : PlayerInventory
	{
		private const int BAG_START = 32;
		public PlayerBeadInventory(GamePlayer player) : base(player, true, 179, 21, 32, false)
		{
		}
		public override void LoadFromDatabase()
		{
			base.BeginChanges();
			try
			{
				base.LoadFromDatabase();
				List<ItemInfo> list = new List<ItemInfo>();
				for (int i = 1; i < 32; i++)
				{
					ItemInfo itemInfo = this.m_items[i];
					if (this.m_items[i] != null && !this.m_items[i].IsValidItem())
					{
						int num = base.FindFirstEmptySlot(32);
						if (num >= 0)
						{
							this.MoveItem(itemInfo.Place, num, itemInfo.Count);
						}
						else
						{
							list.Add(itemInfo);
						}
					}
				}
				if (list.Count > 0)
				{
					this.m_player.SendItemsToMail(list, null, null, eMailType.ItemOverdue);
					this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			finally
			{
				base.CommitChanges();
			}
		}
		public override bool MoveItem(int fromSlot, int toSlot, int count)
		{
			return this.m_items[fromSlot] != null && base.MoveItem(fromSlot, toSlot, count);
		}
		public override void UpdateChangedPlaces()
		{
			int[] array = this.m_changedPlaces.ToArray();
			int[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				int slot = array2[i];
				if (this.IsEquipSlot(slot))
				{
					ItemInfo itemAt = this.GetItemAt(slot);
					if (itemAt == null)
					{
						break;
					}
					itemAt.IsBinds = true;
					if (!itemAt.IsUsed)
					{
						itemAt.BeginDate = DateTime.Now;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			base.UpdateChangedPlaces();
		}
		public bool IsEquipSlot(int slot)
		{
			return slot >= 0 && slot < 32;
		}
	}
}
