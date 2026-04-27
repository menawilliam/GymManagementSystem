using GymManagementSystemDAL.Entities;
using GymManagementSystemDAL.Entities.Enums;

namespace GymManagementSystemBLL.ViewModels.CalculatorViewModels
{
    public class CalorieCalculatorInputViewModel
    {
        public int? MemberId { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public double? BodyFatPercentage { get; set; }
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.LightlyActive;
    }

    public class CalorieCalculatorResultViewModel
    {
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public Dictionary<string, double> AllActivityLevels { get; set; } = new();
        public double IdealWeightMin { get; set; }
        public double IdealWeightMax { get; set; }
        public List<IdealWeightFormulaViewModel> IdealWeightFormulas { get; set; } = new();
        public double BMI { get; set; }
        public string BMICategory { get; set; } = string.Empty;
        public int MaintenanceCalories { get; set; }
        public int CuttingCalories { get; set; }
        public int BulkingCalories { get; set; }
        public MacroPlanViewModel MaintenanceMacros { get; set; } = null!;
        public MacroPlanViewModel CuttingMacros { get; set; } = null!;
        public MacroPlanViewModel BulkingMacros { get; set; } = null!;
    }

    public class IdealWeightFormulaViewModel
    {
        public string FormulaName { get; set; } = string.Empty;
        public double Weight { get; set; }
    }

    public class MacroPlanViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Calories { get; set; }
        public int ProteinGrams { get; set; }
        public int CarbsGrams { get; set; }
        public int FatsGrams { get; set; }
        public string Ratio { get; set; } = string.Empty;
    }
}