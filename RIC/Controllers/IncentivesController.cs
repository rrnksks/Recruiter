using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models;
using RIC.Utility;
namespace RIC.Controllers
{
    public class IncentivesController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Incentives/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult IncentivePartial(int Year)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //  IncentiveIndexAction incetive=new IncentiveIndexAction();

            List < GetIncentiveResult > incentiveResult = unitOfWork.Incentive.getTncetive(empCd, Year).ToList();


            var incentiveData = from incentive in unitOfWork.Incentive.Get()
                                      where incentive.RI_Year == Year && incentive.RI_EmpCd == empCd && incentive.RI_TotalRecurringPaid != null
                                      group incentive by 1 into incentiveSum
                                      select new IncentiveResultData
                                      {
                                          RI_TotalRecurringPaid = incentiveSum.Sum(s=>s.RI_TotalRecurringPaid),
                                          RI_OneTimeIncentive= incentiveSum.Sum(s => s.RI_OneTimeIncentive),
                                          RI_finalDifference= incentiveSum.Sum(s => s.RI_finalDifference)
                                      };

            incentiveResult.FirstOrDefault().IncentiveResultData = incentiveData.ToList();

            ViewBag.Year = Year;

            return PartialView(incentiveResult);
        }

        [Authorize]
        public ActionResult ViewIncentivePopup(string date, string showTotal, int Year,string getType=null)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            List<Get_All_IncentivesResult> incentives = new List<Get_All_IncentivesResult>();
            
           
            ViewBag.Header = getType == null? "Incentives for " + date.ToLower():"Incentives";
            ViewBag.IncentiveType = getType;
            int month = getType == null ? DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month:0;

            if (getType != null)
            {
                switch (getType)
                {
                    case "RecurringPaid":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_TotalRecurringPaid != null).ToList();
                        break;

                    case "OneTimeIncentive":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_OneTimeIncentive != null).ToList();
                        break;

                    case "FinalDiffrence":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_finalDifference != null).ToList();
                        break;
                }

                incentives = incentives.GroupBy(item => new { item.RI_Company, item.RI_Candidate }).Select(grouping => grouping.FirstOrDefault()).ToList();

            }
            else if (showTotal == "Starts In a Month" || showTotal == "Margin ($)")//if start of month click.
            {
                incentives = unitOfWork.Incentive.getAllIncentives().
                           Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_StartDate.Month == month && s.RI_StartDate.Year == Year).ToList();
            }
            else if (showTotal == "Ends in a Month")//if end of month is clicked
            {
                // select if end date is equal to month.
                incentives = unitOfWork.Incentive.getAllIncentives().
                   Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_EndDate != null && s.RI_EndDate.Value.Month == month && s.RI_EndDate.Value.Year == Year && s.RI_Year == Year).ToList();
            }
            else
            {
                // select if end date is equal to month.
                incentives = unitOfWork.Incentive.getAllIncentives().
                             Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_Year == Year).ToList();
            }

            return View(incentives);
        }
        
        [Authorize]
        [HttpGet]
        public ActionResult QuarterlyIncentive()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            string EmployeeName = unitOfWork.User.Get().Where(s => s.RE_Emp_Cd == empCd).Select(s => s.RE_AKA_Name).FirstOrDefault();

            IEnumerable<RIC_QuarterlyIncentive> listOfUsers = unitOfWork.QuarterlyIncentive.Get().Where(s => String.Equals(s.RI_TeamLead, EmployeeName, StringComparison.CurrentCultureIgnoreCase) || s.RI_EmpCd.Trim() == empCd).ToList();

            string IncentiveSum = listOfUsers.Sum(s => s.RI_Incentives).ToString();
            decimal parsed = decimal.Parse(IncentiveSum, CultureInfo.InvariantCulture);
            CultureInfo hindi = new CultureInfo("hi-IN");
            //string text = ;

            ViewBag.IncentivesSum = string.Format(hindi, "{0:c}", parsed).Substring(1);
            ViewBag.NetMarginSum = listOfUsers.Sum(s => s.RI_NetMargin);


            return View(listOfUsers);
        }

    }
}
