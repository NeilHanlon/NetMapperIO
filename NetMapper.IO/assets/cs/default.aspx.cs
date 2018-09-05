using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetMapper.IO.assets.cs
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<String, String> returnPairs = new Dictionary<string, string>();
            try
            {
                if (Request.Form["reqType"]==null || Request.Form["reqType"]==String.Empty)
                {
                    returnPairs.Add("Status", "Error");
                    returnPairs.Add("Exception", "Please define your operation");
                }
                else
                {
                    String ReqType = Request.Form["reqType"];
                    if (ReqType == "userLogin")
                    {
                        if (Request.Form["username"] == null || Request.Form["username"] == String.Empty)
                            throw new Exception("Please input a username.");
                        if (Request.Form["password"] == null || Request.Form["password"] == String.Empty)
                            throw new Exception("Please input a password.");
                        UserObject myUser = new UserObject();
                        myUser.selectByUsername(Request.Form["username"], Request.Form["password"]);
                        Session["userObject"] = myUser;
                        Session.Timeout = 30;
                        returnPairs.Add("Status", "Completed");
                        returnPairs.Add("Redirect", "/_/App");
                    }
                    if (ReqType == "userSignup")
                    {
                        if (Request.Form["forename"] == null || Request.Form["forename"] == String.Empty)
                            throw new Exception("Please enter your forename");
                        if (Request.Form["surname"] == null || Request.Form["surname"] == String.Empty)
                            throw new Exception("Please enter your surname");
                        if (Request.Form["username"] == null || Request.Form["username"] == String.Empty)
                            throw new Exception("Please enter your username");
                        if (Request.Form["email"] == null || Request.Form["email"] == String.Empty)
                            throw new Exception("Please enter your email");
                        if (Request.Form["password1"] == null || Request.Form["password1"] == String.Empty)
                            throw new Exception("Please enter a password");
                        if (Request.Form["password1"] != Request.Form["password2"])
                            throw new Exception("Your passwords do not match");

                        UserObject myUser = new UserObject();
                        myUser.createNew(Request.Form["username"], Request.Form["forename"], Request.Form["surname"], Request.Form["password"]);
                        myUser.setAttribute("usr.eml.0", Request.Form["email"]);
                        Session["userObject"] = myUser;
                        Session.Timeout = 30;
                        returnPairs.Add("Redirect", "/_/App");
                        returnPairs.Add("Status", "Completed");
                    }
                }
            }
            catch (Exception ex)
            {
                if (returnPairs.ContainsKey("Status"))
                    returnPairs.Remove("Status");
                returnPairs.Add("Status", "Error");
                returnPairs.Add("Exception", ex.Message);
                if (ex.InnerException != null)
                    returnPairs.Add("InnerException", ex.InnerException.Message);
            }
            Response.Clear();
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(returnPairs));
        }
    }
}