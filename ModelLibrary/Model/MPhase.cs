﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Logging;

using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MPhase : X_VAB_Std_Stage 
    {
        public MPhase(Ctx ctx, int VAB_Std_Stage_ID, Trx trxName)
            : base(ctx, VAB_Std_Stage_ID, trxName)
        {
            
        }

       public MPhase(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}
