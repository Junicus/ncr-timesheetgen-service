using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IRSI.PayrollGen.Models
{
  [Serializable]
  public class Transaction
  {
    [XmlAttribute]
    public TransactionType Type { get; set; }
    [XmlAttribute]
    public PeriodType PeriodType { get; set; }
    [XmlAttribute("EmpID")]
    public int EmpId { get; set; }
    [XmlAttribute]
    public string SSN { get; set; }
    [XmlElement]
    public string PayCode { get; set; }
    [XmlElement]
    public int JobCode { get; set; }
    [XmlElement]
    public DateTime ClockIn { get; set; }
    [XmlElement]
    public DateTime ClockOut { get; set; }
    [XmlElement]
    public decimal Hours { get; set; }
    [XmlElement]
    public decimal Tips { get; set; }
  }
}
