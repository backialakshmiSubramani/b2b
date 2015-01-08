using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Microsoft.SharePoint.Client;
using Excel = Microsoft.Office.Interop.Excel;
using System.Xml.XPath;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class CrtUpload
    {
        private IWebDriver webDriver;

        public CrtUpload(IWebDriver driver)
        {
            this.webDriver = driver;
        }

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BCrossReferenceListPage B2BCrossReferenceListPage
        {
            get
            {
                return new B2BCrossReferenceListPage(webDriver);
            }
        }

        private B2BCrossReferenceMaintenancePage B2BCrossReferenceMaintenencePage
        {
            get
            {
                return new B2BCrossReferenceMaintenancePage(webDriver);
            }
        }

        private B2BViewCrossReferenceXmlPage B2BViewCrossReferenceXmlPage
        {
            get
            {
                return new B2BViewCrossReferenceXmlPage(webDriver);
            }
        }

        public void UploadCrtFile(
            RunEnvironment environment,
            string crossReferenceType,
            string filePath,
            string description)
        {
            B2BHomePage.SelectEnvironment(environment.ToString());
            B2BHomePage.ClickCrossReferenceListLink();
            B2BCrossReferenceListPage.ClickNewCrossReference();
            ////selects CRT Type
            B2BCrossReferenceMaintenencePage.SelectCrossReferenceType(crossReferenceType);
            ////Sets the path for File Upload
            B2BCrossReferenceMaintenencePage.SetUploadFilePath(filePath);
            B2BCrossReferenceMaintenencePage.SetDescription(description);
            B2BCrossReferenceMaintenencePage.ClickSave();
        }

        public bool IsErrorMessageDisplayed()
        {
            return B2BCrossReferenceMaintenencePage.IsErrorMessageDisplayed();
        }

        public bool IsSuccessfulMessageDisplayed()
        {
            return B2BCrossReferenceMaintenencePage.IsSuccessfulMessageDisplayed();
        }

        public bool IsCrossReferenceXmlAvailable()
        {
            if (!IsSuccessfulMessageDisplayed())
            {
                return false;
            }

            var crId = B2BCrossReferenceMaintenencePage.GetCrId();
            B2BCrossReferenceMaintenencePage.GoToCrossReferenceListPage();
            B2BCrossReferenceListPage.ViewXmlForCrId(crId);
            return B2BViewCrossReferenceXmlPage.ParsePageSourceToXml();
        }

        public bool AreDetailsAvailableInCrossReferenceXmlAfterCrtUpload(string crtFilePath)
        {
            int rowId;

            if (!this.IsCrossReferenceXmlAvailable())
            {
                return false;
            }

            var crtValues = B2BViewCrossReferenceXmlPage.GetCrtValuesFromXml();
            if (crtValues == null)
            {
                return false;
            }

            var excelApplication = new Excel.Application();
            var crtEndUserId = crtValues.XPathSelectElement("//Item[@Id='ID']").Value;
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
            ////worksheet.Activate();
            ((Excel._Worksheet)worksheet).Activate();

            for (var i = 2; ; i++)
            {
                if (worksheet.Cells[i, 1].Text == crtEndUserId)
                {
                    rowId = i;
                    break;
                }

                if (string.IsNullOrWhiteSpace(worksheet.Cells[i, 1].Text))
                {
                    workbook.Close();
                    excelApplication.Quit();
                    return false;
                }
            }


            if (worksheet.Cells[rowId, 2].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EUWorkFlowID']").Value)
                && worksheet.Cells[rowId, 3].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EUPRStatus']").Value)
                && worksheet.Cells[rowId, 4].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EUAffinityID']").Value)
                && worksheet.Cells[rowId, 5].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EUDOMSCust']").Value)
                && worksheet.Cells[rowId, 6].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EULocalChannel']").Value)
                && worksheet.Cells[rowId, 7].Text.Trim().Equals(crtValues.XPathSelectElement("//Item[@Id='EUPartyID']").Value))
            {
                workbook.Close();
                excelApplication.Quit();
                return true;
            }

            workbook.Close();
            excelApplication.Quit();
            return false;
        }
    }
}
