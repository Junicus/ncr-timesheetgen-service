using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using System.Collections.Generic;

namespace IRSI.PayrollGen.Services
{
  public interface IPayrollConverter
  {
    List<Employee> ConvertPayroll(AlohaDataset alohaData, TipCalculation tipStrategy);
  }
}