using System;
using FluentAssertions;
using IRSI.PayrollGen.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IRSI.PayrollGen.Services.Tests
{
  [TestClass]
  public class PayrollReaderTests
  {
    [TestMethod]
    public void Can_Create_Payroll_Reader()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollReader>>();

      var subject = new PayrollReader(logger);

      subject.Should().NotBeNull();
      subject.Should().BeAssignableTo<IPayrollReader>();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void On_ReadPayroll_InvalidFolder_Should_ReturnNull()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollReader>>();
      var folder = @"C:\POS\Aloha\20180602";
      IPayrollReader subject = new PayrollReader(logger);

      var result = subject.ReadPayroll(folder);

      result.Should().BeNull();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void On_ReadPayroll_InvalidFolder_Should_Warn()
    {
      var logger = new Mock<ILoggerAdapter<PayrollReader>>();
      var folder = @"C:\POS\Aloha\20180602";
      IPayrollReader subject = new PayrollReader(logger.Object);

      var result = subject.ReadPayroll(folder);

      logger.Verify(c => c.LogWarning(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [ExpectedException(typeof(ArgumentException))]
    public void On_ReadPayroll_EmptyFolderString_Should_Throw()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollReader>>();
      var folder = "";
      IPayrollReader subject = new PayrollReader(logger);

      var result = subject.ReadPayroll(folder);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void On_ReadPayroll_ValidFolder_Should_ReturnData()
    {
      var logger = new Mock<ILoggerAdapter<PayrollReader>>();
      var folder = @"C:\POS\Aloha\20180601";
      IPayrollReader subject = new PayrollReader(logger.Object);

      var result = subject.ReadPayroll(folder);

      result.Should().NotBeNull();
      result.emp.Count.Should().BeGreaterThan(0);
      result.adjtime.Count.Should().BeGreaterThan(0);
      result.gndbreak.Count.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void On_GetConnectionString_Returns_Correctly_Formatted_String()
    {
      var logger = Mock.Of<ILoggerAdapter<PayrollReader>>();
      var folder = @"C:\POS\Aloha\20180602";
      var expected = $"Provider=VFPOLEDB.1;Data Source={folder}";
      IPayrollReader subject = new PayrollReader(logger);

      var result = subject.GetConnectionString(folder);

      result.Should().Be(expected);
    }
  }
}
