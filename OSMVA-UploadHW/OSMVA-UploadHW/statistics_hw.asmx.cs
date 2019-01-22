using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;


namespace OSMVA_UploadHW
{
    /// <summary>
    ///statistics_hw 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]    
    public class statistics_hw : System.Web.Services.WebService
    {
        int daily_true_time;
        int daily_false_time;     
        int upload_std = 0;//圓餅圖 嘗試上傳學生
        int total_true = 0;//圓餅圖 嘗試上傳答對
        SqlConnection main_DB = DBM.getconn_main();
        List<send_hw_uploads_count_data> shcd = new List<send_hw_uploads_count_data>();
        List<HWRecord> output_time_And_ToF = null;
        [WebMethod(EnableSession = true)]//允許使用session           
        public void hw_uploads_count(string hw)
        {
            DateTime time =  DateTime.Now;          
           
            List<string> date_Interval = new List<string>();
            
            if (Session["course"] != null)
            {
                main_DB.Open();
                string course = Session["course"].ToString();             
                string cmd = "select date from " + hw + course + " order by 'date'";
                
                List<string> output_date = new List<string>();
                List<double> output_avg_true = new List<double>();
                List<double> output_avg_false = new List<double>();
                SqlCommand S_cmd = new SqlCommand(cmd, main_DB);
                SqlDataReader dr = S_cmd.ExecuteReader();

                while (dr.Read())
                {                  
                    if (date_Interval.Count == 0 || find_repeat(dr[0].ToString(), date_Interval))
                    {
                      
                      //send_hw_uploads_count_data temp = new send_hw_uploads_count_data(); //傳資料出去
                      date_Interval.Add(dr[0].ToString());//有年份的日期
                      output_date.Add(dr[0].ToString().Replace(dr[0].ToString().Substring(0, 5), ""));//無年分輸出折線圖
                      //temp.date = dr[0].ToString().Replace(dr[0].ToString().Substring(0,5), "");                      
                      //shcd.Add(temp);//傳資料出去
                    }
                                      
                }
                dr.Close();
                
                for (int i = 0; i < date_Interval.Count(); i++)//重複性高 承接人員 有空請精簡
                {
                    int cout_tatol_std_T = 0;
                    int cout_tatol_std_F = 0;
                    daily_true_time = 0;
                    daily_false_time = 0;
                    S_cmd.CommandText = "select FileName from " + hw + course + " Where date ='"+date_Interval[i].ToString()+"' And ToF = 'True'";//當日true
                    dr = S_cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cout_tatol_std_T++;
                        string[] cut_of_time = dr[0].ToString().Split('-');
                        daily_true_time += Convert.ToInt32(cut_of_time[1].Replace(".cpp", "")) + 1;                      
                        Console.Write(dr[0].ToString());
                    }

                    if (cout_tatol_std_T == 0)
                    {
                        output_avg_true.Add(0);
                    }
                    else
                    {
                        double d_avg_true = daily_true_time / cout_tatol_std_T;
                        output_avg_true.Add(Math.Round(d_avg_true, 2));
                    }
                    dr.Close();
                    S_cmd.CommandText = "select FileName from " + hw + course + " Where date ='" + date_Interval[i].ToString() + "' And ToF = 'False'";//當日true
                    dr = S_cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cout_tatol_std_F++;
                        string[] cut_of_time = dr[0].ToString().Split('-');
                        daily_false_time += Convert.ToInt32(cut_of_time[1].Replace(".cpp", "")) + 1;                    
                    }
                    if (cout_tatol_std_F == 0)
                    {
                        output_avg_false.Add(0);
                    }
                    else
                    {
                        double d_avg_false = daily_false_time / cout_tatol_std_F;
                        d_avg_false = Math.Round(d_avg_false, 2);
                        output_avg_false.Add(d_avg_false);
                    }
                    dr.Close();
                }
            
                main_DB.Close();

                #region

                for (int j = 0; j < output_avg_true.Count(); j++)
                {
                    send_hw_uploads_count_data temp = new send_hw_uploads_count_data(); //傳資料出去
                    temp.date = output_date[j].ToString();
                    temp.true_std = output_avg_true[j];
                    temp.false_std = output_avg_false[j];
                    shcd.Add(temp);
                }

