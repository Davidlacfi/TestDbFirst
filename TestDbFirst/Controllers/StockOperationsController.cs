using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using TestDbFirst.Models;

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
            var existingIngredient = new List<Ingredient>();
            foreach (var ai in db.Ingredients)
            {
                if (db.CurrentIngredientStocks.Any(x=>x.Ingredient_Id == ai.Id))
                {
                    ai.Name = string.Format("{0} - {1} kg", ai.Name, string.Format("{0:0.##}", db.CurrentIngredientStocks.First(x => x.Ingredient_Id == ai.Id).Quantity)) ;
                    existingIngredient.Add(ai);
                };
            }
            ViewBag.Ingredient_Id = new SelectList(existingIngredient, "Id", "Name");

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

            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "correction"), "Id", "Name");
            var existingIngredient = new List<Ingredient>();
            foreach (var ai in db.Ingredients)
            {
                if (db.CurrentIngredientStocks.Any(x => x.Ingredient_Id == ai.Id))
                {
                    ai.Name = string.Format("{0} - {1} kg", ai.Name, string.Format("{0:0.##}", db.CurrentIngredientStocks.First(x => x.Ingredient_Id == ai.Id).Quantity));
                    existingIngredient.Add(ai);
                };
            }
            ViewBag.Ingredient_Id = new SelectList(existingIngredient, "Id", "Name");

            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View(ingredientMovement);
        }

        // GET AddProductCorrections
        public ActionResult AddProductCorrection()
        {
            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "correction"), "Id", "Name");
            var existingProduct = new List<Recipe>();
            foreach (var ai in db.Recipes)
            {
                if (db.CurrentProductStocks.Any(x => x.Recipe_Id == ai.Id))
                {
                    ai.Name = string.Format("{0} - {1} kg", ai.Name, string.Format("{0:0.##}", db.CurrentProductStocks.First(x => x.Recipe_Id == ai.Id).Quantity));
                    existingProduct.Add(ai);
                };
            }
            ViewBag.Recipe_Id = new SelectList(existingProduct, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }



        // POST AddProductCorrections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductCorrection([Bind(Include = "Id,Recipe_Id,MovementType_Id,Warehouse_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] StockOperationViewModel productchange)
        {
            if (ModelState.IsValid)
            {

                //PRODUCTMOVEMENT ELKÉSZÍTÉSE
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                var productMovement = new ProductMovement
                {
                    Recipe_Id = productchange.Recipe_Id,
                    MovementType_Id = productchange.MovementType_Id,
                    Warehouse_Id = productchange.Warehouse_Id,
                    Quantity = productchange.Quantity,
                    Remark = productchange.Remark,
                    IsActive = productchange.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Convert.ToInt32(sid)
                };
                db.ProductMovements.Add(productMovement);

                //CURRENTPRODUCTSTOCK MÓDOSÍTÁSA
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

            ViewBag.MovementType_Id = new SelectList(db.MovementTypes.Where(i => i.MovementKey == "correction"), "Id", "Name");
            var existingProduct = new List<Recipe>();
            foreach (var ai in db.Recipes)
            {
                if (db.CurrentProductStocks.Any(x => x.Recipe_Id == ai.Id))
                {
                    ai.Name = string.Format("{0} - {1} t", ai.Name, string.Format("{0:0.##}", db.CurrentProductStocks.First(x => x.Recipe_Id == ai.Id).Quantity));
                    existingProduct.Add(ai);
                };
            }
            ViewBag.Recipe_Id = new SelectList(existingProduct, "Id", "Name");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View(productchange);
        }
    }
}