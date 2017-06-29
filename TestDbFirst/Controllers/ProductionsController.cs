using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestDbFirst;

namespace TestDbFirst.Models
{
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
                db.Productions.Add(production);
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
