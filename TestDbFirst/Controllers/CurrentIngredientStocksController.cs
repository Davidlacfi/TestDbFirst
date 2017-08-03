using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class CurrentIngredientStocksController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: CurrentIngredientStocks
        public ActionResult Index()
        {
            var currentIngredientStocks = db.CurrentIngredientStocks.Include(c => c.Ingredient).Include(c => c.SystemUser).Include(c => c.SystemUser1).Include(c => c.Warehouse);
            return View(currentIngredientStocks.ToList());
        }

        // GET: CurrentIngredientStocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentIngredientStock currentIngredientStock = db.CurrentIngredientStocks.Find(id);
            if (currentIngredientStock == null)
            {
                return HttpNotFound();
            }
            return View(currentIngredientStock);
        }

        // GET: CurrentIngredientStocks/Create
        public ActionResult Create()
        {
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name");
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        // POST: CurrentIngredientStocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Warehouse_Id,Ingredient_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] CurrentIngredientStock currentIngredientStock)
        {
            if (ModelState.IsValid)
            {
                db.CurrentIngredientStocks.Add(currentIngredientStock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", currentIngredientStock.Ingredient_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentIngredientStock.Warehouse_Id);
            return View(currentIngredientStock);
        }

        // GET: CurrentIngredientStocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentIngredientStock currentIngredientStock = db.CurrentIngredientStocks.Find(id);
            if (currentIngredientStock == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", currentIngredientStock.Ingredient_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentIngredientStock.Warehouse_Id);
            return View(currentIngredientStock);
        }

        // POST: CurrentIngredientStocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Warehouse_Id,Ingredient_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] CurrentIngredientStock currentIngredientStock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currentIngredientStock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ingredient_Id = new SelectList(db.Ingredients, "Id", "Name", currentIngredientStock.Ingredient_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentIngredientStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentIngredientStock.Warehouse_Id);
            return View(currentIngredientStock);
        }

        // GET: CurrentIngredientStocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentIngredientStock currentIngredientStock = db.CurrentIngredientStocks.Find(id);
            if (currentIngredientStock == null)
            {
                return HttpNotFound();
            }
            return View(currentIngredientStock);
        }

        // POST: CurrentIngredientStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CurrentIngredientStock currentIngredientStock = db.CurrentIngredientStocks.Find(id);
            db.CurrentIngredientStocks.Remove(currentIngredientStock);
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
