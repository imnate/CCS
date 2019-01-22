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
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            execute();
        }
        private void execute()
        {
            string[] cut = TextBox1.Text.Split(',');
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
            }

            for (int i = 0; i < cut_array.Count; i++)
            {
                TextBox2.Text += cut_array[i]+"\r\n";
            }
            for(int i =0;i<cut_array.Count;i++)
            {
                Process texecute_process = new Process();
                //texecute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\teacherupload\CS_106test123\bat.bat";
                texecute_process.StartInfo.FileName = @"C:\Users\Imnate\Imnate\Web\teacherupload\CS_106test123\bat.bat";
                //texecute_process.StartInfo.Arguments = string.Format("{0} {1}", 15, "test123");
                texecute_process.StartInfo.Arguments = string.Format("{0} {1}", cut_array[i].ToString(), "test123");
                //texecute_process.StartInfo.Arguments = string.Format("{0} {1} {2}", '"'+cut_array[i].ToString()+'"', "test123",i);
                //texecute_process.StartInfo.Arguments = string.Format("{0} {1} {2} {3}", '"'+course+'"' +'"' +exe_name+'"', +'"'+test_num+'"', '"'+exe_name+'"', '"'+filenum+'"');
                //string.Format("{0} {1}", Argument_1,Argument_2) ; 另一種送參數到bat裡面
                texecute_process.StartInfo.RedirectStandardOutput = true;
                texecute_process.StartInfo.RedirectStandardError = true;
                texecute_process.StartInfo.UseShellExecute = false;
                texecute_process.Start();

                TextBox2.Text += texecute_process.StandardOutput.ReadToEnd();
                TextBox2.Text += texecute_process.StandardError.ReadToEnd();
                TextBox2.Text += " bat = "+string.Format("{0}", cut_array[i], "test123", "test123.exe", i);

                texecute_process.WaitForExit();

            }
        }
    }
}