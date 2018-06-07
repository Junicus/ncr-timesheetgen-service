using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IRSI.PayrollGen.Models
{
  [Serializable]
  public class Store
  {
    public Store()
    {
      Employees = new List<Employee>();
    }

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute]
    public string StoreName { get; set; }

    [XmlElement]
    public List<Employee> Employees { get; set; }
  }
}