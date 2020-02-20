using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace APDI
{
    public partial class Event : System.Web.UI.Page
    {
        string connectionString = @"Server=localhost; Database=faceid; Uid=root; Pwd=10515003; ";
        protected void Page_Load(object sender, EventArgs e)
        {
            //lbdate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //DvEvent.HeaderText = "Hi，" +  + "<br>你的通知訊息";
            //this.lbuser.Text = Request.QueryString["_id"];

            if (!IsPostBack) //判斷Page是否第一次執行，只在第一次執行
            {
                Clear();
                GridFill();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
                {
                    sqlCon.Open();

                    //Session.CodePage = 65001;
                    //Response.Charset = "UTF8";
                    //Request.ContentEncoding = System.Text.Encoding.UTF8;    //請求編碼
                    //Response.ContentEncoding = System.Text.Encoding.UTF8;   //響應編碼
                    MySqlCommand sqlCmd = new MySqlCommand("EventAddOrEdit",sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("_eve_num", Convert.ToInt32(hfevenum.Value == "" ? "0" : hfevenum.Value));
                    sqlCmd.Parameters.AddWithValue("_id", txtid.Text);
                    sqlCmd.Parameters.AddWithValue("_byid", txtbyid.Text);
                    sqlCmd.Parameters.AddWithValue("_eve_time", txttime.Text);
                    //sqlCmd.Parameters.AddWithValue("_eve_time", Convert.ToString("yyyy/MM/dd"));
                    sqlCmd.Parameters.AddWithValue("_eve_locl", txtlocl.Text);
                    sqlCmd.Parameters.AddWithValue("_eve_desc", txtdesc.Text);
                    sqlCmd.ExecuteNonQuery();   //不會返回任何資料庫的資料, 它只會返回整數值來表示成功或受影響的資料列數目
                    GridFill();
                    Clear();
                    lblSuccessMessage.Text = "保存成功";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }

        void Clear()   //把畫面清空
        {
            hfevenum.Value = "";
            txtid.Text = txtbyid.Text = txttime.Text =txtlocl.Text = txtdesc.Text = "";
            btnSave.Text = "保存";
            btnDelete.Enabled = false;
            lblErrorMessage.Text = lblSuccessMessage.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void GridFill()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter();
                MySqlCommand sqlCmd = new MySqlCommand("EventViewAll", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                //sqlCmd.Parameters.AddWithValue("_eve_time", Convert.ToString("yyyy/MM/dd"));
                sqlDa.SelectCommand = sqlCmd;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                gvEvent.DataSource = dtbl;
                gvEvent.DataBind();
            }
        }

        protected void lnkSelect_OnClick(object sender, EventArgs e)
        {
            int eve_num = Convert.ToInt32((sender as LinkButton).CommandArgument);
            //txttime.Text = ;
            //string 
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("EventViewByID", sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("_eve_num", eve_num);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                
                txtid.Text = dtbl.Rows[0][1].ToString();
                txtbyid.Text = dtbl.Rows[0][2].ToString();
                string a = dtbl.Rows[0][3].ToString();
                int start = 1, length = 10;
                //string result = a.Trim();
                //string[] sArray = a.Split(:);
                txttime.Text = a.Substring(start-1,length);
                txtlocl.Text = dtbl.Rows[0][4].ToString();
                txtdesc.Text = dtbl.Rows[0][5].ToString();
                hfevenum.Value = dtbl.Rows[0][0].ToString();

                btnSave.Text = "更新";
                btnDelete.Enabled = true;
 
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlCommand sqlCmd = new MySqlCommand("EventDeleteByID", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("_eve_num", Convert.ToInt32(hfevenum.Value == "" ? "0" : hfevenum.Value));
                sqlCmd.ExecuteNonQuery();
                GridFill();
                Clear();
                lblSuccessMessage.Text = "刪除成功";
            }
        }
    }
}