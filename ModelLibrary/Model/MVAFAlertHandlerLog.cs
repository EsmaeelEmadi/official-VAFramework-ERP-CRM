﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFAlertHandlerLog : X_VAF_AlertHandlerLog, ViennaProcessorLog
    {
        public MVAFAlertHandlerLog(Ctx ctx, int VAF_AlertHandlerLog_ID, Trx trx)
            : base(ctx, VAF_AlertHandlerLog_ID, trx)
        {
            
        }	//	MAlertProcessorLog


        public MVAFAlertHandlerLog(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
            
        }	//	MAlertProcessorLog

        public MVAFAlertHandlerLog(MVAFAlertHandler parent, String summary)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_AlertHandler_ID(parent.GetVAF_AlertHandler_ID());
            SetSummary(summary);
        }	//	MAlertProcessorLog

    }
}