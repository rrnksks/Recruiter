using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlClient;


namespace DBLibrary.Repository
{
	 public class Job_ReportRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Job_Report
	 {
		  string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
		  string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
          string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
          string DirectorStaffingRoleName = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];

        UnitOfWork.UnitOfWork unitOfWork = new UnitOfWork.UnitOfWork();

          DateTime usDate = SystemClock.US_Date;
		  public Job_ReportRepository(RIC_DBEntities context)
				: base(context) // call the base constructor
		  {

		  }
		  public IEnumerable<RIC_Job_Report> GetAll()
		  {
				//get all distinct records.
				IEnumerable<RIC_Job_Report> distinctReport = context.Database.SqlQuery<RIC_Job_Report>("select * from  RIC_JobMasterView").ToList();
				return distinctReport;
		  }

		  public IEnumerable<RIC_Job_Report> Get_JobRepoartForUser(string empCd, DateTime startDt, DateTime endDate, string role = null,string dataJr="All")
		  {
              
              bool getAllrecord = false;
              if (role ==this.directorRoleName || role == this.AccMgrRoleName || role == this.HrRoleName)
                  getAllrecord = true;

				string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
				string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
				//IEnumerable<RIC_Job_Report> Job_Report = new List<RIC_Job_Report>();
                var Job_Report = context.Database.SqlQuery<RIC_Job_Report>(
                                     "[dbo].[SP_Get_JobRepoartForUser] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@DataType",
                                      new SqlParameter("StartDate", startDt),
                                      new SqlParameter("EndDate", endDate),
                                      new SqlParameter("EmpCd", empCd),
                                      new SqlParameter("GetAllRecord", getAllrecord),
                                      new SqlParameter("DataType",dataJr)
                                      ).ToList();
				return Job_Report;
		  }

		  //added by Shaurya..
		  public IEnumerable<RMS_JDIssuedDt> Get_RMS_JDIssuedDt()
		  {
				return context.Database.SqlQuery<RMS_JDIssuedDt>("select * from RMS_JDIssuedDt").ToList();
		  }

		  public IEnumerable<GetDashboardMatricsResult> Get_DashboardMatrics(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null,string getIndividualRecords=null)
		  {
				endDate = endDate.AddSeconds(-1);
				bool getAllrecord = false;

                bool individualRecords = getIndividualRecords == "Yes" ? true : false;
				if (role == directorRoleName||role==AccMgrRoleName)
					 getAllrecord = true;

				IEnumerable<GetDashboardMatricsResult> dashbordMatrix =
				  context.Database.SqlQuery<GetDashboardMatricsResult>(
                                     "[dbo].[SP_Dashboard_Metrics_Week] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth,@GetIndividualRecords",
									  new SqlParameter("StartDate", startOfMonth),
									  new SqlParameter("EndDate", endDate),
									  new SqlParameter("EmpCd", empCd),
									  new SqlParameter("GetAllRecord", getAllrecord),
									  new SqlParameter("@EndOfMonth", endOfMonth),
                                      new SqlParameter("@GetIndividualRecords", individualRecords)
									  );
				return dashbordMatrix;
		  }
         
          public IEnumerable<TeamMatrixResult> GetTeamMatrixForUser(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null)
          {
              endDate = endDate.AddSeconds(-1);
              bool getAllrecord = false;

              if (role == directorRoleName || role == AccMgrRoleName)
                  getAllrecord = true;

              IEnumerable<TeamMatrixResult> dashbordMatrix =
                context.Database.SqlQuery<TeamMatrixResult>(
                                   "[dbo].[SP_TeamMatrixForUser] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
                                    new SqlParameter("@StartDate", startOfMonth),
                                    new SqlParameter("@EndDate", endDate),
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@GetAllRecord", getAllrecord),
                                    new SqlParameter("@EndOfMonth", endOfMonth)
                                    ).ToList();
              return dashbordMatrix;
          }

        public IEnumerable<TeamMatrixResult> GetManageJobTeamMatrixForUser(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null)
        {
            endDate = endDate.AddSeconds(-1);
            bool getAllrecord = false;

            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;

            IEnumerable<TeamMatrixResult> dashbordMatrix =
              context.Database.SqlQuery<TeamMatrixResult>(
                                 "[dbo].[SP_GetManageJobsDashboard] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
                                  new SqlParameter("@StartDate", startOfMonth),
                                  new SqlParameter("@EndDate", endDate),
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@GetAllRecord", getAllrecord),
                                  new SqlParameter("@EndOfMonth", endOfMonth)
                                  ).ToList();
            return dashbordMatrix;
        }

        public RMS_SubmissionAnalysis GetSubmissionAnalysisByEnpCd(String EmpCd)
		  {
            RMS_SubmissionAnalysis RMS_SubmissionAnalysis = new DBLibrary.RMS_SubmissionAnalysis();
            RMS_SubmissionAnalysis=context.Database.SqlQuery<RMS_SubmissionAnalysis>("select * from dbo.RMS_SubmissionAnalysis")
                   .FirstOrDefault(s=>s.Emp_Cd==EmpCd);
            return RMS_SubmissionAnalysis;
        }

		  public IEnumerable<String> Get_ClientList(string empCD, DateTime startDt, DateTime endDate)
		  {
				endDate = endDate.AddSeconds(-1);
				//get the list of companys for user.
				List<string> list = this.Get_JobRepoartForUser(empCD, startDt, endDate).GroupBy(s => s.RJ_Company)
										  .Select(g => g.Key).ToList();
				return list;
		  }

