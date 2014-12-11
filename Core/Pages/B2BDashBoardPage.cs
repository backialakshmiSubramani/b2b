// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 2:34:17 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 2:34:17 PM
// ***********************************************************************
// <copyright file="DashBoardPage.cs" company="Dell">
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
    public class B2BDashBoardPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BDashBoardPage(IWebDriver webDriver)
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

        # region Element
        private IWebElement Title
        {
            get
            { return webDriver.FindElement(By.XPath("//div[@class='uif_mhFooterWrap clearfix']/h1")); }
        }

        private IWebElement ShopElement
        {
            get
            { return webDriver.FindElement(By.XPath("//li[@id='ShopMenu']/div/a")); }
        }

        private IWebElement StandardConfigElement
        {
            get
            { return webDriver.FindElement(By.XPath("//li[@id='ShopMenu1']/div/a")); }
        }


        # endregion

        # region Element Actions

        public void Wait_For_Title()
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(120));
        }

        public String Return_Title()
        {
            return Title.Text;
        }

        public void Open_Shop()
        {
            ShopElement.Click();
        }

        public void Click_Standard_Config()
        {
            StandardConfigElement.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(60));
        }

        # endregion
    }
}


