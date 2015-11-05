// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 3/25/2015 3:29:24 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 3/25/2015 3:29:24 PM
// ***********************************************************************
// <copyright file="B2BCatalogPackagingDataUploadPage.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenQA.Selenium;
using Dell.Adept.Core;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Modules.Channel.B2B.Common;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCatalogPackagingDataUploadPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCatalogPackagingDataUploadPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            Name = "Channel Catalog Packaging Data Upload Page";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            this.webDriver.WaitForPageLoad(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return UploadButton.IsElementVisible();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("channelcatalogux");
        }

        #region Elements

        /// <summary>
        /// Field for selecting the file to upload
        /// </summary>
        public IWebElement FileUpload
        {
            get { return webDriver.FindElement(By.Id("file")); }
        }

        /// <summary>
        /// Upload button
        /// </summary>
        public IWebElement UploadButton
        {
            get { return webDriver.FindElement(By.Id("btnUpload")); }
        }

        /// <summary>
        /// Message displayed on Upload
        /// </summary>
        public IWebElement UploadMessage
        {
            get
            {
                webDriver.WaitForElementVisible(By.Id("divMessage"), new TimeSpan(0, 0, 30));
                return webDriver.FindElement(By.Id("divMessage"));
            }
        }
        
        /// <summary>
        /// Rows of the Audit History table
        /// </summary>
        public IReadOnlyCollection<IWebElement> AuditHistoryRows
        {
            get
            {
                return webDriver.FindElements(AdeptBy.Attribute(ElementTag.tbody, "ng-repeat", "history in auditHistories"));
            }
        }

        public IReadOnlyCollection<IWebElement> AuditHistoryRecords
        {
            get
            {
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
                return webDriver.FindElements(AdeptBy.Attribute(ElementTag.tr, "ng-repeat", "history in auditHistories"));
            }
        }

        public IWebElement AuditHistoryTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("/html/body/div/section/div/div[2]/div/div/table/tbody/tr[2]/td/div/table"));
            }
        }

        #endregion

        internal void UploadExcelFile(string fileToUpload)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToUpload);
            UploadButton.Click();
            webDriver.WaitGetAlert();
            webDriver.SwitchTo().Alert().Accept();
           // webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
        }

        internal IReadOnlyCollection<IWebElement> GetAuditHistoryRowValues(IWebElement auditHistoryRow)
        {
            return auditHistoryRow.FindElements(By.TagName("td"));
        }
    }
}
