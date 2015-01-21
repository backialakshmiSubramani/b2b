using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using Modules.Channel.B2B.Core.Pages;


namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    public class ViewCrList
    {
        private readonly IWebDriver _webDriver;

        private B2BHomePage B2BhomePage
        {
            get { return new B2BHomePage(_webDriver); }
        }

        private B2BCrossReferenceListPage B2BCrossReferenceListPage
        {
            get { return new B2BCrossReferenceListPage(_webDriver); }
        }

        private B2BCrossReferenceMaintenancePage CrossReferenceMaintenancePage
        {
            get
            {
                return new B2BCrossReferenceMaintenancePage(_webDriver);
            }
        }
        private B2BPreviewAssociatedCrossReferenceListPage PreviewAssociatedCrossReferenceList
        {
            get
            {
                return new B2BPreviewAssociatedCrossReferenceListPage(_webDriver);
            }
        }
        public ViewCrList(IWebDriver driver, string environment)
        {
            _webDriver = driver;
            B2BhomePage.SelectEnvironment(environment);
        }
        public string OpenCrossReferenceList()
        {
            B2BhomePage.ClickCrossReferenceListLink();
            return _webDriver.Url;
        }

        public string OpenCrAssociationList()
        {
            B2BhomePage.ClickCrAssociationList();
            return _webDriver.Url;
        }
        public void DispalyCrList(string crossReferenceType)
        {
            B2BCrossReferenceListPage.SelectCrTypeList(crossReferenceType);
            B2BCrossReferenceListPage.ClickViewCrList();
            _webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }
        public bool CheckTypeColumnValue(string columnText)
        {
            return B2BCrossReferenceListPage.GetTypeColumnValues().All(e => e.Text.Contains(columnText));
        }

        public bool ChannelSegmentBookingDropDownCheck(string listOptions)
        {
            OpenCrossReferenceList();
            return B2BCrossReferenceListPage.CheckChannelSegmentBookingDropDown(listOptions);
        }
        private void OpenCrossRefMaintenancePage()
        {
            B2BCrossReferenceListPage.ClickNewCrossReference();
            _webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
        }

        public bool CheckMaintainCrossReferenceLabelsInPage(string crossReferenceType, string fileToUpload, string descriptionText)
        {
            OpenCrossReferenceList();
            OpenCrossRefMaintenancePage();

            if (!CrossReferenceMaintenancePage.CrossReferenceTypeText().Contains(crossReferenceType))
            {

                return false;
            }

            if (!CrossReferenceMaintenancePage.FileUploadText().Contains(fileToUpload))
            {
                return false;
            }

            if (!CrossReferenceMaintenancePage.DescriptionText().Contains(descriptionText))
            {
                return false;
            }

            return true;
        }

        public bool FilterResults(string dropDownText, string environment)
        {
            B2BhomePage.SelectEnvironment(environment);
            OpenCrossReferenceList();
            DispalyCrList(dropDownText);
            return CheckTypeColumnValue(dropDownText);
        }

        public bool ViewAssociatedCrossRefTableAttributeCheck(string accountname, string crossReferenceType, string cRId, string type, string description, string userid, string association, string viewxml, string tableAccountName)
        {
            OpenCrAssociationList();
            PreviewAssociatedCrossReferenceList.SelectAccountName(accountname);
            DispalyCrList(crossReferenceType);
            var crossRefRow = PreviewAssociatedCrossReferenceList.RowText();

            if (!crossRefRow.Contains(cRId))
            {

                return false;
            }

            if (!crossRefRow.Contains(type))
            {

                return false;
            }
            if (!crossRefRow.Contains(description))
            {

                return false;
            }
            if (!crossRefRow.Contains(userid))
            {

                return false;
            }
            if (!crossRefRow.Contains(association))
            {

                return false;
            }
            if (!crossRefRow.Contains(type))
            {

                return false;
            }
            if (!crossRefRow.Contains(viewxml))
            {

                return false;
            }
            if (!crossRefRow.Contains(tableAccountName))
            {

                return false;
            }
            return true;


        }





    }
}
