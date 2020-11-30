using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class ClientDashboardMonthlyDB
    {
        public string RJ_Company { get; set; }

        public int Monthnum { get; set; }

        public int RequirementsCount { get; set; }

        public int SubmissonCount { get; set; }

        public int InterviewCount { get; set; }

        public int HireCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string MonthName { get; set; }

       
    }
}
