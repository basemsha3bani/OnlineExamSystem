using DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.Builder;
using ServicesClasseslibrary.Interface.DataModel;

namespace OnlineExamSystem.Controllers
{
    public class ExamSectionRulesController : Controller
    {
        private readonly IExamService _examService;
        private readonly IDifficultyLevelsService _difficultyLevelsService;
        private readonly IExamBuilder _examBuilder;

        public ExamSectionRulesController(IExamService examService, IStudySubjectsService subjectService, IDifficultyLevelsService difficultyLevelsService,IExamBuilder examBuilder)
        {
            _examService = examService;
            
            _difficultyLevelsService = difficultyLevelsService;
            _examBuilder = examBuilder;
        }

        public ActionResult Index(int sectionId, int examId)
        {
           
            var rules = _examService.GetSectionRules(sectionId);
            ViewBag.SectionId = sectionId;
            ViewBag.ExamId = examId;
           
            return View(rules);
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
