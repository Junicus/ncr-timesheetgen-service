using System.Collections.Generic;
using System.Linq;
using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;

namespace IRSI.PayrollGen.Services
{
  public class PayrollConverterUtils : IPayrollConverterUtils
  {
    private readonly ILoggerAdapter<PayrollConverterUtils> _logger;

    public PayrollConverterUtils(ILoggerAdapter<PayrollConverterUtils> logger)
    {
      _logger = logger;
    }

    public Employee ConverEmployeeRow(AlohaDataset.empRow empRow)
    {
      return new Employee
      {
        ID = empRow.id,
        SocialSecurity = empRow.ssn,
        FirstName = empRow.firstname,
        LastName = empRow.lastname
      };
    }

    public Transaction ConvertShiftRow(AlohaDataset.adjtimeRow adjtimeRow)
    {
      return new Transaction
      {
        Type = TransactionType.ClockInOut,
        PeriodType = PeriodType.Shift,
        EmpId = adjtimeRow.employee,
        SSN = adjtimeRow.ssn,
        JobCode = adjtimeRow.jobcode,
        Hours = (adjtimeRow.IshoursNull() ? 0 : adjtimeRow.hours),
        PayCode = string.Empty,
        ClockIn = adjtimeRow.sysdatein.AddHours(adjtimeRow.inhour).AddMinutes(adjtimeRow.inminute),
        ClockOut = adjtimeRow.sysdateout.AddHours(adjtimeRow.outhour).AddMinutes(adjtimeRow.outminute)
      };
    }

    public Transaction ConvertTipRow(AlohaDataset.adjtimeRow adjtimeRow, TipCalculation tipCalculationStrategy)
    {
      return new Transaction
      {
        Type = TransactionType.PayCode,
        PeriodType = PeriodType.Unknown,
        EmpId = adjtimeRow.employee,
        SSN = adjtimeRow.ssn,
        JobCode = adjtimeRow.jobcode,
        Hours = 0m,
        PayCode = "T",
        ClockIn = adjtimeRow.date.AddHours(12),
        Tips = (tipCalculationStrategy == TipCalculation.Auto) ?
        (adjtimeRow.dectips > adjtimeRow.cctips) ? adjtimeRow.dectips : adjtimeRow.cctips :
        (tipCalculationStrategy == TipCalculation.CCTips) ? adjtimeRow.cctips : adjtimeRow.dectips
      };
    }

    public Transaction ConvertBreakRow(AlohaDataset.gndbreakRow adjtimeRow)
    {
      return new Transaction
      {
        Type = TransactionType.ClockInOut,
        PeriodType = PeriodType.Break,
        EmpId = adjtimeRow.employee,
        SSN = adjtimeRow.ssn,
        JobCode = adjtimeRow.jobcode,
        Hours = (adjtimeRow.IshoursNull() ? 0 : adjtimeRow.hours),
        PayCode = string.Empty,
        ClockIn = adjtimeRow.sysdatebeg.AddHours(adjtimeRow.inhour).AddMinutes(adjtimeRow.inminute),
        ClockOut = adjtimeRow.sysdateend.AddHours(adjtimeRow.outhour).AddMinutes(adjtimeRow.outminute)
      };
    }

    public List<Employee> GetEmployees(AlohaDataset data)
    {
      _logger.LogDebug("GetEmployees called");
      var employees = new List<Employee>();

      foreach (var empRow in data.emp)
      {
        employees.Add(ConverEmployeeRow(empRow));
      }

      return employees;
    }

    public List<Transaction> GetShifts(AlohaDataset data, List<int> ignoreJobCodeList)
    {
      _logger.LogDebug("GetShifts called");
      var transactions = new List<Transaction>();

      foreach (var adjtimeRow in data.adjtime)
      {
        if (ignoreJobCodeList.Contains(adjtimeRow.jobcode)) continue;
        transactions.Add(ConvertShiftRow(adjtimeRow));
      }

      return transactions;
    }

    public List<Transaction> GetTips(AlohaDataset data, List<int> ignoreJobCodeList, TipCalculation tipCalculationStrategy)
    {
      _logger.LogDebug("GetTips called");
      var transactions = new List<Transaction>();

      foreach (var adjtimeRow in data.adjtime.Where(t => t.cctips > 0m || t.dectips > 0m))
      {
        if (ignoreJobCodeList.Contains(adjtimeRow.jobcode)) continue;
        transactions.Add(ConvertTipRow(adjtimeRow, tipCalculationStrategy));
      }

      return transactions;
    }

    public List<Transaction> GetBreaks(AlohaDataset data, List<int> ignoreJobCodeList)
    {
      _logger.LogDebug("GetBreaks called");
      var transactions = new List<Transaction>();

      foreach (var gndbreakRow in data.gndbreak)
      {
        if (ignoreJobCodeList.Contains(gndbreakRow.jobcode)) continue;
        transactions.Add(ConvertBreakRow(gndbreakRow));
      }

      return transactions;
    }
  }
}