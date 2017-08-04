using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class ProductionsController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: Productions
        public ActionResult Index()
        {
            var productions = db.Productions.Include(p => p.Customer).Include(p => p.Recipe).Include(p => p.SystemUser).Include(p => p.SystemUser1).Include(p => p.Warehouse);
            return View(productions.ToList());
        }

        // GET: Productions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // GET: Productions/Create
        public ActionResult Create()
        {
            ViewBag.Customer_Id = new SelectList(db.Customers, "Id", "Name");
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name");
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.Destination_Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        // POST: Productions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Recipe_Id,Customer_Id,Destination_Warehouse_Id,ProductionDate,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Production production)
        {
            if (ModelState.IsValid)
            {
                // PRODUCTION HOZZÁADÁSA
                // INGREDIENTMOVEMENT - CSAK GYÁRTÁS
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                production.CreatedDate = DateTime.Now;
                production.CreatedBy = Convert.ToInt32(sid);
                db.Productions.Add(production);

                //INGREDIENTMOVEMENT TÖLTÉSE
                var movement = db.MovementTypes.FirstOrDefault(i => i.MovementKey == "production");
                var loss = db.MovementTypes.FirstOrDefault(i => i.MovementKey == "loss");
                foreach (var ri in db.RecipeIngredients.Where(i => i.Recipe_Id == production.Recipe_Id))
                {
                    db.IngredientMovements.Add(new IngredientMovement()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                        Ingredient = ri.Ingredient,
                        Production = production,
                        MovementType =movement,
                        Warehouse_Id = production.Destination_Warehouse_Id,
                        Quantity = production.Quantity*(-1*ri.Ammount),
                        IsActive = true
                    });
                    db.IngredientMovements.Add(new IngredientMovement()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                        Ingredient = ri.Ingredient,
                        Production = production,
                        MovementType = loss,
                        Warehouse_Id = production.Destination_Warehouse_Id,
                        Quantity = production.Quantity * (-1 * ri.Ammount) * (decimal)0.006,
                        IsActive = true
                    });
                }

                // CURRENTPRODUCTSTOCK TÖLTÉSE
                var finalproductexists = db.CurrentProductStocks.Where(i => i.Recipe_Id == production.Recipe_Id);

                if (!finalproductexists.Any())
                {
                    //HA NINCS RAKTÁRBAN ILYEN TERMÉK, AKKOR ÚJAT HOZ LÉTRE
                    db.CurrentProductStocks.Add(new CurrentProductStock()
                    {
                        CreatedDate = DateTime.Now,
                        CreatedBy = Convert.ToInt32(sid),
                        IsActive = true,
                        Warehouse_Id = production.Destination_Warehouse_Id,
                        Recipe_Id = production.Recipe_Id,
                        Quantity = production.Quantity

                    });
                }

                else
                {
                    //HA VAN RAKTÁRBAN ILYEN TERMÉK, AKKOR UPDATEEL
                    var finalproduct = db.CurrentProductStocks.First(i => i.Recipe_Id == production.Recipe_Id);
                    var originalquantity = db.CurrentProductStocks.First(i => i.Recipe_Id == production.Recipe_Id).Quantity;
                    finalproduct.Quantity = originalquantity + production.Quantity;
                    finalproduct.ChangedDate = DateTime.Now;
                    finalproduct.ChangedBy = Convert.ToInt32(sid);
                    db.Entry(finalproduct).State = EntityState.Modified;

                }

                // CURRENTINGREDIENTSTOCK TÖLTÉSE - GYÁRTÁS + VESZTESÉG EGYBEN
                foreach (var ri in db.RecipeIngredients.Where(i => i.Recipe_Id == production.Recipe_Id))
                {
                    var ingredienttoupdate = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == ri.Ingredient_Id);
                    var originalingredientquantity = db.CurrentIngredientStocks.First(i => i.Ingredient_Id == ri.Ingredient_Id).Quantity;
                    ingredienttoupdate.Quantity = originalingredientquantity - (production.Quantity * ri.Ammount) - (production.Quantity * ri.Ammount * (decimal)0.006);
                    ingredienttoupdate.ChangedDate = DateTime.Now;
                    ingredienttoupdate.ChangedBy = Convert.ToInt32(sid);
                    db.Entry(ingredienttoupdate).State = EntityState.Modified;

                }


                if (db.SaveChanges()>0)
                {
                    TempData["Operation"] = "success";
                }
                else
                {
                    TempData["Operation"] = "danger";
                }
                return RedirectToAction("Index");
            }

            ViewBag.Customer_Id = new SelectList(db.Customers, "Id", "Name", production.Customer_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", production.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", production.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", production.ChangedBy);
            ViewBag.Destination_Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", production.Destination_Warehouse_Id);
            return View(production);
        }

        // GET: Productions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            ViewBag.Customer_Id = new SelectList(db.Customers, "Id", "Name", production.Customer_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", production.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", production.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", production.ChangedBy);
            ViewBag.Destination_Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", production.Destination_Warehouse_Id);
            return View(production);
        }

        // POST: Productions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Recipe_Id,Customer_Id,Destination_Warehouse_Id,ProductionDate,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Production production)
        {
            if (ModelState.IsValid)
            {
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Customer_Id = new SelectList(db.Customers, "Id", "Name", production.Customer_Id);
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", production.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", production.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", production.ChangedBy);
            ViewBag.Destination_Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", production.Destination_Warehouse_Id);
            return View(production);
        }

        // GET: Productions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // POST: Productions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Production production = db.Productions.Find(id);
            db.Productions.Remove(production);
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
