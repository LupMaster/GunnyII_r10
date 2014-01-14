using System;
namespace Game.Server.Packets
{
	public enum LittleGamePackageOut
	{
		WORLD_LIST = 1,
		START_LOAD = 2,
		GAME_START,
		ADD_SPRITE = 16,
		REMOVE_SPRITE,
		MOVE = 32,
		UPDATE_POS,
		ADD_OBJECT = 64,
		REMOVE_OBJECT,
		INVOKE_OBJECT,
		UPDATELIVINGSPROPERTY = 80,
		DoMovie,
		DoAction = 96,
		KICK_PLAYE = 18,
		PONG = 6,
		NET_DELAY,
		GETSCORE = 49,
		SETCLOCK = 5
	}
}
