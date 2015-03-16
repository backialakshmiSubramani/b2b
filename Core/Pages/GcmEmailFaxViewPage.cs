// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/23/2014 10:10:34 AM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/23/2014 10:10:34 AM
// ***********************************************************************
// <copyright file="GcmEmailFaxViewPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;

namespace Modules.Channel.B2B.Core.Pages
{
    using System.Linq;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class GcmEmailFaxViewPage : PageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public GcmEmailFaxViewPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";

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

        private IWebElement SubTotal
        {
            get
            {
                //return webDriver.FindElement(By.XPath("/html/body/form/div[3]/table/tbody/tr[5]/td[2]/table/tbody/tr[3]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td[2]"));
                return webDriver.FindElement(By.XPath("(//td[.='Subtotal:'])[1]/following-sibling::td[1]"));
            }
        }

        private IReadOnlyCollection<IWebElement> EndUserDetailsTable
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath("//div[@id='ctl378_xpnd']/table/tbody/tr[5]/td[2]/table/tbody/tr/td[2]"));
            }
        }

        private IWebElement ViewXmlLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[img[@src='/GCM/GCMGlobal/Images/xml.gif']]"));
            }
        }

        public string GetSubTotalValue()
        {
            return SubTotal.Text;
        }

        public List<string> GetEndUserDetails()
        {
            return EndUserDetailsTable.Select(e => e.Text).Skip(1).Take(7).ToList();
        }

        public void GoToXmlResultsPage()
        {
            ////ViewXmlLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ViewXmlLink);
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
        }
    }
}
