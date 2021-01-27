﻿/********************************************************
 * Project Name   : VIS
 * Class Name     : AttachmentHistoryModel
 * Purpose        : Used to show history attached of record like mail sent on recod, letter , Appointments saved, inbox mails
 * Chronological    Development
 * Karan            
  ******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class AttachmentHistoryModel
    {


        /// <summary>
        /// Count of total History Records 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <param name="_VAF_TableView_ID"></param>
        /// <param name="_Record_ID"></param>
        /// <returns></returns>
        public int LoadRecordDataCount(Ctx ctx, string searchText, int _VAF_TableView_ID, int _Record_ID)
        {
            string strAppCount = "SELECT count(*) FROM " +
                       "(( SELECT ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE, subject  FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_Id =" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strAppCount += " AND upper(ai.Subject)  like upper('%" + searchText + "%')";
            }
            strAppCount += " AND ai.IsTask='N' And ai.vaf_tableview_Id = " + _VAF_TableView_ID + " And ai.VAF_UserContact_ID = " + ctx.GetVAF_UserContact_ID() + ")   UNION" +

                 "( SELECT ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Task") + "' AS TYPE, subject  FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_Id =" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strAppCount += " AND upper(ai.Subject)  like upper('%" + searchText + "%')";
            }
            strAppCount += " AND ai.IsTask='Y' And ai.vaf_tableview_Id = " + _VAF_TableView_ID + " And ai.VAF_UserContact_ID = " + ctx.GetVAF_UserContact_ID() + ")   UNION" +


            " SELECT MAILATTACHMENT1_ID AS ID, record_ID,created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, TITLE AS Subject FROM mailattachment1 WHERE record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strAppCount += " AND ((upper(title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))";
            }
            strAppCount += " And attachmenttype='M' And vaf_tableview_id=" + _VAF_TableView_ID + " UNION" +
            " SELECT MAILATTACHMENT1_ID AS ID, record_ID,created,'" + Msg.GetMsg(ctx, "InboxMail") + "' AS TYPE, TITLE AS Subject FROM mailattachment1 WHERE record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strAppCount += " AND ((upper(title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))";
            }
            strAppCount += " And attachmenttype='I' And vaf_tableview_id=" + _VAF_TableView_ID + " UNION" +
            " SELECT MAILATTACHMENT1_ID AS ID, record_ID,created,'" + Msg.GetMsg(ctx, "Letter") + "' AS TYPE, TITLE AS Subject FROM mailattachment1  WHERE record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strAppCount += " AND upper(title)  like upper('%" + searchText + "%')";
            }
            //if (VAdvantage.DataBase.DB.IsPostgreSQL())
            //{
            //    strAppCount += " And attachmenttype='L' And vaf_tableview_id=" + _VAF_TableView_ID + ") as foo ";
            //}
            //else
            //{
            //    strAppCount += " And attachmenttype='L' And vaf_tableview_id=" + _VAF_TableView_ID + ") ORDER BY created DESC";
            //}

            strAppCount += " And attachmenttype='L' And vaf_tableview_id=" + _VAF_TableView_ID;

            // Updated for fetching call records for history by vinay
            if (Env.IsModuleInstalled("VA048_"))
            {
                strAppCount += @" UNION 
SELECT VA048_CallDetails_ID AS ID, record_ID, created, '" + Msg.GetMsg(ctx, "VA048_CallType") + @"' AS TYPE, VA048_To AS Subject 
FROM VA048_CallDetails 
WHERE VA048_To is not null and IsActive = 'Y' 
and VAF_TableView_ID = " + _VAF_TableView_ID + @"
and record_id = " + _Record_ID;

                if (searchText != "undefined" && searchText != null && searchText != "")
                {
                    strAppCount += " AND upper(ai.VA048_To) like upper('%" + searchText + "%')";
                }
            }

            strAppCount += ")" + (VAdvantage.DataBase.DB.IsPostgreSQL() ? " as foo" : "") + " ORDER BY created DESC";
            return Util.GetValueOfInt(DB.ExecuteScalar(strAppCount, null, null));

        }

        
        /// <summary>
        /// Load history records for current record
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <param name="_VAF_TableView_ID"></param>
        /// <param name="_Record_ID"></param>
        /// <param name="historyPageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<HistoryRecordInfo> LoadRecordData(Ctx ctx, string searchText, int _VAF_TableView_ID, int _Record_ID, int historyPageNo, int pageSize)
        {
            List<HistoryRecordInfo> hLists = new List<HistoryRecordInfo>();

            var strApp = "SELECT * FROM " +
                      "(( SELECT ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,au.name  FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_Id =" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strApp += " AND upper(ai.Subject)  like upper('%" + searchText + "%')";
            }
            strApp += " AND ai.IsTask='N' And ai.vaf_tableview_Id = " + _VAF_TableView_ID + " And ai.VAF_UserContact_ID = " + ctx.GetVAF_UserContact_ID() + ")   UNION" +


                "( SELECT ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Task") + "' AS TYPE,ai.Subject,au.name  FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_Id =" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strApp += " AND upper(ai.Subject)  like upper('%" + searchText + "%')";
            }
            strApp += " AND ai.IsTask='Y' And ai.vaf_tableview_Id = " + _VAF_TableView_ID + " And ai.VAF_UserContact_ID = " + ctx.GetVAF_UserContact_ID() + ")   UNION" +



            " SELECT ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,au.name  FROM mailattachment1 ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strApp += " AND ((upper(ai.title)  like upper('%" + searchText + "%')) OR (upper(ai.mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))";
            }
            strApp += " And ai.attachmenttype='M' And ai.vaf_tableview_id=" + _VAF_TableView_ID + " UNION" +
            " SELECT ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "InboxMail") + "' AS TYPE, ai.TITLE AS Subject,au.name  FROM mailattachment1 ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strApp += " AND ((upper(ai.title)  like upper('%" + searchText + "%')) OR (upper(ai.mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))";
            }
            strApp += " And ai.attachmenttype='I' And ai.vaf_tableview_id=" + _VAF_TableView_ID + " UNION" +
            " SELECT ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "Letter") + "' AS TYPE, ai.TITLE AS Subject,au.name  FROM mailattachment1 ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby WHERE ai.record_id=" + _Record_ID;
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                strApp += " AND upper(ai.title)  like upper('%" + searchText + "%')";
            }

            strApp += " And ai.attachmenttype='L' And ai.vaf_tableview_id=" + _VAF_TableView_ID;

            // Updated for fetching call records for history by vinay
            if (Env.IsModuleInstalled("VA048_"))
            {
                strApp += @" UNION 
SELECT VA048_CallDetails_ID AS ID, ai.record_ID, ai.created, '" + Msg.GetMsg(ctx, "VA048_CallType") + @"' AS TYPE, ai.VA048_To AS Subject, au.name 
FROM VA048_CallDetails ai 
JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby 
WHERE ai.VA048_To is not null and ai.IsActive = 'Y' 
and ai.VAF_TableView_ID = " + _VAF_TableView_ID + @"
and ai.record_id = " + _Record_ID;

//                if (_VAF_TableView_ID == 291)
//                {
//                    strApp += @" UNION " +
//@"select cd.VA048_CallDetails_ID as ID, cd.record_ID, cd.created, '" + Msg.GetMsg(ctx, "VA048_CallType") + @"' as TYPE, cd.VA048_To as Subject, au.name 
//FROM VA048_CallDetails cd 
//left join VAF_UserContact au on au.VAF_UserContact_ID = cd.VA048_RefRecordID 
//where cd.VA048_To is not null 
//and cd.VAF_TableView_ID = 291 
//and cd.Record_ID = " + _Record_ID + @" 
//UNION
//select cd.VA048_CallDetails_ID as id, cd.record_ID, cd.created, '" + Msg.GetMsg(ctx, "VA048_CallType") + @"' as type, cd.VA048_To as Subject, au.name 
//FROM VA048_CallDetails cd 
//left join VAF_UserContact au on au.VAF_UserContact_ID = cd.VA048_RefRecordID 
//where cd.VA048_To is not null 
//and au.VAB_BusinessPartner_ID = " + _Record_ID + @" 
//and cd.Record_ID = " + _Record_ID;
//                }
//                else
//                {
//                    strApp += " UNION " +
//@"select cd.VA048_CallDetails_ID as id, cd.record_ID, cd.created, '" + Msg.GetMsg(ctx, "VA048_CallType") + @"' as type, cd.VA048_To as Subject, au.name 
//FROM VA048_CallDetails cd 
//left join VAF_UserContact au on au.VAF_UserContact_ID = cd.createdby 
//WHERE cd.VA048_To is not null 
//and cd.Record_ID = " + _Record_ID;
//                }

                if (searchText != "undefined" && searchText != null && searchText != "")
                {
                    strApp += " AND upper(cd.VA048_To) like upper('%" + searchText + "%')";
                }
            }

            strApp += ")" + (VAdvantage.DataBase.DB.IsPostgreSQL() ? " as foo" : "") + " ORDER BY created DESC";

            DataSet ds = DB.ExecuteDatasetPaging(strApp, historyPageNo, pageSize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    HistoryRecordInfo info = new HistoryRecordInfo();
                    info.id = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]);
                    info.record_id = Convert.ToInt32(ds.Tables[0].Rows[i]["record_ID"]);
                    info.created = Convert.ToDateTime(ds.Tables[0].Rows[i]["created"]);
                    info.type = Convert.ToString(ds.Tables[0].Rows[i]["TYPE"]);
                    info.name = Convert.ToString(ds.Tables[0].Rows[i]["name"]);
                    info.subject = Convert.ToString(ds.Tables[0].Rows[i]["Subject"]);
                    hLists.Add(info);
                }
            }

            return hLists;
        }

        /// <summary>
        /// Fetch history bind to the current business partner from all tables in DB
        /// </summary>
        /// <param name="bPartner_ID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public RealtedHistoryInfoDetails history(int bPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText, string keyColName)
        {
            RealtedHistoryInfoDetails hDetails = new RealtedHistoryInfoDetails();
            List<RelatedHistoryInfo> histo = new List<RelatedHistoryInfo>();

            string tableName = keyColName.Substring(0, keyColName.Length - 3);

            //select all tables where VAF_UserContact_ID is used and have an entry in attachment(appointmentinfo OR mailattachment1)

            var sql = @"SELECT ab.VAF_TableView_ID, ab.Record_ID, att.TableName,ab.Attachtype From (select distinct vaf_tableview_id,record_ID,'A' as Attachtype from appointmentsinfo where vaf_tableview_id IN(     SELECT DISTINCT t.VAF_TableView_ID
                        FROM VAF_TableView t WHERE t.VAF_TableView_ID IN (SELECT VAF_TableView_ID FROM VAF_Column WHERE ColumnName='" + keyColName + "' ) AND TableName NOT LIKE 'I%' AND TableName NOT LIKE '" + tableName + @"' )
                        UNION
                        select distinct vaf_tableview_id,record_ID,'M' as Attachtype from mailattachment1 where vaf_tableview_id IN(     SELECT DISTINCT t.VAF_TableView_ID
                        FROM VAF_TableView t WHERE t.VAF_TableView_ID IN (SELECT VAF_TableView_ID FROM VAF_Column WHERE ColumnName='" + keyColName + "' ) AND TableName NOT LIKE 'I%' AND TableName NOT LIKE '" + tableName + @"' )
                        ) ab JOIn VAF_TableView att on att.VAF_TableView_ID=ab.VAF_TableView_ID order by TableName";


            DataSet ds = DB.ExecuteDataset(sql);
            StringBuilder finalsql = new StringBuilder();
            StringBuilder finalSqlCount = new StringBuilder();




            List<string> tableNames = new List<string>();   //contains tables 
            List<int> VAF_TableView_ID = new List<int>();        //Contains TableIDs
            List<string> recIDsA = new List<string>();      //contains record ids  of tables from appointment info like recIDsA[0]="123123,123,123123,12312"
            List<string> recIDsM = new List<string>();      //contains record ids of tables from MailAttachment1 info like recIDsM[0]="123123,123,123123,12312"
            StringBuilder curRecordA = new StringBuilder();
            StringBuilder curRecordM = new StringBuilder();


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                {
                    if (curRecordA.Length > 0)
                    {
                        recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                        curRecordA.Clear();
                    }
                    else if (i > 0)
                    {
                        recIDsA.Add("");
                    }


                    if (curRecordM.Length > 0)
                    {
                        recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                        curRecordM.Clear();
                    }
                    else if (i > 0)
                    {
                        recIDsM.Add("");
                    }

                    tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                    VAF_TableView_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"].ToString()));
                }

                if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                {
                    curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                }
                else
                {
                    curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                }


            } //these lists are being filled with data in this loop


            if (curRecordA.Length > 0)
            {
                recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                curRecordA.Clear();
            }
            else
            {
                recIDsA.Add("");
            }


            if (curRecordM.Length > 0)
            {
                recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                curRecordM.Clear();
            }
            else
            {
                recIDsM.Add("");
            }





            for (int i = 0; i < tableNames.Count; i++)// This will create where clause like (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013547) OR (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013548)
            {
                createquer(VAF_TableView_ID[i], tableNames[i], "", recIDsA[i], recIDsM[i], ctx, keyColName, bPartner_ID);
            }
            if (whereApp.Length > 0)
            {
                whereApp.Remove(whereApp.Length - 4, 3);
            }
            if (whereMAtt.Length > 0)
            {
                whereMAtt.Remove(whereMAtt.Length - 4, 3);
            }




            finalSqlCount.Append(@"Select COUNT(*) from (SELECT created FROM ");


            // For Appointments
            finalsql.Append("  SELECT n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.VAF_TableView_ID FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
            finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
            if (whereApp.Length > 0)
            {
                finalsql.Append(" WHERE " + whereApp.ToString());
                finalSqlCount.Append(" WHERE " + whereApp.ToString());
            }
            else
            {
                finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }

            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
                else
                {
                    finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
            }

            if (finalsql.ToString().Contains("WHERE"))
            {
                finalsql.Append(" AND ai.IsTask='N' AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
                finalSqlCount.Append(" AND ai.IsTask='N' AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
            }
            else
            {
                finalsql.Append(" WHERE  ai.IsTask='N' ");
                finalSqlCount.Append(" WHERE  ai.IsTask='N' ");
            }


            finalsql.Append("  UNION ");
            finalSqlCount.Append("  UNION ");


            //For Tasks
            finalsql.Append("  SELECT n'T' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Task") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.VAF_TableView_ID FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
            finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
            if (whereApp.Length > 0)
            {
                finalsql.Append(" WHERE " + whereApp.ToString());
                finalSqlCount.Append(" WHERE " + whereApp.ToString());
            }
            else
            {
                finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }

            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
                else
                {
                    finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
            }


            if (finalsql.ToString().Contains("WHERE"))
            {
                finalsql.Append(" AND  ai.IsTask='Y' ");
                finalSqlCount.Append(" AND  ai.IsTask='Y' ");
            }
            else
            {
                finalsql.Append(" WHERE  ai.IsTask='Y' ");
                finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
            }


            finalsql.Append("  UNION ");
            finalSqlCount.Append("  UNION ");


            // For Letter, sent Mail, Inbox mail

            finalsql.Append(" SELECT attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.VAF_TableView_ID   FROM mailattachment1 ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID  ");
            finalSqlCount.Append(" SELECT ai.created FROM mailattachment1  ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
            if (whereMAtt.Length > 0)
            {
                finalsql.Append(" WHERE " + whereMAtt.ToString());

                finalSqlCount.Append("WHERE " + whereMAtt.ToString());

            }
            else
            {
                finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                    finalSqlCount.Append(" AND ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                }
                else
                {
                    finalsql.Append(" WHERE ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                    finalSqlCount.Append(" WHERE ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                }
            }


            finalsql.Append(" order by created desc  ");
            if (VAdvantage.DataBase.DB.IsPostgreSQL())
            {
                finalSqlCount.Append(" )) as sub1) as sub2 ");
            }
            else
            {
                finalSqlCount.Append(" ))) ");
            }

            if (finalsql.Length > 0)
            {

                hDetails.TotalRecords = Util.GetValueOfInt(DB.ExecuteScalar(finalSqlCount.ToString(), null, null));

                DataSet dsfinal = VIS.DBase.DB.ExecuteDatasetPaging(finalsql.ToString(), pageNo, pageSize);

                dsfinal = VAdvantage.DataBase.DB.SetUtcDateTime(dsfinal);

                finalsql.Clear();
                finalsql.Length = 0;
                finalsql = null;

                finalSqlCount.Clear();
                finalSqlCount.Length = 0;
                finalSqlCount = null;


                if (dsfinal != null && dsfinal.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsfinal.Tables[0].Rows.Count; i++)
                    {
                        RelatedHistoryInfo related = new RelatedHistoryInfo();
                        related.ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["ID"]);
                        related.Record_ID = dsfinal.Tables[0].Rows[i]["record_ID"].ToString();
                        related.Created = DateTime.SpecifyKind(Convert.ToDateTime(dsfinal.Tables[0].Rows[i]["created"]), DateTimeKind.Utc);
                        related.Type = dsfinal.Tables[0].Rows[i]["attachmenttype"].ToString();
                        related.Subject = dsfinal.Tables[0].Rows[i]["Subject"].ToString();
                        related.TableName = dsfinal.Tables[0].Rows[i]["TableName"].ToString();
                        related.VAF_TableView_ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["VAF_TableView_ID"]);
                        histo.Add(related);
                    }
                }

                dsfinal.Clear();
                dsfinal = null;

            }

            //return list of selcted records 
            hDetails.RHistory = histo;
            return hDetails;

        }



        StringBuilder whereApp = new StringBuilder();
        StringBuilder whereMAtt = new StringBuilder();

        /// <summary>
        /// this list contains where clause for each user 
        /// </summary>
        List<UserInfoForWhereClause> userQuery = new List<UserInfoForWhereClause>();

        public void GetHistoryOfUserTable(int VAB_BusinessPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText)
        {
            RealtedHistoryInfoDetails hDetails = new RealtedHistoryInfoDetails();

            //select all tables where VAF_UserContact_ID is used and have an entry in attachment(appointmentinfo OR mailattachment1)

            int userTableID = MTable.Get_Table_ID("VAF_UserContact");

            var sql = @"SELECT VAF_TableView_ID,Record_ID,Attachtype,TableName FROM (SELECT DISTINCT vaf_tableview_id, record_ID, 'A' AS Attachtype,'VAF_UserContact' as TableName FROM appointmentsinfo
                            WHERE vaf_tableview_id IN(" + userTableID + @") UNION SELECT DISTINCT vaf_tableview_id, record_ID, 'M' AS Attachtype,'VAF_UserContact' as TableName FROM mailattachment1
                            WHERE vaf_tableview_id IN(" + userTableID + @"))";
            DataSet ds = DB.ExecuteDataset(sql);


            //get all users of bpartner
            sql = "SELECT VAF_UserContact_ID  FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND IsActive='Y' ORDER BY VAF_UserContact_ID";
            DataSet dsUsers = DB.ExecuteDataset(sql);

            if (dsUsers != null && dsUsers.Tables[0].Rows.Count == 0)
            {
                return;
            }

            string users = "";

            for (int a = 0; a < dsUsers.Tables[0].Rows.Count; a++)
            {
                users += dsUsers.Tables[0].Rows[a]["VAF_UserContact_ID"] + ",";
            }
            users = users.Substring(0, users.Length - 1);  //users in string like 101,102



            StringBuilder finalsql = new StringBuilder();
            StringBuilder finalSqlCount = new StringBuilder();

            try
            {
                List<string> tableNames = new List<string>();//contains tables 
                List<int> VAF_TableView_ID = new List<int>();     //Contains TableIDs
                List<string> recIDsA = new List<string>();   //contains record ids  of tables from appointment info like recIDsA[0]="123123,123,123123,12312"
                List<string> recIDsM = new List<string>();   //contains record ids of tables from MailAttachment1 info like recIDsM[0]="123123,123,123123,12312"
                StringBuilder curRecordA = new StringBuilder();
                StringBuilder curRecordM = new StringBuilder();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                    {
                        if (curRecordA.Length > 0)
                        {
                            recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                            curRecordA.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsA.Add("");
                        }


                        if (curRecordM.Length > 0)
                        {
                            recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                            curRecordM.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsM.Add("");
                        }

                        tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                        VAF_TableView_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"].ToString()));
                    }

                    if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A" || ds.Tables[0].Rows[i]["Attachtype"].ToString() == "T")
                    {
                        curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }
                    else
                    {
                        curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }


                }           //these lists are being filled with data in this loop


                if (curRecordA.Length > 0)
                {
                    recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                    curRecordA.Clear();
                }
                else
                {
                    recIDsA.Add("");
                }


                if (curRecordM.Length > 0)
                {
                    recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                    curRecordM.Clear();
                }
                else
                {
                    recIDsM.Add("");
                }




                for (int i = 0; i < tableNames.Count; i++)// This will create where clause like (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013547) OR (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013548)
                {
                    createquerForUserTable(VAF_TableView_ID[i], tableNames[i], users, recIDsA[i], recIDsM[i], ctx, "", 0);
                }
            }
            catch (Exception ex)
            { }
        }


        private string createquerForUserTable(int VAF_TableView_ID, string tableName, string users, string recordsA, string recordsM, Ctx ctx, string colName, int colID)
        {

            if (recordsA.Length > 0 && recordsA.EndsWith(","))
            {
                recordsA = recordsA.Substring(0, recordsA.Length - 1);
            }

            if (recordsM.Length > 0 && recordsM.EndsWith(","))
            {
                recordsM = recordsM.Substring(0, recordsM.Length - 1);
            }

            if (recordsA.Length == 0)
            {
                recordsA = "''";
            }

            if (recordsM.Length == 0)
            {
                recordsM = "''";
            }




            string sql = "";

            if (users.Length > 0)  //will work in case of user history
            {
                sql = "SELECT aa.VAF_UserContact_ID,bb.Name,bb.VAF_UserContact_ID FROM VAF_UserContact aa JOIN VAF_UserContact bb ";
                sql += "  ON aa.VAF_UserContact_ID=bb.VAF_UserContact_ID WHERE aa.IsActive='Y' ";
                sql += "    AND  aa.VAF_UserContact_ID IN( " + users + ") ";
            }

            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                            MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


            DataSet dsRecords = DB.ExecuteDataset(sql);

            //if (colID > 0) //in case of related history...
            //{
            //    if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0)
            //    {
            //        for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
            //        {
            //            if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
            //            {
            //                whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
            //            }
            //            if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
            //            {
            //                whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
            //            }
            //        }
            //    }
            //}
            //else
            //{
            if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
                {
                    var key = Util.GetValueOfInt(dsRecords.Tables[0].Rows[j]["VAF_UserContact_ID"].ToString());


                    UserInfoForWhereClause uif = userQuery.Where(aa => aa.UserID == key).FirstOrDefault();

                    if (uif == null)
                    {
                        uif = new UserInfoForWhereClause() { UserID = key };
                        uif.UserName = dsRecords.Tables[0].Rows[j]["Name"].ToString();
                        uif.whereApp = new StringBuilder();
                        uif.whereMAtt = new StringBuilder();
                        if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            uif.whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                        if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            uif.whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                        userQuery.Add(uif);
                    }
                    else
                    {
                        if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            uif.whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                        if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            uif.whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                    }
                }
            }
            //}

            return "";
        }


        private string createquer(int VAF_TableView_ID, string tableName, string users, string recordsA, string recordsM, Ctx ctx, string colName, int colID)
        {
            if (tableName.ToUpper().Equals("VAF_USERCONTACT"))//skip VAF_UserContact Table
            {
                return "";
            }

            if (recordsA.Length > 0 && recordsA.EndsWith(","))
            {
                recordsA = recordsA.Substring(0, recordsA.Length - 1);
            }

            if (recordsM.Length > 0 && recordsM.EndsWith(","))
            {
                recordsM = recordsM.Substring(0, recordsM.Length - 1);
            }

            if (recordsA.Length == 0)
            {
                recordsA = "''";
            }

            if (recordsM.Length == 0)
            {
                recordsM = "''";
            }




            string sql = "";

            if (users.Length > 0)  //will work in case of user history
            {
                sql = "SELECT aa." + tableName + "_ID,bb.Name,bb.VAF_UserContact_ID FROM " + tableName + " aa JOIN VAF_UserContact bb ";
                if (colName.Length > 0)  // if refrenece is Table and refrence column is VAF_UserContact_ID like SalesRep_ID
                {
                    sql += "  ON aa." + colName + "=bb.VAF_UserContact_ID WHERE aa.IsActive='Y' ";
                    sql += "    AND  aa." + colName + " IN (" + users + ") ";
                }
                else// if column Name is VAF_UserContact_ID
                {
                    sql += "  ON aa.VAF_UserContact_ID=bb.VAF_UserContact_ID WHERE aa.IsActive='Y' ";
                    sql += "    AND  aa.VAF_UserContact_ID IN( " + users + ") ";
                }
            }
            if (colID > 0)      // will work in case of related history records
            {
                sql = "SELECT " + tableName + "_ID FROM " + tableName + " WHERE IsActive='Y' ";
                sql += "    AND  " + colName + " = " + colID + " ";
                sql += " AND (" + tableName + "_ID IN (" + recordsA + ") OR " + tableName + "_ID IN (" + recordsM + "))";
            }







            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                            MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


            DataSet dsRecords = DB.ExecuteDataset(sql);

            if (colID > 0) //in case of related history...
            {
                if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
                    {
                        if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                        if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                    }
                }
            }
            else
            {
                if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
                    {
                        var key = Util.GetValueOfInt(dsRecords.Tables[0].Rows[j]["VAF_UserContact_ID"].ToString());


                        UserInfoForWhereClause uif = userQuery.Where(aa => aa.UserID == key).FirstOrDefault();

                        if (uif == null)
                        {
                            uif = new UserInfoForWhereClause() { UserID = key };
                            uif.UserName = dsRecords.Tables[0].Rows[j]["Name"].ToString();
                            uif.whereApp = new StringBuilder();
                            uif.whereMAtt = new StringBuilder();
                            if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            userQuery.Add(uif);
                        }
                        else
                        {
                            if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereApp.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereMAtt.Append(" (ai.VAF_TableView_ID=" + VAF_TableView_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Fetch history bind to the current business partner from all tables in DB
        /// </summary>
        /// <param name="bPartner_ID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public RealtedHistoryInfoDetails Userhistory(int VAB_BusinessPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText)
        {
            GetHistoryOfUserTable(VAB_BusinessPartner_ID, pageSize, pageNo, ctx, searchText);

            RealtedHistoryInfoDetails hDetails = new RealtedHistoryInfoDetails();
            List<RelatedHistoryInfo> histo = new List<RelatedHistoryInfo>();


            //select all tables where VAF_UserContact_ID is used and have an entry in attachment(appointmentinfo OR mailattachment1)
            var sql = @"SELECT ab.VAF_TableView_ID, ab.Record_ID, att.TableName,ab.Attachtype From (select distinct vaf_tableview_id,record_ID,'A' as Attachtype from appointmentsinfo where vaf_tableview_id IN(     SELECT DISTINCT t.VAF_TableView_ID
                        FROM VAF_TableView t WHERE t.VAF_TableView_ID IN (SELECT VAF_TableView_ID FROM VAF_Column WHERE ColumnName='VAF_UserContact_ID' ) AND TableName NOT LIKE 'I%' )
                        UNION
                        select distinct vaf_tableview_id,record_ID,'M' as Attachtype from mailattachment1 where vaf_tableview_id IN(     SELECT DISTINCT t.VAF_TableView_ID
                        FROM VAF_TableView t WHERE t.VAF_TableView_ID IN (SELECT VAF_TableView_ID FROM VAF_Column WHERE ColumnName='VAF_UserContact_ID' ) AND TableName NOT LIKE 'I%' )
                        ) ab JOIn VAF_TableView att on att.VAF_TableView_ID=ab.VAF_TableView_ID order by TableName";
            DataSet ds = DB.ExecuteDataset(sql);


            //get all users of bpartner
            sql = "SELECT VAF_UserContact_ID  FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND IsActive='Y' ORDER BY VAF_UserContact_ID";
            DataSet dsUsers = DB.ExecuteDataset(sql);

            if (dsUsers != null && dsUsers.Tables[0].Rows.Count == 0)
            {
                return hDetails;
            }

            string users = "";

            for (int a = 0; a < dsUsers.Tables[0].Rows.Count; a++)
            {
                users += dsUsers.Tables[0].Rows[a]["VAF_UserContact_ID"] + ",";
            }
            users = users.Substring(0, users.Length - 1);  //users in string like 101,102



            StringBuilder finalsql = new StringBuilder();
            StringBuilder finalSqlCount = new StringBuilder();

            try
            {
                List<string> tableNames = new List<string>();//contains tables 
                List<int> VAF_TableView_ID = new List<int>();     //Contains TableIDs
                List<string> recIDsA = new List<string>();   //contains record ids  of tables from appointment info like recIDsA[0]="123123,123,123123,12312"
                List<string> recIDsM = new List<string>();   //contains record ids of tables from MailAttachment1 info like recIDsM[0]="123123,123,123123,12312"
                StringBuilder curRecordA = new StringBuilder();
                StringBuilder curRecordM = new StringBuilder();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                    {
                        if (curRecordA.Length > 0)
                        {
                            recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                            curRecordA.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsA.Add("");
                        }


                        if (curRecordM.Length > 0)
                        {
                            recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                            curRecordM.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsM.Add("");
                        }

                        tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                        VAF_TableView_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"].ToString()));
                    }

                    if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                    {
                        curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }
                    else
                    {
                        curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }


                }           //these lists are being filled with data in this loop


                if (curRecordA.Length > 0)
                {
                    recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                    curRecordA.Clear();
                }
                else
                {
                    recIDsA.Add("");
                }


                if (curRecordM.Length > 0)
                {
                    recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                    curRecordM.Clear();
                }
                else
                {
                    recIDsM.Add("");
                }




                for (int i = 0; i < tableNames.Count; i++)// This will create where clause like (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013547) OR (ai.VAF_TableView_ID     =259 AND ai.Record_ID       =1013548)
                {
                    createquer(VAF_TableView_ID[i], tableNames[i], users, recIDsA[i], recIDsM[i], ctx, "", 0);
                }

                finalSqlCount.Append(@"Select COUNT(*) from (SELECT created FROM ");


                // here in this query we will find all columns which has Table reference and Key Column is VAF_UserContact_ID.
                //Createdby and UpdatedBy


                sql = @"select * from (select distinct ab.VAF_TableView_ID,ab.ColumnName,ai.record_ID,'A'  as Attachtype,att.TableName from appointmentsinfo ai JOIN   (SELECT  VAF_TableView_ID,
                                ColumnName FROM VAF_Column WHERE lower(ColumnName)   !='updatedby' AND lower(columnname)     !='createdby' AND VAF_Control_Ref_Value_id IN
                                (SELECT VAF_Control_Ref_id FROM VAF_CtrlRef_Table WHERE column_key_id= (SELECT VAF_Column_ID FROM VAF_Column  WHERE columnname='VAF_UserContact_ID'
                                  AND VAF_TableView_ID = (SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName='VAF_UserContact' ) ) ) ) ab ON ab.VAF_TableView_ID=ai.VAF_TableView_ID
                              JOIN VAF_TableView att on ai.VAF_TableView_ID=att.VAF_TableView_ID
                              UNION
                              select distinct ab.VAF_TableView_ID,ab.ColumnName,ai.record_ID,'M' as  Attachtype,att.TableName from mailattachment1 ai JOIN   (SELECT  VAF_TableView_ID,
                                ColumnName FROM VAF_Column WHERE lower(ColumnName)   !='updatedby' AND lower(columnname)     !='createdby' AND VAF_Control_Ref_Value_id IN
                                (SELECT VAF_Control_Ref_id FROM VAF_CtrlRef_Table WHERE column_key_id= (SELECT VAF_Column_ID FROM VAF_Column WHERE columnname='VAF_UserContact_ID'
                                  AND VAF_TableView_ID = (SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName='VAF_UserContact' ) ) ) ) ab ON ab.VAF_TableView_ID=ai.VAF_TableView_ID ";
                             // JOIN VAF_TableView att on ai.VAF_TableView_ID=att.VAF_TableView_ID) as foo order by tablename";

                if (VAdvantage.DataBase.DB.IsPostgreSQL())
                {
                    sql += " JOIN VAF_TableView att on ai.VAF_TableView_ID=att.VAF_TableView_ID)  as foo order by tablename";
                }
                else
                {
                    sql += " JOIN VAF_TableView att on ai.VAF_TableView_ID=att.VAF_TableView_ID) order by tablename";
                }
                ds = DB.ExecuteDataset(sql);


                tableNames = new List<string>();
                VAF_TableView_ID = new List<int>();
                recIDsA = new List<string>();
                recIDsM = new List<string>();
                curRecordA = new StringBuilder();
                curRecordM = new StringBuilder();

                //here in this dictionary, we will have TableName as key  and columnNames in List<string>.
                //here list<string> is used because there may be more that one column with Table reference in a table like Salesrep_ID and BILL_USER_ID in VAB_Order

                Dictionary<string, List<string>> colNamess = new Dictionary<string, List<string>>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                    {
                        if (curRecordA.Length > 0)
                        {
                            recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                            curRecordA.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsA.Add("");
                        }


                        if (curRecordM.Length > 0)
                        {
                            recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                            curRecordM.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsM.Add("");
                        }

                        tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                        VAF_TableView_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TableView_ID"].ToString()));

                        List<string> lst = new List<string>();
                        lst.Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                        colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()] = lst;
                    }

                    if (!colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()].Contains(ds.Tables[0].Rows[i]["ColumnName"].ToString()))
                    {
                        colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()].Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                    {
                        curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }
                    else
                    {
                        curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }


                }


                if (curRecordA.Length > 0)
                {
                    recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                    curRecordA.Clear();
                }
                else
                {
                    recIDsA.Add("");
                }


                if (curRecordM.Length > 0)
                {
                    recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                    curRecordM.Clear();
                }
                else
                {
                    recIDsM.Add("");
                }




                for (int i = 0; i < tableNames.Count; i++)
                {
                    for (int j = 0; j < colNamess[tableNames[i]].Count; j++)        // for each column in each table we are prepareing where clause...
                    {
                        createquer(VAF_TableView_ID[i], tableNames[i], users, recIDsA[i], recIDsM[i], ctx, colNamess[tableNames[i]][j], 0);
                    }
                }


                if (userQuery.Count > 0)
                {
                    for (int i = 0; i < userQuery.Count; i++)           // Create   query 
                    {
                        if (userQuery[i].whereApp.Length > 0)
                        {
                            userQuery[i].whereApp = userQuery[i].whereApp.Remove(userQuery[i].whereApp.Length - 4, 3);
                        }
                        if (userQuery[i].whereMAtt.Length > 0)
                        {
                            userQuery[i].whereMAtt = userQuery[i].whereMAtt.Remove(userQuery[i].whereMAtt.Length - 4, 3);
                        }
                        if (i > 0)
                        {
                            finalSqlCount.Append(" UNION ");
                            finalsql.Append(" UNION ");
                        }

                        // For Appointment
                        finalsql.Append("  SELECT '" + userQuery[i].UserName + "' as UserName,  n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.VAF_TableView_ID FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
                        finalSqlCount.Append(" (SELECT ai.created FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
                        if (userQuery[i].whereApp.Length > 0)
                        {
                            finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                            finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                        }
                        else
                        {
                            finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                            finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        }

                        if (searchText != "undefined" && searchText != null && searchText != "")
                        {
                            if (finalsql.ToString().Contains("WHERE"))
                            {
                                finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                                finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                            }
                            else
                            {
                                finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                                finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                            }
                        }
                        if (finalsql.ToString().Contains("WHERE"))
                        {
                            finalsql.Append(" AND ai.IsTask='N' AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
                            finalSqlCount.Append(" AND ai.IsTask='N' AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
                        }
                        else
                        {
                            finalsql.Append(" WHERE  ai.IsTask='N' AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
                            finalSqlCount.Append(" WHERE  ai.IsTask='N'  AND ai.VAF_UserContact_ID= " + ctx.GetVAF_UserContact_ID());
                        }

                        if (VAdvantage.DataBase.DB.IsPostgreSQL())
                        {
                            finalSqlCount.Append(")  as hoo");
                        }
                        else
                        {
                            finalSqlCount.Append(")");
                        }

                        finalsql.Append("  UNION ");
                        finalSqlCount.Append("  UNION ");


                        //For Tasks
                        finalsql.Append("  SELECT  '" + userQuery[i].UserName + "' as UserName, n'T' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Task") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.VAF_TableView_ID FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
                        finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
                        if (userQuery[i].whereApp.Length > 0)
                        {
                            finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                            finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                        }
                        else
                        {
                            finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                            finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        }

                        if (searchText != "undefined" && searchText != null && searchText != "")
                        {
                            if (finalsql.ToString().Contains("WHERE"))
                            {
                                finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                                finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                            }
                            else
                            {
                                finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                                finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                            }
                        }
                        if (finalsql.ToString().Contains("WHERE"))
                        {
                            finalsql.Append(" AND  ai.IsTask='Y' ");
                            finalSqlCount.Append(" AND  ai.IsTask='Y' ");
                        }
                        else
                        {
                            finalsql.Append(" WHERE  ai.IsTask='Y' ");
                            finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
                        }

                        finalSqlCount.Append(")");
                        finalsql.Append("  UNION ");
                        finalSqlCount.Append("  UNION ");


                        //For Letter, Sent mail, Inbox Mail
                        finalsql.Append(" SELECT  '" + userQuery[i].UserName + "' as UserName, attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.VAF_TableView_ID   FROM mailattachment1 ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby  JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID  ");
                        finalSqlCount.Append(" (SELECT ai.created FROM mailattachment1  ai JOIN VAF_UserContact au on au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt on adt.VAF_TableView_ID =ai.VAF_TableView_ID ");
                        if (userQuery[i].whereMAtt.Length > 0)
                        {
                            finalsql.Append(" WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                            finalSqlCount.Append("WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                        }
                        else
                        {
                            finalsql.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                            finalSqlCount.Append("WHERE (ai.VAF_TableView_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        }

                        if (searchText != "undefined" && searchText != null && searchText != "")
                        {
                            if (finalsql.ToString().Contains("WHERE"))
                            {
                                finalsql.Append(" AND ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                                finalSqlCount.Append(" AND ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                            }
                            else
                            {
                                finalsql.Append(" WHERE ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                                finalSqlCount.Append(" WHERE ((upper(ai.Title)  like upper('%" + searchText + "%')) OR (upper(mailaddressfrom) like upper('%" + searchText + "%')) OR (upper(mailaddress) like upper('%" + searchText + "%')) OR (upper(mailaddressbcc) like upper('%" + searchText + "%')) OR (upper(mailaddresscc) like upper('%" + searchText + "%')))");
                            }
                        }


                        finalSqlCount.Append(")");


                    }


                    finalsql.Append(" order by created desc  ");
                    if (VAdvantage.DataBase.DB.IsPostgreSQL())
                    {
                        finalSqlCount.Append(" ) as foo");
                    }
                    else
                    {
                        finalSqlCount.Append(" )");
                    }
                }
                else
                {
                    finalsql.Clear();
                    finalSqlCount.Clear();

                    finalSqlCount.Append(@"SELECT count(*) FROM mailattachment1 ai JOIN VAF_UserContact au
ON au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt ON adt.VAF_TableView_ID   =ai.VAF_TableView_ID WHERE (ai.VAF_TableView_ID=0 AND ai.Record_ID     =0) ");

                    finalsql.Append(@"SELECT 'IdeasIncAdmin' AS UserName, attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID, ai.created, '[SentMail]' AS TYPE,
  ai.TITLE     AS Subject, adt.Name     AS TableName, ai.VAF_TableView_ID FROM mailattachment1 ai JOIN VAF_UserContact au
ON au.VAF_UserContact_ID=ai.createdby JOIN VAF_TableView adt ON adt.VAF_TableView_ID   =ai.VAF_TableView_ID WHERE (ai.VAF_TableView_ID=0 AND ai.Record_ID     =0) ORDER BY created DESC ");

                }




            }
            catch
            {

            }

            hDetails.TotalRecords = Util.GetValueOfInt(DB.ExecuteScalar(finalSqlCount.ToString(), null, null));

            DataSet dsfinal = VIS.DBase.DB.ExecuteDatasetPaging(finalsql.ToString(), pageNo, pageSize);

            dsfinal = VAdvantage.DataBase.DB.SetUtcDateTime(dsfinal);

            finalsql.Clear();
            finalsql = null;

            finalSqlCount.Clear();
            finalSqlCount = null;


            whereApp.Clear();
            whereApp = null;

            whereMAtt.Clear();
            whereMAtt = null;


            if (dsfinal != null && dsfinal.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsfinal.Tables[0].Rows.Count; i++)
                {
                    RelatedHistoryInfo related = new RelatedHistoryInfo();
                    related.ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["ID"]);
                    related.Record_ID = dsfinal.Tables[0].Rows[i]["record_ID"].ToString();
                    related.Created = DateTime.SpecifyKind(Convert.ToDateTime(dsfinal.Tables[0].Rows[i]["created"]), DateTimeKind.Utc);
                    related.Type = dsfinal.Tables[0].Rows[i]["attachmenttype"].ToString();
                    related.Subject = dsfinal.Tables[0].Rows[i]["Subject"].ToString();
                    related.TableName = dsfinal.Tables[0].Rows[i]["TableName"].ToString();
                    related.VAF_TableView_ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["VAF_TableView_ID"]);
                    related.UserName = Util.GetValueOfString(dsfinal.Tables[0].Rows[i]["UserName"]);
                    histo.Add(related);
                }
            }

            dsfinal.Clear();
            dsfinal = null;


            //return list of selcted records 
            hDetails.RHistory = histo;
            return hDetails;

        }

        /// <summary>
        /// when user click on sent mail record from histort form's Grid, then fetch its information like attachment, to , bccc etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo Sentmails(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select MAILADDRESSFROM, MAILADDRESS,TITLE,MailAddressBcc,MailAddressCc,CREATED" +
                          ",  ISATTACHMENT, MAILATTACHMENT1_ID, VAF_CLIENT_ID, VAF_ORG_ID, VAF_TABLEVIEW_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.To = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddress"]);
                info.From = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressfrom"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                info.Bcc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressbcc"]);
                info.Cc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddresscc"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["vaf_tableview_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on inbox mail record from histort form's Grid, then fetch its information like attachment, to , bccc etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo InboxMails(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select MAILADDRESSFROM, MAILADDRESS,MailAddressBcc,MailAddressCc,TITLE " +
                ",DateMailReceived ,created " +
                ",  ISATTACHMENT, MAILATTACHMENT1_ID, VAF_CLIENT_ID, VAF_ORG_ID, VAF_TABLEVIEW_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.To = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddress"]);
                info.From = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressfrom"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                info.Bcc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressbcc"]);
                info.Cc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddresscc"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["vaf_tableview_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on letter record from histort form's Grid, then fetch its information like attachment, subject etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo Letters(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select TITLE ,CREATED" +
                 ", ISATTACHMENT, MAILATTACHMENT1_ID, VAF_CLIENT_ID, VAF_ORG_ID, VAF_TABLEVIEW_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["vaf_tableview_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        // Updated for fetching call chat for history by vinay
        public List<ChatInfos> ViewChatonHistory(Ctx ctx, int record_ID, bool isAppointment, bool isCall = false)
        {
            string sql = @"SELECT inn.ChatID,inn.EntryID,CH.characterdata,ch.VACM_ChatLine,au.Name AS NAME,ch.created, ai.VAF_Image_id ,ai.binarydata  AS UsrImg,CH.createdby
                    FROM (SELECT * FROM (SELECT CH.VACM_Chat_ID    AS ChatID,MAX(CE.VACM_ChatLine)AS EntryID FROM VACM_ChatLine CE
                        JOIN VACM_Chat CH ON CE.VACM_Chat_ID= CH.VACM_Chat_ID GROUP BY CH.VACM_Chat_ID ORDER BY entryID )inn1  ) inn
                    JOIN VACM_ChatLine CH ON inn.ChatID= ch.VACM_Chat_ID JOIN VACM_Chat CMH ON (cmh.VACM_Chat_ID= inn.chatid)
                    JOIN VAF_UserContact Au ON au.VAF_UserContact_id= CH.createdBy LEFT OUTER JOIN VAF_Image AI ON(ai.VAF_Image_id=au.VAF_Image_id)";
            if (isAppointment)
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='appointmentsinfo')";
            }
            else if (isCall)
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='va048_calldetails')";
            }
            else
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='mailattachment1')";
            }


            sql += @" AND Record_ID=" + record_ID + @" 
                    ORDER BY inn.EntryID DESC,ch.VACM_ChatLine ASC";

            List<ChatInfos> cInfo = new List<ChatInfos>();

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    ChatInfos inf = new ChatInfos();
                    inf.ChatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.EntryID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.CharacterData = Util.GetValueOfString(ds.Tables[0].Rows[i]["characterdata"]);
                    inf.UserName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //inf.created = Util.GetValueOfString(ds.Tables[0].Rows[i]["created"]); ;

                    if (ds.Tables[0].Rows[i]["created"].ToString() != null && ds.Tables[0].Rows[i]["created"].ToString() != "")
                    {
                        DateTime _createdDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["created"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                        inf.Created = _format;
                    }
                    int uimgId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Image_id"].ToString());

                    if (uimgId > 0)
                    {
                        MImage mimg = new MImage(ctx, uimgId, null);
                        var imgfll = mimg.GetThumbnailURL(46, 46);
                        inf.UserImage = imgfll;
                    }
                    inf.CreatedBy = Util.GetValueOfInt(ds.Tables[0].Rows[i]["createdby"]);
                    cInfo.Add(inf);

                }
            }
            return cInfo;
        }

        // Updated for fetching call chat for history by vinay
        public List<ChatInfos> ViewChatonLastHistory(Ctx ctx, int record_ID, bool isAppointment, bool isCall = false)
        {
            string sql = @"SELECT * FROM (SELECT inn.ChatID,inn.EntryID,CH.characterdata,ch.VACM_ChatLine,au.Name AS NAME,ch.created, ai.VAF_Image_id ,ai.binarydata  AS UsrImg,CH.createdby
                    FROM (SELECT * FROM (SELECT CH.VACM_Chat_ID    AS ChatID,MAX(CE.VACM_ChatLine)AS EntryID FROM VACM_ChatLine CE
                        JOIN VACM_Chat CH ON CE.VACM_Chat_ID= CH.VACM_Chat_ID GROUP BY CH.VACM_Chat_ID ORDER BY entryID )inn1  ) inn
                    JOIN VACM_ChatLine CH ON inn.ChatID= ch.VACM_Chat_ID JOIN VACM_Chat CMH ON (cmh.VACM_Chat_ID= inn.chatid)
                    JOIN VAF_UserContact Au ON au.VAF_UserContact_id= CH.createdBy LEFT OUTER JOIN VAF_Image AI ON(ai.VAF_Image_id=au.VAF_Image_id)";
            if (isAppointment)
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='appointmentsinfo')";
            }
            else if (isCall)
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='va048_calldetails')";
            }
            else
            {
                sql += " WHERE cMh.VAF_TableView_ID =(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='mailattachment1')";
            }


            sql += @" AND Record_ID=" + record_ID + @" 
                    ORDER BY inn.EntryID DESC,ch.VACM_ChatLine DESC ) WHERE rownum=1 ";

            List<ChatInfos> cInfo = new List<ChatInfos>();

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    ChatInfos inf = new ChatInfos();
                    inf.ChatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.EntryID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.CharacterData = Util.GetValueOfString(ds.Tables[0].Rows[i]["characterdata"]);
                    inf.UserName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //inf.created = Util.GetValueOfString(ds.Tables[0].Rows[i]["created"]); ;

                    if (ds.Tables[0].Rows[i]["created"].ToString() != null && ds.Tables[0].Rows[i]["created"].ToString() != "")
                    {
                        DateTime _createdDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["created"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                        inf.Created = _format;
                    }
                    int uimgId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Image_id"].ToString());

                    if (uimgId > 0)
                    {
                        MImage mimg = new MImage(ctx, uimgId, null);
                        var imgfll = mimg.GetThumbnailURL(46, 46);
                        inf.UserImage = imgfll;
                    }
                    inf.CreatedBy = Util.GetValueOfInt(ds.Tables[0].Rows[i]["createdby"]);
                    cInfo.Add(inf);

                }
            }
            return cInfo;
        }



        /// <summary>
        /// Used to save comment on on particular history record.
        /// always update  privious one...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="text"></param>
        /// <param name="isAppointment"></param>
        public List<ChatInfos> SaveComment(int ID, string text, bool isAppointment, Ctx ctx, bool isCall = false)
        {
            object chatID = 0;
            if (isAppointment)
            {
                string sql = "SELECT VACM_Chat_ID FROM VACM_Chat WHERE VAF_TableView_ID=(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='appointmentsinfo') and Record_ID=" + ID;
                chatID = DB.ExecuteScalar(sql, null, null);
                if (chatID == null || chatID == DBNull.Value)
                {
                    chatID = DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='appointmentsinfo'", null, null);

                    MChat chats = new MChat(ctx, 0, null);
                    chats.SetVAF_TableView_ID(Convert.ToInt32(chatID));
                    chats.SetRecord_ID(ID);
                    chats.SetDescription("");
                    chats.Save();
                    chatID = chats.GetCM_Chat_ID();
                }

            }
            else if (isCall) // Updated for fetching call comments for history by vinay
            {
                string sql = "SELECT VACM_Chat_ID FROM VACM_Chat WHERE VAF_TableView_ID=(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='va048_calldetails') and Record_ID=" + ID;
                chatID = DB.ExecuteScalar(sql, null, null);
                if (chatID == null || chatID == DBNull.Value)
                {
                    chatID = DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='va048_calldetails'", null, null);

                    MChat chats = new MChat(ctx, 0, null);
                    chats.SetVAF_TableView_ID(Convert.ToInt32(chatID));
                    chats.SetRecord_ID(ID);
                    chats.SetDescription("");
                    chats.Save();
                    chatID = chats.GetCM_Chat_ID();
                }
            }
            else
            {
                string sql = "SELECT VACM_Chat_ID FROM VACM_Chat WHERE VAF_TableView_ID=(SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='mailattachment1') and Record_ID=" + ID;
                chatID = DB.ExecuteScalar(sql, null, null);
                if (chatID == null || chatID == DBNull.Value)
                {
                    chatID = DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE lower(TableName)='mailattachment1'", null, null);
                    MChat chats = new MChat(ctx, 0, null);
                    chats.SetVAF_TableView_ID(Convert.ToInt32(chatID));
                    chats.SetRecord_ID(ID);
                    chats.SetDescription("");
                    chats.Save();
                    chatID = chats.GetCM_Chat_ID();
                }
            }

            MChatEntry entry = new MChatEntry(ctx, 0, null);
            entry.SetCM_Chat_ID(Convert.ToInt32(chatID));
            entry.SetCharacterData(text);
            entry.SetVAF_UserContact_ID(ctx.GetVAF_UserContact_ID());
            entry.Save();

            List<ChatInfos> cInfo = ViewChatonHistory(ctx, ID, isAppointment, isCall);
            return cInfo;

        }

        // Added by Bharat on 07 June 2017
        public Dictionary<string, object> GetAppointmentData(string sql)
        {
            Dictionary<string, object> obj = null;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["AppointmentsInfo_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AppointmentsInfo_ID"]);
                obj["Subject"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Subject"]);
                obj["Location"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Location"]);
                obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                obj["VAF_TableView_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                obj["Record_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Record_ID"]);
                obj["label"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["label"]);
                obj["StartDate"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["StartDate"]);
                obj["EndDate"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["EndDate"]);
                obj["Allday"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Allday"]);
                obj["Status"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Status"]);
                obj["ReminderInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["ReminderInfo"]);
                obj["AttendeeInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["AttendeeInfo"]);
                obj["RecurrenceInfo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["RecurrenceInfo"]);
                obj["IsPrivate"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsPrivate"]);
                obj["comments"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                obj["caname"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["caname"]);
            }
            return obj;
        }

        // Added by Bharat on 07 June 2017
        public List<string> GetUser(string sql)
        {
            List<string> obj = null;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                }
            }
            return obj;
        }

        /// <summary>
        /// by vinay: when user click on history record, then fetch its information like details, attachment etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public CallInfo Calls(Ctx ctx, int ID)
        {
            CallInfo info = new CallInfo();

            string fullNameColumn = string.Empty;
            int getfullnamecolumn = Util.GetValueOfInt(DB.ExecuteScalar("select count(1) from VAF_Column where VAF_TableView_ID = 114 AND ColumnName = 'FullName'", null, null));

            if (getfullnamecolumn > 0)
                fullNameColumn = "CASE WHEN ad.FullName IS NULL THEN ad.Name || ' ' || ad.LastName ELSE ad.FullName END ";
            else
                fullNameColumn = "ad.Name || ' ' || ad.LastName ";

            var strSql = @"
select cd.VA048_IsConference, NVL(cd.VA048_From, '-') as VA048_From, NVL(cd.VA048_To, '-') as VA048_To, cd.VA048_Duration, cd.VA048_Price, 
NVL(cd.VA048_Price_Unit, '-') as VA048_Price_Unit, NVL(cd.VA048_Status, '-') as VA048_Status, 
cd.VAF_TableView_ID, cd.Record_ID, cd.VA048_CallNotes, cd.Created,  
" + fullNameColumn + @" AS VA048_FullName
FROM VA048_CallDetails cd
left join VAF_UserContact ad on cd.CreatedBy = ad.VAF_UserContact_ID 
where cd.IsActive = 'Y' and cd.VA048_CallDetails_ID = " + ID;

            var ds = DB.ExecuteDataset(strSql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                string attachemntsql = @"
select att.VAF_Attachment_ID, att.FileLocation, atl.VAF_AttachmentLine_ID, atl.FileName, atl.FileType from VAF_Attachment att 
left join VAF_AttachmentLine atl on att.VAF_Attachment_ID = atl.VAF_Attachment_ID
where att.vaf_tableview_ID = " + MTable.Get_Table_ID("VA048_CallDetails") + @" and att.Record_ID = " + ID;

                var dsatt = DB.ExecuteDataset(attachemntsql);

                if (dsatt != null && dsatt.Tables.Count > 0 && dsatt.Tables[0].Rows.Count > 0)
                {
                    info.Attach = new List<AttachmentInfos>();
                    AttachmentInfos ai = new AttachmentInfos();
                    for (int i = 0; i < dsatt.Tables[0].Rows.Count; i++)
                    {
                        ai.Name = Util.GetValueOfString(dsatt.Tables[0].Rows[i]["FileName"]);
                        ai.ID = Util.GetValueOfInt(dsatt.Tables[0].Rows[i]["VAF_AttachmentLine_ID"]);
                        ai.AttID = Util.GetValueOfInt(dsatt.Tables[0].Rows[i]["VAF_Attachment_ID"]);
                        info.Attach.Add(ai);
                    }
                    
                }

                //MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
                //List<AttachmentInfos> attac = new List<AttachmentInfos>();
                //foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
                //{
                //    AttachmentInfos i = new AttachmentInfos();
                //    i.Name = oMAttachEntry.GetName();
                //    i.ID = ID;
                //    attac.Add(i);
                //}
                //info.Attach = attac;

                info.VA048_IsConference = Util.GetValueOfChar(ds.Tables[0].Rows[0]["VA048_IsConference"].ToString());
                info.VA048_From = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_From"]);
                info.VA048_To = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_To"]);
                info.VA048_Duration = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Duration"]);
                info.VA048_Price = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["VA048_Price"].ToString().Replace("-", ""));
                info.VA048_Price_Unit = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Price_Unit"]);
                info.VA048_Status = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_Status"]);
                info.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_TableView_ID"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Record_ID"]);
                info.VA048_CallNotes = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_CallNotes"]);
                info.Created = DateTime.SpecifyKind(Convert.ToDateTime(ds.Tables[0].Rows[0]["Created"]), DateTimeKind.Utc);
                info.VA048_FullName = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA048_FullName"]);
            };
            return info;
        }
    }

    public class RealtedHistoryInfoDetails
    {
        public int TotalRecords { get; set; }
        public List<RelatedHistoryInfo> RHistory { get; set; }
    }

    public class RelatedHistoryInfo
    {
        public int ID { get; set; }
        public string Record_ID { get; set; }
        public object Created { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string TableName { get; set; }
        public int VAF_TableView_ID { get; set; }
        public string UserName { get; set; }
    }

    public class MailInfo
    {
        public string Title { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string Detail { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public string Comments { get; set; }
        public bool IsMail { get; set; }
        public bool IsLetter { get; set; }
        public List<AttachmentInfos> Attach { get; set; }
        public int VAF_TableView_ID { get; set; }
        public int Record_ID { get; set; }
        public int ID { get; set; }
    }


    public class AttachmentInfos
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int AttID { get; set; }
    }

    public class ChatInfos
    {
        public int ChatID { get; set; }
        public int EntryID { get; set; }
        public int VAF_Image_ID { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string CharacterData { get; set; }
        public DateTime Created { get; set; }
    }

    public class UserInfoForWhereClause
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public StringBuilder whereApp { get; set; }
        public StringBuilder whereMAtt { get; set; }
    }

    public class HistoryRecordInfo
    {
        public int id { get; set; }
        public int record_id { get; set; }
        public DateTime created { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string subject { get; set; }
    }


    // Added by Bharat on 09 June 2017
    public class RecordData
    {
        public string name { get; set; }
        public List<object> value { get; set; }
        public int RowCount { get; set; }
    }
    // call details for history record
    public class CallInfo
    {
        public int VAF_Client_ID { get; set; }
        public int VAF_Org_ID { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public string Export_ID { get; set; }
        public char IsActive { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedBy { get; set; }
        public string VA048_Annotation { get; set; }
        public string VA048_Answered_By { get; set; }
        public string VA048_Api_Version { get; set; }
        public int VA048_CallDetails_ID { get; set; }
        public int VA048_CallProvider_ID { get; set; }
        public string VA048_Caller_Name { get; set; }
        public string VA048_Direction { get; set; }
        public string VA048_Duration { get; set; }
        public DateTime VA048_End_Time { get; set; }
        public string VA048_Forwarded_From { get; set; }
        public string VA048_From { get; set; }
        public string VA048_Group_SID { get; set; }
        public string VA048_Notifications { get; set; }
        public string VA048_Parent_Call_SID { get; set; }
        public string VA048_Phone_Number_SID { get; set; }
        public Decimal VA048_Price { get; set; }
        public string VA048_Price_Unit { get; set; }
        public string VA048_Recordings { get; set; }
        public string VA048_SID { get; set; }
        public DateTime VA048_Start_Time { get; set; }
        public string VA048_Status { get; set; }
        public string VA048_To { get; set; }
        public string VA048_URI { get; set; }
        public int Record_ID { get; set; }
        public int VAF_TableView_ID { get; set; }
        public int VAF_UserContact_ID { get; set; }
        public string VA048_CallNotes { get; set; }
        public char VA048_IsConference { get; set; }
        public string VA048_ConferenceSID { get; set; }
        public string VA048_Participants { get; set; }
        public int VA048_RefRecordID { get; set; }
        public int VA048_RefTable_ID { get; set; }
        public char Processed { get; set; }
        public string VA048_FullName { get; set; }
        public List<AttachmentInfos> Attach { get; set; }
    }
}
