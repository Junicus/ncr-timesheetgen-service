using System;
using System.Collections.Generic;
using System.Linq;
using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using NLog;

namespace IRSI.PayrollGen.Services
{
  public class PayrollConverter : IPayrollConverter
  {
    private readonly ILogger _logger;
    //private readonly ILoggerAdapter<PayrollConverter> _logger;
    private readonly IPayrollConverterUtils _payrollConverterUtils;
    private readonly IEmployeeRepositoryService _employeeRepositoryService;

    public PayrollConverter(
      ILogger logger,
      IPayrollConverterUtils payrollConverterUtils,
      IEmployeeRepositoryService employeeRepositoryService
      )
    {
      _logger = logger;
      _payrollConverterUtils = payrollConverterUtils;
      _employeeRepositoryService = employeeRepositoryService;
    }

    public List<Employee> ConvertPayroll(AlohaDataset alohaData, TipCalculation tipStrategy)
    {
      var employees = _payrollConverterUtils.GetEmployees(alohaData);
      var ignoreJobList = new List<int> { 11, 33, 90, 91 };
      var shifts = _payrollConverterUtils.GetShifts(alohaData, ignoreJobList);
      var tips = _payrollConverterUtils.GetTips(alohaData, ignoreJobList, tipStrategy);
      var breaks = _payrollConverterUtils.GetBreaks(alohaData, ignoreJobList);

      _employeeRepositoryService.LoadEmployees(employees);
      _employeeRepositoryService.LoadTransactions(shifts);
      _employeeRepositoryService.LoadTransactions(tips);
      _employeeRepositoryService.LoadTransactions(breaks);

      _employeeRepositoryService.AdjustPaycodesForMultipleShifts();

      return _employeeRepositoryService.Employees.Values.ToList<Employee>();
    }
  }
}