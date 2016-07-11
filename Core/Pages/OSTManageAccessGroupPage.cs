// ***********************************************************************
// Author           : AMERICAS\Venkatesh_Vantri
// Created          : 6/24/2016 11:31:10 AM
//
// Last Modified By : AMERICAS\Venkatesh_Vantri
// Last Modified On : 6/24/2016 11:31:10 AM
// ***********************************************************************
// <copyright file="OSTManageAccessGroupPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using Dell.Adept.Core;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class OSTManageAccessGroupPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OSTManageAccessGroupPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
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

        public IWebElement SettingsButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_imgbtnViewAGSettings"));
            }
        }

        public IWebElement StdConfigAG
        {
            get
            {
                return webDriver.FindElement(By.Id("bf5829e3-5501-4859-8f5a-a55c001a5efd"));
            }
        }

        public IWebElement AccessGroup_UpdateButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_uc_PublishAndUpdate_img_Update"));
            }
        }

        public IWebElement StdConfig_ExpandButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_PlusStandard Configurations"));
            }
        }

        public IWebElement ContentTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_MyTable"));
            }
        }

        #endregion

        #region Methods

        public void UpdateProduct(string accessGroupName, string productName, DeltaChange deltaChange)
        {
            webDriver.FindElement(By.XPath("//div[@title='" + accessGroupName + "']/../..//input[@name='chkAccessGroup']")).SendKeys(Keys.Space);
            SettingsButton.SendKeys(Keys.Enter);
            webDriver.WaitForElementVisible(By.Id("ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_uc_PublishAndUpdate_img_Update"), TimeSpan.FromMinutes(2));
            webDriver.FindElement(By.XPath("//table[@id='ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_MyTable']//span[text()='Standard Configurations'][contains(@onclick,'return')]")).Click();

            IWebElement chkElement = ContentTable.FindElement(By.XPath("//tr[@title='" + productName + "']")).FindElement(By.XPath(".//input[contains(@id,'ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_grpchk')]"));
            switch (deltaChange)
            {
                case DeltaChange.Add:
                    if (!chkElement.Selected)
                        chkElement.SendKeys(Keys.Space);
                    break;
                case DeltaChange.Remove:
                    if (chkElement.Selected)
                        chkElement.SendKeys(Keys.Space);
                    break;
            }

            AccessGroup_UpdateButton.SendKeys(Keys.Enter);
            webDriver.WaitForPageLoad(TimeSpan.FromMinutes(2));
        }

        public void ResetProduct(string accessGroupName, string productName, DeltaChange deltaChange)
        {
            webDriver.FindElement(By.XPath("//div[@title='" + accessGroupName + "']/../..//input[@name='chkAccessGroup']")).SendKeys(Keys.Space);
            SettingsButton.SendKeys(Keys.Enter);
            webDriver.WaitForElementVisible(By.Id("ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_uc_PublishAndUpdate_img_Update"), TimeSpan.FromMinutes(2));
            webDriver.FindElement(By.XPath("//table[@id='ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_MyTable']//span[text()='Standard Configurations'][contains(@onclick,'return')]")).Click();

            IWebElement chkElement = ContentTable.FindElement(By.XPath("//tr[@title='" + productName + "']")).FindElement(By.XPath(".//input[contains(@id,'ctl00_ContentPageHolder_ManageGroupsContainer_AccessGroupTab_ManageAccessGrps_grpchk')]"));
            switch (deltaChange)
            {
                case DeltaChange.Add:
                    if (chkElement.Selected)
                        chkElement.SendKeys(Keys.Space);
                    break;
                case DeltaChange.Remove:
                case DeltaChange.Modify:
                    if (!chkElement.Selected)
                        chkElement.SendKeys(Keys.Space);
                    break;
            }

            AccessGroup_UpdateButton.SendKeys(Keys.Enter);
            webDriver.WaitForPageLoad(TimeSpan.FromMinutes(2));
        }

        #endregion
    }
}
