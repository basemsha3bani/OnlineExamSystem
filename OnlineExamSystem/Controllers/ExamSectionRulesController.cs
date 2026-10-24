using DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface;
namespace OnlineExamSystem.Controllers
{
    public class ExamSectionRulesController : Controller
    {
        private readonly IExamService exams;
        private readonly IExamSectionService sections;
        private readonly IDifficultyLevelsService difficulties;
        public ExamSectionRulesController(IExamService exams, IExamSectionService sections, IDifficultyLevelsService difficulties)
        { this.exams = exams; this.sections = sections; this.difficulties = difficulties; }
        public IActionResult Index(int sectionId)
        {
            var section = sections.GetById(sectionId);
            if (section == null) return NotFound();
            ViewBag.Section = section;
            return View(exams.GetSectionRules(sectionId));
        }
        private void Options() => ViewBag.difficultyLevels = new SelectList(difficulties.list(), "Id", "DifficultyLevelName");
        public IActionResult Create(int sectionId)
        {
            if (sections.GetById(sectionId) == null) return NotFound();
            Options();
            return View(new ExamSectionRulesDataModel { SectionId = sectionId });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ExamSectionRulesDataModel model)
        {
            if (sections.GetById(model.SectionId) == null) return NotFound();
            model.Id = 0;
            if (ModelState.IsValid)
            {
                try { exams.AddSectionRule(model); return RedirectToAction("Index", new { sectionId = model.SectionId }); }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
            }
            Options(); return View(model);
        }
        public IActionResult Edit(int id)
        {
            var model = exams.GetSectionRule(id);
            if (model == null) return NotFound();
            Options(); return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ExamSectionRulesDataModel model)
        {
            var current = exams.GetSectionRule(id);
            if (current == null || id != model.Id) return NotFound();
            model.SectionId = current.SectionId;
            if (ModelState.IsValid)
            {
                try { exams.EditSectionRule(model); return RedirectToAction("Index", new { sectionId = model.SectionId }); }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
            }
            Options(); return View(model);
        }
    }
}
