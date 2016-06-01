// ***********************************************************************
// Author           : AMERICAS\Naveen_Kumar31
// Created          : 12/30/2014 11:23:36 AM
//
// Last Modified By : AMERICAS\Naveen_Kumar31
// Last Modified On : 12/30/2014 11:23:36 AM
// ***********************************************************************
// <copyright file="B2BCrossReferenceAssociationPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;
using OpenQA.Selenium.Support.UI;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCrossReferenceAssociationPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceAssociationPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            Url = webDriver.Url;
            ProductUnit = "Channel";
            //webDriver.WaitForPageLoad(new TimeSpan(0, 0, PageUtility.PageTimeOut));
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            PageHeaderLabel.WaitForElementDisplayed(new TimeSpan(0, 0, PageUtility.ControlTimeOut));
            return PageHeaderLabel.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return (webDriver).ToString().Contains("(null)") ? false : true;
        }

        #region elements
        /// <summary>
        /// Associate Link 
        /// </summary>
        private IWebElement AssociateLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnAssociate"));
            }
        }

        /// <summary>
        /// Available Cross Reference List Table
        /// </summary>
        private IWebElement CrossReferenceListTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssociation_grdVwCrossReferenceList"));
            }
        }

        /// <summary>
        /// Cross Reference List Table Rows
        /// </summary>
        private IEnumerable<IWebElement> CrossReferenceListTableRows
        {
            get
            {
                return
                        webDriver.FindElements(
                        By.XPath("//table[@id='ContentPageHolder_CRTGridAssociation_grdVwCrossReferenceList']/tbody/tr"))
                        ;
            }
        }
        /// <summary>
        /// Crt Table First Row Check Box
        /// </summary>
        private IWebElement CrtTableCheckBox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssociation_grdVwCrossReferenceList_chkCRTLookup_0"));
            }
        }

        /// <summary>
        /// Error Message for Association without Affinity Id
        /// </summary>
        private IWebElement ProfileWithOutAffinityIdErrorMessage
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'There is no Affinity ID associated to this profile. Please update the Affinity ID and retry the CRT ')]"), TimeSpan.FromSeconds(20));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'There is no Affinity ID associated to this profile. Please update the Affinity ID and retry the CRT ')]"));
            }
        }

        /// <summary>
        /// Success Message for Association with Affinity Id
        /// </summary>
        private IWebElement ProfileWithAffinityIdSuccessMessage
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'Cross Reference details associated successfully')]"), TimeSpan.FromSeconds(20));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Cross Reference details associated successfully')]"));
            }
        }

        /// <summary>
        /// Success Message for Association with Diff Affinity Id
        /// </summary>
        private IWebElement ProfileWithDiffAffinityIdErrorMessage
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'The Affinity ID associated to this profile does not match the current Affinity ID')]"), TimeSpan.FromSeconds(20));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'The Affinity ID associated to this profile does not match the current Affinity ID')]"));
            }
        }
        /// <summary>
        /// CRT DropDown
        /// </summary>
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

        /// <summary>
        /// Cross reference type dropdown
        /// </summary>
        private string _crossReferenceTypeDropDown;
        private string crossReferenceTypeDropDown
        {
            get
            {
                if (string.IsNullOrEmpty(_crossReferenceTypeDropDown))
                    _crossReferenceTypeDropDown = "Channel_Cross_Segment_Booking";
                return _crossReferenceTypeDropDown;
            }
        }

        /// <summary>
        /// Search CRT List link
        /// </summary>
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

        /// <summary>
        /// Page Header Label
        /// </summary>
        private IWebElement _pageHeaderLabel;
        private IWebElement PageHeaderLabel
        {
            get
            {
                if (_pageHeaderLabel == null)
                    _pageHeaderLabel = webDriver.FindElement(By.Id("ContentPageHolder_lblHeader"));
                return _pageHeaderLabel;
            }
        }

        public SelectElement AccountName
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRT_Profiles")));
            }
        }

        public SelectElement CrossReferenceType
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType")));
            }
        }

        public IWebElement Search
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
            }
        }

        public IWebElement CRTResultTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList"));
            }
        }
        #endregion

        #region CRTAssociation Helper Methods

        /// <summary>
        /// Click on Association Link
        /// </summary>
        public void ClickAssociationLink()
        {
            AssociateLink.Click();
        }
        /// <summary>
        /// Is Profile with Different Affinity Id Error Message Displayed
        /// </summary>
        /// <returns></returns>
        public bool ProfileWithDiffAffinityIdErrorMessageDisplayed()
        {
            return ProfileWithDiffAffinityIdErrorMessage.IsElementVisible();
        }

        /// <summary>
        /// Profile With Affinity Id Success Message Displayed
        /// </summary>
        /// <returns></returns>
        public bool ProfileWithAffinityIdSuccessMessageDisplayed()
        {
            return ProfileWithAffinityIdSuccessMessage.IsElementVisible();
        }

        /// <summary>
        /// Profile WithOut Affinity Id Error Message Displayed
        /// </summary>
        /// <returns></returns>
        public bool ProfileWithOutAffinityIdErrorMessageDisplayed()
        {
            return ProfileWithOutAffinityIdErrorMessage.IsElementVisible();
        }

        /// <summary>
        ///Search based on given crt Type, by default  Channel_Cross_Segment_Booking will get filtered 
        /// </summary>
        /// <param name="crtType"></param>
        public void FilterCRT(string crtType)
        {
            this._crossReferenceTypeDropDown = crtType;
            selectCRTDropDown.Select().SelectByText(crossReferenceTypeDropDown);
            searchCRTList.Click();
        }

        /// <summary>
        /// Select the checkbox for given CR ID in Cross reference list
        /// </summary>
        /// <param name="crId"></param>
        public void SelectCrIdfromCRList(string crId)
        {
            var rowWithCrId = CrossReferenceListTableRows.FirstOrDefault(e => e.FindElements(By.TagName("td"))[1].Text.Trim().Equals(crId));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", rowWithCrId.FindElement(By.TagName("//td[0]")));
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void OpenCRTXML(string userId)
        {
            CRTResultTable.FindElement(By.XPath(string.Format("//td[text()='{0}']/..//a[text()='View Xml']", userId))).Click();
        }

        internal void OpenCrossReferenceListPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("CrossReferenceAssociationListPage") + (b2BEnvironment == B2BEnvironment.Production ? "P" : "U"));
        }
        #endregion
    }
}
