using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Adapters
{
  public interface ILoggerAdapter<T>
  {
    void LogInformation(string message);
    void LogWarning(string message);
    void LogDebug(string message);
    void LogError(Exception e, string message);
  }
}
