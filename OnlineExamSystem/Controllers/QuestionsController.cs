using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataRepository.DataRepositoryEntities;
using DataRepository.GateWay;
using ServicesClasseslibrary.Interface;
using DataModel;
using ServicesClasseslibrary;

namespace OnlineExamSystem.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionsService _questionsService;
        private readonly IDifficultyLevelsService _difficultyLevelsService;

        public QuestionsController(IQuestionsService questionsService, IDifficultyLevelsService difficultyLevelsService)
        {
            _questionsService = questionsService;
            _difficultyLevelsService = difficultyLevelsService;
        }

        // GET: Questions
        public IActionResult Index()
        {
           
            return View(_questionsService.list());
        }

        // GET: Questions/Details/5
       

        // GET: Questions/Create
        public IActionResult Create()
        {
            var x = new List<QuestionAnswersDataModel>(5) {
            new QuestionAnswersDataModel(),new QuestionAnswersDataModel(),new QuestionAnswersDataModel(),new QuestionAnswersDataModel(),new QuestionAnswersDataModel(), };

            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            
            return View(new QuestionsDataModel { QuestionAnswersDataModel=x });
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuestionsDataModel questions)
        {
            if (ModelState.IsValid)
            {
                _questionsService.Add(questions);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            return View(questions);
        }

        // GET: Questions/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = _questionsService.GetById((int) id);
            if (questions == null)
            {
                return NotFound();
            }
            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            return View(questions);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,QuestionText,DifficultyLevelId")] QuestionsDataModel questions)
        {
            if (id != questions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _questionsService.Edit(questions);
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_questionsService.GetById(questions.Id)==null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName", questions.DifficultyLevelId);
            return View(questions);
        }

        // GET: Questions/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = _questionsService.GetById((int)id);
            if (questions == null)
            {
                return NotFound();
            }

            return View(questions);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var questions = _questionsService.GetById(id);
            _questionsService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        
    }
}
