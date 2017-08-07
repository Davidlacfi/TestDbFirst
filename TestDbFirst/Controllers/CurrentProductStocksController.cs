using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class CurrentProductStocksController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: CurrentProductStocks
        public ActionResult Index()
        {
            var currentProductStocks = db.CurrentProductStocks.Include(c => c.Recipe).Include(c => c.SystemUser).Include(c => c.SystemUser1).Include(c => c.Warehouse);
            return View(currentProductStocks.ToList().OrderBy(c=>c.Recipe));
        }

        // GET: CurrentProductStocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentProductStock currentProductStock = db.CurrentProductStocks.Find(id);
            if (currentProductStock == null)
            {
                return HttpNotFound();
            }
            return View(currentProductStock);
        }

        // GET: CurrentProductStocks/Create
        public ActionResult Create()
        {
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name");
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        // POST: CurrentProductStocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Warehouse_Id,Recipe_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] CurrentProductStock currentProductStock)
        {
            if (ModelState.IsValid)
            {
                db.CurrentProductStocks.Add(currentProductStock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", currentProductStock.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentProductStock.Warehouse_Id);
            return View(currentProductStock);
        }

        // GET: CurrentProductStocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentProductStock currentProductStock = db.CurrentProductStocks.Find(id);
            if (currentProductStock == null)
            {
                return HttpNotFound();
            }
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", currentProductStock.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentProductStock.Warehouse_Id);
            return View(currentProductStock);
        }

        // POST: CurrentProductStocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Warehouse_Id,Recipe_Id,Quantity,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] CurrentProductStock currentProductStock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currentProductStock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Recipe_Id = new SelectList(db.Recipes, "Id", "Name", currentProductStock.Recipe_Id);
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", currentProductStock.ChangedBy);
            ViewBag.Warehouse_Id = new SelectList(db.Warehouses, "Id", "Name", currentProductStock.Warehouse_Id);
            return View(currentProductStock);
        }

        // GET: CurrentProductStocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentProductStock currentProductStock = db.CurrentProductStocks.Find(id);
            if (currentProductStock == null)
            {
                return HttpNotFound();
            }
            return View(currentProductStock);
        }

        // POST: CurrentProductStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CurrentProductStock currentProductStock = db.CurrentProductStocks.Find(id);
            db.CurrentProductStocks.Remove(currentProductStock);
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
