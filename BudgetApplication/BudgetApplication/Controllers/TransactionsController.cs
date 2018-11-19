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
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Transactions
        public IQueryable<Transactions> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult GetTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransactions(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = db.Transactions.Where(p => p.Id == transactions.Id).FirstOrDefault();
            if (id != transactions.Id)
            {
                return BadRequest();
            }
            transaction.AccountId = transactions.AccountId;
            transaction.category = transactions.category;
            transaction.Description = transactions.Description;
            transaction.EnteredBy = transactions.EnteredBy;
            transaction.IsVoided = transactions.IsVoided;

            db.Entry(transactions).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionsExists(id))
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

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult updateAccountPerTransaction(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts.Where(p => p.Id == transactions.Id).FirstOrDefault();
            var transaction = db.Transactions.Where(p => p.Id == id).FirstOrDefault();
            var wholeAccount = new Accounts();
            wholeAccount.Balance = account.Balance - transaction.Amount;
            account.Balance = wholeAccount.Balance;
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult VoidTransactions(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var account = db.Accounts.Where(p => p.Id == transactions.Id).FirstOrDefault();
            var transaction = db.Transactions.Where(p => p.Id == id).FirstOrDefault();
            var wholeAccount = new Accounts();
            wholeAccount.Balance = account.Balance + transaction.Amount;
            account.Balance = wholeAccount.Balance;
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult PostTransactions(Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var creatorId = User.Identity.GetUserId();
            var transaction = db.Transactions.Where(p => p.Id == transactions.Id).FirstOrDefault();
            transaction.EnteredById = creatorId;
            db.Transactions.Add(transactions);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = transactions.Id }, transactions);
        }

        [HttpPut]
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult VoidTransaction(Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult DeleteTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transactions);
            db.SaveChanges();

            return Ok(transactions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionsExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}