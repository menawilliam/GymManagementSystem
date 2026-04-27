using GymManagementSystemDAL.Entities.Enums;
using GymManagementSystemDAL.Entities.Inherited;
using System;

namespace GymManagementSystemDAL.Entities
{
    public class MemberBodyStats : BaseEntity
    {
        public int? MemberId { get; set; }
        public Member? Member { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }                    // ← Enum
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public double? BodyFatPercentage { get; set; }
        public ActivityLevel ActivityLevel { get; set; }      // ← Enum (غير لـ Enum)



        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double BMI { get; set; }
        public double IdealWeightMin { get; set; }
        public double IdealWeightMax { get; set; }
        public int MaintenanceCalories { get; set; }
        public int CuttingCalories { get; set; }
        public int BulkingCalories { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.Now;
    }
}