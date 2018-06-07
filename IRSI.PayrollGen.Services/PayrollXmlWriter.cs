using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using IRSI.PayrollGen.Models;
using IRSI.PayrollGen.Services.Adapters;

namespace IRSI.PayrollGen.Services
{
  public class PayrollXmlWriter : IPayrollWriter
  {
    private readonly ILoggerAdapter<PayrollXmlWriter> _logger;

    public PayrollXmlWriter(ILoggerAdapter<PayrollXmlWriter> logger)
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