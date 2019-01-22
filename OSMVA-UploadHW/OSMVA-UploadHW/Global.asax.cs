using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace OSMVA_UploadHW
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // jQuery for ASP.NET
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
              new ScriptResourceDefinition
              {
                  Path = "~/jquery-3.1.1/jquery-3.1.1.min.js"
              }
            );
            ScriptManager.ScriptResourceMapping.AddDefinition("bootstrap",
              new ScriptResourceDefinition
              {
                  Path = "~/bootstrap-3.3.7-dist/js/bootstrap.min.js"
              }
            );
        }
    }
}