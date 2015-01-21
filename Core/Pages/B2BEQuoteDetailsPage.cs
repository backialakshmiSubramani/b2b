// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 6:07:18 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 6:07:18 PM
// ***********************************************************************
// <copyright file="EQuoteDetailsPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BEQuoteDetailsPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BEQuoteDetailsPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
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

        #region Element
        private IWebElement SaveQuoteTitle
        {
            get
            { return webDriver.FindElement(By.XPath("//div[@class='uif_mhFooterWrap clearfix']/h1")); }
        }

        private IWebElement EquoteName
        {
            get
            {
                return webDriver.FindElement(By.Id("EquoteDetails_Name"), new TimeSpan(0, 0, 10));
            }
        }

        private IWebElement SavedBy
        {
            get { return webDriver.FindElement(By.Id("EquoteDetails_Creator")); }

        }

        private IWebElement ContinueButton
        {
            get { return webDriver.FindElement(By.Id("EQuoteDetailsContinue")); }

        }


        #endregion

        #region Element Actions

        public string ReturnTitle()
        {
            return SaveQuoteTitle.Text;
        }

        public void EquoteNameSetting(string name)
        {
            EquoteName.SendKeys(name);
        }

        public void SavedBySetting(string email1)
        {
            SavedBy.SendKeys(email1);
        }

        public void ClickContinueButton()
        {
            ContinueButton.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
        }

        #endregion
    }
}
