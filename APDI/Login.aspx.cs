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
                /*string query = "SELECT COUNT(1) FROM std_id WHERE id=@id AND passwd=@passwd";
                MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@id", txtuserid.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@passwd", txtpasswd.Text.Trim());
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {*/
                switch (ddlst.SelectedItem.Value)
                {
                    case "s":
                        string query = "SELECT COUNT(1) FROM std_id WHERE id=@id AND passwd=@passwd";
                        MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@id", txtuserid.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@passwd", txtpasswd.Text.Trim());
                        int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                        if (count == 1)
                        {
                            Context.Items["id"] = txtuserid.Text;   //如果是學生，頁面轉向student.aspx
                            Server.Transfer("student.aspx", true);
                        }
                        else
                        {
                            lblErrorMessage.Text = "帳號或密碼輸入錯誤";
                            lblErrorMessage.Visible = true;
                        }
                        break;
                    case "t":
                        string query_t = "SELECT COUNT(1) FROM teach_id WHERE id=@id AND passwd=@passwd";
                        MySqlCommand sqlCmd_t = new MySqlCommand(query_t, sqlCon);
                        sqlCmd_t.Parameters.AddWithValue("@id", txtuserid.Text.Trim());
                        sqlCmd_t.Parameters.AddWithValue("@passwd", txtpasswd.Text.Trim());
                        int count_t = Convert.ToInt32(sqlCmd_t.ExecuteScalar());
                        if (count_t == 1)
                        {
                            Session["id"] = txtuserid.Text;   //如果是老師，頁面轉向Event.aspx
                            Response.Redirect("Event.aspx");
                        }
                        else
                        {
                            lblErrorMessage.Text = "帳號或密碼輸入錯誤";
                            lblErrorMessage.Visible = true;
                        }

                        break;
                    case "d":
                        lblErrorMessage.Text = "請選擇老師或學生";
                        lblErrorMessage.Visible = true;
                        break;
                }
                //Context.Items["id"] = txtuserid.Text;   //如果是學生，頁面轉向student.aspx
                //Server.Transfer("student.aspx", true);
                //Session["id"] = txtuserid.Text;
                //Response.Redirect("Event.aspx");   //如果是老師，頁面轉向Event.aspx
            
                /*else
                {
                    lblErrorMessage.Text = "帳號或密碼輸入錯誤";
                    lblErrorMessage.Visible = true;
                }*/

            }

            //Context.Items["id"] = txtuserid.Text;
            //Server.Transfer("student.aspx", true);
        }
    }
}