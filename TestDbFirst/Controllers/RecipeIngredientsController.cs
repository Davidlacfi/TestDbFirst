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
    public class RecipeIngredientsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        //GET Index
        public ActionResult Index()
        {
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name");
            //var recipeIngredients = db.RecipeIngredients.Include(r => r.Recipe);
            //return View(recipeIngredients.ToList());
            return View();
        }


        //POST Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RecipeIngredient recipeIngredient)
        {
            return RedirectToRoute(new
            {
                controller = "RecipeIngredients",
                action = "List",
                id = recipeIngredient.Recipe_Id
            });
        }



        // GET: RecipeIngredients
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var recipeIngredient = db.RecipeIngredients.Where(i=>i.Recipe_Id==id);

            if (!recipeIngredient.Any())
            {
                return RedirectToRoute(new
                {
                    controller = "RecipeIngredients",
                    action = "Create",
                    id = id
                });
            }

            var recipeIngredients = db.RecipeIngredients.Include(r => r.Ingredient).Include(r => r.Recipe).Include(r => r.SystemUser).Include(r => r.SystemUser1)
                .Where(i => i.Recipe_Id==id);
            return View(recipeIngredients.ToList());
        }

        // GET: RecipeIngredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Find(id);
            if (recipeIngredient == null)
            {
                return HttpNotFound();
            }
            return View(recipeIngredient);
        }

        // GET: RecipeIngredients/Create
        public ActionResult Create(int? id)
        {

            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes.Where(i =>i.Id==id), "Id", "Name");
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            return View();
        }

        // POST: RecipeIngredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Recipe_Id,Ingredient_Id,Ammount,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] RecipeIngredient recipeIngredient)
        {
            if (ModelState.IsValid)
            {
                db.RecipeIngredients.Add(recipeIngredient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", recipeIngredient.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", recipeIngredient.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.ChangedBy);
            return View(recipeIngredient);
        }

        // GET: RecipeIngredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Find(id);
            if (recipeIngredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", recipeIngredient.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", recipeIngredient.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.ChangedBy);
            return View(recipeIngredient);
        }

        // POST: RecipeIngredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Recipe_Id,Ingredient_Id,Ammount,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] RecipeIngredient recipeIngredient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipeIngredient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", recipeIngredient.Ingredient_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", recipeIngredient.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", recipeIngredient.ChangedBy);
            return View(recipeIngredient);
        }

        // GET: RecipeIngredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Find(id);
            if (recipeIngredient == null)
            {
                return HttpNotFound();
            }
            return View(recipeIngredient);
        }

        // POST: RecipeIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Find(id);
            db.RecipeIngredients.Remove(recipeIngredient);
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
