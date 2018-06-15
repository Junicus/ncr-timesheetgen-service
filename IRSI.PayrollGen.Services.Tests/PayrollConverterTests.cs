using System;
using System.Linq;
using FluentAssertions;
using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;

namespace IRSI.PayrollGen.Services.Tests
{
  [TestClass]
  public class PayrollConverterTests
  {
    private ILogger logger = Mock.Of<ILogger>();
    [TestMethod]
    public void Can_Create_Payroll_Converter()
    {
      var payrollConverterUtils = Mock.Of<IPayrollConverterUtils>();
      var employeeRepository = Mock.Of<IEmployeeRepositoryService>();

      var subject = new PayrollConverter(logger, payrollConverterUtils, employeeRepository);

      subject.Should().NotBeNull();
      subject.Should().BeAssignableTo<IPayrollConverter>();
    }

    [TestMethod]
    public void On_Convert_Payroll_EmptyData_Returns_EmptyList()
    {
      IPayrollConverterUtils payrollConverterUtils = new PayrollConverterUtils(logger);
      var employeeRepository = new EmployeeRepositoryService(logger);
      IPayrollConverter subject = new PayrollConverter(logger, payrollConverterUtils, employeeRepository);
      var data = new AlohaDataset();
      var tipStrategy = TipCalculation.Auto;

      var result = subject.ConvertPayroll(data, tipStrategy);

      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [TestMethod]
    public void On_Convert_Payroll_OnlyEmployees_ReturnsListWith_EmptyTransactions()
    {
      IPayrollConverterUtils payrollConverterUtils = new PayrollConverterUtils(logger);
      var employeeRepository = new EmployeeRepositoryService(logger);
      IPayrollConverter subject = new PayrollConverter(logger, payrollConverterUtils, employeeRepository);
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee", 123456789);
      data.emp.AddempRow(empRow);
      var tipStrategy = TipCalculation.Auto;

      var result = subject.ConvertPayroll(data, tipStrategy);

      result.Should().NotBeNull();
      result.Count.Should().Be(1);
      result[0].Transactions.Count.Should().Be(0);
    }

    [TestMethod]
    public void On_Convert_Payroll_EmpFullData_ReturnsListWith_Transactions()
    {
      IPayrollConverterUtils payrollConverterUtils = new PayrollConverterUtils(logger);
      var employeeRepository = new EmployeeRepositoryService(logger);
      IPayrollConverter subject = new PayrollConverter(logger, payrollConverterUtils, employeeRepository);
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee", 123456789);
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 14, 30, 0.5m, 10m, 10m);
      var gndbroeakRow = AlohaDataUtils.CreateBreakRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 30, 13, 00, 0.5m);
      data.emp.AddempRow(empRow);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      data.gndbreak.AddgndbreakRow(gndbroeakRow);
      var tipStrategy = TipCalculation.Auto;

      var result = subject.ConvertPayroll(data, tipStrategy);

      result[0].Transactions.Count.Should().Be(3);
      result[0].Transactions.Should().Contain(t => t.Type == TransactionType.ClockInOut && t.PeriodType == PeriodType.Shift);
      result[0].Transactions.Should().Contain(t => t.Type == TransactionType.ClockInOut && t.PeriodType == PeriodType.Break);
      result[0].Transactions.Should().ContainSingle(t => t.Type == TransactionType.PayCode);
    }

    [TestMethod]
    public void On_Convert_Payroll_MultipleShift_FixesTips()
    {
      IPayrollConverterUtils payrollConverterUtils = new PayrollConverterUtils(logger);
      var employeeRepository = new EmployeeRepositoryService(logger);
      IPayrollConverter subject = new PayrollConverter(logger, payrollConverterUtils, employeeRepository);
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee", 123456789);
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 14, 30, 0.5m, 10m, 10m);
      var adjtimeRow2 = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 15, 0, 16, 30, 0.5m, 10m, 10m);
      var gndbroeakRow = AlohaDataUtils.CreateBreakRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 30, 13, 00, 0.5m);
      data.emp.AddempRow(empRow);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      data.adjtime.AddadjtimeRow(adjtimeRow2);
      data.gndbreak.AddgndbreakRow(gndbroeakRow);
      var tipStrategy = TipCalculation.Auto;

      var result = subject.ConvertPayroll(data, tipStrategy);

      var resultTips = result[0].Transactions.Where(t => t.Type == TransactionType.PayCode).ToList();

      resultTips.Count.Should().Be(2);
      resultTips[0].ClockIn.Hour.Should().Be(12);
      resultTips[0].ClockIn.Minute.Should().Be(0);
      resultTips[1].ClockIn.Hour.Should().Be(12);
      resultTips[1].ClockIn.Minute.Should().Be(5);
    }
  }
}
