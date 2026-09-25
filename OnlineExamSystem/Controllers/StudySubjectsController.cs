using Microsoft.AspNetCore.Mvc;
using DataModel;
using ServicesClasseslibrary.Interface.DataModel;

namespace OnlineExamSystem.Controllers
{
    public class StudySubjectsController : Controller
    {
        private readonly IStudySubjectsService _service;
        public StudySubjectsController(IStudySubjectsService service) { _service = service; }

        public IActionResult Index() => View(_service.list());
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(StudySubjectDataModel m)
        {
            if (ModelState.IsValid) { _service.Add(m); return RedirectToAction(nameof(Index)); }
            return View(m);
        }
        // Edit/Delete same as DifficultyLevels - copy paste
        public IActionResult Edit(int? id) { if (id == null) return NotFound(); var e = _service.GetById((int)id); if (e == null) return NotFound(); return View(e); }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StudySubjectDataModel m) { if (id != m.Id) return NotFound(); if (ModelState.IsValid) { _service.Edit(m); return RedirectToAction(nameof(Index)); } return View(m); }
        public IActionResult Delete(int? id) { if (id == null) return NotFound(); var e = _service.GetById((int)id); if (e == null) return NotFound(); return View(e); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) { _service.Delete(id); return RedirectToAction(nameof(Index)); }
    }
}
