using DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.DataModel;

namespace OnlineExamSystem.Controllers
{
    public class ExamSectionRulesController : Controller
    {
        private readonly IExamService _examService;
        private readonly IDifficultyLevelsService _difficultyLevelsService;
       
        public ExamSectionRulesController(IExamService examService, IStudySubjectsService subjectService, IDifficultyLevelsService difficultyLevelsService)
        {
            _examService = examService;
            
            _difficultyLevelsService = difficultyLevelsService;
        }

        public ActionResult Index(int sectionId, int examId)
        {
            
            var rule = _examService.GetSectionRules(sectionId);
            ViewBag.SectionId = sectionId;
            ViewBag.ExamId = examId;
           
            return View(rule);
        }

        [HttpGet]
        public IActionResult Create(int sectionId)
        {
            ExamSectionRulesDataModel examSectionRules= new ExamSectionRulesDataModel();
            examSectionRules.SectionId = sectionId;
            ViewBag.difficultyLevels = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            return View(examSectionRules);
        }
        [HttpPost]
        public IActionResult Create(ExamSectionRulesDataModel model)
        {
            _examService.AddSectionRule(model);
            return RedirectToAction("Index", new { sectionId = model.SectionId });
        }
    }
}
