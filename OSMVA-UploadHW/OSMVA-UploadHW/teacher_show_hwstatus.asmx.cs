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
    ///teacher_show_hwstatus 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class teacher_show_hwstatus : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]//允許使用session
        public void show_status()
        {
            List<send_teacher_hwstatus> sth_list = new List<send_teacher_hwstatus>();
       
            if (Session["ID"] != null && Session["course"] != null)
            {
                string course = Session["course"].ToString();
                string id = Session["ID"].ToString();
                SqlConnection main_conn;
                main_conn = DBM.getconn_main();
                main_conn.Open();
                string Sql_cmd = "Select cppType from "+ course +"HWList order by 'cppType'";
                List<string> hw_list = new List<string>();
                SqlCommand myCommand = new SqlCommand(Sql_cmd, main_conn);
                SqlDataReader dr = myCommand.ExecuteReader();
                
                while(dr.Read())
                {
                    hw_list.Add(dr[0].ToString());                   
                }
                dr.Close();
                for (int i = 0; i < hw_list.Count; i++)
                {
                    send_teacher_hwstatus temp = new send_teacher_hwstatus();   
                    myCommand.CommandText = "select * from " + course + "HWList where cppType ='" + hw_list[i]+ "'";              
                    dr = myCommand.ExecuteReader();

                        while (dr.Read())
                        {
                            temp.hw_name += hw_list[i];
                            temp.hw_answer_script += dr[1].ToString();
                            temp.hw_input_script += dr[2].ToString();
                            temp.hw_mode += dr[3].ToString();
                            temp.hw_structure_condtion += dr[4].ToString();
                        }
                        dr.Close();                  
                        sth_list.Add(temp);

                }  
               
                main_conn.Close();
               
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(sth_list));//這邊是C# object 轉 json format
           
        }
       
    }
}
