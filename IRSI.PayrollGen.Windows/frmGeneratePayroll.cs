using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IRSI.PayrollGen.Services;
using IRSI.PayrollGen.Windows.Services;
using NLog;

namespace IRSI.PayrollGen.Windows
{
  public partial class frmGeneratePayroll : Form
  {
    private readonly ILogger _logger;
    private readonly IPayrollReader _payrollReader;
    private readonly IPayrollConverter _payrollConverter;
    private readonly IPayrollWriter _payrollWriter;
    private readonly IDatedFolderProvider _datedFolderProvider;

    private readonly string _ftpOutputPath;
    private readonly string _portalOutputPath;

    private List<string> _datedFolders;

    public frmGeneratePayroll(ILogger logger, IPayrollReader payrollReader1, IPayrollConverter payrollConverter, IPayrollWriter payrollWriter, IDatedFolderProvider datedFolderProvider)
    {
      _logger = logger;
      _payrollReader = payrollReader1;
      _payrollConverter = payrollConverter;
      _payrollWriter = payrollWriter;
      _datedFolderProvider = datedFolderProvider;
      InitializeComponent();
      var currentFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      _ftpOutputPath = Path.Combine(currentFolder, "ftpOutput");
      _portalOutputPath = Path.Combine(currentFolder, "portalOutput");
      _datedFolders = _datedFolderProvider.GetDatedFolders();
    }
  }
}
