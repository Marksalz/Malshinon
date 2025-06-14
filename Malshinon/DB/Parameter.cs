﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon
{
    /// <summary>
    /// Represents a key-value pair parameter used for configuration or data transfer.
    /// </summary>
    public class Parameter
    {
        public string parameter { get; private set; }
        public object parameterValue { get; private set; }
        public Parameter(string parameter, object parameterValue)
        {
            this.parameter = parameter;
            this.parameterValue = parameterValue;
        }
    }
}
