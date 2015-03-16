// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/24/2014 9:56:15 AM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/24/2014 9:56:15 AM
// ***********************************************************************
// <copyright file="GcmEndUserInfoPage.cs" company="Dell">
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
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class GcmEndUserInfoPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public GcmEndUserInfoPage(IWebDriver webDriver)
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

        private ReadOnlyCollection<IWebElement> EndUserDetailsTable
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath("//div[@id='ctl59_xpnd']/table/tbody/tr[5]/td[2]/table/tbody/tr[2]/td/table/tbody/tr/td[2]"));
            }
        }

        public List<string> GetEndUserDetails()
        {
            return EndUserDetailsTable.Select(e => e.Text).Skip(1).Take(7).ToList();
        }
    }
}
