using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

namespace APDI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(@"Server=localhost; Database=faceid; Uid=root; Pwd=10515003;"))
            {
                sqlCon.Open();
                string query = "SELECT COUNT(1) FROM teach_std WHERE id=@id AND passwd=@passwd";
                MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@id", txtuserid.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@passwd", txtpasswd.Text.Trim());
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {
                    Session["id"] = txtuserid.Text.Trim();
                    Response.Redirect("Event.aspx");
                }
                else { lblErrorMessage.Visible = true; }

            }
        }
    }
}