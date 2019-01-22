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
using System.Text.RegularExpressions;

namespace OSMVA_UploadHW
{
    public partial class upload : System.Web.UI.Page
    {
     
        string path = ""; //path路徑
        string logfolder = @"C:\Users\Imnate\Imnate\Web\log\";//log路徑
        SqlConnection main_conn;
        string id = ""; //資料庫面的ID
        string id_file = "";//檔案儲存面的ID 會這樣寫是因為要判斷重複 
        string course = "";
        DateTime System_time = DateTime.Now;
        string pic = "";
        string System_log;
        string logPath ;
        string logtime;
        string upload_status1 = "[上傳狀態]";
        string process_status1 = "[處理狀態]";
        string Filename;
        string upfile;
        string cpp_path;
        string error = "[檔案狀態]非.CPP格式 或是 超過限制容量2MB";
   
        protected void Page_Load(object sender, EventArgs e)
        {
            main_conn = DBM.getconn_main();
                        
            if (Session["ID"] != null && Session["course"] != null)
            {
                //ListBox_system_feedback.Items.Add(Session["IsPostBack"].ToString());//這個Session["IsPostBack"]是要來控制F5刷新頁面不給他寫入資料庫控制重index傳進來的
                //user_id_show.InnerHtml = id;
                id = Session["ID"].ToString();
                id_file = id;
                course = Session["course"].ToString();              
                Button_Reupdate.Visible = false;
                FileUpload.Enabled = false;
                compiler.Enabled = false;
                updata_DropDownList();
                path = @"C:\Users\Imnate\Imnate\Web\Upload\" + course + DropDownList_HW.SelectedValue + "\\";
                
                // ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('請將cpp檔案名稱改為 " + Encode_file(id) + ".cpp 並且上傳portal');</script>");
                //Encode_file(id);//測試

            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('非法進入!');</script>");
            }
        }
        protected void compiler_Click(object sender, EventArgs e) //繳交與compiler按鈕
        {
            main_conn.Open();

            if (check_DB_Upload_repeat(id))//檢察繳交次數從DB切入
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[成功]" + "[檔名狀態]" + FileUpload.FileName);
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[成功]" + upload_status1 + "上傳成功");
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[成功]" + process_status1 + "Compiler成功,exe產生成功");               
                string extension = Path.GetExtension(id_file + ".cpp").ToLowerInvariant();//處理CPP容量限制的宣告
                int Filesize = FileUpload.PostedFile.ContentLength; //上傳檔案大小宣告
                List<string> allowedExtextsion = new List<string> { ".cpp" };//List 放的是 .cpp附檔名的 資料 其作用是用來檢查學生上傳檔案附檔名是否是cpp 以防止上傳exe被攻擊
                if (allowedExtextsion.IndexOf(extension) == -1 || Filesize > 1100000)//檢查是不是CPP 跟容量限制 1MB
                {
                    ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[失敗]" + error);
                    compiler.Enabled = true;
                    FileUpload.Enabled = true;
                    return;
                }
                else //容量限制合格 檔案格是正確
                {
                    Filename = id_file + ".cpp";
                    upfile = id_file;
                    cpp_path = path + id_file + ".cpp";
                    FileUpload.PostedFile.SaveAs(path + upfile + ".cpp");//上傳檔案 利用id來取檔名不要用原本檔名
                    Show_code();
                    Session["s_fsname"] = upfile;//session upfile 宣告 不紀錄session會沒值
                    Session["s_id"] = id;
                    switch (check_mode(DropDownList_HW.SelectedValue))
                    {
                        case "不跑測資":
                            No_execute_Mode();
                           //ListBox_system_feedback.Items.Add("跑完");
                            break;
                        case "自訂輸入":
                            Customize_input(); //自訂測資模式  
                            break;
                    }
                }          
            }
            else
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]"+"[成功]" + "作業已繳交" + "\r\n");
                DropDownList_HW.Enabled = true;
                DropDownList_HW.SelectedValue = "請選擇上傳作業";
                FileUpload.Enabled = true;
                compiler.Enabled = true;
            }
            //File.WriteAllText(logPath, System_log + "\n\n" + "日誌新建時間: " + logtime);
        }
        private void Show_code()
        {
            //讀檔部分顯示在textbox上面
            StreamReader sr = new StreamReader(path + Filename, System.Text.Encoding.Default);
            //=== 一次讀取全部內容 ===
            string a = sr.ReadToEnd();
            //===逐行讀取，直到檔尾===
            while (!sr.EndOfStream)
            {
                a += sr;
            }
            TextBox_Code.Text = a;
            sr.Close();

        }
        private void No_execute_Mode()//不跑測資模式
        {
            ListBox_system_feedback.Items.Add("進入不跑測資模式");
            string op_Struceture_path = @"C:\Users\Imnate\Imnate\Web\Upload\" + course + DropDownList_HW.SelectedValue;
            string png_path = @"D:\Websites\OSMVA-UploadHW\OSMVA-UploadHW\img\" + course + DropDownList_HW.SelectedValue + @"pic";
            string bat_save_path = course + DropDownList_HW.SelectedValue + @"pic";
            string ToF;
            string fs = Session["s_fsname"].ToString();//在伺服器檔名 這邊會隨著次數更新
            string s_id = Session["s_id"].ToString();         
            try
            {
                Run_AI_System(cpp_path, bat_save_path, id_file, op_Struceture_path + "_Structure", id_file);//call op.bat 跑AI系統 (動態修改要加入 學生跟路徑) ※※注意路徑               
                Thread.Sleep(6000);
                if (chech_file_exsit(op_Struceture_path, id_file) == true)
                {
                    if (struceture_check(course, DropDownList_HW.SelectedValue, op_Struceture_path + "_Structure") != true)
                    {
                        ToF = "False";
                        insert_file_name_toDB(s_id, fs, png_path + "\\" + fs, ToF,"NULL");
                        
                    }
                    else
                    {
                        ToF = "True";
                        insert_file_name_toDB(s_id, fs, png_path + "\\" + fs, ToF, "'"+Encode_file(s_id)+"'");
                        ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('請將cpp檔案名稱改為 " + Encode_file(id) + ".cpp 並且上傳portal');</script>");
                    }
                }
                else
                {
                    ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤] " + "結構產生有問題，請重新嘗試");
                    Button_Reupdate.Visible = true;
                    compiler.Enabled = false;
                    return;
                }
            }
            catch (Exception x)
            {
                System_log += "160 :: " + x + "\n";
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + x);
            }
            finally
            {
                main_conn.Close();
            }
            
        }
        private string check_mode(string HW)
        {
            string cmd = "SELECT TestMode FROM " + course + "HWList" + " WHERE cppType = '" + HW + "'";
            string mode = "";
            SqlCommand myCommand = new SqlCommand(cmd, main_conn);
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                mode = dr[0].ToString();
            }
            dr.Close();
            return mode;
        }
        private void Customize_input()
        {
            path = @"C:\Users\Imnate\Imnate\Web\Upload\" + course + DropDownList_HW.SelectedValue + "\\";          
            string op_Struceture_path = @"C:\Users\Imnate\Imnate\Web\Upload\" + course + DropDownList_HW.SelectedValue;
            string result = "";
            compiler_cpp(Filename);//編譯學生程式碼   
            
                    if (File.Exists(path + upfile + ".exe"))//if 裡面的判斷是compiler成功後就會產生出exe檔案 他會去檢查是否有存在exe檔案(現在上傳而且compiler過後)
                    {
                        compiler.Enabled = false;//關閉繳交按鈕 
                        Button_excute.Visible = true;//打開比對按鈕 
                        try
                        {
                       
                            Button_excute.Visible = true;
                            string png_path = @"D:\Websites\OSMVA-UploadHW\OSMVA-UploadHW\img\" + course + DropDownList_HW.SelectedValue + @"pic";
                            string bat_save_path = course + DropDownList_HW.SelectedValue + @"pic";
                            try
                            {
                                Run_AI_System(cpp_path, bat_save_path, id_file, op_Struceture_path + "_Structure", id_file);//call op.bat 跑AI系統 (動態修改要加入 學生跟路徑) ※※注意路徑
                                Thread.Sleep(5500);
                                if (chech_file_exsit(op_Struceture_path, id_file) == true)
                                {
                                    struceture_check(course, DropDownList_HW.SelectedValue, op_Struceture_path + "_Structure");
                                }
                                else
                                {
                                    ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤] " + "結構產生有問題，請重新嘗試");
                                    Button_Reupdate.Visible = true;
                                    Button_excute.Visible = false;
                                    compiler.Enabled = false;
                                    return;
                                }
                            }
                            catch (Exception x)
                            {
                                System_log += "128 :: " + x + "\n";
                                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + "[圖片狀態] 圖片無法產生，請檢查你的程式碼");
                            }
                            Session["png_path"] = png_path;
                            pic = Session["png_path"].ToString();
                            Image_std_pic.ImageUrl = "/img/" + course + DropDownList_HW.SelectedValue + "pic/" + id_file + ".png";
                        }
                        catch (Exception x)
                        {
                            System_log += "136 :: " + x + "\n";
                        }
                        finally
                        {
                            main_conn.Close();
                        }
                    }
                    else //compiler 失敗
                    {
                        TextBox_Code.Text = result;//直接show出cmd失敗error
                        Button_Reupdate.Visible = true;
                        Button_excute.Visible = false;
                        ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + "[檔案狀態] " + FileUpload.FileName + "編譯失敗，請重新上傳");
                    }

            
        }
        
        private bool struceture_check(string course, string hw, string op_Struceture_path)
        {
            List<string> structure = new List<string>();           
            string[] structure_num = null;           
            string load_teacher_setStructure = "SELECT Structure_Condition FROM " + course +"HWList WHERE cppType = '"+ hw +"'";          
            SqlCommand myCommand = new SqlCommand(load_teacher_setStructure, main_conn);
            SqlDataReader dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                structure.Add(dr[0].ToString());
            }
            dr.Close();            
            foreach (string i in structure)
            {
                structure_num = i.Split(',','=');
            }           
            if (compare_CountStructure(op_Struceture_path, structure_num) != true)
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "][錯誤] 非題目設定之結構");
                Button_Reupdate.Visible = true;
                Button_excute.Visible = false;
                return false;
            }
            ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "][正確] 通過系統檢測題目設定之結構");
            return true;
            //ListBox_system_feedback.Items.Add("tof: "+tof.ToString());
                   
        }
        private bool compare_CountStructure(string count_path,string[] teacher_setStructure)
        {
            string path = count_path + "\\"+id_file+"Count.txt";
            string std_Structure_txt = File.ReadAllText(path);
            string[] std_Structure = Regex.Split(std_Structure_txt.Replace(" ", ""), @"[\s]+");
            List<bool> all_compare = new List<bool>(); ;
            for (int i = 0; i < std_Structure.Length; i++)
            {

                //TextBox2.Text += std_Structure[i]+"\n"; //之後給學生看


                switch(std_Structure[i])//大寫
                {
                    case "FOR":
                        if (Singl_compare(teacher_setStructure, std_Structure[i + 1], "for"))
                        {
                            all_compare.Add(true);                        
                        }   
                        else
                        {
                            all_compare.Add(false);  
                        }
                        break;
                    case "WHILE":
                        if (Singl_compare(teacher_setStructure, std_Structure[i + 1], "while"))
                        {
                            all_compare.Add(true);                           
                        }
                        else
                        {
                            all_compare.Add(false);  
                        }                
                        break;
                    case "DOWHILE":
                        if (Singl_compare(teacher_setStructure, std_Structure[i + 1], "dowhile"))
                        {
                            all_compare.Add(true);                      
                        }
                        else
                        {
                            all_compare.Add(false);  
                        }
                        break;
                    case "IF":
                        if (Singl_compare(teacher_setStructure, std_Structure[i + 1], "if"))
                        {
                            all_compare.Add(true);
                        }
                        else
                        {
                            all_compare.Add(false);
                        }
                        break;
                    case "SWITCH":
                        if (Singl_compare(teacher_setStructure, std_Structure[i + 1], "switch"))
                        {
                            all_compare.Add(true);
                        }
                        else
                        {
                            all_compare.Add(false);
                        }
                        break;
                }

            }
            for (int j = 0; j < all_compare.Count; j++)
            {
                if (all_compare[j] != true)
                {
                    return false;
                }                
            }          
            return true;
        }
        private bool Singl_compare(string[] teacher_setStructure,string std_val,string type)//小寫
        {
            for (int i = 0; i < teacher_setStructure.Length; i++)
            {
                if (teacher_setStructure[i].Equals(type))//比對到一樣的結構
                {
                    try
                    {
                        int teacher = Convert.ToInt32(teacher_setStructure[i + 1]);
                        int std = Convert.ToInt32(std_val);
                        ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "][結構][" + teacher_setStructure[i].ToString() + "] 設定: " + teacher + "次→你的: " + std);
                        //ListBox_system_feedback.Items.Add("teacher_setStructure " + teacher_setStructure[i].ToString() + "   teacher " + teacher + " std" + std);
                        if ((teacher == 0 && std == 0) || (teacher > 0 && std <= teacher && std>=0))
                        {
                            return true;
                        }                 
                        else
                        {
                            return false;
                        }
                    }
                    catch(Exception ex)
                    {
                        System_log += "268 :: " + ex + "\n";
                        ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "][程式例外條件264]");
                    }                   
                }
            }
            return false;
        }
        private bool chech_file_exsit(string path,string id)
        {            
            if(File.Exists(path + "_Structure\\" + id + "Count.txt")&&File.Exists(path + "_Structure\\" + id_file + "Level.txt"))
            {return true;}
            else
            {return false;}
        }
        private void updata_DropDownList()
        {
            try
            {
                main_conn.Open();
                List<string> HWList = new List<string>();
                int count = 0;
                string Sql_cmd = "SELECT cppType FROM " + course + "HWList";
                SqlCommand myCommand = new SqlCommand(Sql_cmd, main_conn);
                SqlDataReader dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    HWList.Add(dr[0].ToString());
                }
                dr.Close();
                for (int i = 0; i < HWList.Count; i++)
                {
                    if (check_DP(id, HWList[i].ToString()))                   
                    {
                        DropDownList_HW.Items.Add(HWList[i].ToString());//裡面要塞還沒繳交功課清單  
                        count++;
                    }
                }
                if (count == 0)
                {
                    DropDownList_HW.Items.Clear();
                    DropDownList_HW.Items.Add("目前無缺作業");
                    DropDownList_HW.Enabled = false;
                }
            }
            catch (Exception x)
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "][程式例外條件423]");
            }
            finally 
            {
                main_conn.Close();
            }
            
           }
        private bool check_DP(string std_id,string HW)
        {
            List<string> ToF_Time = new List<string>();       
            string Sql_cmd = "SELECT ToF FROM " + HW + course + " WHERE std_id = '" + std_id + "'";
            SqlCommand myCommand = new SqlCommand(Sql_cmd, main_conn);       
            SqlDataReader dr = myCommand.ExecuteReader();            
            while (dr.Read())
            {
                ToF_Time.Add(dr[0].ToString());
            }
            dr.Close();
            for (int i = 0; i < ToF_Time.Count; i++)
            {
                if (ToF_Time[i].ToString().Replace(" ", "").Equals("True"))
                {
                    return false;
                }
            }
            return true;      
        }     
        private bool check_DB_Upload_repeat(string std_id)//用來檢察學生上傳幾次 正確後就不給上傳 
        {
            List<string> ToF_Time = new List<string>();
            List<string> Filename = new List<string>();
            string Sql_cmd = "SELECT ToF FROM " + DropDownList_HW.SelectedValue + course + " WHERE std_id = '" + std_id + "'";
            SqlCommand myCommand = new SqlCommand(Sql_cmd, main_conn);        
            SqlDataReader dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                ToF_Time.Add(dr[0].ToString());
            }
            dr.Close();
            for (int i = 0; i < ToF_Time.Count; i++)
            {
                if (ToF_Time[i].ToString().Replace(" ", "").Equals("True"))
                {
                    return false;
                }
            }        
            id_file = std_id +"-"+ToF_Time.Count;            
            return true;
        }      
        private void compiler_cpp(string Filename)
        {
            string toBat = course + DropDownList_HW.SelectedValue +" "+Filename;
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\compile.bat";
            process.StartInfo.Arguments = toBat;  //傳參數"%1 %2 %3" filename = upload.cpp
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string Error2end = process.StandardError.ReadToEnd();
            string result = process.StandardOutput.ReadToEnd();
            DateTime mydate = DateTime.Now;
            string mydatestring = mydate.ToString("yyyy-MM-dd HH:mm:ss");
            logtime = mydatestring;
            logPath = string.Format(logfolder + id_file + course + DropDownList_HW.SelectedValue + "{0:-MMddhhmm}.txt", DateTime.Now);
            string logdataname = logPath.Substring(0, logPath.Length);
            System_log += result+"\n";            
            process.WaitForExit();
        }
        private void Run_AI_System(string cpp,string path,string pic_name,string st_path,string st_name)//建立圖片
        {
            string tobat = cpp + " " + path + " " + pic_name + ".png " + st_path + " " + st_name;
            //ListBox_system_feedback.Items.Add("bat: "+tobat);
            using (Process AI_process = new Process())
            {
                //texecute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\test\OP.bat";
                AI_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\OP.bat";
                AI_process.StartInfo.Arguments = tobat;
                AI_process.StartInfo.RedirectStandardOutput = true;
                AI_process.StartInfo.RedirectStandardError = true;
                AI_process.StartInfo.UseShellExecute = false;
                AI_process.Start();
                AI_process.Dispose();
            };
            //這邊下WaitForExit 會卡住 可能是AI.jar沒讓他結束 程式碼要改
            
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            DropDownList_HW.Items.Clear();
            DropDownList_HW.Items.Add("請選擇上傳作業");
            updata_DropDownList();
            TextBox_Code.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            //compiler.Enabled = true;
            DropDownList_HW.SelectedIndex = 0;
            DropDownList_HW.Enabled = true;
            Session["IsPostBack"] = true;          
        }
        protected void Button3_Click(object sender, EventArgs e)//比對按鈕 (比對按鈕目前 要跟老師端對接 但是 老師端還沒有寫好 比對部分已經可以比對了是利用直接比對輸出的內容)
        {
            main_conn.Open();
            Button_excute.Visible = false;//關閉比對按鈕
            string fs = Session["s_fsname"].ToString();//在伺服器檔名 這邊會隨著次數更新
            string s_id = Session["s_id"].ToString();
            string png_path = Session["png_path"].ToString();
            string send_2bat = "";
            string teacher_script = "";
            teacher_script = getSQL_assign(null, DropDownList_HW.SelectedValue);//老師腳本 //隨機產生設定之測資 動態要改
            //send_2bat = cut_send2stdbat(teacher_script);
            process_compare(fs, teacher_script);//執行比對bat進行測資輸入 process_compare(學生檔案,測資) 

            StreamReader sr = new StreamReader(path + fs + ".txt", System.Text.Encoding.Default);//填路徑 學生
            StreamReader teacherscript = new StreamReader(@"C:\Users\Imnate\Imnate\Web\teacherupload\" + course + DropDownList_HW.SelectedValue + "\\" + teacher_script.Replace("輸入值腳本", "答案腳本"), System.Text.Encoding.Default);//動態要改
            // 一次讀取全部內容 
            string a = sr.ReadToEnd();
            string b = teacherscript.ReadToEnd();
            string ToF = "";
            TextBox2.Text = a;
            TextBox3.Text = b;
            while (!sr.EndOfStream)
            {
                TextBox2.Text = a;//學生               
            }
            sr.Close();
            while (!teacherscript.EndOfStream)
            {
                TextBox3.Text = b;
            }
            teacherscript.Close();

            if (string.Equals(TextBox3.Text, TextBox2.Text))//比對textbox3跟2是否一樣
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[成功]" + "[批改狀態] 正確答案");
                ToF = "True";
                TextBox2.ForeColor = Color.Green;
                ClientScript.RegisterStartupScript(GetType(), "message", "<script>alert('請將cpp檔案名稱改為 " + Encode_file(s_id) + ".cpp 並且上傳portal');</script>");
                Encode_file(s_id);
                try
                {
                    insert_file_name_toDB(s_id, fs, png_path + "\\" + fs, ToF, "'" + Encode_file(s_id) + "'");//將學生上傳檔案名稱 紀錄在DB 的Filename欄位
                }
                catch (Exception ex)
                {
                    insert_file_name_toDB(s_id, fs, "沒有產生圖片", ToF, "'" + Encode_file(s_id) + "'");
                    System_log += "464 :: " + ex + "\n";
                    ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + "[圖片狀態] 沒有產生圖片");
                }
                finally
                {
                    main_conn.Close();
                }

            }
            else
            {
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + "[批改狀態] 錯誤答案，請更正答案在上傳一次!");
                ToF = "False";
                TextBox2.ForeColor = Color.Red;
                wrong_answer();
                try
                {
                    insert_file_name_toDB(s_id, fs, png_path + "\\" + fs, ToF, "NULL");//將學生上傳檔案名稱 紀錄在DB 的Filename欄位
                }
                catch (Exception ex)
                {
                    insert_file_name_toDB(s_id, fs, "沒有產生圖片", ToF, "NULL");
                    System_log += "464 :: " + ex + "\n";
                    ListBox_system_feedback.Items.Add("[" + System_time.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[錯誤]" + "[圖片狀態] 沒有產生圖片");
                }
                finally
                {
                    main_conn.Close();
                }
            }
            
                
        }
        private string Encode_file(string input)
        {
            Random rand = new Random();
            int first_Level;
            int second_Level;
            List<string> list = new List<string>();
            string code ="";
            for (char c = 'A'; c <= 'Z'; c++)
            {
                list.Add(string.Format("{0}", Convert.ToChar(c)));
            }
            for (char i = 'a'; i <= 'z'; i++)
            {
                list.Add(string.Format("{0}", Convert.ToChar(i)));
            }
            for(int j = 0; j <=9;j++)
            {
                list.Add(string.Format("{0}", Convert.ToString(j)));
            }
          
            
            for(int i =0;i<input.Length;i++)
            {
                first_Level = rand.Next(0, list.Count-1);
                second_Level = rand.Next(0,Int32.Parse(input.Substring(i)));
               
                if (first_Level + second_Level < list.Count)
                {

                    code += list[first_Level + second_Level];
                }
                else
                {
                    code += list[first_Level];
                }
            }
           // ListBox_system_feedback.Items.Add(code);                 
            return code;
        }
        private void wrong_answer()
        { 
            Button_Reupdate.Visible = true;
            DropDownList_HW.Enabled = false;
            
        }
        private void insert_file_name_toDB(string s_id,string filename, string pic_path,string ToF,string encode)
        {
            if(Session["Ispostback"].Equals(true))
            {
                DateTime date = DateTime.Now;
                string date_string = date.ToString("yyyy-MM-dd");
                string time = date.ToString("HH:mm:ss");
                string cmd_insert;
                //string cmd_insert = "INSERT INTO " + DropDownList_HW.SelectedValue + course + "(std_id,Filename,pic_path,pic_byte,ToF,date,time) VALUES('" + s_id + "','" + filename + ".cpp" + "','" + pic_path + ".png" + "','" + "" + "','" + ToF + "','" + date_string + "','" + time + "')";
                //ListBox_system_feedback.Items.Add(File.Exists(pic_path+".png").ToString()+" "+pic_path+".png");
                cmd_insert = "INSERT INTO " + DropDownList_HW.SelectedValue + course + "(std_id,Filename,pic_path,ToF,date,time,Verif_Code) VALUES('" + s_id + "','" + filename + ".cpp" + "','" + pic_path + ".png" + "','" + ToF + "','" + date_string + "','" + time + "'," + encode + ")";
                SqlCommand myCommand = new SqlCommand(cmd_insert, main_conn);      
                myCommand.ExecuteNonQuery();
                Session["Ispostback"] = false;
            }
                  
        }
        private string cut_send2stdbat(string cuted_script)//截取老師資料庫記腳本名稱的測資
        {
            string replace = cuted_script.Replace(DropDownList_HW.SelectedValue, "").Replace("-","") ;
            return replace;
        }
        private string getSQL_assign(string class_id,string HW)//決定餵老師的哪個腳本名稱 要餵給bat要再切割
        {
            string assign = "";
            string cmd = "SELECT input_script FROM " + course + "HWList" + " WHERE cppType = '" + HW + "'";
            //string cmd = "SELECT assign FROM "+course+"HWList"+" WHERE cppType = '"+HW+"'";
            List<string> cuted = new List<string>();
            SqlCommand myCommand = new SqlCommand(cmd, main_conn);
            SqlDataReader dr;         
            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                assign += dr[0].ToString();            
            }
            dr.Close();          
            cuted = cut_assign_test(assign);//裁切資料庫測資(這裡面是老師測資腳本 須在截取出測資)
            Random randNumber = new Random();
            assign = cuted[randNumber.Next(0, cuted.Count - 1)].ToString();
            return cuted[randNumber.Next(0, cuted.Count-1)].ToString();
        }
        private List<string> cut_assign_test(string test_assignment)//換多輸入 bat 這邊不需要
        {
            string[] cut = test_assignment.Split(',');
            List<string> cut_array = new List<string>();
            foreach (string cuts in cut)
            {
                if (cuts.ToString()!="")
                {
                    cut_array.Add(cuts);
                }           
            }
            return cut_array;
        }
        private void process_compare(string Filename, string Test_arguments)
        {
            /*
             @echo off
             cd C:\Users\Imnate\Imnate\Web\Upload\%1
             %2.exe < C:\Users\Imnate\Imnate\Web\teacherupload\%3.txt > %4.txt
             exit
             */

            //StreamReader sr = new StreamReader(@"C:\Users\Imnate\Imnate\Web\teacherupload\" + course + DropDownList_HW.SelectedValue+@"\" + Test_arguments, System.Text.Encoding.Default);
            ////=== 一次讀取全部內容 ===
            //string a = sr.ReadToEnd();
            ////===逐行讀取，直到檔尾===
            //while (!sr.EndOfStream)
            //{
            //    a += sr;
            //}
            //TextBox_Code.Text = a;
            //sr.Close();





            string cmd = course + DropDownList_HW.SelectedValue + " " + Filename + " " + Test_arguments;
            TextBox2.Text += "cmd " + cmd;
            //string cmd = course + DropDownList_HW.SelectedValue+" "+ + " " + Test_arguments+" "+ Filename;
            Process execute_process = new Process();
            //execute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\新版execute.bat";
            execute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\std_execute_multiple.bat";
            execute_process.StartInfo.Arguments = cmd;
            execute_process.StartInfo.RedirectStandardOutput = true;
            execute_process.StartInfo.RedirectStandardError = true;
            execute_process.StartInfo.UseShellExecute = false;
            execute_process.Start();
            TextBox_Code.Text += execute_process.StandardOutput.ReadToEnd();
            TextBox2.Text += execute_process.StandardError.ReadToEnd();
           
            if (!execute_process.WaitForExit(1000 * 15))
            {
                execute_process.Kill();
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("MM-dd HH:mm:ss") + "]" + "[警告]" + " 程式執行超時");
                ListBox_system_feedback.Items.Add("[" + System_time.ToString("MM-dd HH:mm:ss") + "]" + "[錯誤]" + " 請檢查程式碼，無限迴圈嫌疑");
                No_limitLoop();//紀錄到無限迴圈表單
            }
            //execute_process.WaitForExit();  

          



        }
        private void No_limitLoop()
        {
            DateTime date = DateTime.Now;
            string time = date.ToString("yyyy-MM-dd HH:mm:ss");
            string cmd_insert = "INSERT INTO No_limtit_loop(std_id,course,hw,time) VALUES('" + id_file + "','" + course  + "','" + DropDownList_HW.SelectedValue.ToString() + "','" + time + "')";
            SqlCommand myCommand = new SqlCommand(cmd_insert, main_conn);        
            myCommand.ExecuteNonQuery(); 
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {        
            FileUpload.Visible = true;
            compiler.Enabled = true;
            string upcpptype = this.DropDownList_HW.SelectedValue;
            FileUpload.Enabled = true;
            DropDownList_HW.Enabled = false;
            Session["DropDownList"] = DropDownList_HW.SelectedValue;          
        }

    }
}