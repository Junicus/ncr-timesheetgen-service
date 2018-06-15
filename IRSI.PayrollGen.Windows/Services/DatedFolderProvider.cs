using IRSI.PayrollGen.Services.Adapters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IRSI.PayrollGen.Windows.Services
{
  public class DatedFolderProvider : IDatedFolderProvider
  {
    private readonly IEnvironmentVariableAdapter _environmentVariableAdapter;

    public DatedFolderProvider(IEnvironmentVariableAdapter environmentVariableAdapter)
    {
      _environmentVariableAdapter = environmentVariableAdapter;
    }

    public List<string> GetDatedFolders()
    {
      var iberdir = _environmentVariableAdapter.GetvEnvironmentVariable("IBERDIR");
      return Directory.GetDirectories(iberdir, "20??????").Select(x => Path.GetFileName(x)).OrderByDescending(x => x)
        .Take(30).ToList();
    }

    public string GetDatedFolderFullName(string datePortion)
    {
      var iberdir = _environmentVariableAdapter.GetvEnvironmentVariable("IBERDIR");
      return Path.Combine(iberdir, datePortion);
    }
  }
}