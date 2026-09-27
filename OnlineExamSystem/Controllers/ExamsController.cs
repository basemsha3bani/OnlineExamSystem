using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface.DataModel;
using System.Collections.Generic;
using System.Linq;

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
            ViewBag.Subjects = new SelectList(_subjectService.list(), "Id", "SubjectName");
            var model = new ExamDataModel
            {
                Sections = new List<ExamSectionsDataModel> { new ExamSectionsDataModel(), new ExamSectionsDataModel() }
            }; // 2 empty rows
            return View(model);
        }
       
        [HttpPost]
        public IActionResult Create(ExamDataModel model)
        {
            ViewBag.Subjects = new SelectList(_subjectService.list(), "Id", "SubjectName");

            // 1. Validate sections exist
            if (model.Sections == null || model.Sections.Count == 0)
            {
                ModelState.AddModelError("", "At least one section is required");
                return View(model);
            }

            // 2. Remove empty sections (user left blank row)
            model.Sections = model.Sections
               .Where(s => !string.IsNullOrWhiteSpace(s.SectionName))
               .ToList();

            // 3. Validate total = 100%
            var total = model.Sections.Sum(s => s.Percentage);
            if (total != 100)
            {
                ModelState.AddModelError("", $"Sections total must be 100%, currently {total}%");
                return View(model);
            }

            // 4. Save if valid
            if (ModelState.IsValid)
            {
                // Set CreatedBy / Id if you need
                _examService.Add(model); // your method that saves Exam + Sections
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var exam = _examService.GetById(id);
            if (exam == null) return NotFound();

           
           

           
            return View(exam);
        }
    }

    
}
