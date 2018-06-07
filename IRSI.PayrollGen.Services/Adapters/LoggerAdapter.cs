using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Adapters
{
  public class LoggerAdapter<T> : ILoggerAdapter<T>
  {
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
      _logger = logger;
    }

    public void LogDebug(string message)
    {
      _logger.LogDebug(message);
    }

    public void LogError(Exception e, string message)
    {
      _logger.LogError(e, message);
    }

    public void LogInformation(string message)
    {
      _logger.LogInformation(message);
    }

    public void LogWarning(string message)
    {
      _logger.LogWarning(message);
    }
  }
}
