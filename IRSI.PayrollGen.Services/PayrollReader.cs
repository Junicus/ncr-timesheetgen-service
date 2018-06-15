using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.AlohaData.AlohaDatasetTableAdapters;
using IRSI.PayrollGen.Services.Adapters;
using NLog;
using System;
using System.IO;

namespace IRSI.PayrollGen.Services
{
  public class PayrollReader : IPayrollReader
  {
    private readonly ILogger _logger;
    //private readonly ILoggerAdapter<PayrollReader> _logger;

    public PayrollReader(ILogger logger)
    {
      _logger = logger;
    }

    public string GetConnectionString(string folder) => $"Provider=VFPOLEDB.1;Data Source={folder}";

    public AlohaDataset ReadPayroll(string folder)
    {
      if (string.IsNullOrEmpty(folder)) throw new ArgumentException("Folder name is null or empty", nameof(folder));

      if (Directory.Exists(folder))
      {
        _logger.Info($"ReadPayroll was called for folder {folder}");
        var result = new AlohaDataset();

        _logger.Debug("Loading emp table");
        var empAdapter = new empTableAdapter();
        empAdapter.Connection.ConnectionString = GetConnectionString(folder);
        empAdapter.Fill(result.emp);
        _logger.Debug($"Done loading emp, {result.emp.Count} loaded");

        _logger.Debug("Loading adjtime table");
        var adjtimeAdapter = new adjtimeTableAdapter();
        adjtimeAdapter.Connection.ConnectionString = GetConnectionString(folder);
        adjtimeAdapter.Fill(result.adjtime);
        _logger.Debug($"Done loading adjtime, {result.adjtime.Count} loaded");

        _logger.Debug("Loading gndbreak table");
        var gndbreakAdapter = new gndbreakTableAdapter();
        gndbreakAdapter.Connection.ConnectionString = GetConnectionString(folder);
        gndbreakAdapter.Fill(result.gndbreak);
        _logger.Debug($"Done loading gndbreak, {result.gndbreak.Count} loaded");

        _logger.Info("Done ReadPayroll");
        return result;
      }
      else
      {
        _logger.Warn($"ReadPayroll: Folder {folder} not found");
        return null;
      }
    }
  }
}