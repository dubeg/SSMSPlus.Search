﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSPlus.Core.App
{
    public interface IVersionProvider
    {
        int GetBuild();
        int[] GetBuildAndRevision();
    }
}
