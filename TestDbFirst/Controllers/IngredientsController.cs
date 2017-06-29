using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestDbFirst;

namespace TestDbFirst.Controllers
{
    public class IngredientsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: Ingredients
        public ActionResult Index()
        {
            var ingredients = db.Ingredients.Include(i => i.SystemUser).Include(i => i.SystemUser1);
            return View(ingredients.ToList());
        }

        // GET: Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: Ingredients/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                ingredient.CreatedDate = DateTime.Now;
                db.Ingredients.Add(ingredient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.ChangedBy);
            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.ChangedBy);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                ingredient.ChangedDate = DateTime.Now;
                db.Entry(ingredient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredient.ChangedBy);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            db.Ingredients.Remove(ingredient);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
