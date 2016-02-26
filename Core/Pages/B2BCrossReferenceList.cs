// ***********************************************************************
// Author           : AMERICAS\Omendra_Singh_Rana
// Created          : 12/18/2014 8:13:39 PM
//
// Last Modified By : AMERICAS\Omendra_Singh_Rana
// Last Modified On : 12/18/2014 8:13:39 PM
// ***********************************************************************
// <copyright file="B2BCrossReferenceList.cs" company="Dell">
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
    public class B2BCrossReferenceList : PageBase
    {
        private readonly IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceList(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
        }

        # region Elements

        private IWebElement crossReferencePageLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'Cross Reference List')]"), TimeSpan.FromSeconds(PageUtility.ControlTimeOut));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Cross Reference List')]"));
            }
        }

        private IWebElement _selectCRTDropDown;
        private IWebElement selectCRTDropDown
        {
            get
            {
                if (_selectCRTDropDown == null)
                    _selectCRTDropDown = webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType"));
                return _selectCRTDropDown;
            }
        }

        private string _crossReferenceDropDown;
        private string crossReferenceDropDown
        {
            get
            {
                if (_crossReferenceDropDown == null)
                    _crossReferenceDropDown = "Channel_Cross_Segment_Booking";
                return _crossReferenceDropDown;
            }
        }

        private IWebElement _searchCRTList;
        private IWebElement searchCRTList
        {
            get
            {
                if (_searchCRTList == null)
                    _searchCRTList = webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
                return _searchCRTList;
            }
        }

        private IWebElement PageTitleHeader
        {
            get
            {
                return webDriver.FindElement(By.Id("ucBreadCrumb_lblPageTitle"));
            }
        }

        #endregion


        /// <summary>
        /// Page Validate
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return PageTitleHeader.Displayed;
        }

        /// <summary>
        /// WebDriver IsActive 
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.Contains("/UI/CrossReferenceMaintence.aspx");
        }


        #region Element Action

        /// <summary>
        /// Initializes a new page <see cref="B2BCrossReferenceMaintenance"/> class.
        /// </summary>
        public B2BCrossReferenceMaintenance OpenCrossReferenceList()
        {
            crossReferencePageLink.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(PageUtility.PageTimeOut));
            selectCRTDropDown.Select().SelectByText(crossReferenceDropDown);
            searchCRTList.Click();

            return new B2BCrossReferenceMaintenance(webDriver);
        }

        #endregion

    }
}
