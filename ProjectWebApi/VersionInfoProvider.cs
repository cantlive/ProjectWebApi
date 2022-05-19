using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWebApi
{
    public static class VersionInfoProvider
    {
        public static void Initialize(string rcsVersion, DateTime buildDate)
        {
            RcsVersion = rcsVersion;
            BuildDate = buildDate;
        }

        public static string RcsVersion { get; set; }
        public static DateTime BuildDate { get; set; }
    }
}