using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface.DataModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace OnlineExamSystem.Controllers
{
    public class ExamSectionsController : Controller    
    {
        private readonly IExamService exams;
        private readonly IExamSectionService sections;
        private readonly IStudySubjectsService subjects;
        private readonly ILogger<ExamSectionsController> logger;
        public ExamSectionsController(IExamService exams, IExamSectionService sections, IStudySubjectsService subjects, ILogger<ExamSectionsController> logger)
        { this.exams = exams; this.sections = sections; this.subjects = subjects; this.logger = logger; }
        public IActionResult Index(int examId)
        {
            var exam = exams.GetById(examId);
            if (exam == null) return NotFound();
            ViewBag.Exam = exam;
            return View(sections.List(examId));
        }
        public IActionResult Create(int examId)
        {
            if (exams.GetById(examId) == null) return NotFound();
            return View(ExamSectionsDTO.Define( ExamId: examId ));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ExamSectionsDTO model)
        {
            if (exams.GetById(model.ExamId) == null) return NotFound();
            if (ModelState.IsValid)
            {
                try { sections.Add(model); return RedirectToAction("Edit", new { id = model.Id }); }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
            }
            return View(model);
        }
        private void Chapters(ExamSectionsDTO model)
        {
            var exam = exams.GetById(model.ExamId);
            ViewBag.Chapters = new SelectList(Enumerable.Range(1, subjects.GetById(exam.StudySubjectId).NumberOfChapters));
        }
        public IActionResult Edit(int id)
        {
            var model = sections.GetById(id);
            if (model == null) return NotFound();
            Chapters(model);
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ExamSectionsDTO model)
        {
            var current = sections.GetById(id);
            if (current == null || id != model.Id) return NotFound();
            model.ChangeExamId(current.ExamId);
            if (ModelState.IsValid)
            {
                try
                {
                    sections.Edit(model);
                    TempData["SectionSuccess"] = "Section configuration saved successfully.";
                    return RedirectToAction("Edit", new { id });
                }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
                catch (DbUpdateException ex)
                {
                    logger.LogError(ex, "Failed to save configuration for section {SectionId}", id);
                    ModelState.AddModelError("", "The section configuration could not be saved. Please try again.");
                }
            }
            ViewData["SectionFailure"] = "Section configuration was not saved. Please review the errors below.";
            model.ChangeExamSectionRules(current.ExamSectionRules);
            Chapters(model);
            return View(model);
        }
    }
}
