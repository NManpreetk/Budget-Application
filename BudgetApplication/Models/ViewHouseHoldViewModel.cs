using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class ViewHouseHoldViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfUsers { get; set; }
        public string HouseHoldId { get; set; }
    }
}