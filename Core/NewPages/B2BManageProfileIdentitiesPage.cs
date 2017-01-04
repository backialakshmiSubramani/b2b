// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 12/26/2016 11:42:58 AM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 12/26/2016 11:42:58 AM
// ***********************************************************************
// <copyright file="B2BManageProfileIdentitiesPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Channel.B2B.Core.NewPages
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
        public B2BManageProfileIdentitiesPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
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

        private IWebElement _manageProfileIdentitiesLink;
        private IWebElement ManageProfileIdentitiesLink
        {
            get
            {
                if (_manageProfileIdentitiesLink == null)
                {
                    _manageProfileIdentitiesLink = webDriver.FindElement(By.XPath("//*[@id='manageprofileplus']/a/img"), new TimeSpan(0, 0, 30));
                }
                return _manageProfileIdentitiesLink;
            }
        }

        private IWebElement _manageProfileIdentitiesRow;
        private IWebElement ManageProfileIdentitiesRow
        {
            get
            {
                if (_manageProfileIdentitiesRow == null)
                {
                    _manageProfileIdentitiesRow = webDriver.FindElement(
                        By.XPath("//table[@id='ContentPageHolder_gvIdentities']/tbody/tr/td/a"),
                        new TimeSpan(0, 0, 30));
                }
                return _manageProfileIdentitiesRow;
            }
        }

        private List<IWebElement> _manageProfileIdentitiesRows;
        private List<IWebElement> ManageProfileIdentitiesRows
        {
            get
            {
                if (_manageProfileIdentitiesRows == null)
                {
                    _manageProfileIdentitiesRows = webDriver.FindElements(
                        By.XPath("//table[@id='ContentPageHolder_gvIdentities']/tbody/tr"), new TimeSpan(0, 0, 30)).ToList();
                }
                return _manageProfileIdentitiesRows;
            }
        }

        private IWebElement BuyerCatalogTab
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_ProfileHeader_hyp_PH_BuyerCatalog"));
            }
        }

        #endregion Elements

        #region ElementActions

        public void ExpandManageProfileIdentitiesSection()
        {
            ManageProfileIdentitiesLink.Click();
        }

        public void ClickIdentityLink()
        {
            ManageProfileIdentitiesRow.Click();
        }

        public List<string> GetIdentities()
        {
            return ManageProfileIdentitiesRows.Skip(1).Select(item => item.FindElements(By.TagName("td"))[0].Text).ToList();
        }

        public void GoToBuyerCatalogTab()
        {
            BuyerCatalogTab.Click();
        }

        #endregion ElementActions
    }
}
