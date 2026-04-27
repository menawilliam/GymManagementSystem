using GymManagementSystemBLL.Services.AttachmentService;
using GymManagementSystemBLL.Services.Interfaces;
using GymManagementSystemBLL.ViewModels.CalculatorViewModels;
using GymManagementSystemBLL.ViewModels.MemberViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using GymManagementSystemDAL.Entities;
using GymManagementSystemDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
namespace GymManagementSystemBLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService _attachmentService;
        private readonly ICalorieCalculatorService _calorieCalculatorService;

        #endregion

        #region CTOR

        public MemberServices(
            IUnitOfWork unitOfWork,
            IAttachmentService attachmentService,
            ICalorieCalculatorService calorieCalculatorService)
        {
            _unitOfWork = unitOfWork;
            _attachmentService = attachmentService;
            _calorieCalculatorService = calorieCalculatorService;
        }

        #endregion

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll() ?? [];

            var memberViewModels = members.Select(x => new MemberViewModel
            {
                Id = x.Id,
                Photo = x.Photo,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Gender = x.Gender.ToString(),
            });

            return memberViewModels;
        }

        public bool CreateMember(CreateMemberViewModel createMember)
        {
            try
            {
                if (IsPhoneExist(createMember.Phone) || IsEmailExist(createMember.Email))
                    return false;

                var photoName = _attachmentService.Upload("members", createMember.Photo);
                if (photoName is null)
                    return false;

                var member = new Member
                {
                    Name = createMember.Name,
                    Email = createMember.Email,
                    Phone = createMember.Phone,
                    Gender = createMember.Gender,
                    DateOfBirth = createMember.DateOfBirth,
                    Address = new Address
                    {
                        BuildingNumber = createMember.BuildingNumber,
                        Street = createMember.Street,
                        City = createMember.City,
                    },
                    HealthRecord = new HealthRecord
                    {
                        Height = createMember.HealthViewModel.Height,
                        Weight = createMember.HealthViewModel.Weight,
                        BloodType = createMember.HealthViewModel.BloodType,
                        Note = createMember.HealthViewModel.Note,
                    },
                    Photo = photoName
                };

                _unitOfWork.GetRepository<Member>().Add(member);

                var isCreated = _unitOfWork.SaveChanges() > 0;

                if (!isCreated)
                {
                    _attachmentService.Delete("members", photoName);
                    return false;
                }

                // ===============================
                // Save first calorie calculation
                // ===============================
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - createMember.DateOfBirth.Year;
                if (createMember.DateOfBirth > today.AddYears(-age))
                    age--;

                var calorieInput = new CalorieCalculatorInputViewModel
                {
                    Age = age,
                    Gender = createMember.Gender,
                    HeightCm = (double)createMember.HealthViewModel.Height,
                    WeightKg = (double)createMember.HealthViewModel.Weight,
                    BodyFatPercentage = createMember.BodyFatPercentage,
                    ActivityLevel = createMember.ActivityLevel
                };

                var calorieResult = _calorieCalculatorService.Calculate(calorieInput);
                _calorieCalculatorService.SaveCalculation(member.Id, calorieInput, calorieResult);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public MemberViewModel? GetMemberDetails(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
                return null;

            var lastStats = _calorieCalculatorService.GetLastCalculation(memberId);

            var history = _calorieCalculatorService.GetMemberHistory(memberId)
                .Select(x => new MemberBodyStatsHistoryItemViewModel
                {
                    Id = x.Id, // 🔥 أهم سطر (لازم)
                    CalculatedAt = x.CalculatedAt,
                    BMI = x.BMI,
                    TDEE = x.TDEE,
                    MaintenanceCalories = x.MaintenanceCalories,
                    CuttingCalories = x.CuttingCalories,
                    BulkingCalories = x.BulkingCalories,
                    WeightKg = x.WeightKg
                })
                .ToList();

            var memberViewModel = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",
                Photo = member.Photo,

                BMI = lastStats?.BMI,
                TDEE = lastStats?.TDEE,
                MaintenanceCalories = lastStats?.MaintenanceCalories,

                BodyStatsHistory = history
            };

            var membership = _unitOfWork.GetRepository<Membership>()
                .GetAll(x => x.MemberId == memberId && x.Status == " ")
                .FirstOrDefault();

            if (membership is not null)
            {
                memberViewModel.MembershipStratDate = membership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = membership.EndDate.ToShortDateString();

                var plan = _unitOfWork.GetRepository<Plan>().GetById(membership.PlanId);
                memberViewModel.PlanName = plan?.Name;
            }

            return memberViewModel;
        }

        public UpdateMemberViewModel? UpdateMemberById(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
                return null;

            return new UpdateMemberViewModel
            {
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City,
            };
        }

        public bool UpdateMemberDetails(int memberId, UpdateMemberViewModel updatedMember)
        {
            try
            {
                var memberRepo = _unitOfWork.GetRepository<Member>();

                var emailExist = _unitOfWork.GetRepository<Member>()
                    .GetAll(x => x.Email == updatedMember.Email && x.Id != memberId);

                var phoneExist = _unitOfWork.GetRepository<Member>()
                    .GetAll(x => x.Phone == updatedMember.Phone && x.Id != memberId);

                if (emailExist.Any() || phoneExist.Any())
                    return false;

                var member = memberRepo.GetById(memberId);
                if (member is null)
                    return false;

                member.Name = updatedMember.Name;
                member.Email = updatedMember.Email;
                member.Phone = updatedMember.Phone;
                member.Address.BuildingNumber = updatedMember.BuildingNumber;
                member.Address.Street = updatedMember.Street;
                member.Address.City = updatedMember.City;
                member.UpdatedAt = DateTime.Now;

                memberRepo.Update(member);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteMember(int memberId)
        {
            var memberRepo = _unitOfWork.GetRepository<Member>();
            var membershipRepo = _unitOfWork.GetRepository<Membership>();

            var member = memberRepo.GetById(memberId);
            if (member is null)
                return false;

            var sessionIds = _unitOfWork.GetRepository<MemberSession>()
                .GetAll(x => x.MemberId == memberId)
                .Select(x => x.SessionId);

            var hasActiveSession = _unitOfWork.GetRepository<Session>()
                .GetAll(x => sessionIds.Contains(x.Id) && x.StartDate > DateTime.Now)
                .Any();

            if (hasActiveSession)
                return false;

            var memberships = membershipRepo.GetAll(x => x.MemberId == memberId);

            try
            {
                if (memberships.Any())
                {
                    foreach (var membership in memberships)
                    {
                        membershipRepo.Delete(membership);
                    }
                }

                memberRepo.Delete(member);

                var isDeleted = _unitOfWork.SaveChanges() > 0;

                if (isDeleted)
                    _attachmentService.Delete("members", member.Photo);

                return isDeleted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public HealthViewModel? GetMemberHealthDetails(int memberId)
        {
            var memberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(memberId);

            if (memberHealthRecord is null)
                return null;

            return new HealthViewModel
            {
                Height = memberHealthRecord.Height,
                Weight = memberHealthRecord.Weight,
                BloodType = memberHealthRecord.BloodType,
                Note = memberHealthRecord.Note,
            };
        }

        public MemberViewModel? GetMemberDetailsWithAttendance(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member is null)
                return null;

            var memberSessions = _unitOfWork.GetRepository<MemberSession>()
                .GetAll(ms => ms.MemberId == memberId)
                .ToList();

            var totalBookings = memberSessions.Count;
            var attendedSessions = memberSessions.Count(ms => ms.IsAttended);
            var absentSessions = totalBookings - attendedSessions;

            var attendanceRate = totalBookings > 0
                ? (attendedSessions / (double)totalBookings) * 100
                : 0;

            var lastAttendance = memberSessions
                .Where(ms => ms.IsAttended)
                .OrderByDescending(ms => ms.Session.StartDate)
                .Select(ms => (DateTime?)ms.Session.StartDate)
                .FirstOrDefault();

            var memberViewModel = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",
                Photo = member.Photo,
                AttendanceSummary = new MemberAttendanceSummaryViewModel
                {
                    TotalBookings = totalBookings,
                    AttendedSessions = attendedSessions,
                    AbsentSessions = absentSessions,
                    AttendanceRate = Math.Round(attendanceRate, 2),
                    AttendanceStatus = attendanceRate switch
                    {
                        >= 90 => "Excellent",
                        >= 70 => "Good",
                        >= 50 => "Warning",
                        _ => "Critical"
                    },
                    LastAttendanceDate = lastAttendance
                }
            };

            var membership = _unitOfWork.GetRepository<Membership>()
                .GetAll(x => x.MemberId == memberId && x.Status == "Active")
                .FirstOrDefault();

            if (membership is not null)
            {
                memberViewModel.MembershipStratDate = membership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = membership.EndDate.ToShortDateString();

                var plan = _unitOfWork.GetRepository<Plan>().GetById(membership.PlanId);
                memberViewModel.PlanName = plan?.Name;
            }

            return memberViewModel;
        }



        public IEnumerable<AttendanceDashboardItemViewModel> GetAttendanceDashboard()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll() ?? [];

            var allSessions = _unitOfWork.GetRepository<Session>().GetAll()?.ToList() ?? [];
            var allMemberSessions = _unitOfWork.GetRepository<MemberSession>().GetAll()?.ToList() ?? [];

            var result = members.Select(member =>
            {
                var memberBookings = allMemberSessions
                    .Where(ms => ms.MemberId == member.Id)
                    .ToList();

                var pastSessions = memberBookings
                    .Where(ms =>
                    {
                        var session = allSessions.FirstOrDefault(s => s.Id == ms.SessionId);
                        return session != null && session.StartDate < DateTime.Now;
                    })
                    .ToList();

                var totalBookings = pastSessions.Count;
                var attendedSessions = pastSessions.Count(ms => ms.IsAttended);
                var absentSessions = pastSessions.Count(ms => !ms.IsAttended);

                var attendanceRate = totalBookings > 0
                    ? (attendedSessions / (double)totalBookings) * 100
                    : 0;

                return new AttendanceDashboardItemViewModel
                {
                    MemberId = member.Id,
                    MemberName = member.Name,
                    Photo = member.Photo,
                    TotalBookings = totalBookings,
                    AttendedSessions = attendedSessions,
                    AbsentSessions = absentSessions,
                    AttendanceRate = Math.Round(attendanceRate, 2),
                    AttendanceStatus = attendanceRate switch
                    {
                        >= 90 => "Excellent",
                        >= 70 => "Good",
                        >= 50 => "Warning",
                        _ => "Critical"
                    }
                };
            }).ToList();

            return result;
        }
        public IEnumerable<MemberSelectViewModel> GetMembersForBooking()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll() ?? [];

            return members.Select(m => new MemberSelectViewModel
            {
                Id = m.Id,
                Name = m.Name
            });
        }

        public bool DeleteBodyStat(int bodyStatId)
        {
            var repo = _unitOfWork.GetRepository<MemberBodyStats>();
            var bodyStat = repo.GetById(bodyStatId);

            if (bodyStat == null)
                return false;

            repo.Delete(bodyStat);
            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper Methods

        private bool IsEmailExist(string email)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(x => x.Email == email).Any();
        }

        private bool IsPhoneExist(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(x => x.Phone == phone).Any();
        }

        #endregion
    }
}