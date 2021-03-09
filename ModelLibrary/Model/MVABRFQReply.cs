﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQResponse
 * Purpose        : RfQ Response Model
 * Class Used     : X_VAB_RFQReply
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
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
using System.IO;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MVABRFQReply : X_VAB_RFQReply
    {
        //	underlying RfQ				
        private MVABRfQ _rfq = null;
        // Lines						
        private MVABRFQReplyLine[] _lines = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_RFQReply_ID"></param>
        /// <param name="trxName"></param>
        public MVABRFQReply(Ctx ctx, int VAB_RFQReply_ID, Trx trxName)
            : base(ctx, VAB_RFQReply_ID, trxName)
        {
            if (VAB_RFQReply_ID == 0)
            {
                SetIsComplete(false);
                SetIsSelectedWinner(false);
                SetIsSelfService(false);
                SetPrice(Env.ZERO);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRFQReply(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="rfq"></param>
        /// <param name="subscriber"></param>
        public MVABRFQReply(MVABRfQ rfq, MVABRFQSubjectMember subscriber)
            : this(rfq, subscriber,
                subscriber.GetVAB_BusinessPartner_ID(),
                subscriber.GetVAB_BPart_Location_ID(),
                subscriber.GetVAF_UserContact_ID())
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="rfq">rfq</param>
        /// <param name="partner">web response</param>
        public MVABRFQReply(MVABRfQ rfq, MVABBusinessPartner partner)
            : this(rfq, null,
                partner.GetVAB_BusinessPartner_ID(),
                partner.GetPrimaryVAB_BPart_Location_ID(),
                partner.GetPrimaryVAF_UserContact_ID())
        {

        }

        /// <summary>
        /// Parent Constructor.
        /// Automatically saved if lines were created 
        /// Saved automatically 
        /// @param rfq 
        /// </summary>
        /// <param name="rfq">rfq</param>
        /// <param name="subscriber">optional subscriber</param>
        /// <param name="VAB_BusinessPartner_ID">bpartner</param>
        /// <param name="VAB_BPart_Location_ID">bpartner location</param>
        /// <param name="VAF_UserContact_ID">bpartner user</param>
        public MVABRFQReply(MVABRfQ rfq, MVABRFQSubjectMember subscriber,
            int VAB_BusinessPartner_ID, int VAB_BPart_Location_ID, int VAF_UserContact_ID)
            : this(rfq.GetCtx(), 0, rfq.Get_TrxName())
        {

            SetClientOrg(rfq);
            SetVAB_RFQ_ID(rfq.GetVAB_RFQ_ID());
            SetVAB_Currency_ID(rfq.GetVAB_Currency_ID());
            SetName(rfq.GetName());
            _rfq = rfq;
            //	Subscriber info
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            SetVAB_BPart_Location_ID(VAB_BPart_Location_ID);
            SetVAF_UserContact_ID(VAF_UserContact_ID);

            //	Create Lines
            MVABRfQLine[] lines = rfq.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].IsActive())
                    continue;

                //	Product on "Only" list
                if (subscriber != null
                    && !subscriber.IsIncluded(lines[i].GetVAM_Product_ID()))
                {
                    continue;
                }
                //
                if (Get_ID() == 0)	//	save Response
                {
                    Save();
                }

                MVABRFQReplyLine line = new MVABRFQReplyLine(this, lines[i]);
                //	line is not saved (dumped) if there are no Qtys 
            }
        }

        /// <summary>
        /// Get Response Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of Response Lines</returns>
        public MVABRFQReplyLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
            {
                return _lines;
            }
            List<MVABRFQReplyLine> list = new List<MVABRFQReplyLine>();
            String sql = "SELECT * FROM VAB_RFQReplyLine "
                + "WHERE VAB_RFQReply_ID=" + GetVAB_RFQReply_ID() + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABRFQReplyLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "getLines", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
                idr.Close();
            }

            _lines = new MVABRFQReplyLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Response Lines (no requery)
        /// </summary>
        /// <returns>array of Response Lines</returns>
        public MVABRFQReplyLine[] GetLines()
        {
            return GetLines(false);
        }

        /// <summary>
        /// 	Get RfQ
        /// </summary>
        /// <returns>rfq</returns>
        public MVABRfQ GetRfQ()
        {
            if (_rfq == null)
            {
                _rfq = MVABRfQ.Get(GetCtx(), GetVAB_RFQ_ID(), Get_TrxName());
            }
            return _rfq;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQResponse[");
            sb.Append(Get_ID())
                .Append(",Complete=").Append(IsComplete())
                .Append(",Winner=").Append(IsSelectedWinner())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 	Send RfQ, mail subject and body from mail template 
        /// </summary>
        /// <returns>true if RfQ is sent per email.</returns>
        public bool SendRfQ()
        {
            try
            {
                MVAFUserContact to = MVAFUserContact.Get(GetCtx(), GetVAF_UserContact_ID());
                MVAFClient client = MVAFClient.Get(GetCtx());
                MVARMailTemplate mtext = new MVARMailTemplate(GetCtx(), GetRfQ().GetVAR_MailTemplate_ID(), Get_TrxName());

                if (to.Get_ID() == 0 || to.GetEMail() == null || to.GetEMail().Length == 0)
                {
                    log.Log(Level.SEVERE, "No User or no EMail - " + to);
                    return false;
                }

                // Check if mail template is set for RfQ window, if not then get from RfQ Topic window.
                if (mtext.GetVAR_MailTemplate_ID() == 0)
                {
                    MVABRFQSubject mRfQTopic = new MVABRFQSubject(GetCtx(), GetRfQ().GetVAB_RFQ_Subject_ID(), Get_TrxName());
                    if (mRfQTopic.GetVAB_RFQ_Subject_ID() > 0)
                    {
                        mtext = new MVARMailTemplate(GetCtx(), mRfQTopic.GetVAR_MailTemplate_ID(), Get_TrxName());
                    }
                }

                //Replace the email template constants with tables values.
                StringBuilder message = new StringBuilder();
                mtext.SetPO(GetRfQ(), true);
                message.Append(mtext.GetMailText(true).Equals(string.Empty) ? "** No Email Body" : mtext.GetMailText(true));

                String subject = String.IsNullOrEmpty(mtext.GetMailHeader()) ? "** No Subject" : mtext.GetMailHeader(); ;

                EMail email = client.CreateEMail(to.GetEMail(), to.GetName(), subject, message.ToString());
                if (email == null)
                {
                    return false;
                }
                email.AddAttachment(CreatePDF());
                if (EMail.SENT_OK.Equals(email.Send()))
                {
                    //SetDateInvited(new Timestamp(System.currentTimeMillis()));
                    SetDateInvited(DateTime.Now);
                    Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                //MessageBox.Show("error--" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            return CreatePDF(null);
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //ReportEngine re = ReportEngine.get(getCtx(), ReportEngine.RFQ, getVAB_RFQReply_ID());
            //if (re == null)
            //   return null;
            //return re.getPDF(file);
            return file;
        }

        /// <summary>
        /// Check if Response is Complete
        /// </summary>
        /// <returns>null if complere - error message otherwise</returns>
        public String CheckComplete()
        {
            if (IsComplete())
            {
                SetIsComplete(false);
            }
            MVABRfQ rfq = GetRfQ();

            //	Is RfQ Total valid
            String error = rfq.CheckQuoteTotalAmtOnly();
            if (error != null && error.Length > 0)
            {
                return error;
            }

            //	Do we have Total Amount ?
            if (rfq.IsQuoteTotalAmt() || rfq.IsQuoteTotalAmtOnly())
            {
                Decimal amt = GetPrice();
                if (Env.ZERO.CompareTo(amt) >= 0)
                {
                    return "No Total Amount";
                }
            }

            //	Do we have an amount/qty for all lines
            if (rfq.IsQuoteAllLines())
            {
                MVABRFQReplyLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MVABRFQReplyLine line = lines[i];
                    if (!line.IsActive())
                        return "Line " + line.GetRfQLine().GetLine()
                            + ": Not Active";
                    bool validAmt = false;
                    MVABRFQReplyLineQty[] qtys = line.GetQtys(false);
                    for (int j = 0; j < qtys.Length; j++)
                    {
                        MVABRFQReplyLineQty qty = qtys[j];
                        if (!qty.IsActive())
                        {
                            continue;
                        }
                        Decimal? amt = qty.GetNetAmt();
                        if (Env.ZERO.CompareTo(amt) < 0)
                        {
                            validAmt = true;
                            break;
                        }
                    }
                    if (!validAmt)
                    {
                        return "Line " + line.GetRfQLine().GetLine()
                            + ": No Amount";
                    }
                }
            }

            //	Do we have an amount for all line qtys
            if (rfq.IsQuoteAllQty())
            {
                MVABRFQReplyLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MVABRFQReplyLine line = lines[i];
                    MVABRFQReplyLineQty[] qtys = line.GetQtys(false);
                    for (int j = 0; j < qtys.Length; j++)
                    {
                        MVABRFQReplyLineQty qty = qtys[j];
                        if (!qty.IsActive())
                            return "Line " + line.GetRfQLine().GetLine()
                            + " Qty=" + qty.GetRfQLineQty().GetQty()
                            + ": Not Active";
                        Decimal? amt = qty.GetNetAmt();
                        if (amt == null || Env.ZERO.CompareTo(amt) >= 0)
                        {
                            return "Line " + line.GetRfQLine().GetLine()
                                 + " Qty=" + qty.GetRfQLineQty().GetQty()
                                 + ": No Amount";
                        }
                    }
                }
            }

            SetIsComplete(true);
            return null;
        }

        /// <summary>
        /// Is Quote Total Amt Only
        /// </summary>
        /// <returns>true if only Total</returns>
        public bool IsQuoteTotalAmtOnly()
        {
            return GetRfQ().IsQuoteTotalAmtOnly();
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Calculate Complete Date (also used to verify)
            if (GetDateWorkStart() != null && GetDeliveryDays() != 0)
            {
                SetDateWorkComplete(TimeUtil.AddDays(GetDateWorkStart(), GetDeliveryDays()));
            }
            //	Calculate Delivery Days
            else if (GetDateWorkStart() != null && GetDeliveryDays() == 0 && GetDateWorkComplete() != null)
            {
                SetDeliveryDays(TimeUtil.GetDaysBetween(GetDateWorkStart(), GetDateWorkComplete()));
            }
            //	Calculate Start Date
            else if (GetDateWorkStart() == null && GetDeliveryDays() != 0 && GetDateWorkComplete() != null)
            {
                SetDateWorkStart(TimeUtil.AddDays(GetDateWorkComplete(), GetDeliveryDays() * -1));
            }
            return true;
        }
    }
}