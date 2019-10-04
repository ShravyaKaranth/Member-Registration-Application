using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MemberRegistrationAppln.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;

namespace MemberRegistrationAppln.Controllers
{
    // An application for an employee of a company to add details of each of his/her household member information.
    public class RegisterController : Controller
    {
        #region Add new User     
        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(RegisterEmployee userObj)
        {
            TryUpdateModel(userObj);
            if (ModelState.IsValid)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RegisterEmployee));
                string serviceurl = string.Format("http://localhost:65364/Service1.svc/OneUser");
                MemoryStream mem = new MemoryStream();
                ser.WriteObject(mem, userObj);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                client.UploadString(serviceurl, "POST", data);
                return RedirectToAction("LoginPage", "Home");
            }
            else
                return View();
        }
        #endregion
    }
}