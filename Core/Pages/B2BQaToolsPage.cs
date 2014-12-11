// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 1:12:13 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 1:12:13 PM
// ***********************************************************************
// <copyright file="B2BQaToolsPage.cs" company="Dell">
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
    public class B2BQaToolsPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BQaToolsPage(IWebDriver webDriver)
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

        // Find Location + Environment, like 'B2BPreview - SIT - GE1'/'B2BDirect - SIT - GE1'
        private IWebElement LocationEnv
        {
            get
            {
                return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[7]/div"));
            }
        }

        // Find sub link of Location + Environment, like 'B2BPreview - SIT - GE1'/'B2BDirect - SIT - GE1'
        private IWebElement LocationEnvLink
        {
            get { return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[7]/ul/li/a")); }

        }

        // Element to find Target URL after setting Location + Environment
        private IWebElement TargetUrl
        {
            get { return webDriver.FindElement(By.Id("@id='InputParameters_Completeurl")); }
        }

        // Find System Type element
        private IWebElement PunchoutCreate
        {
            get { return webDriver.FindElement(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/div")); }
        }

        // Find Element for cxml
        private IWebElement CxmlElement
        {
            get { return webDriver.FindElement(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/div")); }
        }

        // Find Element for 'cXML Punchout (WM) - QA cXML Main Cred PunchOut (Create)'
        private IWebElement CxmlMainCreate
        {
            //get { return webDriver.FindElement(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/ul/li[2]/a")); }
            get
            { return webDriver.FindElement(By.LinkText("cXML Punchout (WM) - QA cXML Main Cred PunchOut (Create)")); }
        }

        // Find Element for Profile Correlator to pass profile ID
        private IWebElement ProfileCorrelator
        {
            get { return webDriver.FindElement(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[3]/td[2]/input")); }
        }

        // Find Element for User ID Identity to pass profile ID
        private IWebElement UserIDIdentity
        {
            get { return webDriver.FindElement(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[4]/td[2]/input")); }
        }


        private IWebElement ApplyParameter
        {
            get { return webDriver.FindElement(By.XPath("//input[@value='Apply Parameters']")); }
        }

        private IWebElement SubmitMessage
        {
            get { return webDriver.FindElement(By.XPath("//input[@value='Submit Message']")); }
        }

        public IWebElement OutputElement
        {
            get { return webDriver.FindElement(By.XPath("//div[@id='Output']/fieldset/div/div/div")); }
        }

        public IWebElement StoreLinkElement
        {
            get { return webDriver.FindElement(By.LinkText("Click here to go to the store")); }
        }

        # endregion

        # region Element Actions

        public void Wait_For_Title()
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }


        // click on env ('B2B Direct - SIT - GE1'/'B2B Preview - SIT - GE1')
        public void Click_LocationEnv()
        {
            LocationEnv.Click();
            webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='environmentTree']/li[7]/ul/li/a"), TimeSpan.FromSeconds(15));
        }

        // click LocationEnvLink
        public void Click_LocationEnvLink()
        {
            LocationEnvLink.Click();
            webDriver.WaitForElementDisplayed(By.Id("@id='InputParameters_Completeurl"), TimeSpan.FromSeconds(20));
        }

        // click System Type- PunchoutCreate
        public void Click_PunchoutCreate()
        {
            PunchoutCreate.Click();
            webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/div"), TimeSpan.FromSeconds(20));
        }

        // Click cxml 
        public void Click_Cxml()
        {
            CxmlElement.Click();
            webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/ul/li[2]/a"), TimeSpan.FromSeconds(20));
        }

        public void ClickCxmlMainCreate()
        {
            CxmlMainCreate.Click();
            webDriver.WaitForElementVisible(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[2]/td[1]/label"), TimeSpan.FromSeconds(20));
        }

        // Provide ID to Profile Correlator
        public void Profile_For_ProfileCorrelator(String profileId)
        {
            ProfileCorrelator.SendKeys(profileId);
        }

        // Provide ID to User Id Identity
        public void Profile_For_UserIdIdentity(String profileId)
        {
            UserIDIdentity.SendKeys(profileId);
        }

        // Click on ApplyParameter button
        public void Click_ApplyParameter()
        {
            ApplyParameter.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(25));
        }

        // Click on SubmitMessage Button
        public void Click_SubmitMessage()
        {
            SubmitMessage.Click();
            webDriver.WaitForElementDisplayed(By.XPath("//div[@id='tabs']/table/tbody/tr[5]/td[2]/table/tbody/tr[2]/td[1]/div/div/div/h3"), TimeSpan.FromSeconds(30));
        }

        # endregion
    }
}