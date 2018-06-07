using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IRSI.PayrollGen.Services.Tests
{
  [TestClass]
  public class PayrollConverterUtilsTests
  {
    [TestMethod]
    public void Can_Create_PayrollConverterUtils()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();

      var subject = new PayrollConverterUtils(logger);

      subject.Should().NotBeNull();
      subject.Should().BeAssignableTo<IPayrollConverterUtils>();
    }

    [TestMethod]
    public void On_ConvertEmployeeRow_GetRightEmployee()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee", 123456789);
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);

      var result = subject.ConverEmployeeRow(empRow);

      result.ID.Should().Be(empRow.id);
      result.SocialSecurity.Should().Be(empRow.ssn);
      result.FirstName.Should().Be(empRow.firstname);
      result.LastName.Should().Be(empRow.lastname);
      result.Transactions.Should().BeEmpty();
    }

    [TestMethod]
    public void On_ConvertShiftRow_GetRightShift()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);

      var result = subject.ConvertShiftRow(adjtimeRow);

      result.Type.Should().Be(TransactionType.ClockInOut);
      result.PeriodType.Should().Be(PeriodType.Shift);
      result.PayCode.Should().Be(string.Empty);
      result.EmpId.Should().Be(adjtimeRow.employee);
      result.SSN.Should().Be(adjtimeRow.ssn);
      result.JobCode.Should().Be(adjtimeRow.jobcode);
      result.ClockIn.Should().Be(DateTime.Today.AddHours(12));
      result.ClockOut.Should().Be(DateTime.Today.AddHours(12).AddMinutes(30));
      result.Hours.Should().BeApproximately(0.5m, 0.001m);
    }

    [TestMethod]
    public void On_ConvertTipRow_GetRightTip_Using_Strategy_Auto()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 1m, 2m);
      var adjtimeRow2 = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 2m, 1m);
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var tipstrategy = TipCalculation.Auto;

      var result = subject.ConvertTipRow(adjtimeRow, tipstrategy);
      var result2 = subject.ConvertTipRow(adjtimeRow2, tipstrategy);

      result.Type.Should().Be(TransactionType.PayCode);
      result.PeriodType.Should().Be(PeriodType.Unknown);
      result.PayCode.Should().Be("T");
      result.EmpId.Should().Be(adjtimeRow.employee);
      result.SSN.Should().Be(adjtimeRow.ssn);
      result.JobCode.Should().Be(adjtimeRow.jobcode);
      result.ClockIn.Should().Be(DateTime.Today.AddHours(12));
      result.ClockOut.Should().Be(DateTime.MinValue);
      result.Hours.Should().BeApproximately(0m, 0.001m);
      result.Tips.Should().BeApproximately(2m, 0.001m);
      result2.Tips.Should().BeApproximately(2m, 0.001m);
    }

    [TestMethod]
    public void On_ConvertBreakRow_GetRightShift()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      var data = new AlohaDataset();
      var gndbroeakRow = AlohaDataUtils.CreateBreakRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m);
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);

      var result = subject.ConvertBreakRow(gndbroeakRow);

      result.Type.Should().Be(TransactionType.ClockInOut);
      result.PeriodType.Should().Be(PeriodType.Break);
      result.PayCode.Should().Be(string.Empty);
      result.EmpId.Should().Be(gndbroeakRow.employee);
      result.SSN.Should().Be(gndbroeakRow.ssn);
      result.JobCode.Should().Be(gndbroeakRow.jobcode);
      result.ClockIn.Should().Be(DateTime.Today.AddHours(12));
      result.ClockOut.Should().Be(DateTime.Today.AddHours(12).AddMinutes(30));
      result.Hours.Should().BeApproximately(0.5m, 0.001m);
    }


    [TestMethod]
    public void On_GetEmployees_Gets_EmployeeEnumerable()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee", 123456789);
      data.emp.AddempRow(empRow);

      List<Employee> result = subject.GetEmployees(data);

      result.Should().NotBeNull();
      result.Should().NotBeEmpty();
      result.Should().Contain(c => c.ID == 1);
    }

    [TestMethod]
    public void On_GetEmployees_Gets_All_Employees()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var empRow = AlohaDataUtils.CreateEmpRow(data, 1, "123456789", "Test", "Employee1", 123456789);
      var empRow2 = AlohaDataUtils.CreateEmpRow(data, 2, "987654321", "Test", "Employee2", 987654321);
      data.emp.AddempRow(empRow);
      data.emp.AddempRow(empRow2);

      var result = subject.GetEmployees(data);

      result.Count().Should().Be(data.emp.Count());
      result.Should().Contain(e => e.ID == 1);
      result.Should().Contain(e => e.ID == 2);
    }

    [TestMethod]
    public void On_GetShifts_Get_TransactionEnumerable()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      var ignoreList = new List<int> { 10 };

      List<Transaction> result = subject.GetShifts(data, ignoreList);

      result.Should().NotBeNull();
      result.Should().NotBeEmpty();
      result.Should().Contain(c => c.EmpId == 1);
    }

    [TestMethod]
    public void On_GetShifts_Gets_All_Transactions()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      var adjtimeRow2 = AlohaDataUtils.CreateAdjTimeRow(data, 2, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 1, 0, 1, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      data.adjtime.AddadjtimeRow(adjtimeRow2);
      var ignoreList = new List<int> { 10 };

      var result = subject.GetShifts(data, ignoreList);

      result.Count().Should().Be(data.adjtime.Count());
      result.Should().Contain(t => t.EmpId == 1);
      result.Should().Contain(t => t.EmpId == 2);
    }

    [TestMethod]
    public void On_GetShifts_Ingores_From_IgnoreList()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      var ignoreList = new List<int> { 1 };

      List<Transaction> result = subject.GetShifts(data, ignoreList);

      result.Should().BeEmpty();
    }

    [TestMethod]
    public void On_GetTips_Get_TransactionEnumerable()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      var ignoreList = new List<int> { 10 };
      var tipstrategy = TipCalculation.Auto;

      List<Transaction> result = subject.GetTips(data, ignoreList, tipstrategy);

      result.Should().NotBeNull();
      result.Should().NotBeEmpty();
      result.Should().Contain(c => c.EmpId == 1);
    }

    [TestMethod]
    public void On_GetTips_Gets_All_Transactions()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      var adjtimeRow2 = AlohaDataUtils.CreateAdjTimeRow(data, 2, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 1, 0, 1, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      data.adjtime.AddadjtimeRow(adjtimeRow2);
      var ignoreList = new List<int> { 10 };
      var tipstrategy = TipCalculation.Auto;

      var result = subject.GetTips(data, ignoreList, tipstrategy);

      result.Count().Should().Be(data.adjtime.Count());
      result.Should().Contain(t => t.EmpId == 1);
      result.Should().Contain(t => t.EmpId == 2);
    }

    [TestMethod]
    public void On_GetTips_Ingores_From_IgnoreList()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      var ignoreList = new List<int> { 1 };
      var tipstrategy = TipCalculation.Auto;

      List<Transaction> result = subject.GetTips(data, ignoreList, tipstrategy);

      result.Should().BeEmpty();
    }

    [TestMethod]
    public void On_GetBreaks_Get_TransactionEnumerable()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var gndbreakRow = AlohaDataUtils.CreateBreakRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m);
      data.gndbreak.AddgndbreakRow(gndbreakRow);
      var ignoreList = new List<int> { 10 };

      List<Transaction> result = subject.GetBreaks(data, ignoreList);

      result.Should().NotBeNull();
      result.Should().NotBeEmpty();
      result.Should().Contain(c => c.EmpId == 1);
    }

    [TestMethod]
    public void On_GetBreaks_Gets_All_Transactions()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var gndbreakRow = AlohaDataUtils.CreateBreakRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m);
      var gndbreakRow2 = AlohaDataUtils.CreateBreakRow(data, 2, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 1, 0, 1, 30, 0.5m);
      data.gndbreak.AddgndbreakRow(gndbreakRow);
      data.gndbreak.AddgndbreakRow(gndbreakRow2);
      var ignoreList = new List<int> { 10 };

      var result = subject.GetBreaks(data, ignoreList);

      result.Count().Should().Be(data.gndbreak.Count());
      result.Should().Contain(t => t.EmpId == 1);
      result.Should().Contain(t => t.EmpId == 2);
    }

    [TestMethod]
    public void On_GetBreaks_Ingores_From_IgnoreList()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollConverterUtils>>();
      IPayrollConverterUtils subject = new PayrollConverterUtils(logger);
      var data = new AlohaDataset();
      var adjtimeRow = AlohaDataUtils.CreateAdjTimeRow(data, 1, "", DateTime.Today, DateTime.Today, DateTime.Today, 1, 12, 0, 12, 30, 0.5m, 10m, 10m);
      data.adjtime.AddadjtimeRow(adjtimeRow);
      var ignoreList = new List<int> { 1 };

      List<Transaction> result = subject.GetBreaks(data, ignoreList);

      result.Should().BeEmpty();
    }
  }
}

