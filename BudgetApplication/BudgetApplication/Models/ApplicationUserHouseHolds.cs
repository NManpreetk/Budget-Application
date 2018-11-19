using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class ApplicationUserHouseHolds
    {
        public int Id { get; set; }
        public string ApplicationUser_Id { get; set; }
        public virtual ApplicationUserHouseHolds ApplicationUserHouseHold { get; set; }
        public string HouseHold_Id { get; set; }
        public virtual HouseHolds HouseHold { get; set; }

        public ApplicationUserHouseHolds()
        {
            HouseHolds = new HashSet<HouseHolds>();
        }

        public virtual ICollection<HouseHolds> HouseHolds { get; set; }
    }
}