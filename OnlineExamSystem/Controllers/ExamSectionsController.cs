using DataModel;
using DataRepository.DataRepositoryEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface.Builder;
using System.Linq;

namespace OnlineExamSystem.Controllers
{
    public class ExamSectionsController : Controller
    {
        private readonly IExamService _examService;
        private readonly IExamSectionService _examSectionService;
        

        public ExamSectionsController(IExamService examService, IExamSectionService examSectionService)
        {
            _examService = examService;
            _examSectionService = examSectionService;
           
        }

        // GET: ExamSections

        public ActionResult Create(int examId)
        {
         
            if (_examService.GetById(examId) == null) return NotFound();
            return PartialView("Create", new ExamSectionsDataModel { ExamId=examId});
        }

        // POST: ExamSections/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExamSectionsDataModel model)
        {


           var exam= _examService.GetById(model.ExamId);
            if (exam == null) return NotFound();
            if (string.IsNullOrWhiteSpace(model.SectionName)) ModelState.AddModelError("SectionName", "Section name is required.");
            if (model.Percentage < 0 || model.Percentage > 100) ModelState.AddModelError("Percentage", "Percentage must be between 0 and 100.");
           

            // 4. Save if valid
            if (ModelState.IsValid)
            {
                // Set CreatedBy / Id if you need
                _examSectionService.Add(model);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_List", _examSectionService.List(model.ExamId));
                return RedirectToAction("Details", "Exams", new { id = model.ExamId });
            }

            Response.StatusCode = 422;
            return PartialView("Create", model);
        }



    }
}
