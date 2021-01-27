﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PeriodControlStatus
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     25-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.ProcessEngine;
using CoreLibrary.Classes;

namespace VAdvantage.Process
{
    public class PeriodControlStatus : ProcessEngine.SvrProcess
    {
        // Period Control				
        private int _VAB_YearPeriodControl_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {

                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAB_YearPeriodControl_ID = GetRecord_ID();
        }

        /// <summary>
        ///	Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            //log.info("VAB_YearPeriodControl_ID=" + _VAB_YearPeriodControl_ID);
            MPeriodControl pc = new MPeriodControl(GetCtx(), _VAB_YearPeriodControl_ID, Get_TrxName());
            if (pc.Get_ID() == 0)
            {
                throw new Exception("@NotFound@  @VAB_YearPeriodControl_ID@=" + _VAB_YearPeriodControl_ID);
            }
            //	Permanently closed
            if (MPeriodControl.PERIODACTION_PermanentlyClosePeriod.Equals(pc.GetPeriodStatus()))
            {
                throw new Exception("@PeriodStatus@ = " + pc.GetPeriodStatus());
            }
            //	No Action
            if (MPeriodControl.PERIODACTION_NoAction.Equals(pc.GetPeriodAction()))
            {
                return "@OK@";
            }
            //	Open
            if (MPeriodControl.PERIODACTION_OpenPeriod.Equals(pc.GetPeriodAction()))
            {
                pc.SetPeriodStatus(MPeriodControl.PERIODSTATUS_Open);
            }
            //	Close
            if (MPeriodControl.PERIODACTION_ClosePeriod.Equals(pc.GetPeriodAction()))
            {
                pc.SetPeriodStatus(MPeriodControl.PERIODSTATUS_Closed);
            }
            //	Close Permanently
            if (MPeriodControl.PERIODACTION_PermanentlyClosePeriod.Equals(pc.GetPeriodAction()))
            {
                pc.SetPeriodStatus(MPeriodControl.PERIODSTATUS_PermanentlyClosed);
            }
            pc.SetPeriodAction(MPeriodControl.PERIODACTION_NoAction);
            Boolean ok = pc.Save();
            //	Reset Cache
            CacheMgt.Get().Reset("VAB_YearPeriodControl", 0);
            CacheMgt.Get().Reset("VAB_YearPeriod", pc.GetVAB_YearPeriod_ID());
            if (!ok)
                return "@Error@";
            return "@OK@";
        }
    }
}
