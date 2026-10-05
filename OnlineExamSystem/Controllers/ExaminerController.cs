using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Models;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataModel;
using ServicesClasseslibrary.Implmentation.Examiner;
using ServicesClasseslibrary.Interface.Examiner;

namespace OnlineExamSystem.Controllers
{
    [RoleAuthorize("Examiner")]
    [AutoValidateAntiforgeryToken]
    public class ExaminerController : Controller
    {
        private readonly IExaminerAttemptService attempts;
        private readonly IExamOprations exams;
        private int UserId => HttpContext.Session.GetInt32("UserId").Value;
        public ExaminerController(IExaminerAttemptService attempts, IExamOprations exams)
        { this.attempts = attempts; this.exams = exams; }
        public IActionResult Index() => View(exams.list());
        [HttpPost]
        public IActionResult Start(int examId)
        {
            try { return RedirectToAction(nameof(Take), new { id = attempts.Start(examId, UserId) }); }
            catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); return View("Index", exams.list()); }
        }
        public IActionResult Take(int id, int sectionIndex = 0)
        {
            var attempt = attempts.Get(id);
            if (attempt == null) return NotFound();
            if (attempt.Status != "InProgress") return RedirectToAction(nameof(Attempts));
            var snapshot = ExaminerAttemptService.Read(attempt);
            if (sectionIndex < 0 || sectionIndex >= snapshot.Sections.Count) return BadRequest();
            var section = snapshot.Sections[sectionIndex];
            return View(new AttemptSectionViewModel { AttemptId = id, ExamTitle = attempt.ExamTitle,
                SectionIndex = sectionIndex, SectionCount = snapshot.Sections.Count, SectionName = section.Name,
                Questions = section.Questions.Select(q => new AttemptQuestionViewModel { Id = q.Id, Text = q.Text,
                    SelectedOptionId = q.SelectedOptionId, Options = q.Options.ToDictionary(o => o.Id, o => o.Text) }).ToList() });
        }
        [HttpPost]
        public IActionResult Save(int id, int sectionIndex, Dictionary<int, int?> answers, string command)
        {
            if (attempts.Get(id) == null) return NotFound();
            if (!ModelState.IsValid || (command != "previous" && command != "next" && command != "submit" && command != "save")) return BadRequest();
           
            try { attempts.Save(id,  sectionIndex, answers ?? new Dictionary<int, int?>(), command == "submit"); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (DbUpdateConcurrencyException) { TempData["Message"] = "This attempt changed in another window. Please review the latest saved answers."; return RedirectToAction(nameof(Take), new { id, sectionIndex }); }
            if (command == "submit") return RedirectToAction(nameof(ThankYou), new { id });
            return RedirectToAction(nameof(Take), new { id, sectionIndex = sectionIndex + (command == "next" ? 1 : command == "previous" ? -1 : 0) });
        }
        public IActionResult ThankYou(int id)
        {
            var attempt = attempts.Get(id);
            if (attempt == null) return NotFound();
            if (attempt.Status == "InProgress") return RedirectToAction(nameof(Take), new { id });
            return View();
        }
        public IActionResult Attempts() => View(attempts.List(UserId));
        public IActionResult Result(int id)
        {
            var attempt = attempts.Get(id);
            if (attempt == null) return NotFound();
            if (attempt.Status != "Scored") return RedirectToAction(nameof(Attempts));
            return View(attempt);
        }
    }
}
