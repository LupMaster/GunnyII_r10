using System;
namespace Game.Server.Packets
{
	public enum DragonBoatPackageType
	{
		START_OR_CLOSE = 1,
		BUILD_DECORATE,
		REFRESH_BOAT_STATUS,
		EXCHANGE,
		REFRESH_RANK = 16,
		REFRESH_RANK_OTHER
	}
}
