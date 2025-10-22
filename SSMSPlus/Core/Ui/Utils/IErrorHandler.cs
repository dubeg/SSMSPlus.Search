﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSPlus.Core.Ui.Utils
{
    public delegate void ErrorHandler(Exception ex);

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
