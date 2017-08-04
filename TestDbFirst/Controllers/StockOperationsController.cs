using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class StockOperationsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();
        // GET AddIngredientCorrections
        public ActionResult AddIngredientCorrection()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i=>i.MovementKey =="correction"), "Id", "Name");
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }



        // POST AddIngredientCorrections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIngredientCorrection([Bind(Include = "Id,Ingredient_Id,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] IngredientMovement ingredientMovement)
        {
            if (ModelState.IsValid)
            {
                //INGREDIENTMOVEMENT ELKÉSZÍTÉSE
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                ingredientMovement.CreatedDate = DateTime.Now;
                ingredientMovement.CreatedBy = Convert.ToInt32(sid);
                db.IngredientMovements.Add(ingredientMovement);
                
                

                //CURRENTINGREDIENTSTOCK MÓDOSÍTÁSA
                var ingredienttoupdate = db.CurrentIngredientStocks.First(i => i.Ingredient_Id==ingredientMovement.Ingredient_Id) ;
                var originalingredientquantity = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == ingredientMovement.Ingredient_Id).Quantity;
                ingredienttoupdate.Quantity = originalingredientquantity + ingredientMovement.Quantity;
                ingredienttoupdate.ChangedDate = DateTime.Now;
                ingredienttoupdate.ChangedBy = Convert.ToInt32(sid);
                db.Entry(ingredienttoupdate).State = EntityState.Modified;


                if (db.SaveChanges() > 0)
                {
                    TempData["Operation"] = "success";
                }
                else
                {
                    TempData["Operation"] = "danger";
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name", ingredientMovement.MovementType_Id);
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", ingredientMovement.Ingredient_Id);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", ingredientMovement.Warehouse_Id);
            return View(ingredientMovement);
        }

        // GET AddProductCorrections
        public ActionResult AddProductCorrection()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "correction"), "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }



        // POST AddProductCorrections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductCorrection([Bind(Include = "Id,Recipe_Id,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] CurrentProductStock productchange)
        {
            if (ModelState.IsValid)
            {
                //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                var producttoupdate = db.CurrentProductStocks.First(i => i.Recipe_Id == productchange.Recipe_Id);
                var originalingredientquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == productchange.Recipe_Id).Quantity;
                producttoupdate.Quantity = originalingredientquantity + productchange.Quantity;
                producttoupdate.ChangedDate = DateTime.Now;
                producttoupdate.ChangedBy = Convert.ToInt32(sid);
                db.Entry(producttoupdate).State = EntityState.Modified;


                if (db.SaveChanges() > 0)
                {
                    TempData["Operation"] = "success";
                }
                else
                {
                    TempData["Operation"] = "danger";
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.MovementType_Id = new SelectList(db.MovementTypes, "Id", "Name", ingredientMovement.MovementType_Id);
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", ingredientMovement.Ingredient_Id);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", ingredientMovement.Warehouse_Id);
            return View(ingredientMovement);
        }
    }
}