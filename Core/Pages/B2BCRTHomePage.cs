// ***********************************************************************
// Author           : AMERICAS\Omendra_Singh_Rana
// Created          : 12/18/2014 11:28:48 PM
//
// Last Modified By : AMERICAS\Omendra_Singh_Rana
// Last Modified On : 12/18/2014 11:28:48 PM
// ***********************************************************************
// <copyright file="B2BCRTHomePage.cs" company="Dell">
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
    public class B2BCRTHomePage : DCSGPageBase
    {
        private readonly IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCRTHomePage(IWebDriver webDriver)
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

        # region Elements

        private string _selectEnvironment;
        private string selectEnvironment
        {
            get
            {
                if (_selectEnvironment == null)
                    _selectEnvironment = "Preview";
                return _selectEnvironment;
            }
        }

        private IWebElement _envDropDown;
        private IWebElement envDropDown
        {
            get
            {
                if (_envDropDown == null)
                    _envDropDown = webDriver.FindElement(By.Id("ucLeftMenu_ddlEnv"));
                return _envDropDown;
            }
        }

        private IWebElement _goToLink;
        private IWebElement goToLink
        {
            get
            {
                if (_goToLink == null)
                    _goToLink = webDriver.FindElement(By.Id("ucLeftMenu_lnkGo"));
                return _goToLink;
            }
        }

        #endregion


        #region Element Action

        /// <summary>
        /// Initializes a new page <see cref="B2BCrossReferenceMaintenance"/> class.
        /// </summary>
        public B2BCrossReferenceMaintenance SelectEnvironment()
        {
            envDropDown.Select().SelectByText(selectEnvironment);
            goToLink.Click();

            return new B2BCrossReferenceMaintenance(webDriver);
        }

        #endregion

    }
}
