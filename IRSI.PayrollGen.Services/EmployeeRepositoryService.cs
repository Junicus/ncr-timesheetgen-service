using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;

namespace IRSI.PayrollGen.Services
{
  public class EmployeeRepositoryService : IEmployeeRepositoryService
  {
    private readonly ILoggerAdapter<EmployeeRepositoryService> _logger;

    public EmployeeRepositoryService(ILoggerAdapter<EmployeeRepositoryService> logger)
    {
      _logger = logger;
      Employees = new Dictionary<int, Employee>();
    }

    public Dictionary<int, Employee> Employees { get; set; }

    public void LoadEmployees(IEnumerable<Employee> employees)
    {
      foreach (var employee in employees)
      {
        if (!Employees.Keys.Contains(employee.ID))
        {
          Employees[employee.ID] = employee;
        }
        else
        {
          _logger.LogWarning($"Employee with ID {employee.ID} already added");
        }
      }
    }

    public void LoadTransactions(IEnumerable<Transaction> transactions)
    {
      foreach (var transaction in transactions)
      {
        if (Employees.Keys.Contains(transaction.EmpId))
        {
          transaction.SSN = Employees[transaction.EmpId].SocialSecurity;
          Employees[transaction.EmpId].Transactions.Add(transaction);
        }
        else
        {
          _logger.LogWarning($"Employee {transaction.EmpId} was not found");
        }
      }
    }

    public void AdjustPaycodesForMultipleShifts()
    {
      foreach (var employee in Employees.Values)
      {
        var minutesToAdd = 0;
        foreach (var t in employee.Transactions.Where(t => t.Type == TransactionType.PayCode))
        {
          t.ClockIn = t.ClockIn.AddMinutes(minutesToAdd);
          minutesToAdd += 5;
        }
      }
    }
  }
}
