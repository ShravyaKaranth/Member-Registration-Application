using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace MemberRegistrationFormService
{
    public class Service1 : IService1
    {

        #region Service name  : Add an employee

        public void OneUser(Employee empObj)
        {
            using (var db = new CompanyDbEntities())
            {
                db.Employees.Add(empObj);
                db.SaveChanges();
            }
        }
        #endregion

        #region Service name : Get all employee
        public List<Employee> Users()
        {
            using (var db = new CompanyDbEntities())
            {
                return db.Employees.ToList();
            }
        }
        #endregion

        #region Service name : Authenticate an employee
        public bool AuthenticateEmployee(Employee empObj)
        {
            string CS = ConfigurationManager.ConnectionStrings["CompanyDbEntities1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spAuthenticateUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramEmployeename = new SqlParameter("@EmployeeName", empObj.EmployeeName);
                SqlParameter paramPasswrd = new SqlParameter("@Passwrd", empObj.Passwrd);

                cmd.Parameters.Add(paramEmployeename);
                cmd.Parameters.Add(paramPasswrd);


                con.Open();
                int ReturnCode = (int)cmd.ExecuteScalar();
                return ReturnCode == 1;
            }
        }
        #endregion

        #region Service name : Get employee ID
        public int GetEmpId(Employee empObj)
        {
            string CS = ConfigurationManager.ConnectionStrings["CompanyDbEntities1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetEmployeeId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramEmployeename = new SqlParameter("@EmployeeName", empObj.EmployeeName);
                SqlParameter paramPasswrd = new SqlParameter("@Passwrd", empObj.Passwrd);

                cmd.Parameters.Add(paramEmployeename);
                cmd.Parameters.Add(paramPasswrd);
                con.Open();
                int empId = (int)cmd.ExecuteScalar();
                return empId;
            }
        }
        #endregion

        #region Service name : Add an applicant
        public void AddApplicant(HouseholdInfoMember memberObj)
        {

            using (var db = new CompanyDbEntities())
            {
                db.HouseholdInfoMembers.Add(memberObj);
                db.SaveChanges();
            }
        }
        #endregion

        #region Service name : List all household members
        public List<HouseholdInfoMember> members(int empId)
        {
            List<HouseholdInfoMember> membersList = new List<HouseholdInfoMember>();           
            using (var obj = new CompanyDbEntities())
            {
                membersList = obj.HouseholdInfoMembers.Where(s => s.Employeeid == empId).ToList();
            }
            return membersList;
        }
        #endregion

        #region Service name : Get application ID
        public HouseholdInfoMember GetApplicationId(HouseholdInfoMember member)
        {
            HouseholdInfoMember memberObj = new HouseholdInfoMember();
            string CS = ConfigurationManager.ConnectionStrings["CompanyDbEntities1"].ConnectionString;
            string query = "select * from HouseholdInfoMember where ApplicationID=" + member.ApplicationID + "and(FirstName='" + member.FirstName +"') or (LastName=' + member.LastName + ')";
            SqlConnection con = new SqlConnection(CS);
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            using (SqlDataReader CustomerReader = cmd.ExecuteReader())
            {
                while (CustomerReader.Read())
                {
                    memberObj.ApplicationID = Convert.ToInt16(CustomerReader["ApplicationID"]);
                    memberObj.Employeeid = Convert.ToInt16(CustomerReader["EmployeeId"]);
                    memberObj.FirstName = CustomerReader["FirstName"].ToString();
                    memberObj.MiddleInitial = CustomerReader["MiddleInitial"].ToString();
                    memberObj.LastName = CustomerReader["LastName"].ToString();
                    memberObj.Suffix = CustomerReader["Suffix"].ToString();
                    memberObj.DOB = CustomerReader["DOB"].ToString();
                    memberObj.Gender = CustomerReader["Gender"].ToString();
                    memberObj.Relationship = CustomerReader["Relationship"].ToString();
                }
            }
            return memberObj;
        }
        #endregion

        #region Service name : Add relationship details
        public void AddRelation(HouseholdInfoMember member)
        {
            string CS = ConfigurationManager.ConnectionStrings["CompanyDbEntities1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spUpdateRelationshipData", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramAppID = new SqlParameter("@appID", member.ApplicationID);
                SqlParameter paramRrelationID = new SqlParameter("@relationship", member.Relationship);
                cmd.Parameters.Add(paramAppID);
                cmd.Parameters.Add(paramRrelationID);

                con.Open();
                cmd.ExecuteScalar();
            }
        }
        #endregion

        #region Service name : Add edited applicant details
        public void AddEditedApplicant(HouseholdInfoMember member)
        {
            string CS = ConfigurationManager.ConnectionStrings["CompanyDbEntities1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spInsertUpdate", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramAppID = new SqlParameter("@appId", member.ApplicationID);
                SqlParameter paramFirstName = new SqlParameter("@firstName", member.FirstName);
                //if (member.MiddleInitial == null)
                  //  member.MiddleInitial = "NA";
                SqlParameter paramMiddleInitial = new SqlParameter("@middleInitial", member.MiddleInitial);
                SqlParameter paramLastName = new SqlParameter("@lastName", member.LastName);
                SqlParameter paramSuffix = new SqlParameter("@suffix", member.Suffix);
                SqlParameter paramDOB = new SqlParameter("@DOB", member.DOB);
                SqlParameter paramGender = new SqlParameter("@gender", member.Gender);
                SqlParameter paramRelationship = new SqlParameter("@relationship", member.Relationship);

                cmd.Parameters.Add(paramAppID);
                cmd.Parameters.Add(paramFirstName);
                cmd.Parameters.Add(paramMiddleInitial);
                cmd.Parameters.Add(paramLastName);
                cmd.Parameters.Add(paramSuffix);
                cmd.Parameters.Add(paramDOB);
                cmd.Parameters.Add(paramGender);
                cmd.Parameters.Add(paramRelationship);

                con.Open();
                cmd.ExecuteScalar();
            }           
        }
        #endregion
    }
}
