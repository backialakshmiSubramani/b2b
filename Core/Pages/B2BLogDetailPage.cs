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
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

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
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BLogDetailPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BLogDetailPage(IWebDriver webDriver)
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

        private IWebElement LogDetailData
        {
            get
            {
                //return webDriver.FindElement(By.Id("ContentPageHolder_TxtLogMessage"));
                return webDriver.FindElement(By.XPath(".//*[@id='ContentPageHolder_TxtLogMessage']"));
                //return webDriver.FindElements(By.XPath(".//*[@id='myForm']/table/tbody/tr/td[3]/table/tbody")); 
            }
        }

        private IWebElement ReturnToLogReportLink
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_LnkToList"));
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

        public List<dynamic> GetPoLineItemsFromMapperRequestXml()
        {
            var poLineItems =
                XDocument.Parse(LogDetailData.Text).XPathSelectElements("//LineItems/MapperRequestPOLine");

            var listOfItemInfo = new List<dynamic>();

            var poLineItemCount = poLineItems.ToList().Count();

            for (var i = 0; i < poLineItemCount; i++)
            {
                var poLineItem = poLineItems.FirstOrDefault();

                listOfItemInfo.Add(new
                {
                    Quantity = poLineItem.Element("Quantity").Value,
                    Price = poLineItem.Element("UnitPrice").Value
                });

                poLineItem.Remove();
            }

            return listOfItemInfo.Any() ? listOfItemInfo : null;
        }

        public string GetLogDetail()
        {
            return LogDetailData.Text.Replace("&", "&amp;");
        }

        public string GetDpidFromMapperRequestXml()
        {
            var dpid = XDocument.Parse(LogDetailData.Text).XPathSelectElement("//DPID");

            return dpid != null ? dpid.Value : string.Empty;
        }

        public IEnumerable<string> GetItemIdsFromMapperRequestXml()
        {
            var itemIds =
                XDocument.Parse(LogDetailData.Text)
                    .XPathSelectElements("//LineItems/MapperRequestPOLine/FulfillmentItems/Id")
                    .Select(i => i.Value);
            return itemIds;
        }

        public void ReturnToLogReport()
        {
            ////ReturnToLogReportLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ReturnToLogReportLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
    }
}
