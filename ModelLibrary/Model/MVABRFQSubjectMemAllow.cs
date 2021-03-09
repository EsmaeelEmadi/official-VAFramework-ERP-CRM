﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopicSubscriberOnly
 * Purpose        : Subcriber Topic Only List (positive - i.e. must be a match if exists)
 * Class Used     : X_VAB_RFQ_SubjectMem_Allow
 * Chronological    Development
 * Raghunandan     11-Aug.-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
namespace VAdvantage.Model
{
    public class MVABRFQSubjectMemAllow : X_VAB_RFQ_SubjectMem_Allow
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_RFQ_SubjectMem_Allow_ID"></param>
        /// <param name="trxName"></param>
        public MVABRFQSubjectMemAllow(Ctx ctx, int VAB_RFQ_SubjectMem_Allow_ID, Trx trxName)
            : base(ctx, VAB_RFQ_SubjectMem_Allow_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRFQSubjectMemAllow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // JID_0474: "RFQ Topic window third tab (Restriction) system allow to save blank entry
            //	No Product OR Product Category
            if (GetVAM_ProductCategory_ID() == 0 && GetVAM_Product_ID() == 0)
            {
                log.SaveError("NoProduct/Category", "");
                return false;
            }
            return true;
        }
    }
}