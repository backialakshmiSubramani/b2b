using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;
using System.Reflection;


namespace Modules.Channel.B2B.Core.Pages
{/// <summary>
    /// This class contains methods used commonly for Pages.
    /// </summary>
    public static class PageUtility
    {
        public static int PageTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PageTimeout"]);
        public static int ControlTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["ControlTimeout"]);
        public static int SleepTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["SleepTimeOut"]);
        public static int DBTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["DBTimeout"]);
        public static int CPTTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["CPTTimeOut"]);
     
        public static void WaitFor(IWebDriver Driver, Func<IWebDriver, bool> waitCondition, int timeout = 60)
        {
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, PageUtility.PageTimeOut));
            wait.Until(waitCondition);

        }


        public static string DecryptPassword()
        {
            byte[] pwdByteData = Convert.FromBase64String(ConfigurationManager.AppSettings["SitePwd"]);

            return System.Text.Encoding.Unicode.GetString(pwdByteData);

        }

    
        /// <summary>
        /// Use this method to wait till the page has loaded (till the 'document.readyState' gives 'complete')
        /// </summary>
        /// <param name="webDriver"></param>
        public static void WaitForPageRefresh(IWebDriver webDriver)
        {
            var isloaded = string.Empty;
            do
            {
                Thread.Sleep(4000);

                try
                {
                    isloaded = ((IJavaScriptExecutor)webDriver).ExecuteScript("return window.document.readyState") as string;
                }
                catch
                {
                    // ignored
                }
            } while (isloaded != "complete");
        }
    }

    /// <summary>
    /// This class is used to add the String value as attribute
    /// </summary>
    public class StringValue : System.Attribute
    {
        private string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }

    /// <summary>
    /// This class is used to retrieve the string value from the attribute
    /// </summary>
    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
