using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;




namespace WMS.Services.Utility
{
    public static class AppSettings
    {
        public static string PrintCCEmail
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["PRINTTCC"].ToString()) ? "" : ConfigurationManager.AppSettings["PRINTTCC"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        public static string ProductPath
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebApiPath"].ToString()) ? "" : ConfigurationManager.AppSettings["WebApiPath"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        public static string PublicSiteAddress
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["PublicSite"].ToString()) ? "" : ConfigurationManager.AppSettings["PublicSite"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        public static string EmailServicePath
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailServicePath"].ToString()) ? "" : ConfigurationManager.AppSettings["EmailServicePath"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        public static string TOCC
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["TOCC"].ToString()) ? "" : ConfigurationManager.AppSettings["TOCC"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
        }
        public static string BCC
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(ConfigurationManager.AppSettings["BCC"].ToString()) ? "" : ConfigurationManager.AppSettings["BCC"].ToString();
                }
                catch (Exception ex)
                {
                    return "";
                }

            }
        }
    }
}