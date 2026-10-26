using Domain.Entities;
using Domain.Aggregates;
﻿using Domain.DTOs;
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
            return View(ExamDTO.Define());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ExamDTO model)
        {
            if (ModelState.IsValid)
            {
                try { _examService.Add(model); return RedirectToAction("Index", "ExamSections", new { examId = model.Id }); }
                catch (Domain.Exceptions.DomainException ex) { ModelState.AddModelError("", ex.Message); }
            }
            ViewBag.Subjects = new SelectList(_subjectService.list(), "Id", "SubjectName");
            return View(model);
        }
        public IActionResult Details(int id) => RedirectToAction("Index", "ExamSections", new { examId = id });
    }
}
