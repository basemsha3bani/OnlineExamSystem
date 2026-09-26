using DataModel;
using DataRepository.DataRepositoryEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface.DataModel;

namespace OnlineExamSystem.Controllers
{
    public class ExamsController : Controller
    {
        private readonly IExamService _examService;
        private readonly IStudySubjectsService _subjectService;
        public ExamsController(IExamService examService, IStudySubjectsService subjectService)
        {
            _examService = examService;
            _subjectService = subjectService;
        }
        public IActionResult Index() => View(_examService.List());

        public IActionResult Create()
        {
            ViewBag.StudySubjectsId = new SelectList(_subjectService.list(), "Value", "Text");
            return View(new ExamDataModel());
        }
        [HttpPost]
        public IActionResult Create(ExamDataModel model)
        {
            if (ModelState.IsValid) { _examService.Add(model); return RedirectToAction("Index"); }
            ViewBag.StudySubjectsId = new SelectList(_subjectService.list(), "Value", "Text");
            return View(model);
        }
    
}
}
