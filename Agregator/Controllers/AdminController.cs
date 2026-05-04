using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Agregator.Controllers
{
    using Models;

    [Authorize]	
    public class AdminController : Controller
    {

        public ActionResult Grid() => View();
        public ActionResult Positions() => View();
        public ActionResult Works() => View();
        public ActionResult DevelopmentPlan() => View();


        public ActionResult Test()
        {
            return Content("Hello");
        }


        // GET: AllUsersController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AllUsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AllUsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AllUsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AllUsersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AllUsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AllUsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AllUsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
