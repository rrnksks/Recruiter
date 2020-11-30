using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Client
{
    public class ClientDashboardQuaterly
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

        public double Q1InterviewByHire { get; set; }
        public double Q2InterviewByHire { get; set; }
        public double Q3InterviewByHire { get; set; }
        public double Q4InterviewByHire { get; set; }

        public double Q1SubByInterview { get; set; }
        public double Q2SubByInterview { get; set; }
        public double Q3SubByInterview { get; set; }
        public double Q4SubByInterview { get; set; }

        public double Q1SubByHire { get; set; }
        public double Q2SubByHire { get; set; }
        public double Q3SubByHire { get; set; }
        public double Q4SubByHire { get; set; }

        public string Quarter1 { get; set; }
        public string Quarter2 { get; set; }
        public string Quarter3 { get; set; }
        public string Quarter4 { get; set; }


        public string EmpCd { get; set; }
        public List<SelectListItem> ClientList { get; set; }

        [Display(Name = "Requirements")]
        public int Requirements { get; set; }

        [Display(Name = "Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }


        [Display(Name = "Hires")]
        public int Hires { get; set; }
        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        public string ClientSelected { get; set; }
        public int GetYear { get; set; }

        [Display(Name = "I/S Ratio")]
        public double SubByInterview { get; set; }

        [Display(Name = "H/S Ratio")]
        public double SubByHire { get; set; }

        [Display(Name = "H/I Ratio")]
        public double InterviewByHire { get; set; }

        public DateTime Q1StartDate { get; set; }

        public DateTime Q1EndDate { get; set; }

        public DateTime Q2StartDate { get; set; }

        public DateTime Q2EndDate { get; set; }    
        
        public DateTime Q3StartDate { get; set; }

        public DateTime Q3EndDate { get; set; }    
        
        public DateTime Q4StartDate { get; set; }

        public DateTime Q4EndDate { get; set; }

        public int TotalSubmissons { get; set; }

        public int TotalInterviews { get; set; }

        public int TotalHires { get; set; }

        public int TotalRequirements { get; set; }

        [Display(Name = "Company")]
        public string RJ_Company { get; set; }

        public double TotalSubByInterview { get; set; }
        public double TotalSubByHire { get; set; }
        public double TotalInterviewByHire { get; set; }
       



    }

}