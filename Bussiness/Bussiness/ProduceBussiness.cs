using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class ProduceBussiness : BaseBussiness
    {
        public LoadUserBoxInfo[] GetAllTimeBoxAward()
        {
            List<LoadUserBoxInfo> list = new List<LoadUserBoxInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_TimeBox_Award_All");
                while (ResultDataReader.Read())
                    list.Add(new LoadUserBoxInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Type = (int)ResultDataReader["Type"],
                        Level = (int)ResultDataReader["Level"],
                        Condition = (int)ResultDataReader["Condition"],
                        TemplateID = (int)ResultDataReader["TemplateID"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllDaily", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public GoldEquipTemplateLoadInfo[] GetAllGoldEquipTemplateLoad()
        {
            List<GoldEquipTemplateLoadInfo> list = new List<GoldEquipTemplateLoadInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_GoldEquipTemplateLoad_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitGoldEquipTemplateLoad(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllGoldEquipTemplateLoad", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public GoldEquipTemplateLoadInfo InitGoldEquipTemplateLoad(SqlDataReader reader)
        {
            return new GoldEquipTemplateLoadInfo()
            {
                ID = (int)reader["ID"],
                OldTemplateId = (int)reader["OldTemplateId"],
                NewTemplateId = (int)reader["NewTemplateId"],
                CategoryID = (int)reader["CategoryID"],
                Strengthen = (int)reader["Strengthen"],
                Attack = (int)reader["Attack"],
                Defence = (int)reader["Defence"],
                Agility = (int)reader["Agility"],
                Luck = (int)reader["Luck"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Boold = (int)reader["Boold"],
                BlessID = (int)reader["BlessID"],
                Pic = reader["pic"] == null ? "" : reader["pic"].ToString()
            };
        }

        public AchievementInfo[] GetALlAchievement()
        {
            List<AchievementInfo> list = new List<AchievementInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Achievement_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitAchievement(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetALlAchievement:", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public AchievementConditionInfo[] GetALlAchievementCondition()
        {
            List<AchievementConditionInfo> list = new List<AchievementConditionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Achievement_Condition_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitAchievementCondition(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetALlAchievementCondition:", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public AchievementDataInfo[] GetAllAchievementData(int userID)
        {
            List<AchievementDataInfo> list = new List<AchievementDataInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) userID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Achievement_Data_All", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitAchievementData(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllAchievementData", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public AchievementRewardInfo[] GetALlAchievementReward()
        {
            List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Achievement_Reward_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitAchievementReward(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetALlAchievementReward", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ActiveAwardInfo[] GetAllActiveAwardInfo()
        {
            List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Active_Award");
                while (ResultDataReader.Read())
                {
                    ActiveAwardInfo activeAwardInfo = new ActiveAwardInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        ActiveID = (int)ResultDataReader["ActiveID"],
                        AgilityCompose = (int)ResultDataReader["AgilityCompose"],
                        AttackCompose = (int)ResultDataReader["AttackCompose"],
                        Count = (int)ResultDataReader["Count"],
                        DefendCompose = (int)ResultDataReader["DefendCompose"],
                        Gold = (int)ResultDataReader["Gold"],
                        ItemID = (int)ResultDataReader["ItemID"],
                        LuckCompose = (int)ResultDataReader["LuckCompose"],
                        Mark = (int)ResultDataReader["Mark"],
                        Money = (int)ResultDataReader["Money"],
                        Sex = (int)ResultDataReader["Sex"],
                        StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
                        ValidDate = (int)ResultDataReader["ValidDate"],
                        GiftToken = (int)ResultDataReader["GiftToken"]
                    };
                    list.Add(activeAwardInfo);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllActiveAwardInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ActiveConditionInfo[] GetAllActiveConditionInfo()
        {
            List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Active_Condition");
                while (ResultDataReader.Read())
                {
                    ActiveConditionInfo activeConditionInfo = new ActiveConditionInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        ActiveID = (int)ResultDataReader["ActiveID"],
                        Conditiontype = (int)ResultDataReader["Conditiontype"],
                        Condition = (int)ResultDataReader["Condition"],
                        LimitGrade = ResultDataReader["LimitGrade"].ToString() == null ? "" : ResultDataReader["LimitGrade"].ToString(),
                        AwardId = ResultDataReader["AwardId"].ToString() == null ? "" : ResultDataReader["AwardId"].ToString(),
                        IsMult = (bool)ResultDataReader["IsMult"],
                        StartTime = (DateTime)ResultDataReader["StartTime"],
                        EndTime = (DateTime)ResultDataReader["EndTime"]
                    };
                    list.Add(activeConditionInfo);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllActiveConditionInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public List<BigBugleInfo> GetAllAreaBigBugleRecord()
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            List<BigBugleInfo> list = new List<BigBugleInfo>();
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Get_AreaBigBugle_Record");
                while (ResultDataReader.Read())
                {
                    BigBugleInfo bigBugleInfo = new BigBugleInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        AreaID = (int)ResultDataReader["AreaID"],
                        NickName = ResultDataReader["NickName"] == null ? "" : ResultDataReader["NickName"].ToString(),
                        Message = ResultDataReader["Message"] == null ? "" : ResultDataReader["Message"].ToString(),
                        State = (bool)ResultDataReader["State"],
                        IP = ResultDataReader["IP"] == null ? "" : ResultDataReader["IP"].ToString()
                    };
                    list.Add(bigBugleInfo);
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllAreaBigBugleRecord", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public AchievementInfo InitAchievement(SqlDataReader reader)
        {
            return new AchievementInfo()
            {
                ID = (int)reader["ID"],
                PlaceID = (int)reader["PlaceID"],
                Title = reader["Title"] == null ? "" : reader["Title"].ToString(),
                Detail = reader["Detail"] == null ? "" : reader["Detail"].ToString(),
                NeedMinLevel = (int)reader["NeedMinLevel"],
                NeedMaxLevel = (int)reader["NeedMaxLevel"],
                PreAchievementID = reader["PreAchievementID"] == null ? "" : reader["PreAchievementID"].ToString(),
                IsOther = (int)reader["IsOther"],
                AchievementType = (int)reader["AchievementType"],
                CanHide = (bool)reader["CanHide"],
                StartDate = (DateTime)reader["StartDate"],
                EndDate = (DateTime)reader["EndDate"],
                AchievementPoint = (int)reader["AchievementPoint"],
                IsActive = (int)reader["IsActive"],
                PicID = (int)reader["PicID"],
                IsShare = (bool)reader["IsShare"]
            };
        }

        public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
        {
            return new AchievementConditionInfo()
            {
                AchievementID = (int)reader["AchievementID"],
                CondictionID = (int)reader["CondictionID"],
                CondictionType = (int)reader["CondictionType"],
                Condiction_Para1 = reader["Condiction_Para1"] == null ? "" : reader["Condiction_Para1"].ToString(),
                Condiction_Para2 = (int)reader["Condiction_Para2"]
            };
        }

        public AchievementDataInfo InitAchievementData(SqlDataReader reader)
        {
            return new AchievementDataInfo()
            {
                UserID = (int)reader["UserID"],
                AchievementID = (int)reader["AchievementID"],
                IsComplete = (bool)reader["IsComplete"],
                CompletedDate = (DateTime)reader["CompletedDate"]
            };
        }

        public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
        {
            return new AchievementRewardInfo()
            {
                AchievementID = (int)reader["AchievementID"],
                RewardType = (int)reader["RewardType"],
                RewardPara = reader["RewardPara"] == null ? "" : reader["RewardPara"].ToString(),
                RewardValueId = (int)reader["RewardValueId"],
                RewardCount = (int)reader["RewardCount"]
            };
        }

        public ItemRecordTypeInfo[] GetAllItemRecordType()
        {
            List<ItemRecordTypeInfo> list = new List<ItemRecordTypeInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Item_Record_Type_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitItemRecordType(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllItemRecordType:", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
        {
            return new ItemRecordTypeInfo()
            {
                RecordID = (int)reader["RecordID"],
                Name = reader["Name"] == null ? "" : reader["Name"].ToString(),
                Description = reader["Description"] == null ? "" : reader["Description"].ToString()
            };
        }

        public ItemTemplateInfo[] GetAllGoodsASC()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Items_All_ASC");
                while (ResultDataReader.Read())
                    list.Add(this.InitItemTemplateInfo(ResultDataReader));
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

        public ItemTemplateInfo[] GetAllGoods()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Items_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitItemTemplateInfo(ResultDataReader));
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

        public ShopGoodsShowListInfo InitShopGoodsShowListInfo(SqlDataReader reader)
        {
            return new ShopGoodsShowListInfo()
            {
                Type = (int)reader["Type"],
                ShopId = (int)reader["ShopId"]
            };
        }

        public ShopGoodsShowListInfo[] GetAllShopGoodsShowList()
        {
            List<ShopGoodsShowListInfo> list = new List<ShopGoodsShowListInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_ShopGoodsShowList_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitShopGoodsShowListInfo(ResultDataReader));
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

        public ItemBoxInfo[] GetSingleItemsBox(int DataID)
        {
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)DataID;
                this.db.GetReader(ref ResultDataReader, "SP_ItemsBox_Single", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItemBoxInfo(ResultDataReader));
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

        public ItemTemplateInfo GetSingleGoods(int goodsID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@ID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)goodsID;
                this.db.GetReader(ref ResultDataReader, "SP_Items_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitItemTemplateInfo(ResultDataReader);
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
            return (ItemTemplateInfo)null;
        }

        public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@CategoryID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)CategoryID;
                this.db.GetReader(ref ResultDataReader, "SP_Items_Category_Single", SqlParameters);
                while (ResultDataReader.Read())
                    list.Add(this.InitItemTemplateInfo(ResultDataReader));
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

        public ItemTemplateInfo[] GetFusionType()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Items_FusionType");
                while (ResultDataReader.Read())
                    list.Add(this.InitItemTemplateInfo(ResultDataReader));
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

        public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
        {
            ItemTemplateInfo itemTemplateInfo = new ItemTemplateInfo();
            itemTemplateInfo.AddTime = reader["AddTime"].ToString();
            itemTemplateInfo.Agility = (int)reader["Agility"];
            itemTemplateInfo.Attack = (int)reader["Attack"];
            itemTemplateInfo.CanDelete = (bool)reader["CanDelete"];
            itemTemplateInfo.CanDrop = (bool)reader["CanDrop"];
            itemTemplateInfo.CanEquip = (bool)reader["CanEquip"];
            itemTemplateInfo.CanUse = (bool)reader["CanUse"];
            itemTemplateInfo.CategoryID = (int)reader["CategoryID"];
            itemTemplateInfo.Colors = reader["Colors"].ToString();
            itemTemplateInfo.Defence = (int)reader["Defence"];
            itemTemplateInfo.Description = reader["Description"].ToString();
            itemTemplateInfo.Level = (int)reader["Level"];
            itemTemplateInfo.Luck = (int)reader["Luck"];
            itemTemplateInfo.MaxCount = (int)reader["MaxCount"];
            itemTemplateInfo.Name = reader["Name"].ToString();
            itemTemplateInfo.NeedSex = (int)reader["NeedSex"];
            itemTemplateInfo.Pic = reader["Pic"].ToString();
            itemTemplateInfo.Data = reader["Data"] == null ? "" : reader["Data"].ToString();
            itemTemplateInfo.Property1 = (int)reader["Property1"];
            itemTemplateInfo.Property2 = (int)reader["Property2"];
            itemTemplateInfo.Property3 = (int)reader["Property3"];
            itemTemplateInfo.Property4 = (int)reader["Property4"];
            itemTemplateInfo.Property5 = (int)reader["Property5"];
            itemTemplateInfo.Property6 = (int)reader["Property6"];
            itemTemplateInfo.Property7 = (int)reader["Property7"];
            itemTemplateInfo.Property8 = (int)reader["Property8"];
            itemTemplateInfo.Quality = (int)reader["Quality"];
            itemTemplateInfo.Script = reader["Script"].ToString();
            itemTemplateInfo.TemplateID = (int)reader["TemplateID"];
            itemTemplateInfo.CanCompose = (bool)reader["CanCompose"];
            itemTemplateInfo.CanStrengthen = (bool)reader["CanStrengthen"];
            itemTemplateInfo.NeedLevel = (int)reader["NeedLevel"];
            itemTemplateInfo.BindType = (int)reader["BindType"];
            itemTemplateInfo.FusionType = (int)reader["FusionType"];
            itemTemplateInfo.FusionRate = (int)reader["FusionRate"];
            itemTemplateInfo.FusionNeedRate = (int)reader["FusionNeedRate"];
            itemTemplateInfo.Hole = reader["Hole"] == null ? "" : reader["Hole"].ToString();
            itemTemplateInfo.RefineryLevel = (int)reader["RefineryLevel"];
            itemTemplateInfo.ReclaimValue = (int)reader["ReclaimValue"];
            itemTemplateInfo.ReclaimType = (int)reader["ReclaimType"];
            itemTemplateInfo.CanRecycle = (int)reader["CanRecycle"];
            itemTemplateInfo.SuitId = (int)reader["SuitId"];
            itemTemplateInfo.FloorPrice = (int)reader["FloorPrice"];
            itemTemplateInfo.IsDirty = false;
            return itemTemplateInfo;
        }

        public ItemBoxInfo[] GetItemBoxInfos()
        {
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_ItemsBox_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitItemBoxInfo(ResultDataReader));
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)("Init@Shop_Goods_Box：" + (object)ex));
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
        {
            return new ItemBoxInfo()
            {
                Id = (int)reader["id"],
                DataId = (int)reader["DataId"],
                TemplateId = (int)reader["TemplateId"],
                IsSelect = (bool)reader["IsSelect"],
                IsBind = (bool)reader["IsBind"],
                ItemValid = (int)reader["ItemValid"],
                ItemCount = (int)reader["ItemCount"],
                StrengthenLevel = (int)reader["StrengthenLevel"],
                AttackCompose = (int)reader["AttackCompose"],
                DefendCompose = (int)reader["DefendCompose"],
                AgilityCompose = (int)reader["AgilityCompose"],
                LuckCompose = (int)reader["LuckCompose"],
                Random = (int)reader["Random"],
                IsTips = (int)reader["IsTips"],
                IsLogs = (bool)reader["IsLogs"]
            };
        }

        public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@LastQuestsTime", (object) info.LastQuestsTime),
          new SqlParameter("@LastTreasureTime", (object) info.LastTreasureTime),
          new SqlParameter("@OutPut", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_User_Update_History", SqlParameters);
                flag = (int)SqlParameters[6].Value == 1;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"User_Update_BoxProgression", ex);
            }
            return flag;
        }

        public CategoryInfo[] GetAllCategory()
        {
            List<CategoryInfo> list = new List<CategoryInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Items_Category_All");
                while (ResultDataReader.Read())
                    list.Add(new CategoryInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"] == null ? "" : ResultDataReader["Name"].ToString(),
                        Place = (int)ResultDataReader["Place"],
                        Remark = ResultDataReader["Remark"] == null ? "" : ResultDataReader["Remark"].ToString()
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

        public PropInfo[] GetAllProp()
        {
            List<PropInfo> list = new List<PropInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Prop_All");
                while (ResultDataReader.Read())
                    list.Add(new PropInfo()
                    {
                        AffectArea = (int)ResultDataReader["AffectArea"],
                        AffectTimes = (int)ResultDataReader["AffectTimes"],
                        AttackTimes = (int)ResultDataReader["AttackTimes"],
                        BoutTimes = (int)ResultDataReader["BoutTimes"],
                        BuyGold = (int)ResultDataReader["BuyGold"],
                        BuyMoney = (int)ResultDataReader["BuyMoney"],
                        Category = (int)ResultDataReader["Category"],
                        Delay = (int)ResultDataReader["Delay"],
                        Description = ResultDataReader["Description"].ToString(),
                        Icon = ResultDataReader["Icon"].ToString(),
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        Parameter = (int)ResultDataReader["Parameter"],
                        Pic = ResultDataReader["Pic"].ToString(),
                        Property1 = (int)ResultDataReader["Property1"],
                        Property2 = (int)ResultDataReader["Property2"],
                        Property3 = (int)ResultDataReader["Property3"],
                        Random = (int)ResultDataReader["Random"],
                        Script = ResultDataReader["Script"].ToString()
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

        public BallInfo[] GetAllBall()
        {
            List<BallInfo> list = new List<BallInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Ball_All");
                while (ResultDataReader.Read())
                    list.Add(new BallInfo()
                    {
                        Amount = (int)ResultDataReader["Amount"],
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        Crater = ResultDataReader["Crater"] == null ? "" : ResultDataReader["Crater"].ToString(),
                        Power = (double)ResultDataReader["Power"],
                        Radii = (int)ResultDataReader["Radii"],
                        AttackResponse = (int)ResultDataReader["AttackResponse"],
                        BombPartical = ResultDataReader["BombPartical"].ToString(),
                        FlyingPartical = ResultDataReader["FlyingPartical"].ToString(),
                        IsSpin = (bool)ResultDataReader["IsSpin"],
                        Mass = (int)ResultDataReader["Mass"],
                        SpinV = (int)ResultDataReader["SpinV"],
                        SpinVA = (double)ResultDataReader["SpinVA"],
                        Wind = (int)ResultDataReader["Wind"],
                        DragIndex = (int)ResultDataReader["DragIndex"],
                        Weight = (int)ResultDataReader["Weight"],
                        Shake = (bool)ResultDataReader["Shake"],
                        Delay = (int)ResultDataReader["Delay"],
                        ShootSound = ResultDataReader["ShootSound"] == null ? "" : ResultDataReader["ShootSound"].ToString(),
                        BombSound = ResultDataReader["BombSound"] == null ? "" : ResultDataReader["BombSound"].ToString(),
                        ActionType = (int)ResultDataReader["ActionType"],
                        HasTunnel = (bool)ResultDataReader["HasTunnel"]
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

        public BallConfigInfo[] GetAllBallConfig()
        {
            List<BallConfigInfo> list = new List<BallConfigInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "[SP_Ball_Config_All]");
                while (ResultDataReader.Read())
                    list.Add(new BallConfigInfo()
                    {
                        Common = (int)ResultDataReader["Common"],
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        CommonAddWound = (int)ResultDataReader["CommonAddWound"],
                        CommonMultiBall = (int)ResultDataReader["CommonMultiBall"],
                        Special = (int)ResultDataReader["Special"]
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

        public ShopItemInfo[] GetALllShop()
        {
            List<ShopItemInfo> list = new List<ShopItemInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Shop_All");
                while (ResultDataReader.Read())
                    list.Add(new ShopItemInfo()
                    {
                        ID = int.Parse(ResultDataReader["ID"].ToString()),
                        ShopID = int.Parse(ResultDataReader["ShopID"].ToString()),
                        GroupID = int.Parse(ResultDataReader["GroupID"].ToString()),
                        TemplateID = int.Parse(ResultDataReader["TemplateID"].ToString()),
                        BuyType = int.Parse(ResultDataReader["BuyType"].ToString()),
                        Sort = int.Parse(ResultDataReader["Sort"].ToString()),
                        IsVouch = int.Parse(ResultDataReader["IsVouch"].ToString()),
                        Label = (float)int.Parse(ResultDataReader["Label"].ToString()),
                        Beat = Decimal.Parse(ResultDataReader["Beat"].ToString()),
                        AUnit = int.Parse(ResultDataReader["AUnit"].ToString()),
                        APrice1 = int.Parse(ResultDataReader["APrice1"].ToString()),
                        AValue1 = int.Parse(ResultDataReader["AValue1"].ToString()),
                        APrice2 = int.Parse(ResultDataReader["APrice2"].ToString()),
                        AValue2 = int.Parse(ResultDataReader["AValue2"].ToString()),
                        APrice3 = int.Parse(ResultDataReader["APrice3"].ToString()),
                        AValue3 = int.Parse(ResultDataReader["AValue3"].ToString()),
                        BUnit = int.Parse(ResultDataReader["BUnit"].ToString()),
                        BPrice1 = int.Parse(ResultDataReader["BPrice1"].ToString()),
                        BValue1 = int.Parse(ResultDataReader["BValue1"].ToString()),
                        BPrice2 = int.Parse(ResultDataReader["BPrice2"].ToString()),
                        BValue2 = int.Parse(ResultDataReader["BValue2"].ToString()),
                        BPrice3 = int.Parse(ResultDataReader["BPrice3"].ToString()),
                        BValue3 = int.Parse(ResultDataReader["BValue3"].ToString()),
                        CUnit = int.Parse(ResultDataReader["CUnit"].ToString()),
                        CPrice1 = int.Parse(ResultDataReader["CPrice1"].ToString()),
                        CValue1 = int.Parse(ResultDataReader["CValue1"].ToString()),
                        CPrice2 = int.Parse(ResultDataReader["CPrice2"].ToString()),
                        CValue2 = int.Parse(ResultDataReader["CValue2"].ToString()),
                        CPrice3 = int.Parse(ResultDataReader["CPrice3"].ToString()),
                        CValue3 = int.Parse(ResultDataReader["CValue3"].ToString()),
                        IsContinue = bool.Parse(ResultDataReader["IsContinue"].ToString()),
                        IsCheap = bool.Parse(ResultDataReader["IsCheap"].ToString()),
                        LimitCount = (float)int.Parse(ResultDataReader["LimitCount"].ToString()),
                        StartDate = DateTime.Parse(ResultDataReader["StartDate"].ToString()),
                        EndDate = DateTime.Parse(ResultDataReader["EndDate"].ToString())
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

        public FusionInfo[] GetAllFusionDesc()
        {
            List<FusionInfo> list = new List<FusionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Fusion_All_Desc");
                while (ResultDataReader.Read())
                    list.Add(new FusionInfo()
                    {
                        FusionID = (int)ResultDataReader["FusionID"],
                        Item1 = (int)ResultDataReader["Item1"],
                        Item2 = (int)ResultDataReader["Item2"],
                        Item3 = (int)ResultDataReader["Item3"],
                        Item4 = (int)ResultDataReader["Item4"],
                        Formula = (int)ResultDataReader["Formula"],
                        Reward = (int)ResultDataReader["Reward"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllFusion", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public FusionInfo[] GetAllFusion()
        {
            List<FusionInfo> list = new List<FusionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Fusion_All");
                while (ResultDataReader.Read())
                    list.Add(new FusionInfo()
                    {
                        FusionID = (int)ResultDataReader["FusionID"],
                        Item1 = (int)ResultDataReader["Item1"],
                        Item2 = (int)ResultDataReader["Item2"],
                        Item3 = (int)ResultDataReader["Item3"],
                        Item4 = (int)ResultDataReader["Item4"],
                        Formula = (int)ResultDataReader["Formula"],
                        Reward = (int)ResultDataReader["Reward"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllFusion", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public StrengthenInfo[] GetAllStrengthen()
        {
            List<StrengthenInfo> list = new List<StrengthenInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Item_Strengthen_All");
                while (ResultDataReader.Read())
                    list.Add(new StrengthenInfo()
                    {
                        StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
                        Random = (int)ResultDataReader["Random"],
                        Rock = (int)ResultDataReader["Rock"],
                        Rock1 = (int)ResultDataReader["Rock1"],
                        Rock2 = (int)ResultDataReader["Rock2"],
                        Rock3 = (int)ResultDataReader["Rock3"],
                        StoneLevelMin = (int)ResultDataReader["StoneLevelMin"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllStrengthen", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public RuneTemplateInfo[] GetAllRuneTemplate()
        {
            List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_RuneTemplate_All");
                while (ResultDataReader.Read())
                    list.Add(new RuneTemplateInfo()
                    {
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        NextTemplateID = (int)ResultDataReader["NextTemplateID"],
                        Name = (string)ResultDataReader["Name"],
                        BaseLevel = (int)ResultDataReader["BaseLevel"],
                        MaxLevel = (int)ResultDataReader["MaxLevel"],
                        Type1 = (int)ResultDataReader["Type1"],
                        Attribute1 = (string)ResultDataReader["Attribute1"],
                        Turn1 = (int)ResultDataReader["Turn1"],
                        Rate1 = (int)ResultDataReader["Rate1"],
                        Type2 = (int)ResultDataReader["Type2"],
                        Attribute2 = (string)ResultDataReader["Attribute2"],
                        Turn2 = (int)ResultDataReader["Turn2"],
                        Rate2 = (int)ResultDataReader["Rate2"],
                        Type3 = (int)ResultDataReader["Type3"],
                        Attribute3 = (string)ResultDataReader["Attribute3"],
                        Turn3 = (int)ResultDataReader["Turn3"],
                        Rate3 = (int)ResultDataReader["Rate3"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetRuneTemplateInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public StrengThenExpInfo[] GetAllStrengThenExp()
        {
            List<StrengThenExpInfo> list = new List<StrengThenExpInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_StrengThenExp_All");
                while (ResultDataReader.Read())
                    list.Add(new StrengThenExpInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Level = (int)ResultDataReader["Level"],
                        Exp = (int)ResultDataReader["Exp"],
                        NecklaceStrengthExp = (int)ResultDataReader["NecklaceStrengthExp"],
                        NecklaceStrengthPlus = (int)ResultDataReader["NecklaceStrengthPlus"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetStrengThenExpInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
        {
            List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Item_StrengthenGoodsInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new StrengthenGoodsInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Level = (int)ResultDataReader["Level"],
                        CurrentEquip = (int)ResultDataReader["CurrentEquip"],
                        GainEquip = (int)ResultDataReader["GainEquip"],
                        OrginEquip = (int)ResultDataReader["OrginEquip"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllStrengthenGoodsInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public StrengthenInfo[] GetAllRefineryStrengthen()
        {
            List<StrengthenInfo> list = new List<StrengthenInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Item_Refinery_Strengthen_All");
                while (ResultDataReader.Read())
                    list.Add(new StrengthenInfo()
                    {
                        StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
                        Rock = (int)ResultDataReader["Rock"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllRefineryStrengthen", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public List<RefineryInfo> GetAllRefineryInfo()
        {
            List<RefineryInfo> list = new List<RefineryInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Item_Refinery_All");
                while (ResultDataReader.Read())
                    list.Add(new RefineryInfo()
                    {
                        RefineryID = (int)ResultDataReader["RefineryID"],
                        m_Equip = {
              (int) ResultDataReader["Equip1"],
              (int) ResultDataReader["Equip2"],
              (int) ResultDataReader["Equip3"],
              (int) ResultDataReader["Equip4"]
            },
                        Item1 = (int)ResultDataReader["Item1"],
                        Item2 = (int)ResultDataReader["Item2"],
                        Item3 = (int)ResultDataReader["Item3"],
                        Item1Count = (int)ResultDataReader["Item1Count"],
                        Item2Count = (int)ResultDataReader["Item2Count"],
                        Item3Count = (int)ResultDataReader["Item3Count"],
                        m_Reward = {
              (int) ResultDataReader["Material1"],
              (int) ResultDataReader["Operate1"],
              (int) ResultDataReader["Reward1"],
              (int) ResultDataReader["Material2"],
              (int) ResultDataReader["Operate2"],
              (int) ResultDataReader["Reward2"],
              (int) ResultDataReader["Material3"],
              (int) ResultDataReader["Operate3"],
              (int) ResultDataReader["Reward3"],
              (int) ResultDataReader["Material4"],
              (int) ResultDataReader["Operate4"],
              (int) ResultDataReader["Reward4"]
            }
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllRefineryInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list;
        }

        public QuestInfo[] GetALlQuest()
        {
            List<QuestInfo> list = new List<QuestInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Quest_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitQuest(ResultDataReader));
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

        public QuestAwardInfo[] GetAllQuestGoods()
        {
            List<QuestAwardInfo> list = new List<QuestAwardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Quest_Goods_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitQuestGoods(ResultDataReader));
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

        public QuestConditionInfo[] GetAllQuestCondiction()
        {
            List<QuestConditionInfo> list = new List<QuestConditionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Quest_Condiction_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitQuestCondiction(ResultDataReader));
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

        public QuestRateInfo[] GetAllQuestRate()
        {
            List<QuestRateInfo> list = new List<QuestRateInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Quest_Rate_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitQuestRate(ResultDataReader));
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

        public QuestInfo GetSingleQuest(int questID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@QuestID", SqlDbType.Int, 4)
        };
                SqlParameters[0].Value = (object)questID;
                this.db.GetReader(ref ResultDataReader, "SP_Quest_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return this.InitQuest(ResultDataReader);
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
            return (QuestInfo)null;
        }

        public QuestInfo InitQuest(SqlDataReader reader)
        {
            return new QuestInfo()
            {
                ID = (int)reader["ID"],
                QuestID = (int)reader["QuestID"],
                Title = reader["Title"] == null ? "" : reader["Title"].ToString(),
                Detail = reader["Detail"] == null ? "" : reader["Detail"].ToString(),
                Objective = reader["Objective"] == null ? "" : reader["Objective"].ToString(),
                NeedMinLevel = (int)reader["NeedMinLevel"],
                NeedMaxLevel = (int)reader["NeedMaxLevel"],
                PreQuestID = reader["PreQuestID"] == null ? "" : reader["PreQuestID"].ToString(),
                NextQuestID = reader["NextQuestID"] == null ? "" : reader["NextQuestID"].ToString(),
                IsOther = (int)reader["IsOther"],
                CanRepeat = (bool)reader["CanRepeat"],
                RepeatInterval = (int)reader["RepeatInterval"],
                RepeatMax = (int)reader["RepeatMax"],
                RewardGP = (int)reader["RewardGP"],
                RewardGold = (int)reader["RewardGold"],
                RewardBindMoney = (int)reader["RewardBindMoney"],
                RewardOffer = (int)reader["RewardOffer"],
                RewardRiches = (int)reader["RewardRiches"],
                RewardBuffID = (int)reader["RewardBuffID"],
                RewardBuffDate = (int)reader["RewardBuffDate"],
                RewardMoney = (int)reader["RewardMoney"],
                Rands = (Decimal)reader["Rands"],
                RandDouble = (int)reader["RandDouble"],
                TimeMode = (bool)reader["TimeMode"],
                StartDate = (DateTime)reader["StartDate"],
                EndDate = (DateTime)reader["EndDate"]
            };
        }

        public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
        {
            return new QuestAwardInfo()
            {
                QuestID = (int)reader["QuestID"],
                RewardItemID = (int)reader["RewardItemID"],
                IsSelect = (bool)reader["IsSelect"],
                RewardItemValid = (int)reader["RewardItemValid"],
                RewardItemCount1 = (int)reader["RewardItemCount1"],
                RewardItemCount2 = (int)reader["RewardItemCount2"],
                RewardItemCount3 = (int)reader["RewardItemCount3"],
                RewardItemCount4 = (int)reader["RewardItemCount4"],
                RewardItemCount5 = (int)reader["RewardItemCount5"],
                StrengthenLevel = (int)reader["StrengthenLevel"],
                AttackCompose = (int)reader["AttackCompose"],
                DefendCompose = (int)reader["DefendCompose"],
                AgilityCompose = (int)reader["AgilityCompose"],
                LuckCompose = (int)reader["LuckCompose"],
                IsCount = (bool)reader["IsCount"],
                IsBind = (bool)reader["IsBind"]
            };
        }

        public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
        {
            return new QuestConditionInfo()
            {
                QuestID = (int)reader["QuestID"],
                CondictionID = (int)reader["CondictionID"],
                CondictionTitle = reader["CondictionTitle"] == null ? "" : reader["CondictionTitle"].ToString(),
                CondictionType = (int)reader["CondictionType"],
                Para1 = (int)reader["Para1"],
                Para2 = (int)reader["Para2"],
                isOpitional = (bool)reader["isOpitional"]
            };
        }

        public QuestRateInfo InitQuestRate(SqlDataReader reader)
        {
            return new QuestRateInfo()
            {
                BindMoneyRate = reader["BindMoneyRate"] == null ? "" : reader["BindMoneyRate"].ToString(),
                ExpRate = reader["ExpRate"] == null ? "" : reader["ExpRate"].ToString(),
                GoldRate = reader["GoldRate"] == null ? "" : reader["GoldRate"].ToString(),
                ExploitRate = reader["ExploitRate"] == null ? "" : reader["ExploitRate"].ToString(),
                CanOneKeyFinishTime = (int)reader["CanOneKeyFinishTime"]
            };
        }

        public DropCondiction[] GetAllDropCondictions()
        {
            List<DropCondiction> list = new List<DropCondiction>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Drop_Condiction_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitDropCondiction(ResultDataReader));
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

        public DropItem[] GetAllDropItems()
        {
            List<DropItem> list = new List<DropItem>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Drop_Item_All");
                while (ResultDataReader.Read())
                    list.Add(this.InitDropItem(ResultDataReader));
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

        public DropCondiction InitDropCondiction(SqlDataReader reader)
        {
            return new DropCondiction()
            {
                DropId = (int)reader["DropID"],
                CondictionType = (int)reader["CondictionType"],
                Para1 = (string)reader["Para1"],
                Para2 = (string)reader["Para2"]
            };
        }

        public DropItem InitDropItem(SqlDataReader reader)
        {
            return new DropItem()
            {
                Id = (int)reader["Id"],
                DropId = (int)reader["DropId"],
                ItemId = (int)reader["ItemId"],
                ValueDate = (int)reader["ValueDate"],
                IsBind = (bool)reader["IsBind"],
                Random = (int)reader["Random"],
                BeginData = (int)reader["BeginData"],
                EndData = (int)reader["EndData"],
                IsLogs = (bool)reader["IsLogs"],
                IsTips = (bool)reader["IsTips"]
            };
        }

        public AASInfo[] GetAllAASInfo()
        {
            List<AASInfo> list = new List<AASInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_AASInfo_All");
                while (ResultDataReader.Read())
                    list.Add(new AASInfo()
                    {
                        UserID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"].ToString(),
                        IDNumber = ResultDataReader["IDNumber"].ToString(),
                        State = (int)ResultDataReader["State"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllAASInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public bool AddAASInfo(AASInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[5]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@Name", (object) info.Name),
          new SqlParameter("@IDNumber", (object) info.IDNumber),
          new SqlParameter("@State", (object) info.State),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[4].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_ASSInfo_Add", SqlParameters);
                flag = (int)SqlParameters[4].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateAASInfo", ex);
            }
            return flag;
        }

        public string GetASSInfoSingle(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) UserID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_ASSInfo_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return ResultDataReader["IDNumber"].ToString();
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetASSInfoSingle", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return "";
        }

        public bool AddDailyLogList(DailyLogListInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[5]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@UserAwardLog", (object) info.UserAwardLog),
          new SqlParameter("@DayLog", (object) info.DayLog),
          new SqlParameter("@Result", SqlDbType.Int),
          null
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_DailyLogList_Add", SqlParameters);
                flag = (int)SqlParameters[3].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateAASInfo", ex);
            }
            return flag;
        }

        public bool UpdateDailyLogList(DailyLogListInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[5]
        {
          new SqlParameter("@UserID", (object) info.UserID),
          new SqlParameter("@UserAwardLog", (object) info.UserAwardLog),
          new SqlParameter("@DayLog", (object) info.DayLog),
          new SqlParameter("@LastDate", (object) info.LastDate.ToString()),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[4].Direction = ParameterDirection.ReturnValue;
                flag = this.db.RunProcedure("SP_DailyLogList_Update", SqlParameters);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"User_Update_BoxProgression", ex);
            }
            return flag;
        }

        public DailyLogListInfo GetDailyLogListSingle(int UserID)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@UserID", (object) UserID)
        };
                this.db.GetReader(ref ResultDataReader, "SP_DailyLogList_Single", SqlParameters);
                if (ResultDataReader.Read())
                    return new DailyLogListInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        UserID = (int)ResultDataReader["UserID"],
                        UserAwardLog = (int)ResultDataReader["UserAwardLog"],
                        DayLog = (string)ResultDataReader["DayLog"],
                        LastDate = (DateTime)ResultDataReader["LastDate"]
                    };
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"DailyLogList", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return (DailyLogListInfo)null;
        }

        public DailyAwardInfo[] GetAllDailyAward()
        {
            List<DailyAwardInfo> list = new List<DailyAwardInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Daily_Award_All");
                while (ResultDataReader.Read())
                    list.Add(new DailyAwardInfo()
                    {
                        Count = (int)ResultDataReader["Count"],
                        ID = (int)ResultDataReader["ID"],
                        IsBinds = (bool)ResultDataReader["IsBinds"],
                        TemplateID = (int)ResultDataReader["TemplateID"],
                        Type = (int)ResultDataReader["Type"],
                        ValidDate = (int)ResultDataReader["ValidDate"],
                        Sex = (int)ResultDataReader["Sex"],
                        Remark = ResultDataReader["Remark"] == null ? "" : ResultDataReader["Remark"].ToString(),
                        CountRemark = ResultDataReader["CountRemark"] == null ? "" : ResultDataReader["CountRemark"].ToString(),
                        GetWay = (int)ResultDataReader["GetWay"],
                        AwardDays = (int)ResultDataReader["AwardDays"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllDaily", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public NpcInfo[] GetAllNPCInfo()
        {
            List<NpcInfo> list = new List<NpcInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_NPC_Info_All");
                while (ResultDataReader.Read())
                    list.Add(new NpcInfo()
                    {
                        ID = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"] == null ? "" : ResultDataReader["Name"].ToString(),
                        Level = (int)ResultDataReader["Level"],
                        Camp = (int)ResultDataReader["Camp"],
                        Type = (int)ResultDataReader["Type"],
                        Blood = (int)ResultDataReader["Blood"],
                        X = (int)ResultDataReader["X"],
                        Y = (int)ResultDataReader["Y"],
                        Width = (int)ResultDataReader["Width"],
                        Height = (int)ResultDataReader["Height"],
                        MoveMin = (int)ResultDataReader["MoveMin"],
                        MoveMax = (int)ResultDataReader["MoveMax"],
                        BaseDamage = (int)ResultDataReader["BaseDamage"],
                        BaseGuard = (int)ResultDataReader["BaseGuard"],
                        Attack = (int)ResultDataReader["Attack"],
                        Defence = (int)ResultDataReader["Defence"],
                        Agility = (int)ResultDataReader["Agility"],
                        Lucky = (int)ResultDataReader["Lucky"],
                        ModelID = ResultDataReader["ModelID"] == null ? "" : ResultDataReader["ModelID"].ToString(),
                        ResourcesPath = ResultDataReader["ResourcesPath"] == null ? "" : ResultDataReader["ResourcesPath"].ToString(),
                        DropRate = ResultDataReader["DropRate"] == null ? "" : ResultDataReader["DropRate"].ToString(),
                        Experience = (int)ResultDataReader["Experience"],
                        Delay = (int)ResultDataReader["Delay"],
                        Immunity = (int)ResultDataReader["Immunity"],
                        Alert = (int)ResultDataReader["Alert"],
                        Range = (int)ResultDataReader["Range"],
                        Preserve = (int)ResultDataReader["Preserve"],
                        Script = ResultDataReader["Script"] == null ? "" : ResultDataReader["Script"].ToString(),
                        FireX = (int)ResultDataReader["FireX"],
                        FireY = (int)ResultDataReader["FireY"],
                        DropId = (int)ResultDataReader["DropId"],
                        CurrentBallId = (int)ResultDataReader["CurrentBallId"],
                        speed = (int)ResultDataReader["speed"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllNPCInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }

        public MissionInfo[] GetAllMissionInfo()
        {
            List<MissionInfo> list = new List<MissionInfo>();
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            try
            {
                this.db.GetReader(ref ResultDataReader, "SP_Mission_Info_All");
                while (ResultDataReader.Read())
                    list.Add(new MissionInfo()
                    {
                        Id = (int)ResultDataReader["ID"],
                        Name = ResultDataReader["Name"] == null ? "" : ResultDataReader["Name"].ToString(),
                        TotalCount = (int)ResultDataReader["TotalCount"],
                        TotalTurn = (int)ResultDataReader["TotalTurn"],
                        Script = ResultDataReader["Script"] == null ? "" : ResultDataReader["Script"].ToString(),
                        Success = ResultDataReader["Success"] == null ? "" : ResultDataReader["Success"].ToString(),
                        Failure = ResultDataReader["Failure"] == null ? "" : ResultDataReader["Failure"].ToString(),
                        Description = ResultDataReader["Description"] == null ? "" : ResultDataReader["Description"].ToString(),
                        IncrementDelay = (int)ResultDataReader["IncrementDelay"],
                        Delay = (int)ResultDataReader["Delay"],
                        Title = ResultDataReader["Title"] == null ? "" : ResultDataReader["Title"].ToString(),
                        Param1 = (int)ResultDataReader["Param1"],
                        Param2 = (int)ResultDataReader["Param2"]
                    });
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetAllMissionInfo", ex);
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return list.ToArray();
        }
    }
}
