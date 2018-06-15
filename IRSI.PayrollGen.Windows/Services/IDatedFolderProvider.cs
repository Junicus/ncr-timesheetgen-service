using System.Collections.Generic;

namespace IRSI.PayrollGen.Windows.Services
{
  public interface IDatedFolderProvider
  {
    List<string> GetDatedFolders();
    string GetDatedFolderFullName(string datePortion);
  }
}