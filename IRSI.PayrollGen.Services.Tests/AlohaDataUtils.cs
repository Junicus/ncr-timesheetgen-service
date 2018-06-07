using IRSI.PayrollGen.AlohaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRSI.PayrollGen.Services.Tests
{
  public static class AlohaDataUtils
  {
    public static AlohaDataset.empRow CreateEmpRow(AlohaDataset data, int id, string ssn, string firstname, string lastname, decimal sec_num)
    {
      var empRow = data.emp.NewempRow();
      empRow.id = id;
      empRow.ssn = ssn;
      empRow.firstname = firstname;
      empRow.lastname = lastname;
      empRow.sec_num = sec_num;
      return empRow;
    }

    public static AlohaDataset.adjtimeRow CreateAdjTimeRow(AlohaDataset data, int employee,
      string ssn, DateTime date, DateTime sysdatein, DateTime sysdateout, int jobcode,
      int inhour, int inminute, int outhour, int outminute, decimal hours, decimal cctips,
      decimal dectips)
    {
      var adjtimeRow = data.adjtime.NewadjtimeRow();
      adjtimeRow.employee = employee;
      adjtimeRow.ssn = ssn;
      adjtimeRow.date = date;
      adjtimeRow.sysdatein = sysdatein;
      adjtimeRow.sysdateout = sysdateout;
      adjtimeRow.jobcode = jobcode;
      adjtimeRow.inhour = inhour;
      adjtimeRow.inminute = inminute;
      adjtimeRow.outhour = outhour;
      adjtimeRow.outminute = outminute;
      adjtimeRow.hours = hours;
      adjtimeRow.cctips = cctips;
      adjtimeRow.dectips = dectips;
      return adjtimeRow;
    }

    public static AlohaDataset.gndbreakRow CreateBreakRow(AlohaDataset data, int employee,
  string ssn, DateTime date, DateTime sysdatebeg, DateTime sysdateend, int jobcode,
  int inhour, int inminute, int outhour, int outminute, decimal hours)
    {
      var gndbreakRow = data.gndbreak.NewgndbreakRow();
      gndbreakRow.employee = employee;
      gndbreakRow.ssn = ssn;
      gndbreakRow.date = date;
      gndbreakRow.sysdatebeg = sysdatebeg;
      gndbreakRow.sysdateend = sysdateend;
      gndbreakRow.jobcode = jobcode;
      gndbreakRow.inhour = inhour;
      gndbreakRow.inminute = inminute;
      gndbreakRow.outhour = outhour;
      gndbreakRow.outminute = outminute;
      gndbreakRow.hours = hours;
      return gndbreakRow;
    }
  }
}
