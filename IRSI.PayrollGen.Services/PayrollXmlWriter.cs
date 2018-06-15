using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;
using NLog;

namespace IRSI.PayrollGen.Services
{
  public class PayrollXmlWriter : IPayrollWriter
  {
    public readonly ILogger _logger;
    //private readonly ILoggerAdapter<PayrollXmlWriter> _logger;

    public PayrollXmlWriter(ILogger logger)
    {
      _logger = logger;
    }

    public void WriteToStream(Stream stream, List<Employee> employees, int storeId, string storeName)
    {
      var store = new Store()
      {
        ID = storeId,
        StoreName = storeName,
        Employees = employees
      };
      var xmlSerializer = new XmlSerializer(typeof(Store));
      xmlSerializer.Serialize(stream, store);
    }
  }
}