                #endregion



            }

           
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(shcd));//這邊是C# object 轉 json format           
        }

        [WebMethod(EnableSession = true)]//允許使用session
        public void avg_correct(string hw ,string mode)
        {
            if (Session["course"] != null)
            {               
                    string course = Session["course"].ToString();
                    string a = hw + course;
                    int MAX_temp_time = 0;
                    output_time_And_ToF = new List<HWRecord>();
                    List<string> s_id = new List<string>();//全班名單
                    List<float> output_avg = new List<float>();
                    main_DB.Open();
                    string cmd = "select Filename,ToF from " + hw + course;
                    SqlCommand sql_cmd = new SqlCommand(cmd, main_DB);
                    SqlDataReader dr = sql_cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (split_Upload_time(dr[0].ToString()) > MAX_temp_time)//找最大的值
                        {
                            MAX_temp_time = split_Upload_time(dr[0].ToString());
                        }
                        output_time_And_ToF.Add(
                            new HWRecord
                            {
                                UploadCount = split_Upload_time(dr[0].ToString()),
                                ToF = bool.Parse(dr[1].ToString())
                            });
                    }
                    
                    output_avg = session_percent(MAX_temp_time, output_time_And_ToF);//指向
                    dr.Close();
                    sql_cmd.CommandText = "select S_ID from " + course;//抓全班名單
                    dr = sql_cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        s_id.Add(dr[0].ToString());
                    }
                    dr.Close();
                    main_DB.Close();

                    if (mode =="0")//寫入圓餅圖資料
                    {
                        chart_3D_Donut chart_3D = new chart_3D_Donut();
                        List<chart_3D_Donut> c3d = new List<chart_3D_Donut>();
                        chart_3D.total_std = s_id.Count;//全部學生
                        chart_3D.try_upload_std = upload_std;
                        chart_3D.no_try_upload_std = s_id.Count - upload_std;//未嘗試上傳學生
                        chart_3D.try_upload_true = total_true;
                        chart_3D.try_upload_false = upload_std - total_true;//嘗試上傳答錯學生
                        c3d.Add(chart_3D);
                        JavaScriptSerializer js1 = new JavaScriptSerializer();
                        Context.Response.Write(js1.Serialize(c3d));//這邊是C# object 轉 json format
                    }
                    //計算各區間正確率
                    else if (mode == "1")//寫入彩色長條圖資料
                    {
                        for (int i = 0; i < MAX_temp_time; i++)
                        {
                            send_hw_uploads_count_data temp = new send_hw_uploads_count_data(); //傳資料出去
                            temp.upload_session = i + 1;
                            temp.avg = output_avg[i];
                            shcd.Add(temp);
                        }

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Context.Response.Write(js.Serialize(shcd));//這邊是C# object 轉 json format
                    }
            }

        }
        private List<float> session_percent(int max_upload, List<HWRecord> data)
        {
            List<float> output_avg = new List<float>();
            for (int i = 1; i <= max_upload; i++)
            {
                List<HWRecord> items = data.FindAll(o => o.UploadCount.Equals(i));//o會去找data裡面的UploadCount元素等於i的會存到o裡面再pass過去item      
                if(i==1)
                {
                    upload_std = items.Count; //嘗試上傳學生                  
                }
                if (items != null)//防爆裝置
                {
                    List<HWRecord> truepeople = items.FindAll(o => o.ToF);//這邊會找全部的true相關存在list裡面
                    float result = (float)truepeople.Count / upload_std * 100;
                    total_true += truepeople.Count;//答對人數
                    output_avg.Add(result);
                }          
            }
            
            return output_avg;
        }
        private int split_Upload_time(string data)
        {
            string[] temp_spile = data.Replace(".cpp", "").Split('-');
            return Convert.ToInt32(temp_spile[1]) + 1;//+1是因為資料庫我的記法是0開始
        }
        private bool find_repeat(string date,List<string>date_list)
        {
            for (int i = 0; i < date_list.Count(); i++)
            {
                if (date.Equals(date_list[i].ToString()))
                {
                    return false;
                }
            
            }
                return true;
        }
    }
}
