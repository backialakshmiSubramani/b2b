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
using System.Xml.Linq;
using OpenQA.Selenium.Interactions;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BQaToolsPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BQaToolsPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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

        // Find Location + Environment, like 'B2BPreview - SIT - GE1'/'B2BDirect - SIT - GE1'
        private IWebElement LocationEnvironmentPreview
        {
            get
            {
                return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[7]/div"), new TimeSpan(0, 0, 10));
            }
        }

        private IWebElement LocationEnvironmentProduction
        {
            get
            {
                return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[6]/div"));
            }
        }

        // Find sub link of Location + Environment, like 'B2BPreview - SIT - GE1'/'B2BDirect - SIT - GE1'
        private IWebElement LocationEnvironmentLinkPreview
        {
            get
            {
                return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[7]/ul/li/a"));
            }

        }

        private IWebElement LocationEnvironmentLinkProduction
        {
            get
            {
                return webDriver.FindElement(By.XPath("//ul[@id='environmentTree']/li[6]/ul/li/a"));
            }

        }

        // Element to find Target URL after setting Location + Environment
        private IWebElement TargetUrl
        {
            get
            {
                return webDriver.FindElement(By.Id("InputParameters_Completeurl"));
            }
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
            {
                return webDriver.FindElement(By.LinkText("cXML Punchout (WM) - QA cXML Main Cred PunchOut (Create)"));
            }
        }

        // Find Element for Profile Correlator to pass profile ID
        private IWebElement ProfileCorrelatorIdentity
        {
            get
            {
                return webDriver.FindElement(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[3]/td[2]/input"));
            }
        }

        // Find Element for User ID Identity to pass profile ID
        private IWebElement UserIdIdentity
        {
            get
            {
                return webDriver.FindElement(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[4]/td[2]/input"));
            }
        }


        private IWebElement ApplyParameter
        {
            get { return webDriver.FindElement(By.XPath("//input[@value='Apply Parameters']")); }
        }

        private IWebElement SubmitMessage
        {
            get { return webDriver.FindElement(By.XPath("//input[@value='Submit Message']")); }
        }

        private IWebElement SubmissionResult
        {
            get { return webDriver.FindElement(By.XPath("//div[@id='Output']/fieldset/div/div[1]/div")); }
        }

        private IWebElement StoreLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Click here to go to the store"));
            }
        }

        #endregion

        #region Element Actions

        public void WaitForTitle()
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }


        // click on env ('B2B Direct - SIT - GE1'/'B2B Preview - SIT - GE1')
        public void ClickLocationEnvironment(string environment)
        {
            if (environment.ToUpper().Equals("PREVIEW"))
            {
                ////LocationEnvironmentPreview.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", LocationEnvironmentPreview);
                webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='environmentTree']/li[7]/ul/li/a"), TimeSpan.FromSeconds(15));
            }
            else if (environment.ToUpper().Equals("PRODUCTION"))
            {
                ////LocationEnvironmentProduction.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", LocationEnvironmentProduction);
                webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='environmentTree']/li[6]/ul/li/a"), TimeSpan.FromSeconds(15));
            }
        }



        // click LocationEnvLink
        public void ClickLocationEnvironmentLink(string environment)
        {
            if (environment.ToUpper().Equals("PREVIEW"))
            {
                ////LocationEnvironmentLinkPreview.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", LocationEnvironmentLinkPreview);
            }
            else if (environment.ToUpper().Equals("PRODUCTION"))
            {
                ////LocationEnvironmentLinkProduction.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", LocationEnvironmentLinkProduction);
            }

            webDriver.WaitForElementDisplayed(By.Id("@id='InputParameters_Completeurl"), TimeSpan.FromSeconds(20));
        }

        // click System Type- PunchoutCreate
        public void ClickPunchoutCreate()
        {
            ////PunchoutCreate.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PunchoutCreate);
            webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/div"), TimeSpan.FromSeconds(20));
        }

        // Click cxml 
        public void ClickCxml()
        {
            ////CxmlElement.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CxmlElement);
            webDriver.WaitForElementDisplayed(By.XPath("//ul[@id='OperationAndSampleTree']/li[1]/ul/li[6]/ul/li[2]/a"), TimeSpan.FromSeconds(20));
        }

        public void ClickCxmlMainCreate()
        {
            ////CxmlMainCreate.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CxmlMainCreate);
            webDriver.WaitForElementVisible(By.XPath("//div[@id='dynamicParams']/table/tbody/tr[2]/td[1]/label"), TimeSpan.FromSeconds(20));
        }

        // Provide ID to Profile Correlator
        public void IdentityForProfileCorrelator(string profileId)
        {
            ProfileCorrelatorIdentity.SendKeys(profileId);
        }

        // Provide ID to User Id Identity
        public void IdentityForUserId(string profileId)
        {
            UserIdIdentity.SendKeys(profileId);
        }

        // Click on ApplyParameter button
        public void ClickApplyParameter()
        {
            ////ApplyParameter.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ApplyParameter);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(25));
        }

        // Click on SubmitMessage Button
        public void ClickSubmitMessage()
        {
            ////SubmitMessage.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SubmitMessage);
            webDriver.WaitForElementDisplayed(By.XPath("//div[@id='Output']/fieldset/div/div[1]/h3"), TimeSpan.FromSeconds(120));
        }

        #endregion

        ////public void PasteInputXml(IEnumerable<string> poXml)
        public void PasteInputXml(string poXml)
        {
            javaScriptExecutor.ExecuteScript("arguments[0].value = arguments[1];", InputXmlTextArea, poXml);
        }

        private IWebElement InputXmlTextArea
        {
            get
            {
                return webDriver.FindElement(By.Id("TxtContent"));
            }
        }

        public void PasteTargetUrl(string targetUrl)
        {
            TargetUrl.Clear();
            TargetUrl.SendKeys(targetUrl);
        }

        public string GetSubmissionResult()
        {
            return SubmissionResult.Text;
        }

        public string GetStoreLinkText()
        {
            return StoreLink.Text;
        }

        public void ClickStoreLink()
        {
            ////StoreLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", StoreLink);
        }
    }
}