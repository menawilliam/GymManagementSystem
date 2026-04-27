using GymManagementSystemBLL.Services.Interfaces;
using GymManagementSystemBLL.ViewModels.CalculatorViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystemPL.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CalorieCalculatorController : Controller
    {
        private readonly ICalorieCalculatorService _calculatorService;

        public CalorieCalculatorController(ICalorieCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecalculateForMember(int memberId)
        {
            var last = _calculatorService.GetLastCalculation(memberId);

            if (last == null)
                return RedirectToAction("MemberDetails", "Member", new { id = memberId });

            var input = new CalorieCalculatorInputViewModel
            {
                MemberId = memberId,
                Age = last.Age,
                Gender = last.Gender,
                HeightCm = last.HeightCm,
                WeightKg = last.WeightKg,
                BodyFatPercentage = last.BodyFatPercentage,
                ActivityLevel = last.ActivityLevel
            };

            return View("Index", input); // أو Index حسب اسم الملف عندك
        }

        [HttpPost]
        public IActionResult Calculate(CalorieCalculatorInputViewModel input)
        {
            try
            {
                var result = _calculatorService.Calculate(input);

                if (input.MemberId.HasValue)
                {
                    _calculatorService.SaveCalculation(input.MemberId, input, result);
                }

                return PartialView("_ResultPartial", result);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }
        }
    }
}