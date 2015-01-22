// ***********************************************************************
// Author           : AMERICAS\O_Maity
// Created          : 12/5/2014 6:17:25 PM
//
// Last Modified By : AMERICAS\O_Maity
// Last Modified On : 12/5/2014 6:17:25 PM
// ***********************************************************************
// <copyright file="OstShipToAddress.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OstShipToAddressPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstShipToAddressPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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

        //private IWebElement ShipToAddSelectButton
        //{
        //    get
        //    {
        //        return webDriver.FindElement(By.Id("TabContainerAddress_TabPanelShiptoAddress_AffinityShipAddress_ddlSearchParams"));
        //    }
        //}

        private SelectElement SelectShipToAddlElement
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("TabContainerAddress_TabPanelShiptoAddress_AffinityShipAddress_ddlSearchParams")));
            }
        }

        private IWebElement SelectShipToAddlDropdown
        {
            get
            {
                return webDriver.FindElement(By.XPath("//select[@id='TabContainerAddress_TabPanelShiptoAddress_AffinityShipAddress_ddlSearchParams']"));

            }
        }

        public bool SelectShipToAddOptions()
        {
            ////SelectShipToAddlDropdown.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SelectShipToAddlDropdown);
            return SelectShipToAddlElement.Options.Any(e => e.Text.Contains("Local Channel #"));
        }
    }
}
