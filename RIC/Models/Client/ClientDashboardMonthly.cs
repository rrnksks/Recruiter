using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class ClientDashboardMonthly
    {
        public int RequirementsCount { get; set; }

        public int SubmissonCount { get; set; }

        public int InterviewCount { get; set; }

        public int HireCount { get; set; }

        public int Monthnum { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string MonthName { get; set; }

        public double SubByInterview { get; set; }

        public double SubByHire { get; set; }

        public double InterviewByHire { get; set; }

        public string RJ_Company { get; set; }

    }
}