// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 9/30/2016 6:09:42 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 9/30/2016 6:09:42 PM
// ***********************************************************************
// <copyright file="B2BToolsHomePage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BToolsHomePage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BToolsHomePage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";

        }

        private SelectElement EnvironmentList
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//*[@id='ucLeftMenu_ddlEnv']"), new TimeSpan(0, 0, 30));
                return new SelectElement(webDriver.FindElement(By.XPath("//*[@id='ucLeftMenu_ddlEnv']")));
            }
        }

        private IWebElement GoButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ucLeftMenu_lnkGo"));
            }
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

        public void OpenB2BHomePage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            SelectEnvironment(b2BEnvironment.ConvertToString());
        }

        public void SelectEnvironment(string environmentValue)
        {
            int index = 0;
            for (; index < EnvironmentList.Options.Count; index++)
            {
                if (EnvironmentList.Options[index].Text.Trim() == environmentValue)
                    break;
            }
            EnvironmentList.SelectByIndex(index);
            Console.WriteLine("B2B environment selected is: ** {0} **", environmentValue);
            UtilityMethods.ClickElement(webDriver, GoButton);
        }
    }
}
