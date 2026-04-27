using GymManagementSystemBLL.ViewModels.CalculatorViewModels;
using GymManagementSystemDAL.Entities;
using System.Collections.Generic;

namespace GymManagementSystemBLL.Services.Interfaces
{
    public interface ICalorieCalculatorService
    {
        CalorieCalculatorResultViewModel Calculate(CalorieCalculatorInputViewModel input);
        bool SaveCalculation(int? memberId, CalorieCalculatorInputViewModel input, CalorieCalculatorResultViewModel result);
        IEnumerable<MemberBodyStats> GetMemberHistory(int memberId);
        MemberBodyStats? GetLastCalculation(int memberId);
    }
}