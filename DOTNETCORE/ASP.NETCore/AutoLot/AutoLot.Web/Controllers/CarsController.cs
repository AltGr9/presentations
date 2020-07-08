﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoLot.Dal.Models.Entities;
using AutoLot.Dal.Models.Exceptions;
using AutoLot.Dal.Repos.Interfaces;

namespace AutoLot.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class CarsController : Controller
    {
        private readonly ICarRepo _repo;

        public CarsController(ICarRepo repo)
        {
            _repo = repo;
        }

        // GET: Cars
        [HttpGet]
        [Route("/[controller]")]
        [Route("/[controller]/[action]")]
        public IActionResult Index() 
            => View(_repo.GetAll());

        [HttpGet("{makeId}/{makeName}")]
        [Route("/[controller]/[action]")]
        public IActionResult ByMake(int makeId, string makeName)
        {
            ViewBag.MakeName = makeName;
            return View(_repo.GetAllBy(makeId));
        }

        // GET: Cars/Details/5
        [HttpGet("{id?}")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _repo.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        internal SelectList GetMakes(IMakeRepo makeRepo)
            => new SelectList(makeRepo.GetAll(), nameof(Make.Id), nameof(Make.Name));

        // GET: Cars/Create
        [HttpGet]
        public IActionResult Create([FromServices] IMakeRepo makeRepo)
        {
            ViewData["MakeId"] = GetMakes(makeRepo);
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public async Task<IActionResult> Create([Bind("MakeId,Color,PetName,Id,TimeStamp")] Car car)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromServices] IMakeRepo makeRepo, Car car)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(car);
                return RedirectToAction(nameof(Details), new {id = car.Id});
            }

            ViewData["MakeId"] = GetMakes(makeRepo);
            return View(car);
        }
        // GET: Cars/Edit/5
        [HttpGet("{id?}")]
        public IActionResult Edit([FromServices] IMakeRepo makeRepo, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _repo.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            ViewData["MakeId"] = GetMakes(makeRepo);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromServices] IMakeRepo makeRepo, int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repo.Update(car);
                }
                catch (CustomConcurrencyException ex)
                {
                    throw;
                }

                return RedirectToAction(nameof(Details), new {id = car.Id});
            }

            ViewData["MakeId"] = GetMakes(makeRepo);
            return View(car);
        }
        //Model Binding: https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2([FromServices] IMakeRepo makeRepo, int id)
        {
            var vm = new Car();
            if (await TryUpdateModelAsync(vm,"",
                c=>c.Id,c=>c.MakeId, c=>c.TimeStamp))
            {
                //Color doesn't get updated because it's not in the list
                //c=>c.Color, 
                //Petname from the forms is ignored but hard coded later
                //c=>c.PetName, 
            }
            var valid0 = ModelState.IsValid;
            ModelState.Clear();
            vm.PetName = "Model T";
            vm.Color = "Black";
            var valid1 = TryValidateModel(vm);
            var valid2 = ModelState.IsValid;
            ViewData["MakeId"] = GetMakes(makeRepo);
            return View("Edit",vm);
        }

        // GET: Cars/Delete/5
        [HttpGet("{id?}")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _repo.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        //[ActionName("Delete")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, byte[] timeStamp)
        {
            _repo.Delete(id, timeStamp);
            return RedirectToAction(nameof(Index));
        }
    }
}