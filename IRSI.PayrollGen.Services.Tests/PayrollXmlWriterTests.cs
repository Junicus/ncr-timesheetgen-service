using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using FluentAssertions.Xml;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IRSI.PayrollGen.Services.Tests
{
  [TestClass]
  public class PayrollXmlWriterTests
  {
    [TestMethod]
    public void Can_Create_PayrollXmlWriter()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollXmlWriter>>();

      var subject = new PayrollXmlWriter(logger);

      subject.Should().NotBeNull();
      subject.Should().BeAssignableTo<IPayrollWriter>();
    }

    [TestMethod]
    public void On_WriteToStream_WritesToStream()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollXmlWriter>>();
      IPayrollWriter subject = new PayrollXmlWriter(logger);
      var stream = new MemoryStream();
      var employees = new List<Employee>();
      var storeId = 1;
      var storeName = "Test";

      subject.WriteToStream(stream, employees, storeId, storeName);

      stream.Length.Should().BeGreaterThan(0);
      stream.Position = 0;
      XDocument doc = XDocument.Load(stream);
      var xroot = doc.Root;
      doc.Should().HaveRoot("Store");
      xroot.Should().HaveAttribute("ID", storeId.ToString());
      xroot.Should().HaveAttribute("StoreName", storeName);
    }

    [TestMethod]
    public void On_WriteToStream_ShouldHaveCorrectData()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollXmlWriter>>();
      IPayrollWriter subject = new PayrollXmlWriter(logger);
      var stream = new MemoryStream();
      var transaction = new Transaction
      {
        Type = TransactionType.ClockInOut,
        PeriodType = PeriodType.Shift,
        EmpId = 1,
        SSN = "123456789",
        PayCode = "",
        JobCode = 1,
        Hours = 0.5m,
        Tips = 10m,
        ClockIn = DateTime.Today.AddHours(12),
        ClockOut = DateTime.Today.AddHours(12).AddMinutes(30)
      };
      var employee = new Employee
      {
        ID = 1,
        SocialSecurity = "123456789",
        FirstName = "Test Name",
        LastName = "Test LastName",
        Transactions = new List<Transaction> { transaction }
      };
      var employees = new List<Employee>() { employee };
      var storeId = 1;
      var storeName = "Test";

      subject.WriteToStream(stream, employees, storeId, storeName);
      stream.Position = 0;

      XDocument doc = XDocument.Load(stream);
      var xroot = doc.Root;
      var xelements = xroot.Elements().ToList();
      var xtransactions = xelements[0].Elements().ToList();
      xelements.Count.Should().Be(1);
      xelements[0].Should().HaveAttribute("ID", employee.ID.ToString());
      xelements[0].Should().HaveAttribute("SocialSecurity", employee.SocialSecurity);
      xelements[0].Should().HaveAttribute("FirstName", employee.FirstName);
      xelements[0].Should().HaveAttribute("LastName", employee.LastName);
      xtransactions.Count.Should().Be(1);
      xtransactions[0].Should().HaveAttribute("PeriodType", transaction.PeriodType.ToString());
      xtransactions[0].Should().HaveAttribute("EmpID", transaction.EmpId.ToString());
      xtransactions[0].Should().HaveAttribute("SSN", transaction.SSN);
      xtransactions[0].Should().HaveAttribute("Type", transaction.Type.ToString());
      xtransactions[0].Should().HaveElement("JobCode");
      xtransactions[0].Should().HaveElement("ClockIn");
      xtransactions[0].Should().HaveElement("ClockOut");
      xtransactions[0].Should().HaveElement("PayCode");
      xtransactions[0].Should().HaveElement("Tips");
      xtransactions[0].Should().HaveElement("Hours");
    }
  }
}
