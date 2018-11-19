using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using BudgetApplication.Models;
using Microsoft.AspNet.Identity;

namespace BudgetApplication.Controllers
{
    //[Authorize]
    public class HouseHoldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHolds
        public IQueryable<HouseHolds> GetHouseHolds()
        {
            return db.HouseHolds;
        }

        // GET: api/HouseHolds/5
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult GetHouseHolds(int id)
        {
            HouseHolds houseHolds = db.HouseHolds.Find(id);
            if (houseHolds == null)
            {
                return NotFound();
            }

            return Ok(houseHolds);
        }

        // PUT: api/HouseHolds/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHouseHolds(int id, HouseHolds houseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var household = db.HouseHolds.Where(p => p.Id == id).FirstOrDefault();
            if (household == null)
            {
                return BadRequest("Category not found");
            }
            household.Name = houseHolds.Name;
            //db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();

        }

        // POST: api/HouseHolds
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult PostHouseHolds(HouseHolds houseHolds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var houseHold = new HouseHolds();
            houseHold.CreatorId = User.Identity.GetUserId();
            houseHold.Name = houseHolds.Name;
            db.HouseHolds.Add(houseHold);
            db.SaveChanges();
            var houseHoldViewModel = new CreateHouseHoldViewModel();
            houseHoldViewModel.Id = houseHold.Id;
            houseHoldViewModel.Name = houseHold.Name;
            return CreatedAtRoute("DefaultApi", new { id = houseHolds.Id }, houseHolds);
        }

        // DELETE: api/HouseHolds/5
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult DeleteHouseHolds(int id)
        {
            HouseHolds houseHolds = db.HouseHolds.Find(id);
            if (houseHolds == null)
            {
                return NotFound();
            }

            db.HouseHolds.Remove(houseHolds);
            db.SaveChanges();

            return Ok(houseHolds);
        }

        [HttpPost]
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult Invite(InviteUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var CreatorId = User.Identity.GetUserId();
            var household = db.HouseHolds.Where(p => p.Id == model.Id).FirstOrDefault();
            var invitedUser = db.Users.Where(p => p.Email == model.Email).FirstOrDefault();
            if (invitedUser == null && household == null)
            {
                return NotFound();
            }
            if (CreatorId == household.CreatorId)
            {
                var personalEmailService = new PersonalEmailService();
                var mailMessage = new MailMessage(WebConfigurationManager.AppSettings["emailto"], model.Email);
                mailMessage.IsBodyHtml = true;
                personalEmailService.Send(mailMessage);

                var houseHoldInvites = new HouseHoldInvites();
                houseHoldInvites.HouseHoldId = household.Id;
                houseHoldInvites.InvitedUserId = invitedUser.Id;
                db.HouseHoldInvites.Add(houseHoldInvites);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("not found");
            }
        }

        [HttpPost]
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult Join(int invitedId)
        {
            //var user = User.Identity.GetUserId();
            //var invitedUser = db.Users.Where(p => p.Email == Email).FirstOrDefault();
            //var houseHolds = new HouseHolds();
            //houseHolds.Name = invitedUser.Email;
            //houseHolds.CreatorId = User.Identity.GetUserId();
            //db.HouseHolds.Add(houseHolds);
            //db.SaveChanges();
            //return Ok();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var invite = db.HouseHoldInvites.FirstOrDefault(p => p.Id == invitedId);
            if (invite == null)
            {
                return BadRequest("Invite not found");
            }
            var userId = User.Identity.GetUserId();
            if (invite.InvitedUserId != userId)
            {
                return BadRequest("Invite not found");
            }
            var houseHold = db.HouseHolds.FirstOrDefault(p => p.Id == invite.HouseHoldId);
            var user = db.Users.FirstOrDefault(p => p.Id == userId);
            houseHold.HouseHoldUser.Add(user);
            db.HouseHoldInvites.Remove(invite);
            db.SaveChanges();
            return Ok("Invite processed sucessfully");
        }

        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult Leave(int HouseHoldId)
        {
            //var user = User.Identity.GetUserId();
            //HouseHoldInvites houseHoldInvites = db.HouseHoldInvites.Where(p => p.Id == id).FirstOrDefault();
            //db.HouseHoldInvites.Remove(houseHoldInvites);
            //db.SaveChanges();
            //return Ok();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var houseHold = db.HouseHolds.FirstOrDefault(p => p.Id == HouseHoldId);
            if (houseHold == null)
            {
                return NotFound();
            }
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(p => p.Id == userId);
            houseHold.HouseHoldUser.Remove(user);
            db.SaveChanges();
            return Ok("User has been removed from the household");
        }

        [HttpGet]
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult ViewHouseHolds()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var houseHold = db.HouseHolds.Include(p => p.Name).Include(p => p.CreatorId)
                .Select(p => new ViewHouseHoldViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    NumberOfUsers = p.HouseHoldUser.Count
                }).ToList();
            return Ok(houseHold);

        }

        [HttpGet]
        [ResponseType(typeof(HouseHolds))]
        public IHttpActionResult ViewHouseHoldPerUser(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var houseHold = db.HouseHolds.Where(p => p.Id == id)
                .Select(p => new ViewUserPerHouseHoldViewModel
                {
                    Email = p.HouseHoldInvites.Select(a => a.InvitedUser.Email).ToList()
                });
            return Ok(houseHold);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseHoldsExists(int id)
        {
            return db.HouseHolds.Count(e => e.Id == id) > 0;
        }
    }
}