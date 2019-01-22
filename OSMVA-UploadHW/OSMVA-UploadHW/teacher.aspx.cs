using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Collections;

namespace OSMVA_UploadHW
{
    public partial class teacher : System.Web.UI.Page
    {
        //string connectionString = "Server=140.138.155.188; Database=stdcpp; User Id=sa; password=g1992312"; //資料庫位子 我的
        string connectionString = "Server=140.138.155.176; Database=OnlineJudgeCpp; User Id=sa; password=ltlab1612b"; //資料庫位子
        DateTime System_time = DateTime.Now;
        //string teacherpath = @"C:\Users\Imnate\Desktop\Web\";     //老師資料夾路徑
        string teacherpath = @"C:\Users\Imnate\Imnate\Web\";     //老師資料夾路徑
        //string teacherfolderpath = @"C:\Users\Imnate\Desktop\Web\teacherupload\";//老師上傳路徑
        string teacherfolderpath = @"C:\Users\Imnate\Imnate\Web\teacherupload\";//老師上傳路徑
        //string texcute = @"C:\Users\Imnate\Desktop\Web\tExecute.bat";
        string texcute = @"C:\Users\Imnate\Imnate\Web\tExecute.bat";
        //string new_texcute = @"C:\Users\Imnate\Desktop\Web\新版texecute.bat";
        string new_texcute = @"C:\Users\Imnate\Imnate\Web\新版texecute.bat";
        SqlConnection main_conn;
        string course = "";
        string id = "";
        