          public List<OperationalDashboard_SpResult> GetOperationalSubmission(string empCd, DateTime startDate, DateTime endDate, string role = null)
          {
              //endDate = startDate.AddDays(1).AddTicks(-1);
              bool getAllrecord = false;

              if (role == directorRoleName || role == AccMgrRoleName)
                  getAllrecord = true;



              List<OperationalDashboard_SpResult> OperationalSubmission =
                context.Database.SqlQuery<OperationalDashboard_SpResult>(
                                   "[dbo].[SP_GetOperationalDashboard] @StartDt,@EndDate,@EmpCd,@GetAllRecord",
                                    new SqlParameter("@StartDt", startDate),
                                    new SqlParameter("@EndDate", endDate),
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    ).ToList();
              return OperationalSubmission;
          }

          public IEnumerable<ClientMatrixDashboard> Get_ClientDashboardMatrics(DateTime startOfMonth, DateTime endOfMonth, string empCd, DateTime TodayDate)
          {

              IEnumerable<ClientMatrixDashboard> dashbordMatrix =
                context.Database.SqlQuery<ClientMatrixDashboard>(
                                   "[dbo].[SP_ClientDashboard] @StartDate,@EndDate,@EmpCd,@TodayDate",
                                    new SqlParameter("@StartDate", startOfMonth),
                                    new SqlParameter("@EndDate", endOfMonth),
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@TodayDate", TodayDate)
                                    );
              return dashbordMatrix;
          }
          public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWise(string empCd, int Year, string role)
          {
            bool getAllrecord = false;

            if (role == directorRoleName ||  role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWise =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise] @EmpCd,@year,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@year", Year),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
              return ClientQuaterWise;
          }

          public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWiseDetails(string empCd, string Type, string Client, DateTime Fromdate, DateTime Todate,string role)
          {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWiseDetails =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise_Details] @EmpCd,@Type,@Client,@Fromdate,@Todate,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@Type", Type),
                                    new SqlParameter("@Client", Client),
                                    new SqlParameter("@Fromdate", Fromdate),
                                    new SqlParameter("@Todate", Todate),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
              return ClientQuaterWiseDetails;
          }
         public IEnumerable<Sp_GetTeamPerformanceResult> Get_TeamPerformanceResult(DateTime fromDate,DateTime toDate,string empCd,string role)
          {
              bool getAllrecord = false;

              if (role == directorRoleName || role == AccMgrRoleName)
                  getAllrecord = true;
              IEnumerable<Sp_GetTeamPerformanceResult> teamPerformanceResult =
                 context.Database.SqlQuery<Sp_GetTeamPerformanceResult>(
                                    "[dbo].[Sp_GetTeamPerformance] @FromDate,@ToDate,@EmpCd,@GetAllRecord",
                                     new SqlParameter("@FromDate", fromDate),
                                     new SqlParameter("@ToDate", toDate),
                                     new SqlParameter("@EmpCd", empCd),
                                     new SqlParameter("@GetAllRecord", getAllrecord)
                                     );
              return teamPerformanceResult;

          }

        //added by suman.

        public IEnumerable<ClientDashboardMonthlyDB> Get_ClientDashboard_Monthly(string empCd, int Year, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardMonthlyDB> ClientMonthlyWise =
              context.Database.SqlQuery<ClientDashboardMonthlyDB>(
                                 "[dbo].[SP_ClientDashboard_Monthly] @EmpCd,@year,@GetAllRecord",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@year", Year),
                                  new SqlParameter("@GetAllRecord", getAllrecord)
                                  ).ToList();
            return ClientMonthlyWise;
        }

        //added by suman.
        //to get last four quarters data for user(submissons, interviews, hires)
        public IEnumerable<ClientDashboardQuaterlyDB> GetSubmissionAnalysisByEmpCd(DateTime ReviewDate, string empCd)
        {

            IEnumerable<ClientDashboardQuaterlyDB> GetSubmissionAnalysisQuarterWise =
              context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                 "[dbo].[SP_QuarterWiseEmpData] @EmpCd,@ReviewDate",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@ReviewDate", ReviewDate)
                                  ).ToList();
            return GetSubmissionAnalysisQuarterWise;
        }

        //added by suman.
        //send a mail to employee have a discussion with hr.
        public string SendEmailHrDiscussion(string Empcd, string subject, string location, DateTime StartDate, DateTime EndDate, string Body)
        {
                        context.Database.ExecuteSqlCommand("EXEC sp_SendEmail @Empcd,@Subject,@Location,@StartDate,@EndDate,@body",
                                               new SqlParameter("@Empcd", Empcd),
                                               new SqlParameter("@Subject", subject),
                                               new SqlParameter("@Location", location),
                                               new SqlParameter("@StartDate", StartDate),
                                               new SqlParameter("@EndDate", EndDate),
                                               new SqlParameter("@body", Body)
                                               );

        return null;
        }

        public IEnumerable<RMS_JobsDB> Get_ManageJobs(string empCd)
        {

            IEnumerable<RMS_JobsDB> objDataResult =
              context.Database.SqlQuery<RMS_JobsDB>(
                                 "[dbo].[Sp_Get7DaysJobs] @Emp_Cd",
                                  new SqlParameter("@Emp_Cd", empCd)
                                  );
            return objDataResult;
        }

        public IEnumerable<RMS_JobsDB> Get7daysActiveJobs(string empCd)
        {

            IEnumerable<RMS_JobsDB> objDataResult =
              context.Database.SqlQuery<RMS_JobsDB>(
                                 "[dbo].[Sp_Get7DaysActiveJobs] @Emp_Cd",
                                  new SqlParameter("@Emp_Cd", empCd)
                                  );
            return objDataResult;
        }

    }

}
