using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BudgetApplication.Models;

namespace BudgetApplication.Controllers
{
    public class ApplicationUserHouseHoldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ApplicationUserHouseHolds
        public IQueryable<ApplicationUserHouseHolds> GetApplicationUserHouseHolds()
        {
            return db.ApplicationUserHouseHolds;
        }

        // GET: api/ApplicationUserHouseHolds/5
        [ResponseType(typeof(ApplicationUserHouseHolds))]
        public IHttpActionResult GetApplicationUserHouseHolds(int id)
        {
            ApplicationUserHouseHolds applicationUserHouseHolds = db.ApplicationUserHouseHolds.Find(id);
            if (applicationUserHouseHolds == null)
            {
                return NotFound();
            }

            return Ok(applicationUserHouseHolds);
        }

        // PUT: api/ApplicationUserHouseHolds/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutApplicationUserHouseHolds(int id, ApplicationUserHouseHolds applicationUserHouseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != applicationUserHouseHolds.Id)
            {
                return BadRequest();
            }

            db.Entry(applicationUserHouseHolds).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationUserHouseHoldsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ApplicationUserHouseHolds
        [ResponseType(typeof(ApplicationUserHouseHolds))]
        public IHttpActionResult PostApplicationUserHouseHolds(ApplicationUserHouseHolds applicationUserHouseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ApplicationUserHouseHolds.Add(applicationUserHouseHolds);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = applicationUserHouseHolds.Id }, applicationUserHouseHolds);
        }

        // DELETE: api/ApplicationUserHouseHolds/5
        [ResponseType(typeof(ApplicationUserHouseHolds))]
        public IHttpActionResult DeleteApplicationUserHouseHolds(int id)
        {
            ApplicationUserHouseHolds applicationUserHouseHolds = db.ApplicationUserHouseHolds.Find(id);
            if (applicationUserHouseHolds == null)
            {
                return NotFound();
            }

            db.ApplicationUserHouseHolds.Remove(applicationUserHouseHolds);
            db.SaveChanges();

            return Ok(applicationUserHouseHolds);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationUserHouseHoldsExists(int id)
        {
            return db.ApplicationUserHouseHolds.Count(e => e.Id == id) > 0;
        }
    }
}