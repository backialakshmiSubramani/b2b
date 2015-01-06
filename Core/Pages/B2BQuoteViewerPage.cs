// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 1/6/2015 5:10:46 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 1/6/2015 5:10:46 PM
// ***********************************************************************
// <copyright file="B2BQuoteViewerPage.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
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
    using System.Security.Cryptography;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BQuoteViewerPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BQuoteViewerPage(IWebDriver webDriver)
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

        #region Elements

        private IWebElement FirstItemRow
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='ContentPageHolder_dg_QV_Items']/tbody/tr[2]"));
            }
        }

        #endregion

        #region Reusable Methods

        public bool CheckItemDetails(string description, string quantity, string unitPrice)
        {
            return FirstItemRow.FindElements(By.TagName("td"))[2].Text.Trim().ToLower().Equals(description.ToLower())
                   && FirstItemRow.FindElements(By.TagName("td"))[6].Text.Trim().ToLower().Equals(quantity.ToLower())
                   && FirstItemRow.FindElements(By.TagName("td"))[7].Text.Trim().ToLower().Equals(unitPrice.ToLower());
        }

        #endregion

    }
}
