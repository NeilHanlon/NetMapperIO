using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetMapper.IO.__.App.User
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void updateDetails_Click(object sender, EventArgs e)
        {
            try
            {
                UserObject user = new UserObject().selectById(((UserObject)Session["userObject"]).userId);
                if (userForname.Text != String.Empty) user.forename = userForname.Text;
                if (userSurname.Text != String.Empty) user.surname = userSurname.Text;
                userDetailError.Visible = true;
                userDetailError.CssClass = "alert alert-success";
                userDetailErrorText.Text = "User details have been updated successfully";
            }
            catch (Exception ex)
            {
                userDetailError.Visible = true;
                userDetailError.CssClass = "alert alert-danger";
                if (ex.InnerException != null)
                    userDetailErrorText.Text = String.Format("An error occured while updating details. {0}", ex.InnerException.Message);
                else
                    userDetailErrorText.Text = String.Format("An error occured while updating details. {0}", ex.Message);
            }
        }

        protected void savePassword_Click(object sender, EventArgs e)
        {
            UserObject user = new UserObject();
            try
            {
                user.selectByUsername(((UserObject)Session["userObject"]).userName, oldPassword.Text);
                if (new1Password.Text == new2Password.Text)
                {
                    user.password = new1Password.Text;
                    passwordError.Visible = true;
                    passwordError.CssClass = "alert alert-success";
                    passwordErrorText.Text = "Password updated successfully";
                }
                else
                {
                    passwordError.Visible = true;
                    passwordError.CssClass = "alert alert-warning";
                    passwordErrorText.Text = "The passwords do not match";
                }
            }
            catch (Exception ex)
            {
                passwordError.Visible = true;
                passwordError.CssClass = "alert alert-danger";
                passwordErrorText.Text = "Your old password is incorrect";
            }
        }
    }
}