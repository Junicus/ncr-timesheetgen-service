﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Adapters
{
  public interface IPayrollFileAdapter
  {
    void CreateMarker(string path);
  }
}
