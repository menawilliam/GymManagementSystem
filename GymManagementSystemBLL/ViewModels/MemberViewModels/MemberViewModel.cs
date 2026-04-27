using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystemBLL.ViewModels.MemberViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; }= null!;
        public string Name { get; set; }  = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gender { get; set; } = null!;

        //Details [Address , BithDate , Membership StratDate , Membership EndDate , Plan Name]
        public string? PlanName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? MembershipStratDate{ get; set; }
        public string? MembershipEndDate{ get; set; }
        public string? Address{ get; set; }

        public double? BMI { get; set; }
        public double? TDEE { get; set; }
        public int? MaintenanceCalories { get; set; }

        // 🆕 الجديد: ملخص الحضور
        public MemberAttendanceSummaryViewModel? AttendanceSummary { get; set; }
        public IEnumerable<MemberBodyStatsHistoryItemViewModel> BodyStatsHistory { get; set; } = new List<MemberBodyStatsHistoryItemViewModel>();
    }

    // 🆕 Class جديد للحضور
    public class MemberAttendanceSummaryViewModel
    {
        public int TotalBookings { get; set; }           // إجمالي الحجوزات
        public int AttendedSessions { get; set; }        // الحصص اللي حضرها
        public int AbsentSessions { get; set; }          // الحصص اللي غاب عنها

        public double AttendanceRate { get; set; }       // نسبة الحضور (0-100)

        public string AttendanceStatus { get; set; } = string.Empty;  // Excellent / Good / Warning / Critical

        public DateTime? LastAttendanceDate { get; set; } // تاريخ آخر حضور

        // خاصية مساعدة للتنبيه
        public bool IsLowAttendance => AttendanceRate < 50 && TotalBookings > 0;
    }
    public class MemberBodyStatsHistoryItemViewModel
    {
        public int Id { get; set; }
        public DateTime CalculatedAt { get; set; }
        public double BMI { get; set; }
        public double TDEE { get; set; }
        public int MaintenanceCalories { get; set; }
        public int CuttingCalories { get; set; }
        public int BulkingCalories { get; set; }
        public double WeightKg { get; set; }
    }




}

