﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSPlus.Core.Utils.Validation
{
    public interface IValidatable<T>
    {
        T Validate();
    }
}
