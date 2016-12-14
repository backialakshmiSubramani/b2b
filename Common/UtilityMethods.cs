using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Modules.Channel.B2B.Common
{
    public static class UtilityMethods
    {
        public static IJavaScriptExecutor javaScriptExecutor = null;

        public static IAlert WaitGetAlert(this IWebDriver driver, TimeSpan timeOut)
        {
            IAlert alert = null;
            double timeInSeconds = timeOut.TotalSeconds;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeInSeconds));

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

        public static void WaitForTableLoadComplete(this IWebElement tableElement, TimeSpan timespan)
        {
            double timeoutInSecs = timespan.TotalSeconds;
            do
            {
                Thread.Sleep(5000);
                timeoutInSecs -= 5;
            } while (tableElement.FindElements(By.TagName("tr")).Count == 0 && timeoutInSecs > 0);
        }

        public static void WaitForPageLoadNew(this IWebDriver webDriver, TimeSpan timeSpan)
        {
            Thread.Sleep(5000);
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
                try
                {
                    if (element.Displayed)
                        return true;
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
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

        public static void WaitForTableRowCount(this IWebDriver webDriver, IWebElement webElement, Computation compute, int expectedCount)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromMinutes(1));
            wait.Until<bool>(d =>
            {
                int rowCount = webElement.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr")).Count;

                switch (compute)
                {
                    case Computation.EqualTo:
                        if (rowCount == expectedCount)
                            return true;
                        break;
                    case Computation.GreaterThan:
                        if (rowCount > expectedCount)
                            return true;
                        break;
                    case Computation.LessThan:
                        if (rowCount < expectedCount)
                            return true;
                        break;
                    case Computation.GreaterThanOrEqualTo:
                        if (rowCount >= expectedCount)
                            return true;
                        break;
                    case Computation.LessThanOrEqualTo:
                        if (rowCount <= expectedCount)
                            return true;
                        break;
                }
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

        public static FileInfo WaitForDownLoadToComplete(this IWebDriver webDriver, string dirPath, string fileNameStartsWith, DateTime anyTimeAfter, TimeSpan timespan)
        {
            FileInfo fileFound = null;
            double timeInSeconds = timespan.TotalSeconds;

            do
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Name.StartsWith(fileNameStartsWith, true, System.Globalization.CultureInfo.InvariantCulture) && file.CreationTime > anyTimeAfter && file.Extension == ".xml")
                    {
                        fileFound = file;
                        break;
                    }
                }

                if (fileFound == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    timeInSeconds -= 5;
                }
            } while (fileFound == null && timeInSeconds > 0);
            return fileFound;
        }

        public static DateTime ConvertToUtcTimeZone(this DateTime dateTime)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, timeZone);
        }

        public static DateTime ConvertToDateTime(this object value)
        {
            return Convert.ToDateTime(value);
        }

        public static bool CompareValues<T>(string fieldName, T actualValue, T expectedValue)
        {
            Type valueType = typeof(T);

            bool result = Convert.ChangeType(actualValue, typeof(T)).Equals((Convert.ChangeType(expectedValue, typeof(T))));
            if (!result)
                Console.WriteLine(string.Format("[FieldName]: {0} ---- [Actual]: {1}, [Expected]: {2} ---- [Match]: {3}", fieldName, actualValue, expectedValue, result));

            return result;
        }

        public static bool CompareActualAndExpectedValues<T,K>(
            T sourceElement,
            K expected,
            Expression<Func<T, object>> sourceExpression,
            Expression<Func<K, object>> expectedExpression = null)
        {
            MemberInfo expectedMemberInfo, actualMemberInfo;
            expectedMemberInfo = actualMemberInfo = GetMemberInfoFrom(sourceExpression);
            if (expectedExpression != null)
            {
                expectedMemberInfo = GetMemberInfoFrom(expectedExpression);
            }

            var actualValue = actualMemberInfo.GetValue(sourceElement);
            var expectedValue = expectedMemberInfo.GetValue(expected);

            if (actualMemberInfo.GetMemberType() == typeof(string))
            {
                actualValue = actualValue.ToString().Trim() ?? "";
                expectedValue = expectedValue.ToString().Trim() ?? "";
            }

            bool result = actualValue.Equals(expectedValue);
            if (!result)
                Console.WriteLine(string.Format("[FieldName]: {0} ---- [Actual]: {1}, [Expected]: {2} ---- [Match]: {3}", actualMemberInfo.Name, actualValue, expectedValue, result));

            return result;
        }

        public static bool CompareValues<T>(string fieldName, T actualValue, T expectedValue, Computation computation = Computation.EqualTo) where T : IComparable
        {
            bool result = false;

            switch (computation)
            {
                case Computation.EqualTo:
                    result = actualValue.Equals(expectedValue);
                    break;
                case Computation.GreaterThan:
                    result = actualValue.CompareTo(expectedValue) > 0;
                    break;
                case Computation.LessThan:
                    result = actualValue.CompareTo(expectedValue) < 0;
                    break;
                case Computation.GreaterThanOrEqualTo:
                    result = actualValue.CompareTo(expectedValue) >= 0;
                    break;
                case Computation.LessThanOrEqualTo:
                    result = actualValue.CompareTo(expectedValue) <= 0;
                    break;
                default:
                    break;
            }

            if (!result)
                Console.WriteLine(string.Format("[FieldName]: {0} ---- [Actual]: {1}, [Expected]: {2}, [Operation]: {3}", fieldName, actualValue, expectedValue, computation.ConvertToString()));

            return result;
        }

        public static string ConvertToString<T>(this T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            System.ComponentModel.DescriptionAttribute[] attributes =
                 (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
                 typeof(System.ComponentModel.DescriptionAttribute),
                 false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T ConvertToEnum<T>(this string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (field.CustomAttributes.Count() > 0 && field.CustomAttributes.First().ConstructorArguments.Count() > 0)
                {
                    string attribute = field.CustomAttributes.First().ConstructorArguments.First().Value.ToString();
                    if (attribute != null)
                    {
                        if (attribute == value || field.Name == value)
                            return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException(string.Format("Cannot convert string {0} to type {1}", value, typeof(T)));
        }

        public static List<string> GetColumnNames(this IWebElement tableElement)
        {
            List<IWebElement> columnHeaders = tableElement.FindElements(By.CssSelector("thead tr th")).ToList();

            List<string> columnNames = new List<string>();

            foreach (IWebElement element in columnHeaders)
                columnNames.Add(element.Text.Trim());

            return columnNames;
        }

        public static List<string> GetLogReportColumnNames(this IWebElement tableElement)
        {
            List<IWebElement> columnHeaders = tableElement.FindElements(By.CssSelector("tbody tr th")).ToList();
            List<string> columnNames = new List<string>();

            foreach (IWebElement element in columnHeaders)
                columnNames.Add(element.Text.Trim());

            return columnNames;
        }

        public static int GetLogReportColumnIndex(this IWebElement tableElement, string columnName)
        {
            List<string> columnNames = tableElement.GetLogReportColumnNames();
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (columnNames[i].Contains(columnName))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        public static int GetColumnIndex(this IWebElement tableElement, string columnName)
        {
            List<string> columnNames = tableElement.GetColumnNames();
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (columnNames[i].Contains(columnName))
                {
                    return i + 1;
                }
            }
            //if (columnIndex == 0)
            //    throw new Exception("Error: Table does not contain the given column name: " + columnName);

            return 0;
        }

        public static string GetLogReportCellValue(this IWebElement tableElement, int rowIndex, string columnName)
        {
            int columnIndex = tableElement.GetLogReportColumnIndex(columnName);
            if (columnIndex == 0)
                return null;

            string cellValue = tableElement.FindElement(By.CssSelector("tbody tr:nth-of-type(" + rowIndex + ") td:nth-of-type(" + columnIndex + ")")).Text;

            return cellValue;
        }

        public static string GetCellValue(this IWebElement tableElement, int rowIndex, string columnName)
        {
            int columnIndex = tableElement.GetColumnIndex(columnName);
            if (columnIndex == 0)
                return null;

            if (columnIndex > 4 && columnIndex < 7)
                columnIndex += 2;
            else if (columnIndex >= 7)
                columnIndex += 3;

            string cellValue = tableElement.FindElement(By.CssSelector("tbody tr:nth-of-type(" + rowIndex + ") td:nth-of-type(" + columnIndex + ")")).Text;

            return cellValue;
        }

        public static IWebElement GetCellElement(this IWebElement tableElement, int rowIndex, string columnName)
        {
            int columnIndex = tableElement.GetColumnIndex(columnName);

            if (columnIndex > 4 && columnIndex < 7)
                columnIndex += 2;
            else if (columnIndex >= 7)
                columnIndex += 3;

            var cellValue = tableElement.FindElement(By.CssSelector("tbody tr:nth-of-type(" + rowIndex + ") td:nth-of-type(" + columnIndex + ")"));

            return cellValue;
        }

        public static BrowserName GetBrowserName(this IWebDriver webDriver)
        {
            ICapabilities capabilities = ((RemoteWebDriver)webDriver).Capabilities;
            return ConvertToEnum<BrowserName>(capabilities.BrowserName);
        }

        public static void ClickElement(this IWebDriver webDriver, IWebElement webElement)
        {
            BrowserName browserName = webDriver.GetBrowserName();

            switch (browserName)
            {
                case BrowserName.InternetExplorer:
                    webElement.SendKeys(Keys.Enter);
                    break;
                case BrowserName.Chrome:
                    webElement.Click();
                    break;
                case BrowserName.MicrosoftEdge:
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();",webElement);
                    break;
            }
        }

        public static IWebDriver SwitchBrowser(this IWebDriver webDriver, BrowserName browserName)
        {
            webDriver.Quit();
            switch (browserName)
            {
                case BrowserName.Chrome:
                    ChromeOptions options = new ChromeOptions();
                    options.AddUserProfilePreference("safebrowsing.enabled", "true");
                    options.AddUserProfilePreference("download.default_directory", ConfigurationManager.AppSettings["CatalogDownloadPath"]);
                    options.AddArguments("--start-maximized");
                    webDriver = new ChromeDriver(options);
                    break;
                case BrowserName.InternetExplorer:
                    webDriver = new InternetExplorerDriver();
                    webDriver.Manage().Window.Maximize();
                    break;
                case BrowserName.MicrosoftEdge:
                    webDriver = new EdgeDriver();
                    break;
            }

            return webDriver;
        }

        public static void SelectOptionFromDDL(IWebDriver webDriver, IWebElement webElement, string xPath)
        {
            UtilityMethods.ClickElement(webDriver, webElement);
            IWebElement textElement = webElement.FindElement(By.XPath(xPath));
            UtilityMethods.ClickElement(webDriver, textElement);
        }

        #region Private Methods

        private static MemberInfo GetMemberInfoFrom<T>(Expression<Func<T, object>> sourceExpression)
        {
            var lambda = sourceExpression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            return memberExpression.Member;
        }

        #endregion Private Methods
    }

    public static class MemberInfoExtension
    {
        public static object GetValue(this MemberInfo memberInfo, object objInstance)
        {
            if (memberInfo is FieldInfo)
            {
                var fieldInfo = memberInfo as FieldInfo;
                return fieldInfo.GetValue(objInstance);
            }

            var propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo.GetValue(objInstance);
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
            {
                var fieldInfo = memberInfo as FieldInfo;
                return fieldInfo.FieldType;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo.PropertyType;
        }
    }
}
