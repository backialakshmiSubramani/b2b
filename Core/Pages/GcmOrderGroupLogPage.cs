// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/23/2014 10:00:53 AM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/23/2014 10:00:53 AM
// ***********************************************************************
// <copyright file="GcmOrderGroupLogPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;


namespace Modules.Channel.B2B.Core.Pages
{
    using System.Linq;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class GcmOrderGroupLogPage : DCSGPageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public GcmOrderGroupLogPage(IWebDriver webDriver)
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

        private IWebElement OnlineCustomerNumber
        {
            get
            {
                return webDriver.FindElement(By.XPath("//td[.='Customer Number:']/following-sibling::td[1]"));
            }
        }

        private IWebElement OmsCustomerNumber
        {
            get
            {
                return webDriver.FindElement(By.XPath("//td[.='Customer Number:']/following-sibling::td[2]"));
            }
        }

        private IWebElement PrintFaxViewLink
        {
            get
            {
                return webDriver.FindElements(By.XPath("//a[img]"))[0];
                //return webDriver.FindElement(By.XPath("//a[img/@src='/GCM/GCMGlobal/Images/printfax_view.gif']"));
                //return webDriver.FindElement(By.XPath("//a/img[@src='/GCM/GCMGlobal/Images/printfax_view.gif'"));
            }
        }

        private IWebElement CustomerLink
        {
            get
            {
                //return webDriver.FindElements(By.XPath("//a[@class='lnk_mhtab']"))[1];
                //return webDriver.FindElement(By.XPath("//a[.='Customer']"));
                return webDriver.FindElement(By.LinkText("Customer"));
            }
        }

        public void GoToPrintFaxViewPage()
        {
            ////PrintFaxViewLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PrintFaxViewLink);
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
        }

        public bool DoCustomerNumbersMatch()
        {
            if (!OnlineCustomerNumber.Text.Trim().Equals(OmsCustomerNumber.Text.Trim()))
            {
                return false;
            }

            return true;
        }

        public void GoToCustomerContactsPage()
        {
            ////CustomerLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CustomerLink);
        }
    }
}
