﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUtil
{
    /// <summary>
    /// 标识该属性是主健
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class DBKeyAttribute : Attribute
    {
    }
}
