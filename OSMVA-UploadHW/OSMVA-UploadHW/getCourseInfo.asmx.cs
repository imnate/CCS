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
    ///getCourseInfo 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class getCourseInfo : System.Web.Services.WebService
    {
        SqlConnection main_DB = DBM.getconn_main();
        [WebMethod(EnableSession = true)]
        public void gCI(string hw)
        {
            if (Session["course"] != null)
            {
                send_CourseInfo sCI = new send_CourseInfo();
                List<send_CourseInfo> L_sCI = new List<send_CourseInfo>();
                string course = Session["course"].ToString();
                main_DB.Open();
                string cmd = "select S_ID from " + course;
                SqlCommand S_cmd = new SqlCommand(cmd, main_DB);
                SqlDataReader dr = S_cmd.ExecuteReader();
                List<string> course_num = new List<string>();
                while(dr.Read())
                {
                    course_num.Add(dr[0].ToString());
                }
                dr.Close();
                main_DB.Close();
                sCI.Course_id = course;
                sCI.Course_num = course_num.Count();
                L_sCI.Add(sCI);
                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(L_sCI));//這邊是C# object 轉 json format 
            }
        }
    }
}
