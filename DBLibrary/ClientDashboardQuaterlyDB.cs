using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class ClientDashboardQuaterlyDB
    {
        public int Q1Requirements { get; set; }
        public int Q2Requirements { get; set; }
        public int Q3Requirements { get; set; }
        public int Q4Requirements { get; set; }
     
        public int Q1Submissions { get; set; }
        public int Q2Submissions { get; set; }
        public int Q3Submissions { get; set; }
        public int Q4Submissions { get; set; }

        public int Q1Interviews { get; set; }
        public int Q2Interviews { get; set; }
        public int Q3Interviews { get; set; }
        public int Q4Interviews { get; set; }

        public int Q1Hires { get; set; }
        public int Q2Hires { get; set; }
        public int Q3Hires { get; set; }
        public int Q4Hires { get; set; }

        public string Quarter1 { get; set; }
        public string Quarter2 { get; set; }
        public string Quarter3 { get; set; }
        public string Quarter4 { get; set; }


        public string RJ_EmpCd { get; set; }
        public string RJ_Submitted_By { get; set; }
        public int count { get; set; }
        public string RJ_Company { get; set; }

        public int TotalSubmissons { get; set; }
        public int TotalInterviews { get; set; }
        public int TotalHires { get; set; }
        public int TotalRequirements { get; set; }

    }


}
