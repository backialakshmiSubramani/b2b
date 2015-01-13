using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using Modules.Channel.B2B.Core.Pages;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.SharePoint.Client;
using OpenQA.Selenium.Internal;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class PoOperations
    {
        private IWebDriver webDriver;
        private ArrayList crtDetails;

        public PoOperations(IWebDriver driver)
        {
            this.webDriver = driver;
            crtDetails = new ArrayList();
        }

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BQaToolsPage B2BQaToolsPage
        {
            get
            {
                return new B2BQaToolsPage(webDriver);
            }
        }

        private B2BLogReportPage B2BLogReportPage
        {
            get
            {
                return new B2BLogReportPage(webDriver);
            }
        }

        private B2BLogDetailPage B2BLogDetailPage
        {
            get
            {
                return new B2BLogDetailPage(webDriver);
            }
        }

        private B2BPoViewerPage B2BPoViewerPage
        {
            get
            {
                return new B2BPoViewerPage(webDriver);
            }
        }

        private B2BQuoteViewerPage B2BQuoteViewerPage
        {
            get
            {
                return new B2BQuoteViewerPage(webDriver);
            }
        }

        private GcmMainPage GcmMainPage
        {
            get
            {
                return new GcmMainPage(webDriver);
            }
        }

        private GcmFindEOrderPage GcmFindEOrderPage
        {
            get
            {
                return new GcmFindEOrderPage(webDriver);
            }
        }

        private GcmOrderGroupLogPage GcmOrderGroupLogPage
        {
            get
            {
                return new GcmOrderGroupLogPage(webDriver);
            }
        }

        private GcmEmailFaxViewPage GcmEmailFaxViewPage
        {
            get
            {
                return new GcmEmailFaxViewPage(webDriver);
            }
        }

        private GcmXmlResultsPage GcmXmlResultsPage
        {
            get
            {
                return new GcmXmlResultsPage(webDriver);
            }
        }

        private GcmCustomerContactsPage GcmCustomerContactsPage
        {
            get
            {
                return new GcmCustomerContactsPage(webDriver);
            }
        }

        private GcmEndUserInfoPage GcmEndUserInfoPage
        {
            get
            {
                return new GcmEndUserInfoPage(webDriver);
            }
        }

        public bool AllOperations(
            string poNumber,
            Workflow workflow,
            RunEnvironment environment,
            string crtFilePath,
            string crtEndUserId,
            string gcmUrl,
            string baseItemPrice, string expectedDpidMessage, string expectedPurchaseOderMessage)
        {
            ////poNumber = "CBLORPRODdec192";
            ////crtEndUserId = "2";
            ////baseItemPrice = "3.99";

            this.GetCrtDetails(crtFilePath, crtEndUserId);

            if (!SearchPoInLogReportPage(poNumber))
            {
                return false;
            }

            if (workflow == Workflow.Eudc)
            {
                // 2. Click on PONumber, PO Viewer page viewed. Verify the enduser details against the CRT File data.
                B2BLogReportPage.ClickPoNumberInTable();
                if (!CheckEndUserInfo(B2BPoViewerPage.GetEndUserDetails()))
                {
                    Console.WriteLine("PO Viewer Page does not contain End User Info. Workflow {0}", workflow);
                    return false;
                }

                webDriver.Navigate().Back();
                webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

                // 3. "CBL PO: sendPurchaseOrder" Message, click on the link,verify the enduser details against the CRT file data.

                if (!B2BLogReportPage.FindMessageAndGoToLogDetailPage(expectedPurchaseOderMessage))
                {
                    return false;
                }

                var logDetail = B2BLogDetailPage.GetEndUserDetailsFromLogDetail();

                if (!CheckEndUserInfo(logDetail))
                {
                    Console.WriteLine("Log Detail does not contain End User Info. Workflow {0}", workflow);
                    return false;
                }

                webDriver.Navigate().Back();
                webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            }

            // 4. "Continue Purchase Order: Purchase Order Success: <DPID>" - capture DPID & verify in GCM

            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);

            if (string.IsNullOrEmpty(dellPurchaseId))
            {
                if (environment == RunEnvironment.Production)
                {
                    Console.WriteLine("DP Id is not generated in Production Environment. Stopping the test");
                    return false;
                }

                Console.WriteLine("DP Id is not generated. Stopping verfication. Not continuing with GCM verification");
                return true;
            }

            Console.WriteLine("Starting GCM verification");
            if (!VerifyOrderStatusInGcm(gcmUrl, dellPurchaseId))
            {
                return false;
            }

            // GCM verification for EUDC
            if (workflow == Workflow.Eudc)
            {
                if (!GcmVerificationsForEudc(dellPurchaseId, baseItemPrice))
                {
                    return false;
                }
            }

            return true;
        }

        public void GetCrtDetails(string crtFilePath, string crtEndUserId)
        {
            var excelApplication = new Excel.Application();

            Excel.Workbook workbook =
                excelApplication.Workbooks.Open(
                    System.IO.Directory.GetCurrentDirectory() + "\\" + crtFilePath,
                    Type.Missing,
                    true,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing);

            var worksheet = (Excel.Worksheet)workbook.Worksheets.Item[1];
            ((Excel._Worksheet)worksheet).Activate();

            for (int i = 2; ; i++)
            {
                if (worksheet.Cells[i, 1].Text == crtEndUserId)
                {
                    crtDetails.Add(worksheet.Cells[i, 1].Text);
                    crtDetails.Add(worksheet.Cells[i, 2].Text);
                    crtDetails.Add(worksheet.Cells[i, 3].Text);
                    crtDetails.Add(worksheet.Cells[i, 4].Text);
                    crtDetails.Add(worksheet.Cells[i, 5].Text);
                    crtDetails.Add(worksheet.Cells[i, 6].Text);
                    crtDetails.Add(worksheet.Cells[i, 7].Text);
                    break;
                }

                if (string.IsNullOrWhiteSpace(worksheet.Cells[i, 1].Text))
                {
                    break;
                }
            }

            workbook.Close();
            excelApplication.Quit();
        }

        public bool GcmVerificationsForEudc(string dellPurchaseId, string baseItemPrice)
        {
            GcmFindEOrderPage.ClickViewButton(dellPurchaseId);
            if (!GcmOrderGroupLogPage.DoCustomerNumbersMatch())
            {
                return false;
            }

            GcmOrderGroupLogPage.GoToCustomerContactsPage();
            GcmCustomerContactsPage.GoToEndUserInfoPage();
            if (!CheckEndUserInfo(GcmEndUserInfoPage.GetEndUserDetails()))
            {
                return false;
            }

            webDriver.Navigate().Back(); // Go back to CustomerContacts page
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            webDriver.Navigate().Back(); // Go back to OrderGroupLog page
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

            var parentWindow = webDriver.CurrentWindowHandle;
            GcmOrderGroupLogPage.GoToPrintFaxViewPage();
            if (!GcmEmailFaxViewPage.GetSubTotalValue().Substring(1).Equals(baseItemPrice))
            {
                return false;
            }

            if (!CheckEndUserInfo(GcmEmailFaxViewPage.GetEndUserDetails()))
            {
                return false;
            }

            GcmEmailFaxViewPage.GoToXmlResultsPage();
            if (!CheckEndUserInfo(GcmXmlResultsPage.GetEndUserDetails()))
            {
                return false;
            }

            return true;
        }

        public bool SubmitXmlForPoCreation(string poXml, string environment, string targetUrl, out string poNumber)
        {
            B2BQaToolsPage.ClickLocationEnvironment(environment);
            B2BQaToolsPage.ClickLocationEnvironmentLink(environment);
            B2BQaToolsPage.PasteTargetUrl(targetUrl);
            B2BQaToolsPage.PasteInputXml(poXml);
            B2BQaToolsPage.ClickSubmitMessage();

            var submissionResult = B2BQaToolsPage.GetSubmissionResult();
            Console.WriteLine("Submission Result is: " + submissionResult);

            if (!submissionResult.Contains("200"))
            {
                Console.WriteLine("The status is not 200. The submission result: {0} ", submissionResult);
                poNumber = string.Empty;
                return false;
            }

            poNumber = submissionResult.Split(' ').Last();
            Console.WriteLine("PO Created: " + poNumber);
            return true;
        }

        /// <summary>
        /// Verification points for Sprint16_563614_P1 & Sprint16_563614_P2
        /// </summary>
        /// <param name="quoteRetrievedMessagePrefix"></param>
        /// <param name="quoteRetrievedMessageSuffix"></param>
        /// <param name="enteringMasterOrderGroupMessage"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="unitPrice"></param>
        /// <returns></returns>
        public bool VerifyQmsQuoteCreation(
            string quoteRetrievedMessagePrefix,
            string quoteRetrievedMessageSuffix,
            string enteringMasterOrderGroupMessage,
            string description,
            string quantity,
            string unitPrice)
        {
            quoteRetrievedMessagePrefix = "Continue Purchase Order: ";
            quoteRetrievedMessageSuffix = " retrieved";
            if (!B2BLogReportPage.FindQuoteRetrievalMessage(quoteRetrievedMessagePrefix, quoteRetrievedMessageSuffix))
            {
                return false;
            }

            B2BLogReportPage.FindMessageAndGoToQuoteViewerPage(enteringMasterOrderGroupMessage);
            return B2BQuoteViewerPage.CheckItemDetails(description, quantity, unitPrice);
        }

        /// <summary>
        /// Verification points for Sprint16_570456_P1 & Sprint16_570456_P2
        /// </summary>
        /// <param name="mapperRequestMessage"></param>
        public bool VerifyMapperRequestXmlDataInLogDetailPage(string mapperRequestMessage)
        {
            // TODO: check for multiple lines too
            //mapperRequestMessage = "Continue Purchase Order: ASN Process Start MapperRequest xml";
            B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage);
            var poLineItems = B2BLogDetailPage.GetPoLineItemsFromMapperRequestXml();
            return VerifyMapperRequestDetailsAgainstPoTemplate(poLineItems);
        }

        private bool VerifyMapperRequestDetailsAgainstPoTemplate(List<string> poLineItems)
        {
            var poTemplate = XDocument.Load("CblTemplate.xml");

            if (!poLineItems[0].Equals(poTemplate.XPathSelectElement("//OrderDetail/BaseItemDetail/Quantity/Qty")))
            {
                return false;
            }

            if (!poLineItems[1].Equals(poTemplate.XPathSelectElement("//OrderDetail/BuyerExpectedUnitPrice/Price/UnitPrice")))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verification points for Sprint17_565299_P1 & Sprint17_565299_P2
        /// </summary>
        /// <param name="asnLogEventMessages"></param>
        /// <param name="asnLogDetailMessages"></param>
        /// <returns></returns>
        public bool VerifyMappingEntriesForChannelAsnEnabledProfile(List<string> asnLogEventMessages, List<string> asnLogDetailMessages)
        {
            for (var i = 0; i < asnLogEventMessages.Count; i++)
            {
                B2BLogReportPage.FindMessageAndGoToLogDetailPage(asnLogEventMessages[i]);
                if (!B2BLogDetailPage.GetLogDetail().Equals(asnLogDetailMessages[i]))
                {
                    return false;
                }

                webDriver.Navigate().Back();
            }

            return true;
        }

        /// <summary>
        /// Verification points for Sprint18_P1 & Sprint18_P2 --> 532584
        /// </summary>
        /// <param name="expectedDpidMessage"></param>
        /// <param name="mapperRequestMessage"></param>
        /// <returns></returns>
        public bool VerifyDpidInMapperXml(string expectedDpidMessage, string mapperRequestMessage)
        {
            var dellPurchaseId = B2BLogReportPage.FindDellPurchaseId(expectedDpidMessage);
            B2BLogReportPage.FindMessageAndGoToLogDetailPage(mapperRequestMessage);
            return B2BLogDetailPage.GetDpidFromMapperRequestXml().Equals(dellPurchaseId);
        }

        private bool VerifyOrderStatusInGcm(string gcmUrl, string dellPurchaseId)
        {
            webDriver.Navigate().GoToUrl(gcmUrl);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            // GCM verifcation start
            GcmMainPage.ClickDomsElement();

            // Checking the status, if COMPLETE or not
            var status = GcmFindEOrderPage.SearchByDpidAndGetOrderStatus(dellPurchaseId);
            if (status.ToUpper().Trim().Equals("COMPLETE"))
            {
                return true;
            }

            Console.WriteLine("Aborting test. Status is ** {0} **", status);
            return false;
        }

        private bool SearchPoInLogReportPage(string poNumber)
        {
            // Log Report page operations. Provide po number and search
            B2BHomePage.ClickLogReport();

            return B2BLogReportPage.SearchByPoNumber(poNumber);
        }

        private bool CheckEndUserInfo(List<string> endUserDetails)
        {
            if (endUserDetails != null && endUserDetails.Count() != 0)
            {
                if (endUserDetails[0] == crtDetails[0].ToString() && endUserDetails[1] == crtDetails[1].ToString()
                    && endUserDetails[2] == crtDetails[2].ToString() && endUserDetails[3] == crtDetails[3].ToString()
                    && endUserDetails[4] == crtDetails[4].ToString() && endUserDetails[5] == crtDetails[5].ToString()
                    && endUserDetails[6] == crtDetails[6].ToString())
                {
                    return true;
                }
            }

            return false;
        }

        private bool VerifyMapperRequestDetailsAgainstPoTemplate(List<XElement> poLineItems)
        {
            var orderDetails = XDocument.Load("CblTemplate.xml").XPathSelectElements("//OrderDetail").ToList();

            if (poLineItems.Count != orderDetails.Count)
            {
                return false;
            }

            for (var i = 0; i < poLineItems.Count; i++)
            {
                if (
                    !(poLineItems[i].Element("Quantity")
                          .Value.Equals(orderDetails[i].XPathSelectElement("//BaseItemDetail/Quantity/Qty").Value)
                      && poLineItems[i].Element("UnitPrice")
                             .Value.Equals(
                                 orderDetails[i].XPathSelectElement("//BuyerExpectedUnitPrice/Price/UnitPrice").Value)))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public enum Workflow
    {
        Asn,
        Eudc
    }

    public enum RunEnvironment
    {
        Preview,
        Production
    }

    public enum PoXmlFormat
    {
        Cxml,
        Cbl
    }
}
