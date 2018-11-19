using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class Accounts
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        public virtual HouseHolds HouseHold { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public Accounts()
        {
            Transactions = new HashSet<Transactions>();

        }
        public virtual ICollection<Transactions> Transactions { get; set; }

    }
}