﻿using System;
using System.Collections.Generic;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;

namespace VIS.Models
{
    public class CardViewModel
    {
        public List<CardViewPropeties> GetCardView(int ad_Window_ID, int ad_Tab_ID, Ctx ctx)
        {
            List<CardViewPropeties> lstCardView = null;
            //string sqlQuery = "SELECT * FROM AD_CardView WHERE AD_Window_id=" + ad_Window_ID + " and AD_Tab_id=" + ad_Tab_ID + " AND (createdby=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL OR AD_User_ID = " + ctx.GetAD_User_ID() + ") AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            //string sqlQuery = " SELECT * FROM AD_CardView c WHERE c.AD_Window_id=" + ad_Window_ID + " and c.AD_Tab_id=" + ad_Tab_ID + " AND (c.createdby=" + ctx.GetAD_User_ID() +
            //                  " OR ((c.AD_USER_ID    IS NULL) AND exists (select * from ad_cardview_role r where r.ad_cardview_id = c.ad_cardview_id and r.ad_role_id = " + ctx.GetAD_Role_ID() + ")) OR c.AD_User_ID     = " + ctx.GetAD_User_ID() +
            //                  " ) AND c.AD_Client_ID  =" + ctx.GetAD_Client_ID();

            //   string sqlQuery = " SELECT * FROM AD_CardView c WHERE c.AD_Window_id=" + ad_Window_ID + " and c.AD_Tab_id=" + ad_Tab_ID + " AND c.AD_Client_ID  =" + ctx.GetAD_Client_ID();

            string sqlQuery = @"SELECT CV.*,DCV.AD_DefaultCardView_ID,DCV.AD_User_ID AS userID,AU.Name AS CreatedName
                            FROM AD_CardView CV
                            INNER JOIN AD_User AU ON CV.createdBy=AU.AD_User_ID
                            LEFT OUTER JOIN AD_DefaultCardView DCV
                            ON CV.AD_CardView_ID=DCV.AD_CardView_ID
                            WHERE CV.IsActive='Y' AND  CV.AD_Window_id=" + ad_Window_ID + " AND CV.AD_Tab_id=" + ad_Tab_ID + " AND (CV.AD_User_ID IS NULL OR CV.AD_User_ID  =" + ctx.GetAD_User_ID() + @" 
                            ) ORDER BY CV.Name ASC";

            sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "AD_CardView", true, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardView = new List<CardViewPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bool isDefault = false;
                    if (ds.Tables[0].Rows[i]["AD_DefaultCardView_ID"] != null && ds.Tables[0].Rows[i]["AD_DefaultCardView_ID"] != DBNull.Value
                       && ds.Tables[0].Rows[i]["userID"] != null && ds.Tables[0].Rows[i]["userID"] != DBNull.Value
                       && ctx.GetAD_User_ID() == Util.GetValueOfInt(ds.Tables[0].Rows[i]["userID"]))
                    {
                        isDefault = true;
                    }


                    CardViewPropeties objCardView = new CardViewPropeties()
                    {
                        CardViewName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        CardViewID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_CardView_ID"]),
                        UserID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_USER_ID"]),
                        AD_GroupField_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_FIELD_ID"]),
                        CreatedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["CREATEDBY"]),
                        CreatedName = Convert.ToString(ds.Tables[0].Rows[i]["CreatedName"]),
                        AD_HeaderLayout_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_HEADERLAYOUT_ID"]),
                        groupSequence = Convert.ToString(ds.Tables[0].Rows[i]["GROUPSEQUENCE"]),
                        excludedGroup = Convert.ToString(ds.Tables[0].Rows[i]["EXCLUDEDGROUP"]),
                        OrderByClause = Convert.ToString(ds.Tables[0].Rows[i]["ORDERBYCLAUSE"]),
                        disableWindowPageSize = Convert.ToString(ds.Tables[0].Rows[i]["DISABLEWINDOWPAGESIZE"]) == "Y",
                        //IsDefault = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["ISDEFAULT"])=="Y"?true:false,
                        DefaultID = isDefault,
                        Updated = Convert.ToDateTime(ds.Tables[0].Rows[i]["UPDATED"])

                    };
                    lstCardView.Add(objCardView);
                }
            }
            return lstCardView;
        }
        public List<RolePropeties> GetCardViewRole(int ad_CardView_ID, Ctx ctx)
        {
            List<RolePropeties> lstCardViewRole = null;
            RolePropeties objCardView = null;
            string sqlQuery = "SELECT AD_ROLE_ID,AD_CardView_ID from AD_CARDVIEW_ROLE WHERE AD_CardView_id=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            //  string sqlQuery = "SELECT * FROM AD_CardView WHERE AD_Window_id=" + ad_Window_ID + " and AD_Tab_id=" + ad_Tab_ID + " AND (AD_USER_ID=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL )" ;
            // sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "AD_CardView", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardViewRole = new List<RolePropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objCardView = new RolePropeties()
                    {
                        AD_Role_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Role_ID"]),
                        AD_CardView_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_CardView_ID"])
                    };
                    lstCardViewRole.Add(objCardView);
                }
            }
            return lstCardViewRole;
        }
        public List<CardViewConditionPropeties> GetCardViewCondition(int ad_CardView_ID, Ctx ctx)
        {
            List<CardViewConditionPropeties> lstCardViewRole = null;
            CardViewConditionPropeties objCardView = null;
            string sqlQuery = "SELECT * FROM AD_CARDVIEW_Condition WHERE AD_CardView_id=" + ad_CardView_ID + " ORDER BY AD_CARDVIEW_Condition_ID";
            //  string sqlQuery = "SELECT * FROM AD_CardView WHERE AD_Window_id=" + ad_Window_ID + " and AD_Tab_id=" + ad_Tab_ID + " AND (AD_USER_ID=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL )" ;
            // sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "AD_CardView", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardViewRole = new List<CardViewConditionPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objCardView = new CardViewConditionPropeties()
                    {
                        Color = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["Color"]),
                        ConditionValue = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["ConditionValue"]),
                        ConditionText = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["ConditionText"]),
                    };
                    lstCardViewRole.Add(objCardView);
                }
            }
            return lstCardViewRole;
        }
        public List<UserPropeties> GetAllUsers(Ctx ctx)
        {
            List<UserPropeties> lstUser = null;
            string sqlQuery = "SELECT * FROM AD_User WHERE ISACTIVE='Y' AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            //   sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "AD_User", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstUser = new List<UserPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    UserPropeties objCardView = new UserPropeties()
                    {
                        UserName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        AD_User_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"])
                    };
                    lstUser.Add(objCardView);
                }
            }
            return lstUser;
        }
        public List<RolePropeties> GetAllRoles(Ctx ctx)
        {
            List<RolePropeties> lstRole = null;

            string sqlQuery = "SELECT  r.AD_Role_ID,  r.Name  FROM AD_User u INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive ='Y') " +
                        " INNER JOIN AD_Role r ON (ur.AD_Role_ID =r.AD_Role_ID AND r.IsActive ='Y') WHERE u.AD_User_ID = " + ctx.GetAD_User_ID() + " AND u.IsActive ='Y' AND EXISTS " +
                        " (SELECT * FROM AD_Client c WHERE u.AD_Client_ID=c.AD_Client_ID AND c.IsActive      ='Y' ) " +
                        " AND EXISTS (SELECT * FROM AD_Client c WHERE r.AD_Client_ID=c.AD_Client_ID AND c.IsActive      ='Y' )";

            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstRole = new List<RolePropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RolePropeties objCardView = new RolePropeties()
                    {
                        RoleName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        AD_Role_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Role_ID"])
                    };
                    lstRole.Add(objCardView);
                }
            }
            return lstRole;
        }
        public List<CardViewPropeties> GetCardViewColumns(int ad_cardview_id, Ctx ctx)
        {
            int uid = 0;
            int fid = 0;
            string sortOrder = "";
            List<CardViewPropeties> lstCardViewColumns = new List<CardViewPropeties>();
            string sqlQuery1 = "SELECT AD_User_ID,AD_Field_ID, orderByClause  FROM AD_CardView WHERE ad_cardview_id=" + ad_cardview_id;
            DataSet ds1 = DB.ExecuteDataset(sqlQuery1);
            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                uid = VAdvantage.Utility.Util.GetValueOfInt(ds1.Tables[0].Rows[0][0]);
                fid = VAdvantage.Utility.Util.GetValueOfInt(ds1.Tables[0].Rows[0][1]);
                sortOrder = VAdvantage.Utility.Util.GetValueOfString(ds1.Tables[0].Rows[0][2]);
            }
            string sqlQuery = "SELECT * FROM(SELECT crdcol.*,fl.name FROM ad_cardview_column crdcol INNER JOIN ad_field fl on crdcol.ad_field_id=fl.ad_field_id  WHERE ad_cardview_id=" + ad_cardview_id + ") cardviewcols";
            //  sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "cardviewcols", false, false);
            sqlQuery += " ORDER BY seqno";
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CardViewPropeties objCardView = new CardViewPropeties()
                    {
                        FieldName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        AD_Field_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_FIELD_ID"]),
                        AD_GroupField_ID = fid,
                        sort = Util.GetValueOfInt(ds.Tables[0].Rows[i]["SORTNO"]),
                        SeqNo = Util.GetValueOfInt(ds.Tables[0].Rows[i]["SeqNo"]),
                        UserID = uid,
                        OrderByClause = sortOrder

                    };
                    lstCardViewColumns.Add(objCardView);
                }
            }
            else
            {
                CardViewPropeties objCardView = new CardViewPropeties()
                {
                    FieldName = "",
                    AD_Field_ID = 0,
                    AD_GroupField_ID = fid,
                    UserID = uid

                };
                lstCardViewColumns.Add(objCardView);
            }
            return lstCardViewColumns;
        }
        public int SaveCardViewRecord(string cardViewName, int ad_Window_ID, int ad_Tab_ID, int ad_User_ID, int ad_Field_ID, Ctx ctx, int cardViewID/*, List<RolePropeties> lstRoleId*/, List<CardViewConditionPropeties> lstCVCondition, int AD_HeaderLayout_ID, bool isPublic, string groupSequence, string excludeGrp, string orderByClause)
        {
            string conditionValue = string.Empty;
            string conditionText = string.Empty;
            bool isupdate = false;
            MCardView objCardView = null;
            if (cardViewID <= 0)
            {
                objCardView = new MCardView(ctx, 0, null);

            }
            else
            {
                objCardView = new MCardView(ctx, cardViewID, null);
                isupdate = true;
            }
            objCardView.SetAD_Window_ID(ad_Window_ID);
            objCardView.SetAD_Tab_ID(ad_Tab_ID);
            if (MUser.Get(ctx).IsAdministrator() && isPublic)
            {
                objCardView.SetAD_User_ID(0);
            }
            else
            {
                objCardView.SetAD_User_ID(ad_User_ID);
            }
            objCardView.SetAD_Field_ID(ad_Field_ID);
            objCardView.SetName(cardViewName);
            if (AD_HeaderLayout_ID > 0)
            {
                objCardView.Set_ValueNoCheck("AD_HeaderLayout_ID", AD_HeaderLayout_ID);
            }
            else
            {
                objCardView.Set_ValueNoCheck("AD_HeaderLayout_ID", null);
            }
            objCardView.Set_ValueNoCheck("groupSequence", groupSequence);
            objCardView.Set_ValueNoCheck("excludedGroup", excludeGrp);
            objCardView.Set_Value("OrderByClause", orderByClause);
            if (!objCardView.Save())
            {
            }
            if (isupdate)
            {
                //DeleteAllCardViewRole(objCardView.Get_ID(), ctx);
                DeleteAllCardViewCondition(objCardView.Get_ID(), ctx);
            }
            //if (lstRoleId != null && lstRoleId.Count > 0)
            //{
            //    for (int i = 0; i < lstRoleId.Count; i++)
            //    {
            //        MCardViewRole objMCVR = new MCardViewRole(ctx, 0, null);
            //        objMCVR.SetAD_CardView_ID(objCardView.Get_ID());
            //        objMCVR.SetAD_Role_ID(lstRoleId[i].AD_Role_ID);
            //        if (!objMCVR.Save())
            //        {
            //        }
            //    }
            //}

            if (lstCVCondition != null && lstCVCondition.Count > 0)
            {
                for (int i = 0; i < lstCVCondition.Count; i++)
                {
                    if (lstCVCondition[i].ConditionValue != null && lstCVCondition[i].ConditionText != null)
                    {
                        conditionValue = lstCVCondition[i].ConditionValue.Trim();
                        conditionText = lstCVCondition[i].ConditionText.Trim();
                        MCardViewCondition objMCVR = new MCardViewCondition(ctx, 0, null);
                        objMCVR.Set_ValueNoCheck("AD_CardView_ID", objCardView.Get_ID());
                        objMCVR.SetColor(lstCVCondition[i].Color);
                        objMCVR.SetConditionValue(conditionValue.Trim());
                        objMCVR.SetConditionText(conditionText.Trim());
                        if (!objMCVR.Save())
                        {
                        }
                    }
                }
            }
            return objCardView.Get_ID();
        }
        public void SetDefaultCardView(Ctx ctx, int cardViewID, int AD_Tab_ID)
        {
            string sql = "SELECT AD_DefaultCardView_ID FROM AD_DefaultCardView WHERE AD_Tab_ID=" + AD_Tab_ID + " AND AD_User_ID=" + ctx.GetAD_User_ID();
            object id = DB.ExecuteScalar(sql);

            int AD_DefaultCardView_ID = 0;
            if (id != null && id != DBNull.Value)
            {
                AD_DefaultCardView_ID = Convert.ToInt32(id);
            }

            X_AD_DefaultCardView cardView = new X_AD_DefaultCardView(ctx, AD_DefaultCardView_ID, null);
            cardView.SetAD_Tab_ID(AD_Tab_ID);
            cardView.SetAD_User_ID(ctx.GetAD_User_ID());
            cardView.SetAD_CardView_ID(Convert.ToInt32(cardViewID));
            cardView.Save();
        }

        public void SaveCardViewColumns(int ad_cardview_id, int ad_Field_ID, int sqNo, Ctx ctx, int sort)
        {

            MCardViewColumn objCardViewColumn = new MCardViewColumn(ctx, 0, null);
            objCardViewColumn.SetAD_CardView_ID(ad_cardview_id);
            objCardViewColumn.SetAD_Field_ID(ad_Field_ID);
            objCardViewColumn.SetSeqNo(sqNo);
            objCardViewColumn.Set_Value("SortNo", sort);
            if (!objCardViewColumn.Save())
            {
            }
        }

        public void DeleteCardView(int ad_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_CARDVIEW WHERE AD_CARDVIEW_ID=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewColumns(int ad_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_CARDVIEW_COLUMN WHERE AD_CARDVIEW_ID=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewRole(int ad_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_CARDVIEW_ROLE WHERE AD_CARDVIEW_ID=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewCondition(int ad_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_CARDVIEW_CONDITION WHERE AD_CARDVIEW_ID=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        /// <summary>
        ///  Delete default card
        /// </summary>
        /// <param name="ad_CardView_ID"></param>
        /// <param name="ctx"></param>
        public void DeleteDefaultCardView(int ad_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_DefaultCardView WHERE AD_CARDVIEW_ID=" + ad_CardView_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }

        public void DeleteCardViewColumns(int ad_CardViewColumn_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM AD_CARDVIEW_COLUMN WHERE AD_CARDVIEW_COLUMN_ID=" + ad_CardViewColumn_ID;
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void UpdateCardViewColumnPosition(int ad_CardViewColumn_ID, int seqNo, int ad_CardView_ID, bool isUp, Ctx ctx)
        {
            int seqNo1 = 0;
            int id = 0; if (isUp)
            {
                seqNo1 = seqNo - 10;
            }
            else
            {
                seqNo1 = seqNo + 10;
            }
            string query = "SELECT ad_cardview_column_ID FROM ad_cardview_column WHERE ad_cardview_id=" + ad_CardView_ID + " AND seqno =" + seqNo1;
            id = Util.GetValueOfInt(DB.ExecuteScalar(query));

            MCardViewColumn objCardViewColumn = new MCardViewColumn(ctx, ad_CardViewColumn_ID, null);
            if (isUp)
            {
                objCardViewColumn.SetSeqNo(seqNo - 10);
            }
            else
            {
                objCardViewColumn.SetSeqNo(seqNo + 10);
            }
            if (!objCardViewColumn.Save())
            {
            }
            MCardViewColumn objCardViewColumn1 = new MCardViewColumn(ctx, id, null);
            if (isUp)
            {
                objCardViewColumn1.SetSeqNo(seqNo);
            }
            else
            {
                objCardViewColumn1.SetSeqNo(seqNo);
            }
            if (!objCardViewColumn.Save())
            {
            }
        }
        public void DeleteCardViewRecord(int ad_CardView_ID, Ctx ctx)
        {
            DeleteCardView(ad_CardView_ID, ctx);
            DeleteAllCardViewColumns(ad_CardView_ID, ctx);
            DeleteAllCardViewRole(ad_CardView_ID, ctx);
            DeleteDefaultCardView(ad_CardView_ID, ctx);
        }


        public void SetDefaultView(Ctx ctx, int AD_Tab_ID, int cardView)
        {

            string sql = "SELECT AD_DefaultCardView_ID FROM AD_DefaultCardView WHERE AD_Tab_ID=" + AD_Tab_ID + " AND AD_User_ID=" + ctx.GetAD_User_ID();
            object id = DB.ExecuteScalar(sql);
            int AD_DefaultcarView_ID = 0;
            if (id != null && id != DBNull.Value)
            {
                AD_DefaultcarView_ID = Convert.ToInt32(id);
            }


            X_AD_DefaultCardView userQuery = new X_AD_DefaultCardView(ctx, AD_DefaultcarView_ID, null);
            userQuery.SetAD_Tab_ID(AD_Tab_ID);
            userQuery.SetAD_User_ID(ctx.GetAD_User_ID());
            userQuery.SetAD_CardView_ID(cardView);
            userQuery.Save();
        }

        /// <summary>
        /// Update card from Drag and drop
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="grpID"></param>
        /// <param name="recordID"></param>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string UpdateCardByDragDrop(Ctx ctx, string grpValue, int recordID, string columnName, string tableName, int dataType)
        {
            string result = "1";
            try
            {
                if (string.IsNullOrEmpty(grpValue))
                {
                    grpValue = null;
                }

                PO _po = MTable.GetPO(ctx, tableName, recordID, null);
                if (VAdvantage.Classes.DisplayType.YesNo == dataType)
                {
                    _po.Set_ValueNoCheck(columnName, Convert.ToBoolean(grpValue));
                }
                else if (VAdvantage.Classes.DisplayType.IsDate(dataType))
                {
                    _po.Set_ValueNoCheck(columnName, Convert.ToDateTime(grpValue));
                }
                else
                {
                    _po.Set_ValueNoCheck(columnName, grpValue);
                }


                if (!_po.Save())
                {
                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();

                    string error = pp != null ? pp.GetName() : ""; ;
                    if (string.IsNullOrEmpty(error))
                    {
                        error = pp != null ? pp.GetValue() : "";
                    }

                    if (string.IsNullOrEmpty(error))
                    {
                        ValueNamePair pp1 = VAdvantage.Logging.VLogger.RetrieveWarning();
                        error = pp1 != null ? pp1.GetName() : ""; ;
                        if (string.IsNullOrEmpty(error))
                        {
                            error = pp1 != null ? pp1.GetValue() : "";
                        }
                    }

                    if (string.IsNullOrEmpty(error))
                    {
                        error = Msg.GetMsg(ctx, "Error");
                    }

                    result = error;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            //string keyColumn = tableName + "_ID";
            //string ColumnName = MColumn.GetColumnName(ctx, columnID);
            //string sqlQuery = "";
            //if (Regex.IsMatch(grpValue, @"^\d+$"))
            //{
            //    sqlQuery = "UPDATE " + tableName + " SET " + ColumnName + "=" + grpValue + " WHERE " + keyColumn + "=" + recordID;
            //}
            //else
            //{
            //    sqlQuery = "UPDATE " + tableName + " SET " + ColumnName + "='" + grpValue + "' WHERE " + keyColumn + "=" + recordID;
            //}

            //int result = DB.ExecuteQuery(sqlQuery);
            return result;
        }
        public string GetColumnIDWindowID(string tableName, string columnName)
        {

            string sql = " SELECT cl.ad_column_id FROM ad_column cl WHERE cl.ad_table_id=" +
                     "(SELECT tb.ad_table_id FROM ad_table tb WHERE tb.tablename='" + tableName + "'" +
                      ") and cl.columnname='" + columnName + "'";
            string columnID = Util.GetValueOfString(DB.GetSQLValue(null, sql));
            sql = "SELECT ad_window_id FROM AD_Window WHERE UPPER(NAME)=UPPER('Card Template') AND isActive='Y'";
            string windowID = Util.GetValueOfString(DB.GetSQLValue(null, sql));

            return columnID + "," + windowID;
        }

        /// <summary>
        /// Get card Template
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ad_Window_ID"></param>
        /// <param name="ad_Tab_ID"></param>
        /// <returns></returns>
        public string getTemplateDesign(Ctx ctx, int ad_Window_ID, int ad_Tab_ID)
        {
            string design = "";
            string sqlQuery = "SELECT AD_HEADERLAYOUT.*,CASE when to_date(updated)=to_date(CURRENT_DATE) THEN to_char(updated, 'HH:MI:SS AM') ELSE to_char(updated) END AS lastUpdated  FROM AD_HEADERLAYOUT WHERE ISACTIVE='Y' AND ISHEADERVIEW='N' AND (IsSystemTemplate='Y' OR AD_HeaderLayout_ID IN (SELECT AD_HeaderLayout_ID  FROM AD_CardView WHERE AD_CardView.AD_Window_id=" + ad_Window_ID + " and AD_CardView.AD_Tab_id=" + ad_Tab_ID + " AND (AD_CardView.AD_User_ID IS NULL OR AD_CardView.AD_User_ID  =" + ctx.GetAD_User_ID() + ")))";
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    if (Util.GetValueOfString(ds.Tables[0].Rows[i]["IsSystemTemplate"]) == "Y")
                    {
                        design += "<div lastUpdated='" + Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["lastUpdated"]).Value.ToLocalTime().ToString("hh:mm:ss tt") + "' isSystemTemplate='Y' createdBy='" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["createdby"]) + "' class='vis-cardSingleViewTemplate d-flex align-items-center justify-content-center'>";
                    }
                    else
                    {
                        design += "<div lastUpdated='" + Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["lastUpdated"]).Value.ToLocalTime().ToString("hh:mm:ss tt") + "' isSystemTemplate='N' createdBy='" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["createdby"]) + "' class='vis-cardSingleViewTemplate d-flex align-items-center justify-content-center displayNone'>";
                    }

                    design += "<div class='mainTemplate' name='" + Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) + "' templateID='" + Util.GetValueOfString(ds.Tables[0].Rows[i]["AD_HeaderLayout_ID"]) + "' style='" + Util.GetValueOfString(ds.Tables[0].Rows[i]["BackgroundColor"]) + "'>";
                    sqlQuery = "SELECT * FROM AD_GRIDLAYOUT WHERE AD_HeaderLayout_ID=" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_HeaderLayout_ID"]) + " AND ISACTIVE='Y'";
                    DataSet dsSec = DB.ExecuteDataset(sqlQuery);
                    if (dsSec != null && dsSec.Tables.Count > 0 && dsSec.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsSec.Tables[0].Rows.Count; j++)
                        {

                            string gridStyle = Util.GetValueOfString(dsSec.Tables[0].Rows[j]["BackgroundColor"]);
                            int totalRow = Util.GetValueOfInt(dsSec.Tables[0].Rows[j]["TotalRows"]);
                            int totalCol = Util.GetValueOfInt(dsSec.Tables[0].Rows[j]["TotalColumns"]);
                            if (gridStyle.IndexOf("grid-template-rows") == -1 || gridStyle.IndexOf("grid-template-columns") == -1)
                            {
                                gridStyle += ";grid-template-rows:repeat(" + totalRow + ",1fr)";
                                gridStyle += ";grid-template-columns:repeat(" + totalCol + ",1fr)";
                            }

                            design += "<div row='" + totalRow + "' col='" + totalCol + "' sectionID='" + Util.GetValueOfInt(dsSec.Tables[0].Rows[j]["AD_GridLayout_ID"]) + "' sectionCount='" + (j + 1) + "' class='section" + (j + 1) + " vis-wizard-section' style='" + gridStyle + "'>";
                            sqlQuery = "SELECT * FROM AD_GRIDLAYOUTITEMS WHERE ISACTIVE='Y' AND AD_GRIDLAYOUT_ID=" + Util.GetValueOfInt(dsSec.Tables[0].Rows[j]["AD_GridLayout_ID"]);
                            DataSet dsItem = DB.ExecuteDataset(sqlQuery);
                            if (dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
                            {
                                for (int k = 0; k < dsItem.Tables[0].Rows.Count; k++)
                                {
                                    string style = Util.GetValueOfString(dsItem.Tables[0].Rows[k]["BackgroundColor"]);
                                    if (style.IndexOf("grid-area") == -1)
                                    {
                                        style += ";grid-area:" + Util.GetValueOfInt(dsItem.Tables[0].Rows[k]["StartRow"]) + "/" + Util.GetValueOfInt(dsItem.Tables[0].Rows[k]["StartColumn"]);
                                        style += "/" + Util.GetValueOfInt(dsItem.Tables[0].Rows[k]["Rowspan"]) + "/" + Util.GetValueOfInt(dsItem.Tables[0].Rows[k]["ColumnSpan"]);
                                    }
                                    design += "<div seqNo='" + Util.GetValueOfInt(dsItem.Tables[0].Rows[k]["SeqNo"]) + "' cardFieldID ='" + Util.GetValueOfString(dsItem.Tables[0].Rows[k]["AD_GRIDLAYOUTITEMS_ID"]) + "' class='grdDiv' style='" + style + "' fieldValuestyle='" + Util.GetValueOfString(dsItem.Tables[0].Rows[k]["FieldValueStyle"]) + "' showfieldicon='" + Util.GetValueOfString(dsItem.Tables[0].Rows[k]["HideFieldIcon"]) + "' showfieldtext='" + Util.GetValueOfString(dsItem.Tables[0].Rows[k]["HideFieldText"]) + "' query='" + SecureEngineBridge.EncryptByClientKey(dsItem.Tables[0].Rows[k]["columnSQL"].ToString(), ctx.GetSecureKey()) + "'>";
                                    //design += "<fields draggable='true' ondragstart='drag(event)'></fields>";
                                    string msg = Msg.GetMsg(ctx, Util.GetValueOfString(dsItem.Tables[0].Rows[k]["content"]));
                                    if (msg.IndexOf("[") > -1)
                                    {
                                        msg = Util.GetValueOfString(dsItem.Tables[0].Rows[k]["content"]);
                                    }
                                    string valueStyle = "";
                                    string imgStyle = "";
                                    string htmlStyle = Util.GetValueOfString(dsItem.Tables[0].Rows[k]["FieldValueStyle"]);

                                    if (htmlStyle != null && htmlStyle.Length > 0 && htmlStyle.IndexOf("@") > -1)
                                    {
                                        string[] stylearr = htmlStyle.Split('|');

                                        if (stylearr != null && stylearr.Length > 0)
                                        {
                                            for (int m = 0; m < stylearr.Length; m++)
                                            {
                                                if (stylearr[m].IndexOf("@img::") > -1)
                                                {
                                                    imgStyle = stylearr[m].Replace("@img::", "");
                                                }
                                                else if (stylearr[m].IndexOf("@value::") > -1)
                                                {
                                                    valueStyle = stylearr[m].Replace("@value::", "");
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        valueStyle = htmlStyle;
                                    }

                                    if (valueStyle.ToUpper().Trim() == "UNDEFINED")
                                    {
                                        valueStyle = "";
                                    }
                                    if (imgStyle.ToUpper().Trim() == "UNDEFINED")
                                    {
                                        imgStyle = "";
                                    }

                                    if (msg.IndexOf("<img") > -1 || msg.IndexOf("<svg") > -1)
                                    {
                                        msg = msg.Replace("<img", "<img style='" + imgStyle + "' ");
                                    }

                                    design += "<fieldValue style='" + valueStyle + "'>" + msg + "</fieldValue>";
                                    design += "</div>";
                                }
                            }
                            design += "</div>";
                        }
                    }
                    design += "</div></div>";
                }
            }

            return design;
        }

        /// <summary>
        /// Save template
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="CardViewID"></param>
        /// <param name="templateID"></param>
        /// <param name="templateName"></param>
        /// <param name="style"></param>
        /// <param name="cardSection"></param>
        /// <param name="cardTempField"></param>
        /// <returns></returns>
        public int saveCardTemplate(Ctx ctx, int CardViewID, int templateID, string templateName, string style, List<CardSection> cardSection, List<CardTempField> cardTempField)
        {
            MHeaderLayout mhl = new MHeaderLayout(ctx, templateID, null);
            if (templateID > 0)
            {
                string sql = "DELETE FROM AD_GridLayoutItems WHERE AD_GridLayout_ID IN (SELECT AD_GridLayout_ID FROM AD_GridLayout WHERE AD_HeaderLayout_ID=" + mhl.GetAD_HeaderLayout_ID() + ")";
                DB.ExecuteQuery(sql);
                DB.ExecuteQuery("DELETE FROM AD_GridLayout WHERE AD_HeaderLayout_ID=" + mhl.GetAD_HeaderLayout_ID());
            }
            else
            {
                mhl.Set_Value("IsSystemTemplate", "N");
            }
            mhl.SetName(templateName);
            mhl.SetIsHeaderView(false);
            mhl.SetBackgroundColor(style);
            if (mhl.Save())
            {
                templateID = mhl.GetAD_HeaderLayout_ID();

                for (int i = 0; i < cardSection.Count; i++)
                {
                    MGridLayout mgl = new MGridLayout(ctx, 0, null);
                    mgl.SetAD_HeaderLayout_ID(mhl.GetAD_HeaderLayout_ID());
                    mgl.SetName(cardSection[i].sectionName.Trim());
                    mgl.SetBackgroundColor(cardSection[i].style.Trim());
                    mgl.SetTotalRows(cardSection[i].totalRow);
                    mgl.SetTotalColumns(cardSection[i].totalCol);
                    mgl.SetSeqNo(cardSection[i].sectionNo);
                    if (mgl.Save())
                    {

                        for (int j = 0; j < cardTempField.Count; j++)
                        {
                            if (cardSection[i].sectionNo == cardTempField[j].sectionNo)
                            {
                                string columnSQL = null;
                                if (!string.IsNullOrEmpty(cardTempField[j].columnSQL))
                                {
                                    columnSQL = SecureEngineBridge.DecryptByClientKey(cardTempField[j].columnSQL, ctx.GetSecureKey());
                                }
                                MGridLayoutItems mli = new MGridLayoutItems(ctx, 0, null);
                                mli.SetAD_GridLayout_ID(mgl.GetAD_GridLayout_ID());
                                mli.SetSeqNo(cardTempField[j].seq);
                                mli.SetStartRow(cardTempField[j].rowStart);
                                mli.SetRowspan(cardTempField[j].rowEnd);
                                mli.SetStartColumn(cardTempField[j].colStart);
                                mli.SetColumnSpan(cardTempField[j].colEnd);
                                mli.SetBackgroundColor(cardTempField[j].style);
                                mli.SetJustifyItems(null);
                                mli.SetAlignItems(null);
                                mli.SetFieldValueStyle(cardTempField[j].valueStyle);
                                mli.SetHideFieldIcon(cardTempField[j].hideFieldIcon);
                                mli.SetHideFieldText(cardTempField[j].hideFieldText);
                                mli.SetColumnSQL(columnSQL);
                                if (mli.Save())
                                {
                                    //SaveCardViewColumns(CardViewID, cardTempField[j].fieldID, cardTempField[j].seq,  ctx, 0);
                                }
                            }
                        }
                    }
                }
            }
            return templateID;
        }


    }

    public class CardViewPropeties
    {
        public string CardViewName { get; set; }
        public int CardViewID { get; set; }
        public int AD_Window_ID { get; set; }
        public int AD_Tab_ID { get; set; }
        public int UserID { get; set; }

        public string FieldName { get; set; }
        public string groupSequence { get; set; }
        public string excludedGroup { get; set; }
        public int AD_Field_ID { get; set; }
        public int AD_GroupField_ID { get; set; }
        public int AD_CardViewColumn_ID { get; set; }
        public int SeqNo { get; set; }
        public bool isNewRecord { get; set; }
        public bool IsDefault { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedName { get; set; }
        public bool DefaultID { get; set; }
        public bool isPublic { get; set; }
        public int AD_HeaderLayout_ID { get; set; }
        public int sort { get; set; }
        public string OrderByClause { get; set; }
        public bool disableWindowPageSize { get; set; }
        public DateTime Updated { get; set; }
    }

    public class UserPropeties
    {
        public string UserName { get; set; }
        public int AD_User_ID { get; set; }

    }
    public class RolePropeties
    {
        public string RoleName { get; set; }
        public int AD_Role_ID { get; set; }
        public int AD_CardView_ID { get; set; }

    }
    public class CardViewConditionPropeties
    {
        public string Color { get; set; }
        public string ConditionValue { get; set; }
        public string ConditionText { get; set; }

    }
    public class ParameterPropeties
    {
        public List<UserPropeties> lstUserData { get; set; }
        public List<RolePropeties> lstRoleData { get; set; }
        public List<CardViewPropeties> lstCardViewData { get; set; }
        public List<List<RolePropeties>> lstCardViewRoleData { get; set; }
        public List<CardViewConditionPropeties> lstCardViewConditonData { get; set; }
    }

    public class CardSection
    {
        public int sectionID { get; set; }
        public string sectionName { get; set; }
        public string style { get; set; }
        public int sectionNo { get; set; }
        public int totalRow { get; set; }
        public int totalCol { get; set; }
        public int rowGap { get; set; }
        public int colGap { get; set; }
    }
    public class CardTempField
    {
        public int cardFieldID { get; set; }
        public string fieldStyle { get; set; }
        public string valueStyle { get; set; }
        public string style { get; set; }
        public string columnSQL { get; set; }
        public bool hideFieldIcon { get; set; }
        public bool hideFieldText { get; set; }
        public int sectionNo { get; set; }
        public int fieldID { get; set; }
        public int rowStart { get; set; }
        public int rowEnd { get; set; }
        public int colStart { get; set; }
        public int colEnd { get; set; }
        public int seq { get; set; }
    }
}