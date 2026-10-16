using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataRepository.DataRepositoryEntities;
using DataRepository.GateWay;
using DataModel;
using ServicesClasseslibrary.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace OnlineExamSystem.Controllers
{
    public class DifficultyLevelsController : Controller
    {
       
        private readonly IDifficultyLevelsService _difficultyLevelsService;
        IDistributedCache _cache;

        public DifficultyLevelsController(IDifficultyLevelsService difficultyLevelsService,
            IDistributedCache cache)
        {

            _difficultyLevelsService = difficultyLevelsService;
            _cache = cache;
        }

        // GET: DifficultyLevels
        public async Task<IActionResult> Index()
        {
            const string key = "lookup:subjects";
            var cached = await _cache.GetStringAsync(key);
            if (cached != null) return 
                    View( JsonSerializer.Deserialize<List<DifficultyLevelsDataModel>>(cached));
            var data= _difficultyLevelsService.list();
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(data),
           new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
            return View(data);
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
        public IActionResult Create(DifficultyLevelsDataModel difficultyLevels)
        {
            if (ModelState.IsValid)
            {
                _difficultyLevelsService.Add(difficultyLevels);
                return RedirectToAction(nameof(Index));
            }
            return View(difficultyLevels);
        }

        // GET: DifficultyLevels/Edit/5
        public  IActionResult Edit(int? id)
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
        public IActionResult Edit(int id,DifficultyLevelsDataModel difficultyLevels)
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
        public IActionResult Delete(int? id)
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
        public IActionResult DeleteConfirmed(int id)
        {
            _difficultyLevelsService.Delete((int)id);
            return RedirectToAction(nameof(Index));
        }

       
    }
}
