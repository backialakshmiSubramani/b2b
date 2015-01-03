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
        IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstAddressWizardPage(IWebDriver webDriver)
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

        private IWebElement ShipToAddressOption
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("__tab_TabContainerAddress_TabPanelShiptoAddress"), TimeSpan.FromSeconds(30));
                //Xpath = //span[@id='__tab_TabContainerAddress_TabPanelShiptoAddress']
                return webDriver.FindElement(By.Id("__tab_TabContainerAddress_TabPanelShiptoAddress"));
            }
        }

        private IWebElement SelectButton
        {
            get
            {
                return webDriver.FindElement(By.Name("TabContainerAddress$TabPanelBilltoAddress$AffinityBillAddress$imgbtnSearch"));
            }
        }

        private SelectElement SelectLocalChannelDropdown
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_ddlSearchParams")));
            }
        }

        private IWebElement AddressResultsTable
        {
            get
            {
                return webDriver.FindElement(By.Id("TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress"));
            }
        }

        private IWebElement CustomerNumberColumnPosition
        {
            get
            {

                return webDriver.FindElement(By.XPath("//table[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress']/thead/tr/th[5]"));
            }
        }

        private IWebElement ChannelNumberColumnPosition
        {
            get
            {

                return webDriver.FindElement(By.XPath("//table[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress']/thead/tr/th[6]"));
            }
        }

        private IWebElement Hyperlink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Refresh Addresses"));
            }
        }

        public bool CheckLocalChannelNumber()
        {
            return BillToAddSearchByField.Options.Any(e => e.Text.Contains("Local Channel #"));
        }

        public void SelectLocalChannelOption()
        {
            SelectLocalChannelDropdown.SelectByText("Local Channel #");
        }

        public void SearchByLocalChannelNumber(string localChannelValue)
        {
            BillToAddressOption.SendKeys(localChannelValue);
            ////SelectButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SelectButton);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public bool FindLocalChannel(string localChannelNumber)
        {
            String columnPath = "//table[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress']/tbody/tr/td[6]";
            return webDriver.FindElements(By.XPath(columnPath)).All(e => e.Text.Contains(localChannelNumber));
        }

        public string ChannelNumberColumnText()
        {
            return ChannelNumberColumnPosition.Text;
        }

        public string CustomerNumberColumnText()
        {
            return CustomerNumberColumnPosition.Text;
        }

        public void ShipToAddress()
        {
            ////ShipToAddressOption.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ShipToAddressOption);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void BillToAddHyperLinkClick()
        {
            ////Hyperlink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", Hyperlink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public string BillToAddUpdateTextCValidate()
        {
            webDriver.WaitForElementDisplayed(By.XPath("//span[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_lbl_Error']"), TimeSpan.FromSeconds(30));
            return webDriver.FindElement(By.XPath("//span[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_lbl_Error']")).Text;
        }

        public string OmsAdd(string localChannelNumber)
        {
            string AddressPath = "//span[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_gvAffinityAddress_lblAddressAffinity1_0']";
            return webDriver.FindElement(By.XPath(AddressPath)).Text;
        }

        public void AddNewOmsAddressClick()
        {
            webDriver.FindElement(By.XPath("//input[@id='TabContainerAddress_TabPanelBilltoAddress_AffinityBillAddress_img_Add_New_BLG_OMS_Address']")).Click();
        }

        public string AddOmsAddressNewWindow()
        {
            webDriver.WaitForElementDisplayed(By.XPath("//h2[contains(text(),'Add')]"), TimeSpan.FromSeconds(30));

            return webDriver.FindElement(By.XPath("//h2[contains(text(),'Add')]")).Text;
        }

        /// <summary>
        /// method to check Local Channel # coloumn in Grid by Xpath.
        /// no other unique identifier is present.
        /// </summary>
        /// <returns>true,if Local channel # is displayed </returns>
        public bool ChannelNumberColumnTextExist()
        {
            bool exist = true;

            if (webDriver.ElementExists(
                By.XPath(
                    "//table[@class='affinityAddress']/tbody/tr[1]/th[6]")))
            {
                if (!(webDriver.FindElement(By.XPath(
                    "//table[@class='affinityAddress']/tbody/tr[1]/th[6]")).Text.Length > 1))
                    exist = false;
            }
            else
            {
                exist = false;
            }

            return exist;
        }

        public bool CheckIfResultsTableIsAvailable()
        {
            try
            {
                return AddressResultsTable.IsElementVisible();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Address Wizard Results Table not found");
                return false;
            }
        }
    }
}

