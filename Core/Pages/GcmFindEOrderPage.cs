// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/22/2014 6:25:22 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/22/2014 6:25:22 PM
// ***********************************************************************
// <copyright file="GcmFindEOrderPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

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
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class GcmFindEOrderPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public GcmFindEOrderPage(IWebDriver webDriver)
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


        #region Elements

        private SelectElement SearchCriteriaElement
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ddlSearchCriteria")));
            }
        }

        private IWebElement SearchTextBox
        {
            get
            {
                return webDriver.FindElement(By.Id("txtSearch"));
            }
        }

        private IWebElement OrderStatusElement
        {
            get
            {
                //return webDriver.FindElement(By.XPath("//table[@id='tblContent']/tbody/tr[2]/td[3]/table[3]/tbody/tr[5]/td[2]/table/tbody/tr[3]/td[2]"));
                return webDriver.FindElement(By.XPath("//*[@id='tblContent']/tbody/tr[2]/td[3]/table[3]/tbody/tr[5]/td[2]/table/tbody/tr[2]/td[2]"));
            }
        }

        private IWebElement SearchButton
        {
            get
            {
                return webDriver.FindElement(By.Id("btnSearch"));
            }
        }

        #endregion

        #region Element Actions

        public string SearchByDpidAndGetOrderStatus(string dpid)
        {
            if (!SearchCriteriaElement.SelectedOption.Text.Equals("Dell Purchase Id"))
            {
                SearchCriteriaElement.SelectByText("Dell Purchase Id");
            }

            webDriver.WaitForElementDisplayed(By.Id("txtSearch"), TimeSpan.FromSeconds(10));
            SearchTextBox.Clear();
            SearchTextBox.SendKeys(dpid);
            ////SearchButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchButton);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
            try
            {
                return OrderStatusElement.Text;
            }
            catch (NoSuchElementException)
            {
                return string.Format("DP Id {0} not reflecting in GCM", dpid);
            }
        }

        public void ClickViewButton(string dellPurchaseId)
        {
            javaScriptExecutor.ExecuteScript(
                "arguments[0].click();",
                webDriver.FindElement(By.Id("_Vw" + dellPurchaseId + "CMP")));
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 20));
        }

        #endregion
    }
}
