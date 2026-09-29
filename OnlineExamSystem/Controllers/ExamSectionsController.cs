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
    public class ExamSectionsContoller : Controller
    {
        private readonly IExamService _examService;
        private readonly IExamQuestionsService _examSectionService;
        

        public ExamSectionsContoller(IExamService examService)
        {
            _examService = examService;
           
        }

        // GET: ExamSections

        public ActionResult Create(int examId)
        {
         
            return View(new ExamSectionsDataModel { ExamId=examId});
        }

        // POST: ExamSections/Create

        [HttpPost]
        public IActionResult Create(ExamSectionsDataModel model)
        {


           var exam= _examService.GetById(model.ExamId);
            exam.Sections.Add(model);
            var total = exam.Sections.Sum(s => s.Percentage);
           

            // 4. Save if valid
            if (ModelState.IsValid)
            {
                // Set CreatedBy / Id if you need
                _examService.Edit(exam); // your method that saves Exam + Sections
                return RedirectToAction("Index","Exams");
            }

            return View(model);
        }



    }
}
