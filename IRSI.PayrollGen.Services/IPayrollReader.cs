using IRSI.PayrollGen.AlohaData;

namespace IRSI.PayrollGen.Services
{
  public interface IPayrollReader
  {
    AlohaDataset ReadPayroll(string folder);
    string GetConnectionString(string folder);
  }
}