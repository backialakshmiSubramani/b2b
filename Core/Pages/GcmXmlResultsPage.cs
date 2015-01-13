// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/23/2014 7:06:45 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/23/2014 7:06:45 PM
// ***********************************************************************
// <copyright file="GcmXmlResultsPage.cs" company="Dell">
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
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class GcmXmlResultsPage : DCSGPageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public GcmXmlResultsPage(IWebDriver webDriver)
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

        private IWebElement OgXmlAuditLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='tblResults']/tbody/tr[th/text()='OG XML Audit']/following-sibling::tr[2]/td/a"));
            }
        }

        public List<string> GetEndUserDetails()
        {
            var ogXml = this.GetOgXml();

            if (ogXml == null)
            {
                return null;
            }

            var endUserDetailElement = ogXml.XPathSelectElement("//EndUserInfo");

            if (endUserDetailElement != null)
            {
                return new List<string>
                           {
                               endUserDetailElement.Element("EndUserId").Value,
                               endUserDetailElement.Element("PartnerRequestId").Value,
                               endUserDetailElement.Element("PartnerRequestStatus").Value,
                               endUserDetailElement.Element("CustomerAccountId").Value,
                               endUserDetailElement.Element("OMSCustomerId").Value,
                               endUserDetailElement.Element("SalesChannelId").Value,
                               endUserDetailElement.Element("PartyId").Value
                           };
            }

            return null;
        }

        public XDocument GetOgXml()
        {
            ////OgXmlAuditLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", OgXmlAuditLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

            XDocument pageSourceXml;
            string bodyContent;
            try
            {
                bodyContent = webDriver.FindElement(By.TagName("body")).Text;
            }
            catch
            {
                bodyContent = (string)((IJavaScriptExecutor)this.webDriver).ExecuteScript("return document.getElementsByTagName('body')[0].innerText;");
            }

            bodyContent = bodyContent.Trim()
                .Replace("- ", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty);
            pageSourceXml = XDocument.Parse(bodyContent);

            return pageSourceXml;
        }
    }
}
