using System;
namespace Game.Server.Packets
{
	public enum eMessageType
	{
		Normal,
		ERROR,
		ChatNormal,
		ChatERROR,
		ALERT,
		DailyAward,
		Defence,
		GM_NOTICE = 0,
		BIGBUGLE_NOTICE,
		SYS_TIP_NOTICE,
		SYS_NOTICE,
		CONSORTIA_NOTICE = 8,
		CROSS_NOTICE = 12
	}
}
