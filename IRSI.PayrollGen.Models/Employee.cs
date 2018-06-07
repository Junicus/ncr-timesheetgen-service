using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IRSI.PayrollGen.Models
{
  [Serializable]
  public class Employee
  {
    public Employee()
    {
      Transactions = new List<Transaction>();
    }

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute]
    public string SocialSecurity { get; set; }

    [XmlAttribute]
    public string FirstName { get; set; }

    [XmlAttribute]
    public string LastName { get; set; }

    [XmlElement]
    public List<Transaction> Transactions { get; set; }
  }
}
