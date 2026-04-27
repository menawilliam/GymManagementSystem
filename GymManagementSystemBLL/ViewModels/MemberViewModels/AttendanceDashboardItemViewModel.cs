namespace GymManagementSystemBLL.ViewModels.MemberViewModels
{
    public class AttendanceDashboardItemViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? Photo { get; set; }

        public int TotalBookings { get; set; }
        public int AttendedSessions { get; set; }
        public int AbsentSessions { get; set; }

        public double AttendanceRate { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty;
    }
}