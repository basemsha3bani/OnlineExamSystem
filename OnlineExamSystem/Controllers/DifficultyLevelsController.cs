using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataRepository.DataRepositoryEntities;
using DataRepository.GateWay;
using ServicesClasseslibrary;
using DataModel;

namespace OnlineExamSystem.Controllers
{
    public class DifficultyLevelsController : Controller
    {
        private readonly DbConext _context;
        private readonly IDifficultyLevelsService _difficultyLevelsService;

        public DifficultyLevelsController(IDifficultyLevelsService difficultyLevelsService)
        {

            _difficultyLevelsService = difficultyLevelsService;
        }

        // GET: DifficultyLevels
        public async Task<IActionResult> Index()
        {
            return View(_difficultyLevelsService.list());
        }

        // GET: DifficultyLevels/Details/5
       

        // GET: DifficultyLevels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DifficultyLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DifficultyLevelsDataModel difficultyLevels)
        {
            if (ModelState.IsValid)
            {
                _difficultyLevelsService.Add(difficultyLevels);
                return RedirectToAction(nameof(Index));
            }
            return View(difficultyLevels);
        }

        // GET: DifficultyLevels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var difficultyLevels = _difficultyLevelsService.GetById((int)id);
            if (difficultyLevels == null)
            {
                return NotFound();
            }
            return View(difficultyLevels);
        }

        // POST: DifficultyLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,DifficultyLevelsDataModel difficultyLevels)
        {
            if (id != difficultyLevels.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _difficultyLevelsService.Edit(difficultyLevels);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_difficultyLevelsService.GetById(difficultyLevels.Id)==null)
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
            return View(difficultyLevels);
        }

        // GET: DifficultyLevels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var difficultyLevels = _difficultyLevelsService.GetById((int)id);
            if (difficultyLevels == null)
            {
                return NotFound();
            }

            return View(difficultyLevels);
        }

        // POST: DifficultyLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _difficultyLevelsService.Delete((int)id);
            return RedirectToAction(nameof(Index));
        }

       
    }
}
