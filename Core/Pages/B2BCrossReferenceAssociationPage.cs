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
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using Modules.Channel.EUDC.Core.Pages;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCrossReferenceAssociationPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceAssociationPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            Name = PageHeaderLabel.Text;
            //Url = "";
            //ProductUnit = "";
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, PageUtility.PageTimeOut));
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
        public IWebElement AssociateLink
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
        /// Crt Table First Row Check Box
        /// </summary>
        public IWebElement CrtTableCheckBox
        {
            get 
            { 
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssociation_grdVwCrossReferenceList_chkCRTLookup_0")); 
            }
        }

        /// <summary>
        /// Error Message for Association without Affinity Id
        /// </summary>
        public IWebElement ProfileWithOutAffinityIdErrorMessage
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
        public IWebElement ProfileWithAffinityIdSuccessMessage
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'Cross Reference details associated successfully')]"), TimeSpan.FromSeconds(20));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Cross Reference details associated successfully')]"));
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
                if (_pageHeaderLabel ==null)
                    _pageHeaderLabel = webDriver.FindElement(By.Id("ContentPageHolder_lblHeader"));
                return _pageHeaderLabel;
            }
        }
        #endregion

        #region CRTAssociation Helper Methods
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
        #endregion
    }
}