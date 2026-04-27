using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystemBLL.ViewModels.SessionViewModels
{
    public class SessionMemberViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public bool IsAttended { get; set; }
        public DateTime? CheckInTime { get; set; } // optional, لو عايز تسجل وقت الحضور
    }
}
