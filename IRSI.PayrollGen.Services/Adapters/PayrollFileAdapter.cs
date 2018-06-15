using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Adapters
{
  public class PayrollFileAdapter : IPayrollFileAdapter
  {
    public void CreateMarker(string path)
    {
      var markerFilePath = Path.Combine(path, "IRSIPAYGEN");
      using (var filestream = File.Create(path)) { };
    }
  }
}
