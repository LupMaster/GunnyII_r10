using Bussiness.CenterService;
using Bussiness.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class PlayerBussiness : BaseBussiness
    {
        public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
        {
            List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)ConsortiaID;
                this.db.GetReader(ref ResultDataReader, "SP_Consortia_Users_All", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitConsortiaUserInfo(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
        {
            ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
            consortiaUserInfo.ID = (int)dr["ID"];
            consortiaUserInfo.ConsortiaID = (int)dr["ConsortiaID"];
            consortiaUserInfo.DutyID = (int)dr["DutyID"];
            consortiaUserInfo.DutyName = dr["DutyName"].ToString();
            consortiaUserInfo.IsExist = (bool)dr["IsExist"];
            consortiaUserInfo.RatifierID = (int)dr["RatifierID"];
            consortiaUserInfo.RatifierName = dr["RatifierName"].ToString();
            consortiaUserInfo.Remark = dr["Remark"].ToString();
            consortiaUserInfo.UserID = (int)dr["UserID"];
            consortiaUserInfo.UserName = dr["UserName"].ToString();
            consortiaUserInfo.Grade = (int)dr["Grade"];
            consortiaUserInfo.GP = (int)dr["GP"];
            consortiaUserInfo.Repute = (int)dr["Repute"];
            consortiaUserInfo.State = (int)dr["State"];
            consortiaUserInfo.Right = (int)dr["Right"];
            consortiaUserInfo.Offer = (int)dr["Offer"];
            consortiaUserInfo.Colors = dr["Colors"].ToString();
            consortiaUserInfo.Style = dr["Style"].ToString();
            consortiaUserInfo.Hide = (int)dr["Hide"];
            consortiaUserInfo.Skin = dr["Skin"] == null ? "" : consortiaUserInfo.Skin;
            consortiaUserInfo.Level = (int)dr["Level"];
            consortiaUserInfo.LastDate = (DateTime)dr["LastDate"];
            consortiaUserInfo.Sex = (bool)dr["Sex"];
            consortiaUserInfo.IsBanChat = (bool)dr["IsBanChat"];
            consortiaUserInfo.Win = (int)dr["Win"];
            consortiaUserInfo.Total = (int)dr["Total"];
            consortiaUserInfo.Escape = (int)dr["Escape"];
            consortiaUserInfo.RichesOffer = (int)dr["RichesOffer"];
            consortiaUserInfo.RichesRob = (int)dr["RichesRob"];
            consortiaUserInfo.LoginName = dr["LoginName"] == null ? "" : dr["LoginName"].ToString();
            consortiaUserInfo.Nimbus = (int)dr["Nimbus"];
            consortiaUserInfo.FightPower = (int)dr["FightPower"];
            consortiaUserInfo.typeVIP = Convert.ToByte(dr["typeVIP"]);
            consortiaUserInfo.VIPLevel = (int)dr["VIPLevel"];
            return consortiaUserInfo;
        }

        public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
        {
            bool flag = false;
            try
            {
                player = new PlayerInfo();
                player.Agility = 0;
                player.Attack = 0;
                player.Colors = ",,,,,,";
                player.Skin = "";
                player.ConsortiaID = 0;
                player.Defence = 0;
                player.Gold = 20000;
                player.GP = 1;
                player.Grade = 1;
                player.ID = 0;
                player.Luck = 0;
                player.Money = 0;
                player.NickName = "";
                player.Sex = sex;
                player.State = 0;
                player.Style = ",,,,,,";
                player.Hide = 1111111111;
                player.GiftToken = 20000;
                SqlParameter[] SqlParameters = new SqlParameter[22];
                SqlParameters[0] = new SqlParameter("@UserID", SqlDbType.Int);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@Attack", (object)player.Attack);
                SqlParameters[2] = new SqlParameter("@Colors", player.Colors == null ? (object)"" : (object)player.Colors);
                SqlParameters[3] = new SqlParameter("@ConsortiaID", (object)player.ConsortiaID);
                SqlParameters[4] = new SqlParameter("@Defence", (object)player.Defence);
                SqlParameters[5] = new SqlParameter("@Gold", (object)player.Gold);
                SqlParameters[6] = new SqlParameter("@GP", (object)player.GP);
                SqlParameters[7] = new SqlParameter("@Grade", (object)player.Grade);
                SqlParameters[8] = new SqlParameter("@Luck", (object)player.Luck);
                SqlParameters[9] = new SqlParameter("@Money", (object)player.Money);
                SqlParameters[10] = new SqlParameter("@Style", player.Style == null ? (object)"" : (object)player.Style);
                SqlParameters[11] = new SqlParameter("@Agility", (object)player.Agility);
                SqlParameters[12] = new SqlParameter("@State", (object)player.State);
                SqlParameters[13] = new SqlParameter("@UserName", (object)userName);
                SqlParameters[14] = new SqlParameter("@PassWord", (object)passWord);
                SqlParameters[15] = new SqlParameter("@Sex", (object)(int)(sex ? 1 : 0));
                SqlParameters[16] = new SqlParameter("@Hide", (object)player.Hide);
                SqlParameters[17] = new SqlParameter("@ActiveIP", (object)IP);
                SqlParameters[18] = new SqlParameter("@Skin", player.Skin == null ? (object)"" : (object)player.Skin);
                SqlParameters[19] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[19].Direction = ParameterDirection.ReturnValue;
                SqlParameters[20] = new SqlParameter("@Site", (object)site);
                SqlParameters[21] = new SqlParameter("@GiftToken", (object)player.GiftToken);
                flag = this.db.RunProcedure("SP_Users_Active", SqlParameters);
                player.ID = (int)SqlParameters[0].Value;
                flag = (int)SqlParameters[19].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
        {
            bool flag = false;
            try
            {
                string[] strArray1 = bStyle.Split(new char[1]
        {
          ','
        });
                string[] strArray2 = gStyle.Split(new char[1]
        {
          ','
        });
                SqlParameter[] SqlParameters = new SqlParameter[21]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@PassWord", (object) passWord),
          new SqlParameter("@NickName", (object) nickName),
          new SqlParameter("@BArmID", (object) int.Parse(strArray1[0])),
          new SqlParameter("@BHairID", (object) int.Parse(strArray1[1])),
          new SqlParameter("@BFaceID", (object) int.Parse(strArray1[2])),
          new SqlParameter("@BClothID", (object) int.Parse(strArray1[3])),
          new SqlParameter("@BHatID", (object) int.Parse(strArray1[4])),
          new SqlParameter("@GArmID", (object) int.Parse(strArray2[0])),
          new SqlParameter("@GHairID", (object) int.Parse(strArray2[1])),
          new SqlParameter("@GFaceID", (object) int.Parse(strArray2[2])),
          new SqlParameter("@GClothID", (object) int.Parse(strArray2[3])),
          new SqlParameter("@GHatID", (object) int.Parse(strArray2[4])),
          new SqlParameter("@ArmColor", (object) armColor),
          new SqlParameter("@HairColor", (object) hairColor),
          new SqlParameter("@FaceColor", (object) faceColor),
          new SqlParameter("@ClothColor", (object) clothColor),
          new SqlParameter("@HatColor", (object) clothColor),
          new SqlParameter("@Sex", (object) sex),
          new SqlParameter("@StyleDate", (object) validDate),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[20].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Users_RegisterNotValidate", SqlParameters);
                int num = (int)SqlParameters[20].Value;
                flag = num == 0;
                switch (num)
                {
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2", new object[0]);
                        break;
                    case 3:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3", new object[0]);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@NickName", (object) nickName),
          new SqlParameter("@NewNickName", (object) newNickName),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Users_RenameNick", SqlParameters);
                int num = (int)SqlParameters[3].Value;
                flag = num == 0;
                switch (num)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"RenameNick", ex);
            }
            return flag;
        }

        public bool RenameNick(string userName, string nickName, string newNickName)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@NickName", (object) nickName),
          new SqlParameter("@NewNickName", (object) newNickName),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Users_RenameNick2", SqlParameters);
                flag = (int)SqlParameters[3].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"RenameNick", ex);
            }
            return flag;
        }

        public bool DisableUser(string userName, bool isExit)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@IsExist", (object) (int) (isExit ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Disable_User", SqlParameters);
                if ((int)SqlParameters[2].Value == 0)
                    flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"DisableUser", ex);
            }
            return flag;
        }

        public bool UpdatePassWord(int userID, string password)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_UpdatePassword", new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@Password", (object) password)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Password_Add", new SqlParameter[6]
        {
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@PasswordQuestion1", (object) PasswordQuestion1),
          new SqlParameter("@PasswordAnswer1", (object) PasswordAnswer1),
          new SqlParameter("@PasswordQuestion2", (object) PasswordQuestion2),
          new SqlParameter("@PasswordAnswer2", (object) PasswordAnswer2),
          new SqlParameter("@FailedPasswordAttemptCount", (object) Count)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) userID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Users_PasswordInfo", SqlParameters);
                while (ResultDataReader.Read())
                {
                    PasswordQuestion1 = ResultDataReader["PasswordQuestion1"] == null ? "" : ResultDataReader["PasswordQuestion1"].ToString();
                    PasswordAnswer1 = ResultDataReader["PasswordAnswer1"] == null ? "" : ResultDataReader["PasswordAnswer1"].ToString();
                    PasswordQuestion2 = ResultDataReader["PasswordQuestion2"] == null ? "" : ResultDataReader["PasswordQuestion2"].ToString();
                    PasswordAnswer2 = ResultDataReader["PasswordAnswer2"] == null ? "" : ResultDataReader["PasswordAnswer2"].ToString();
                    Count = !((DateTime)ResultDataReader["LastFindDate"] == DateTime.Today) ? 5 : (int)ResultDataReader["FailedPasswordAttemptCount"];
                }
            }
            catch (Exception ex)
            {
                if (!BaseBussiness.log.IsErrorEnabled)
                    return;
                BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
        }

        public bool UpdatePasswordTwo(int userID, string passwordTwo)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_UpdatePasswordTwo", new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@PasswordTwo", (object) passwordTwo)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public PlayerInfo[] GetUserLoginList(string userName)
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserName", SqlDbType.VarChar, 200)
        };
                SqlParameters[0].Value = (object)userName;
                this.db.GetReader(ref ResultDataReader, "SP_Users_LoginList", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitPlayerInfo(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname, string ActiveIP)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[5]
        {
          new SqlParameter("@UserName", (object) username),
          new SqlParameter("@Password", (object) ""),
          new SqlParameter("@FirstValidate", (object) (int) (firstValidate ? 1 : 0)),
          new SqlParameter("@Nickname", (object) nickname),
          new SqlParameter("@ActiveIP", (object) ActiveIP)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Users_LoginWeb", SqlParameters);
                if (ResultDataReader.Read())
                {
                    isFirst = (int)ResultDataReader["IsFirst"];
                    isExist = (bool)ResultDataReader["IsExist"];
                    forbidDate = (DateTime)ResultDataReader["ForbidDate"];
                    if (isFirst > 1)
                        --isFirst;
                    return this.InitPlayerInfo(ResultDataReader);
                }
            }
            catch (Exception ex)
            {
                isError = true;
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public PlayerInfo LoginGame(string username, string password)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserName", (object) username),
          new SqlParameter("@Password", (object) password)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Users_Login", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPlayerInfo(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public PlayerInfo ReLoadPlayer(int ID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", (object) ID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Users_Reload", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPlayerInfo(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public bool UpdatePlayer(PlayerInfo player)
        {
            bool flag = false;
            try
            {
                if (player.Grade < 1)
                    return flag;
                SqlParameter[] SqlParameters = new SqlParameter[81];
                SqlParameters[0] = new SqlParameter("@UserID", (object)player.ID);
                SqlParameters[1] = new SqlParameter("@Attack", (object)player.Attack);
                SqlParameters[2] = new SqlParameter("@Colors", player.Colors == null ? (object)"" : (object)player.Colors);
                SqlParameters[3] = new SqlParameter("@ConsortiaID", (object)player.ConsortiaID);
                SqlParameters[4] = new SqlParameter("@Defence", (object)player.Defence);
                SqlParameters[5] = new SqlParameter("@Gold", (object)player.Gold);
                SqlParameters[6] = new SqlParameter("@GP", (object)player.GP);
                SqlParameters[7] = new SqlParameter("@Grade", (object)player.Grade);
                SqlParameters[8] = new SqlParameter("@Luck", (object)player.Luck);
                SqlParameters[9] = new SqlParameter("@Money", (object)player.Money);
                SqlParameters[10] = new SqlParameter("@Style", player.Style == null ? (object)"" : (object)player.Style);
                SqlParameters[11] = new SqlParameter("@Agility", (object)player.Agility);
                SqlParameters[12] = new SqlParameter("@State", (object)player.State);
                SqlParameters[13] = new SqlParameter("@Hide", (object)player.Hide);
                SqlParameters[14] = new SqlParameter("@ExpendDate", (object)(!player.ExpendDate.HasValue ? new DateTime?(DateTime.Now) : player.ExpendDate));
                SqlParameters[15] = new SqlParameter("@Win", (object)player.Win);
                SqlParameters[16] = new SqlParameter("@Total", (object)player.Total);
                SqlParameters[17] = new SqlParameter("@Escape", (object)player.Escape);
                SqlParameters[18] = new SqlParameter("@Skin", player.Skin == null ? (object)"" : (object)player.Skin);
                SqlParameters[19] = new SqlParameter("@Offer", (object)player.Offer);
                SqlParameters[20] = new SqlParameter("@AntiAddiction", (object)player.AntiAddiction);
                SqlParameters[20].Direction = ParameterDirection.InputOutput;
                SqlParameters[21] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[21].Direction = ParameterDirection.ReturnValue;
                SqlParameters[22] = new SqlParameter("@RichesOffer", (object)player.RichesOffer);
                SqlParameters[23] = new SqlParameter("@RichesRob", (object)player.RichesRob);
                SqlParameters[24] = new SqlParameter("@CheckCount", (object)player.CheckCount);
                SqlParameters[24].Direction = ParameterDirection.InputOutput;
                SqlParameters[25] = new SqlParameter("@MarryInfoID", (object)player.MarryInfoID);
                SqlParameters[26] = new SqlParameter("@DayLoginCount", (object)player.DayLoginCount);
                SqlParameters[27] = new SqlParameter("@Nimbus", (object)player.Nimbus);
                SqlParameters[28] = new SqlParameter("@LastAward", (object)player.LastAward);
                SqlParameters[29] = new SqlParameter("@GiftToken", (object)player.GiftToken);
                SqlParameters[30] = new SqlParameter("@QuestSite", (object)player.QuestSite);
                SqlParameters[31] = new SqlParameter("@PvePermission", (object)player.PvePermission);
                SqlParameters[32] = new SqlParameter("@FightPower", (object)player.FightPower);
                SqlParameters[33] = new SqlParameter("@AnswerSite", (object)player.AnswerSite);
                SqlParameters[34] = new SqlParameter("@LastAuncherAward", (object)player.LastAward);
                SqlParameters[35] = new SqlParameter("@hp", (object)player.hp);
                SqlParameters[36] = new SqlParameter("@ChatCount", (object)player.ChatCount);
                SqlParameters[37] = new SqlParameter("@SpaPubGoldRoomLimit", (object)player.SpaPubGoldRoomLimit);
                SqlParameters[38] = new SqlParameter("@LastSpaDate", (object)player.LastSpaDate);
                SqlParameters[39] = new SqlParameter("@FightLabPermission", (object)player.FightLabPermission);
                SqlParameters[40] = new SqlParameter("@SpaPubMoneyRoomLimit", (object)player.SpaPubMoneyRoomLimit);
                SqlParameters[41] = new SqlParameter("@IsInSpaPubGoldToday", (object)(int)(player.IsInSpaPubGoldToday ? 1 : 0));
                SqlParameters[42] = new SqlParameter("@IsInSpaPubMoneyToday", (object)(int)(player.IsInSpaPubMoneyToday ? 1 : 0));
                SqlParameters[43] = new SqlParameter("@AchievementPoint", (object)player.AchievementPoint);
                SqlParameters[44] = new SqlParameter("@LastWeekly", (object)player.LastWeekly);
                SqlParameters[45] = new SqlParameter("@LastWeeklyVersion", (object)player.LastWeeklyVersion);
                SqlParameters[46] = new SqlParameter("@GiftGp", (object)player.GiftGp);
                SqlParameters[47] = new SqlParameter("@GiftLevel", (object)player.GiftLevel);
                SqlParameters[48] = new SqlParameter("@IsOpenGift", (object)(int)(player.IsOpenGift ? 1 : 0));
                SqlParameters[49] = new SqlParameter("@WeaklessGuildProgressStr", (object)player.WeaklessGuildProgressStr);
                SqlParameters[50] = new SqlParameter("@IsOldPlayer", (object)(int)(player.IsOldPlayer ? 1 : 0));
                SqlParameters[51] = new SqlParameter("@VIPLevel", (object)player.VIPLevel);
                SqlParameters[52] = new SqlParameter("@VIPExp", (object)player.VIPExp);
                SqlParameters[53] = new SqlParameter("@Score", (object)player.Score);
                SqlParameters[54] = new SqlParameter("@OptionOnOff", (object)player.OptionOnOff);
                SqlParameters[55] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", (object)(int)(player.isOldPlayerHasValidEquitAtLogin ? 1 : 0));
                SqlParameters[56] = new SqlParameter("@badLuckNumber", (object)player.badLuckNumber);
                SqlParameters[57] = new SqlParameter("@luckyNum", (object)player.luckyNum);
                SqlParameters[58] = new SqlParameter("@lastLuckyNumDate", (object)player.lastLuckyNumDate);
                SqlParameters[59] = new SqlParameter("@lastLuckNum", (object)player.lastLuckNum);
                SqlParameters[60] = new SqlParameter("@CardSoul", (object)player.CardSoul);
                SqlParameters[61] = new SqlParameter("@uesedFinishTime", (object)player.uesedFinishTime);
                SqlParameters[62] = new SqlParameter("@totemId", (object)player.totemId);
                SqlParameters[63] = new SqlParameter("@damageScores", (object)player.damageScores);
                SqlParameters[64] = new SqlParameter("@petScore", (object)player.petScore);
                SqlParameters[65] = new SqlParameter("@IsShowConsortia", (object)(int)(player.IsShowConsortia ? 1 : 0));
                SqlParameters[66] = new SqlParameter("@LastRefreshPet", (object)player.LastRefreshPet);
                SqlParameters[67] = new SqlParameter("@receiebox", (object)player.receiebox);
                SqlParameters[68] = new SqlParameter("@receieGrade", (object)player.receieGrade);
                SqlParameters[69] = new SqlParameter("@needGetBoxTime", (object)player.needGetBoxTime);
                SqlParameters[70] = new SqlParameter("@myScore", (object)player.myScore);
                SqlParameters[71] = new SqlParameter("@TimeBox", (object)player.TimeBox.ToString());
                SqlParameters[72] = new SqlParameter("@IsFistGetPet", (object)(int)(player.IsFistGetPet ? 1 : 0));
                SqlParameters[73] = new SqlParameter("@MaxBuyHonor", (object)player.MaxBuyHonor);
                SqlParameters[74] = new SqlParameter("@Medal", (object)player.medal);
                SqlParameters[75] = new SqlParameter("@myHonor", (object)player.myHonor);
                SqlParameters[76] = new SqlParameter("@LeagueMoney", (object)player.LeagueMoney);
                SqlParameters[77] = new SqlParameter("@Honor", (object)player.Honor);
                SqlParameters[78] = new SqlParameter("@necklaceExp", (object)player.necklaceExp);
                SqlParameters[79] = new SqlParameter("@necklaceExpAdd", (object)player.necklaceExpAdd);
                SqlParameters[80] = new SqlParameter("@hardCurrency", (object)player.hardCurrency);
                this.db.RunProcedure("SP_Users_Update", SqlParameters);
                flag = (int)SqlParameters[21].Value == 0;
                if (flag)
                {
                    player.AntiAddiction = (int)SqlParameters[20].Value;
                    player.CheckCount = (int)SqlParameters[24].Value;
                }
                player.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdatePlayerMarry(PlayerInfo player)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Marry", new SqlParameter[7]
        {
          new SqlParameter("@UserID", (object) player.ID),
          new SqlParameter("@IsMarried", (object) (int) (player.IsMarried ? 1 : 0)),
          new SqlParameter("@SpouseID", (object) player.SpouseID),
          new SqlParameter("@SpouseName", (object) player.SpouseName),
          new SqlParameter("@IsCreatedMarryRoom", (object) (int) (player.IsCreatedMarryRoom ? 1 : 0)),
          new SqlParameter("@SelfMarryRoomID", (object) player.SelfMarryRoomID),
          new SqlParameter("@IsGotRing", (object) (int) (player.IsGotRing ? 1 : 0))
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdatePlayerMarry", ex);
            }
            return flag;
        }

        public bool UpdatePlayerLastAward(int id, int type)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_LastAward", new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) id),
          new SqlParameter("@Type", (object) type)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdatePlayerAward", ex);
            }
            return flag;
        }

        public PlayerInfo GetUserSingleByUserID(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserID", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPlayerInfo(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public PlayerInfo GetUserSingleByUserName(string userName)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserName", SqlDbType.VarChar, 200)
        };
                SqlParameters[0].Value = (object)userName;
                this.db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserName", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPlayerInfo(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public PlayerInfo GetUserSingleByNickName(string nickName)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@NickName", SqlDbType.VarChar, 200)
        };
                SqlParameters[0].Value = (object)nickName;
                this.db.GetReader(ref ResultDataReader, "SP_Users_SingleByNickName", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPlayerInfo(ResultDataReader);
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (PlayerInfo)null;
        }

        public PlayerInfo InitPlayerInfo(SqlDataReader reader)
        {
            PlayerInfo playerInfo1 = new PlayerInfo();
            playerInfo1.Password = (string)reader["Password"];
            playerInfo1.IsConsortia = (bool)reader["IsConsortia"];
            playerInfo1.Agility = (int)reader["Agility"];
            playerInfo1.Attack = (int)reader["Attack"];
            playerInfo1.hp = (int)reader["hp"];
            playerInfo1.Colors = reader["Colors"] == null ? "" : reader["Colors"].ToString();
            playerInfo1.ConsortiaID = (int)reader["ConsortiaID"];
            playerInfo1.Defence = (int)reader["Defence"];
            playerInfo1.Gold = (int)reader["Gold"];
            playerInfo1.GP = (int)reader["GP"];
            playerInfo1.Grade = (int)reader["Grade"];
            playerInfo1.ID = (int)reader["UserID"];
            playerInfo1.Luck = (int)reader["Luck"];
            playerInfo1.Money = (int)reader["Money"];
            playerInfo1.NickName = (string)reader["NickName"] == null ? "" : (string)reader["NickName"];
            playerInfo1.Sex = (bool)reader["Sex"];
            playerInfo1.State = (int)reader["State"];
            playerInfo1.Style = reader["Style"] == null ? "" : reader["Style"].ToString();
            playerInfo1.Hide = (int)reader["Hide"];
            playerInfo1.Repute = (int)reader["Repute"];
            playerInfo1.UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
            playerInfo1.ConsortiaName = reader["ConsortiaName"] == null ? "" : reader["ConsortiaName"].ToString();
            playerInfo1.Offer = (int)reader["Offer"];
            playerInfo1.Win = (int)reader["Win"];
            playerInfo1.Total = (int)reader["Total"];
            playerInfo1.Escape = (int)reader["Escape"];
            playerInfo1.Skin = reader["Skin"] == null ? "" : reader["Skin"].ToString();
            playerInfo1.IsBanChat = (bool)reader["IsBanChat"];
            playerInfo1.ReputeOffer = (int)reader["ReputeOffer"];
            playerInfo1.ConsortiaRepute = (int)reader["ConsortiaRepute"];
            playerInfo1.ConsortiaLevel = (int)reader["ConsortiaLevel"];
            playerInfo1.StoreLevel = (int)reader["StoreLevel"];
            playerInfo1.ShopLevel = (int)reader["ShopLevel"];
            playerInfo1.SmithLevel = (int)reader["SmithLevel"];
            playerInfo1.ConsortiaHonor = (int)reader["ConsortiaHonor"];
            playerInfo1.RichesOffer = (int)reader["RichesOffer"];
            playerInfo1.RichesRob = (int)reader["RichesRob"];
            playerInfo1.AntiAddiction = (int)reader["AntiAddiction"];
            playerInfo1.DutyLevel = (int)reader["DutyLevel"];
            playerInfo1.DutyName = reader["DutyName"] == null ? "" : reader["DutyName"].ToString();
            playerInfo1.Right = (int)reader["Right"];
            playerInfo1.ChairmanName = reader["ChairmanName"] == null ? "" : reader["ChairmanName"].ToString();
            playerInfo1.AddDayGP = (int)reader["AddDayGP"];
            playerInfo1.AddDayOffer = (int)reader["AddDayOffer"];
            playerInfo1.AddWeekGP = (int)reader["AddWeekGP"];
            playerInfo1.AddWeekOffer = (int)reader["AddWeekOffer"];
            playerInfo1.ConsortiaRiches = (int)reader["ConsortiaRiches"];
            playerInfo1.CheckCount = (int)reader["CheckCount"];
            playerInfo1.IsMarried = (bool)reader["IsMarried"];
            playerInfo1.SpouseID = (int)reader["SpouseID"];
            playerInfo1.SpouseName = reader["SpouseName"] == null ? "" : reader["SpouseName"].ToString();
            playerInfo1.MarryInfoID = (int)reader["MarryInfoID"];
            playerInfo1.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
            playerInfo1.DayLoginCount = (int)reader["DayLoginCount"];
            playerInfo1.PasswordTwo = reader["PasswordTwo"] == null ? "" : reader["PasswordTwo"].ToString();
            playerInfo1.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
            playerInfo1.IsGotRing = (bool)reader["IsGotRing"];
            playerInfo1.Rename = (bool)reader["Rename"];
            playerInfo1.ConsortiaRename = (bool)reader["ConsortiaRename"];
            playerInfo1.IsDirty = false;
            playerInfo1.IsFirst = (int)reader["IsFirst"];
            playerInfo1.Nimbus = (int)reader["Nimbus"];
            playerInfo1.LastAward = (DateTime)reader["LastAward"];
            playerInfo1.GiftToken = (int)reader["GiftToken"];
            playerInfo1.QuestSite = reader["QuestSite"] == null ? new byte[200] : (byte[])reader["QuestSite"];
            playerInfo1.PvePermission = reader["PvePermission"] == null ? "" : reader["PvePermission"].ToString();
            playerInfo1.FightPower = (int)reader["FightPower"];
            playerInfo1.PasswordQuest1 = reader["PasswordQuestion1"] == null ? "" : reader["PasswordQuestion1"].ToString();
            playerInfo1.PasswordQuest2 = reader["PasswordQuestion2"] == null ? "" : reader["PasswordQuestion2"].ToString();
            PlayerInfo playerInfo2 = playerInfo1;
            DateTime dateTime = (DateTime)reader["LastFindDate"];
            playerInfo2.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
            playerInfo1.AnswerSite = (int)reader["AnswerSite"];
            playerInfo1.medal = (int)reader["Medal"];
            playerInfo1.ChatCount = (int)reader["ChatCount"];
            playerInfo1.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
            playerInfo1.LastSpaDate = (DateTime)reader["LastSpaDate"];
            playerInfo1.FightLabPermission = (string)reader["FightLabPermission"];
            playerInfo1.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
            playerInfo1.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
            playerInfo1.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
            playerInfo1.AchievementPoint = (int)reader["AchievementPoint"];
            playerInfo1.LastWeekly = (DateTime)reader["LastWeekly"];
            playerInfo1.LastWeeklyVersion = (int)reader["LastWeeklyVersion"];
            playerInfo1.GiftGp = (int)reader["GiftGp"];
            playerInfo1.GiftLevel = (int)reader["GiftLevel"];
            playerInfo1.IsOpenGift = (bool)reader["IsOpenGift"];
            playerInfo1.badgeID = (int)reader["badgeID"];
            playerInfo1.typeVIP = Convert.ToByte(reader["typeVIP"]);
            playerInfo1.VIPLevel = (int)reader["VIPLevel"];
            playerInfo1.VIPExp = (int)reader["VIPExp"];
            playerInfo1.VIPExpireDay = (DateTime)reader["VIPExpireDay"];
            playerInfo1.LastVIPPackTime = (DateTime)reader["LastVIPPackTime"];
            playerInfo1.CanTakeVipReward = (bool)reader["CanTakeVipReward"];
            playerInfo1.WeaklessGuildProgressStr = (string)reader["WeaklessGuildProgressStr"];
            playerInfo1.IsOldPlayer = (bool)reader["IsOldPlayer"];
            playerInfo1.LastDate = (DateTime)reader["LastDate"];
            playerInfo1.VIPLastDate = (DateTime)reader["VIPLastDate"];
            playerInfo1.Score = (int)reader["Score"];
            playerInfo1.OptionOnOff = (int)reader["OptionOnOff"];
            playerInfo1.isOldPlayerHasValidEquitAtLogin = (bool)reader["isOldPlayerHasValidEquitAtLogin"];
            playerInfo1.badLuckNumber = (int)reader["badLuckNumber"];
            playerInfo1.luckyNum = (int)reader["luckyNum"];
            playerInfo1.lastLuckyNumDate = (DateTime)reader["lastLuckyNumDate"];
            playerInfo1.lastLuckNum = (int)reader["lastLuckNum"];
            playerInfo1.CardSoul = (int)reader["CardSoul"];
            playerInfo1.uesedFinishTime = (int)reader["uesedFinishTime"];
            playerInfo1.totemId = (int)reader["totemId"];
            playerInfo1.damageScores = (int)reader["damageScores"];
            playerInfo1.petScore = (int)reader["petScore"];
            playerInfo1.IsShowConsortia = (bool)reader["IsShowConsortia"];
            playerInfo1.LastRefreshPet = (DateTime)reader["LastRefreshPet"];
            playerInfo1.receiebox = (int)reader["receiebox"];
            playerInfo1.receieGrade = (int)reader["receieGrade"];
            playerInfo1.needGetBoxTime = (int)reader["needGetBoxTime"];
            playerInfo1.myScore = (int)reader["myScore"];
            playerInfo1.LastGetEgg = (DateTime)reader["LastGetEgg"];
            playerInfo1.TimeBox = (DateTime)reader["TimeBox"];
            playerInfo1.IsFistGetPet = (bool)reader["IsFistGetPet"];
            playerInfo1.myHonor = (int)reader["myHonor"];
            playerInfo1.hardCurrency = (int)reader["hardCurrency"];
            playerInfo1.MaxBuyHonor = (int)reader["MaxBuyHonor"];
            playerInfo1.LeagueMoney = (int)reader["LeagueMoney"];
            playerInfo1.Honor = (string)reader["Honor"];
            playerInfo1.necklaceExp = (int)reader["necklaceExp"];
            playerInfo1.necklaceExpAdd = (int)reader["necklaceExpAdd"];
            return playerInfo1;
        }

        public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            try
            {
                string queryWhere = " IsExist=1 and IsFirst<> 0 ";
                if (userID != -1)
                    queryWhere = string.Concat(new object[4]
          {
            (object) queryWhere,
            (object) " and UserID =",
            (object) userID,
            (object) " "
          });
                string str = "GP desc";
                switch (order)
                {
                    case 1:
                        str = "Offer desc";
                        break;
                    case 2:
                        str = "AddDayGP desc";
                        break;
                    case 3:
                        str = "AddWeekGP desc";
                        break;
                    case 4:
                        str = "AddDayOffer desc";
                        break;
                    case 5:
                        str = "AddWeekOffer desc";
                        break;
                    case 6:
                        str = "FightPower desc";
                        break;
                    case 7:
                        str = "AchievementPoint desc";
                        break;
                    case 8:
                        str = "AddDayAchievementPoint desc";
                        break;
                    case 9:
                        str = "AddWeekAchievementPoint desc";
                        break;
                    case 10:
                        str = "GiftGp desc";
                        break;
                    case 11:
                        str = "AddDayGiftGp desc";
                        break;
                    case 12:
                        str = "AddWeekGiftGp desc";
                        break;
                }
                string fdOreder = str + ",UserID";
                foreach (DataRow dataRow in (InternalDataCollectionBase)this.GetPage("V_Sys_Users_Detail", queryWhere, page, size, "*", fdOreder, "UserID", ref total).Rows)
                {
                    PlayerInfo playerInfo = new PlayerInfo();
                    playerInfo.Agility = (int)dataRow["Agility"];
                    playerInfo.Attack = (int)dataRow["Attack"];
                    playerInfo.Colors = dataRow["Colors"] == null ? "" : dataRow["Colors"].ToString();
                    playerInfo.ConsortiaID = (int)dataRow["ConsortiaID"];
                    playerInfo.Defence = (int)dataRow["Defence"];
                    playerInfo.Gold = (int)dataRow["Gold"];
                    playerInfo.GP = (int)dataRow["GP"];
                    playerInfo.Grade = (int)dataRow["Grade"];
                    playerInfo.ID = (int)dataRow["UserID"];
                    playerInfo.Luck = (int)dataRow["Luck"];
                    playerInfo.Money = (int)dataRow["Money"];
                    playerInfo.NickName = dataRow["NickName"] == null ? "" : dataRow["NickName"].ToString();
                    playerInfo.Sex = (bool)dataRow["Sex"];
                    playerInfo.State = (int)dataRow["State"];
                    playerInfo.Style = dataRow["Style"] == null ? "" : dataRow["Style"].ToString();
                    playerInfo.Hide = (int)dataRow["Hide"];
                    playerInfo.Repute = (int)dataRow["Repute"];
                    playerInfo.UserName = dataRow["UserName"] == null ? "" : dataRow["UserName"].ToString();
                    playerInfo.ConsortiaName = dataRow["ConsortiaName"] == null ? "" : dataRow["ConsortiaName"].ToString();
                    playerInfo.Offer = (int)dataRow["Offer"];
                    playerInfo.Skin = dataRow["Skin"] == null ? "" : dataRow["Skin"].ToString();
                    playerInfo.IsBanChat = (bool)dataRow["IsBanChat"];
                    playerInfo.ReputeOffer = (int)dataRow["ReputeOffer"];
                    playerInfo.ConsortiaRepute = (int)dataRow["ConsortiaRepute"];
                    playerInfo.ConsortiaLevel = (int)dataRow["ConsortiaLevel"];
                    playerInfo.StoreLevel = (int)dataRow["StoreLevel"];
                    playerInfo.ShopLevel = (int)dataRow["ShopLevel"];
                    playerInfo.SmithLevel = (int)dataRow["SmithLevel"];
                    playerInfo.ConsortiaHonor = (int)dataRow["ConsortiaHonor"];
                    playerInfo.RichesOffer = (int)dataRow["RichesOffer"];
                    playerInfo.RichesRob = (int)dataRow["RichesRob"];
                    playerInfo.DutyLevel = (int)dataRow["DutyLevel"];
                    playerInfo.DutyName = dataRow["DutyName"] == null ? "" : dataRow["DutyName"].ToString();
                    playerInfo.Right = (int)dataRow["Right"];
                    playerInfo.ChairmanName = dataRow["ChairmanName"] == null ? "" : dataRow["ChairmanName"].ToString();
                    playerInfo.Win = (int)dataRow["Win"];
                    playerInfo.Total = (int)dataRow["Total"];
                    playerInfo.Escape = (int)dataRow["Escape"];
                    playerInfo.AddDayGP = (int)dataRow["AddDayGP"] == 0 ? playerInfo.GP : (int)dataRow["AddDayGP"];
                    playerInfo.AddDayOffer = (int)dataRow["AddDayOffer"] == 0 ? playerInfo.Offer : (int)dataRow["AddDayOffer"];
                    playerInfo.AddWeekGP = (int)dataRow["AddWeekGP"] == 0 ? playerInfo.GP : (int)dataRow["AddWeekyGP"];
                    playerInfo.AddWeekOffer = (int)dataRow["AddWeekOffer"] == 0 ? playerInfo.Offer : (int)dataRow["AddWeekOffer"];
                    playerInfo.ConsortiaRiches = (int)dataRow["ConsortiaRiches"];
                    playerInfo.CheckCount = (int)dataRow["CheckCount"];
                    playerInfo.Nimbus = (int)dataRow["Nimbus"];
                    playerInfo.GiftToken = (int)dataRow["GiftToken"];
                    playerInfo.QuestSite = dataRow["QuestSite"] == null ? new byte[200] : (byte[])dataRow["QuestSite"];
                    playerInfo.PvePermission = dataRow["PvePermission"] == null ? "" : dataRow["PvePermission"].ToString();
                    playerInfo.FightPower = (int)dataRow["FightPower"];
                    list.Add(playerInfo);
                }
                resultValue = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return list.ToArray();
        }

        public SqlDataProvider.Data.ItemInfo[] GetUserItem(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Items_All", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItem(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public SqlDataProvider.Data.ItemInfo[] GetUserBagByType(int UserID, int bagType)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4),
          null
        };
                SqlParameters[0].Value = (object)UserID;
                SqlParameters[1] = new SqlParameter("@BagType", (object)bagType);
                this.db.GetReader(ref ResultDataReader, "SP_Users_BagByType", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItem(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserEuqip(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItem(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserBeadEuqip(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Bead_Equip", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItem(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserEuqipByNick(string Nick)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
        };
                SqlParameters[0].Value = (object)Nick;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip_By_Nick", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItem(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public SqlDataProvider.Data.ItemInfo GetUserItemSingle(int itemID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)itemID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Items_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitItem(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (SqlDataProvider.Data.ItemInfo)null;
        }

        public SqlDataProvider.Data.ItemInfo InitItem(SqlDataReader reader)
        {
            SqlDataProvider.Data.ItemInfo itemInfo = new SqlDataProvider.Data.ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
            itemInfo.AgilityCompose = (int)reader["AgilityCompose"];
            itemInfo.AttackCompose = (int)reader["AttackCompose"];
            itemInfo.Color = reader["Color"].ToString();
            itemInfo.Count = (int)reader["Count"];
            itemInfo.DefendCompose = (int)reader["DefendCompose"];
            itemInfo.ItemID = (int)reader["ItemID"];
            itemInfo.LuckCompose = (int)reader["LuckCompose"];
            itemInfo.Place = (int)reader["Place"];
            itemInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
            itemInfo.TemplateID = (int)reader["TemplateID"];
            itemInfo.UserID = (int)reader["UserID"];
            itemInfo.ValidDate = (int)reader["ValidDate"];
            itemInfo.IsDirty = false;
            itemInfo.IsExist = (bool)reader["IsExist"];
            itemInfo.IsBinds = (bool)reader["IsBinds"];
            itemInfo.IsUsed = (bool)reader["IsUsed"];
            itemInfo.BeginDate = (DateTime)reader["BeginDate"];
            itemInfo.IsJudge = (bool)reader["IsJudge"];
            itemInfo.BagType = (int)reader["BagType"];
            itemInfo.Skin = reader["Skin"].ToString();
            itemInfo.RemoveDate = (DateTime)reader["RemoveDate"];
            itemInfo.RemoveType = (int)reader["RemoveType"];
            itemInfo.Hole1 = (int)reader["Hole1"];
            itemInfo.Hole2 = (int)reader["Hole2"];
            itemInfo.Hole3 = (int)reader["Hole3"];
            itemInfo.Hole4 = (int)reader["Hole4"];
            itemInfo.Hole5 = (int)reader["Hole5"];
            itemInfo.Hole6 = (int)reader["Hole6"];
            itemInfo.StrengthenTimes = (int)reader["StrengthenTimes"];
            itemInfo.StrengthenExp = (int)reader["StrengthenExp"];
            itemInfo.Hole5Level = (int)reader["Hole5Level"];
            itemInfo.Hole5Exp = (int)reader["Hole5Exp"];
            itemInfo.Hole6Level = (int)reader["Hole6Level"];
            itemInfo.Hole6Exp = (int)reader["Hole6Exp"];
            itemInfo.IsGold = (bool)reader["IsGold"];
            itemInfo.goldBeginTime = (DateTime)reader["goldBeginTime"];
            itemInfo.goldValidDate = (int)reader["goldValidDate"];
            itemInfo.beadExp = (int)reader["beadExp"];
            itemInfo.beadLevel = (int)reader["beadLevel"];
            itemInfo.beadIsLock = (bool)reader["beadIsLock"];
            itemInfo.isShowBind = (bool)reader["isShowBind"];
            itemInfo.latentEnergyCurStr = (string)reader["latentEnergyCurStr"];
            itemInfo.latentEnergyNewStr = (string)reader["latentEnergyNewStr"];
            itemInfo.latentEnergyEndTime = (DateTime)reader["latentEnergyEndTime"];
            itemInfo.Damage = (int)reader["Damage"];
            itemInfo.Guard = (int)reader["Guard"];
            itemInfo.Blood = (int)reader["Blood"];
            itemInfo.Bless = (int)reader["Bless"];
            return itemInfo;
        }

        public bool AddGoods(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[46];
                SqlParameters[0] = new SqlParameter("@ItemID", (object)item.ItemID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[2] = new SqlParameter("@TemplateID", (object)item.TemplateID);
                SqlParameters[3] = new SqlParameter("@Place", (object)item.Place);
                SqlParameters[4] = new SqlParameter("@AgilityCompose", (object)item.AgilityCompose);
                SqlParameters[5] = new SqlParameter("@AttackCompose", (object)item.AttackCompose);
                SqlParameters[6] = new SqlParameter("@BeginDate", (object)item.BeginDate);
                SqlParameters[7] = new SqlParameter("@Color", item.Color == null ? (object)"" : (object)item.Color);
                SqlParameters[8] = new SqlParameter("@Count", (object)item.Count);
                SqlParameters[9] = new SqlParameter("@DefendCompose", (object)item.DefendCompose);
                SqlParameters[10] = new SqlParameter("@IsBinds", (object)(int)(item.IsBinds ? 1 : 0));
                SqlParameters[11] = new SqlParameter("@IsExist", (object)(int)(item.IsExist ? 1 : 0));
                SqlParameters[12] = new SqlParameter("@IsJudge", (object)(int)(item.IsJudge ? 1 : 0));
                SqlParameters[13] = new SqlParameter("@LuckCompose", (object)item.LuckCompose);
                SqlParameters[14] = new SqlParameter("@StrengthenLevel", (object)item.StrengthenLevel);
                SqlParameters[15] = new SqlParameter("@ValidDate", (object)item.ValidDate);
                SqlParameters[16] = new SqlParameter("@BagType", (object)item.BagType);
                SqlParameters[17] = new SqlParameter("@Skin", item.Skin == null ? (object)"" : (object)item.Skin);
                SqlParameters[18] = new SqlParameter("@IsUsed", (object)(int)(item.IsUsed ? 1 : 0));
                SqlParameters[19] = new SqlParameter("@RemoveType", (object)item.RemoveType);
                SqlParameters[20] = new SqlParameter("@Hole1", (object)item.Hole1);
                SqlParameters[21] = new SqlParameter("@Hole2", (object)item.Hole2);
                SqlParameters[22] = new SqlParameter("@Hole3", (object)item.Hole3);
                SqlParameters[23] = new SqlParameter("@Hole4", (object)item.Hole4);
                SqlParameters[24] = new SqlParameter("@Hole5", (object)item.Hole5);
                SqlParameters[25] = new SqlParameter("@Hole6", (object)item.Hole6);
                SqlParameters[26] = new SqlParameter("@StrengthenTimes", (object)item.StrengthenTimes);
                SqlParameters[27] = new SqlParameter("@Hole5Level", (object)item.Hole5Level);
                SqlParameters[28] = new SqlParameter("@Hole5Exp", (object)item.Hole5Exp);
                SqlParameters[29] = new SqlParameter("@Hole6Level", (object)item.Hole6Level);
                SqlParameters[30] = new SqlParameter("@Hole6Exp", (object)item.Hole6Exp);
                SqlParameters[31] = new SqlParameter("@IsGold", (object)(int)(item.IsGold ? 1 : 0));
                SqlParameters[32] = new SqlParameter("@goldValidDate", (object)item.goldValidDate);
                SqlParameters[33] = new SqlParameter("@StrengthenExp", (object)item.StrengthenExp);
                SqlParameters[34] = new SqlParameter("@beadExp", (object)item.beadExp);
                SqlParameters[35] = new SqlParameter("@beadLevel", (object)item.beadLevel);
                SqlParameters[36] = new SqlParameter("@beadIsLock", (object)(int)(item.beadIsLock ? 1 : 0));
                SqlParameters[37] = new SqlParameter("@isShowBind", (object)(int)(item.isShowBind ? 1 : 0));
                SqlParameters[38] = new SqlParameter("@Damage", (object)item.Damage);
                SqlParameters[39] = new SqlParameter("@Guard", (object)item.Guard);
                SqlParameters[40] = new SqlParameter("@Blood", (object)item.Blood);
                SqlParameters[41] = new SqlParameter("@Bless", (object)item.Bless);
                SqlParameters[42] = new SqlParameter("@goldBeginTime", (object)item.goldBeginTime);
                SqlParameters[43] = new SqlParameter("@latentEnergyEndTime", (object)item.latentEnergyEndTime);
                SqlParameters[44] = new SqlParameter("@latentEnergyCurStr", (object)item.latentEnergyCurStr);
                SqlParameters[45] = new SqlParameter("@latentEnergyNewStr", (object)item.latentEnergyNewStr);
                flag = this.db.RunProcedure("SP_Users_Items_Add", SqlParameters);
                item.ItemID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateGoods(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Items_Update", new SqlParameter[47]
        {
          new SqlParameter("@ItemID", (object) item.ItemID),
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@TemplateID", (object) item.TemplateID),
          new SqlParameter("@Place", (object) item.Place),
          new SqlParameter("@AgilityCompose", (object) item.AgilityCompose),
          new SqlParameter("@AttackCompose", (object) item.AttackCompose),
          new SqlParameter("@BeginDate", (object) item.BeginDate),
          new SqlParameter("@Color", item.Color == null ? (object) "" : (object) item.Color),
          new SqlParameter("@Count", (object) item.Count),
          new SqlParameter("@DefendCompose", (object) item.DefendCompose),
          new SqlParameter("@IsBinds", (object) (int) (item.IsBinds ? 1 : 0)),
          new SqlParameter("@IsExist", (object) (int) (item.IsExist ? 1 : 0)),
          new SqlParameter("@IsJudge", (object) (int) (item.IsJudge ? 1 : 0)),
          new SqlParameter("@LuckCompose", (object) item.LuckCompose),
          new SqlParameter("@StrengthenLevel", (object) item.StrengthenLevel),
          new SqlParameter("@ValidDate", (object) item.ValidDate),
          new SqlParameter("@BagType", (object) item.BagType),
          new SqlParameter("@Skin", (object) item.Skin),
          new SqlParameter("@IsUsed", (object) (int) (item.IsUsed ? 1 : 0)),
          new SqlParameter("@RemoveDate", (object) item.RemoveDate),
          new SqlParameter("@RemoveType", (object) item.RemoveType),
          new SqlParameter("@Hole1", (object) item.Hole1),
          new SqlParameter("@Hole2", (object) item.Hole2),
          new SqlParameter("@Hole3", (object) item.Hole3),
          new SqlParameter("@Hole4", (object) item.Hole4),
          new SqlParameter("@Hole5", (object) item.Hole5),
          new SqlParameter("@Hole6", (object) item.Hole6),
          new SqlParameter("@StrengthenTimes", (object) item.StrengthenTimes),
          new SqlParameter("@Hole5Level", (object) item.Hole5Level),
          new SqlParameter("@Hole5Exp", (object) item.Hole5Exp),
          new SqlParameter("@Hole6Level", (object) item.Hole6Level),
          new SqlParameter("@Hole6Exp", (object) item.Hole6Exp),
          new SqlParameter("@IsGold", (object) (int) (item.IsGold ? 1 : 0)),
          new SqlParameter("@goldBeginTime", (object) item.goldBeginTime.ToString()),
          new SqlParameter("@goldValidDate", (object) item.goldValidDate),
          new SqlParameter("@StrengthenExp", (object) item.StrengthenExp),
          new SqlParameter("@beadExp", (object) item.beadExp),
          new SqlParameter("@beadLevel", (object) item.beadLevel),
          new SqlParameter("@beadIsLock", (object) (int) (item.beadIsLock ? 1 : 0)),
          new SqlParameter("@isShowBind", (object) (int) (item.isShowBind ? 1 : 0)),
          new SqlParameter("@latentEnergyCurStr", (object) item.latentEnergyCurStr),
          new SqlParameter("@latentEnergyNewStr", (object) item.latentEnergyNewStr),
          new SqlParameter("@latentEnergyEndTime", (object) item.latentEnergyEndTime.ToString()),
          new SqlParameter("@Damage", (object) item.Damage),
          new SqlParameter("@Guard", (object) item.Guard),
          new SqlParameter("@Blood", (object) item.Blood),
          new SqlParameter("@Bless", (object) item.Bless)
        });
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool DeleteGoods(int itemID)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Items_Delete", new SqlParameter[1]
        {
          new SqlParameter("@ID", (object) itemID)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public BestEquipInfo[] GetCelebByDayBestEquip()
        {
            List<BestEquipInfo> list = new List<BestEquipInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Users_BestEquip");
                while (ResultDataReader.Read())
                    list.Add(new BestEquipInfo()
                    {
                        Date = (DateTime)ResultDataReader["RemoveDate"],
                        GP = (int)ResultDataReader["GP"],
                        Grade = (int)ResultDataReader["Grade"],
                        ItemName = ResultDataReader["Name"] == null ? "" : ResultDataReader["Name"].ToString(),
                        NickName = ResultDataReader["NickName"] == null ? "" : ResultDataReader["NickName"].ToString(),
                        Sex = (bool)ResultDataReader["Sex"],
                        Strengthenlevel = (int)ResultDataReader["Strengthenlevel"],
                        UserName = ResultDataReader["UserName"] == null ? "" : ResultDataReader["UserName"].ToString()
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public MailInfo InitMail(SqlDataReader reader)
        {
            return new MailInfo()
            {
                Annex1 = reader["Annex1"].ToString(),
                Annex2 = reader["Annex2"].ToString(),
                Content = reader["Content"].ToString(),
                Gold = (int)reader["Gold"],
                ID = (int)reader["ID"],
                IsExist = (bool)reader["IsExist"],
                Money = (int)reader["Money"],
                GiftToken = (int)reader["GiftToken"],
                Receiver = reader["Receiver"].ToString(),
                ReceiverID = (int)reader["ReceiverID"],
                Sender = reader["Sender"].ToString(),
                SenderID = (int)reader["SenderID"],
                Title = reader["Title"].ToString(),
                Type = (int)reader["Type"],
                ValidDate = (int)reader["ValidDate"],
                IsRead = (bool)reader["IsRead"],
                SendTime = (DateTime)reader["SendTime"],
                Annex1Name = reader["Annex1Name"] == null ? "" : reader["Annex1Name"].ToString(),
                Annex2Name = reader["Annex2Name"] == null ? "" : reader["Annex2Name"].ToString(),
                Annex3 = reader["Annex3"].ToString(),
                Annex4 = reader["Annex4"].ToString(),
                Annex5 = reader["Annex5"].ToString(),
                Annex3Name = reader["Annex3Name"] == null ? "" : reader["Annex3Name"].ToString(),
                Annex4Name = reader["Annex4Name"] == null ? "" : reader["Annex4Name"].ToString(),
                Annex5Name = reader["Annex5Name"] == null ? "" : reader["Annex5Name"].ToString(),
                AnnexRemark = reader["AnnexRemark"] == null ? "" : reader["AnnexRemark"].ToString()
            };
        }

        public MailInfo[] GetMailByUserID(int userID)
        {
            List<MailInfo> list = new List<MailInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)userID;
                this.db.GetReader(ref ResultDataReader, "SP_Mail_ByUserID", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitMail(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public MailInfo[] GetMailBySenderID(int userID)
        {
            List<MailInfo> list = new List<MailInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)userID;
                this.db.GetReader(ref ResultDataReader, "SP_Mail_BySenderID", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitMail(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public MailInfo GetMailSingle(int UserID, int mailID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) mailID),
          new SqlParameter("@UserID", (object) UserID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Mail_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitMail(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MailInfo)null;
        }

        public bool SendMail(MailInfo mail)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[29];
                SqlParameters[0] = new SqlParameter("@ID", (object)mail.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? (object)"" : (object)mail.Annex1);
                SqlParameters[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? (object)"" : (object)mail.Annex2);
                SqlParameters[3] = new SqlParameter("@Content", mail.Content == null ? (object)"" : (object)mail.Content);
                SqlParameters[4] = new SqlParameter("@Gold", (object)mail.Gold);
                SqlParameters[5] = new SqlParameter("@IsExist", (object)true);
                SqlParameters[6] = new SqlParameter("@Money", (object)mail.Money);
                SqlParameters[7] = new SqlParameter("@Receiver", mail.Receiver == null ? (object)"" : (object)mail.Receiver);
                SqlParameters[8] = new SqlParameter("@ReceiverID", (object)mail.ReceiverID);
                SqlParameters[9] = new SqlParameter("@Sender", mail.Sender == null ? (object)"" : (object)mail.Sender);
                SqlParameters[10] = new SqlParameter("@SenderID", (object)mail.SenderID);
                SqlParameters[11] = new SqlParameter("@Title", mail.Title == null ? (object)"" : (object)mail.Title);
                SqlParameters[12] = new SqlParameter("@IfDelS", (object)false);
                SqlParameters[13] = new SqlParameter("@IsDelete", (object)false);
                SqlParameters[14] = new SqlParameter("@IsDelR", (object)false);
                SqlParameters[15] = new SqlParameter("@IsRead", (object)false);
                SqlParameters[16] = new SqlParameter("@SendTime", (object)DateTime.Now);
                SqlParameters[17] = new SqlParameter("@Type", (object)mail.Type);
                SqlParameters[18] = new SqlParameter("@Annex1Name", mail.Annex1Name == null ? (object)"" : (object)mail.Annex1Name);
                SqlParameters[19] = new SqlParameter("@Annex2Name", mail.Annex2Name == null ? (object)"" : (object)mail.Annex2Name);
                SqlParameters[20] = new SqlParameter("@Annex3", mail.Annex3 == null ? (object)"" : (object)mail.Annex3);
                SqlParameters[21] = new SqlParameter("@Annex4", mail.Annex4 == null ? (object)"" : (object)mail.Annex4);
                SqlParameters[22] = new SqlParameter("@Annex5", mail.Annex5 == null ? (object)"" : (object)mail.Annex5);
                SqlParameters[23] = new SqlParameter("@Annex3Name", mail.Annex3Name == null ? (object)"" : (object)mail.Annex3Name);
                SqlParameters[24] = new SqlParameter("@Annex4Name", mail.Annex4Name == null ? (object)"" : (object)mail.Annex4Name);
                SqlParameters[25] = new SqlParameter("@Annex5Name", mail.Annex5Name == null ? (object)"" : (object)mail.Annex5Name);
                SqlParameters[26] = new SqlParameter("@ValidDate", (object)mail.ValidDate);
                SqlParameters[27] = new SqlParameter("@AnnexRemark", mail.AnnexRemark == null ? (object)"" : (object)mail.AnnexRemark);
                SqlParameters[28] = new SqlParameter("GiftToken", (object)mail.GiftToken);
                flag = this.db.RunProcedure("SP_Mail_Send", SqlParameters);
                mail.ID = (int)SqlParameters[0].Value;
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    centerServiceClient.MailNotice(mail.ReceiverID);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool DeleteMail(int UserID, int mailID, out int senderID)
        {
            bool flag = false;
            senderID = 0;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@ID", (object) mailID),
          new SqlParameter("@UserID", (object) UserID),
          new SqlParameter("@SenderID", SqlDbType.Int),
          null
        };
                SqlParameters[2].Value = (object)senderID;
                SqlParameters[2].Direction = ParameterDirection.InputOutput;
                SqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Mail_Delete", SqlParameters);
                if ((int)SqlParameters[3].Value == 0)
                {
                    flag = true;
                    senderID = (int)SqlParameters[2].Value;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateMail(MailInfo mail, int oldMoney)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[30]
        {
          new SqlParameter("@ID", (object) mail.ID),
          new SqlParameter("@Annex1", mail.Annex1 == null ? (object) "" : (object) mail.Annex1),
          new SqlParameter("@Annex2", mail.Annex2 == null ? (object) "" : (object) mail.Annex2),
          new SqlParameter("@Content", mail.Content == null ? (object) "" : (object) mail.Content),
          new SqlParameter("@Gold", (object) mail.Gold),
          new SqlParameter("@IsExist", (object) (int) (mail.IsExist ? 1 : 0)),
          new SqlParameter("@Money", (object) mail.Money),
          new SqlParameter("@Receiver", mail.Receiver == null ? (object) "" : (object) mail.Receiver),
          new SqlParameter("@ReceiverID", (object) mail.ReceiverID),
          new SqlParameter("@Sender", mail.Sender == null ? (object) "" : (object) mail.Sender),
          new SqlParameter("@SenderID", (object) mail.SenderID),
          new SqlParameter("@Title", mail.Title == null ? (object) "" : (object) mail.Title),
          new SqlParameter("@IfDelS", (object) false),
          new SqlParameter("@IsDelete", (object) false),
          new SqlParameter("@IsDelR", (object) false),
          new SqlParameter("@IsRead", (object) (int) (mail.IsRead ? 1 : 0)),
          new SqlParameter("@SendTime", (object) mail.SendTime),
          new SqlParameter("@Type", (object) mail.Type),
          new SqlParameter("@OldMoney", (object) oldMoney),
          new SqlParameter("@ValidDate", (object) mail.ValidDate),
          new SqlParameter("@Annex1Name", (object) mail.Annex1Name),
          new SqlParameter("@Annex2Name", (object) mail.Annex2Name),
          new SqlParameter("@Result", SqlDbType.Int),
          null,
          null,
          null,
          null,
          null,
          null,
          null
        };
                SqlParameters[22].Direction = ParameterDirection.ReturnValue;
                SqlParameters[23] = new SqlParameter("@Annex3", mail.Annex3 == null ? (object)"" : (object)mail.Annex3);
                SqlParameters[24] = new SqlParameter("@Annex4", mail.Annex4 == null ? (object)"" : (object)mail.Annex4);
                SqlParameters[25] = new SqlParameter("@Annex5", mail.Annex5 == null ? (object)"" : (object)mail.Annex5);
                SqlParameters[26] = new SqlParameter("@Annex3Name", mail.Annex3Name == null ? (object)"" : (object)mail.Annex3Name);
                SqlParameters[27] = new SqlParameter("@Annex4Name", mail.Annex4Name == null ? (object)"" : (object)mail.Annex4Name);
                SqlParameters[28] = new SqlParameter("@Annex5Name", mail.Annex5Name == null ? (object)"" : (object)mail.Annex5Name);
                SqlParameters[29] = new SqlParameter("GiftToken", (object)mail.GiftToken);
                this.db.RunProcedure("SP_Mail_Update", SqlParameters);
                flag = (int)SqlParameters[22].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@userid", (object) userid),
          new SqlParameter("@mailID", (object) mailID),
          new SqlParameter("@senderID", SqlDbType.Int),
          null
        };
                SqlParameters[2].Value = (object)senderID;
                SqlParameters[2].Direction = ParameterDirection.InputOutput;
                SqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Mail_PaymentCancel", SqlParameters);
                flag = (int)SqlParameters[3].Value == 0;
                if (flag)
                    senderID = (int)SqlParameters[2].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool ScanMail(ref string noticeUserID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
        };
                SqlParameters[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Mail_Scan", SqlParameters);
                noticeUserID = SqlParameters[0].Value.ToString();
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool SendMailAndItem(MailInfo mail, SqlDataProvider.Data.ItemInfo item, ref int returnValue)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[34]
        {
          new SqlParameter("@ItemID", (object) item.ItemID),
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@TemplateID", (object) item.TemplateID),
          new SqlParameter("@Place", (object) item.Place),
          new SqlParameter("@AgilityCompose", (object) item.AgilityCompose),
          new SqlParameter("@AttackCompose", (object) item.AttackCompose),
          new SqlParameter("@BeginDate", (object) item.BeginDate),
          new SqlParameter("@Color", item.Color == null ? (object) "" : (object) item.Color),
          new SqlParameter("@Count", (object) item.Count),
          new SqlParameter("@DefendCompose", (object) item.DefendCompose),
          new SqlParameter("@IsBinds", (object) (int) (item.IsBinds ? 1 : 0)),
          new SqlParameter("@IsExist", (object) (int) (item.IsExist ? 1 : 0)),
          new SqlParameter("@IsJudge", (object) (int) (item.IsJudge ? 1 : 0)),
          new SqlParameter("@LuckCompose", (object) item.LuckCompose),
          new SqlParameter("@StrengthenLevel", (object) item.StrengthenLevel),
          new SqlParameter("@ValidDate", (object) item.ValidDate),
          new SqlParameter("@BagType", (object) item.BagType),
          new SqlParameter("@ID", (object) mail.ID),
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null
        };
                SqlParameters[17].Direction = ParameterDirection.Output;
                SqlParameters[18] = new SqlParameter("@Annex1", mail.Annex1 == null ? (object)"" : (object)mail.Annex1);
                SqlParameters[19] = new SqlParameter("@Annex2", mail.Annex2 == null ? (object)"" : (object)mail.Annex2);
                SqlParameters[20] = new SqlParameter("@Content", mail.Content == null ? (object)"" : (object)mail.Content);
                SqlParameters[21] = new SqlParameter("@Gold", (object)mail.Gold);
                SqlParameters[22] = new SqlParameter("@Money", (object)mail.Money);
                SqlParameters[23] = new SqlParameter("@Receiver", mail.Receiver == null ? (object)"" : (object)mail.Receiver);
                SqlParameters[24] = new SqlParameter("@ReceiverID", (object)mail.ReceiverID);
                SqlParameters[25] = new SqlParameter("@Sender", mail.Sender == null ? (object)"" : (object)mail.Sender);
                SqlParameters[26] = new SqlParameter("@SenderID", (object)mail.SenderID);
                SqlParameters[27] = new SqlParameter("@Title", mail.Title == null ? (object)"" : (object)mail.Title);
                SqlParameters[28] = new SqlParameter("@IfDelS", (object)false);
                SqlParameters[29] = new SqlParameter("@IsDelete", (object)false);
                SqlParameters[30] = new SqlParameter("@IsDelR", (object)false);
                SqlParameters[31] = new SqlParameter("@IsRead", (object)false);
                SqlParameters[32] = new SqlParameter("@SendTime", (object)DateTime.Now);
                SqlParameters[33] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[33].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Admin_SendUserItem", SqlParameters);
                returnValue = (int)SqlParameters[33].Value;
                flag = returnValue == 0;
                if (flag)
                {
                    using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                        centerServiceClient.MailNotice(mail.ReceiverID);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[18];
                SqlParameters[0] = new SqlParameter("@ID", (object)mail.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? (object)"" : (object)mail.Annex1);
                SqlParameters[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? (object)"" : (object)mail.Annex2);
                SqlParameters[3] = new SqlParameter("@Content", mail.Content == null ? (object)"" : (object)mail.Content);
                SqlParameters[4] = new SqlParameter("@Gold", (object)mail.Gold);
                SqlParameters[5] = new SqlParameter("@IsExist", (object)true);
                SqlParameters[6] = new SqlParameter("@Money", (object)mail.Money);
                SqlParameters[7] = new SqlParameter("@Receiver", mail.Receiver == null ? (object)"" : (object)mail.Receiver);
                SqlParameters[8] = new SqlParameter("@ReceiverID", (object)mail.ReceiverID);
                SqlParameters[9] = new SqlParameter("@Sender", mail.Sender == null ? (object)"" : (object)mail.Sender);
                SqlParameters[10] = new SqlParameter("@SenderID", (object)mail.SenderID);
                SqlParameters[11] = new SqlParameter("@Title", mail.Title == null ? (object)"" : (object)mail.Title);
                SqlParameters[12] = new SqlParameter("@IfDelS", (object)false);
                SqlParameters[13] = new SqlParameter("@IsDelete", (object)false);
                SqlParameters[14] = new SqlParameter("@IsDelR", (object)false);
                SqlParameters[15] = new SqlParameter("@IsRead", (object)false);
                SqlParameters[16] = new SqlParameter("@SendTime", (object)DateTime.Now);
                SqlParameters[17] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[17].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Admin_SendUserMoney", SqlParameters);
                returnValue = (int)SqlParameters[17].Value;
                flag = returnValue == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            MailInfo mail = new MailInfo();
            mail.Annex1 = "";
            mail.Content = title;
            mail.Gold = gold;
            mail.Money = money;
            mail.Receiver = "";
            mail.ReceiverID = UserID;
            mail.Sender = "Administrators";
            mail.SenderID = 0;
            mail.Title = content;
            SqlDataProvider.Data.ItemInfo itemInfo = new SqlDataProvider.Data.ItemInfo((ItemTemplateInfo)null);
            itemInfo.AgilityCompose = AgilityCompose;
            itemInfo.AttackCompose = AttackCompose;
            itemInfo.BeginDate = DateTime.Now;
            itemInfo.Color = "";
            itemInfo.DefendCompose = DefendCompose;
            itemInfo.IsDirty = false;
            itemInfo.IsExist = true;
            itemInfo.IsJudge = true;
            itemInfo.LuckCompose = LuckCompose;
            itemInfo.StrengthenLevel = StrengthenLevel;
            itemInfo.TemplateID = templateID;
            itemInfo.ValidDate = validDate;
            itemInfo.Count = count;
            itemInfo.IsBinds = isBinds;
            int returnValue = 1;
            this.SendMailAndItem(mail, itemInfo, ref returnValue);
            return returnValue;
        }

        public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo singleByUserName = this.GetUserSingleByUserName(userName);
            if (singleByUserName != null)
                return this.SendMailAndItem(title, content, singleByUserName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            else
                return 2;
        }

        public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo singleByNickName = this.GetUserSingleByNickName(NickName);
            if (singleByNickName != null)
                return this.SendMailAndItem(title, content, singleByNickName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            else
                return 2;
        }

        public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
        {
            int num = 1;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[8]
        {
          new SqlParameter("@Title", (object) title),
          new SqlParameter("@Content", (object) content),
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@Gold", (object) gold),
          new SqlParameter("@Money", (object) money),
          new SqlParameter("@GiftToken", SqlDbType.BigInt),
          new SqlParameter("@Param", (object) param),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[7].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Admin_SendAllItem", SqlParameters);
                num = (int)SqlParameters[7].Value;
                if (num == 0)
                {
                    using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                        centerServiceClient.MailNotice(userID);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return num;
        }

        public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, string param)
        {
            PlayerInfo singleByUserName = this.GetUserSingleByUserName(userName);
            if (singleByUserName != null)
                return this.SendMailAndItem(title, content, singleByUserName.ID, gold, money, param);
            else
                return 2;
        }

        public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, string param)
        {
            PlayerInfo singleByNickName = this.GetUserSingleByNickName(nickName);
            if (singleByNickName != null)
                return this.SendMailAndItem(title, content, singleByNickName.ID, gold, money, param);
            else
                return 2;
        }

        public Dictionary<int, int> GetFriendsIDAll(int UserID)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Friends_All", SqlParameters);
                while (ResultDataReader.Read())
                {
                    if (!dictionary.ContainsKey((int)ResultDataReader["FriendID"]))
                        dictionary.Add((int)ResultDataReader["FriendID"], (int)ResultDataReader["Relation"]);
                    else
                        dictionary[(int)ResultDataReader["FriendID"]] = (int)ResultDataReader["Relation"];
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return dictionary;
        }

        public bool AddFriends(FriendInfo info)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Friends_Add", new SqlParameter[7]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@AddDate", (object) DateTime.Now),
          new SqlParameter("@FriendID", (object) info.FriendID),
          new SqlParameter("@IsExist", (object) true),
          new SqlParameter("@Remark", info.Remark == null ? (object) "" : (object) info.Remark),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Relation", (object) info.Relation)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool DeleteFriends(int UserID, int FriendID)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Friends_Delete", new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) FriendID),
          new SqlParameter("@UserID", (object) UserID)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public FriendInfo[] GetFriendsAll(int UserID)
        {
            List<FriendInfo> list = new List<FriendInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Friends", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new FriendInfo()
                    {
                        AddDate = (DateTime)ResultDataReader["AddDate"],
                        Colors = ResultDataReader["Colors"] == null ? "" : ResultDataReader["Colors"].ToString(),
                        FriendID = (int)ResultDataReader["FriendID"],
                        Grade = (int)ResultDataReader["Grade"],
                        Hide = (int)ResultDataReader["Hide"],
                        ID = (int)ResultDataReader["ID"],
                        IsExist = (bool)ResultDataReader["IsExist"],
                        NickName = ResultDataReader["NickName"] == null ? "" : ResultDataReader["NickName"].ToString(),
                        Remark = ResultDataReader["Remark"] == null ? "" : ResultDataReader["Remark"].ToString(),
                        Sex = (bool)ResultDataReader["Sex"] ? 1 : 0,
                        State = (int)ResultDataReader["State"],
                        Style = ResultDataReader["Style"] == null ? "" : ResultDataReader["Style"].ToString(),
                        UserID = (int)ResultDataReader["UserID"],
                        ConsortiaName = ResultDataReader["ConsortiaName"] == null ? "" : ResultDataReader["ConsortiaName"].ToString(),
                        Offer = (int)ResultDataReader["Offer"],
                        Win = (int)ResultDataReader["Win"],
                        Total = (int)ResultDataReader["Total"],
                        Escape = (int)ResultDataReader["Escape"],
                        Relation = (int)ResultDataReader["Relation"],
                        Repute = (int)ResultDataReader["Repute"],
                        UserName = ResultDataReader["UserName"] == null ? "" : ResultDataReader["UserName"].ToString(),
                        DutyName = ResultDataReader["DutyName"] == null ? "" : ResultDataReader["DutyName"].ToString(),
                        Nimbus = (int)ResultDataReader["Nimbus"],
                        typeVIP = Convert.ToByte(ResultDataReader["typeVIP"]),
                        VIPLevel = (int)ResultDataReader["VIPLevel"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ArrayList GetFriendsGood(string UserName)
        {
            ArrayList arrayList = new ArrayList();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserName", SqlDbType.VarChar)
        };
                SqlParameters[0].Value = (object)UserName;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Friends_Good", SqlParameters);
                while (ResultDataReader.Read())
                    arrayList.Add(ResultDataReader["UserName"] == null ? (object)"" : (object)ResultDataReader["UserName"].ToString());
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return arrayList;
        }

        public FriendInfo[] GetFriendsBbs(string condictArray)
        {
            List<FriendInfo> list = new List<FriendInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
        };
                SqlParameters[0].Value = (object)condictArray;
                this.db.GetReader(ref ResultDataReader, "SP_Users_FriendsBbs", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new FriendInfo()
                    {
                        NickName = ResultDataReader["NickName"] == null ? "" : ResultDataReader["NickName"].ToString(),
                        UserID = (int)ResultDataReader["UserID"],
                        UserName = ResultDataReader["UserName"] == null ? "" : ResultDataReader["UserName"].ToString(),
                        IsExist = (int)ResultDataReader["UserID"] > 0
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public QuestDataInfo[] GetUserQuest(int userID)
        {
            List<QuestDataInfo> list = new List<QuestDataInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)userID;
                this.db.GetReader(ref ResultDataReader, "SP_QuestData_All", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new QuestDataInfo()
                    {
                        CompletedDate = (DateTime)ResultDataReader["CompletedDate"],
                        IsComplete = (bool)ResultDataReader["IsComplete"],
                        Condition1 = (int)ResultDataReader["Condition1"],
                        Condition2 = (int)ResultDataReader["Condition2"],
                        Condition3 = (int)ResultDataReader["Condition3"],
                        Condition4 = (int)ResultDataReader["Condition4"],
                        QuestID = (int)ResultDataReader["QuestID"],
                        UserID = (int)ResultDataReader["UserId"],
                        IsExist = (bool)ResultDataReader["IsExist"],
                        RandDobule = (int)ResultDataReader["RandDobule"],
                        RepeatFinish = (int)ResultDataReader["RepeatFinish"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool UpdateDbQuestDataInfo(QuestDataInfo info)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_QuestData_Add", new SqlParameter[11]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@QuestID", (object) info.QuestID),
          new SqlParameter("@CompletedDate", (object) info.CompletedDate),
          new SqlParameter("@IsComplete", (object) (int) (info.IsComplete ? 1 : 0)),
          new SqlParameter("@Condition1", (object) info.Condition1),
          new SqlParameter("@Condition2", (object) info.Condition2),
          new SqlParameter("@Condition3", (object) info.Condition3),
          new SqlParameter("@Condition4", (object) info.Condition4),
          new SqlParameter("@IsExist", (object) (int) (info.IsExist ? 1 : 0)),
          new SqlParameter("@RepeatFinish", (object) info.RepeatFinish),
          new SqlParameter("@RandDobule", (object) info.RandDobule)
        });
                info.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public BufferInfo[] GetUserBuffer(int userID)
        {
            List<BufferInfo> list1 = new List<BufferInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)userID;
                this.db.GetReader(ref ResultDataReader, "SP_User_Buff_All", SqlParameters);
                while (ResultDataReader.Read())
                {
                    List<BufferInfo> list2 = list1;
                    BufferInfo bufferInfo1 = new BufferInfo();
                    bufferInfo1.BeginDate = (DateTime)ResultDataReader["BeginDate"];
                    bufferInfo1.Data = ResultDataReader["Data"] == null ? "" : ResultDataReader["Data"].ToString();
                    bufferInfo1.Type = (int)ResultDataReader["Type"];
                    bufferInfo1.UserID = (int)ResultDataReader["UserID"];
                    bufferInfo1.ValidDate = (int)ResultDataReader["ValidDate"];
                    bufferInfo1.Value = (int)ResultDataReader["Value"];
                    bufferInfo1.IsExist = (bool)ResultDataReader["IsExist"];
                    bufferInfo1.ValidCount = (int)ResultDataReader["ValidCount"];
                    bufferInfo1.TemplateID = (int)ResultDataReader["TemplateID"];
                    bufferInfo1.IsDirty = false;
                    BufferInfo bufferInfo2 = bufferInfo1;
                    list2.Add(bufferInfo2);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list1.ToArray();
        }

        public bool SaveBuffer(BufferInfo info)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_User_Buff_Add", new SqlParameter[9]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Type", (object) info.Type),
          new SqlParameter("@BeginDate", (object) info.BeginDate),
          new SqlParameter("@Data", info.Data == null ? (object) "" : (object) info.Data),
          new SqlParameter("@IsExist", (object) (int) (info.IsExist ? 1 : 0)),
          new SqlParameter("@ValidDate", (object) info.ValidDate),
          new SqlParameter("@Value", (object) info.Value),
          new SqlParameter("@Value", (object) info.ValidCount),
          new SqlParameter("@TemplateID", (object) info.TemplateID)
        });
                info.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, Decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
        {
            bool flag = false;
            userID = 0;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[10]
        {
          new SqlParameter("@ChargeID", (object) chargeID),
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@Money", (object) money),
          new SqlParameter("@Date", (object) date.ToString("yyyy-MM-dd HH:mm:ss")),
          new SqlParameter("@PayWay", (object) payWay),
          new SqlParameter("@NeedMoney", (object) needMoney),
          new SqlParameter("@UserID", (object) userID),
          null,
          null,
          null
        };
                SqlParameters[6].Direction = ParameterDirection.InputOutput;
                SqlParameters[7] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[7].Direction = ParameterDirection.ReturnValue;
                SqlParameters[8] = new SqlParameter("@IP", (object)IP);
                SqlParameters[9] = new SqlParameter("@NickName", (object)nickName);
                flag = this.db.RunProcedure("SP_Charge_Money_Add", SqlParameters);
                userID = (int)SqlParameters[6].Value;
                isResult = (int)SqlParameters[7].Value;
                flag = isResult == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool ChargeToUser(string userName, ref int money, string nickName)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@money", SqlDbType.Int),
          null
        };
                SqlParameters[1].Direction = ParameterDirection.Output;
                SqlParameters[2] = new SqlParameter("@NickName", (object)nickName);
                flag = this.db.RunProcedure("SP_Charge_To_User", SqlParameters);
                money = (int)SqlParameters[1].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
        {
            List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@Date", (object) date.ToString("yyyy-MM-dd HH:mm:ss")),
          new SqlParameter("@Second", (object) SaveRecordSecond)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Charge_Record", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new ChargeRecordInfo()
                    {
                        BoyTotalPay = (int)ResultDataReader["BoyTotalPay"],
                        GirlTotalPay = (int)ResultDataReader["GirlTotalPay"],
                        PayWay = ResultDataReader["PayWay"] == null ? "" : ResultDataReader["PayWay"].ToString(),
                        TotalBoy = (int)ResultDataReader["TotalBoy"],
                        TotalGirl = (int)ResultDataReader["TotalGirl"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public AuctionInfo GetAuctionSingle(int auctionID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@AuctionID", (object) auctionID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Auction_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitAuctionInfo(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (AuctionInfo)null;
        }

        public bool AddAuction(AuctionInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[18];
                SqlParameters[0] = new SqlParameter("@AuctionID", (object)info.AuctionID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@AuctioneerID", (object)info.AuctioneerID);
                SqlParameters[2] = new SqlParameter("@AuctioneerName", info.AuctioneerName == null ? (object)"" : (object)info.AuctioneerName);
                SqlParameters[3] = new SqlParameter("@BeginDate", (object)info.BeginDate);
                SqlParameters[4] = new SqlParameter("@BuyerID", (object)info.BuyerID);
                SqlParameters[5] = new SqlParameter("@BuyerName", info.BuyerName == null ? (object)"" : (object)info.BuyerName);
                SqlParameters[6] = new SqlParameter("@IsExist", (object)(int)(info.IsExist ? 1 : 0));
                SqlParameters[7] = new SqlParameter("@ItemID", (object)info.ItemID);
                SqlParameters[8] = new SqlParameter("@Mouthful", (object)info.Mouthful);
                SqlParameters[9] = new SqlParameter("@PayType", (object)info.PayType);
                SqlParameters[10] = new SqlParameter("@Price", (object)info.Price);
                SqlParameters[11] = new SqlParameter("@Rise", (object)info.Rise);
                SqlParameters[12] = new SqlParameter("@ValidDate", (object)info.ValidDate);
                SqlParameters[13] = new SqlParameter("@TemplateID", (object)info.TemplateID);
                SqlParameters[14] = new SqlParameter("Name", (object)info.Name);
                SqlParameters[15] = new SqlParameter("Category", (object)info.Category);
                SqlParameters[16] = new SqlParameter("Random", (object)info.Random);
                SqlParameters[17] = new SqlParameter("goodsCount", (object)info.goodsCount);
                flag = this.db.RunProcedure("SP_Auction_Add", SqlParameters);
                info.AuctionID = (int)SqlParameters[0].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateAuction(AuctionInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[16]
        {
          new SqlParameter("@AuctionID", (object) info.AuctionID),
          new SqlParameter("@AuctioneerID", (object) info.AuctioneerID),
          new SqlParameter("@AuctioneerName", info.AuctioneerName == null ? (object) "" : (object) info.AuctioneerName),
          new SqlParameter("@BeginDate", (object) info.BeginDate),
          new SqlParameter("@BuyerID", (object) info.BuyerID),
          new SqlParameter("@BuyerName", info.BuyerName == null ? (object) "" : (object) info.BuyerName),
          new SqlParameter("@IsExist", (object) (int) (info.IsExist ? 1 : 0)),
          new SqlParameter("@ItemID", (object) info.ItemID),
          new SqlParameter("@Mouthful", (object) info.Mouthful),
          new SqlParameter("@PayType", (object) info.PayType),
          new SqlParameter("@Price", (object) info.Price),
          new SqlParameter("@Rise", (object) info.Rise),
          new SqlParameter("@ValidDate", (object) info.ValidDate),
          new SqlParameter("Name", (object) info.Name),
          new SqlParameter("Category", (object) info.Category),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[15].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Update", SqlParameters);
                flag = (int)SqlParameters[15].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool DeleteAuction(int auctionID, int userID, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@AuctionID", (object) auctionID),
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Delete", SqlParameters);
                int num = (int)SqlParameters[2].Value;
                flag = num == 0;
                switch (num)
                {
                    case 0:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1", new object[0]);
                        break;
                    case 1:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2", new object[0]);
                        break;
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3", new object[0]);
                        break;
                    default:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
        {
            List<AuctionInfo> list = new List<AuctionInfo>();
            try
            {
                string str1 = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                    str1 = str1 + " and Name like '%" + name + "%' ";
                switch (type)
                {
                    case -1:
                        if (pay != -1)
                            str1 = string.Concat(new object[4]
              {
                (object) str1,
                (object) " and PayType =",
                (object) pay,
                (object) " "
              });
                        if (userID != -1)
                            str1 = string.Concat(new object[4]
              {
                (object) str1,
                (object) " and AuctioneerID =",
                (object) userID,
                (object) " "
              });
                        if (buyID != -1)
                            str1 = str1 + (object)" and (BuyerID =" + (string)(object)buyID + " or AuctionID in (" + AuctionIDs + ")) ";
                        string str2 = "Category,Name,Price,dd,AuctioneerID";
                        switch (order)
                        {
                            case 0:
                                str2 = "Name";
                                break;
                            case 2:
                                str2 = "dd";
                                break;
                            case 3:
                                str2 = "AuctioneerName";
                                break;
                            case 4:
                                str2 = "Price";
                                break;
                            case 5:
                                str2 = "BuyerName";
                                break;
                        }
                        string str3 = str2 + (sort ? " desc" : "") + ",AuctionID ";
                        SqlParameter[] SqlParameters = new SqlParameter[8]
            {
              new SqlParameter("@QueryStr", (object) "V_Auction_Scan"),
              new SqlParameter("@QueryWhere", (object) str1),
              new SqlParameter("@PageSize", (object) size),
              new SqlParameter("@PageCurrent", (object) page),
              new SqlParameter("@FdShow", (object) "*"),
              new SqlParameter("@FdOrder", (object) str3),
              new SqlParameter("@FdKey", (object) "AuctionID"),
              new SqlParameter("@TotalRow", (object) total)
            };
                        SqlParameters[7].Direction = ParameterDirection.Output;
                        DataTable dataTable = this.db.GetDataTable("Auction", "SP_CustomPage", SqlParameters);
                        total = (int)SqlParameters[7].Value;
                        IEnumerator enumerator = dataTable.Rows.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                DataRow dataRow = (DataRow)enumerator.Current;
                                list.Add(new AuctionInfo()
                                {
                                    AuctioneerID = (int)dataRow["AuctioneerID"],
                                    AuctioneerName = dataRow["AuctioneerName"].ToString(),
                                    AuctionID = (int)dataRow["AuctionID"],
                                    BeginDate = (DateTime)dataRow["BeginDate"],
                                    BuyerID = (int)dataRow["BuyerID"],
                                    BuyerName = dataRow["BuyerName"].ToString(),
                                    Category = (int)dataRow["Category"],
                                    IsExist = (bool)dataRow["IsExist"],
                                    ItemID = (int)dataRow["ItemID"],
                                    Name = dataRow["Name"].ToString(),
                                    Mouthful = (int)dataRow["Mouthful"],
                                    PayType = (int)dataRow["PayType"],
                                    Price = (int)dataRow["Price"],
                                    Rise = (int)dataRow["Rise"],
                                    ValidDate = (int)dataRow["ValidDate"],
                                    goodsCount = (int)dataRow["dd"]
                                });
                            }
                            goto label_46;
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                                disposable.Dispose();
                        }
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        str1 = string.Concat(new object[4]
            {
              (object) str1,
              (object) " and Category =",
              (object) type,
              (object) " "
            });
                        break;
                    case 21:
                        str1 = str1 + " and Category in(1,2,5,8,9) ";
                        break;
                    case 22:
                        str1 = str1 + " and Category in(13,15,6,4,3) ";
                        break;
                    case 23:
                        str1 = str1 + " and Category in(16,11,10) ";
                        break;
                    case 24:
                        str1 = str1 + " and Category in(8,9) ";
                        break;
                    case 25:
                        str1 = str1 + " and Category in (7,17) ";
                        break;
                    case 26:
                        str1 = str1 + " and TemplateId>=311000 and TemplateId<=313999";
                        break;
                    case 27:
                        str1 = str1 + " and TemplateId>=311000 and TemplateId<=311999 ";
                        break;
                    case 28:
                        str1 = str1 + " and TemplateId>=312000 and TemplateId<=312999 ";
                        break;
                    case 29:
                        str1 = str1 + " and TemplateId>=313000 and TempLateId<=313999";
                        break;
                    case 1100:
                        str1 = str1 + " and TemplateID in (11019,11021,11022,11023) ";
                        break;
                    case 1101:
                        str1 = str1 + " and TemplateID='11019' ";
                        break;
                    case 1102:
                        str1 = str1 + " and TemplateID='11021' ";
                        break;
                    case 1103:
                        str1 = str1 + " and TemplateID='11022' ";
                        break;
                    case 1104:
                        str1 = str1 + " and TemplateID='11023' ";
                        break;
                    case 1105:
                        str1 = str1 + " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
                        break;
                    case 1106:
                        str1 = str1 + " and TemplateID in (11001,11002,11003,11004) ";
                        break;
                    case 1107:
                        str1 = str1 + " and TemplateID in (11005,11006,11007,11008) ";
                        break;
                    case 1108:
                        str1 = str1 + " and TemplateID in (11009,11010,11011,11012) ";
                        break;
                    case 1109:
                        str1 = str1 + " and TemplateID in (11013,11014,11015,11016) ";
                        break;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
        label_46:
            return list.ToArray();
        }

        public AuctionInfo InitAuctionInfo(SqlDataReader reader)
        {
            return new AuctionInfo()
            {
                AuctioneerID = (int)reader["AuctioneerID"],
                AuctioneerName = reader["AuctioneerName"] == null ? "" : reader["AuctioneerName"].ToString(),
                AuctionID = (int)reader["AuctionID"],
                BeginDate = (DateTime)reader["BeginDate"],
                BuyerID = (int)reader["BuyerID"],
                BuyerName = reader["BuyerName"] == null ? "" : reader["BuyerName"].ToString(),
                IsExist = (bool)reader["IsExist"],
                ItemID = (int)reader["ItemID"],
                Mouthful = (int)reader["Mouthful"],
                PayType = (int)reader["PayType"],
                Price = (int)reader["Price"],
                Rise = (int)reader["Rise"],
                ValidDate = (int)reader["ValidDate"],
                Name = reader["Name"].ToString(),
                Category = (int)reader["Category"],
                goodsCount = (int)reader["goodsCount"]
            };
        }

        public bool ScanAuction(ref string noticeUserID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
        };
                SqlParameters[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Auction_Scan", SqlParameters);
                noticeUserID = SqlParameters[0].Value.ToString();
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool AddMarryInfo(MarryInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[5];
                SqlParameters[0] = new SqlParameter("@ID", (object)info.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)info.UserID);
                SqlParameters[2] = new SqlParameter("@IsPublishEquip", (object)(int)(info.IsPublishEquip ? 1 : 0));
                SqlParameters[3] = new SqlParameter("@Introduction", (object)info.Introduction);
                SqlParameters[4] = new SqlParameter("@RegistTime", (object)info.RegistTime);
                flag = this.db.RunProcedure("SP_MarryInfo_Add", SqlParameters);
                info.ID = (int)SqlParameters[0].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"AddMarryInfo", ex);
            }
            return flag;
        }

        public bool DeleteMarryInfo(int ID, int userID, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@ID", (object) ID),
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Delete", SqlParameters);
                int num = (int)SqlParameters[2].Value;
                flag = num == 0;
                if (num == 0)
                    msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed", new object[0]);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"DeleteAuction", ex);
            }
            return flag;
        }

        public MarryInfo GetMarryInfoSingle(int ID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", (object) ID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_MarryInfo_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return new MarryInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        IsPublishEquip = (bool)ResultDataReader["IsPublishEquip"],
                        Introduction = ResultDataReader["Introduction"].ToString(),
                        RegistTime = (DateTime)ResultDataReader["RegistTime"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetMarryInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MarryInfo)null;
        }

        public bool UpdateMarryInfo(MarryInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@IsPublishEquip", (object) (int) (info.IsPublishEquip ? 1 : 0)),
          new SqlParameter("@Introduction", (object) info.Introduction),
          new SqlParameter("@RegistTime", (object) info.RegistTime),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Update", SqlParameters);
                flag = (int)SqlParameters[5].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> list = new List<MarryInfo>();
            try
            {
                string str1 = !sex ? " IsExist=1 and Sex=0 and UserExist=1" : " IsExist=1 and Sex=1 and UserExist=1";
                if (!string.IsNullOrEmpty(name))
                    str1 = str1 + " and NickName like '%" + name + "%' ";
                string str2 = "State desc,IsMarried";
                SqlParameter[] SqlParameters = new SqlParameter[8]
        {
          new SqlParameter("@QueryStr", (object) "V_Sys_Marry_Info"),
          new SqlParameter("@QueryWhere", (object) str1),
          new SqlParameter("@PageSize", (object) size),
          new SqlParameter("@PageCurrent", (object) page),
          new SqlParameter("@FdShow", (object) "*"),
          new SqlParameter("@FdOrder", (object) str2),
          new SqlParameter("@FdKey", (object) "ID"),
          new SqlParameter("@TotalRow", (object) total)
        };
                SqlParameters[7].Direction = ParameterDirection.Output;
                DataTable dataTable = this.db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", SqlParameters);
                total = (int)SqlParameters[7].Value;
                foreach (DataRow dataRow in (InternalDataCollectionBase)dataTable.Rows)
                    list.Add(new MarryInfo()
                    {
                        ID = (int)dataRow["ID"],
                        UserID = (int)dataRow["UserID"],
                        IsPublishEquip = (bool)dataRow["IsPublishEquip"],
                        Introduction = dataRow["Introduction"].ToString(),
                        NickName = dataRow["NickName"].ToString(),
                        IsConsortia = (bool)dataRow["IsConsortia"],
                        ConsortiaID = (int)dataRow["ConsortiaID"],
                        Sex = (bool)dataRow["Sex"],
                        Win = (int)dataRow["Win"],
                        Total = (int)dataRow["Total"],
                        Escape = (int)dataRow["Escape"],
                        GP = (int)dataRow["GP"],
                        Honor = dataRow["Honor"].ToString(),
                        Style = dataRow["Style"].ToString(),
                        Colors = dataRow["Colors"].ToString(),
                        Hide = (int)dataRow["Hide"],
                        Grade = (int)dataRow["Grade"],
                        State = (int)dataRow["State"],
                        Repute = (int)dataRow["Repute"],
                        Skin = dataRow["Skin"].ToString(),
                        Offer = (int)dataRow["Offer"],
                        IsMarried = (bool)dataRow["IsMarried"],
                        ConsortiaName = dataRow["ConsortiaName"].ToString(),
                        DutyName = dataRow["DutyName"].ToString(),
                        Nimbus = (int)dataRow["Nimbus"],
                        FightPower = (int)dataRow["FightPower"],
                        typeVIP = Convert.ToByte(dataRow["typeVIP"]),
                        VIPLevel = (int)dataRow["VIPLevel"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return list.ToArray();
        }

        public bool InsertPlayerMarryApply(MarryApplyInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[7]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@ApplyUserID", (object) info.ApplyUserID),
          new SqlParameter("@ApplyUserName", (object) info.ApplyUserName),
          new SqlParameter("@ApplyType", (object) info.ApplyType),
          new SqlParameter("@ApplyResult", (object) (int) (info.ApplyResult ? 1 : 0)),
          new SqlParameter("@LoveProclamation", (object) info.LoveProclamation),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Apply", SqlParameters);
                flag = (int)SqlParameters[6].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"InsertPlayerMarryApply", ex);
            }
            return flag;
        }

        public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@UserID", (object) UserID),
          new SqlParameter("@LoveProclamation", (object) loveProclamation),
          new SqlParameter("@isExist", (object) (int) (isExist ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Apply", SqlParameters);
                flag = (int)SqlParameters[3].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdatePlayerMarryApply", ex);
            }
            return flag;
        }

        public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<MarryApplyInfo> list = new List<MarryApplyInfo>();
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) UserID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_Marry_Apply", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new MarryApplyInfo()
                    {
                        UserID = (int)ResultDataReader["UserID"],
                        ApplyUserID = (int)ResultDataReader["ApplyUserID"],
                        ApplyUserName = ResultDataReader["ApplyUserName"].ToString(),
                        ApplyType = (int)ResultDataReader["ApplyType"],
                        ApplyResult = (bool)ResultDataReader["ApplyResult"],
                        LoveProclamation = ResultDataReader["LoveProclamation"].ToString(),
                        ID = (int)ResultDataReader["Id"]
                    });
                return list.ToArray();
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetPlayerMarryApply", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MarryApplyInfo[])null;
        }

        public bool InsertMarryRoomInfo(MarryRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[20];
                SqlParameters[0] = new SqlParameter("@ID", (object)info.ID);
                SqlParameters[0].Direction = ParameterDirection.InputOutput;
                SqlParameters[1] = new SqlParameter("@Name", (object)info.Name);
                SqlParameters[2] = new SqlParameter("@PlayerID", (object)info.PlayerID);
                SqlParameters[3] = new SqlParameter("@PlayerName", (object)info.PlayerName);
                SqlParameters[4] = new SqlParameter("@GroomID", (object)info.GroomID);
                SqlParameters[5] = new SqlParameter("@GroomName", (object)info.GroomName);
                SqlParameters[6] = new SqlParameter("@BrideID", (object)info.BrideID);
                SqlParameters[7] = new SqlParameter("@BrideName", (object)info.BrideName);
                SqlParameters[8] = new SqlParameter("@Pwd", (object)info.Pwd);
                SqlParameters[9] = new SqlParameter("@AvailTime", (object)info.AvailTime);
                SqlParameters[10] = new SqlParameter("@MaxCount", (object)info.MaxCount);
                SqlParameters[11] = new SqlParameter("@GuestInvite", (object)(int)(info.GuestInvite ? 1 : 0));
                SqlParameters[12] = new SqlParameter("@MapIndex", (object)info.MapIndex);
                SqlParameters[13] = new SqlParameter("@BeginTime", (object)info.BeginTime);
                SqlParameters[14] = new SqlParameter("@BreakTime", (object)info.BreakTime);
                SqlParameters[15] = new SqlParameter("@RoomIntroduction", (object)info.RoomIntroduction);
                SqlParameters[16] = new SqlParameter("@ServerID", (object)info.ServerID);
                SqlParameters[17] = new SqlParameter("@IsHymeneal", (object)(int)(info.IsHymeneal ? 1 : 0));
                SqlParameters[18] = new SqlParameter("@IsGunsaluteUsed", (object)(int)(info.IsGunsaluteUsed ? 1 : 0));
                SqlParameters[19] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[19].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Room_Info", SqlParameters);
                flag = (int)SqlParameters[19].Value == 0;
                if (flag)
                    info.ID = (int)SqlParameters[0].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"InsertMarryRoomInfo", ex);
            }
            return flag;
        }

        public bool UpdateMarryRoomInfo(MarryRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@AvailTime", (object) info.AvailTime),
          new SqlParameter("@BreakTime", (object) info.BreakTime),
          new SqlParameter("@roomIntroduction", (object) info.RoomIntroduction),
          new SqlParameter("@isHymeneal", (object) (int) (info.IsHymeneal ? 1 : 0)),
          new SqlParameter("@Name", (object) info.Name),
          new SqlParameter("@Pwd", (object) info.Pwd),
          new SqlParameter("@IsGunsaluteUsed", (object) (int) (info.IsGunsaluteUsed ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info", SqlParameters);
                flag = (int)SqlParameters[8].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateMarryRoomInfo", ex);
            }
            return flag;
        }

        public bool DisposeMarryRoomInfo(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Dispose_Marry_Room_Info", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"DisposeMarryRoomInfo", ex);
            }
            return flag;
        }

        public MarryRoomInfo[] GetMarryRoomInfo()
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<MarryRoomInfo> list = new List<MarryRoomInfo>();
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info");
                while (ResultDataReader.Read())
                    list.Add(new MarryRoomInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        PlayerID = (int)ResultDataReader["PlayerID"],
                        PlayerName = ResultDataReader["PlayerName"].ToString(),
                        GroomID = (int)ResultDataReader["GroomID"],
                        GroomName = ResultDataReader["GroomName"].ToString(),
                        BrideID = (int)ResultDataReader["BrideID"],
                        BrideName = ResultDataReader["BrideName"].ToString(),
                        Pwd = ResultDataReader["Pwd"].ToString(),
                        AvailTime = (int)ResultDataReader["AvailTime"],
                        MaxCount = (int)ResultDataReader["MaxCount"],
                        GuestInvite = (bool)ResultDataReader["GuestInvite"],
                        MapIndex = (int)ResultDataReader["MapIndex"],
                        BeginTime = (DateTime)ResultDataReader["BeginTime"],
                        BreakTime = (DateTime)ResultDataReader["BreakTime"],
                        RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString(),
                        ServerID = (int)ResultDataReader["ServerID"],
                        IsHymeneal = (bool)ResultDataReader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)ResultDataReader["IsGunsaluteUsed"]
                    });
                return list.ToArray();
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetMarryRoomInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MarryRoomInfo[])null;
        }

        public MarryRoomInfo GetMarryRoomInfoSingle(int id)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", (object) id)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return new MarryRoomInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        PlayerID = (int)ResultDataReader["PlayerID"],
                        PlayerName = ResultDataReader["PlayerName"].ToString(),
                        GroomID = (int)ResultDataReader["GroomID"],
                        GroomName = ResultDataReader["GroomName"].ToString(),
                        BrideID = (int)ResultDataReader["BrideID"],
                        BrideName = ResultDataReader["BrideName"].ToString(),
                        Pwd = ResultDataReader["Pwd"].ToString(),
                        AvailTime = (int)ResultDataReader["AvailTime"],
                        MaxCount = (int)ResultDataReader["MaxCount"],
                        GuestInvite = (bool)ResultDataReader["GuestInvite"],
                        MapIndex = (int)ResultDataReader["MapIndex"],
                        BeginTime = (DateTime)ResultDataReader["BeginTime"],
                        BreakTime = (DateTime)ResultDataReader["BreakTime"],
                        RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString(),
                        ServerID = (int)ResultDataReader["ServerID"],
                        IsHymeneal = (bool)ResultDataReader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)ResultDataReader["IsGunsaluteUsed"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetMarryRoomInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MarryRoomInfo)null;
        }

        public bool UpdateBreakTimeWhereServerStop()
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[0].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", SqlParameters);
                flag = (int)SqlParameters[0].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateBreakTimeWhereServerStop", ex);
            }
            return flag;
        }

        public MarryProp GetMarryProp(int id)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) id)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Select_Marry_Prop", SqlParameters);
                if (ResultDataReader.Read())
                    return new MarryProp()
                    {
                        IsMarried = (bool)ResultDataReader["IsMarried"],
                        SpouseID = (int)ResultDataReader["SpouseID"],
                        SpouseName = ResultDataReader["SpouseName"].ToString(),
                        IsCreatedMarryRoom = (bool)ResultDataReader["IsCreatedMarryRoom"],
                        SelfMarryRoomID = (int)ResultDataReader["SelfMarryRoomID"],
                        IsGotRing = (bool)ResultDataReader["IsGotRing"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetMarryProp", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (MarryProp)null;
        }

        public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@ApplyUserID", (object) info.ApplyUserID),
          new SqlParameter("@ApplyUserName", (object) info.ApplyUserName),
          new SqlParameter("@ApplyType", (object) info.ApplyType),
          new SqlParameter("@ApplyResult", (object) (int) (info.ApplyResult ? 1 : 0)),
          new SqlParameter("@LoveProclamation", (object) info.LoveProclamation),
          new SqlParameter("@AnswerId", (object) answerId),
          new SqlParameter("@ouototal", SqlDbType.Int),
          null
        };
                SqlParameters[7].Direction = ParameterDirection.Output;
                SqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Notice", SqlParameters);
                id = (int)SqlParameters[7].Value;
                flag = (int)SqlParameters[8].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SavePlayerMarryNotice", ex);
            }
            return flag;
        }

        public bool UpdatePlayerGotRingProp(int groomID, int brideID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@GroomID", (object) groomID),
          new SqlParameter("@BrideID", (object) brideID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_GotRing_Prop", SqlParameters);
                flag = (int)SqlParameters[2].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdatePlayerGotRingProp", ex);
            }
            return flag;
        }

        public HotSpringRoomInfo[] GetHotSpringRoomInfo()
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<HotSpringRoomInfo> list = new List<HotSpringRoomInfo>();
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Get_HotSpring_Room");
                while (ResultDataReader.Read())
                {
                    HotSpringRoomInfo hotSpringRoomInfo = new HotSpringRoomInfo()
                    {
                        RoomID = (int)ResultDataReader["RoomID"],
                        RoomName = ResultDataReader["RoomName"] == null ? "" : ResultDataReader["RoomName"].ToString(),
                        PlayerID = (int)ResultDataReader["PlayerID"],
                        PlayerName = ResultDataReader["PlayerName"] == null ? "" : ResultDataReader["PlayerName"].ToString(),
                        Pwd = ResultDataReader["Pwd"].ToString() == null ? "" : ResultDataReader["Pwd"].ToString(),
                        AvailTime = (int)ResultDataReader["AvailTime"],
                        MaxCount = (int)ResultDataReader["MaxCount"],
                        BeginTime = (DateTime)ResultDataReader["BeginTime"],
                        BreakTime = (DateTime)ResultDataReader["BreakTime"],
                        RoomIntroduction = ResultDataReader["RoomIntroduction"] == null ? "" : ResultDataReader["RoomIntroduction"].ToString(),
                        RoomType = (int)ResultDataReader["RoomType"],
                        ServerID = (int)ResultDataReader["ServerID"],
                        RoomNumber = (int)ResultDataReader["RoomNumber"]
                    };
                    list.Add(hotSpringRoomInfo);
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"HotSpringRoomInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (HotSpringRoomInfo[])null;
        }

        public HotSpringRoomInfo GetHotSpringRoomInfoSingle(int id)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@RoomID", (object) id)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_HotSpringRoomInfo_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return new HotSpringRoomInfo()
                    {
                        RoomID = (int)ResultDataReader["RoomID"],
                        RoomName = ResultDataReader["RoomName"].ToString(),
                        PlayerID = (int)ResultDataReader["PlayerID"],
                        PlayerName = ResultDataReader["PlayerName"].ToString(),
                        Pwd = ResultDataReader["Pwd"].ToString(),
                        AvailTime = (int)ResultDataReader["AvailTime"],
                        MaxCount = (int)ResultDataReader["MaxCount"],
                        MapIndex = (int)ResultDataReader["MapIndex"],
                        BeginTime = (DateTime)ResultDataReader["BeginTime"],
                        BreakTime = (DateTime)ResultDataReader["BreakTime"],
                        RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString(),
                        RoomType = (int)ResultDataReader["RoomType"],
                        ServerID = (int)ResultDataReader["ServerID"],
                        RoomNumber = (int)ResultDataReader["RoomNumber"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"HotSpringRoomInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (HotSpringRoomInfo)null;
        }

        public bool UpdateHotSpringRoomInfo(HotSpringRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[7]
        {
          new SqlParameter("@RoomID", (object) info.RoomID),
          new SqlParameter("@RoomName", (object) info.RoomName),
          new SqlParameter("@Pwd", (object) info.Pwd),
          new SqlParameter("@AvailTime", (object) info.AvailTime.ToString()),
          new SqlParameter("@BreakTime", (object) info.BreakTime.ToString()),
          new SqlParameter("@roomIntroduction", (object) info.RoomIntroduction),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_HotSpringRoomInfo", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateHotSpringRoomInfo", ex);
            }
            return flag;
        }

        public bool UpdateLastVIPPackTime(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@LastVIPPackTime", (object) DateTime.Now.Date),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserLastVIPPackTime", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateUserLastVIPPackTime", ex);
            }
            return flag;
        }

        public bool UpdateVIPInfo(PlayerInfo p)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[10]
        {
          new SqlParameter("@ID", (object) p.ID),
          new SqlParameter("@VIPLevel", (object) p.VIPLevel),
          new SqlParameter("@VIPExp", (object) p.VIPExp),
          new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt),
          new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt),
          new SqlParameter("@VIPExpireDay", (object) p.VIPExpireDay.ToString()),
          new SqlParameter("@VIPLastDate", (object) DateTime.Now),
          new SqlParameter("@VIPNextLevelDaysNeeded", SqlDbType.BigInt),
          new SqlParameter("@CanTakeVipReward", (object) (int) (p.CanTakeVipReward ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateVIPInfo", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateVIPInfo", ex);
            }
            return flag;
        }

        public int VIPRenewal(string nickName, int renewalDays, ref DateTime ExpireDayOut)
        {
            int num = 0;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@NickName", (object) nickName),
          new SqlParameter("@RenewalDays", (object) renewalDays),
          new SqlParameter("@ExpireDayOut", (object) DateTime.Now),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.Output;
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_VIPRenewal_Single", SqlParameters);
                ExpireDayOut = (DateTime)SqlParameters[2].Value;
                num = (int)SqlParameters[3].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_VIPRenewal_Single", ex);
            }
            return num;
        }

        public int VIPLastdate(int ID)
        {
            int num = 0;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_VIPLastdate_Single", SqlParameters);
                num = (int)SqlParameters[1].Value;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_VIPLastdate_Single", ex);
            }
            return num;
        }

        public bool Test(string DutyName)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Test1", new SqlParameter[1]
        {
          new SqlParameter("@DutyName", (object) DutyName)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool TankAll()
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Tank_All", new SqlParameter[0]);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[8]
        {
          new SqlParameter("@UserName", (object) UserName),
          new SqlParameter("@Password", (object) Password),
          new SqlParameter("@NickName", (object) NickName),
          new SqlParameter("@Sex", (object) (int) (Sex ? 1 : 0)),
          new SqlParameter("@Money", (object) Money),
          new SqlParameter("@GiftToken", (object) GiftToken),
          new SqlParameter("@Gold", (object) Gold),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[7].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Account_Register", SqlParameters);
                if ((int)SqlParameters[7].Value == 0)
                    flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init Register", ex);
            }
            return flag;
        }

        public bool CheckEmailIsValid(string Email)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@Email", (object) Email),
          new SqlParameter("@count", SqlDbType.BigInt)
        };
                SqlParameters[1].Direction = ParameterDirection.Output;
                this.db.RunProcedure("CheckEmailIsValid", SqlParameters);
                if (int.Parse(SqlParameters[1].Value.ToString()) == 0)
                    flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init CheckEmailIsValid", ex);
            }
            return flag;
        }

        public bool RegisterUserInfo(UserInfo userinfo)
        {
            bool flag = false;
            try
            {
                return this.db.RunProcedure("SP_User_Info_Add", new SqlParameter[6]
        {
          new SqlParameter("@UserID", (object) userinfo.UserID),
          new SqlParameter("@UserEmail", (object) userinfo.UserEmail),
          new SqlParameter("@UserPhone", userinfo.UserPhone == null ? (object) string.Empty : (object) userinfo.UserPhone),
          new SqlParameter("@UserOther1", userinfo.UserOther1 == null ? (object) string.Empty : (object) userinfo.UserOther1),
          new SqlParameter("@UserOther2", userinfo.UserOther2 == null ? (object) string.Empty : (object) userinfo.UserOther2),
          new SqlParameter("@UserOther3", userinfo.UserOther3 == null ? (object) string.Empty : (object) userinfo.UserOther3)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public UserInfo GetUserInfo(int UserId)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            UserInfo userInfo = new UserInfo()
            {
                UserID = UserId
            };
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) UserId)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_User_Info", SqlParameters);
                while (ResultDataReader.Read())
                {
                    userInfo.UserID = int.Parse(ResultDataReader["UserID"].ToString());
                    userInfo.UserEmail = ResultDataReader["UserEmail"] == null ? "" : ResultDataReader["UserEmail"].ToString();
                    userInfo.UserPhone = ResultDataReader["UserPhone"] == null ? "" : ResultDataReader["UserPhone"].ToString();
                    userInfo.UserOther1 = ResultDataReader["UserOther1"] == null ? "" : ResultDataReader["UserOther1"].ToString();
                    userInfo.UserOther2 = ResultDataReader["UserOther2"] == null ? "" : ResultDataReader["UserOther2"].ToString();
                    userInfo.UserOther3 = ResultDataReader["UserOther3"] == null ? "" : ResultDataReader["UserOther3"].ToString();
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return userInfo;
        }

        public LevelInfo[] GetAllLevel()
        {
            List<LevelInfo> list = new List<LevelInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Level_All");
                while (ResultDataReader.Read())
                    list.Add(new LevelInfo()
                    {
                        Grade = (int)ResultDataReader["Grade"],
                        GP = (int)ResultDataReader["GP"],
                        Blood = (int)ResultDataReader["Blood"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllLevel", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ExerciseInfo[] GetAllExercise()
        {
            List<ExerciseInfo> list = new List<ExerciseInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Exercise_All");
                while (ResultDataReader.Read())
                    list.Add(new ExerciseInfo()
                    {
                        Grage = (int)ResultDataReader["Grage"],
                        GP = (int)ResultDataReader["GP"],
                        ExerciseA = (int)ResultDataReader["ExerciseA"],
                        ExerciseAG = (int)ResultDataReader["ExerciseAG"],
                        ExerciseD = (int)ResultDataReader["ExerciseD"],
                        ExerciseH = (int)ResultDataReader["ExerciseH"],
                        ExerciseL = (int)ResultDataReader["ExerciseL"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllExercise", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public LevelInfo GetUserLevelSingle(int Grade)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@Grade", (object) Grade)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_Level_By_Grade", SqlParameters);
                if (ResultDataReader.Read())
                    return new LevelInfo()
                    {
                        Grade = (int)ResultDataReader["Grade"],
                        GP = (int)ResultDataReader["GP"],
                        Blood = (int)ResultDataReader["Blood"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetLevelInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (LevelInfo)null;
        }

        public ExerciseInfo GetExerciseSingle(int Grade)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@Grage", (object) Grade)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_Exercise_By_Grade", SqlParameters);
                if (ResultDataReader.Read())
                    return new ExerciseInfo()
                    {
                        Grage = (int)ResultDataReader["Grage"],
                        GP = (int)ResultDataReader["GP"],
                        ExerciseA = (int)ResultDataReader["ExerciseA"],
                        ExerciseAG = (int)ResultDataReader["ExerciseAG"],
                        ExerciseD = (int)ResultDataReader["ExerciseD"],
                        ExerciseH = (int)ResultDataReader["ExerciseH"],
                        ExerciseL = (int)ResultDataReader["ExerciseL"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetExerciseInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (ExerciseInfo)null;
        }

        public TexpInfo GetUserTexpInfoSingle(int ID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) ID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserTexp_By_ID", SqlParameters);
                if (ResultDataReader.Read())
                    return new TexpInfo()
                    {
                        UserID = (int)ResultDataReader["UserID"],
                        attTexpExp = (int)ResultDataReader["attTexpExp"],
                        defTexpExp = (int)ResultDataReader["defTexpExp"],
                        hpTexpExp = (int)ResultDataReader["hpTexpExp"],
                        lukTexpExp = (int)ResultDataReader["lukTexpExp"],
                        spdTexpExp = (int)ResultDataReader["spdTexpExp"],
                        texpCount = (int)ResultDataReader["texpCount"],
                        texpTaskCount = (int)ResultDataReader["texpTaskCount"],
                        texpTaskDate = (DateTime)ResultDataReader["texpTaskDate"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetTexpInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (TexpInfo)null;
        }

        public bool UpdateUserTexpInfo(TexpInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[10]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@attTexpExp", (object) info.attTexpExp),
          new SqlParameter("@defTexpExp", (object) info.defTexpExp),
          new SqlParameter("@hpTexpExp", (object) info.hpTexpExp),
          new SqlParameter("@lukTexpExp", (object) info.lukTexpExp),
          new SqlParameter("@spdTexpExp", (object) info.spdTexpExp),
          new SqlParameter("@texpCount", (object) info.texpCount),
          new SqlParameter("@texpTaskCount", (object) info.texpTaskCount),
          new SqlParameter("@texpTaskDate", (object) info.texpTaskDate.ToString()),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserTexp_Update", SqlParameters);
                flag = (int)SqlParameters[9].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool InsertUserTexpInfo(TexpInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[10]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@attTexpExp", (object) info.attTexpExp),
          new SqlParameter("@defTexpExp", (object) info.defTexpExp),
          new SqlParameter("@hpTexpExp", (object) info.hpTexpExp),
          new SqlParameter("@lukTexpExp", (object) info.lukTexpExp),
          new SqlParameter("@spdTexpExp", (object) info.spdTexpExp),
          new SqlParameter("@texpCount", (object) info.texpCount),
          new SqlParameter("@texpTaskCount", (object) info.texpTaskCount),
          new SqlParameter("@texpTaskDate", (object) info.texpTaskDate.ToString()),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserTexp_Add", SqlParameters);
                flag = (int)SqlParameters[9].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"InsertTexpInfo", ex);
            }
            return flag;
        }

        public bool AddeqPet(PetEquipDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9];
                SqlParameters[0] = new SqlParameter("@ID", (object)info.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)info.UserID);
                SqlParameters[2] = new SqlParameter("@PetID", (object)info.PetID);
                SqlParameters[3] = new SqlParameter("@eqType", (object)info.eqType);
                SqlParameters[4] = new SqlParameter("@eqTemplateID", (object)info.eqTemplateID);
                SqlParameters[5] = new SqlParameter("@startTime", (object)info.startTime);
                SqlParameters[6] = new SqlParameter("@ValidDate", (object)info.ValidDate);
                SqlParameters[7] = new SqlParameter("@IsExit", (object)(int)(info.IsExit ? 1 : 0));
                SqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_User_Add_eqPet", SqlParameters);
                flag = (int)SqlParameters[8].Value == 0;
                info.ID = (int)SqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateqPet(PetEquipDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@PetID", (object) info.PetID),
          new SqlParameter("@eqType", (object) info.eqType),
          new SqlParameter("@eqTemplateID", (object) info.eqTemplateID),
          new SqlParameter("@startTime", (object) info.startTime),
          new SqlParameter("@ValidDate", (object) info.ValidDate),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_eqPet_Update", SqlParameters);
                flag = (int)SqlParameters[8].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public PetEquipDataInfo[] GetEqPetSingles(int UserID)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_eqPet_Single", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new PetEquipDataInfo(ItemMgr.FindItemTemplate((int)ResultDataReader["eqTemplateID"]))
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        PetID = (int)ResultDataReader["PetID"],
                        eqType = (int)ResultDataReader["eqType"],
                        eqTemplateID = (int)ResultDataReader["eqTemplateID"],
                        startTime = (DateTime)ResultDataReader["startTime"],
                        ValidDate = (int)ResultDataReader["ValidDate"],
                        IsExit = (bool)ResultDataReader["IsExit"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool AddUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[30]
        {
          new SqlParameter("@TemplateID", (object) info.TemplateID),
          new SqlParameter("@Name", info.Name == null ? (object) "Error!" : (object) info.Name),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Attack", (object) info.Attack),
          new SqlParameter("@Defence", (object) info.Defence),
          new SqlParameter("@Luck", (object) info.Luck),
          new SqlParameter("@Agility", (object) info.Agility),
          new SqlParameter("@Blood", (object) info.Blood),
          new SqlParameter("@Damage", (object) info.Damage),
          new SqlParameter("@Guard", (object) info.Guard),
          new SqlParameter("@AttackGrow", (object) info.AttackGrow),
          new SqlParameter("@DefenceGrow", (object) info.DefenceGrow),
          new SqlParameter("@LuckGrow", (object) info.LuckGrow),
          new SqlParameter("@AgilityGrow", (object) info.AgilityGrow),
          new SqlParameter("@BloodGrow", (object) info.BloodGrow),
          new SqlParameter("@DamageGrow", (object) info.DamageGrow),
          new SqlParameter("@GuardGrow", (object) info.GuardGrow),
          new SqlParameter("@Level", (object) info.Level),
          new SqlParameter("@GP", (object) info.GP),
          new SqlParameter("@MaxGP", (object) info.MaxGP),
          new SqlParameter("@Hunger", (object) info.Hunger),
          new SqlParameter("@PetHappyStar", (object) info.PetHappyStar),
          new SqlParameter("@MP", (object) info.MP),
          new SqlParameter("@IsEquip", (object) (int) (info.IsEquip ? 1 : 0)),
          new SqlParameter("@Skill", (object) info.Skill),
          new SqlParameter("@SkillEquip", (object) info.SkillEquip),
          new SqlParameter("@Place", (object) info.Place),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@ID", (object) info.ID),
          null
        };
                SqlParameters[28].Direction = ParameterDirection.Output;
                SqlParameters[29] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[29].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_User_Add_Pet", SqlParameters);
                flag = (int)SqlParameters[29].Value == 0;
                info.ID = (int)SqlParameters[28].Value;
                info.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[30]
        {
          new SqlParameter("@TemplateID", (object) info.TemplateID),
          new SqlParameter("@Name", info.Name == null ? (object) "Error!" : (object) info.Name),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Attack", (object) info.Attack),
          new SqlParameter("@Defence", (object) info.Defence),
          new SqlParameter("@Luck", (object) info.Luck),
          new SqlParameter("@Agility", (object) info.Agility),
          new SqlParameter("@Blood", (object) info.Blood),
          new SqlParameter("@Damage", (object) info.Damage),
          new SqlParameter("@Guard", (object) info.Guard),
          new SqlParameter("@AttackGrow", (object) info.AttackGrow),
          new SqlParameter("@DefenceGrow", (object) info.DefenceGrow),
          new SqlParameter("@LuckGrow", (object) info.LuckGrow),
          new SqlParameter("@AgilityGrow", (object) info.AgilityGrow),
          new SqlParameter("@BloodGrow", (object) info.BloodGrow),
          new SqlParameter("@DamageGrow", (object) info.DamageGrow),
          new SqlParameter("@GuardGrow", (object) info.GuardGrow),
          new SqlParameter("@Level", (object) info.Level),
          new SqlParameter("@GP", (object) info.GP),
          new SqlParameter("@MaxGP", (object) info.MaxGP),
          new SqlParameter("@Hunger", (object) info.Hunger),
          new SqlParameter("@PetHappyStar", (object) info.PetHappyStar),
          new SqlParameter("@MP", (object) info.MP),
          new SqlParameter("@IsEquip", (object) (int) (info.IsEquip ? 1 : 0)),
          new SqlParameter("@Place", (object) info.Place),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@Skill", (object) info.Skill),
          new SqlParameter("@SkillEquip", (object) info.SkillEquip),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[29].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserPet_Update", SqlParameters);
                flag = (int)SqlParameters[29].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool RemoveUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[28]
        {
          new SqlParameter("@TemplateID", (object) info.TemplateID),
          new SqlParameter("@Name", info.Name == null ? (object) "Error!" : (object) info.Name),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Attack", (object) info.Attack),
          new SqlParameter("@Defence", (object) info.Defence),
          new SqlParameter("@Luck", (object) info.Luck),
          new SqlParameter("@Agility", (object) info.Agility),
          new SqlParameter("@Blood", (object) info.Blood),
          new SqlParameter("@Damage", (object) info.Damage),
          new SqlParameter("@Guard", (object) info.Guard),
          new SqlParameter("@AttackGrow", (object) info.AttackGrow),
          new SqlParameter("@DefenceGrow", (object) info.DefenceGrow),
          new SqlParameter("@LuckGrow", (object) info.LuckGrow),
          new SqlParameter("@AgilityGrow", (object) info.AgilityGrow),
          new SqlParameter("@BloodGrow", (object) info.BloodGrow),
          new SqlParameter("@DamageGrow", (object) info.DamageGrow),
          new SqlParameter("@GuardGrow", (object) info.GuardGrow),
          new SqlParameter("@Level", (object) info.Level),
          new SqlParameter("@GP", (object) info.GP),
          new SqlParameter("@MaxGP", (object) info.MaxGP),
          new SqlParameter("@Hunger", (object) info.Hunger),
          new SqlParameter("@PetHappyStar", (object) info.PetHappyStar),
          new SqlParameter("@MP", (object) info.MP),
          new SqlParameter("@IsEquip", (object) (int) (info.IsEquip ? 1 : 0)),
          new SqlParameter("@Place", (object) info.Place),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[27].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserPet_Remove", SqlParameters);
                flag = (int)SqlParameters[27].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public UsersPetinfo GetAdoptPetSingle(int PetID)
        {
            UsersPetinfo usersPetinfo = new UsersPetinfo();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)PetID;
                this.db.GetReader(ref ResultDataReader, "SP_AdoptPet_By_Id", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPet(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UsersPetinfo)null;
        }

        public UsersPetinfo[] GetUserAdoptPetSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_User_AdoptPetList", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitPet(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool RemoveUserAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Remove_User_AdoptPet", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool ClearAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Clear_AdoptPet", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateUserAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@ID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_User_AdoptPet", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool AddUserAdoptPet(UsersPetinfo info, bool isUse)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[23]
        {
          new SqlParameter("@TemplateID", (object) info.TemplateID),
          new SqlParameter("@Name", info.Name == null ? (object) "Error!" : (object) info.Name),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Attack", (object) info.Attack),
          new SqlParameter("@Defence", (object) info.Defence),
          new SqlParameter("@Luck", (object) info.Luck),
          new SqlParameter("@Agility", (object) info.Agility),
          new SqlParameter("@Blood", (object) info.Blood),
          new SqlParameter("@Damage", (object) info.Damage),
          new SqlParameter("@Guard", (object) info.Guard),
          new SqlParameter("@AttackGrow", (object) info.AttackGrow),
          new SqlParameter("@DefenceGrow", (object) info.DefenceGrow),
          new SqlParameter("@LuckGrow", (object) info.LuckGrow),
          new SqlParameter("@AgilityGrow", (object) info.AgilityGrow),
          new SqlParameter("@BloodGrow", (object) info.BloodGrow),
          new SqlParameter("@DamageGrow", (object) info.DamageGrow),
          new SqlParameter("@GuardGrow", (object) info.GuardGrow),
          new SqlParameter("@Skill", (object) info.Skill),
          new SqlParameter("@SkillEquip", (object) info.SkillEquip),
          new SqlParameter("@Place", (object) info.Place),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@IsUse", (object) (int) (isUse ? 1 : 0)),
          new SqlParameter("@ID", (object) info.ID)
        };
                SqlParameters[22].Direction = ParameterDirection.Output;
                flag = this.db.RunProcedure("SP_User_AdoptPet", SqlParameters);
                info.ID = (int)SqlParameters[22].Value;
                info.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public UsersPetinfo[] GetUserPetSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitPet(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public List<UsersPetinfo> GetUserPetIsExitSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_IsExit", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitPet(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public UsersPetinfo GetUserPetSingle(int ID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)ID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitPet(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetPetInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UsersPetinfo)null;
        }

        public UsersPetinfo InitPet(SqlDataReader reader)
        {
            return new UsersPetinfo()
            {
                ID = (int)reader["ID"],
                TemplateID = (int)reader["TemplateID"],
                Name = reader["Name"].ToString(),
                UserID = (int)reader["UserID"],
                Attack = (int)reader["Attack"],
                AttackGrow = (int)reader["AttackGrow"],
                Agility = (int)reader["Agility"],
                AgilityGrow = (int)reader["AgilityGrow"],
                Defence = (int)reader["Defence"],
                DefenceGrow = (int)reader["DefenceGrow"],
                Luck = (int)reader["Luck"],
                LuckGrow = (int)reader["LuckGrow"],
                Blood = (int)reader["Blood"],
                BloodGrow = (int)reader["BloodGrow"],
                Damage = (int)reader["Damage"],
                DamageGrow = (int)reader["DamageGrow"],
                Guard = (int)reader["Guard"],
                GuardGrow = (int)reader["GuardGrow"],
                Level = (int)reader["Level"],
                GP = (int)reader["GP"],
                MaxGP = (int)reader["MaxGP"],
                Hunger = (int)reader["Hunger"],
                PetHappyStar = (int)reader["PetHappyStar"],
                MP = (int)reader["MP"],
                Place = (int)reader["Place"],
                IsEquip = (bool)reader["IsEquip"],
                IsExit = (bool)reader["IsExit"],
                Skill = reader["Skill"].ToString(),
                SkillEquip = reader["SkillEquip"].ToString()
            };
        }

        public PetConfig[] GetAllPetConfig()
        {
            List<PetConfig> list = new List<PetConfig>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetConfig_All");
                while (ResultDataReader.Read())
                    list.Add(new PetConfig()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        Value = ResultDataReader["Value"].ToString()
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetConfig", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetLevel[] GetAllPetLevel()
        {
            List<PetLevel> list = new List<PetLevel>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetLevel_All");
                while (ResultDataReader.Read())
                    list.Add(new PetLevel()
                    {
                        Level = (int)ResultDataReader["Level"],
                        GP = (int)ResultDataReader["GP"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetLevel", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetTemplateInfo[] GetAllPetTemplateInfo()
        {
            List<PetTemplateInfo> list = new List<PetTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetTemplateInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new PetTemplateInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        Name = ResultDataReader["Name"].ToString(),
                        KindID = (int)ResultDataReader["KindID"],
                        Description = ResultDataReader["Description"].ToString(),
                        Pic = ResultDataReader["Pic"].ToString(),
                        RareLevel = (int)ResultDataReader["RareLevel"],
                        MP = (int)ResultDataReader["MP"],
                        StarLevel = (int)ResultDataReader["StarLevel"],
                        GameAssetUrl = ResultDataReader["GameAssetUrl"].ToString(),
                        EvolutionID = (int)ResultDataReader["EvolutionID"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetSkillTemplateInfo[] GetAllPetSkillTemplateInfo()
        {
            List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetSkillTemplateInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new PetSkillTemplateInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        PetTemplateID = (int)ResultDataReader["PetTemplateID"],
                        KindID = (int)ResultDataReader["KindID"],
                        GetTypes = (int)ResultDataReader["GetType"],
                        SkillID = (int)ResultDataReader["SkillID"],
                        SkillBookID = (int)ResultDataReader["SkillBookID"],
                        MinLevel = (int)ResultDataReader["MinLevel"],
                        DeleteSkillIDs = ResultDataReader["DeleteSkillIDs"].ToString()
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetSkillTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetSkillInfo[] GetAllPetSkillInfo()
        {
            List<PetSkillInfo> list = new List<PetSkillInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetSkillInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new PetSkillInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        ElementIDs = ResultDataReader["ElementIDs"].ToString(),
                        Description = ResultDataReader["Description"].ToString(),
                        BallType = (int)ResultDataReader["BallType"],
                        NewBallID = (int)ResultDataReader["NewBallID"],
                        CostMP = (int)ResultDataReader["CostMP"],
                        Pic = (int)ResultDataReader["Pic"],
                        Action = ResultDataReader["Action"].ToString(),
                        EffectPic = ResultDataReader["EffectPic"].ToString(),
                        Delay = (int)ResultDataReader["Delay"],
                        ColdDown = (int)ResultDataReader["ColdDown"],
                        GameType = (int)ResultDataReader["GameType"],
                        Probability = (int)ResultDataReader["Probability"],
                        Damage = (int)ResultDataReader["Damage"],
                        DamageCrit = (int)ResultDataReader["DamageCrit"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetSkillInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetSkillElementInfo[] GetAllPetSkillElementInfo()
        {
            List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetSkillElementInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new PetSkillElementInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        EffectPic = ResultDataReader["EffectPic"].ToString(),
                        Description = ResultDataReader["Description"].ToString(),
                        Pic = (int)ResultDataReader["Pic"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetSkillElementInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public PetExpItemPriceInfo[] GetAllPetExpItemPriceInfoInfo()
        {
            List<PetExpItemPriceInfo> list = new List<PetExpItemPriceInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_PetExpItemPriceInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new PetExpItemPriceInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Count = (int)ResultDataReader["Count"],
                        Money = (int)ResultDataReader["Money"],
                        ItemCount = (int)ResultDataReader["ItemCount"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllPetTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool AddCards(UsersCardInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[16];
                SqlParameters[0] = new SqlParameter("@CardID", (object)item.CardID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@CardType", (object)item.CardType);
                SqlParameters[2] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[3] = new SqlParameter("@Place", (object)item.Place);
                SqlParameters[4] = new SqlParameter("@TemplateID", (object)item.TemplateID);
                SqlParameters[5] = new SqlParameter("@isFirstGet", (object)false);
                SqlParameters[6] = new SqlParameter("@Attack", (object)item.Attack);
                SqlParameters[7] = new SqlParameter("@Defence", (object)item.Defence);
                SqlParameters[8] = new SqlParameter("@Luck", (object)item.Luck);
                SqlParameters[9] = new SqlParameter("@Agility", (object)item.Agility);
                SqlParameters[10] = new SqlParameter("@Damage", (object)item.Damage);
                SqlParameters[11] = new SqlParameter("@Guard", (object)item.Guard);
                SqlParameters[12] = new SqlParameter("@IsExit", (object)(int)(item.IsExit ? 1 : 0));
                SqlParameters[13] = new SqlParameter("@Level", (object)item.Level);
                SqlParameters[14] = new SqlParameter("@CardGP", (object)item.CardGP);
                SqlParameters[15] = new SqlParameter("@Type", (object)item.Type);
                flag = this.db.RunProcedure("SP_Users_Cards_Add", SqlParameters);
                item.CardID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateCards(UsersCardInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[17]
        {
          new SqlParameter("@CardType", (object) info.CardType),
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Place", (object) info.Place),
          new SqlParameter("@TemplateID", (object) info.TemplateID),
          new SqlParameter("@isFirstGet", (object) (int) (info.isFirstGet ? 1 : 0)),
          new SqlParameter("@Attack", (object) info.Attack),
          new SqlParameter("@Defence", (object) info.Defence),
          new SqlParameter("@Luck", (object) info.Luck),
          new SqlParameter("@Agility", (object) info.Agility),
          new SqlParameter("@Damage", (object) info.Damage),
          new SqlParameter("@Guard", (object) info.Guard),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@Level", (object) info.Level),
          new SqlParameter("@CardGP", (object) info.CardGP),
          new SqlParameter("@Type", (object) info.Type),
          new SqlParameter("@CardID", (object) info.CardID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[16].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserCardProp_Update", SqlParameters);
                flag = (int)SqlParameters[16].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public List<UsersCardInfo> GetUserCardEuqip(int UserID)
        {
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Items_Card_Equip", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitCard(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public UsersCardInfo GetUserCardByPlace(int Place)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@Place", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)Place;
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_Place", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitCard(ResultDataReader);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UsersCardInfo)null;
        }

        public UsersCardInfo[] GetUserCardSingles(int UserID)
        {
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_ID", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitCard(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public UsersCardInfo InitCard(SqlDataReader reader)
        {
            return new UsersCardInfo()
            {
                UserID = (int)reader["UserID"],
                TemplateID = (int)reader["TemplateID"],
                CardID = (int)reader["CardID"],
                CardType = (int)reader["CardType"],
                Attack = (int)reader["Attack"],
                Agility = (int)reader["Agility"],
                Defence = (int)reader["Defence"],
                Luck = (int)reader["Luck"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Level = (int)reader["Level"],
                Place = (int)reader["Place"],
                isFirstGet = (bool)reader["isFirstGet"],
                Type = (int)reader["Type"],
                CardGP = (int)reader["CardGP"]
            };
        }

        public CardGrooveUpdateInfo[] GetAllCardGrooveUpdate()
        {
            List<CardGrooveUpdateInfo> list = new List<CardGrooveUpdateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_CardGrooveUpdate_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitCardGrooveUpdate(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllCardGrooveUpdate", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public CardGrooveUpdateInfo InitCardGrooveUpdate(SqlDataReader reader)
        {
            return new CardGrooveUpdateInfo()
            {
                ID = (int)reader["ID"],
                Attack = (int)reader["Attack"],
                Defend = (int)reader["Defend"],
                Agility = (int)reader["Agility"],
                Lucky = (int)reader["Lucky"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Level = (int)reader["Level"],
                Type = (int)reader["Type"],
                Exp = (int)reader["Exp"]
            };
        }

        public CardTemplateInfo[] GetAllCardTemplate()
        {
            List<CardTemplateInfo> list = new List<CardTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_CardTemplate_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitCardTemplate(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllCardTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public CardTemplateInfo InitCardTemplate(SqlDataReader reader)
        {
            return new CardTemplateInfo()
            {
                ID = (int)reader["ID"],
                CardID = (int)reader["CardID"],
                CardType = (int)reader["CardType"],
                probability = (int)reader["probability"],
                AttackRate = (int)reader["AttackRate"],
                AddAttack = (int)reader["AddAttack"],
                DefendRate = (int)reader["DefendRate"],
                AddDefend = (int)reader["AddDefend"],
                AgilityRate = (int)reader["AgilityRate"],
                AddAgility = (int)reader["AddAgility"],
                LuckyRate = (int)reader["LuckyRate"],
                AddLucky = (int)reader["AddLucky"],
                DamageRate = (int)reader["DamageRate"],
                AddDamage = (int)reader["AddDamage"],
                GuardRate = (int)reader["GuardRate"],
                AddGuard = (int)reader["AddGuard"]
            };
        }

        public bool AddFarm(UserFarmInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[15]
        {
          new SqlParameter("@FarmID", (object) item.FarmID),
          new SqlParameter("@PayFieldMoney", (object) item.PayFieldMoney),
          new SqlParameter("@PayAutoMoney", (object) item.PayAutoMoney),
          new SqlParameter("@AutoPayTime", (object) item.AutoPayTime.ToString()),
          new SqlParameter("@AutoValidDate", (object) item.AutoValidDate),
          new SqlParameter("@VipLimitLevel", (object) item.VipLimitLevel),
          new SqlParameter("@FarmerName", (object) item.FarmerName),
          new SqlParameter("@GainFieldId", (object) item.GainFieldId),
          new SqlParameter("@MatureId", (object) item.MatureId),
          new SqlParameter("@KillCropId", (object) item.KillCropId),
          new SqlParameter("@isAutoId", (object) item.isAutoId),
          new SqlParameter("@isFarmHelper", (object) (int) (item.isFarmHelper ? 1 : 0)),
          new SqlParameter("@ID", (object) item.ID),
          null,
          null
        };
                SqlParameters[12].Direction = ParameterDirection.Output;
                SqlParameters[13] = new SqlParameter("@buyExpRemainNum", (object)item.buyExpRemainNum);
                SqlParameters[14] = new SqlParameter("@isArrange", (object)(int)(item.isArrange ? 1 : 0));
                flag = this.db.RunProcedure("SP_Users_Farm_Add", SqlParameters);
                item.ID = (int)SqlParameters[12].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateFarm(UserFarmInfo info)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Farm_Update", new SqlParameter[15]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@FarmID", (object) info.FarmID),
          new SqlParameter("@PayFieldMoney", (object) info.PayFieldMoney),
          new SqlParameter("@PayAutoMoney", (object) info.PayAutoMoney),
          new SqlParameter("@AutoPayTime", (object) info.AutoPayTime.ToString()),
          new SqlParameter("@AutoValidDate", (object) info.AutoValidDate),
          new SqlParameter("@VipLimitLevel", (object) info.VipLimitLevel),
          new SqlParameter("@FarmerName", (object) info.FarmerName),
          new SqlParameter("@GainFieldId", (object) info.GainFieldId),
          new SqlParameter("@MatureId", (object) info.MatureId),
          new SqlParameter("@KillCropId", (object) info.KillCropId),
          new SqlParameter("@isAutoId", (object) info.isAutoId),
          new SqlParameter("@isFarmHelper", (object) (int) (info.isFarmHelper ? 1 : 0)),
          new SqlParameter("@buyExpRemainNum", (object) info.buyExpRemainNum),
          new SqlParameter("@isArrange", (object) (int) (info.isArrange ? 1 : 0))
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool AddFields(UserFieldInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[17]
        {
          new SqlParameter("@FarmID", (object) item.FarmID),
          new SqlParameter("@FieldID", (object) item.FieldID),
          new SqlParameter("@SeedID", (object) item.SeedID),
          new SqlParameter("@PlantTime", (object) item.PlantTime.ToString()),
          new SqlParameter("@AccelerateTime", (object) item.AccelerateTime),
          new SqlParameter("@FieldValidDate", (object) item.FieldValidDate),
          new SqlParameter("@PayTime", (object) item.PayTime.ToString()),
          new SqlParameter("@GainCount", (object) item.GainCount),
          new SqlParameter("@AutoSeedID", (object) item.AutoSeedID),
          new SqlParameter("@AutoFertilizerID", (object) item.AutoFertilizerID),
          new SqlParameter("@AutoSeedIDCount", (object) item.AutoSeedIDCount),
          new SqlParameter("@AutoFertilizerCount", (object) item.AutoFertilizerCount),
          new SqlParameter("@isAutomatic", (object) (int) (item.isAutomatic ? 1 : 0)),
          new SqlParameter("@AutomaticTime", (object) item.AutomaticTime.ToString()),
          new SqlParameter("@IsExit", (object) (int) (item.IsExit ? 1 : 0)),
          new SqlParameter("@payFieldTime", (object) item.payFieldTime),
          new SqlParameter("@ID", (object) item.ID)
        };
                SqlParameters[16].Direction = ParameterDirection.Output;
                flag = this.db.RunProcedure("SP_Users_Fields_Add", SqlParameters);
                item.ID = (int)SqlParameters[16].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateFields(UserFieldInfo info)
        {
            bool flag = false;
            try
            {
                flag = this.db.RunProcedure("SP_Users_Fields_Update", new SqlParameter[17]
        {
          new SqlParameter("@ID", (object) info.ID),
          new SqlParameter("@FarmID", (object) info.FarmID),
          new SqlParameter("@FieldID", (object) info.FieldID),
          new SqlParameter("@SeedID", (object) info.SeedID),
          new SqlParameter("@PlantTime", (object) info.PlantTime.ToString()),
          new SqlParameter("@AccelerateTime", (object) info.AccelerateTime),
          new SqlParameter("@FieldValidDate", (object) info.FieldValidDate),
          new SqlParameter("@PayTime", (object) info.PayTime.ToString()),
          new SqlParameter("@GainCount", (object) info.GainCount),
          new SqlParameter("@AutoSeedID", (object) info.AutoSeedID),
          new SqlParameter("@AutoFertilizerID", (object) info.AutoFertilizerID),
          new SqlParameter("@AutoSeedIDCount", (object) info.AutoSeedIDCount),
          new SqlParameter("@AutoFertilizerCount", (object) info.AutoFertilizerCount),
          new SqlParameter("@isAutomatic", (object) (int) (info.isAutomatic ? 1 : 0)),
          new SqlParameter("@AutomaticTime", (object) info.AutomaticTime.ToString()),
          new SqlParameter("@IsExit", (object) (int) (info.IsExit ? 1 : 0)),
          new SqlParameter("@payFieldTime", (object) info.payFieldTime)
        });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public UserFarmInfo GetSingleFarm(int Id)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)Id;
                this.db.GetReader(ref ResultDataReader, "SP_Get_SingleFarm", SqlParameters);
                if (ResultDataReader.Read())
                    return new UserFarmInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        FarmID = (int)ResultDataReader["FarmID"],
                        PayFieldMoney = (string)ResultDataReader["PayFieldMoney"],
                        PayAutoMoney = (string)ResultDataReader["PayAutoMoney"],
                        AutoPayTime = (DateTime)ResultDataReader["AutoPayTime"],
                        AutoValidDate = (int)ResultDataReader["AutoValidDate"],
                        VipLimitLevel = (int)ResultDataReader["VipLimitLevel"],
                        FarmerName = (string)ResultDataReader["FarmerName"],
                        GainFieldId = (int)ResultDataReader["GainFieldId"],
                        MatureId = (int)ResultDataReader["MatureId"],
                        KillCropId = (int)ResultDataReader["KillCropId"],
                        isAutoId = (int)ResultDataReader["isAutoId"],
                        isFarmHelper = (bool)ResultDataReader["isFarmHelper"],
                        buyExpRemainNum = (int)ResultDataReader["buyExpRemainNum"],
                        isArrange = (bool)ResultDataReader["isArrange"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetSingleFarm", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UserFarmInfo)null;
        }

        public UserFieldInfo[] GetSingleFields(int ID)
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)ID;
                this.db.GetReader(ref ResultDataReader, "SP_Get_SingleFields", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new UserFieldInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        FarmID = (int)ResultDataReader["FarmID"],
                        FieldID = (int)ResultDataReader["FieldID"],
                        SeedID = (int)ResultDataReader["SeedID"],
                        PlantTime = (DateTime)ResultDataReader["PlantTime"],
                        AccelerateTime = (int)ResultDataReader["AccelerateTime"],
                        FieldValidDate = (int)ResultDataReader["FieldValidDate"],
                        PayTime = (DateTime)ResultDataReader["PayTime"],
                        GainCount = (int)ResultDataReader["GainCount"],
                        AutoSeedID = (int)ResultDataReader["AutoSeedID"],
                        AutoFertilizerID = (int)ResultDataReader["AutoFertilizerID"],
                        AutoSeedIDCount = (int)ResultDataReader["AutoSeedIDCount"],
                        AutoFertilizerCount = (int)ResultDataReader["AutoFertilizerCount"],
                        isAutomatic = (bool)ResultDataReader["isAutomatic"],
                        AutomaticTime = (DateTime)ResultDataReader["AutomaticTime"],
                        IsExit = (bool)ResultDataReader["IsExit"],
                        payFieldTime = (int)ResultDataReader["payFieldTime"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleFields", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool DeleteAllFields(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_RemoveAllFields", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_RemoveAllFields", ex);
            }
            return flag;
        }

        public List<UserGemStone> GetSingleGemStones(int ID)
        {
            List<UserGemStone> list = new List<UserGemStone>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)ID;
                this.db.GetReader(ref ResultDataReader, "SP_GetSingleGemStone", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new UserGemStone()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        FigSpiritId = (int)ResultDataReader["FigSpiritId"],
                        FigSpiritIdValue = (string)ResultDataReader["FigSpiritIdValue"],
                        EquipPlace = (int)ResultDataReader["EquipPlace"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleUserGemStones", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public bool AddUserGemStone(UserGemStone item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6];
                SqlParameters[0] = new SqlParameter("@ID", (object)item.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[2] = new SqlParameter("@FigSpiritId", (object)item.FigSpiritId);
                SqlParameters[3] = new SqlParameter("@FigSpiritIdValue", (object)item.FigSpiritIdValue);
                SqlParameters[4] = new SqlParameter("@EquipPlace", (object)item.EquipPlace);
                SqlParameters[5] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_GemStones_Add", SqlParameters);
                flag = (int)SqlParameters[5].Value == 0;
                item.ID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateGemStoneInfo(UserGemStone g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6]
        {
          new SqlParameter("@ID", (object) g.ID),
          new SqlParameter("@UserID", (object) g.UserID),
          new SqlParameter("@FigSpiritId", (object) g.FigSpiritId),
          new SqlParameter("@FigSpiritIdValue", (object) g.FigSpiritIdValue),
          new SqlParameter("@EquipPlace", (object) g.EquipPlace),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateGemStoneInfo", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateGemStoneInfo", ex);
            }
            return flag;
        }

        public UserLabyrinthInfo GetSingleLabyrinth(int ID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)ID;
                this.db.GetReader(ref ResultDataReader, "SP_GetSingleLabyrinth", SqlParameters);
                if (ResultDataReader.Read())
                    return new UserLabyrinthInfo()
                    {
                        UserID = (int)ResultDataReader["UserID"],
                        myProgress = (int)ResultDataReader["myProgress"],
                        myRanking = (int)ResultDataReader["myRanking"],
                        completeChallenge = (bool)ResultDataReader["completeChallenge"],
                        isDoubleAward = (bool)ResultDataReader["isDoubleAward"],
                        currentFloor = (int)ResultDataReader["currentFloor"],
                        accumulateExp = (int)ResultDataReader["accumulateExp"],
                        remainTime = (int)ResultDataReader["remainTime"],
                        currentRemainTime = (int)ResultDataReader["currentRemainTime"],
                        cleanOutAllTime = (int)ResultDataReader["cleanOutAllTime"],
                        cleanOutGold = (int)ResultDataReader["cleanOutGold"],
                        tryAgainComplete = (bool)ResultDataReader["tryAgainComplete"],
                        isInGame = (bool)ResultDataReader["isInGame"],
                        isCleanOut = (bool)ResultDataReader["isCleanOut"],
                        serverMultiplyingPower = (bool)ResultDataReader["serverMultiplyingPower"],
                        LastDate = (DateTime)ResultDataReader["LastDate"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleUserLabyrinth", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UserLabyrinthInfo)null;
        }

        public bool AddUserLabyrinth(UserLabyrinthInfo laby)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[17]
        {
          new SqlParameter("@UserID", (object) laby.UserID),
          new SqlParameter("@myProgress", (object) laby.myProgress),
          new SqlParameter("@myRanking", (object) laby.myRanking),
          new SqlParameter("@completeChallenge", (object) (int) (laby.completeChallenge ? 1 : 0)),
          new SqlParameter("@isDoubleAward", (object) (int) (laby.isDoubleAward ? 1 : 0)),
          new SqlParameter("@currentFloor", (object) laby.currentFloor),
          new SqlParameter("@accumulateExp", (object) laby.accumulateExp),
          new SqlParameter("@remainTime", (object) laby.remainTime),
          new SqlParameter("@currentRemainTime", (object) laby.currentRemainTime),
          new SqlParameter("@cleanOutAllTime", (object) laby.cleanOutAllTime),
          new SqlParameter("@cleanOutGold", (object) laby.cleanOutGold),
          new SqlParameter("@tryAgainComplete", (object) (int) (laby.tryAgainComplete ? 1 : 0)),
          new SqlParameter("@isInGame", (object) (int) (laby.isInGame ? 1 : 0)),
          new SqlParameter("@isCleanOut", (object) (int) (laby.isCleanOut ? 1 : 0)),
          new SqlParameter("@serverMultiplyingPower", (object) (int) (laby.serverMultiplyingPower ? 1 : 0)),
          new SqlParameter("@LastDate", (object) laby.LastDate),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[16].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_Labyrinth_Add", SqlParameters);
                flag = (int)SqlParameters[16].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateLabyrinthInfo(UserLabyrinthInfo laby)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[17]
        {
          new SqlParameter("@UserID", (object) laby.UserID),
          new SqlParameter("@myProgress", (object) laby.myProgress),
          new SqlParameter("@myRanking", (object) laby.myRanking),
          new SqlParameter("@completeChallenge", (object) (int) (laby.completeChallenge ? 1 : 0)),
          new SqlParameter("@isDoubleAward", (object) (int) (laby.isDoubleAward ? 1 : 0)),
          new SqlParameter("@currentFloor", (object) laby.currentFloor),
          new SqlParameter("@accumulateExp", (object) laby.accumulateExp),
          new SqlParameter("@remainTime", (object) laby.remainTime),
          new SqlParameter("@currentRemainTime", (object) laby.currentRemainTime),
          new SqlParameter("@cleanOutAllTime", (object) laby.cleanOutAllTime),
          new SqlParameter("@cleanOutGold", (object) laby.cleanOutGold),
          new SqlParameter("@tryAgainComplete", (object) (int) (laby.tryAgainComplete ? 1 : 0)),
          new SqlParameter("@isInGame", (object) (int) (laby.isInGame ? 1 : 0)),
          new SqlParameter("@isCleanOut", (object) (int) (laby.isCleanOut ? 1 : 0)),
          new SqlParameter("@serverMultiplyingPower", (object) (int) (laby.serverMultiplyingPower ? 1 : 0)),
          new SqlParameter("@LastDate", (object) laby.LastDate),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[16].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateLabyrinthInfo", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateLabyrinthInfo", ex);
            }
            return flag;
        }

        public TotemInfo[] GetAllTotem()
        {
            List<TotemInfo> list = new List<TotemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Totem_All");
                while (ResultDataReader.Read())
                    list.Add(new TotemInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        ConsumeExp = (int)ResultDataReader["ConsumeExp"],
                        ConsumeHonor = (int)ResultDataReader["ConsumeHonor"],
                        AddAttack = (int)ResultDataReader["AddAttack"],
                        AddDefence = (int)ResultDataReader["AddDefence"],
                        AddAgility = (int)ResultDataReader["AddAgility"],
                        AddLuck = (int)ResultDataReader["AddLuck"],
                        AddBlood = (int)ResultDataReader["AddBlood"],
                        AddDamage = (int)ResultDataReader["AddDamage"],
                        AddGuard = (int)ResultDataReader["AddGuard"],
                        Random = (int)ResultDataReader["Random"],
                        Page = (int)ResultDataReader["Page"],
                        Layers = (int)ResultDataReader["Layers"],
                        Location = (int)ResultDataReader["Location"],
                        Point = (int)ResultDataReader["Point"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetTotemAll", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public FightSpiritTemplateInfo[] GetAllFightSpiritTemplate()
        {
            List<FightSpiritTemplateInfo> list = new List<FightSpiritTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_FightSpiritTemplate_All");
                while (ResultDataReader.Read())
                    list.Add(new FightSpiritTemplateInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        FightSpiritID = (int)ResultDataReader["FightSpiritID"],
                        FightSpiritIcon = (string)ResultDataReader["FightSpiritIcon"],
                        Level = (int)ResultDataReader["Level"],
                        Exp = (int)ResultDataReader["Exp"],
                        Attack = (int)ResultDataReader["Attack"],
                        Defence = (int)ResultDataReader["Defence"],
                        Agility = (int)ResultDataReader["Agility"],
                        Lucky = (int)ResultDataReader["Lucky"],
                        Blood = (int)ResultDataReader["Blood"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetFightSpiritTemplateAll", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public TotemHonorTemplateInfo[] GetAllTotemHonorTemplate()
        {
            List<TotemHonorTemplateInfo> list = new List<TotemHonorTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_TotemHonorTemplate_All");
                while (ResultDataReader.Read())
                    list.Add(new TotemHonorTemplateInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        NeedMoney = (int)ResultDataReader["NeedMoney"],
                        Type = (int)ResultDataReader["Type"],
                        AddHonor = (int)ResultDataReader["AddHonor"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetTotemHonorTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public Dictionary<int, UserDrillInfo> GetPlayerDrillByID(int UserID)
        {
            Dictionary<int, UserDrillInfo> dictionary = new Dictionary<int, UserDrillInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_Users_Drill_All", SqlParameters);
                while (ResultDataReader.Read())
                {
                    UserDrillInfo userDrillInfo = new UserDrillInfo();
                    userDrillInfo.UserID = (int)ResultDataReader["UserID"];
                    userDrillInfo.BeadPlace = (int)ResultDataReader["BeadPlace"];
                    userDrillInfo.HoleLv = (int)ResultDataReader["HoleLv"];
                    userDrillInfo.HoleExp = (int)ResultDataReader["HoleExp"];
                    userDrillInfo.DrillPlace = (int)ResultDataReader["DrillPlace"];
                    if (!dictionary.ContainsKey(userDrillInfo.DrillPlace))
                        dictionary.Add(userDrillInfo.DrillPlace, userDrillInfo);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return dictionary;
        }

        public bool AddUserUserDrill(UserDrillInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6]
        {
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@BeadPlace", (object) item.BeadPlace),
          new SqlParameter("@HoleExp", (object) item.HoleExp),
          new SqlParameter("@HoleLv", (object) item.HoleLv),
          new SqlParameter("@DrillPlace", (object) item.DrillPlace),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_UserDrill_Add", SqlParameters);
                flag = (int)SqlParameters[5].Value == 0;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateUserDrillInfo(UserDrillInfo g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6]
        {
          new SqlParameter("@UserID", (object) g.UserID),
          new SqlParameter("@BeadPlace", (object) g.BeadPlace),
          new SqlParameter("@HoleExp", (object) g.HoleExp),
          new SqlParameter("@HoleLv", (object) g.HoleLv),
          new SqlParameter("@DrillPlace", (object) g.DrillPlace),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserDrillInfo", SqlParameters);
                flag = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateUserDrillInfo", ex);
            }
            return flag;
        }

        public TreasureAwardInfo[] GetAllTreasureAward()
        {
            List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Treasure_All");
                while (ResultDataReader.Read())
                    list.Add(new TreasureAwardInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        Name = (string)ResultDataReader["Name"],
                        Count = (int)ResultDataReader["Count"],
                        Validate = (int)ResultDataReader["Validate"],
                        Random = (int)ResultDataReader["Random"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetTreasureAwardAll", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public UserTreasureInfo GetSingleTreasure(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_GetSingleTreasure", SqlParameters);
                if (ResultDataReader.Read())
                    return new UserTreasureInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        NickName = (string)ResultDataReader["NickName"],
                        logoinDays = (int)ResultDataReader["logoinDays"],
                        treasure = (int)ResultDataReader["treasure"],
                        treasureAdd = (int)ResultDataReader["treasureAdd"],
                        friendHelpTimes = (int)ResultDataReader["friendHelpTimes"],
                        isEndTreasure = (bool)ResultDataReader["isEndTreasure"],
                        isBeginTreasure = (bool)ResultDataReader["isBeginTreasure"],
                        LastLoginDay = (DateTime)ResultDataReader["LastLoginDay"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleTreasure", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (UserTreasureInfo)null;
        }

        public List<TreasureDataInfo> GetSingleTreasureData(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<TreasureDataInfo> list = new List<TreasureDataInfo>();
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_GetSingleTreasureData", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new TreasureDataInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        Count = (int)ResultDataReader["Count"],
                        ValidDate = (int)ResultDataReader["Validate"],
                        pos = (int)ResultDataReader["pos"],
                        BeginDate = (DateTime)ResultDataReader["BeginDate"],
                        IsExit = (bool)ResultDataReader["IsExit"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleTreasureData", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public bool AddUserTreasureInfo(UserTreasureInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[11];
                SqlParameters[0] = new SqlParameter("@ID", (object)item.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[2] = new SqlParameter("@NickName", (object)item.NickName);
                SqlParameters[3] = new SqlParameter("@logoinDays", (object)item.logoinDays);
                SqlParameters[4] = new SqlParameter("@treasure", (object)item.treasure);
                SqlParameters[5] = new SqlParameter("@treasureAdd", (object)item.treasureAdd);
                SqlParameters[6] = new SqlParameter("@friendHelpTimes", (object)item.friendHelpTimes);
                SqlParameters[7] = new SqlParameter("@isEndTreasure", (object)(int)(item.isEndTreasure ? 1 : 0));
                SqlParameters[8] = new SqlParameter("@isBeginTreasure", (object)(int)(item.isBeginTreasure ? 1 : 0));
                SqlParameters[9] = new SqlParameter("@LastLoginDay", (object)item.LastLoginDay);
                SqlParameters[10] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[10].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_Treasure_Add", SqlParameters);
                flag = (int)SqlParameters[10].Value == 0;
                item.ID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateUserTreasureInfo(UserTreasureInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[10]
        {
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@NickName", (object) item.NickName),
          new SqlParameter("@logoinDays", (object) item.logoinDays),
          new SqlParameter("@treasure", (object) item.treasure),
          new SqlParameter("@treasureAdd", (object) item.treasureAdd),
          new SqlParameter("@friendHelpTimes", (object) item.friendHelpTimes),
          new SqlParameter("@isEndTreasure", (object) (int) (item.isEndTreasure ? 1 : 0)),
          new SqlParameter("@isBeginTreasure", (object) (int) (item.isBeginTreasure ? 1 : 0)),
          new SqlParameter("@LastLoginDay", (object) item.LastLoginDay),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserTreasure", SqlParameters);
                flag = (int)SqlParameters[9].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateUserTreasure", ex);
            }
            return flag;
        }

        public bool AddTreasureData(TreasureDataInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9];
                SqlParameters[0] = new SqlParameter("@ID", (object)item.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[2] = new SqlParameter("@TemplateID", (object)item.TemplateID);
                SqlParameters[3] = new SqlParameter("@Count", (object)item.Count);
                SqlParameters[4] = new SqlParameter("@Validate", (object)item.ValidDate);
                SqlParameters[5] = new SqlParameter("@Pos", (object)item.pos);
                SqlParameters[6] = new SqlParameter("@BeginDate", (object)item.BeginDate);
                SqlParameters[7] = new SqlParameter("@IsExit", (object)(int)(item.IsExit ? 1 : 0));
                SqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_TreasureData_Add", SqlParameters);
                flag = (int)SqlParameters[8].Value == 0;
                item.ID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateTreasureData(TreasureDataInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[9]
        {
          new SqlParameter("@ID", (object) item.ID),
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@TemplateID", (object) item.TemplateID),
          new SqlParameter("@Count", (object) item.Count),
          new SqlParameter("@Validate", (object) item.ValidDate),
          new SqlParameter("@Pos", (object) item.pos),
          new SqlParameter("@BeginDate", (object) item.BeginDate),
          new SqlParameter("@IsExit", (object) (int) (item.IsExit ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateTreasureData", SqlParameters);
                flag = (int)SqlParameters[8].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateTreasureData", ex);
            }
            return flag;
        }

        public bool RemoveTreasureDataByUser(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_RemoveTreasureDataByUser", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_RemoveTreasureDataByUser", ex);
            }
            return flag;
        }

        public bool RemoveIsArrange(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_RemoveIsArrange", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_RemoveIsArrange", ex);
            }
            return flag;
        }

        public bool UpdateFriendHelpTimes(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[2]
        {
          new SqlParameter("@UserID", (object) ID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateFriendHelpTimes", SqlParameters);
                flag = (int)SqlParameters[1].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateFriendHelpTimes", ex);
            }
            return flag;
        }

        public List<UserRankInfo> GetSingleUserRank(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<UserRankInfo> list = new List<UserRankInfo>();
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)UserID;
                this.db.GetReader(ref ResultDataReader, "SP_GetSingleUserRank", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(new UserRankInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        UserRank = (string)ResultDataReader["UserRank"],
                        Attack = (int)ResultDataReader["Attack"],
                        Defence = (int)ResultDataReader["Defence"],
                        Luck = (int)ResultDataReader["Luck"],
                        Agility = (int)ResultDataReader["Agility"],
                        HP = (int)ResultDataReader["HP"],
                        Damage = (int)ResultDataReader["Damage"],
                        Guard = (int)ResultDataReader["Guard"],
                        BeginDate = (DateTime)ResultDataReader["BeginDate"],
                        Validate = (int)ResultDataReader["Validate"],
                        IsExit = (bool)ResultDataReader["IsExit"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_GetSingleUserRankInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public bool AddUserRank(UserRankInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[14];
                SqlParameters[0] = new SqlParameter("@ID", (object)item.ID);
                SqlParameters[0].Direction = ParameterDirection.Output;
                SqlParameters[1] = new SqlParameter("@UserID", (object)item.UserID);
                SqlParameters[2] = new SqlParameter("@UserRank", (object)item.UserRank);
                SqlParameters[3] = new SqlParameter("@Attack", (object)item.Attack);
                SqlParameters[4] = new SqlParameter("@Defence", (object)item.Defence);
                SqlParameters[5] = new SqlParameter("@Luck", (object)item.Luck);
                SqlParameters[6] = new SqlParameter("@Agility", (object)item.Agility);
                SqlParameters[7] = new SqlParameter("@HP", (object)item.HP);
                SqlParameters[8] = new SqlParameter("@Damage", (object)item.Damage);
                SqlParameters[9] = new SqlParameter("@Guard", (object)item.Guard);
                SqlParameters[10] = new SqlParameter("@BeginDate", (object)item.BeginDate);
                SqlParameters[11] = new SqlParameter("@Validate", (object)item.Validate);
                SqlParameters[12] = new SqlParameter("@IsExit", (object)(int)(item.IsExit ? 1 : 0));
                SqlParameters[13] = new SqlParameter("@Result", SqlDbType.Int);
                SqlParameters[13].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserRank_Add", SqlParameters);
                flag = (int)SqlParameters[13].Value == 0;
                item.ID = (int)SqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool UpdateUserRank(UserRankInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[14]
        {
          new SqlParameter("@ID", (object) item.ID),
          new SqlParameter("@UserID", (object) item.UserID),
          new SqlParameter("@UserRank", (object) item.UserRank),
          new SqlParameter("@Attack", (object) item.Attack),
          new SqlParameter("@Defence", (object) item.Defence),
          new SqlParameter("@Luck", (object) item.Luck),
          new SqlParameter("@Agility", (object) item.Agility),
          new SqlParameter("@HP", (object) item.HP),
          new SqlParameter("@Damage", (object) item.Damage),
          new SqlParameter("@Guard", (object) item.Guard),
          new SqlParameter("@BeginDate", (object) item.BeginDate),
          new SqlParameter("@Validate", (object) item.Validate),
          new SqlParameter("@IsExit", (object) (int) (item.IsExit ? 1 : 0)),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[13].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserRank", SqlParameters);
                flag = (int)SqlParameters[13].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SP_UpdateUserRank", ex);
            }
            return flag;
        }
    }
}
