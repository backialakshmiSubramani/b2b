using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;

namespace Modules.Channel.B2B.Core.NewPages
{
    public class OstStandardConfigurationPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstStandardConfigurationPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            throw new NotImplementedException();
        }
        
        private IWebElement TxtDiscount
        {
            get
            {
                 return webDriver.FindElement(By.Id("txtDiscount"), TimeSpan.FromSeconds(30));                
            }
        }
        
        private IWebElement ContentPageHolderFrame
        {
            get
            {               
                    return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ifrmRedirect"), TimeSpan.FromSeconds(30));                
            }
        }

        public IWebElement BtnApplyChanges
        {
            get
            {
                return webDriver.FindElement(By.Id("btnApplyChanges"));  
            }
        }

        public IWebElement BtnUpdateNow
        {
            get
            {
                return webDriver.FindElement(By.Id("btnUpdateNow"));
            }
        }

        public string CurrentConfigPrice
        {
            get
            {
                return webDriver.FindElement(By.Id("lblConfigPrice")).Text;
            }
        }

        private IWebElement StandardConfigFrame
        {
            get
            {                
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ifrmRedirect"), TimeSpan.FromSeconds(30));
            }
        }
       
        public void SelectLiveConfig(string orderCode)
        {
            webDriver.SwitchTo().Frame(StandardConfigFrame);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));

            javaScriptExecutor.ExecuteScript("arguments[0].click();", webDriver.FindElement(By.XPath("//td[text() = '" + orderCode + "']")));
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(30));
        }

        public void OpenPriceChangeDialog(string linkId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            webDriver.FindElement(By.Id(linkId)).SendKeys(Keys.Enter);
        }        

        public void SwitchWindowToPricingPopupToApplyDiscount(string discount)
        {
            string oldWindow = webDriver.CurrentWindowHandle;
            string popWindowHandle = null;
            WebDriverWait wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 5));
            wait.Until((d) => webDriver.WindowHandles.Count == 2);
            var windowHandles = webDriver.WindowHandles;
            ReadOnlyCollection<string> handles = new ReadOnlyCollection<string>(windowHandles);
            foreach (string handle in handles)
            {
                if (handle != oldWindow)
                {
                    popWindowHandle = handle;
                }
            }
            webDriver.SwitchTo().Window(popWindowHandle);
            TxtDiscount.Clear();
            TxtDiscount.SendKeys(discount);
            BtnApplyChanges.SendKeys(Keys.Enter);
            webDriver.SwitchTo().Window(oldWindow);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
        }

        private IAlert alertBox;
        public bool UpdateConfigrationByUpdateNow()
        {
            BtnUpdateNow.SendKeys(Keys.Enter);

            bool isAlertExist = false;

            while (!isAlertExist)
            {
                isAlertExist = IsAlertExists();
            }

             
            string alertMsg = alertBox.Text;
            alertBox.Accept();

            if (alertMsg.Contains("Pinpoint refresh call is successful."))
            {
                return true;
            }

            return false;            
        }

        public void SwitchToFrame()
        {
            webDriver.SwitchTo().Frame(ContentPageHolderFrame);
        }

        private bool IsAlertExists()
        {
            try
            {                
                alertBox = webDriver.SwitchTo().Alert();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
