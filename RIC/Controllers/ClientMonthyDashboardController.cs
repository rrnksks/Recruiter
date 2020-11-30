using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using OfficeOpenXml;
using RIC.Models;
using RIC.Utility;
using PagedList;
using System.Data.SqlClient;
using System.Diagnostics;
using RIC.Models.Dashboard;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using RIC.Models.Client;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;


namespace RIC.Controllers
{
    public class ClientMonthyDashboardController : Controller
    {
        //
        // GET: /ClientMonthyDashboard/
        UnitOfWork unitOfwork = new UnitOfWork();

        public ActionResult ClientMonthyDashboardDisplayView() 
        {
            return View();

        }
        DateTime usDate = SystemClock.US_Date;

        public ActionResult ClientMonthlyPartial(int month, string empCd)
        {

            //get the date of current month.
            DateTime date = SystemClock.US_Date.AddMonths(month);
            ViewBag.date = date;

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            //get the start date of month.
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime TodayDate = SystemClock.US_Date.Date;

            //get end date of month.
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var DashboardMatrics = dashboardIndex.getClientDashboard(empCd, startOfMonth, endOfMonth, TodayDate);


            //get the submission matrics 
            //jrView.JobReportWeek = DashboardMatrics.Item1;

            return PartialView(DashboardMatrics);
        }

        [HttpGet]
        public ActionResult ClientReport(string empCd)
        {

            ClientReportView view = new ClientReportView();
            view.FromDate = usDate.Date;
            view.ToDate = usDate.Date;
            ViewBag.Empcd = empCd;
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();
            List<SelectListItem> _lstClient = new List<SelectListItem>();

            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });

            
            foreach (var rule in company)// add the items in select list.
            {

                   SelectListItem selectlistitem = new SelectListItem();              
                    selectlistitem.Text = rule.RC_ClientName;
                    selectlistitem.Value = rule.RC_ClientName.ToString();
                    _lstClient.Add(selectlistitem);
                    
            }

            List<SelectListItem> _lstYear = new List<SelectListItem>();

            _lstYear.Add(new SelectListItem() { Text = "All", Value = "0" });

            var yearList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();

