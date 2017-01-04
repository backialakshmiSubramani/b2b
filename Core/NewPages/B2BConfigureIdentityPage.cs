// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 12/26/2016 3:03:14 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 12/26/2016 3:03:14 PM
// ***********************************************************************
// <copyright file="B2BConfigureIdentityPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using OpenQA.Selenium;
using System;


namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BConfigureIdentityPage : PageBase

    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BConfigureIdentityPage(IWebDriver webDriver) : base(ref webDriver)
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

        private IWebElement DisableIdentityCheckbox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_chkEnabled"));
            }
        }

        private IWebElement SaveChangesLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnkSaveChanges"));
            }
        }

        private IWebElement _confirmationLabel;
        private IWebElement ConfirmationLabel
        {
            get
            {
                if (_confirmationLabel == null)
                    _confirmationLabel = webDriver.FindElement(By.Id("ContentPageHolder_lbl_ID_OKmsg"), new TimeSpan(0, 0, 60));
                return _confirmationLabel;
            }
        }

        #endregion Elements

        #region ElementActions

        public void DisableIdentity()
        {
            if (DisableIdentityCheckbox.Selected)
            {
                DisableIdentityCheckbox.Click();
            }
        }

        public bool SaveChanges()
        {
            SaveChangesLink.Click();
            return string.Equals(ConfirmationLabel.Text.Trim(), "Profile Identity details saved successfully.");
        }

        #endregion ElementActions
    }
}
