﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Adapters
{
  public class EnvironmentVariableAdapter : IEnvironmentVariableAdapter
  {
    public string GetvEnvironmentVariable(string variableName)
    {
      return Environment.GetEnvironmentVariable(variableName);
    }
  }
}
