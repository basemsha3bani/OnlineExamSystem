using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.GateWay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamSystem.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionsService _questionsService;
        private readonly IDifficultyLevelsService _difficultyLevelsService;
        private readonly IStudySubjectsService _studySubjectsService;
        

        public QuestionsController(IQuestionsService questionsService, IDifficultyLevelsService difficultyLevelsService,
            IStudySubjectsService studySubjectsService )
        {
            _questionsService = questionsService;
            _difficultyLevelsService = difficultyLevelsService;
            _studySubjectsService = studySubjectsService;
        }

        // GET: Questions
        public IActionResult Index(int? studySubjectId, int? chapterNumber, int page = 1, int pageSize = 20)
        {
            var result = _questionsService.Search(new QuestionSearch(studySubjectId, chapterNumber, page, pageSize));
            ModelState.Clear(); // Render the normalized filter and page-size values.
            var subjects = _studySubjectsService.list();
            var chapterCount = result.Search.StudySubjectId.HasValue
                ? subjects.FirstOrDefault(s => s.Id == result.Search.StudySubjectId)?.NumberOfChapters ?? 0
                : subjects.Select(s => s.NumberOfChapters).DefaultIfEmpty(0).Max();
            return View(new OnlineExamSystem.Models.QuestionListViewModel {
                Results = result, Subjects = subjects, Chapters = Enumerable.Range(1, chapterCount).Union(result.Search.ChapterNumber.HasValue ? new[] { result.Search.ChapterNumber.Value } : System.Array.Empty<int>()).OrderBy(c => c)
            });
        }

        // GET: Questions/Details/5
       

        // GET: Questions/Create
        public IActionResult Create()
        {
            var x = new List<QuestionAnswersDataModel>(4) {
            new QuestionAnswersDataModel(),new QuestionAnswersDataModel(),new QuestionAnswersDataModel(),new QuestionAnswersDataModel() };

            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            ViewData["StudySubjectsId"] = new SelectList(_studySubjectsService.list(), "Id", "SubjectName");
            return View(new QuestionsDataModel { QuestionAnswersDataModel=x });
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuestionsDataModel questions,int  CorrectAnswerIndex)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < questions.QuestionAnswersDataModel.Count; i++)
                {
                    questions.QuestionAnswersDataModel[i].IsCorrect = (i == CorrectAnswerIndex);
                }
                try { _questionsService.Add(questions); return RedirectToAction(nameof(Index)); }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
            }
            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName");
            ViewData["StudySubjectsId"] = new SelectList(_studySubjectsService.list(), "Id", "SubjectName");
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
            ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName",questions.DifficultyLevelId);
            ViewData["StudySubjectsId"] = new SelectList(_studySubjectsService.list(), "Id", "SubjectName",questions.StudySubjectId);
            return View(questions);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, QuestionsDataModel questions, int CorrectAnswerIndex)
        {
            if (id != questions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    for (int i = 0; i < questions.QuestionAnswersDataModel.Count; i++)
                    {
                        questions.QuestionAnswersDataModel[i].IsCorrect = (i == CorrectAnswerIndex);
                    }
                    _questionsService.Edit(questions);
                  
                }
                catch (Domain.Exceptions.DomainException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewData["StudySubjectsId"] = new SelectList(_studySubjectsService.list(), "Id", "SubjectName", questions.StudySubjectId);
                    ViewData["DifficultyLevelId"] = new SelectList(_difficultyLevelsService.list(), "Id", "DifficultyLevelName", questions.DifficultyLevelId);
                    return View(questions);
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
