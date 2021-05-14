using InDepthSearch.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services
{
    public class AppService : IAppService
    {
        public string GetVersion()
        {
            var assemblyVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return assemblyVer != null ? assemblyVer.Major.ToString() + "." + assemblyVer.Minor.ToString() + "."
                + assemblyVer.Build.ToString() : "x.x.x";
              
        }
    }
}
