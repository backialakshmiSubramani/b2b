// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/22/2014 4:22:02 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/22/2014 4:22:02 PM
// ***********************************************************************
// <copyright file="B2BPoViewer.cs" company="Dell">
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
    using System.Linq;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BPoViewerPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BPoViewerPage(IWebDriver webDriver)
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

        private List<IWebElement> EndUserDetailsTable
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath(
                            "//table[@id='ContentPageHolder_Tabs_tabPanel_B2B_PO_Viewer_tblEndUserDetails']/tbody/tr/td[2]"))
                        .ToList();
            }

        }

        public List<string> GetEndUserDetails()
        {
            return EndUserDetailsTable.Select(e => e.Text).ToList();
        }

    }
}
