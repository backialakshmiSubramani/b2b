// ***********************************************************************
// Author           : AMERICAS\Omendra_Singh_Rana
// Created          : 12/5/2014 4:41:12 PM
//
// Last Modified By : AMERICAS\Omendra_Singh_Rana
// Last Modified On : 12/5/2014 4:41:12 PM
// ***********************************************************************
// <copyright file="B2BCrossReferenceMaintenance.cs" company="Dell">
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
    public class B2BCrossReferenceMaintenance : PageBase
    {
        
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceMaintenance(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
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
        

        # region Elements

        private IWebElement _selectTestProfile;
        private IWebElement selectTestProfile
        {
            get
            {
                if (_selectTestProfile == null)
                    _selectTestProfile = webDriver.FindElement(By.XPath("//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr/td[4]"), new TimeSpan(0, 0, 20));
                return _selectTestProfile;
            }
        }

        private IWebElement _goToTestProfile;
        private IWebElement goToTestProfile
        {
            get
            {
                if (_goToTestProfile == null)
                    _goToTestProfile = webDriver.FindElement(By.LinkText(selectTestProfile.Text));
                return _goToTestProfile;
            }
        }

        public IList<IWebElement> GetTypeColumnValues()
        {
            return crossReferenceTypeColumn;
        }
        
        private IList<IWebElement> crossReferenceTypeColumn
        {
            get
            {
                return webDriver.FindElements(By.XPath("//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr/td[0]"));
            }
        }


        private IWebElement _chkMyCustomerSyncId;
        private IWebElement chkMyCustomerSyncId
        {
            get
            {
                if (_chkMyCustomerSyncId == null)
                    _chkMyCustomerSyncId = webDriver.FindElement(By.Id("ContentPageHolder_chkMyCustomerSync"));
                return _chkMyCustomerSyncId;
            }
        }


        private IWebElement _fileUploadId;
        private IWebElement fileUploadId
        {
            get
            {
                if (_fileUploadId == null)
                    _fileUploadId = webDriver.FindElement(By.Id("ContentPageHolder_chkMyCustomerSync"));
                return _fileUploadId;
            }
        }


        private IWebElement _hplnkViewXml;
        private IWebElement hplnkViewXml
        {
            get
            {
                if (_hplnkViewXml == null)
                    _hplnkViewXml = webDriver.FindElement(By.Id("ContentPageHolder_chkMyCustomerSync"));
                return _hplnkViewXml;
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


        #region Element Action

        /// <summary>
        /// Initializes a new page <see cref="B2BCrossReferenceMaintenance"/> class.
        /// </summary>
        public void Validate_MyCustomerCheckBox_TurnOnAndOff()
        {
            goToTestProfile.Click();
            chkMyCustomerSyncId.Click();
        }

        #endregion
        
    }
}
