using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetApplication.Models
{
    public class HouseHoldViewModel
    {
        public HouseHoldViewModel()
        {
            Members = new List<HouseHoldMembersViewModel>();
        }
        public string Name { get; set; }
        public List<HouseHoldMembersViewModel> Members { get; set; }
    }

    public class HouseHoldMembersViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsCreator { get; set; }
    }                   
}