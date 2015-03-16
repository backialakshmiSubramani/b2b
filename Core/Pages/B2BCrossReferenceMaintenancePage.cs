// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/11/2014 6:59:23 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/11/2014 6:59:23 PM
// ***********************************************************************
// <copyright file="B2BCrossReferenceMaintenancePage.cs" company="Dell">
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

using System.Linq;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCrossReferenceMaintenancePage : PageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceMaintenancePage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            Name = "B2B Preview Cross Reference Maintenance";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

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
            return webDriver.Url.Contains("/B2BToolsCE");
        }

        #region Elements
        private IWebElement _pageTitleHeader;
        private IWebElement PageTitleHeader
        {
            get
            {
                if (_pageTitleHeader == null)
                    _pageTitleHeader = webDriver.FindElement(By.Id("ucBreadCrumb_lblPageTitle"), new TimeSpan(0, 0, 10));
                return _pageTitleHeader;
            }
        }
        private SelectElement CrossReferenceTypeList
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_drp_CRTType"), TimeSpan.FromSeconds(60));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType")));
            }
        }

        private IWebElement DescriptionTxtBox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txtLookupDesc"));
            }
        }

        private IWebElement CrossReferneceTypeLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lblCRType"));
            }
        }
        private IWebElement DescriptionLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label3"));
            }
        }
        private IWebElement FileToUploadLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label4"));
            }
        }

        private IWebElement FileToUpload
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_fileUpEx"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.Id("ContentPageHolder_fileUpEx"));
            }
        }

        private IWebElement SaveButton
        {
            get 
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSave"));
            }
        }

        private IWebElement CrtUploadSuccessMsg
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_lblMsgSave"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.Id("ContentPageHolder_lblMsgSave"));
            }
        }

        private IWebElement CrossReferenceListLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'Cross Reference List')]"), new TimeSpan(0, 0, 10));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Cross Reference List')]"));
            }
        }

        private IWebElement CrtUploadErrorMsg
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_lbl_CRT_Error1"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.Id("ContentPageHolder_lbl_CRT_Error1"));
            }
        }

        private IWebElement _myCustomerSyncCheckbox;
        private IWebElement MyCustomerSyncCheckbox
        {
            get
            {
                if (_myCustomerSyncCheckbox == null)
                    _myCustomerSyncCheckbox = webDriver.FindElement(By.Id("ContentPageHolder_chkMyCustomerSync"));
                return _myCustomerSyncCheckbox;
            }
        }

        /// <summary>
        /// Sync CRT Link
        /// </summary>
        private IWebElement _syncCRTLink;
        private IWebElement SyncCRTLink
        {
            get
            {
                if (_syncCRTLink == null)
                    _syncCRTLink = webDriver.FindElement(By.Id("ContentPageHolder_divRefreshCRT"));
                return _syncCRTLink;
            }
        }

        /// <summary>
        /// Append and Save link
        /// </summary>
        private IWebElement _appendandSave;
        private IWebElement AppendandSaveLink
        {
            get
            {
                if (_appendandSave == null)
                    _appendandSave = webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnAppend"));
                return _appendandSave;
            }
        }

        /// <summary>
        /// Overwrite and Save link
        /// </summary>
        private IWebElement _overWriteandSave;
        private IWebElement OverWriteandSaveLink
        {
            get
            {
                if (_overWriteandSave == null)
                    _overWriteandSave = webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnOverwrite"));
                return _overWriteandSave;
            }
        }

        /// <summary>
        /// Button Sync CRT 
        /// </summary>
        private IWebElement _btnSyncCRTLink;
        private IWebElement BtnSyncCRTLink
        {
            get
            {
                if (_btnSyncCRTLink == null)
                    _btnSyncCRTLink = webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnRefreshCRT"));
                return _btnSyncCRTLink;
            }
        }
        #endregion

        #region Element Actions

        public void SelectCrossReferenceType(string crType)
        {
            CrossReferenceTypeList.SelectByText(crType);
        }

        public void SetDescription(string description)
        {
            DescriptionTxtBox.SendKeys(description);
        }

        public void SetUploadFilePath(string filePath)
        {
            Console.WriteLine("File path {0}", System.IO.Directory.GetCurrentDirectory());
            FileToUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + filePath);
        }

        public void ClickSave()
        {
            ////SaveButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SaveButton);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public void ClickSyncCRT()
        {
            javaScriptExecutor.ExecuteScript("arguments[0].click();", BtnSyncCRTLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
        public bool IsSuccessfulMessageDisplayed()
        {
            return this.CrtUploadSuccessMsg.IsElementVisible();
        }

        public bool IsErrorMessageDisplayed()
        {
            return this.CrtUploadErrorMsg.IsElementVisible();
        }

        /// <summary>
        /// Is Sync CRT Link Displayed
        /// </summary>
        /// <returns></returns>
        public bool IsSyncCRTDisplayed()
        {
            return this.SyncCRTLink.Displayed;
        }

        /// <summary>
        /// Sync CRT Link Enabled
        /// </summary>
        /// <returns></returns>
        public bool IsSyncCRTEnabled()
        {
            return this.SyncCRTLink.Enabled;
        }
        /// <summary>
        /// Is OverWriteandSaveLink Enabled
        /// </summary>
        /// <returns></returns>
        public bool IsOverWriteandSaveLinkEnabled()
         {
             return this.OverWriteandSaveLink.Enabled;
         }

        /// <summary>
        /// Append and Save Link Enabled
        /// </summary>
        /// <returns></returns>
        public bool IsAppendandSaveLinkEnabled()
         {
             return this.AppendandSaveLink.Enabled;
         }
        
        public string CrossReferenceTypeText()
        {
            return CrossReferneceTypeLabel.Text;
        }

        public string FileUploadText()
        {
            return FileToUploadLabel.Text;
        }

        public string DescriptionText()
        {
            return DescriptionLabel.Text;
        }

        public string GetCrId()
        {
            return CrtUploadSuccessMsg.Text.Trim().Split(' ').Last();
        }

        public void GoToCrossReferenceListPage()
        {
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CrossReferenceListLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Check Is Save button visible
        /// </summary>
        /// <returns></returns>
        public bool SaveButtonDisplayed()
        {
            return this.SaveButton.IsElementVisible();
        }
        /// <summary>
        /// My Customer Sync Checkbox display
        /// </summary>
        /// <returns></returns>
        public bool MyCustomerSyncCheckboxDisplay()
        {
            return webDriver.ElementExists(AdeptBy.Id("ContentPageHolder_chkMyCustomerSync"));
        }

        /// <summary>
        /// Enable or Disable My Customer Sync Checkbox
        /// </summary>
        /// <param name="Option">Enable or Disable</param>
        public void EnableorDisableMyCustomerSyncCheckBox(string Option)
        {

            switch (Option)
            {
                case "Enable":
                    if (MyCustomerSyncCheckbox.GetAttribute("checked") != "true")
                    {
                        //chkMyCustomerSyncId.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", MyCustomerSyncCheckbox);
                    }

                    break;

                case "Disable":
                    if (MyCustomerSyncCheckbox.GetAttribute("checked") == "true")
                    {
                        //chkMyCustomerSyncId.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", MyCustomerSyncCheckbox);
                    }

                    break;
            }
        }
        #endregion
    }
}
