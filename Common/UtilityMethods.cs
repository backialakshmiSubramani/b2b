using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Channel.B2B.Common
{
    public static class UtilityMethods
    {
        public static IJavaScriptExecutor javaScriptExecutor = null;

        public static IAlert WaitGetAlert(this IWebDriver driver, int waitTimeInSeconds = 10)
        {
            IAlert alert = null;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTimeInSeconds));

            try
            {
                alert = wait.Until(d =>
                {
                    try
                    {
                        return driver.SwitchTo().Alert();
                    }
                    catch (NoAlertPresentException)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException) { alert = null; }

            return alert;
        }

        public static DataTable GetDataFromExcel(string filePath, string query)
        {
            string sConnection = null;
            OleDbConnection oleCon = default(OleDbConnection);
            OleDbDataAdapter oleDA = default(OleDbDataAdapter);
            DataTable dataTable = new DataTable();

            sConnection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"", filePath);

            oleCon = new OleDbConnection(sConnection);
            oleCon.Open();

            oleDA = new OleDbDataAdapter(query, oleCon);
            oleDA.Fill(dataTable);

            oleCon.Close();
            return dataTable;
        }

        public static void WaitForTableLoadComplete(this IWebElement tableElement, int timeoutInSecs)
        {
            do
            {
                Thread.Sleep(5000);
                timeoutInSecs -= 5;
            } while (tableElement.FindElements(By.TagName("tr")).Count == 0 && timeoutInSecs > 0);
        }

        public static void WaitForPageLoadNew(this IWebDriver webDriver, TimeSpan timeSpan)
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(webDriver, timeSpan);

            wait.Until(driver1 => ((IJavaScriptExecutor)webDriver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static T ConvertValue<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static int RoundValue(this object value)
        {
            return ConvertValue<int>(Math.Round(ConvertValue<decimal>(value), MidpointRounding.AwayFromZero));
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static void WaitForElement(this IWebDriver driver, IWebElement element, int timeoutInSeconds = 30)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until<bool>(d =>
            {
                if (element.Displayed)
                    return true;
                return false;
            });
        }

        public static void WaitForTableRowCount(this IWebDriver driver, IWebElement element, int rowCount, int timeoutInSeconds = 30)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until<bool>(d =>
            {
                if (element.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr")).Count == rowCount)
                    return true;
                return false;
            });
        }

        public static void WaitForElement(this IWebDriver driver, By by, int timeoutInSeconds = 30)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until<bool>(d =>
            {
                if (driver.FindElement(by).Displayed)
                    return true;
                return false;
            });
        }
    }
}
