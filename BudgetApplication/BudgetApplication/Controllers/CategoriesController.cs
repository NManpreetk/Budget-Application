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
using Microsoft.AspNet.Identity;

namespace BudgetApplication.Controllers
{
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Categories
        public IQueryable<Categories> GetCategories()
        {
            return db.Categories;
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Categories))]
        public IHttpActionResult GetCategories(int id)
        {
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategories(int id, Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = db.Categories.Where(p => p.Id == categories.Id).FirstOrDefault();
            if (id != categories.Id)
            {
                return BadRequest();
            }
            category.Name = categories.Name;
            category.HouseHold = categories.HouseHold;
            //db.Entry(categories).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriesExists(id))
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

        // POST: api/Categories
        [ResponseType(typeof(Categories))]
        public IHttpActionResult PostCategories(Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var creatorId = User.Identity.GetUserId();
            var household = db.Categories.Where(p => p.HouseHoldId == categories.HouseHoldId).FirstOrDefault();
            db.Categories.Add(categories);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = categories.Id }, categories);
        }

        [HttpGet]
        [ResponseType(typeof(Categories))]
        public IHttpActionResult ViewCategories(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = db.Categories
                .Include(p => p.HouseHold)
                .FirstOrDefault(p => p.Id == id);

            var household = db.HouseHolds.Where(p => p.Id== category.HouseHoldId).FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }
            var userId = User.Identity.GetUserId();
            var categoryViewModel = new CategoryViewModel();
            categoryViewModel.Id = category.Id;
            categoryViewModel.Name = category.Name;
            categoryViewModel.HouseHoldName = household.Name;
            return Ok(categoryViewModel);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Categories))]
        public IHttpActionResult DeleteCategories(int id)
        {
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }

            db.Categories.Remove(categories);
            db.SaveChanges();

            return Ok(categories);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoriesExists(int id)
        {
            return db.Categories.Count(e => e.Id == id) > 0;
        }
    }
}