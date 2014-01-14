using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server;
using Game.Server.Packets;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
    [PacketHandler(94, "游戏创建")]
    public class GameRoomHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num1 = packet.ReadInt();
            switch (num1)
            {
                case 0:
                    byte num2 = packet.ReadByte();
                    byte timeType = packet.ReadByte();
                    string name = packet.ReadString();
                    string password1 = packet.ReadString();
                    if ((int)num2 == 15)
                    {
                        if (!client.Player.Labyrinth.completeChallenge)
                        {
                            client.Player.SendMessage("Bạn đã hết lượt khiêu chến hôm nay!");
                            return 0;
                        }
                        else
                            client.Player.Labyrinth.isInGame = true;
                    }
                    RoomMgr.CreateRoom(client.Player, name, password1, (eRoomType)num2, timeType);
                    break;
                case 1:
                    packet.ReadBoolean();
                    int type = packet.ReadInt();
                    int num3 = packet.ReadInt();
                    int roomId = -1;
                    string pwd = (string)null;
                    if (num3 == -1)
                    {
                        roomId = packet.ReadInt();
                        pwd = packet.ReadString();
                    }
                    if (type == 1)
                        type = 0;
                    else if (type == 2)
                        type = 4;
                    RoomMgr.EnterRoom(client.Player, roomId, pwd, type);
                    break;
                case 2:
                    if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host && !client.Player.CurrentRoom.IsPlaying)
                    {
                        int mapId = packet.ReadInt();
                        eRoomType roomType = (eRoomType)packet.ReadByte();
                        bool isOpenBoss = packet.ReadBoolean();
                        string Pic = "";
                        if (isOpenBoss)
                            Pic = packet.ReadString();
                        string password2 = packet.ReadString();
                        string roomname = packet.ReadString();
                        byte timeMode = packet.ReadByte();
                        byte num4 = packet.ReadByte();
                        int levelLimits = packet.ReadInt();
                        bool isCrosszone = packet.ReadBoolean();
                        packet.ReadInt();
                        int currentFloor = 0;
                        if (mapId == 0 && roomType == eRoomType.Lanbyrinth)
                        {
                            mapId = 401;
                            currentFloor = client.Player.Labyrinth.currentFloor;
                        }
                        RoomMgr.UpdateRoomGameType(client.Player.CurrentRoom, roomType, timeMode, (eHardLevel)num4, levelLimits, mapId, password2, roomname, isCrosszone, isOpenBoss, Pic, currentFloor);
                        break;
                    }
                    else
                        break;
                case 3:
                    if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
                    {
                        RoomMgr.KickPlayer(client.Player.CurrentRoom, packet.ReadByte());
                        break;
                    }
                    else
                        break;
                case 5:
                    if (client.Player.CurrentRoom != null)
                    {
                        RoomMgr.ExitRoom(client.Player.CurrentRoom, client.Player);
                        break;
                    }
                    else
                        break;
                case 6:
                    if (client.Player.CurrentRoom == null || client.Player.CurrentRoom.RoomType == eRoomType.Match)
                        return 0;
                    RoomMgr.SwitchTeam(client.Player);
                    break;
                case 7:
                    BaseRoom currentRoom = client.Player.CurrentRoom;
                    if (currentRoom != null && currentRoom.Host == client.Player)
                    {
                        if (client.Player.MainWeapon == null)
                        {
                            client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                            return 0;
                        }
                        else if (currentRoom.RoomType == eRoomType.Dungeon && !client.Player.IsPvePermission(currentRoom.MapId, currentRoom.HardLevel))
                        {
                            client.Player.SendMessage("Do not PvePermission enter this map!");
                            return 0;
                        }
                        else
                        {
                            RoomMgr.StartGame(client.Player.CurrentRoom);
                            break;
                        }
                    }
                    else
                        break;
                case 9:
                    packet.ReadInt();
                    int num5 = packet.ReadInt();
                    int num6 = 1011;
                    if (num5 == -2)
                    {
                        packet.ReadInt();
                        num6 = packet.ReadInt();
                    }
                    BaseRoom[] rooms = RoomMgr.Rooms;
                    List<BaseRoom> room = new List<BaseRoom>();
                    for (int index = 0; index < rooms.Length; ++index)
                    {
                        if (!rooms[index].IsEmpty)
                        {
                            switch (num5)
                            {
                                case 3:
                                    if (rooms[index].RoomType == eRoomType.Match || rooms[index].RoomType == eRoomType.Freedom)
                                    {
                                        room.Add(rooms[index]);
                                        continue;
                                    }
                                    else
                                        continue;
                                case 4:
                                    if (rooms[index].RoomType == eRoomType.Match)
                                    {
                                        room.Add(rooms[index]);
                                        continue;
                                    }
                                    else
                                        continue;
                                case 5:
                                    if (rooms[index].RoomType == eRoomType.Freedom)
                                    {
                                        room.Add(rooms[index]);
                                        continue;
                                    }
                                    else
                                        continue;
                                default:
                                    if (rooms[index].RoomType == eRoomType.Dungeon)
                                    {
                                        switch (num6)
                                        {
                                            case 1007:
                                                if (rooms[index].HardLevel == eHardLevel.Simple)
                                                {
                                                    room.Add(rooms[index]);
                                                    continue;
                                                }
                                                else
                                                    continue;
                                            case 1008:
                                                if (rooms[index].HardLevel == eHardLevel.Normal)
                                                {
                                                    room.Add(rooms[index]);
                                                    continue;
                                                }
                                                else
                                                    continue;
                                            case 1009:
                                                if (rooms[index].HardLevel == eHardLevel.Hard)
                                                {
                                                    room.Add(rooms[index]);
                                                    continue;
                                                }
                                                else
                                                    continue;
                                            case 1010:
                                                if (rooms[index].HardLevel == eHardLevel.Terror)
                                                {
                                                    room.Add(rooms[index]);
                                                    continue;
                                                }
                                                else
                                                    continue;
                                            default:
                                                room.Add(rooms[index]);
                                                continue;
                                        }
                                    }
                                    else
                                        continue;
                            }
                        }
                    }
                    if (room.Count > 0)
                    {
                        client.Out.SendUpdateRoomList(room);
                        break;
                    }
                    else
                        break;
                case 10:
                    if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
                    {
                        byte num4 = packet.ReadByte();
                        int place = packet.ReadInt();
                        bool isOpened = packet.ReadBoolean();
                        int placeView = packet.ReadInt();
                        RoomMgr.UpdateRoomPos(client.Player.CurrentRoom, (int)num4, isOpened, place, placeView);
                        break;
                    }
                    else
                        break;
                case 11:
                    if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.BattleServer != null)
                    {
                        client.Player.CurrentRoom.BattleServer.RemoveRoom(client.Player.CurrentRoom);
                        if (client.Player != client.Player.CurrentRoom.Host)
                        {
                            client.Player.CurrentRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed", new object[0]));
                            RoomMgr.UpdatePlayerState(client.Player, (byte)0);
                            break;
                        }
                        else
                        {
                            RoomMgr.UpdatePlayerState(client.Player, (byte)2);
                            break;
                        }
                    }
                    else
                        break;
                case 12:
                    packet.ReadInt();
                    if (client.Player.CurrentRoom != null)
                    {
                        client.Player.CurrentRoom.GameType = packet.ReadInt() != 0 ? eGameType.Guild : eGameType.Free;
                        GSPacketIn pkg = client.Player.Out.SendRoomType(client.Player, client.Player.CurrentRoom);
                        client.Player.CurrentRoom.SendToAll(pkg, client.Player);
                        break;
                    }
                    else
                        break;
                case 15:
                    if (client.Player.MainWeapon == null)
                    {
                        client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                        return 0;
                    }
                    else if (client.Player.CurrentRoom != null)
                    {
                        RoomMgr.UpdatePlayerState(client.Player, packet.ReadByte());
                        break;
                    }
                    else
                        break;
                default:
                    Console.WriteLine("??????????????GameRoomHandler: " + (object)(GameRoomPackageType)num1);
                    break;
            }
            return 0;
        }
    }
}
