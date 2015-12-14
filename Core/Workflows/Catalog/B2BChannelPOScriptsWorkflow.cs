// ***********************************************************************
// Author           : AMERICAS\Tawan_Jyot_Kaur_Bhat
// Created          : 9/24/2015 7:47:01 PM
//
// Last Modified By : AMERICAS\Tawan_Jyot_Kaur_Bhat
// Last Modified On : 9/24/2015 7:47:01 PM
// ***********************************************************************
// <copyright file="DellWebUIPage1.cs" company="Dell">
//     Copyright (c) Dell 2015. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.UI.WebControls;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Microsoft.BusinessData.MetadataModel;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;



namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BChannelPOScriptsWorkflow 
    {
        IWebDriver webDriver;
        private B2BQaToolsPage B2BQaToolsPage;
        private B2BLogReportPage B2BLogReportPage;
        private GcmMainPage GcmMainPage;
        private GcmFindEOrderPage GcmFindEOrderPage;
        private string uniquePoRefNum;
        private string dpid;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BChannelPOScriptsWorkflow(IWebDriver webDriver)
            
        {
            this.webDriver = webDriver;
            
        }
        
        /// <summary>
        /// Verifies Po Posting with CBL1.
        /// </summary>
        /// <param name="qatoolsTargetUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="poRefNum"></param>
        /// <param name="identityName"></param>
        /// <param name="supplierPartIdExt"></param>
        /// <param name="unitPrice"></param>
        /// <param name="quantity"></param>
        public string VerifyPoPosting(string qatoolsTargetUrl, string fileName, string poRefNum,
            string identityName, string supplierPartIdExt, string unitPrice, string quantity)
        {
            uniquePoRefNum = poRefNum + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            B2BQaToolsPage = new B2BQaToolsPage(webDriver);
            B2BQaToolsPage.PasteTargetUrl(qatoolsTargetUrl);
            var file = PoXmlGenerator.GeneratePoCblwithoutCrt(fileName, uniquePoRefNum, identityName, supplierPartIdExt,
                unitPrice, quantity);
            B2BQaToolsPage.PasteInputXml(file);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
            B2BQaToolsPage.ClickSubmitMessage();
            if (
                 B2BQaToolsPage.GetSubmissionResult()
                    .Equals("XML Response received from server Code: 200. Message: PO = " + uniquePoRefNum))
            {
                return uniquePoRefNum;
            }
            throw new Exception("Error while posting PO" + uniquePoRefNum);
   }
        
        /// <summary>
        /// Verifies Po Posting and check in log report.
        /// </summary>
       /// <param name="logReport"></param>
        public string RetrievePodpidFromLogReport(string logReport)
        {
            
            
                webDriver.Navigate().GoToUrl(logReport);
                webDriver.Navigate().GoToUrl(logReport);
                B2BLogReportPage= new B2BLogReportPage(webDriver);
                B2BLogReportPage.SearchPoNumber(uniquePoRefNum);
                dpid = B2BLogReportPage.FindDellPurchaseId("Continue Purchase Order: Purchase Order Success:");
             if (!dpid.Equals(string.Empty))
                {
                    return B2BLogReportPage.FindDellPurchaseId("Continue Purchase Order: Purchase Order Success:");
                }
            throw new Exception("Error while getting dpid from log report");
            
        }
           
        
        /// <summary>
        /// Verifies Po Posting and get dpid from log report and verify in GCM.
        /// </summary>
        /// <param name="gcmUrl"></param>
        public bool VerifyPodpidInGcm(string gcmUrl)
        {
                    webDriver.Navigate().GoToUrl(gcmUrl);
                    GcmMainPage= new GcmMainPage(webDriver);
                    GcmMainPage.ClickDomsElement();
                    GcmFindEOrderPage= new GcmFindEOrderPage(webDriver);
                    string orderStatus = GcmFindEOrderPage.SearchByDpidAndGetOrderStatus(dpid);
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            if (orderStatus.Equals("Available") || orderStatus.Equals("Complete"))
            {
                return true;
            }
            throw new Exception("Status is not Available or Complete in GCM page. Check manually with this dpid:" + dpid);
        }


        /// <summary>
        /// Verifies Po Posting with CBL3.
        /// </summary>
        /// <param name="qatoolsTargetUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="poRefNum"></param>
        /// <param name="identityName"></param>
        /// <param name="supplierPartIdExt"></param>
        /// <param name="unitPrice"></param>
        /// <param name="quantity"></param>
        public string VerifyPoPostingwithCbl3(string qatoolsTargetUrl, string fileName, string poRefNum,
            string identityName, string supplierPartIdExt, string unitPrice, string quantity)
        {
            uniquePoRefNum = poRefNum + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            B2BQaToolsPage = new B2BQaToolsPage(webDriver);
            B2BQaToolsPage.PasteTargetUrl(qatoolsTargetUrl);
            var file = PoXmlGenerator.GeneratePoCbl3(fileName, uniquePoRefNum, identityName, supplierPartIdExt,
                unitPrice, quantity);
            B2BQaToolsPage.PasteInputXml(file);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
            B2BQaToolsPage.ClickSubmitMessage();
            if (
                 B2BQaToolsPage.GetSubmissionResult()
                    .Equals("XML Response received from server Code: 200. Message: PO = " + uniquePoRefNum))
            {
                return uniquePoRefNum;
            }
            throw new Exception("Error while posting PO" + uniquePoRefNum);
        }

        /// <summary>
        /// Verifies Po Posting with CXML.
        /// </summary>
        /// <param name="qatoolsTargetUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="poRefNum"></param>
        /// /// <param name="profileName"></param>
        /// <param name="identityName"></param>
        /// <param name="supplierPartIdExt"></param>
        /// <param name="unitPrice"></param>
        /// <param name="quantity"></param>
        public string VerifyPoPostingwithCxml(string qatoolsTargetUrl, string fileName, string poRefNum,
            string profileName,string identityName, string supplierPartIdExt, string unitPrice, string quantity)
        {
            uniquePoRefNum = poRefNum + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            B2BQaToolsPage = new B2BQaToolsPage(webDriver);
            B2BQaToolsPage.PasteTargetUrl(qatoolsTargetUrl);
            var file = PoXmlGenerator.GeneratePoCxmll(fileName, uniquePoRefNum,profileName,
                identityName, supplierPartIdExt,unitPrice, quantity);
            B2BQaToolsPage.PasteInputXml(file);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
            B2BQaToolsPage.ClickSubmitMessage();
            if (
                 B2BQaToolsPage.GetSubmissionResult()
                    .Equals("XML Response received from server Code: 200. Message: PO = " + uniquePoRefNum))
            {
                return uniquePoRefNum;
            }
            throw new Exception("Error while posting PO" + uniquePoRefNum);
        }
    }
}
