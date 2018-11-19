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
    public class HouseHoldInvitesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHoldInvites
        public IQueryable<HouseHoldInvites> GetHouseHoldInvites()
        {
            return db.HouseHoldInvites;
        }

        // GET: api/HouseHoldInvites/5
        [ResponseType(typeof(HouseHoldInvites))]
        public IHttpActionResult GetHouseHoldInvites(int id)
        {
            HouseHoldInvites houseHoldInvites = db.HouseHoldInvites.Find(id);
            if (houseHoldInvites == null)
            {
                return NotFound();
            }

            return Ok(houseHoldInvites);
        }

        // PUT: api/HouseHoldInvites/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHouseHoldInvites(int id, HouseHoldInvites houseHoldInvites)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != houseHoldInvites.Id)
            {
                return BadRequest();
            }

            db.Entry(houseHoldInvites).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseHoldInvitesExists(id))
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

        // POST: api/HouseHoldInvites
        [ResponseType(typeof(HouseHoldInvites))]
        public IHttpActionResult PostHouseHoldInvites(HouseHoldInvites houseHoldInvites)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HouseHoldInvites.Add(houseHoldInvites);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = houseHoldInvites.Id }, houseHoldInvites);
        }

        // DELETE: api/HouseHoldInvites/5
        [ResponseType(typeof(HouseHoldInvites))]
        public IHttpActionResult DeleteHouseHoldInvites(int id)
        {
            HouseHoldInvites houseHoldInvites = db.HouseHoldInvites.Find(id);
            if (houseHoldInvites == null)
            {
                return NotFound();
            }

            db.HouseHoldInvites.Remove(houseHoldInvites);
            db.SaveChanges();

            return Ok(houseHoldInvites);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseHoldInvitesExists(int id)
        {
            return db.HouseHoldInvites.Count(e => e.Id == id) > 0;
        }
    }
}