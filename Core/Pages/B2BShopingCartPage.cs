// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 5:11:59 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 5:11:59 PM
// ***********************************************************************
// <copyright file="ShopingCartPage.cs" company="Dell">
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
using Modules.Channel.B2B.Core.Workflows.Common;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BShopingCartPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BShopingCartPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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
        private IWebElement ShopingCartTitle
        {
            get
            { return webDriver.FindElement(By.XPath("//div[@id='divTitleInMasthead']/div/h1")); }
        }

        private IWebElement SaveEQuoteElement
        {
            get
            { return webDriver.FindElement(By.XPath("//a[@id='CartSaveEQuote']/span")); }
        }

        private IWebElement CreateOrQuoteElement
        {
            get
            { return webDriver.FindElement(By.Id("CartCheckout")); }
        }

        #endregion

        #region Element Actions



        public string ReturnShopingCartTitle()
        {
            return ShopingCartTitle.Text;
        }


        public void ClickSaveQuote(QuoteType quoteType)
        {
            if (quoteType == QuoteType.EQuote)
            {
                ////SaveEQuoteElement.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", SaveEQuoteElement);
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(50));
            }

            if (quoteType == QuoteType.OrQuote)
            {
                ////CreateOrQuoteElement.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", CreateOrQuoteElement);
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(50));
            }
        }

        #endregion
    }
}
