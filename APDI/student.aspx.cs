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
    public partial class student : System.Web.UI.Page
    {
        string connectionString = @"Server=localhost; Database=faceid; Uid=root; Pwd=10515003;";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            //DvEvent.HeaderText = "Hi，" + Session["_id"].ToString() + "<br>你的通知訊息";
            lbdate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //DvEvent.HeaderText = "Hi，" +  + "<br>你的通知訊息";
            this.lbuser.Text = Request.QueryString["_id"];


            if (!IsPostBack) //判斷Page是否第一次執行，只在第一次執行
            {
                //DvEvent.HeaderText = "Hi，" + Context.Items["id"].ToString() + "<br>你的通知訊息";
                GridFill();
               
            }
        }

        /*protected void DataBound(object sender, EventArgs e)
        {

        }*/

        void GridFill()
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
                {
                    sqlCon.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter();
                    MySqlCommand sqlCmd = new MySqlCommand("StudentMessageView", sqlCon);
                    //MySqlCommand sqlCmd = new MySqlCommand("EventViewAll", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("_id", lbuser.Text);
                    sqlCmd.Parameters.AddWithValue("_date", lbdate.Text);
                    //sqlCmd.Parameters.AddWithValue("",DvEvent.D)
                    sqlDa.SelectCommand = sqlCmd;
                    DataTable dtbl = new DataTable();
                    sqlDa.Fill(dtbl);
                    DvEvent.DataSource = dtbl;
                    DvEvent.DataBind();
                    //string numstr = Convert.ToString(DvEvent.Rows[0].Cells[0].Text.Trim());
                    lbnum.Text = dtbl.Rows[0][0].ToString();


                }
            }

            catch (Exception ex)
            {
                lblErrorMessage.Text = "沒有通知訊息";
            }
        }

        protected void btnPre_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter();
                MySqlCommand sqlCmd = new MySqlCommand("p", sqlCon);
                //MySqlCommand sqlCmd = new MySqlCommand("EventViewAll", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("_eve_num",lbnum.Text);
                sqlCmd.Parameters.AddWithValue("_id", lbuser.Text);
                sqlCmd.Parameters.AddWithValue("_date", lbdate.Text);
                //sqlCmd.Parameters.AddWithValue("",DvEvent.D)
                sqlDa.SelectCommand = sqlCmd;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                DvEvent.DataSource = dtbl;
                DvEvent.DataBind();
                //string numstr = Convert.ToString(DvEvent.Rows[0].Cells[0].Text.Trim());
                lbnum.Text = dtbl.Rows[0][0].ToString();
                
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter();
                MySqlCommand sqlCmd = new MySqlCommand("n", sqlCon);
                //MySqlCommand sqlCmd = new MySqlCommand("EventViewAll", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("_eve_num", lbnum.Text);
                sqlCmd.Parameters.AddWithValue("_id", lbuser.Text);
                sqlCmd.Parameters.AddWithValue("_date", lbdate.Text);
                //sqlCmd.Parameters.AddWithValue("",DvEvent.D)
                sqlDa.SelectCommand = sqlCmd;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                DvEvent.DataSource = dtbl;
                DvEvent.DataBind();
                //string numstr = Convert.ToString(DvEvent.Rows[0].Cells[0].Text.Trim());
                lbnum.Text = dtbl.Rows[0][0].ToString();

            }
        }
    }
}