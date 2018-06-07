using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using System.Collections.Generic;

namespace IRSI.PayrollGen.Services
{
  public interface IPayrollConverterUtils
  {
    List<Employee> GetEmployees(AlohaDataset data);
    List<Transaction> GetShifts(AlohaDataset data, List<int> ignoreJobCodeList);
    List<Transaction> GetTips(AlohaDataset data, List<int> ignoreJobCodeList, TipCalculation tipCalculationStrategy);
    List<Transaction> GetBreaks(AlohaDataset data, List<int> ignoreJobCodeList);

    Employee ConverEmployeeRow(AlohaDataset.empRow emprow);
    Transaction ConvertShiftRow(AlohaDataset.adjtimeRow adjtimeRow);
    Transaction ConvertTipRow(AlohaDataset.adjtimeRow adjtimeRow, TipCalculation tipCalculationStrategy);
    Transaction ConvertBreakRow(AlohaDataset.gndbreakRow adjtimeRow);
  }
}