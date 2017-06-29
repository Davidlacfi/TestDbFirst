﻿using System;
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
    public class SystemUsersController : Controller
    {
        private MecsekTransitEntities db = new MecsekTransitEntities();

        // GET: SystemUsers
        public ActionResult Index()
        {
            var systemUsers = db.SystemUsers.Include(s => s.SystemUser2).Include(s => s.SystemUser3);
            return View(systemUsers.ToList());
        }

        // GET: SystemUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            return View(systemUser);
        }

        // GET: SystemUsers/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email");
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email");
            return View();
        }

        // POST: SystemUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Name,Role,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] SystemUser systemUser)
        {
            if (ModelState.IsValid)
            {
                systemUser.CreatedDate = DateTime.Now;
                db.SystemUsers.Add(systemUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.ChangedBy);
            return View(systemUser);
        }

        // GET: SystemUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.ChangedBy);
            return View(systemUser);
        }

        // POST: SystemUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Name,Role,IsActive,CreatedBy,CreatedDate,ChangedBy,ChangedDate")] SystemUser systemUser)
        {
            if (ModelState.IsValid)
            {
                systemUser.ChangedDate = DateTime.Now;
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.CreatedBy);
            ViewBag.ChangedBy = new SelectList(db.SystemUsers, "Id", "Email", systemUser.ChangedBy);
            return View(systemUser);
        }

        // GET: SystemUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            return View(systemUser);
        }

        // POST: SystemUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemUser systemUser = db.SystemUsers.Find(id);
            db.SystemUsers.Remove(systemUser);
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
