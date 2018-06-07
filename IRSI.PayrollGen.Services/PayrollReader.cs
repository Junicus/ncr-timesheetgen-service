using IRSI.PayrollGen.AlohaData;
using IRSI.PayrollGen.AlohaData.AlohaDatasetTableAdapters;
using IRSI.PayrollGen.Services.Adapters;
using System;
using System.IO;

namespace IRSI.PayrollGen.Services
{
  public class PayrollReader : IPayrollReader
  {
    private readonly ILoggerAdapter<PayrollReader> _logger;

    public PayrollReader(ILoggerAdapter<PayrollReader> logger)
    {
      _logger = logger;
    }

    public string GetConnectionString(string folder) => $"Provider=VFPOLEDB.1;Data Source={folder}";

    public AlohaDataset ReadPayroll(string folder)
    {
      if (string.IsNullOrEmpty(folder)) throw new ArgumentException("Folder name is null or empty", nameof(folder));

      if (Directory.Exists(folder))
      {
        _logger.LogInformation($"ReadPayroll was called for folder {folder}");
        var result = new AlohaDataset();

        _logger.LogDebug("Loading emp table");
        var empAdapter = new empTableAdapter();
        empAdapter.Connection.ConnectionString = GetConnectionString(folder);
        empAdapter.Fill(result.emp);
        _logger.LogDebug($"Done loading emp, {result.emp.Count} loaded");

        _logger.LogDebug("Loading adjtime table");
        var adjtimeAdapter = new adjtimeTableAdapter();
        adjtimeAdapter.Connection.ConnectionString = GetConnectionString(folder);
        adjtimeAdapter.Fill(result.adjtime);
        _logger.LogDebug($"Done loading adjtime, {result.adjtime.Count} loaded");

        _logger.LogDebug("Loading gndbreak table");
        var gndbreakAdapter = new gndbreakTableAdapter();
        gndbreakAdapter.Connection.ConnectionString = GetConnectionString(folder);
        gndbreakAdapter.Fill(result.gndbreak);
        _logger.LogDebug($"Done loading gndbreak, {result.gndbreak.Count} loaded");

        _logger.LogInformation("Done ReadPayroll");
        return result;
      }
      else
      {
        _logger.LogWarning($"ReadPayroll: Folder {folder} not found");
        return null;
      }
    }
  }
}