// ***********************************************************************
// Author           : AMERICAS\Naveen_Kumar31
// Created          : 12/29/2014 6:01:38 PM
//
// Last Modified By : AMERICAS\Naveen_Kumar31
// Last Modified On : 12/29/2014 6:01:38 PM
// ***********************************************************************
// <copyright file="B2BManageProfileIdentitiesPage.cs" company="Dell">
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
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BManageProfileIdentitiesPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BManageProfileIdentitiesPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            Name = PageTitleHeader.Text;
            //Url = "";
            //ProductUnit = "";
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, PageUtility.PageTimeOut));
            PageUtility.WaitForPageRefresh(webDriver);
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            PageTitleHeader.WaitForElementDisplayed(new TimeSpan(0, 0, PageUtility.ControlTimeOut));
            return PageTitleHeader.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return (webDriver).ToString().Contains("(null)") ? false : true;
        }

        #region Elements

        /// <summary>
        /// CR Association Link
        /// </summary>
        public IWebElement CRAssociationLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("/html/body/form/table[2]/tbody/tr[1]/td[2]/table/tbody/tr[2]/td/div[2]/div[2]/table[6]/tbody/tr/th/label[2]/a"), new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("/html/body/form/table[2]/tbody/tr[1]/td[2]/table/tbody/tr[2]/td/div[2]/div[2]/table[6]/tbody/tr/th/label[2]/a"));
            }
        }

        /// <summary>
        /// Associate Cross Reference Link under CR Association
        /// </summary>
        public IWebElement AssociateCrossReferenceLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnkAssociateCrossReference"));
            }
        }

        /// <summary>
        /// Page Title Header
        /// </summary>
        private IWebElement PageTitleHeader
        {
            get
            {
                return webDriver.FindElement(By.Id("ucBreadCrumb_lblPageTitle"));
            }
        }

        public IWebElement BuyerCatalogTab
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_ProfileHeader_hyp_PH_BuyerCatalog"));
            }
        }

        public IWebElement RegionName_Globalization
        {
            get
            {
                return webDriver.FindElement(By.XPath("//select[@id='ContentPageHolder_lstProfileContextData']/option[contains(text(),'Region')]"));
            }
        }

        #endregion
    }
}
