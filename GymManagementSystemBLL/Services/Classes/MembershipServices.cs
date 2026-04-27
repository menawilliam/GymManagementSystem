using AutoMapper;
using GymManagementSystemBLL.Services.Interfaces;
using GymManagementSystemBLL.ViewModels.MembershipsViewModels;
using GymManagementSystemDAL.Entities;
using GymManagementSystemDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystemBLL.Services.Classes
{
    public class MembershipServices : IMembershipServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<MembershipViewModel> GetAllMemberships()
        {
            var memberships = _unitOfWork.MembershipRepository.GetAllMembershipsWithMembersAndPlans(m => m.Status == "Active");
            var membershipViewModels = _mapper.Map<IEnumerable<MembershipViewModel>>(memberships);
            return membershipViewModels;
        }
        public bool CreateMembership(CreateMembershipViewModel model)
        {
               // ما تعملش اشتراك لعضو مش موجود
               // ما تعملش اشتراك بخطة مش موجودة
               // ما تعملش اشتراك جديد لعضو عنده اشتراك شغال بالفعل
            if (!IsMemberExists(model.MemberId) || !IsPlanExists(model.PlanId) || HasActiveMemberships(model.MemberId))
                return false;

            var membershipRepo = _unitOfWork.MembershipRepository;
            var membershipToCreate  = _mapper.Map<Membership>(model);//بيحوّل الـ ViewModel اللي جاي من الفورم إلى Entity من نوع:

            var plan = _unitOfWork.GetRepository<Plan>().GetById(model.PlanId);
            membershipToCreate.EndDate = DateTime.UtcNow.AddDays(plan!.DurationDays);// هنا احنا بنحسب تاريخ بدايه الاشتراك ونهايته

            membershipRepo.Add(membershipToCreate);// بنضيف الاشتراك ف ال db 

            return _unitOfWork.SaveChanges() > 0;
        }

        public IEnumerable<MemberForSelectListViewModel> GetMembersForDropdown()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            var memberSelectList = _mapper.Map<IEnumerable<MemberForSelectListViewModel>>(members);
            return memberSelectList;
        }

        public IEnumerable<PlanForSelectListViewModel> GetPlansForDropdown()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll(p => p.IsActive);
            var planSelectList = _mapper.Map<IEnumerable<PlanForSelectListViewModel>>(plans);
            return planSelectList;
        }
        public bool DeleteMembership(int memberId)
        {
            var membershipRepo = _unitOfWork.MembershipRepository;

            var membershipToDelete = membershipRepo.GetFirstOrDefault(m => m.MemberId == memberId && m.Status == "Active");
            if (membershipToDelete is null)
                return false;

            membershipRepo.Delete(membershipToDelete);
            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper Methods
        private bool IsMemberExists(int memberId)
            => _unitOfWork.GetRepository<Member>().GetById(memberId) is not null;
        private bool IsPlanExists(int planId)
            => _unitOfWork.GetRepository<Plan>().GetById(planId) is not null;

        private bool HasActiveMemberships(int memberId)
            => _unitOfWork.MembershipRepository
            .GetAllMembershipsWithMembersAndPlans(m => m.MemberId == memberId && m.Status == "Active").Any();

        #endregion
    }
}
