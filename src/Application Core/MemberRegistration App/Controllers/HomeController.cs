using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MemberRegistrationAppln.Models;
using Newtonsoft.Json;

namespace MemberRegistrationAppln.Controllers
{
    // Implements CRUD operations to do the necessary editing in the application; Also implements search functionality which fetches household member's application based on the Application ID.
    public static class Globals
    {
        public static List<HouseholdMember> membersList = new List<HouseholdMember>();
    }
    public class HomeController : Controller
    {

        #region LoginPage
        public ActionResult LoginPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginPage(LoginDetails obj)
        {
            TryUpdateModel(obj);
            if (ModelState.IsValid)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LoginDetails));
                string serviceurl = string.Format("http://localhost:65364/Service1.svc/AuthenticateEmployee");
                MemoryStream mem = new MemoryStream();
                ser.WriteObject(mem, obj);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                obj.authorziedEmployee = client.UploadString(serviceurl, "POST", data);
                Session["EmployeeName"] = obj.EmployeeName.ToString();
            }
            //Validate model failed
            else
            {
                return View();
            }
            //Check if Admin is logged in
            if (obj.EmployeeName.ToLower() == "admin" && obj.Passwrd == "admin")
                Session["Authorized"] = "true";
            //Check if the user has valid credentials
            else if (obj.authorziedEmployee == "true")
            {
                Session["Authorized"] = "false";
            }
            //Invalid credentials
            else
            {
                //Session["Authenticate"] = "false";
                ViewBag.isValid = "false";
                return View();
            }
            //Get Employee ID
            DataContractJsonSerializer ser2 = new DataContractJsonSerializer(typeof(LoginDetails));
            string serviceurl2 = string.Format("http://localhost:65364/Service1.svc/GetEmpId");
            MemoryStream mem2 = new MemoryStream();
            ser2.WriteObject(mem2, obj);
            string data2 = Encoding.UTF8.GetString(mem2.ToArray(), 0, (int)mem2.Length);
            WebClient client2 = new WebClient();
            client2.Headers["Content-type"] = "application/json";
            client2.Encoding = Encoding.UTF8;
            int empId = int.Parse(client2.UploadString(serviceurl2, "POST", data2));
            Session["EmployeeID"] = empId;
            return RedirectToAction("HomePage");

        }
        #endregion

        #region HomePage
        public ActionResult HomePage()
        {
            if (Session["Authorized"].ToString() == "true")
                ViewBag.flag = "true";
            return View();
        }
        /// <summary>
        /// Add Applicant 
        /// </summary>
        /// <returns></returns>
        #endregion

        #region Add Household member
        public ActionResult HouseholdInfo()
        {
            //ViewBag.EmployeeID = Session["EmployeeID"];
            return View();
        }
        [HttpPost]
        public ActionResult HouseholdInfo(HouseholdMember memberObj)
        {
            TryUpdateModel(memberObj);
            if (ModelState.IsValid)
            {
                string empId = Session["EmployeeID"].ToString();
                memberObj.Employeeid = int.Parse(empId);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(HouseholdMember));
                string serviceurl = string.Format("http://localhost:65364/Service1.svc/AddApplicant");
                MemoryStream mem = new MemoryStream();
                ser.WriteObject(mem, memberObj);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                client.UploadString(serviceurl, "POST", data);

                if (memberObj != null)
                    Globals.membersList.Add(memberObj);


                return RedirectToAction("HouseholdInfo", "Home");
            }
            else
                return View();
        }
        #endregion

        #region Relationship Page
        public ActionResult Relation()
        {
            HouseholdMember member = new HouseholdMember();
            member.MembersList = Globals.membersList;
            //ViewBag.membersList = membersList;
            return View(member);
        }
        [HttpPost]
        public ActionResult Relation(HouseholdMember memberObj)
        {
            foreach (string relation in memberObj.RelationshipMembers)
            {
                memberObj.Relationship = relation;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(HouseholdMember));
                string serviceurl = string.Format("http://localhost:65364/Service1.svc/AddRelationshipDetails");
                MemoryStream mem = new MemoryStream();
                ser.WriteObject(mem, memberObj);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                client.UploadString(serviceurl, "POST", data);
            }
            return RedirectToAction("SearchUser");
        }
        #endregion

        #region SearchUser
        public ActionResult SearchUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchUser(HouseholdMember memberObj)
        {
            WebClient Proxy1 = new WebClient();
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(string));
            Proxy1.Headers["Content-type"] = "application/json";
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializerToUpload = new DataContractJsonSerializer(typeof(HouseholdMember));
            serializerToUpload.WriteObject(ms, memberObj);
            HouseholdMember searchMember = new HouseholdMember();
            try
            {
                byte[] data = Proxy1.UploadData("http://localhost:65364/Service1.svc/SearchById", "POST", ms.ToArray());
                Stream stream = new MemoryStream(data);
                obj = new DataContractJsonSerializer(typeof(HouseholdMember));
                var resultStudent = obj.ReadObject(stream) as HouseholdMember;
                searchMember.ApplicationID = resultStudent.ApplicationID;
                searchMember.FirstName = resultStudent.FirstName;
                searchMember.MiddleInitial = resultStudent.MiddleInitial;
                searchMember.LastName = resultStudent.LastName;
                searchMember.Suffix = resultStudent.Suffix;
                searchMember.DOB = resultStudent.DOB;
                searchMember.Gender = resultStudent.Gender;
                searchMember.Relationship = resultStudent.Relationship;
            }
            catch (Exception ex)
            {
                ViewBag.criteria = false;
                return View();
            }
            return RedirectToAction("EditApplicant", searchMember);
        }
        #endregion

        #region Edit an applicant
        public ActionResult EditApplicant(HouseholdMember member)
        {
            
            return View(member);
        }
        [HttpPost]
        public ActionResult AddEditedApplicant(HouseholdMember member)
        {
            member.MiddleInitial = (member.MiddleInitial != null ? member.MiddleInitial : "NA");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(HouseholdMember));
            string serviceurl = string.Format("http://localhost:65364/Service1.svc/AddEditedApplicant");
            MemoryStream mem = new MemoryStream();
            ser.WriteObject(mem, member);
            string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            client.UploadString(serviceurl, "POST", data);
            return View("~/Views/Home/HomePage.cshtml");
        }
        #endregion
    }
}