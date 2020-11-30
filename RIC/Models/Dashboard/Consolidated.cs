using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
	public class Consolidated
	{
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime IssuedDate { get; set; }
		public string JobDiva_Ref { get; set; }
        public string Title { get; set; }
		public string Company { get; set; }
		public int Count { get; set; }
	}

    public class DetailsByUser 
    {
        public string HeaderText { get; set; }
        public string EmpCd { get; set; }
        public string  EmployeeName { get; set; }
        public int Count { get; set; }
    
    }
}