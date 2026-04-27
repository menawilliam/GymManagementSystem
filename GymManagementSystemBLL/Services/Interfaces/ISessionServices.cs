using GymManagementSystemBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystemBLL.Services.Interfaces
{
    public interface ISessionServices
    {
        IEnumerable<SessionViewModel> GetAllSessions();
        SessionViewModel? GetSessionById(int id);
        bool CreateSession(CreateSessionViewModel createdSession);

        UpdateSessionViewModel? GetSessionToUpdate(int sessionId);
        bool UpdateSession(UpdateSessionViewModel updatedSession, int sessionId);

        bool RemoveSession(int sessionId);
        SessionViewModel? GetSessionToDelete(int id);

        IEnumerable<TrainerSelectViewModel> GetTrainerForSession();
        IEnumerable<CategorySelectViewModel> GetCategoryForSession();

        IEnumerable<MemberSelectViewModel> GetMembersForBooking();
        // ===== جديد =====
        bool MarkMemberAttendance(int sessionId, int memberId);
        bool BookSession(int sessionId, int memberId);

    }
}
