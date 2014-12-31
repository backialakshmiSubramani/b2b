// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/11/2014 12:06:27 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/11/2014 12:06:27 PM
// ***********************************************************************
// <copyright file="LogReportPage.cs" company="Dell">
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
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BLogReportPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BLogReportPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)webDriver;
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

        private IWebElement ThreadIdElement
        {
            get
            {
                return webDriver.FindElement(By.Id("tBox_ThreadID"));
            }
        }

        private IWebElement PoNumberElement
        {
            get
            {
                return webDriver.FindElement(By.Id("tBox_PONum"));
            }
        }
        private IWebElement QuoteIdElement
        {
            get
            {
                return webDriver.FindElement(By.Id("tbox_QuoteID"));
            }
        }

        private IWebElement IncludeInsertLogCheckBox
        {
            get
            {
                return webDriver.FindElement(By.Id("chkb_InsertLog"));
            }
        }

        private IWebElement SubmitLink
        {
            get
            {
                return webDriver.FindElement(By.Id("btn_Submit"));
            }
        }
        private IWebElement ThreadIdInTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='DetailedGridView_grdMain']/tbody/tr[2]/td[5]/span"));

            }
        }

        private IWebElement QuoteIdInTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='DetailedGridView_grdMain']/tbody/tr[2]/td[8]/a"));
            }
        }

        private IWebElement PoNumberInTable
        {
            get
            {
                return webDriver.FindElement(By.XPath("//table[@id='DetailedGridView_grdMain']/tbody/tr[2]/td[9]/a"));
            }
        }

        private IEnumerable<IWebElement> PoLogReportRows
        {
            get
            {
                return webDriver.FindElements(By.XPath("//table[@id='DetailedGridView_grdMain']/tbody/tr")).Skip(1);
            }
        }

        #endregion

        #region Element Actions

        public void ProvidePO(string poNumber)
        {
            PoNumberElement.SendKeys(poNumber);
        }

        public void ClickSubmit()
        {
            ////IncludeInsertLogCheckBox.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", IncludeInsertLogCheckBox);
            ////SubmitLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SubmitLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        public string FindThreadIdInTable()
        {
            return ThreadIdInTable.Text;
        }

        public string FindQuoteIdInTable()
        {
            return QuoteIdInTable.Text;
        }

        public void ClickPoNumberInTable()
        {
            ////PoNumberInTable.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PoNumberInTable);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Common
        /// </summary>
        /// <param name="expectedLogEntry"></param>
        /// <returns></returns>
        public string FindDellPurchaseId(string expectedLogEntry)
        {
            var dellPurchaseIdEntry =
                PoLogReportRows.FirstOrDefault(e => e.FindElements(By.TagName("td"))[5].Text.Contains(expectedLogEntry));

            var dellPurchaseIdMessage = dellPurchaseIdEntry.FindElements(By.TagName("td"))[5].Text;

            Console.WriteLine("DP ID is generated with message -> {0}", dellPurchaseIdMessage);

            return dellPurchaseIdMessage.Trim().Split(' ').LastOrDefault();
        }

        #endregion

        /// <summary>
        /// Looks for a specific message in the table, clicks on the timestamp link.
        /// </summary>
        /// <param name="purchaseOderMessage">Message to look for</param>
        public void FindMessageAndCheckEndUserInfoInLogDetailPage(string purchaseOderMessage)
        {
            var purchaseOrderMessageRow =
                PoLogReportRows.FirstOrDefault(
                    e => e.FindElements(By.TagName("td"))[5].Text.Contains(purchaseOderMessage));
            ////purchaseOrderMessageRow.FindElements(By.TagName("td"))[0].Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", purchaseOrderMessageRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")));
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
    }
}
