using GymManagementSystemBLL.Services.Interfaces;
using GymManagementSystemBLL.ViewModels.CalculatorViewModels;
using GymManagementSystemDAL.Entities;
using GymManagementSystemDAL.Entities;
using GymManagementSystemDAL.Entities.Enums;
using GymManagementSystemDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymManagementSystemBLL.Services.Classes
{
    public class CalorieCalculatorService : ICalorieCalculatorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalorieCalculatorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CalorieCalculatorResultViewModel Calculate(CalorieCalculatorInputViewModel input)
        {
            double bmr = CalculateBMR(input);
            double tdee = bmr * GetActivityMultiplier(input.ActivityLevel);

            var allLevels = new Dictionary<string, double>
            {
                { "Basal Metabolic Rate", bmr },
                { "Sedentary", bmr * 1.2 },
                { "Light Exercise", bmr * 1.375 },
                { "Moderate Exercise", bmr * 1.55 },
                { "Heavy Exercise", bmr * 1.725 },
                { "Athlete", bmr * 1.9 }
            };

            var idealWeights = CalculateIdealWeight(input.HeightCm, input.Gender);
            double idealMin = idealWeights.Min(w => w.Weight);
            double idealMax = idealWeights.Max(w => w.Weight);

            double bmi = input.WeightKg / Math.Pow(input.HeightCm / 100, 2);
            string bmiCategory = GetBMICategory(bmi);

            int maintenance = (int)tdee;
            int cutting = maintenance - 500;
            int bulking = maintenance + 500;

            return new CalorieCalculatorResultViewModel
            {
                BMR = Math.Round(bmr),
                TDEE = Math.Round(tdee),
                AllActivityLevels = allLevels.ToDictionary(k => k.Key, v => Math.Round(v.Value)),
                IdealWeightMin = idealMin,
                IdealWeightMax = idealMax,
                IdealWeightFormulas = idealWeights,
                BMI = Math.Round(bmi, 1),
                BMICategory = bmiCategory,
                MaintenanceCalories = maintenance,
                CuttingCalories = cutting,
                BulkingCalories = bulking,
                MaintenanceMacros = CalculateMacros(maintenance, "Maintenance"),
                CuttingMacros = CalculateMacros(cutting, "Fat Loss"),
                BulkingMacros = CalculateMacros(bulking, "Muscle Gain")
            };
        }

        // 
        private double CalculateBMR(CalorieCalculatorInputViewModel input)
        {
            if (input.BodyFatPercentage.HasValue && input.BodyFatPercentage > 0)
            {
                double leanMass = input.WeightKg * (1 - (input.BodyFatPercentage.Value / 100));
                return 370 + (21.6 * leanMass);
            }

            if (input.Gender == Gender.Male)
                return (10 * input.WeightKg) + (6.25 * input.HeightCm) - (5 * input.Age) + 5;
            else
                return (10 * input.WeightKg) + (6.25 * input.HeightCm) - (5 * input.Age) - 161;
        }

        // ✅ Enum بدل string
        private double GetActivityMultiplier(ActivityLevel level) => level switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.LightlyActive => 1.375,
            ActivityLevel.ModeratelyActive => 1.55,
            ActivityLevel.VeryActive => 1.725,
            ActivityLevel.SuperActive => 1.9,
            _ => 1.2
        };

        // ✅ Enum بدل string
        private List<IdealWeightFormulaViewModel> CalculateIdealWeight(double heightCm, Gender gender)
        {
            double heightInches = heightCm / 2.54;
            var formulas = new List<IdealWeightFormulaViewModel>();

            if (gender == Gender.Male)
            {
                formulas.Add(new IdealWeightFormulaViewModel
                {
                    FormulaName = "Hamwi Formula",
                    Weight = Math.Round(48 + (2.7 * (heightInches - 60)), 0)
                });

                formulas.Add(new IdealWeightFormulaViewModel
                {
                    FormulaName = "Devine Formula",
                    Weight = Math.Round(50 + (2.3 * (heightInches - 60)), 0)
                });
            }
            else
            {
                formulas.Add(new IdealWeightFormulaViewModel
                {
                    FormulaName = "Hamwi Formula",
                    Weight = Math.Round(45.5 + (2.2 * (heightInches - 60)), 0)
                });

                formulas.Add(new IdealWeightFormulaViewModel
                {
                    FormulaName = "Devine Formula",
                    Weight = Math.Round(45.5 + (2.3 * (heightInches - 60)), 0)
                });
            }

            return formulas;
        }

        private string GetBMICategory(double bmi) => bmi switch
        {
            < 18.5 => "Underweight",
            < 25 => "Normal",
            < 30 => "Overweight",
            _ => "Obese"
        };

        private MacroPlanViewModel CalculateMacros(int calories, string name)
        {
            int protein = (int)((calories * 0.30) / 4);
            int fats = (int)((calories * 0.35) / 9);
            int carbs = (int)((calories * 0.35) / 4);

            return new MacroPlanViewModel
            {
                Name = name,
                Calories = calories,
                ProteinGrams = protein,
                FatsGrams = fats,
                CarbsGrams = carbs,
                Ratio = "30/35/35"
            };
        }

        // ✅ حفظ البيانات (Enum مباشرة)
        public bool SaveCalculation(int? memberId, CalorieCalculatorInputViewModel input, CalorieCalculatorResultViewModel result)
        {
            try
            {
                var stats = new MemberBodyStats
                {
                    MemberId = memberId,
                    Age = input.Age,
                    Gender = input.Gender, // ✅ صح
                    HeightCm = input.HeightCm,
                    WeightKg = input.WeightKg,
                    BodyFatPercentage = input.BodyFatPercentage,
                    ActivityLevel = input.ActivityLevel,
                    BMR = result.BMR,
                    TDEE = result.TDEE,
                    BMI = result.BMI,
                    IdealWeightMin = result.IdealWeightMin,
                    IdealWeightMax = result.IdealWeightMax,
                    MaintenanceCalories = result.MaintenanceCalories,
                    CuttingCalories = result.CuttingCalories,
                    BulkingCalories = result.BulkingCalories,
                    CalculatedAt = DateTime.Now
                };

                _unitOfWork.GetRepository<MemberBodyStats>().Add(stats);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MemberBodyStats> GetMemberHistory(int memberId)
        {
            return _unitOfWork.GetRepository<MemberBodyStats>()
                .GetAll(m => m.MemberId == memberId)
                .OrderByDescending(m => m.CalculatedAt);
        }

        public MemberBodyStats? GetLastCalculation(int memberId)
        {
            return _unitOfWork.GetRepository<MemberBodyStats>()
                .GetAll(m => m.MemberId == memberId)
                .OrderByDescending(m => m.CalculatedAt)
                .FirstOrDefault();
        }
    }
}