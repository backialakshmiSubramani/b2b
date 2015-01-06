// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/19/2014 5:45:56 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/19/2014 5:45:56 PM
// ***********************************************************************
// <copyright file="B2BLogDetailPages.cs" company="Dell">
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
    public class B2BLogDetailPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BLogDetailPage(IWebDriver webDriver)
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

        private IWebElement LogDetailData
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_TxtLogMessage"));
            }
        }

        public List<string> GetEndUserDetailsFromLogDetail()
        {
            var endUserDetailElement =
                XDocument.Parse(LogDetailData.Text).XPathSelectElement("//EndUserParty/Party/EndUserDetail");

            if (endUserDetailElement != null)
            {
                return new List<string>
                           {
                               endUserDetailElement.Element("B2BCRTEndUserId").Value,
                               endUserDetailElement.Element("EUWorkFlowID").Value,
                               endUserDetailElement.Element("EUPRStatus").Value,
                               endUserDetailElement.Element("EUAffinityID").Value,
                               endUserDetailElement.Element("EUDOMSCust").Value,
                               endUserDetailElement.Element("EULocalChannel").Value,
                               endUserDetailElement.Element("EUPartyID").Value
                           };
            }

            return null;
        }

        public List<string> GetPoLineItemsFromMapperRequestXml()
        {
            var poLineItem =
                XDocument.Parse(LogDetailData.Text).XPathSelectElement("//LineItems/MapperRequestPOLine");
            if (poLineItem != null)
            {
                return new List<string>
                           {
                               poLineItem.Element("Quantity").Value,
                               poLineItem.Element("UnitPrice").Value
                           };
            }

            return null;
        }

        public string GetLogDetail()
        {
            return LogDetailData.Text;
        }

        public string GetDpidFromMapperRequestXml()
        {
            var dpid = XDocument.Parse(LogDetailData.Text).XPathSelectElement("//DPID");

            return dpid != null ? dpid.Value : null;
        }
    }
}