        ArrayList Repeat = new ArrayList();       
        DataSet ds = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
           //Page.MaintainScrollPositionOnPostBack = true; 
            main_conn = DBM.getconn_main();
            if (Session["ID"] != null && Session["course"]!=null)
            {
                 id = Session["ID"].ToString();
                 course = Session["course"].ToString();
                 //ShowGridView(id, course);//顯示新增的作業
                 //TextBox2.Text += id + course;                 
            }
            else 
            {
                ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('非法進入!');</script>");
            }
            
        }
        private void ShowGridView(string id,string course)//show 資料
        {
            
            SqlConnection Conn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlDataReader dr ;
            ds.Reset();
            cmd.Connection = Conn;
            Conn.ConnectionString = "Server=140.138.155.176; Database=OnlineJudgeCpp; User Id=sa; password=ltlab1612b"; 
           // Conn.ConnectionString = "Server=140.138.155.188; Database=stdcpp; User Id=sa; password=g1992312";
            cmd.CommandText = "SELECT cppType,assign,input_script,TestMode FROM " + course + "HWList";
            da.SelectCommand = cmd;
            da.Fill(ds, course + "HWList");//da.Fill["teacher_upload_2"]
                     
            GridView_showHW.DataSource = ds;
            ds.Tables[course + "HWList"].Columns[0].ColumnName = "作業名稱";//ds.Tables["teacher_upload_2"]
            ds.Tables[course + "HWList"].Columns[1].ColumnName = "答案腳本";//ds.Tables["teacher_upload_2"]
            ds.Tables[course + "HWList"].Columns[2].ColumnName = "輸入腳本";//ds.Tables["teacher_upload_2"].Columns[2].ColumnName = "測資模式";
            ds.Tables[course + "HWList"].Columns[3].ColumnName = "測資模式";
            GridView_showHW.DataBind();
            
            Conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
               Repeat.Add(dr[0].ToString());
            }
            Conn.Close();
        }
        protected void send_Button_Click(object sender, EventArgs e)//送資料
        {
            if (checkRepeat(textbox_hwname.Value) == true || TextBox_for.Text == "" || TextBox_DoWhile.Text == "" || TextBox_If.Text == "" || TextBox_While.Text == "")
            {

                textbox_hwname.Value = "";
                TextBox2.Text = "上傳失敗  作業題目重複";
            }
            else
            {
                switch (DropDownList_Chose_TestType.SelectedValue)
                {
                    case "1":
                        Customize_input_mode();
                        break;
                    case "2":
                        No_execute_Mode();
                        break;
                }
                
            }
        }
        private void No_execute_Mode()
        {
            string sqlcmd = "INSERT INTO " + course + "HWList(cppType,assign,TestMode,Structure_Condition) VALUES('" + textbox_hwname.Value + "','" + "不跑測資" + "','" + DropDownList_Chose_TestType.SelectedItem + "','"
                                                                                                              + "for=" + TextBox_for.Text + ","
                                                                                                              + "while=" + TextBox_While.Text + ","
                                                                                                              + "dowhile=" + TextBox_DoWhile.Text + ","
                                                                                                              + "if=" + TextBox_If.Text + ","
                                                                                                              + "switch=" + TextBox_switch.Text + "')";//資料庫測試使用          
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(sqlcmd, myconn);

            if (textbox_hwname.Value == "" || FileUpload1.HasFile == false || check_textbox_num() != true)//上傳有問題
            {
                TextBox2.Text = "上傳失敗 請檢查上傳檔案或是作業題目或是設定結構";
            }
            else//成功
            {
                myconn.Open();
                myCommand.ExecuteNonQuery();
                myconn.Close();
                create_new_table(textbox_hwname.Value, course);///前面要加一個防止重複
                To_DB_Announce(System_time.ToString("yyyy-MM-dd"), course, "新增程式作業", " " + textbox_hwname.Value + " 已開放繳交");//公告部分 整合需改顯示方法html部分
            }
        
        }
        private void Customize_input_mode()
        {

            string sqlcmd = "INSERT INTO " + course + "HWList(cppType,TestMode,Structure_Condition) VALUES('" + textbox_hwname.Value + "','" + DropDownList_Chose_TestType.SelectedItem + "','"
                                                                                                              + "for=" + TextBox_for.Text + ","
                                                                                                              + "while=" + TextBox_While.Text + ","
                                                                                                              + "dowhile=" + TextBox_DoWhile.Text + ","
                                                                                                              + "if=" + TextBox_If.Text + ","
                                                                                                              + "switch=" + TextBox_switch.Text + "')";         
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(sqlcmd, myconn);

            if (textbox_hwname.Value == "" || FileUpload1.HasFile == false || check_textbox_num() != true)//上傳有問題
            {
                TextBox2.Text = "上傳失敗 請檢查上傳檔案或是作業題目或是設定結構";
            }
            else//成功
            {
                myconn.Open();
                string topic = textbox_hwname.Value;
                myCommand.ExecuteNonQuery();
                myconn.Close();

                Directory.CreateDirectory(teacherfolderpath + course + topic + "\\");//新增裝老師腳本資料夾
                FileUpload1.PostedFile.SaveAs(teacherfolderpath + course + topic + "\\" + topic + ".cpp");

                create_new_table(textbox_hwname.Value, course);///前面要加一個防止重複
                                                        ///
                Compiler(topic);//執行compiler

                Show_All_Outpput(topic);//顯示輸出內容
                //ShowGridView(id, course);//顯示目前新增作業

                Directory.CreateDirectory(@"C:\Users\Imnate\Imnate\Web\Upload\" + course + topic + "\\");//新增裝作業資料夾                
                Directory.CreateDirectory(@"D:\Websites\OSMVA-UploadHW\OSMVA-UploadHW\img\" + course + topic + "pic\\");
                Directory.CreateDirectory(@"C:\Users\Imnate\Imnate\Web\Upload\" + course + topic + "_Structure\\");//結構分析資料夾
                To_DB_Announce(System_time.ToString("yyyy-MM-dd"), course, "新增程式作業", " " + textbox_hwname.Value + " 已開放繳交");//公告部分 整合需改顯示方法html部分

            }
        }      
        private void Compiler(string Filename)//題目名稱
        {
            Process process = new Process();//抓Cmd call compiler.bat
            process.StartInfo.FileName = teacherpath + "tcompile.bat";
            process.StartInfo.Arguments = course + Filename+" " + Filename + ".cpp";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            TextBox2.Text += process.StandardOutput.ReadToEnd() + "\n" + "===================================" + "\n";
            process.WaitForExit();

            if (File.Exists(teacherfolderpath + course + Filename +"\\"+ Filename + ".exe"))//路徑要注意
            {
                //讀檔部分
                StreamReader sr = new StreamReader(teacherfolderpath + course + Filename + "\\" + Filename + ".cpp", System.Text.Encoding.Default);//路徑要注意

                //=== 一次讀取全部內容 ===
                string a = sr.ReadToEnd();
                TextBox2.Text += "===========================================\n\t\t產生EXE成功\n===========================================\n\n" + a;


                //===逐行讀取，直到檔尾===
                while (!sr.EndOfStream)
                {
                    TextBox2.Text += a;

                }
                sr.Close();
                send_button.Visible = false;
                textbox_hwname.Disabled = false;
               // TextBox1.Enabled = false;
                DropDownList_Chose_TestType.Enabled = false;
                FileUpload1.Enabled = false;
                int index = Panel1.Controls.OfType<TextBox>().ToList().Count + 1;

                Assign_Test(DropDownList_Chose_TestType.SelectedItem.ToString(), TextBox_Customize_Input.Text, textbox_hwname.Value);//跑texecute

            }

        }

           /*
            *此部分尚未將四種模式進入來判斷
            *還有要有給上傳作業者一個預覽的模式 可以確定後再送出 
            */
        private void Assign_Test(string mode,string test_assignment,string Hw_name)
        {
            //string upfile = FileUpload1.FileName.Substring(0, FileUpload1.FileName.Length - 4);
            //Session["s_upfile"] = upfile;//存upload 1 的 name 在session裡面 
            //string s_up = Session["s_upfile"].ToString();
            string assign_script = ""; //放資料庫的資料 答案 腳本
            string input_script = "";//放資料庫的資料 輸入值 腳本
            string[] cut = test_assignment.Split(',');
            ArrayList cut_array = new ArrayList();
            foreach(string cuts in cut)
            {
                if (cuts.Equals(""))
                {
                }
                else
                {
                    cut_array.Add(cuts);
                }
                //TextBox2.Text +=cuts; //debug用
            }

            for (int i = 0; i < cut_array.Count; i++)
            {
                string name = Hw_name + "-" + i.ToString();
                input_script += Creat_execute_Script(cut_array[i].ToString(), name) + ",";
                assign_script += execute(name, i) + ","; // cut_array[i].ToString()+",";
             
                //TextBox4.Text += test_scrip;
            }
            string sqlcmd = "UPDATE " + course + "HWList" + " SET assign ='" + assign_script + "',input_script ='" + input_script + "' WHERE cppType='" + textbox_hwname.Value + "' AND TestMode ='" + DropDownList_Chose_TestType.SelectedItem.ToString() + "'";//資料庫測試使用
            //TextBox2.Text += "SQL CMD = "+sqlcmd;
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(sqlcmd, myconn);
            myconn.Open();
            myCommand.ExecuteNonQuery();
            myconn.Close();


        }
        private string Creat_execute_Script(string input,string name)
        {
            StreamWriter sw = new StreamWriter(@"C:\Users\Imnate\Imnate\Web\teacherupload\" + course + textbox_hwname.Value + "\\輸入值腳本" + name + ".txt");
            sw.WriteLine(input);
            sw.Close();
            return "輸入值腳本" + name + ".txt";
        }
        private string execute(string InputTxtName,int i)
        {
            /* execute_multiple.bat <---- bat檔案內容
             * 
               @echo off
               cd C:\Users\Imnate\Imnate\Web\teacherupload\%1
               %2.exe < %3.txt > %4.txt
               exit
             * 
             */
            string cmd = course + textbox_hwname.Value + " " + textbox_hwname.Value + " " + "輸入值腳本" + InputTxtName + " " + "答案腳本" + textbox_hwname.Value + "-" + i.ToString();
            //TextBox3.Text += "傳到bat參數 = " +cmd+"\r\n";
            Process texecute_process = new Process();
            texecute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\execute_multiple.bat";
            texecute_process.StartInfo.Arguments = cmd;
          //string.Format("{0} {1}", Argument_1,Argument_2) ; 另一種送參數到bat裡面
            texecute_process.StartInfo.RedirectStandardOutput = true;
            texecute_process.StartInfo.RedirectStandardError = true;
            texecute_process.StartInfo.UseShellExecute = false;
            texecute_process.Start();
            TextBox3.Text += texecute_process.StandardOutput.ReadToEnd();
            TextBox3.Text += texecute_process.StandardError.ReadToEnd();
            texecute_process.WaitForExit();
            return "答案腳本" + textbox_hwname.Value + "-" + i.ToString();
        }
        private bool check_textbox_num()
        {
            try
            {
               Convert.ToInt32(TextBox_for.Text);
               Convert.ToInt32(TextBox_While.Text);
               Convert.ToInt32(TextBox_DoWhile.Text);
               Convert.ToInt32(TextBox_If.Text);
               return true;
            }
            catch
            {
                
            }
            return false;
        }
        private void To_DB_Announce(string date, string course, string type, string message)
        {
            string Announce_cmd = "INSERT INTO "+course+"Announce(date,course,type,message) VALUES('" + date + "','" + course + "','" + type + "','" + message + "')";
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(Announce_cmd, myconn);
            myconn.Open();
            myCommand.ExecuteNonQuery();
            myconn.Close();
        }
        private void create_new_table(string table_name,string course)
        {
            string sqlcmd = "CREATE TABLE " + table_name + course +
                            "([std_id] VARCHAR(40) NULL,"+
                            "[FileName] VARCHAR(Max) NULL,"+
                            "[pic_path] VARCHAR(MAX) NULL,"+
                            "[pic_byte] image NULL,"+
                            "[ToF] VARCHAR(MAX) NULL,"+
                            "[date] VARCHAR(MAX) NULL," +
                            "[time] VARCHAR(MAX) NULL," +
                            "[Verif_Code] VARCHAR(MAX) NULL)"
                            ;          
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(sqlcmd, myconn);
            myconn.Open();
            myCommand.ExecuteNonQuery();
            myconn.Close();

        }
        private void Show_All_Outpput(string topic)
        {
            ArrayList cuted_assign = new ArrayList();
            string sqlcmd = "SELECT assign FROM "+course+"HWList"+" WHERE cppType='" + topic + "'";
            string assign = "";          
            SqlConnection myconn = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(sqlcmd, myconn);
            SqlDataReader dr;
            myconn.Open();

            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                assign=dr[0].ToString();
            }
            myconn.Close();
            cuted_assign = cut_assign_test(assign);//切割完了資料庫裡面欄位


           
            for (int i = 0; i < cuted_assign.Count; i++)
            {
               //TextBox2.Text += cuted_assign[i].ToString();
               read_asw2Textbox(cuted_assign[i].ToString(), course + textbox_hwname.Value);
            }

        }
        private void read_asw2Textbox(string File,string path)//這邊會錯的話資料庫的欄位要調成nchar(MAX)
        {
            //StreamReader file = new StreamReader(@"C:\Users\Imnate\Desktop\Web\teacherupload\" + path + @"\" + File.Replace(" ", "") + ".txt", System.Text.Encoding.Default);
            StreamReader file = new StreamReader(@"C:\Users\Imnate\Imnate\Web\teacherupload\" + path + @"\" + File.Replace(" ", "") + ".txt", System.Text.Encoding.Default);
            string line = "測試腳本:" + File+"\n";
            line += file.ReadToEnd();
            file.Close();
            TextBox3.Text += line;
        }
        private ArrayList cut_assign_test(string test_assignment)//cut出來是用,分的會有一個陣列會是空白
        {
            string[] cut = test_assignment.Split(',');
            ArrayList cut_array = new ArrayList();
            foreach (string cuts in cut)
            {
                if (cuts.Equals(""))
                {
                }
                else
                {
                    cut_array.Add(cuts);
                }
                //TextBox2.Text +=cuts;
            }


            return cut_array;
        }
        private bool checkRepeat(string checkName)
        {
            for (int j = 0; j < Repeat.Count;j++ )
            {
                if (Repeat[j].ToString().Equals(checkName))
                {
                    return true; //true 抓到重複
                }
      
            }
            return false; //未抓到重複
           
        }
 
        protected void DropDownList_Chose_TestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(DropDownList_Chose_TestType.SelectedValue)
            {
                case "1":
                        
                        TextBox_Customize_Input.Visible = true;
              
                        if (TextBox_Customize_Input.Text.Length < 1)
                        {
                            Label_inputWarning.Visible = true;
                        }
                        else
                        {
                            Label_inputWarning.Visible = false;
                        }
                    break;
                case "2":
                        
                        TextBox_Customize_Input.Visible = false;
                        Label_inputWarning.Visible = false;
                    break;
            }

        }

        protected void TextBox_Customize_Input_TextChanged(object sender, EventArgs e)
        {
            if (TextBox_Customize_Input.Text.Length < 1)
            {
                Label_inputWarning.Visible = true;
            }
            else
            {
                Label_inputWarning.Visible = false;
            }
        }
        protected void button_send_message_Click(object sender, EventArgs e)
        {
            main_conn.Open();
            DateTime date = DateTime.Now;
            string time = date.ToString("yyyy-MM-dd");
            string cmd_insert = "INSERT INTO " + course + "Announce(date,course,type,message) VALUES('" + time + "','" + course + "','" + annouce_title.Value.ToString() + "','" + annouce_message.Value.ToString() + "')";          
            SqlCommand myCommand = new SqlCommand(cmd_insert, main_conn);
            myCommand.ExecuteNonQuery();
            main_conn.Close();
        }
   
    }
}