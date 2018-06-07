using System.Collections.Generic;
using System.IO;
using IRSI.PayrollGen.Models;

namespace IRSI.PayrollGen.Services
{
  public interface IPayrollWriter
  {
    void WriteToStream(Stream stream, List<Employee> employees, int storeId, string storeName);
  }
}