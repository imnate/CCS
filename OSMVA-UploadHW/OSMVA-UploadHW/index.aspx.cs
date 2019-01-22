using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Data.SqlClient;
using System.Drawing;


namespace OSMVA_UploadHW
{
    public partial class Test_send : System.Web.UI.Page
    {
        
        //string nbDBaccount = "Server=140.138.155.183; Database=NianBaoSystem; User Id=Radar; password=Radar";
        //string main_DB = "Server=140.138.155.176; Database=OnlineJudgeCpp; User Id=sa; password=ltlab1612b";
        SqlConnection NB_conn;
        SqlConnection main_conn;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Refesh_Announce("CS_106");
            }
            catch (Exception x)
            {

            }
            finally
            {
                main_conn.Close();
            }
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Length <7 || TextBox2.Text.Length <7 || TextBox1.Text.Length >7 ||TextBox1.Text.Length >7)
            {
                ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('帳號密碼格式錯誤!');</script>");//帳號密碼格式錯誤         
            }
            else
            {
                Log_in();
                
            }
           
        }
        private void Refesh_Announce(string course_id)//這邊還沒加入判斷其他課程的
        {
            ListBox_newMessage.Items.Clear();
            string announce_cmd = "SELECT * FROM " + course_id + "Announce order by date";//這邊改掉
            List<string> date = new List<string>();
            List<string> course = new List<string>();
            List<string> type = new List<string>();
            List<string> message = new List<string>();
            List<string> all = new List<string>();
            main_conn = DBM.getconn_main();
            SqlCommand myCommandannounce = new SqlCommand(announce_cmd, main_conn);
            SqlDataReader dr_announce;
            main_conn.Open();
            dr_announce = myCommandannounce.ExecuteReader();        
            while (dr_announce.Read())
            {
                date.Add(dr_announce[0].ToString());
                course.Add(dr_announce[1].ToString());
                type.Add(dr_announce[2].ToString());
                message.Add(dr_announce[3].ToString());
            }
            dr_announce.Close();
            for (int i = 0; i < date.Count; i++)
            {
                all.Add("[" + date[i].ToString() + "][" + course[i].ToString() + "][" + type[i].ToString() + "] " + message[i].ToString());
            }
            for (int j = all.Count; j > 0; j--)
            {
               ListBox_newMessage.Items.Add(all[j-1].ToString());
            }

           
        }
        private void Log_in()
        {

            string acount = "";
            string pwd = "";
            string name = "";           
            string cmd = "SELECT REPLACE(id,' ',''),REPLACE(pwd,' ',''),name FROM acountT_Std WHERE id='" + TextBox1.Text + "' AND pwd = '" + TextBox2.Text + "'";
            //SqlConnection conn =  new SqlConnection(nbDBaccount);
            try
            {
                NB_conn = DBM.getdbconn_nb();
                SqlCommand myCommandLogin = new SqlCommand(cmd, NB_conn);
                SqlDataReader drLogin;
                NB_conn.Open();
                drLogin = myCommandLogin.ExecuteReader();
                while (drLogin.Read())
                {
                    acount = drLogin[0].ToString();
                    pwd = drLogin[1].ToString();
                    name = drLogin[2].ToString();
                }
                drLogin.Close();
            }
            catch (Exception x)
            {

            }
            finally
            {
                NB_conn.Close();           
            }
            check_AcountInfo(acount, pwd, name);
        }
        private bool check_AcountInfo(string acount,string pwd,string name)
        {
            //TextBox3.Text += "進入check_AcountInfo acc: " + acount + " pwd: " + pwd+"\r\n";

            if (TextBox1.Text.Equals(acount) && TextBox2.Text.Equals(pwd))
            {
                permission("CS_106", TextBox1.Text); 
            }
            else
            {
               ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('帳號或密碼錯誤!');</script>");
            }
            return false;
        }
        private void permission(string course_id, string s_id)
        {
            
            string course = "";
            string assistant = "";
            string conn = "Server=140.138.155.176; Database=OnlineJudgeCpp; User Id=sa; password=ltlab1612b";
            string Sql_cmd = "SELECT REPLACE(C_ID,' ',''),REPLACE(assistant,' ','') FROM enable_course WHERE C_ID ='" + course_id + "'";//course
            SqlConnection myconn = new SqlConnection(conn);
            SqlCommand myCommand = new SqlCommand(Sql_cmd, myconn);
            SqlDataReader dr;
            myconn.Open();

            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                course = dr[0].ToString();
                assistant = dr[1].ToString();
            }
            dr.Close();
            myconn.Close();


           
            if (course.Equals(course_id))//course
            {
                     
                string Sql_cmd_s_id = "SELECT REPLACE(S_ID,' ','')  FROM " + course + " WHERE S_ID = '" + s_id + "'" + "\r\n"; //S_ID & COURSE
                string id = "";

                SqlCommand myCommand_s_id = new SqlCommand(Sql_cmd_s_id, myconn);
                SqlDataReader dr2;
                myconn.Open();
                dr2 = myCommand_s_id.ExecuteReader();
                while (dr2.Read())
                {
                    id = dr2[0].ToString();
                }
                dr2.Close();
                myconn.Close();
                //TextBox3.Text += "進入permission id: " + id + " s_id: " + s_id + "\r\n";
                if (id == s_id)
                {

                    Session["ID"] = s_id;
                    Session["course"] = course_id;
                    Session["Ispostback"] = true;
                    Server.Transfer("CppJudge.aspx");
                    //TextBox3.Text += "學生進入點\r\n";
                }
                else
                {
                    

                    if (assistant == s_id)//判斷助教
                    {
                        Session["ID"] = s_id;
                        Session["course"] = course_id;
                        Server.Transfer("teacher.aspx");
                        //TextBox3.Text += "助教進入點\r\n";
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert(" + s_id + "'非本系統開放之學生！');</script>");
                    }
                }
            }
            else
            {
                //TextBox3.Text = course_id + " " + assistant + " " + s_id;
                ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('非本系統開放之課程！');</script>");
            }

        }
    }

}