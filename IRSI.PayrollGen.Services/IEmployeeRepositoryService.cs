using IRSI.PayrollGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services
{
  public interface IEmployeeRepositoryService
  {
    Dictionary<int, Employee> Employees { get; set; }
    void LoadEmployees(IEnumerable<Employee> employees);
    void LoadTransactions(IEnumerable<Transaction> transactions);
    void AdjustPaycodesForMultipleShifts();
  }
}
