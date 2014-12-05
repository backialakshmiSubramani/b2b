// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/3/2014 3:40:16 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/3/2014 3:40:16 PM
// ***********************************************************************
// <copyright file="OstAddressWizardPage.cs" company="Dell">
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
    public class OstAddressWizardPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstAddressWizardPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
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

        private SelectElement BillToAddSearchByField
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_ddlSearchParams")));
            }

        }

        private IWebElement BillToAddSearchField
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$ddlSearchParams"), TimeSpan.FromSeconds(20));
                //return webDriver.FindElement(By.Id("TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_txtSearch"));
                return webDriver.FindElement(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$ddlSearchParams"));
            }

        }

        private IWebElement BillToAddressOption
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$txtSearch"), TimeSpan.FromSeconds(20));
                return webDriver.FindElement(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$txtSearch"));
            }

        }

        private IWebElement SelectButton
        {
            get
            {
                return webDriver.FindElement(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$imgbtnSearch"));
            }
        }

        private IWebElement SelecttLocalChannel
        {
            get
            {
                return webDriver.FindElement(By.XPath("//select[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_ddlSearchParams']"));

            }
        }

        private SelectElement SelectLocalChannelDropdown
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_ddlSearchParams")));
            }
        }

        public bool CheckLocalChannelNumber()
        {
            return BillToAddSearchByField.Options.Select(e => e.Text.Contains("Local Channel #")).Any();
        }

        public void SelectLocalChannelOption()
        {
            SelecttLocalChannel.Click();
            SelectLocalChannelDropdown.SelectByText("Local Channel #");

        }

        public void LocalChannelNumberValue(string localchannelvalue)
        {
            BillToAddressOption.SendKeys(localchannelvalue);
            SelectButton.Click();
        }

        public bool FindLocalChannel(string localchannelvalue)
        {
            String columnPath = "//table[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress']/tbody/tr/td[6]";
            return webDriver.FindElements(By.XPath(columnPath)).All(e => e.Text.Contains(localchannelvalue));
        }
    }
}

