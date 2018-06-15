using IRSI.PayrollGen.Services;
using IRSI.PayrollGen.Services.Adapters;
using IRSI.PayrollGen.Windows.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRSI.PayrollGen.Windows
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      SetupLogger();

      IPayrollReader payrollReader = new PayrollReader(LogManager.GetLogger("PayrollReader"));
      IPayrollConverter payrollConverter = new PayrollConverter(LogManager.GetLogger("PayrollConverter"),
        new PayrollConverterUtils(LogManager.GetLogger("PayrollConverterUtils")),
        new EmployeeRepositoryService(LogManager.GetLogger("EmployeeRepositoryService")));
      IPayrollWriter payrollWriter = new PayrollXmlWriter(LogManager.GetLogger("PayrollWriter"));

      IDatedFolderProvider datedFolderProvider = new DatedFolderProvider(new EnvironmentVariableAdapter());

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new frmGeneratePayroll(LogManager.GetLogger("frmGeneratePayroll"), payrollReader, payrollConverter, payrollWriter, datedFolderProvider));
    }

    static void SetupLogger()
    {
      var config = new LoggingConfiguration();

      var consoleTarget = new ColoredConsoleTarget();
      config.AddTarget("console", consoleTarget);

      var fileTarget = new FileTarget();
      config.AddTarget("file", fileTarget);

      consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
      fileTarget.FileName = "${basedir}/logs/payrollGenWindows.log";
      fileTarget.ArchiveOldFileOnStartup = true;
      fileTarget.MaxArchiveFiles = 7;
      fileTarget.ArchiveEvery = FileArchivePeriod.Day;

      var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
      config.LoggingRules.Add(rule1);

      var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
      config.LoggingRules.Add(rule2);

      LogManager.Configuration = config;
    }
  }
}
