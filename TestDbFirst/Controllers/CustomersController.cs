using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();
        [Authorize]
        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.SystemUser).Include(c => c.SystemUser1);
            //return View(customers.Where(c => c.IsActive).ToList());
            return View(customers.ToList());
        }

        [Authorize]
        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [Authorize]
        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            return View();
        }

        [Authorize]
        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsSupplier,ZipCode,City,StreetAddress,ContactPerson1,Telephone1,ContactPerson2,Telephone2,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {

                customer.CreatedDate = DateTime.Now;
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                customer.CreatedBy = Convert.ToInt32(sid);
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.ChangedBy);
            return View(customer);
        }
        [Authorize]
        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.ChangedBy);
            return View(customer);
        }
        [Authorize]
        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsSupplier,ZipCode,City,StreetAddress,ContactPerson1,Telephone1,ContactPerson2,Telephone2,Remark,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.ChangedDate = DateTime.Now;
                var identity = (ClaimsIdentity)User.Identity;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                customer.ChangedBy = Convert.ToInt32(sid);
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", customer.ChangedBy);
            return View(customer);
        }
        [Authorize]
        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        [Authorize]
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            customer.IsActive= false;
            customer.ChangedDate = DateTime.Now;
            var identity = (ClaimsIdentity)User.Identity;
            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            customer.ChangedBy = Convert.ToInt32(sid);
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
