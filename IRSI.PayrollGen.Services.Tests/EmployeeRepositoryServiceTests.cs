using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IRSI.PayrollGen.Services.Tests
{
  [TestClass]
  public class EmployeeRepositoryServiceTests
  {
    [TestMethod]
    public void Can_Create_Employee_Repository_Service()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      var subject = new EmployeeRepositoryService(logger);
      subject.Should().NotBeNull();
      subject.Should().BeAssignableTo<IEmployeeRepositoryService>();
      subject.Employees.Should().NotBeNull();
      subject.Employees.Count.Should().Be(0);
    }

    [TestMethod]
    public void On_LoadEmployees_Employee_Are_Same()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var id2 = 2;
      var employee1 = new Employee { ID = id1 };
      var employee2 = new Employee { ID = id2 };
      var employees = new List<Employee> { employee1, employee2 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);

      subject.LoadEmployees(employees);

      subject.Employees.Count.Should().Be(employees.Count);
      subject.Employees.Keys.Should().Contain(id1);
      subject.Employees.Keys.Should().Contain(id2);
      subject.Employees[id1].Should().Be(employee1);
      subject.Employees[id2].Should().Be(employee2);
    }

    [TestMethod]
    public void On_LoadEmployees_Adding_Duplicates_Should_Add_First()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var employee1 = new Employee { ID = id1, SocialSecurity = "1" };
      var employee2 = new Employee { ID = id1, SocialSecurity = "2" };
      var employees = new List<Employee> { employee1, employee2 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);

      subject.LoadEmployees(employees);
      subject.Employees.Count.Should().Be(1);
      subject.Employees[id1].Should().Be(employee1);
    }

    [TestMethod]
    public void On_LoadEmployees_Adding_Suplicates_Should_LogWarning()
    {
      var logger = new Mock<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var employee1 = new Employee { ID = id1 };
      var employee2 = new Employee { ID = id1 };
      var employees = new List<Employee> { employee1, employee2 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger.Object);

      subject.LoadEmployees(employees);

      logger.Verify(c => c.LogWarning(It.IsAny<string>()), Times.Once());
    }

    [TestMethod]
    public void On_LoadTransactions_Single_Employee_Should_Have_Transactions()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var employee1 = new Employee { ID = id1 };
      var employees = new List<Employee> { employee1 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);
      subject.LoadEmployees(employees);
      var transaction = new Transaction { EmpId = id1, SSN = "" };
      var transactions = new List<Transaction> { transaction };

      var transactionsBeforeAction = subject.Employees[id1].Transactions.Count;
      subject.LoadTransactions(transactions);
      var transactionsAfterAction = subject.Employees[id1].Transactions.Count;

      transactionsBeforeAction.Should().Be(transactionsAfterAction - 1);
      subject.Employees[id1].Transactions[0].Should().Be(transaction);
    }

    [TestMethod]
    public void On_LoadTransactions_Single_Employee_Transaction_Should_Have_SSN()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var ssn1 = "1";
      var employee1 = new Employee { ID = id1, SocialSecurity = ssn1 };
      var employees = new List<Employee> { employee1 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);
      subject.LoadEmployees(employees);
      var transaction = new Transaction { EmpId = id1, SSN = "" };
      var transactions = new List<Transaction> { transaction };

      subject.LoadTransactions(transactions);

      subject.Employees[id1].Transactions[0].SSN.Should().Be(ssn1);
    }

    [TestMethod]
    public void On_LoadTransactions_Employee_NotFound_Should_Warn()
    {
      var logger = new Mock<ILoggerAdapter<EmployeeRepositoryService>>();
      var id1 = 1;
      var ssn1 = "1";
      var employee1 = new Employee { ID = id1, SocialSecurity = ssn1 };
      var employees = new List<Employee> { employee1 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger.Object);
      subject.LoadEmployees(employees);
      var transaction = new Transaction { EmpId = 2, SSN = "" };
      var transactions = new List<Transaction> { transaction };

      subject.LoadTransactions(transactions);

      logger.Verify(c => c.LogWarning(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void On_LoadTransactions_Mult_Employee_Should_Have_Transactions()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      int id1 = 1, id2 = 2;
      string ssn1 = "1", ssn2 = "2";
      var employee1 = new Employee { ID = id1, SocialSecurity = ssn1 };
      var employee2 = new Employee { ID = id2, SocialSecurity = ssn2 };
      var employees = new List<Employee> { employee1, employee2 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);
      subject.LoadEmployees(employees);
      var transaction1_e1 = new Transaction { EmpId = id1, SSN = "" };
      var transaction2_e1 = new Transaction { EmpId = id1, SSN = "" };
      var transaction3_e2 = new Transaction { EmpId = id2, SSN = "" };
      var transaction4_e2 = new Transaction { EmpId = id2, SSN = "" };
      var transactions = new List<Transaction> { transaction1_e1, transaction2_e1, transaction3_e2, transaction4_e2 };

      subject.LoadTransactions(transactions);

      subject.Employees[id1].Transactions.Count.Should().Be(2);
      subject.Employees[id2].Transactions.Count.Should().Be(2);
    }

    [TestMethod]
    public void On_LoadTransactions_Mult_Employee_Transaction_Should_Have_SSN()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      int id1 = 1, id2 = 2;
      string ssn1 = "1", ssn2 = "2";
      var employee1 = new Employee { ID = id1, SocialSecurity = ssn1 };
      var employee2 = new Employee { ID = id2, SocialSecurity = ssn2 };
      var employees = new List<Employee> { employee1, employee2 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);
      subject.LoadEmployees(employees);
      var transaction1_e1 = new Transaction { EmpId = id1, SSN = "" };
      var transaction2_e1 = new Transaction { EmpId = id1, SSN = "" };
      var transaction3_e2 = new Transaction { EmpId = id2, SSN = "" };
      var transaction4_e2 = new Transaction { EmpId = id2, SSN = "" };
      var transactions = new List<Transaction> { transaction1_e1, transaction2_e1, transaction3_e2, transaction4_e2 };

      subject.LoadTransactions(transactions);

      subject.Employees[id1].Transactions[0].SSN.Should().Be(ssn1);
      subject.Employees[id1].Transactions[1].SSN.Should().Be(ssn1);
      subject.Employees[id2].Transactions[0].SSN.Should().Be(ssn2);
      subject.Employees[id2].Transactions[1].SSN.Should().Be(ssn2);
    }

    [TestMethod]
    public void On_AdjustPaycodesForMultipleShifts_PayCodeTransactionsAreFixed()
    {
      var logger = Mock.Of<ILoggerAdapter<EmployeeRepositoryService>>();
      int id1 = 1;
      string ssn1 = "1";
      var employee1 = new Employee { ID = id1, SocialSecurity = ssn1 };
      var employees = new List<Employee> { employee1 };
      IEmployeeRepositoryService subject = new EmployeeRepositoryService(logger);
      subject.LoadEmployees(employees);
      var transaction1 = new Transaction { EmpId = id1, Type = TransactionType.ClockInOut, ClockIn = DateTime.Today.AddHours(12) };
      var transaction2 = new Transaction { EmpId = id1, Type = TransactionType.PayCode, ClockIn = DateTime.Today.AddHours(12) };
      var transaction3 = new Transaction { EmpId = id1, Type = TransactionType.PayCode, ClockIn = DateTime.Today.AddHours(12) };
      var transactions = new List<Transaction> { transaction1, transaction2, transaction3 };
      subject.LoadTransactions(transactions);

      subject.AdjustPaycodesForMultipleShifts();

      transaction1.ClockIn.Hour.Should().Be(12);
      transaction1.ClockIn.Minute.Should().Be(0);
      transaction2.ClockIn.Hour.Should().Be(12);
      transaction2.ClockIn.Minute.Should().Be(0);
      transaction3.ClockIn.Hour.Should().Be(12);
      transaction3.ClockIn.Minute.Should().Be(5);
    }
  }
}
