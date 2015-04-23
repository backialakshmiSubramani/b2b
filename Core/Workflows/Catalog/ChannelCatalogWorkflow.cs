using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;

namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    public class ChannelCatalogWorkflow
    {
        private IWebDriver webDriver;
        private B2BHomePage b2BHomePage;
        private B2BCustomerProfileListPage b2BCustomerProfileListPage;
        private B2BManageProfileIdentitiesPage b2BManageProfileIdentitiesPage;
        private B2BBuyerCatalogPage b2BBuyerCatalogPage;
        private B2BProfileSettingsGeneralPage b2BProfileSettingsGeneralPage;
        private B2BCatalogPackagingDataUploadPage b2BCatalogPackagingDataUploadPage;
        private IJavaScriptExecutor javaScriptExecutor;

        public ChannelCatalogWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            b2BHomePage = new B2BHomePage(webDriver);
        }

        public void GoToBuyerCatalogTab(string b2BProfileName)
        {
            b2BHomePage.ClickB2BProfileList();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", b2BProfileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(b2BProfileName);

            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBHCCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        public void CreateNewProfileAndGoToBuyerCatalogTab(string customerSet, string accessGroup, string profileNameBase)
        {
            var newProfileName = profileNameBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            Console.WriteLine(newProfileName);
            b2BHomePage.B2BProfileListLink.Click();
            WaitForPageRefresh();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.CreateNewProfileLink.Click();
            WaitForPageRefresh();
            b2BProfileSettingsGeneralPage = new B2BProfileSettingsGeneralPage(webDriver);
            b2BProfileSettingsGeneralPage.EnterUserId(newProfileName);
            b2BProfileSettingsGeneralPage.EnterCustomerName(newProfileName);
            b2BProfileSettingsGeneralPage.EnterIdentityName(newProfileName);
            b2BProfileSettingsGeneralPage.EnterCustomerSet(customerSet);
            b2BProfileSettingsGeneralPage.SearchLink.Click();
            WaitForPageRefresh();
            if (b2BProfileSettingsGeneralPage.SelectAccessGroupMsgDisplayed())
            {
                b2BProfileSettingsGeneralPage.EnterAccessGroup(accessGroup);
                b2BProfileSettingsGeneralPage.CreateNewProfileButton.Click();
                WaitForPageRefresh();
            }
            else
            {
                throw new ElementNotVisibleException();
            }

            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBHCCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        private bool UploadAndCheckMessageAndValidate(string fileToBeUploaded, string uploadMessage)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
            WaitForPageRefresh();
            return b2BCatalogPackagingDataUploadPage.UploadSuccessMessage.Text.Trim().Equals(uploadMessage);
        }

        private bool UploadAndCheckMessageAndValidate(string fileToBeUploaded, string uploadMessageStartsWith,
            string uploadMessageEndsWith)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
            WaitForPageRefresh();
            return
                b2BCatalogPackagingDataUploadPage.UploadSuccessMessage.Text.Trim().StartsWith(uploadMessageStartsWith) &&
                b2BCatalogPackagingDataUploadPage.UploadSuccessMessage.Text.Trim().EndsWith(uploadMessageEndsWith);
        }

        private void UploadPackagingData(string fileToBeUploaded)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
            WaitForPageRefresh();
        }

        private void WaitForPageRefresh()
        {
            string isloaded = string.Empty;
            do
            {
                Thread.Sleep(4000);
                try
                {
                    isloaded = javaScriptExecutor.ExecuteScript("return window.document.readyState") as string;
                }
                catch
                {
                    // ignored
                }
            } while (isloaded != "complete");
        }

        public string VerifyCancelClickOnUploadAlert(string fileToUpload)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Dismiss();
            return b2BCatalogPackagingDataUploadPage.UploadSuccessMessage.Text.Trim();
        }

        public int VerifyCountOfRecordsInAuditHistory(string packagingDataFile)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            for (var i = 0; i <= 13; i++)
            {
                UploadPackagingData(packagingDataFile);
            }

            return b2BCatalogPackagingDataUploadPage.AuditHistoryRows.Count();
        }

        public bool VerifyOkClickOnUploadAlert(string fileToBeUploaded, string uploadMessage)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            return UploadAndCheckMessageAndValidate(fileToBeUploaded, uploadMessage);
        }

        public bool VerifyInvalidValuesInAlphanumericFields(string invalidValueInOrderCodeFile,
            string invalidValueInOrderCodeErrorMessage, string invalidValueInLobFile, string invalidValueInLobErrorMessage,
            string invalidValueInConfigNameFile, string invalidValueInConfigNameErrorMessage)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            if (!UploadAndCheckMessageAndValidate(invalidValueInOrderCodeFile, invalidValueInOrderCodeErrorMessage))
                return false;

            if (!UploadAndCheckMessageAndValidate(invalidValueInLobFile, invalidValueInLobErrorMessage))
                return false;

            if (!UploadAndCheckMessageAndValidate(invalidValueInConfigNameFile, invalidValueInConfigNameErrorMessage))
                return false;

            return true;
        }

        public bool VerifyInvalidValuesInNumericFields(string invalidValueInPackageHeightFile,
            string nullValueInPackageHeightErrorMessageStart, string nullValueInPackageHeightErrorMessageEnd,
            string invalidValueInPackageLengthFile, string nullValueInPackageLengthErrorMessageStart,
            string nullValueInPackageLengthErrorMessageEnd, string invalidValueInPackageWidthFile,
            string nullValueInPackageWidthErrorMessageStart, string nullValueInPackageWidthErrorMessageEnd,
            string invalidValueInShipWeightFile, string nullValueInShipWeightErrorMessageStart,
            string nullValueInShipWeightErrorMessageEnd)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            if (
                !UploadAndCheckMessageAndValidate(invalidValueInPackageHeightFile,
                    nullValueInPackageHeightErrorMessageStart, nullValueInPackageHeightErrorMessageEnd))
                return false;

            if (
                !UploadAndCheckMessageAndValidate(invalidValueInPackageLengthFile,
                    nullValueInPackageLengthErrorMessageStart, nullValueInPackageLengthErrorMessageEnd))
                return false;

            if (
                !UploadAndCheckMessageAndValidate(invalidValueInPackageWidthFile,
                    nullValueInPackageWidthErrorMessageStart, nullValueInPackageWidthErrorMessageEnd))
                return false;

            if (
                !UploadAndCheckMessageAndValidate(invalidValueInShipWeightFile, nullValueInShipWeightErrorMessageStart,
                    nullValueInShipWeightErrorMessageEnd))
                return false;

            return true;
        }
    }

    public enum FrequencyType
    {
        Days,
        Weeks
    }
}
