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
    [Authorize]
    public class IngredientMovementsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: IngredientMovements
        public ActionResult Index()
        {
            var ingredientMovements = db.IngredientMovements.Include(i => i.Ingredient).Include(i => i.MovementType).Include(i => i.Production).Include(i => i.SystemUser).Include(i => i.SystemUser1).Include(i => i.Warehouse);
            return View(ingredientMovements.ToList());
        }

        // GET: IngredientMovements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientMovement ingredientMovement = db.IngredientMovements.Find(id);
            if (ingredientMovement == null)
            {
                return HttpNotFound();
            }
            return View(ingredientMovement);
        }

        // GET: IngredientMovements/Create
        public ActionResult Create()
        {
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name");
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name");
            ViewBag.Production_Id = new SelectList(db.Productions, "Id", "Remark");
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        // POST: IngredientMovements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Production_Id,Ingredient_Id,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] IngredientMovement ingredientMovement)
        {
            if (ModelState.IsValid)
            {
                db.IngredientMovements.Add(ingredientMovement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", ingredientMovement.Ingredient_Id);
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name", ingredientMovement.MovementType_Id);
            ViewBag.Production_Id = new SelectList(db.Productions, "Id", "Remark", ingredientMovement.Production_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", ingredientMovement.Warehouse_Id);
            return View(ingredientMovement);
        }

        // GET: IngredientMovements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientMovement ingredientMovement = db.IngredientMovements.Find(id);
            if (ingredientMovement == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", ingredientMovement.Ingredient_Id);
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name", ingredientMovement.MovementType_Id);
            ViewBag.Production_Id = new SelectList(db.Productions, "Id", "Remark", ingredientMovement.Production_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", ingredientMovement.Warehouse_Id);
            return View(ingredientMovement);
        }

        // POST: IngredientMovements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Production_Id,Ingredient_Id,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] IngredientMovement ingredientMovement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredientMovement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", ingredientMovement.Ingredient_Id);
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name", ingredientMovement.MovementType_Id);
            ViewBag.Production_Id = new SelectList(db.Productions, "Id", "Remark", ingredientMovement.Production_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", ingredientMovement.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", ingredientMovement.Warehouse_Id);
            return View(ingredientMovement);
        }

        // GET: IngredientMovements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientMovement ingredientMovement = db.IngredientMovements.Find(id);
            if (ingredientMovement == null)
            {
                return HttpNotFound();
            }
            return View(ingredientMovement);
        }

        // POST: IngredientMovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IngredientMovement ingredientMovement = db.IngredientMovements.Find(id);
            db.IngredientMovements.Remove(ingredientMovement);
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
