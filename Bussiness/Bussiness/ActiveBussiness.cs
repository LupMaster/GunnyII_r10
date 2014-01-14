using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class ActiveBussiness : BaseBussiness
    {
        public bool AddActiveNumber(string AwardID, int ActiveID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[3]
        {
          new SqlParameter("@AwardID", (object) AwardID),
          new SqlParameter("@ActiveID", (object) ActiveID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[2].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_Active_Number_Add", SqlParameters);
                flag = (int)SqlParameters[2].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public ActiveInfo[] GetAllActives()
        {
            List<ActiveInfo> list = new List<ActiveInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Active_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitActiveInfo(ResultDataReader));
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

        public ActiveInfo GetSingleActives(int activeID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)activeID;
                this.db.GetReader(ref ResultDataReader, "SP_Active_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitActiveInfo(ResultDataReader);
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
            return (ActiveInfo)null;
        }

        public ActiveInfo InitActiveInfo(SqlDataReader reader)
        {
            ActiveInfo activeInfo = new ActiveInfo();
            activeInfo.ActiveID = (int)reader["ActiveID"];
            activeInfo.Description = reader["Description"] == null ? "" : reader["Description"].ToString();
            activeInfo.Content = reader["Content"] == null ? "" : reader["Content"].ToString();
            activeInfo.AwardContent = reader["AwardContent"] == null ? "" : reader["AwardContent"].ToString();
            activeInfo.HasKey = (int)reader["HasKey"];
            if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
                activeInfo.EndDate = new DateTime?((DateTime)reader["EndDate"]);
            activeInfo.IsOnly = (int)reader["IsOnly"];
            activeInfo.StartDate = (DateTime)reader["StartDate"];
            activeInfo.Title = reader["Title"].ToString();
            activeInfo.Type = (int)reader["Type"];
            activeInfo.ActiveType = (int)reader["ActiveType"];
            activeInfo.ActionTimeContent = reader["ActionTimeContent"] == null ? "" : reader["ActionTimeContent"].ToString();
            activeInfo.IsAdvance = (bool)reader["IsAdvance"];
            activeInfo.GoodsExchangeTypes = reader["GoodsExchangeTypes"] == null ? "" : reader["GoodsExchangeTypes"].ToString();
            activeInfo.GoodsExchangeNum = reader["GoodsExchangeNum"] == null ? "" : reader["GoodsExchangeNum"].ToString();
            activeInfo.limitType = reader["limitType"] == null ? "" : reader["limitType"].ToString();
            activeInfo.limitValue = reader["limitValue"] == null ? "" : reader["limitValue"].ToString();
            activeInfo.IsShow = (bool)reader["IsShow"];
            activeInfo.IconID = (int)reader["IconID"];
            return activeInfo;
        }

        public ActiveConvertItemInfo[] GetSingleActiveConvertItems(int activeID)
        {
            List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)activeID;
                this.db.GetReader(ref ResultDataReader, "SP_Active_Convert_Item_Info_Single", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitActiveConvertItemInfo(ResultDataReader));
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

        public ActiveConvertItemInfo InitActiveConvertItemInfo(SqlDataReader reader)
        {
            return new ActiveConvertItemInfo()
            {
                ID = (int)reader["ID"],
                ActiveID = (int)reader["ActiveID"],
                TemplateID = (int)reader["TemplateID"],
                ItemType = (int)reader["ItemType"],
                ItemCount = (int)reader["ItemCount"],
                LimitValue = (int)reader["LimitValue"],
                IsBind = (bool)reader["IsBind"],
                ValidDate = (int)reader["ValidDate"]
            };
        }

        public int PullDown(int activeID, string awardID, int userID, ref string msg)
        {
            int num = 1;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@ActiveID", (object) activeID),
          new SqlParameter("@AwardID", (object) awardID),
          new SqlParameter("@UserID", (object) userID),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                if (this.db.RunProcedure("SP_Active_PullDown", SqlParameters))
                {
                    num = (int)SqlParameters[3].Value;
                    switch (num)
                    {
                        case 0:
                            msg = "ActiveBussiness.Msg0";
                            break;
                        case 1:
                            msg = "ActiveBussiness.Msg1";
                            break;
                        case 2:
                            msg = "ActiveBussiness.Msg2";
                            break;
                        case 3:
                            msg = "ActiveBussiness.Msg3";
                            break;
                        case 4:
                            msg = "ActiveBussiness.Msg4";
                            break;
                        case 5:
                            msg = "ActiveBussiness.Msg5";
                            break;
                        case 6:
                            msg = "ActiveBussiness.Msg6";
                            break;
                        case 7:
                            msg = "ActiveBussiness.Msg7";
                            break;
                        case 8:
                            msg = "ActiveBussiness.Msg8";
                            break;
                        default:
                            msg = "ActiveBussiness.Msg9";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return num;
        }
    }
}
