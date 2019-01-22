using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
namespace OSMVA_UploadHW
{
    /// <summary>
    ///table_refresh 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class table_refresh : System.Web.Services.WebService
    {          
        SqlConnection main_conn;
        string id = "";
        string course = "";
        [WebMethod(EnableSession = true)]//亂放位子有錯誤 要放在function上面 有[]教驗證
        public void getHwTable()
        {     
            List<hw_status> hw_status_list = new List<hw_status>();
            #region         
            if (Session["ID"] != null && Session["course"] != null)
            {              
                main_conn = DBM.getconn_main();
                main_conn.Open();
                id = Session["ID"].ToString();
                course = Session["course"].ToString();


                    string Sql_cmd_Hwlist = "select cppType from " + course + "HWList";
                    List<string> hwlist = new List<string>();
                    SqlCommand myCommand1 = new SqlCommand(Sql_cmd_Hwlist, main_conn);
                    SqlDataReader dr1 = myCommand1.ExecuteReader();


                    while (dr1.Read())
                    {
                        hwlist.Add(dr1[0].ToString());
                    }
                    dr1.Close();

                    for (int i = 0; i < hwlist.Count; i++)
                    {
                        hw_status temp = new hw_status();
                        string Sql_cmd_true = "select FileName,date,Verif_Code from " + hwlist[i].ToString() + course + " Where ToF = 'True' AND std_id ='" + id + "'";
                        SqlCommand myCommand2 = new SqlCommand(Sql_cmd_true, main_conn);
                        SqlDataReader dr2 = myCommand2.ExecuteReader();
                        if (dr2.HasRows)//有繳交正確
                        {
                            while (dr2.Read())
                            {                                
                                temp.hw_name = hwlist[i].ToString();
                                temp.hw_update = dr2[1].ToString();
                                temp.hw_times = count_time(dr2[0].ToString());
                                temp.hw_stuts = "答案正確";
                                if (dr2[2].Equals(DBNull.Value))
                                {
                                    temp.hw_encode = "無驗證碼";
                                }
                                else
                                {
                                    temp.hw_encode = dr2[2].ToString();
                                }
                            }
                            hw_status_list.Add(temp);
                            
                        }
                        else
                        {
                            dr2.Close();
                            string Sql_cmd_false = "select FileName,date,Verif_Code from " + hwlist[i].ToString() + course + " Where ToF = 'False' AND std_id ='" + id + "'";
                            SqlCommand myCommand3 = new SqlCommand(Sql_cmd_false, main_conn);
                            SqlDataReader dr3 = myCommand3.ExecuteReader();
                            if (dr3.HasRows)
                            {
                                while (dr3.Read())
                                {
                                    temp.hw_name = hwlist[i].ToString();
                                    temp.hw_update = dr3[1].ToString();
                                    temp.hw_times = "已嘗試 " + count_time(dr3[0].ToString());//次數
                                    temp.hw_stuts = "答案不正確";
                                    temp.hw_encode = "尚未繳交成功無驗證碼";
                                }
                                hw_status_list.Add(temp);
                                dr3.Close();                              
                            }                                
                            else
                            {
                                dr3.Close();
                                temp.hw_name = hwlist[i].ToString();
                                temp.hw_update = "尚未繳交";
                                temp.hw_times =  "尚未繳交";
                                temp.hw_stuts = "尚未繳交";
                                temp.hw_encode = "尚未繳交成功無驗證碼";
                                hw_status_list.Add(temp);                           
                            }
                           
                        }
                        dr2.Close();
                    }
                    main_conn.Close();                 
            }
            #endregion //1212121212

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(hw_status_list));//這邊是C# object 轉 json format
            Console.Write("測試");
        }
        private string count_time(string time)
        {
            string count = time.Replace(id + "-", "");
            count = count.Replace(".cpp","");
            int con_count = Convert.ToInt32(count)+1;          
            return con_count.ToString()+ "次";
        }      
    }
}
