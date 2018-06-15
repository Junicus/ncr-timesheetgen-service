using IRSI.PayrollGen.Services.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateMarker
{
  class Program
  {
    static void Main(string[] args)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      var datedFolders = Directory.GetDirectories(iberdir, "20??????");
      IPayrollFileAdapter fileAdapter = new PayrollFileAdapter();
      foreach (var datedFolder in datedFolders)
      {
        if (!File.Exists(datedFolder))
        {
          fileAdapter.CreateMarker(datedFolder);
        }
      }
    }
  }
}