            foreach (var year in yearList)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = year.Text;
                selectlistitem.Value = year.Value;
                _lstYear.Add(selectlistitem);

            }

            view.YearSelectList = _lstYear;

            view.ClientList = _lstClient;
            return View(view);


        }

        [HttpPost]
        public ActionResult ClientReport(ClientReportView ClientView, string empCd, string GetDates)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;


            DateTime fromDate = ClientView.FromDate;
            DateTime todate = ClientView.ToDate.AddDays(1);
            
            //get the submissions for all users..

            bool getDateBool= (GetDates == "") ?true:false;
            var crosslist = (from Company in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                 from emp in unitOfwork.RIC_Employee.Get()
                                 where emp.RE_Resign_Date == null
                                 select new
                                 {
                                     Client = Company.RIC_Client.RC_ClientName,
                                     Empcd = emp.RE_Emp_Cd,
                                     JobDivaUserName = emp.RE_Jobdiva_User_Name
                                 }).ToList();
          

          
                var submissionsYear = unitOfwork.RIC_Job_Report.GetAll()                  
                  .Where(s => (ClientView.GetYear != 0 ? (getDateBool ==  false ? (s.RJ_Submit_Date >= fromDate && s.RJ_Submit_Date <= todate) : (s.RJ_Submit_Date.Year == ClientView.GetYear)) : (s.RJ_Submit_Date != null)))
                                   .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By })
                                   .Select(s => new
                                   {
                                       Client = s.Key.RJ_Company,
                                       Empcode = s.Key.RJ_EmpCd,
                                       EmployeeName = s.Key.RJ_Submitted_By,
                                       Submissions = s.Count()
                                   }).ToList();

                var InterviewYear = unitOfwork.RIC_Job_Report.GetAll()
                          .Where(s => (ClientView.GetYear != 0 ? (getDateBool == false ? (s.RJ_Interview_Date >= fromDate && s.RJ_Interview_Date <= todate) : (s.RJ_Interview_Date != null && s.RJ_Interview_Date.Value.Year == ClientView.GetYear)) : (s.RJ_Interview_Date != null)))
                          .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By })
                                      .Select(s => new
                                      {
                                          Client = s.Key.RJ_Company,
                                          Empcode = s.Key.RJ_EmpCd,
                                          EmployeeName = s.Key.RJ_Submitted_By,
                                          Interviews = s.Count()
                                      }).ToList();

                var HiresYear = unitOfwork.RIC_Job_Report.GetAll()
                                          .Where(s => (ClientView.GetYear != 0 ? (getDateBool == false ? (s.RJ_Hire_Date >= fromDate && s.RJ_Hire_Date <= todate) : (s.RJ_Hire_Date != null &&s.RJ_Hire_Date.Value.Year == ClientView.GetYear)) : (s.RJ_Hire_Date != null)))
               
                                   .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
                                   {
                                       Client = s.Key.RJ_Company,
                                       Empcode = s.Key.RJ_EmpCd,
                                       EmployeeName = s.Key.RJ_Submitted_By,
                                       Hires = s.Count()
                                   }).ToList();


                ClientView.FilterData = (from getData in
                                             (from data in crosslist
                                              join Sub in submissionsYear on new { a = data.Client, b = data.Empcd } equals new { a = Sub.Client, b = Sub.Empcode } into sj
                                              from sSub in sj.DefaultIfEmpty()
                                              join Int in InterviewYear on new { a = data.Client, b = data.Empcd } equals new { a = Int.Client, b = Int.Empcode } into si
                                              from sInt in si.DefaultIfEmpty()
                                              join Hire in HiresYear on new { a = data.Client, b = data.Empcd } equals new { a = Hire.Client, b = Hire.Empcode } into sh
                                              from sHir in sh.DefaultIfEmpty()
                                              where sSub != null || sInt != null || sHir != null
                                              select new ClientOperationalList()
                                              {
                                                  Client = data.Client,
                                                  EmpCd = data.Empcd,
                                                  EmployeeName = data.JobDivaUserName,
                                                  Submissions = sSub != null ? sSub.Submissions : 0,
                                                  Interviews = sInt != null ? sInt.Interviews : 0,
                                                  Hires = sHir != null ? sHir.Hires : 0,
                                                  SubByInterview = Math.Round((Convert.ToDouble(sInt != null ? sInt.Interviews : 0) / Convert.ToDouble(sSub != null ? sSub.Submissions : 1) * 100), 2),
                                                  SubByHire = Math.Round((Convert.ToDouble(sHir != null ? sHir.Hires : 0) / Convert.ToDouble(sSub != null ? sSub.Submissions : 1) * 100), 2),
                                                  InterviewByHire = Math.Round((Convert.ToDouble(sHir != null ? sHir.Hires : 0) / Convert.ToDouble(sInt != null ? sInt.Interviews : 1) * 100), 2)
                                              })
                                         where ((ClientView.ClientSelected != "All" ? ClientView.ClientSelected == getData.Client : null != getData.Client) &&
                                              (ClientView.SubSelected == ">" ? getData.Submissions >= ClientView.Submissions : getData.Submissions <= ClientView.Submissions) &&
                                              (ClientView.InterviewSelected == ">" ? getData.Interviews >= ClientView.Interviews : getData.Interviews <= ClientView.Interviews) &&
                                              (ClientView.HireSelected == ">" ? getData.Hires >= ClientView.Hires : getData.Hires <= ClientView.Hires))
                                         select getData)
                                   .ToList();

               
       
            ViewBag.ShowTable = true;


            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();
            List<SelectListItem> _lstClient = new List<SelectListItem>();
            List<SelectListItem> _lstYear = new List<SelectListItem>();


            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });


            foreach (var rule in company)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RC_ClientName;
                selectlistitem.Value = rule.RC_ClientName.ToString();
                _lstClient.Add(selectlistitem);

            }

            _lstYear.Add(new SelectListItem() { Text = "All", Value = "0" });

            var yearList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();

            foreach (var year in yearList)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = year.Text;
                selectlistitem.Value = year.Value;
                _lstYear.Add(selectlistitem);

            }

            ClientView.YearSelectList = _lstYear;

            ClientView.ClientList = _lstClient;

            return View(ClientView);
        
        }

        [HttpPost]
        public JsonResult ClientList(string Prefix, string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();

            var ClientList = (from N in company
                              where N.RC_ClientName.ToUpper().StartsWith(Prefix)
                              select new { N.RC_ClientName }).ToList();

            return Json(ClientList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ClientDetailsByQuaterly(string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            ViewBag.Empcd = empCd;
                      
            ClientReportView ClientView = new ClientReportView();

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName

                                        }).ToList();
            List<SelectListItem> _lstClient = new List<SelectListItem>();

            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });

            foreach (var rule in company)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RC_ClientName;
                selectlistitem.Value = rule.RC_ClientName.ToString();
                _lstClient.Add(selectlistitem);

            }

            ClientView.YearSelectList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();

            ClientView.ClientList = _lstClient;
            return View(ClientView);
        }

        [HttpPost]
        public ActionResult ClientDetailsByQuaterly(ClientReportView ClientView,string empCd)
        {

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            

           // ClientReportView ClientView = new ClientReportView();

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName

                                        }).ToList();
            List<SelectListItem> _lstClient = new List<SelectListItem>();

            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });

            foreach (var rule in company)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RC_ClientName;
                selectlistitem.Value = rule.RC_ClientName.ToString();
                _lstClient.Add(selectlistitem);

            }
            ClientView.YearSelectList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s=>s.Value).ToList();


            ClientView.ClientList = _lstClient;
            return View(ClientView);
         
        }


        public ActionResult ClientDetailsByQuaterlyPartial(string empCd, int Year, string Client)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientDashboardQuaterly view = new ClientDashboardQuaterly();           

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var ClientQuaterly = dashboardIndex.ClientDashboardQuaterWise(empCd, Year,role);
            var ClientMonthly = dashboardIndex.ClientDashboardMonthly(empCd, Year,role);

            var tupleData = new Tuple<List<ClientDashboardQuaterly>, List<ClientDashboardMonthly>>(ClientQuaterly,ClientMonthly);
            return PartialView("ClientDetailsByQuaterlyPartial", tupleData);

        }


        [Authorize]
        public ActionResult DetailsViewPopup(string empCd,DateTime fromDate, DateTime toDate, string data_Jr, string company)
        {
          

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            string headerText = null;
            string empTblHeaderText = null;


            toDate = toDate.AddDays(1);
            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var DashboardMatrics = dashboardIndex.ClientDashboardQuaterWiseDetails(empCd, data_Jr, company, fromDate, toDate,role);
            




            //if submission clicked.
            if (data_Jr == "Submission")
            {
                empTblHeaderText = "Submissions";
                headerText = "Submissions";// if submission clicked.
                
            }
            else if (data_Jr == "Interview")//if interview clicked.
            {
                empTblHeaderText = "Interviews";
                headerText = "Interviews";
                
            }
            else if (data_Jr == "Hire")//if hire click.
            {
                empTblHeaderText = "Hires";
                headerText = "Hires";
               
            }
           

            if (fromDate.AddDays(1) != toDate)//add the header text
                headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-1).ToString("MM-dd-yyyy");
            else
                headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");
            ViewBag.empTblHeaderText = empTblHeaderText;

            var tupleData =  new Tuple<string, List<ClientOperationalList>>(headerText, DashboardMatrics);
            return PartialView("DetailsViewPopup",tupleData);
        }


    }
}
