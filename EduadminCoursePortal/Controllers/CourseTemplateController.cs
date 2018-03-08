using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.Models;

namespace EduadminCoursePortal.Controllers
{
    public class CourseTemplateController : Controller
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly BLL.ICourseTemplateService _courseTemplateService;

        public CourseTemplateController(BLL.ICourseTemplateService courseTemplateService, IStringLocalizer<SharedResources> localizer)
        {
            _courseTemplateService = courseTemplateService;
            _localizer = localizer;
        }

        // GET: ObjectList
        //[HttpGet("Lista")]
        //[Route("~/")]
        [HttpGet]
        public async Task<IActionResult> CourseTemplatesList()
        {
            var courseTemplates = await _courseTemplateService.GetCourseTemplates();

            return View("CourseTemplatesList", courseTemplates);
        }

        [Route("{courseTemplateId}")]
        public async Task<ActionResult> CourseTemplate(string courseTemplateId)
        {
            int.TryParse(courseTemplateId, out var courseTemplateIdOut);

            var courseTemplate = await _courseTemplateService.GetCourseTemplate(courseTemplateIdOut);

            return View("CourseTemplate", courseTemplate);
        }

        public async Task<ActionResult> GetCourseTemplatesBySelection(int? subjectID, int? categoryID)
        {
            var courseTemplates = await _courseTemplateService.GetCourseTemplates(subjectID, categoryID);

            return PartialView("_CourseTemplates", courseTemplates);
        }

        [HttpPost]
        public async Task<JsonResult> CreateInterestReg(InterestRegModel model)
        {
            var interestReg = await _courseTemplateService.CreateInterestReg(model);

            if (interestReg == null)
            {
                return Json(new
                {
                    Success = false
                });
            }

            return Json(new
            {
                Success = true,
                Message = _localizer["InterestRegistrationCreated"].ToString()
            });
        }
    }
}