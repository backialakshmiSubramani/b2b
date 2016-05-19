using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium.Support.UI;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using System.Data;
using System.Configuration;
using CatalogTests.Common.CatalogXMLTemplates;
using Modules.Channel.Utilities;

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
        private CPTAutoCatalogInventoryListPage b2BAutoCatalogListPage;
        private B2BChannelUx b2bChannelUx;
        private IJavaScriptExecutor javaScriptExecutor;
        private const string MMDDYYYY = "MM/dd/yyyy";
        private int HeaderRowsCount = 0, Headercount = 0, subHeaderRows = 0;

        /// <summary>
        /// Constructor for ChannelCatalogWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelCatalogWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            b2BHomePage = new B2BHomePage(webDriver);
        }

        /// <summary>
        /// Searches for the profile provided in B2B Profile List page and 
        /// navigates to the Buyer Catalog Tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        public void GoToBuyerCatalogTab(string environment, string profileName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ClickB2BProfileList();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", profileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName);
            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("Opened Profile Page for profile: {0}", profileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Creates a new profile with the CustomerSet & AccessGroup provided 
        /// and navigates to the Buyer Catalog Tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        public void CreateNewProfileAndGoToBuyerCatalogTab(string environment, string customerSet, string accessGroup, string profileNameBase)
        {
            var newProfileName = profileNameBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");

            Console.WriteLine("Profile creation start with name: {0}", newProfileName);
            b2BHomePage.SelectEnvironment(environment);
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
            //WaitForPageRefresh();
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
            Console.WriteLine("New profile created with Name: {0}", newProfileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Creates a new profile with the CustomerSet & AccessGroup provided
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        public string CreateNewProfile(string environment, string customerSet, string accessGroup, string profileNameBase)
        {
            var newProfileName = profileNameBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            Console.WriteLine("Profile creation start with name: {0}", newProfileName);
            b2BHomePage.SelectEnvironment(environment);
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
            Console.WriteLine("New profile created with Name: {0}", newProfileName);
            return newProfileName;
        }

        /// <summary>
        /// Waits for the page to refresh after navigation
        /// </summary>
        public void WaitForPageRefresh()
        {
            var isloaded = string.Empty;
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

        /// <summary>
        /// Verifies the impact of clicking on Cancel on 
        /// Upload alert in Packaging Data Upload Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="fileToUpload"></param>
        /// <returns></returns>
        public string VerifyCancelClickOnUploadAlert(string environment, string fileToUpload)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Dismiss();
            return b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim();
        }

        /// <summary>
        /// Returns the count of Audit History Records on Packaging Data Upload Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="packagingDataFile"></param>
        /// <returns></returns>
        public int VerifyCountOfRecordsInAuditHistory(string environment, string packagingDataFile)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ChannelCatalogUxLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            for (var i = 0; i <= 13; i++)
            {
                Console.WriteLine("Upload No: {0}", i);
                UploadPackagingData(packagingDataFile);
            }

            return b2BCatalogPackagingDataUploadPage.AuditHistoryRows.Count();
        }

        /// <summary>
        /// Verifies the impact of clicking on OK on 
        /// Upload alert in Packaging Data Upload Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="fileToBeUploaded"></param>
        /// <param name="uploadMessage"></param>
        /// <returns></returns>
        public bool VerifyOkClickOnUploadAlert(B2BEnvironment environment, string fileToBeUploaded, string uploadMessage)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.ChannelCatalogUxLink.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            return UploadAndCheckMessageAndValidate(fileToBeUploaded, uploadMessage);
        }

        /// <summary>
        /// Validates the messages received on uploading packaging data files 
        /// with Invalid Values in OrderCode, LOB & ConfigName columns in Packaging Data Upload Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="invalidValueInOrderCodeFile"></param>
        /// <param name="invalidValueInOrderCodeErrorMessage"></param>
        /// <param name="invalidValueInLobFile"></param>
        /// <param name="invalidValueInLobErrorMessage"></param>
        /// <param name="invalidValueInConfigNameFile"></param>
        /// <param name="invalidValueInConfigNameErrorMessage"></param>
        /// <returns></returns>
        public bool VerifyInvalidValuesInAlphanumericFields(string environment, string invalidValueInOrderCodeFile,
            string invalidValueInOrderCodeErrorMessage, string invalidValueInLobFile, string invalidValueInLobErrorMessage,
            string invalidValueInConfigNameFile, string invalidValueInConfigNameErrorMessage)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            if (!UploadAndCheckMessageAndValidate(invalidValueInOrderCodeFile, invalidValueInOrderCodeErrorMessage))
                return false;

            if (!UploadAndCheckMessageAndValidate(invalidValueInLobFile, invalidValueInLobErrorMessage))
                return false;

            if (!UploadAndCheckMessageAndValidate(invalidValueInConfigNameFile, invalidValueInConfigNameErrorMessage))
                return false;

            return true;
        }

        /// <summary>
        /// Validates the messages received on uploading packaging data files with Invalid Values
        /// in PackageHeight, PackageLength, PackageWidth & ShipWeight columns in Packaging Data Upload Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="invalidValueInPackageHeightFile"></param>
        /// <param name="nullValueInPackageHeightErrorMessageStart"></param>
        /// <param name="nullValueInPackageHeightErrorMessageEnd"></param>
        /// <param name="invalidValueInPackageLengthFile"></param>
        /// <param name="nullValueInPackageLengthErrorMessageStart"></param>
        /// <param name="nullValueInPackageLengthErrorMessageEnd"></param>
        /// <param name="invalidValueInPackageWidthFile"></param>
        /// <param name="nullValueInPackageWidthErrorMessageStart"></param>
        /// <param name="nullValueInPackageWidthErrorMessageEnd"></param>
        /// <param name="invalidValueInShipWeightFile"></param>
        /// <param name="nullValueInShipWeightErrorMessageStart"></param>
        /// <param name="nullValueInShipWeightErrorMessageEnd"></param>
        /// <returns></returns>
        public bool VerifyInvalidValuesInNumericFields(string environment, string invalidValueInPackageHeightFile,
            string nullValueInPackageHeightErrorMessageStart, string nullValueInPackageHeightErrorMessageEnd,
            string invalidValueInPackageLengthFile, string nullValueInPackageLengthErrorMessageStart,
            string nullValueInPackageLengthErrorMessageEnd, string invalidValueInPackageWidthFile,
            string nullValueInPackageWidthErrorMessageStart, string nullValueInPackageWidthErrorMessageEnd,
            string invalidValueInShipWeightFile, string nullValueInShipWeightErrorMessageStart,
            string nullValueInShipWeightErrorMessageEnd)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(1));
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
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

        /// <summary>
        /// Verifies default delta scheduling options for a new profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDeltaSchedulingDefaultOptionsForNewProfile(string environment, string customerSet, string accessGroup,
            string profileNameBase, string defaultDeltaTimeOfSend)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(environment, customerSet, accessGroup, profileNameBase);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            return VerifyDeltaCatalogSchedulingOptions(DateTime.Now.AddDays(1).ToString(MMDDYYYY), "0", "1",
                DateTime.Now.AddDays(59).ToString(MMDDYYYY), defaultDeltaTimeOfSend);
        }

        /// <summary>
        /// Verifies default delta scheduling options for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDeltaSchedulingDefaultOptionsForExistingProfile(string environment, string profileName,
            string defaultDeltaTimeOfSend)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (
                !VerifyDeltaCatalogSchedulingOptions(DateTime.Now.AddDays(1).ToString(MMDDYYYY), "0", "1",
                    DateTime.Now.AddDays(59).ToString(MMDDYYYY), defaultDeltaTimeOfSend))
                return false;

            return true;
        }

        /// <summary>
        /// Changes the default delta scheduling options and verifies 
        /// if the options are retained on updating and navigating back to Buyer Catalog Tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool ChangeDeltaSchedulingDefaultOptionsForExistingProfile(string environment, string profileName)
        {
            var startDate = DateTime.Now.AddDays(5).ToString(MMDDYYYY);
            var frequencyDays = "2";
            var endDate = DateTime.Now.AddDays(30).ToString(MMDDYYYY);
            var timeOfSend = "9";
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            SetDeltaSchedule(startDate, frequencyDays, FrequencyType.Days, endDate, timeOfSend);
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            if (!VerifyDeltaCatalogSchedulingOptions(startDate, frequencyDays, "0", endDate, timeOfSend))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the status table values on Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="autoCatalogStatus"></param>
        /// <param name="autoCatalogStatusDescription"></param>
        /// <returns></returns>
        public bool VerifyStatusOnAutoCatalogListPage(B2BEnvironment b2BEnvironment, string autoCatalogStatus, string autoCatalogStatusDescription, string regionName, string countryName)
        {
            var autoCatStatus = autoCatalogStatus.Split(',');
            var autoCatStatusDescription = autoCatalogStatusDescription.Split(',');
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var statusDictionary = b2BAutoCatalogListPage.GetStatusDictionary();
            return CheckDictionary(statusDictionary, autoCatStatus, autoCatStatusDescription);
        }

        /// <summary>
        /// Verifies both Default Original Time Of Send and 
        /// Default Delta Time Of Send for a new profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDefaultTimeOfSendForNewProfile(string environment, string customerSet, string accessGroup, string profileNameBase,
            string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(environment, customerSet, accessGroup, profileNameBase);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            if (
                !b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value")
                    .Equals(defaultOriginalTimeOfSend))
            {
                Console.WriteLine("Original Time Of Send does not match the Default value: {0}",
                    defaultOriginalTimeOfSend);
                return false;
            }

            if (!b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value").Equals(defaultDeltaTimeOfSend))
            {
                Console.WriteLine("Delta Time Of Send does not match the Default value: {0}", defaultDeltaTimeOfSend);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies both Default Original Time Of Send and 
        /// Default Delta Time Of Send for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDefaultTimeOfSendForExistingProfile(string environment, string profileName, string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            if (
                !b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value")
                    .Equals(defaultOriginalTimeOfSend))
            {
                Console.WriteLine("Original Time Of Send does not match the Default value: {0}",
                    defaultOriginalTimeOfSend);
                return false;
            }

            if (!b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value").Equals(defaultDeltaTimeOfSend))
            {
                Console.WriteLine("Delta Time Of Send does not match the Default value: {0}", defaultDeltaTimeOfSend);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies the autopopulation of RequestedBy field for a new profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="autoBhcSaveMessage"></param>
        /// <param name="windowsLogin"></param>
        /// <returns></returns>
        public bool VerifyRequestedByAutopopulationForNewProfile(string environment, string customerSet,
            string accessGroup,
            string profileNameBase, string autoBhcSaveMessage, string windowsLogin)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(environment, customerSet, accessGroup, profileNameBase);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(autoBhcSaveMessage))
            {
                Console.WriteLine("The auto catalog setting were not saved successfully.");
                return false;
            }
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.RequestedBy.GetAttribute("value").ToLowerInvariant().Equals(windowsLogin))
            {
                Console.WriteLine("Requested By does not match with the windows login ID: {0}", windowsLogin);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies the autopopulation of RequestedBy field for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <param name="windowsLogin"></param>
        /// <returns></returns>
        public bool VerifyRequestedByAutopopulationForExistingProfile(string environment, string profileName, string scheduleSaveConfirmation, string windowsLogin)
        {
            var centralTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddHours(1);
            GoToBuyerCatalogTab(environment, profileName);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.RequestedBy.GetAttribute("value").ToLowerInvariant().Equals(windowsLogin))
            {
                Console.WriteLine("Requested By does not match with the windows login ID: {0} when verifying the first time", windowsLogin);
                return false;
            }
            if (b2BBuyerCatalogPage.RequestedBy.Enabled)
            {
                Console.WriteLine("Requested By field is not disabled");
                return false;
            }
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.CatalogConfigUpsellDownSell.Click();
            var originalTimeOfSend = centralTime.Hour.ToString();
            if (originalTimeOfSend == "0")
            {
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            }
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(originalTimeOfSend);
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
            {
                Console.WriteLine("The auto catalog setting were not saved successfully.");
                return false;
            }
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.RequestedBy.GetAttribute("value").ToLowerInvariant().Equals(windowsLogin))
            {
                Console.WriteLine("Requested By does not match with the windows login ID: {0} when verifying the second time", windowsLogin);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed for Catalog Operation change for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForCatalogOperationChange(string environment, string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(environment, profileName);
            var configDictionary = GetConfigurationsSelected();

            var originalStartDate = b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value");
            var originalFrequencyDays = b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var originalFrequencyWeeks =
                b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var originalEndDate = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value");
            var originalTimeOfSend = b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value");

            var deltaStartDate = b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value");
            var deltaFrequencyDays = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var deltaFrequencyWeeks =
                b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var deltaEndDate = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value");
            var deltaTimeOfSend = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");

            var internalEmailAdresses = b2BBuyerCatalogPage.InternalEMail.Text;
            var customerEmailAddresses = b2BBuyerCatalogPage.CustomerEmail.Text;

            switch (b2BBuyerCatalogPage.CatalogOperationSelected.GetAttribute("value"))
            {
                case "1":
                    b2BBuyerCatalogPage.CatalogOperationCreatePublish.Click();
                    break;
                case "2":
                    b2BBuyerCatalogPage.CatalogOperationCreate.Click();
                    break;
                default:
                    b2BBuyerCatalogPage.CatalogOperationCreatePublish.Click();
                    break;
            }

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
                return false;
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (!VerifyConfigurationsSelected(configDictionary))
                return false;

            if (
                !VerifyOriginalCatalogSchedulingOptions(originalStartDate, originalFrequencyDays, originalFrequencyWeeks, originalEndDate,
                    originalTimeOfSend))
                return false;

            if (
                !VerifyDeltaCatalogSchedulingOptions(deltaStartDate, deltaFrequencyDays, deltaFrequencyWeeks, deltaEndDate,
                    deltaTimeOfSend))
                return false;

            if (!b2BBuyerCatalogPage.InternalEMail.Text.Equals(internalEmailAdresses))
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Text.Equals(customerEmailAddresses))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed for Configuration type change for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForConfigChange(string environment, string profileName, string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend, string scheduleSaveConfirmation)
        {
            var originalStartDate = DateTime.Now.AddDays(10).ToString(MMDDYYYY);
            var originalFrequencyDays = "2";
            var originalEndDate = DateTime.Now.AddDays(30).ToString(MMDDYYYY);
            var originalTimeOfSend = "8";

            var deltaStartDate = DateTime.Now.AddDays(11).ToString(MMDDYYYY);
            var deltaFrequencyDays = "1";
            var deltaEndDate = DateTime.Now.AddDays(29).ToString(MMDDYYYY);
            var deltaTimeOfSend = "9";

            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            if (
                !VerifyOriginalCatalogSchedulingOptions(DateTime.Now.ToString(MMDDYYYY), "0", "4",
                    DateTime.Now.AddDays(60).ToString(MMDDYYYY), defaultOriginalTimeOfSend))
                return false;
            if (
                !VerifyDeltaCatalogSchedulingOptions(DateTime.Now.AddDays(1).ToString(MMDDYYYY), "0", "1",
                    DateTime.Now.AddDays(59).ToString(MMDDYYYY), defaultDeltaTimeOfSend))
                return false;

            SetOriginalSchedule(originalStartDate, originalFrequencyDays, FrequencyType.Days, originalEndDate,
                originalTimeOfSend);
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            SetDeltaSchedule(deltaStartDate, deltaFrequencyDays, FrequencyType.Days, deltaEndDate, deltaTimeOfSend);
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
                return false;
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (
                !VerifyOriginalCatalogSchedulingOptions(originalStartDate, originalFrequencyDays, "0", originalEndDate,
                    originalTimeOfSend))
                return false;

            if (
                !VerifyDeltaCatalogSchedulingOptions(deltaStartDate, deltaFrequencyDays, "0", deltaEndDate,
                    deltaTimeOfSend))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed when Identities selected are changed - for an existing profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForMultipleIdentity(string environment, string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(environment, profileName);

            if (!b2BBuyerCatalogPage.Identities[0].Selected)
                b2BBuyerCatalogPage.Identities[0].Click();
            b2BBuyerCatalogPage.Identities[1].Click();

            var configDictionary = GetConfigurationsSelected();
            var originalStartDate = b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value");
            var originalFrequencyDays = b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var originalFrequencyWeeks =
                b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var originalEndDate = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value");
            var originalTimeOfSend = b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value");
            var deltaStartDate = b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value");
            var deltaFrequencyDays = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var deltaFrequencyWeeks =
                b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var deltaEndDate = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value");
            var deltaTimeOfSend = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");
            var internalEmailAdresses = b2BBuyerCatalogPage.InternalEMail.Text;
            var customerEmailAddresses = b2BBuyerCatalogPage.CustomerEmail.Text;

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
                return false;

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (!VerifyConfigurationsSelected(configDictionary))
                return false;

            if (
                !VerifyOriginalCatalogSchedulingOptions(originalStartDate, originalFrequencyDays, originalFrequencyWeeks, originalEndDate,
                    originalTimeOfSend))
                return false;

            if (
                !VerifyDeltaCatalogSchedulingOptions(deltaStartDate, deltaFrequencyDays, deltaFrequencyWeeks, deltaEndDate,
                    deltaTimeOfSend))
                return false;

            if (!b2BBuyerCatalogPage.InternalEMail.Text.Equals(internalEmailAdresses))
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Text.Equals(customerEmailAddresses))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed when Internal & Customer Email fields are changed - Existing Profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="internalEmailAddress"></param>
        /// <param name="customerEmailAddress"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponEmailFieldChange(string environment, string profileName, string internalEmailAddress,
            string customerEmailAddress, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(environment, profileName);

            b2BBuyerCatalogPage.InternalEMail.Clear();
            b2BBuyerCatalogPage.CustomerEmail.Clear();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            b2BBuyerCatalogPage.InternalEMail.SendKeys(internalEmailAddress);
            b2BBuyerCatalogPage.CustomerEmail.SendKeys(customerEmailAddress);

            var configDictionary = GetConfigurationsSelected();
            var originalStartDate = b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value");
            var originalFrequencyDays = b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var originalFrequencyWeeks =
                b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var originalEndDate = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value");
            var originalTimeOfSend = b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value");
            var deltaStartDate = b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value");
            var deltaFrequencyDays = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var deltaFrequencyWeeks =
                b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var deltaEndDate = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value");
            var deltaTimeOfSend = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
                return false;
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (!VerifyConfigurationsSelected(configDictionary))
                return false;

            if (
                !VerifyOriginalCatalogSchedulingOptions(originalStartDate, originalFrequencyDays, originalFrequencyWeeks,
                    originalEndDate,
                    originalTimeOfSend))
                return false;

            if (
                !VerifyDeltaCatalogSchedulingOptions(deltaStartDate, deltaFrequencyDays, deltaFrequencyWeeks,
                    deltaEndDate,
                    deltaTimeOfSend))
                return false;

            if (!b2BBuyerCatalogPage.InternalEMail.Text.Equals(internalEmailAddress))
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Text.Equals(customerEmailAddress))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed when Auto BHC is turned off - Existing Profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponTurningAutoBhcOff(string environment, string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(environment, profileName);
            if (b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            else
            {
                Console.WriteLine("Auto BHC is already disabled for this profile. Please use a profile for which Auto BHC is enabled.");
                return false;
            }

            if (b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                Console.WriteLine("Enable BHC Catalog Auto Generation checkbox is checked.");
                return false;
            }

            //Check if all controls are greyed out
            if (!VerifyDisablingOfControls())
                return false;

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed when Auto BHC is turned on - New Profile
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponTurningAutoBhcOnForNewProfile(string environment, string customerSet, string accessGroup, string profileNameBase, string scheduleSaveConfirmation)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(environment, customerSet, accessGroup, profileNameBase);

            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            b2BBuyerCatalogPage.Identities[0].Click();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            SetOriginalSchedule(DateTime.Now.AddDays(10).ToString(MMDDYYYY), "2", FrequencyType.Days,
                DateTime.Now.AddDays(30).ToString(MMDDYYYY), "8");

            b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            SetDeltaSchedule(DateTime.Now.AddDays(11).ToString(MMDDYYYY), "1", FrequencyType.Days,
                DateTime.Now.AddDays(29).ToString(MMDDYYYY), "9");

            b2BBuyerCatalogPage.InternalEMail.SendKeys("test@dell.com");
            b2BBuyerCatalogPage.CustomerEmail.SendKeys("test@gmail.com");

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.ConfirmationLabel.Text.ToLowerInvariant().Equals(scheduleSaveConfirmation))
                return false;

            b2BBuyerCatalogPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (!b2BBuyerCatalogPage.EditScheduleButton.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.CatalogOperationCreate.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.CatalogOperationCreatePublish.Enabled)
                return false;

            if (b2BBuyerCatalogPage.ConfigElements.Any(configElement => configElement.Enabled))
                return false;

            if (b2BBuyerCatalogPage.EnableOriginalCatalog.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalCatalogStartDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalFrequencyDays.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalFrequencyWeeks.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalCatalogEndDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalTimeOfSend.Enabled)
                return false;

            if (b2BBuyerCatalogPage.EnableDeltaCatalog.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaCatalogStartDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaFrequencyDays.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaFrequencyWeeks.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaTimeOfSend.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.InternalEMail.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Enabled)
                return false;

            if (b2BBuyerCatalogPage.RequestedBy.Enabled)
                return false;

            return true;
        }

        /// <summary>
        /// Verifies if error due to selecting a past time 
        /// is locking the catalog scheduling section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="pastTimeErrorStart"></param>
        /// <returns></returns>
        public bool VerifyEditingErrorNotLockingScheduleSection(string environment, string profileName, string pastTimeErrorStart)
        {
            var centralTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            GoToBuyerCatalogTab(environment, profileName);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            var configDictionary = GetConfigurationsSelected();
            var originalStartDate = centralTime.ToString(MMDDYYYY);
            var originalFrequencyDays = b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var originalFrequencyWeeks =
                b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var originalEndDate = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value");
            var originalTimeOfSend = centralTime.Hour.ToString();

            var deltaStartDate = centralTime.AddDays(1).ToString(MMDDYYYY);
            var deltaFrequencyDays = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value");
            var deltaFrequencyWeeks =
                b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value");
            var deltaEndDate = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value");
            var deltaTimeOfSend = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");

            var internalEmailAdresses = b2BBuyerCatalogPage.InternalEMail.Text;
            var customerEmailAddresses = b2BBuyerCatalogPage.CustomerEmail.Text;

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, originalStartDate);
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(originalTimeOfSend);
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, deltaStartDate);

            b2BBuyerCatalogPage.UpdateButton.Click();

            //WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            if (!b2BBuyerCatalogPage.BuyerCatalogError.Text.StartsWith(pastTimeErrorStart))
            {
                return false;
            }

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            if (!VerifyConfigurationsSelected(configDictionary))
                return false;

            if (
                !VerifyOriginalCatalogSchedulingOptions(originalStartDate, originalFrequencyDays, originalFrequencyWeeks,
                    originalEndDate,
                    originalTimeOfSend))
                return false;

            if (
                !VerifyDeltaCatalogSchedulingOptions(deltaStartDate, deltaFrequencyDays, deltaFrequencyWeeks,
                    deltaEndDate,
                    deltaTimeOfSend))
                return false;

            if (!b2BBuyerCatalogPage.InternalEMail.Text.Equals(internalEmailAdresses))
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Text.Equals(customerEmailAddresses))
                return false;

            if(!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

            return VerifyConfigScheduleSectionEnabled();
        }

        /// <summary>
        /// Checks if navigation to Auto Catalog List page is successful
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="pageHeaderText"></param>
        /// <returns></returns>
        public bool VerifyNavigationToAutoCatalogListPage(B2BEnvironment b2BEnvironment, string pageHeaderText)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            return b2BAutoCatalogListPage.PageHeader.Text.Contains(pageHeaderText);
        }

        /// <summary>
        /// Checks if the Clear All link removes the previously set search criteria
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyClearAllLink(B2BEnvironment b2BEnvironment, string profileName, string identities, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SelectTheCustomer(profileName);
            b2BAutoCatalogListPage.SelectTheIdentity();
            b2BAutoCatalogListPage.CatalogName.SendKeys("abc123");
            b2BAutoCatalogListPage.ThreadId.SendKeys("123");
            b2BAutoCatalogListPage.CreationDateStart.SendKeys(DateTime.Now.AddDays(-5).ToString(MMDDYYYY));
            b2BAutoCatalogListPage.CreationDateEnd.SendKeys(DateTime.Now.ToString(MMDDYYYY));
            b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.ScheduledCheckbox.Click();
            b2BAutoCatalogListPage.ClearAllLink.Click();

            if (!webDriver.FindElement(By.XPath("//div[@ng-model='customer']/a/span")).Text.Equals("Select Customer Profile"))
            {
                return false;
            }

            if (!webDriver.FindElement(By.XPath("//div[@ng-model='Identity']/a/span")).Text.Equals("Select Profile Identity"))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(b2BAutoCatalogListPage.CatalogName.GetAttribute("value")))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(b2BAutoCatalogListPage.ThreadId.GetAttribute("value")))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(b2BAutoCatalogListPage.CreationDateStart.GetAttribute("value")))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(b2BAutoCatalogListPage.CreationDateEnd.GetAttribute("value")))
            {
                return false;
            }

            if (b2BAutoCatalogListPage.OriginalCatalogCheckbox.Selected)
                return false;

            if (b2BAutoCatalogListPage.DeltaCatalogCheckbox.Selected)
                return false;

            return !b2BAutoCatalogListPage.ScheduledCheckbox.Selected;
        }

        /// <summary>
        /// Verifies the presence and functionality of Thread Id link in Auto Cataog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool VerifyThreadIdLinkInAutoCatalogListPage(string environment, string region, string country, CatalogStatus status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var firstThreadIdElement = b2BAutoCatalogListPage.CatalogsTable.GetCellElement(1, "Thread Id");
            var threadId = firstThreadIdElement.Text;
            if (!firstThreadIdElement.ElementExists(By.TagName("a")))
            {
                Console.WriteLine("Thread ID column with value **{0}** is not a hyperlink ", threadId);
                return false;
            }

            var threadIdLink = firstThreadIdElement.FindElement(By.TagName("a"));
            threadId = threadIdLink.Text;
            threadIdLink.Click();
            WaitForPageRefresh();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            return webDriver.Url.ToLowerInvariant().Contains("b2blogreportb.aspx?threadid=" + threadId);
        }

        /// <summary>
        /// Verifies the presence and functionality of Thread Id link in Auto Cataog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool VerifyThreadIdLinkInAutoCatalogListPage(string environment, string profilename, string region, string country, CatalogStatus status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var firstThreadIdElement = b2BAutoCatalogListPage.CatalogsTable.GetCellElement(1, "Thread Id");
            var threadId = firstThreadIdElement.Text;
            if (!firstThreadIdElement.ElementExists(By.TagName("a")))
            {
                Console.WriteLine("Thread ID column with value **{0}** is not a hyperlink ", threadId);
                return false;
            }

            var threadIdLink = firstThreadIdElement.FindElement(By.TagName("a"));
            threadId = threadIdLink.Text;
            threadIdLink.Click();
            WaitForPageRefresh();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            return webDriver.Url.ToLowerInvariant().Contains("b2blogreportb.aspx?threadid=" + threadId);
        }

        ///<summary>
        /// Verifies country region and currency fields for original/delta created/published catalogs in Auto Cat List Page
        /// </summary>
        public bool VerifyCountryCodepublishedcreatedInAutoCatListPage(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status,
            string CountryCode, string region, string currencyCode, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
            var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
            var Currencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            if (Status.Equals(status.ToString()) && countryCode.Equals(CountryCode) && regioncode.Equals(region) && Currencycode.Equals(currencyCode))
            {
                return true;
            }
            return false;

        }

        ///<summary>
        /// Verified country region and currency fields for scheduled catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeScheduledInAutoCatalogListPage(B2BEnvironment b2BEnvironment,CatalogType type, CatalogStatus status, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            
            if(status == CatalogStatus.Scheduled)
                b2BAutoCatalogListPage.ScheduledCheckbox.Click();

            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
            var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
            var cUrrencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            if (Status.Equals(status.ToString()) && countryCode.Equals("") && regioncode.Equals(""))// && cUrrencycode.Equals(""))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence and functionality of country region and currency fields in Auto Cataog List Page for failed catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeFailedInAutoCatalogListPage(B2BEnvironment b2BEnvironment,string regionName, string countryName, CatalogType type, CatalogStatus status)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());

            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
            var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
            var currencyCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            if (Status.Equals(status) && countryCode.Equals("") && regioncode.Equals("") && currencyCode.Equals(""))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// Verifies the presence and functionality of Test harness Checkbox for created catalogs in Auto Cataog List Page.
        /// </summary>
        /// <returns></returns>
        public bool VerifyTestHarnessCheckboxInAutoCatalogListPage(string environment,
            string profilename, string countrycode, string regionCode, string currencycode, string status, string regionName, string countryName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh(); WaitForPageRefresh();
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            
            string countryCode = string.Empty;
            string regioncode = string.Empty;
            string cUrrencycode = string.Empty;
            int i = 1;
            if (Status.Equals(status))
            {
                countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
                regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
                cUrrencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            }
            else
            {
                do
                {
                    Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
                    countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
                    regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
                    cUrrencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
                    i++;
                } while (!Status.Equals(status));

            }
            if (countryCode.Equals(countrycode) && regioncode.Equals(regionCode) && cUrrencycode.Equals(currencycode) && Status.Equals(status))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence and functionality of Test Harness Checkbox for failed catalogs in Auto Cataog List Page.
        /// </summary>
        /// <returns></returns>
        public bool VerifyTestHarnessCheckboxFailedInAutoCatalogListPage(string environment, string profilename, string status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            string countryCode = string.Empty;
            string regioncode = string.Empty;
            string cUrrencycode = string.Empty;
            int i = 1;
            if (Status.Equals(status))
            {
                countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
                regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
                cUrrencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            }
            else
            {
                do
                {
                    Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
                    countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
                    regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
                    cUrrencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
                    i++;
                } while (!Status.Equals(status));

            }
            if (countryCode.Equals("") && regioncode.Equals("") && cUrrencycode.Equals("") && Status.Equals(status))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence of Download link for original/delta published/Created catalogs in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyDownloadLinkInAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country, CatalogType type, CatalogStatus status)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            if(type == CatalogType.Delta)
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[24];
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            if (Type.Equals(type.ToString()) && Status.Equals(status.ToString()))
            {
                DownloadLinkElement.Click();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence of Download link for original/delta published/Created catalogs in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyDownloadLinkInAutoCatListPage(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status, string region, string country)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[24];
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            if (Type.Equals(type.ToString()) && Status.Equals(status.ToString()))
            {
                DownloadLinkElement.Click();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence of Download link for Failed catalogs in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyDownloadLinkFailedOrigInAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country, CatalogType type, CatalogStatus status)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            if (type == CatalogType.Delta)
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[20];
            //String download = DownloadLinkElement.FindElement(By.ClassName("ng-hide")).GetAttribute("value");
            if (Type.Equals(type.ToString()) && Status.Equals(status.ToString()))
            {
                if (DownloadLinkElement.Text.Equals(""))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence of Download link for original scheduled catalogs in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyDownloadLinkScheduledInAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country, CatalogStatus status, CatalogType type)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);

            switch(type)
            {
                case CatalogType.Original:
                    b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
                    break;
                default:
                    b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
                    break;
            }

            if (status == CatalogStatus.Scheduled)
                b2BAutoCatalogListPage.ScheduledCheckbox.Click();

            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[21];
            //String download = DownloadLinkElement.FindElement(By.ClassName("ng-hide")).GetAttribute("value");
            if (Type.Equals(type.ToString()) && Status.Equals(status.ToString()))
            {
                if (DownloadLinkElement.Text.Equals(""))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
        /// <summary>
        /// Verifies the presence of Download link for Created through Test harness in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyDownloadLinkInAutoCatListPageCreatedTestHarness(B2BEnvironment b2BEnvironment, string region, string country, string profilename, CatalogType type, string status, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[21];
            if (Type.Equals(type) && Status.Equals(status))
            {
                DownloadLinkElement.Click();
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies the presence of Download link for Failed catalogs thru Test Harness in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkFailedThrutestHarnessInAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country, string profile, CatalogType type, string status)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[20];
            String download = DownloadLinkElement.FindElement(By.ClassName("ng-hide")).GetAttribute("value");
            if (Type.Equals(type) && Status.Equals(status))
            {
                if (download.Equals(""))
                {

                    return true;
                }
                return false;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies Catalog Name in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyCatalogNameInAutoCatListPage(B2BEnvironment b2BEnvironment, string profile, string identity, CatalogType type, CatalogStatus status, string region, string country)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheCustomer(profile);
            b2BAutoCatalogListPage.SelectTheStatus(CatalogStatus.Published.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            string catalogName = uxWorkflow.DownloadCatalog(identity, beforeSchedTime);

            var Type = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Type");
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[20];
            var CatalogNamefromLocator = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Catalog/Inventory Name");
            
            if (CatalogNamefromLocator.Contains(".."))
            {
                CatalogNamefromLocator = CatalogNamefromLocator.Remove(CatalogNamefromLocator.IndexOf('.'), 2);
            }
            if (catalogName.Contains(CatalogNamefromLocator))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies Status Time in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyStatusTimeInAutoCatListPage(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status, string region, string country)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheStatus(status.ToString()); ;
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            //webDriver.WaitForTableRowCount(b2BAutoCatalogListPage.CatalogsTable, 1);
            var statusTimeElement = Convert.ToDateTime(b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Last Status Date"), System.Globalization.CultureInfo.InvariantCulture);
            return statusTimeElement < DateTime.Now;
            //if (statusTimeElement.Equals(statusTime))
            //    return true;
            //return false;
        }

        /// <summary>
        /// Verifies Region and Country Codes in Auto Cat List page
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryandRegionCodesInAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country, string regionCode, string countryCode, string customerName = "", string identity = "",
                                                                 string catalogName = "", string creationStartDate = "", string creationEndDate = "",
                                                                 string status = "", string configurationType = "", string catalogType = "")
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            if (!string.IsNullOrEmpty(catalogName))
            {
                b2BAutoCatalogListPage.SelectTheCustomer(customerName);
                b2BAutoCatalogListPage.SelectTheIdentity(identity);
                b2BAutoCatalogListPage.SearchRecordsLink.Click();
                b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
                catalogName = uxWorkflow.DownloadCatalog(identity, beforeSchedTime);
                catalogName = catalogName.Substring(catalogName.LastIndexOf("\\") + 1);
                catalogName = catalogName.Remove(catalogName.IndexOf('.'), 4);
            }
            if (customerName != "")
            { b2BAutoCatalogListPage.SelectTheCustomer(customerName); }

            if (identity != "")
            {
                b2BAutoCatalogListPage.SelectTheCustomer(customerName);
                b2BAutoCatalogListPage.SelectTheIdentity(identity);
            }

            if (catalogName != "")
            { b2BAutoCatalogListPage.CatalogName.SendKeys(catalogName); }

            if (creationStartDate != "")
            {
                b2BAutoCatalogListPage.SelectTheStatus(status);
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            }

            if (creationEndDate != "")
            {
                b2BAutoCatalogListPage.SelectTheStatus(status);
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            }

            if (status != "")
            { b2BAutoCatalogListPage.SelectTheStatus(status); }

            if (configurationType != "")
            {
                b2BAutoCatalogListPage.SelectTheStatus(status);
                b2BAutoCatalogListPage.ConfigTypeSTDCheckbox.Click();
            }

            if (catalogType != "")
            {
                b2BAutoCatalogListPage.SelectTheStatus(status);
                b2BAutoCatalogListPage.CatalogTypeOriginalCheckbox.Click();
            }
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var cOuntryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
            var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
            if (cOuntryCode.Equals(countryCode) && regioncode.Equals(regionCode))
                return true;
            return false;
        }

        ///// <summary>
        ///// Verifies Region and Country Codes in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyCountryandRegionFieldsInAutoCatListPage(B2BEnvironment b2BEnvironment)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            if (b2BAutoCatalogListPage.RegionDropDown.Enabled && b2BAutoCatalogListPage.CountryDropDown.Enabled)
                return true;
            return false;
        }

        ///// <summary>
        ///// Verifies Region and Country Codes in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyCustomerListInAutoCatListPage(B2BEnvironment b2BEnvironment, string region1, string customerName1, string region2, string customerName2, string country1 = "", string country2 = "")
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            bool tempCustomerName1 = false;
            bool tempCustomerName2 = false;

            b2BAutoCatalogListPage.SelectTheRegion(region1);
            if (country1 != "")
            { b2BAutoCatalogListPage.SelectTheCountry(country1); }
            if (b2BAutoCatalogListPage.VerifyCustomerExists(customerName1))
                tempCustomerName1 = true;
            b2BAutoCatalogListPage.ClickClearAll();

            b2BAutoCatalogListPage.SelectTheRegion(region2);
            if (country2 != "")
            { b2BAutoCatalogListPage.SelectTheCountry(country2); }
            if (!(b2BAutoCatalogListPage.VerifyCustomerExists(customerName1)))
                tempCustomerName2 = true;

            if (tempCustomerName1 == true && tempCustomerName2 == true)
                return true;

            return false;
        }

        ///// <summary>
        ///// Verifies Status Time in Auto Cat List page for Test Harness
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyStatusTimeforTestHarnessInAutoCatListPage(string environment, string profile, string statusTime, string regionName, string countryName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            //webDriver.WaitForElement(b2BAutoCatalogListPage.NextButton);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            webDriver.WaitForTableRowCount(b2BAutoCatalogListPage.CatalogsTable, 1);
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            if (Status.Equals(statusTime))
                return true;
            return false;
        }

        ///// <summary>
        ///// Verifies Profile Name for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyProfileNameAutoCatPage(B2BEnvironment b2BEnvironment, CatalogStatus status, string profileName, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SelectTheCustomer(profileName);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var profileNamefromLocator = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Profile Name");
            if (profileNamefromLocator.Equals(profileName))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies Profile Name for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyProfileNameAutoCatPage(B2BEnvironment b2BEnvironment, CatalogStatus status, CatalogType type, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            var profileNamefromLocator = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Profile Name");

            return !string.IsNullOrEmpty(profileNamefromLocator);
        }

        ///// <summary>
        ///// Verifies Profile Name for Test Harness for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyProfileNameforTestHarnessAutoCatPage(string environment, string profile, string regionName, string countryName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            WaitForPageRefresh();
            var profileNamefromLocator = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Profile Name"); ;
            if (profileNamefromLocator.Equals(String.Empty))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies RemoveItemsLt Checkbox defaultValue for new profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyRemoveItemsLtCheckboxdefaultValue()
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected)
            {
                return false;
            }
            b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Click();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return true;
        }

        ///// <summary>
        ///// Verifies RemoveItems Lt Checkbox default Value for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyRemoveItemsLtCheckboxdefaultValueExistingProfile(string environment, string profileName)
        {

            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected)
            {
                b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Click();

                b2BBuyerCatalogPage.UpdateButton.Click();
                return true;
            }
            else if (!b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected)
            {
                b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Click();

                b2BBuyerCatalogPage.UpdateButton.Click();
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// Verifies the Select Customer drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool VerifySelectCustomerFieldOnAutoCatalogListPage(B2BEnvironment b2BEnvironment, string profileName, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            WaitForPageRefresh();
            return b2BAutoCatalogListPage.SelectCustomer.Select().Options.Count() > 1 &&
                   b2BAutoCatalogListPage.SelectCustomer.Select().Options.Any(o => o.GetAttribute("text").Equals(profileName));
        }

        /// <summary>
        /// Verifies the Identity drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyIdentityFieldOnAutoCatalogListPage(B2BEnvironment b2BEnvironment, string profileName, string identities, string regionName, string countryName)
        {
            var identityList = identities.Split(',');
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SelectOption(b2BAutoCatalogListPage.SelectCustomerNameSpan, profileName);
            WaitForPageRefresh();
            if (identityList.Count() ==
                b2BAutoCatalogListPage.SelectIdentity.Select()
                    .Options.Count(o => !string.IsNullOrEmpty(o.GetAttribute("text"))))
            {
                return
                    b2BAutoCatalogListPage.SelectIdentity.Select()
                        .Options.Where(o => !string.IsNullOrEmpty(o.GetAttribute("text")))
                        .All(option => identityList.Contains(option.GetAttribute("text")));
            }

            Console.WriteLine(
                "No. of identities passed for profile **{0}** does not match with the no. of identities in Identities drop down", profileName);
            return false;
        }


        /// <summary>
        /// Verifies the Select Region drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public bool VerifyRegionFieldOnAutoCatalogListPage(string environment, string regionName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            return b2BAutoCatalogListPage.SelectRegion.Select().Options.Count() > 1 &&
                   b2BAutoCatalogListPage.SelectRegion.Select().Options.Any(o => o.GetAttribute("text").Equals(regionName));
        }

        /// <summary>
        /// Verifies the Identity drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="regionName"></param>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public bool VerifyCountryFieldOnAutoCatalogListPage(string environment, string regionName, string countryName)
        {
            var identityList = countryName.Split(',');
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            WaitForPageRefresh();
            if (identityList.Count() ==
                b2BAutoCatalogListPage.SelectCountry.Select()
                    .Options.Count(o => !string.IsNullOrEmpty(o.GetAttribute("text"))))
            {
                return
                    b2BAutoCatalogListPage.SelectCountry.Select()
                        .Options.Where(o => !string.IsNullOrEmpty(o.GetAttribute("text")))
                        .All(option => identityList.Contains(option.GetAttribute("text")));
            }

            Console.WriteLine(
                "No. of countryNames passed for Region **{0}** does not match with the no. of countryNames in countryNames drop down", regionName);
            return false;
        }


        /// <summary>
        /// Verifies the search results on Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public bool VerifySearchResultsOnAutoCatalogListPage(B2BEnvironment b2BEnvironment, string customerName, string profileName, string identity, string region, string country)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheCustomer(customerName);
            b2BAutoCatalogListPage.SelectTheIdentity(identity);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));

            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r =>
                        r.FindElements(By.TagName("td"))[1].Text.ToLowerInvariant()
                            .Equals(profileName.ToLowerInvariant())))
                return false;

            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r =>
                        r.FindElements(By.TagName("td"))[2].Text.ToLowerInvariant().Equals("delta") ||
                        r.FindElements(By.TagName("td"))[2].Text.ToLowerInvariant().Equals("original")))
                return false;

            if (
                 !b2BAutoCatalogListPage.CatalogListTableRows.All(
                     r =>
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[0].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[1].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[2].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[3].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[4].FindElements(By.TagName("td"))[0].Text)))
                return false;

            return true;
        }

        /// <summary>
        /// To verify all the fields are selected
        /// </summary>
        public void AuditHistoryProperties()
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            b2BBuyerCatalogPage.UpdateButton.Click();

        }

        /// <summary>
        /// Verify user name in the latest entry of Audit History
        /// </summary>
        /// <param name="valueToBeChecked"></param>
        /// <returns></returns>
        public bool VerifyAuditHistoryUserName(string valueToBeChecked)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AuditHistoryLink.Click();
            WaitForPageRefresh();
            var valuetocheck = b2BBuyerCatalogPage.AuditHistoryRows[0].FindElements(By.TagName("td"))[0].Text;
            return valuetocheck.Contains(valueToBeChecked);
        }

        /// <summary>
        /// To verify the logging of change in Delta Catalog Frequency under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="deltaFreqAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool VerifyAuditHistoryDeltaCatalogFrequency(string environment, string profile, string deltaFreqAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue("5");
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectByValue("4");

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value") + " Day(s)";
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue("5");
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectByValue("3");

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            newValue = b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value") + " Day(s)";
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, deltaFreqAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Original start date under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="originalStartDateAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool VerifyAuditHistoryOriginalStartDate(string environment, string profile, string originalStartDateAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)


                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();


            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            oldValue = b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");
            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)

                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            newValue = b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(3).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, originalStartDateAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in Delta start date under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="deltaStartDateAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryDeltaStartDate(string environment, string profile, string deltaStartDateAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            oldValue = b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(3).ToString(MMDDYYYY));

            newValue = b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, deltaStartDateAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in Delta End date under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="deltaEndDateAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryDeltaEndDate(string environment, string profile, string deltaEndDateAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, DateTime.Now.AddDays(35).ToString(MMDDYYYY));

            oldValue = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, DateTime.Now.AddDays(40).ToString(MMDDYYYY));

            newValue = b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value").ToString();


            b2BBuyerCatalogPage.UpdateButton.Click();

            return VerifyAuditHistoryRow(oldValue, newValue, deltaEndDateAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Delta catalog Time of send under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="deltaTimeOfSendAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryDeltaCatalogTimeofSend(string environment, string profile, string deltaTimeOfSendAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("7");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("8");

            var oldValue1 = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");
            oldValue = "0" + oldValue1 + ":00:00";
            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();


            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            var newValue1 = b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value");
            newValue = "0" + newValue1 + ":00:00";
            b2BBuyerCatalogPage.UpdateButton.Click();

            return VerifyAuditHistoryRow(oldValue, newValue, deltaTimeOfSendAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in original catalog end date under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="originalEndDateAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryOriginalCatalogEndDate(string environment, string profile, string originalEndDateAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogEndDate, DateTime.Now.AddDays(55).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");


            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, DateTime.Now.AddDays(54).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            oldValue = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogEndDate, DateTime.Now.AddDays(59).ToString(MMDDYYYY));
            newValue = b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value").ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, DateTime.Now.AddDays(58).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            b2BBuyerCatalogPage.UpdateButton.Click();

            return VerifyAuditHistoryRow(oldValue, newValue, originalEndDateAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in Original catalog frequency under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="originalFrequencyAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryOriginalCatalogFrequency(string environment, string profile, string originalFrequencyAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();

            b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue("4");
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");


            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectByValue("3");
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            var oldValue1 = b2BBuyerCatalogPage.OriginalFrequencyDays.GetAttribute("value").ToString();
            oldValue = oldValue1 + " Day(s)";
            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue("5");



            var newValue1 = b2BBuyerCatalogPage.OriginalFrequencyDays.GetAttribute("value").ToString();
            newValue = newValue1 + " Day(s)";
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectByValue("4");
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("9");

            b2BBuyerCatalogPage.UpdateButton.Click();

            return VerifyAuditHistoryRow(oldValue, newValue, originalFrequencyAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in original Catalog Time of send under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="originalTimeOfSendAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryOriginalCatalogTimeofSend(string environment, string profile, string originalTimeOfSendAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;

            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();


            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");

            var oldValue1 = b2BBuyerCatalogPage.OriginalTimeOfSend.GetAttribute("value").ToString();
            oldValue = "0" + oldValue1 + ":00:00";
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");

            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.EditScheduleButton.Click();


            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();


            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("7");

            var newValue1 = b2BBuyerCatalogPage.OriginalTimeOfSend.GetAttribute("value").ToString();

            newValue = "0" + newValue1 + ":00:00";
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();


            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("8");

            b2BBuyerCatalogPage.UpdateButton.Click();

            return VerifyAuditHistoryRow(oldValue, newValue, originalTimeOfSendAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in internal email under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="internalEmail1"></param>
        /// <param name="internalEmail2"></param>
        /// <param name="internalEmailAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryInternalEmail(string environment, string profile, string internalEmail1, string internalEmail2, string internalEmailAuditHistoryProperty)
        {
            var oldValue = string.Empty;
            var newValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.InternalEMail.GetAttribute("value");
            b2BBuyerCatalogPage.InternalEMail.Clear();
            if (oldValue.Equals(internalEmail1))
                newValue = internalEmail1;
            else
                newValue = internalEmail2;

            b2BBuyerCatalogPage.InternalEMail.SendKeys(newValue);
            b2BBuyerCatalogPage.UpdateButton.Click();


            return VerifyAuditHistoryRow(oldValue, newValue, internalEmailAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in customer email under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="customerEmail1"></param>
        /// <param name="customerEmail2"></param>
        /// <param name="customerEmailAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryCustomerEmail(string environment, string profile, string customerEmail1, string customerEmail2, string customerEmailAuditHistoryProperty)
        {
            var oldValue = string.Empty;
            var newValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            oldValue = b2BBuyerCatalogPage.CustomerEmail.GetAttribute("value");
            b2BBuyerCatalogPage.CustomerEmail.Clear();
            if (oldValue.Equals(customerEmail1))
                newValue = customerEmail2;
            else
                newValue = customerEmail1;

            b2BBuyerCatalogPage.CustomerEmail.SendKeys(newValue);
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, customerEmailAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in EnableBHCAutoGen under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="enableAutoBhcAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryEnableAutoCatGen(string environment, string profile, string enableAutoBhcAuditHistoryProperty)
        {
            var oldValue = string.Empty;
            var newValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            oldValue = b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected.ToString();

            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            newValue = b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected.ToString();
            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, enableAutoBhcAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in AutoBHC under audit history section
        /// </summary>
        /// <param name="autoBhcAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryAutoBhc(string autoBhcAuditHistoryProperty)
        {
            var oldValue = string.Empty;
            var newValue = string.Empty;
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            oldValue = b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected.ToString();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            newValue = b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected.ToString();
            b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            //b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
            //    DateTime.Now.AddDays(3).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, autoBhcAuditHistoryProperty);
        }

        /// <summary>
        /// To verify the logging of change in Catlog operation under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="catalogOperationAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryCatalogOperation(string environment, string profile, string catalogOperationAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            if (b2BBuyerCatalogPage.CatalogOperationCreate.Selected)
            {
                oldValue = "Create";
                b2BBuyerCatalogPage.CatalogOperationCreatePublish.Click();
                newValue = "Create & Publish";
            }
            else
            {
                oldValue = "Create & Publish";
                b2BBuyerCatalogPage.CatalogOperationCreate.Click();
                newValue = "Create";
            }

            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, catalogOperationAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Enable Delta Catalog under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="enableDeltaCatalogAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryEnableDeltaCatalog(string environment, string profile, string enableDeltaCatalogAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue("8");
            oldValue = b2BBuyerCatalogPage.EnableDeltaCatalog.Selected.ToString();

            b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            newValue = b2BBuyerCatalogPage.EnableDeltaCatalog.Selected.ToString();
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue("9");
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, enableDeltaCatalogAuditHistoryProperty);

        }

        ///// <summary>
        ///// Retrieve Delta Published Auto BHC Config Quote ID thru Part Viewer. Verify all required info in the table
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyRetrieveCatalogConfigAquoteId(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, string Header, string SubHeader, CatalogType type, CatalogStatus status, string profile, string identity)
        {
            Dictionary<int, string> dict = GetPartViewerInformation(b2BEnvironment, catalogItemType, region, country, type, status, profile, identity);

            if (dict.Count < 2)
            {
                Console.WriteLine("Catalog is empty");
                return false;
            }
            var Headervalue = dict[1]; //dict.Where(p => p.Key == 1).FirstOrDefault().Value;
            var SubRowvalue1 = dict[2]; //dict.Where(p => p.Key == 2).FirstOrDefault().Value;

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            //b2BChannelUx.OpenAutoCatalogAndInventoryListPage( b2BEnvironment);

            b2BChannelUx.OpenAutoPartViewerPage(b2BEnvironment);

            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            
            string[] HeaderRowStringValue = Header.Split(',');
            string[] SubHeaderStringValue = SubHeader.Split(',');
            string[] HeaderStringvalue = Headervalue.ToString().Split(',');
            string[] SubRow1StringValue = SubRowvalue1.ToString().Split(new string[] { " ," }, StringSplitOptions.None);
            
            int quoteidlength = SubRow1StringValue[4].Length;
            string quoteid = SubRow1StringValue[4].Substring(4, quoteidlength-4);
            
            b2BAutoCatalogListPage.PartViewerQuoteIdsLink.SendKeys(quoteid);
            b2BAutoCatalogListPage.PartViewerSearchButton.Click();
            b2BAutoCatalogListPage.PartViewerPlusButton.WaitForElementVisible(TimeSpan.FromSeconds(30));
            b2BAutoCatalogListPage.PartViewerPlusButton.Click();

            string TableXpath_First = "//*[@id='quoteTable']";
            string Table1FirstRow_End = "/tbody[1]/tr[1]/td";
            string Table1SubHeadingEnd = "/tbody[1]/tr[2]/td[2]/table/thead/tr/th";
            string Table1SubRow_End = "/tbody[1]/tr[2]/td[2]/table/tbody/tr/td";
            for (int j = 0; j < SubHeaderStringValue.Length; j++)
            {
                var SubHeaderElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubHeadingEnd))[j];
                var subHeaderTable1fromLocator = SubHeaderElement.Text;
                var SubRowElemt = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubRow_End))[j];
                var subRowTable1fromLocator = SubRowElemt.Text;
                var subHeaderTestdata = SubHeaderStringValue[j];
                //var subRow1Testdata = SubRow1StringValue[j].Replace('_', ',');
                var subRow1Testdata = SubRow1StringValue[j];
                
                if (subHeaderTable1fromLocator.Equals(subHeaderTestdata) && subRowTable1fromLocator.Equals(subRow1Testdata) )
                {
                    subHeaderRows++;
                }
            }
            //Header
            for (int i = 0; i < HeaderRowStringValue.Length; i++)
            {
                var HeaderElement = b2BAutoCatalogListPage.PartViewerHeader.FirstOrDefault().FindElements(By.TagName("th"))[i];
                var HeaderTextfromLocator = HeaderElement.Text;
                var HeadTestdata = HeaderRowStringValue[i];
                if (HeaderTextfromLocator.Equals(HeadTestdata))
                {
                    Headercount++;
                }
            }
            //HeaderRows
            for (int z = 1; z < HeaderStringvalue.Length; z++)
            {
                var HeaderRowElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1FirstRow_End))[z];
                var HeaderRowTextfromLocator = HeaderRowElement.Text;
                var HeaderRowTestData = HeaderStringvalue[z];
                if (HeaderRowTextfromLocator.ToUpper().Contains(HeaderRowTestData.ToUpper()))
                {
                    HeaderRowsCount++;
                }
            }
            // Sub Header and Sub Rows Table1
            if (Headercount.Equals(8) && HeaderRowsCount.Equals(8) && subHeaderRows.Equals(8))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if all the fields in Auto BHC section are disabled
        /// </summary>
        /// <returns></returns>
        private bool VerifyDisablingOfControls()
        {
            if (b2BBuyerCatalogPage.Identities.Any(i => i.Enabled))
                return false;

            if (b2BBuyerCatalogPage.CatalogOperationCreate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.CatalogOperationCreatePublish.Enabled)
                return false;

            if (b2BBuyerCatalogPage.EditScheduleButton.Enabled)
                return false;

            if (b2BBuyerCatalogPage.ConfigElements.Any(c => c.Enabled))
                return false;

            if (b2BBuyerCatalogPage.EnableOriginalCatalog.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalCatalogStartDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalFrequencyDays.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalFrequencyWeeks.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalCatalogEndDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.OriginalTimeOfSend.Enabled)
                return false;

            if (b2BBuyerCatalogPage.EnableDeltaCatalog.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaCatalogStartDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaFrequencyDays.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaFrequencyWeeks.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled)
                return false;

            if (b2BBuyerCatalogPage.DeltaTimeOfSend.Enabled)
                return false;

            if (b2BBuyerCatalogPage.InternalEMail.Enabled)
                return false;

            if (b2BBuyerCatalogPage.CustomerEmail.Enabled)
                return false;

            if (b2BBuyerCatalogPage.RequestedBy.Enabled)
                return false;

            return true;
        }

        /// <summary>
        /// Verifies if all the fields in config & schedule sections are enabled
        /// </summary>
        /// <returns></returns>
        private bool VerifyConfigScheduleSectionEnabled()
        {
            if (!b2BBuyerCatalogPage.ConfigElements.All(c => c.Enabled))
                return false;

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.OriginalCatalogStartDate.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.OriginalFrequencyDays.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.OriginalFrequencyWeeks.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.OriginalCatalogEndDate.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.OriginalTimeOfSend.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.DeltaCatalogStartDate.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.DeltaFrequencyDays.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.DeltaFrequencyWeeks.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled)
                return false;

            if (!b2BBuyerCatalogPage.DeltaTimeOfSend.Enabled)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the dictionary has status passed as key & description as its value
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="status"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private bool CheckDictionary(IDictionary<string, string> dictionary, IReadOnlyList<string> status, IReadOnlyList<string> description)
        {
            for (var i = 0; i < status.Count(); i++)
            {
                if (!dictionary.ContainsKey(status[i]))
                {
                    Console.WriteLine("Status **{0}** not found", status[i]);
                    return false;
                }
                if (!dictionary[status[i]].Equals(description[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compares the Delta Catalog Schedule values against the values passed
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="frequencyDays"></param>
        /// <param name="frequencyWeeks"></param>
        /// <param name="endDate"></param>
        /// <param name="timeOfSend"></param>
        /// <returns></returns>
        private bool VerifyDeltaCatalogSchedulingOptions(string startDate, string frequencyDays, string frequencyWeeks,
            string endDate, string timeOfSend)
        {
            if (!Convert.ToDateTime(b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value")).Equals(Convert.ToDateTime(startDate)))
            {
                Console.WriteLine("Delta Catalog Start Date does not match: {0}", startDate);
                return false;
            }

            if (!b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value").Equals(frequencyDays))
            {
                Console.WriteLine("Delta Catalog Frequency in Days does not match: {0}", frequencyDays);
                return false;
            }

            if (!b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value").Equals(frequencyWeeks))
            {
                Console.WriteLine("Delta Catalog Frequency in Weeks does not match: {0}", frequencyWeeks);
                return false;
            }

            if (!Convert.ToDateTime(b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value")).Equals(Convert.ToDateTime(endDate)))
            {
                Console.WriteLine("Delta Catalog End Date does not match: {0}", endDate);
                return false;
            }

            if (!b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value").Equals(timeOfSend))
            {
                Console.WriteLine("Delta Catalog Time of Send does not match: {0}", timeOfSend);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the Original Catalog Schedule values against the values passed
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="frequencyDays"></param>
        /// <param name="frequencyWeeks"></param>
        /// <param name="endDate"></param>
        /// <param name="timeOfSend"></param>
        /// <returns></returns>
        private bool VerifyOriginalCatalogSchedulingOptions(string startDate, string frequencyDays,
            string frequencyWeeks, string endDate, string timeOfSend)
        {
            if (
                !Convert.ToDateTime(b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value"))
                    .Equals(Convert.ToDateTime(startDate)))
            {
                Console.WriteLine("Original Catalog Start Date does not match: {0}", startDate);
                return false;
            }

            if (!b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value").Equals(frequencyDays))
            {
                Console.WriteLine("Original Catalog Frequency in Days does not match: {0}", frequencyDays);
                return false;
            }

            if (!b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value").Equals(frequencyWeeks))
            {
                Console.WriteLine("Original Catalog Frequency in Weeks does not match: {0}", frequencyWeeks);
                return false;
            }

            if (
                !Convert.ToDateTime(b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value"))
                    .Equals(Convert.ToDateTime(endDate)))
            {
                Console.WriteLine("Original Catalog End Date does not match: {0}", endDate);
                return false;
            }

            if (!b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value").Equals(timeOfSend))
            {
                Console.WriteLine("Original Catalog Time Of Send does not match: {0}", timeOfSend);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the Delta Schedule with the parameters passed
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="frequency"></param>
        /// <param name="frequencyType"></param>
        /// <param name="endDate"></param>
        /// <param name="timeOfSend"></param>
        private void SetDeltaSchedule(string startDate, string frequency, FrequencyType frequencyType, string endDate,
            string timeOfSend)
        {
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, startDate);

            if (frequencyType.Equals(FrequencyType.Days))
            {
                b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectByValue(frequency);
            }
            else
            {
                b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectByValue(frequency);
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, endDate);
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue(timeOfSend);
        }

        /// <summary>
        /// Sets the Original Schedule with the parameters passed
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="frequency"></param>
        /// <param name="frequencyType"></param>
        /// <param name="endDate"></param>
        /// <param name="timeOfSend"></param>
        private void SetOriginalSchedule(string startDate, string frequency, FrequencyType frequencyType, string endDate, string timeOfSend)
        {
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, startDate);

            if (frequencyType.Equals(FrequencyType.Days))
            {
                b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue(frequency);
            }
            else
            {
                b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectByValue(frequency);
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogEndDate, endDate);
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(timeOfSend);
        }

        /// <summary>
        /// Returns a dictionary of configs with Key = id and Value = selected
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, bool> GetConfigurationsSelected()
        {
            return b2BBuyerCatalogPage.ConfigElements.ToDictionary(configElement => configElement.GetAttribute("id"),
                configElement => configElement.Selected);
        }

        /// <summary>
        /// Verifies the configs selected by checking against the Dictionary of (id, selected) passed
        /// </summary>
        /// <param name="configDictionary"></param>
        /// <returns></returns>
        private bool VerifyConfigurationsSelected(IReadOnlyDictionary<string, bool> configDictionary)
        {
            for (var i = 0; i < configDictionary.Count(); i++)
            {
                if (configDictionary.ContainsKey(b2BBuyerCatalogPage.ConfigElements[i].GetAttribute("id")))
                {
                    if (!configDictionary[b2BBuyerCatalogPage.ConfigElements[i].GetAttribute("id")] == b2BBuyerCatalogPage.ConfigElements[i].Selected)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Uploads the Packaging Data File in Packaging Data Upload Page
        /// and validates the message received after upload.
        /// </summary>
        /// <param name="fileToBeUploaded"></param>
        /// <param name="uploadMessage"></param>
        /// <returns></returns>
        private bool UploadAndCheckMessageAndValidate(string fileToBeUploaded, string uploadMessage)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
            Thread.Sleep(5000);
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
            WaitForPageRefresh();
            Console.WriteLine("Message received on upload: **{0}**",
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text);
            return b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(uploadMessage);
        }

        /// <summary>
        /// Uploads the Packaging Data File in Packaging Data Upload Page
        /// and validates the message received after upload.
        /// </summary>
        /// <param name="fileToBeUploaded"></param>
        /// <param name="uploadMessageStartsWith"></param>
        /// <param name="uploadMessageEndsWith"></param>
        /// <returns></returns>
        private bool UploadAndCheckMessageAndValidate(string fileToBeUploaded, string uploadMessageStartsWith,
            string uploadMessageEndsWith)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
            Thread.Sleep(5000);
            try
            {
                var uploadAlert = webDriver.SwitchTo().Alert();
                uploadAlert.Accept();
            }
            catch { }
            WaitForPageRefresh();
            Console.WriteLine("Message received on upload: **{0}**",
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text);
            return
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().StartsWith(uploadMessageStartsWith) &&
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().EndsWith(uploadMessageEndsWith);
        }

        /// <summary>
        /// Uploads the Packaging Data file in Packaging Data Upload Page
        /// </summary>
        /// <param name="fileToBeUploaded"></param>
        private void UploadPackagingData(string fileToBeUploaded)
        {
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
            try
            {
                var uploadAlert = webDriver.SwitchTo().Alert();
                uploadAlert.Accept();
            }
            catch { }
            WaitForPageRefresh();
            Console.WriteLine("Message received on upload: **{0}**",
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text);
        }

        /// <summary>
        /// Validates an Audit History entry base on provided property, old value & new value
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="auditHistoryProperty"></param>
        /// <returns></returns>
        private bool VerifyAuditHistoryRow(string oldValue, string newValue, string auditHistoryProperty)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AuditHistoryLink.Click();
            WaitForPageRefresh();
            var auditHistoryRow =
                b2BBuyerCatalogPage.AuditHistoryRows.FirstOrDefault(
                    r => r.FindElements(By.TagName("td"))[0].Text.Equals(auditHistoryProperty));
            var oldv = auditHistoryRow.FindElements(By.TagName("td"))[1].Text;
            Console.WriteLine(oldv);
            var newv = auditHistoryRow.FindElements(By.TagName("td"))[2].Text;
            Console.Write(newv);

            if (!oldValue.Equals(oldv))
                return false;

            return newValue.Equals(newv);
        }

        /// <summary>
        /// Verifies Sys checkbox is present and non editable in Auto Cat List page. 
        /// </summary>
        public bool VerifySysCheckboxinAutoCatListPage(B2BEnvironment b2BEnvironment, string region, string country)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            var firstSysElements = webDriver.FindElements(By.XPath("//input[@ng-model='Catalog.IsSystem' and @type='checkbox']"));
            foreach (var sysEelement in firstSysElements)
            {
                if (sysEelement.Enabled)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies System Catalog checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifySystemCataloginAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                {
                    return false;
                }
                return true;
            }
            else
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Verifies System Catalog Sub options in Auto BHC section when System Catalog is set as True. 
        /// </summary>
        public bool VerifySystemCatalogSubOptionsinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            }
            else
            {
                b2BBuyerCatalogPage.EditScheduleButton.Click();
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (!(b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Selected)
                    )
                {
                    return false;
                }
                return true;
            }
            else
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (!(b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                   b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Selected &&
                   b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Selected &&
                   b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Selected)
                   )
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Verifies System Catalog and STD Sub options in Auto BHC section when System Catalog and STD are set as True. 
        /// </summary>
        public bool VerifySystemAndSTDCatalogSubOptionsinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            }
            else
            {
                b2BBuyerCatalogPage.EditScheduleButton.Click();
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Click();

                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Click();
                }
                else
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Click();
                }

                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (!(b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigStandard.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Selected)
                    )
                {
                    return false;
                }
                return true;
            }
            else
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Click();

                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Click();
                }
                else
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Click();
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Click();
                }

                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                if (!(b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigStandard.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Selected &&
                    b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Selected)
                    )
                {
                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// Verifies SPL Flag checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifySplFlaginAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(2).ToString(MMDDYYYY));
                if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    return false;
                }
                return true;
            }
            else
            {
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(2).ToString(MMDDYYYY));
                if (b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// To verify the logging of change in Remove Items with LT under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="removeItemswithLtAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryRemoveItemswithLt(string environment, string profile,
            string removeItemswithLtAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected.ToString();
            b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Click();
            newValue = b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, removeItemswithLtAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Cross ref Std under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="crossrefstdAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryCrossRefStd(string environment, string profile, string crossrefstdAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected.ToString();
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                if(!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            }
            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, crossrefstdAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Cross Ref SNP under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="crossrefsnpAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryCrossRefSnp(string environment, string profile, string crossrefsnpAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected.ToString();
            b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, crossrefsnpAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Remove Items with LT under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="crossrefsysAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryCrossRefSys(string environment, string profile, string crossrefsysAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            oldValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected.ToString();
            b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, crossrefsysAuditHistoryProperty);

        }

        /// <summary>
        /// To verify the logging of change in Enable SPL/distributor under audit history section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="enableSplAuditHistoryProperty"></param>
        /// <returns></returns>
        public bool AuditHistoryEnableSpl(string environment, string profile, string enableSplAuditHistoryProperty)
        {
            var newValue = string.Empty;
            var oldValue = string.Empty;
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            oldValue = b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected.ToString();
            b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
            newValue = b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            return VerifyAuditHistoryRow(oldValue, newValue, enableSplAuditHistoryProperty);

        }

        /// <summary>
        /// Verifies Cross reference checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifyCrossRefCheckboxinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceUpdate.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceUpdate.Selected)
                {
                    return false;
                }
                return true;
            }

            b2BBuyerCatalogPage.BcpchkCrossRefernceUpdate.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceUpdate.Selected)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Verifies Cross reference Std config checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifyCrossRefstdConfigsCheckboxinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                {
                    return false;
                }
                return true;
            }

            b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Verifies Cross reference SNP checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifyCrossRefSnpCheckboxinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    return false;
                }
                return true;
            }

            b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Verifies Cross reference SYS checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifyCrossRefSysCheckboxinAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                {
                    return false;
                }
                return true;
            }

            b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Turn ON Auto CRT update by Config type = STD->Verify configuration type would be defaulted to STD automatically. 
        /// </summary>
        public bool VerifyAutoCrtStddefaulttoConfigtypeStd(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                {
                    if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                        b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    if (b2BBuyerCatalogPage.CatalogConfigStandard.Selected &&
                        b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                    b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                if (!(b2BBuyerCatalogPage.CatalogConfigStandard.Selected &&
                    b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Turn ON Auto CRT update by Config type = Sys->Verify configuration type would be defaulted to Sys automatically. 
        /// </summary>
        public bool VerifyAutoCrtSysdefaulttoConfigtypeSys(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                {
                    if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                        b2BBuyerCatalogPage.CatalogConfigStandard.Click();

                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
                    if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                        b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
                if (!(b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Req 877481- Existing Profile - PROD - Turn ON/OFF Auto CRT update by Config type = SNP->Verify configuration type would be defaulted to SnP automatically or not
        /// </summary>
        public bool VerifyAutoCrtSnpdefaulttoConfigtypeSnp(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigSnP.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                        b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
                    if (b2BBuyerCatalogPage.CatalogConfigSnP.Selected &&
                        b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                    b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                DateTime.Now.AddDays(3).ToString(MMDDYYYY));
                if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected &&
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
        public void VerifyRoundOffValuesPackageUploadForAllFields(B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            WaitForPageRefresh();
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message).Should().BeTrue();
        }

        public bool VerifyPackageUploadForAllFieldsPrev(B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            return b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Contains(message);
        }
        public void VerifyAuditHistoryRecordsForPackageUpload(B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.WaitForElementDisplayed(TimeSpan.FromSeconds(10));
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message);

            b2BCatalogPackagingDataUploadPage.AuditHistoryTable.WaitForTableLoadComplete(TimeSpan.FromMinutes(1));
            IReadOnlyCollection<IWebElement> historyRows = b2BCatalogPackagingDataUploadPage.AuditHistoryRecords;
            historyRows.Count.Should().BeInRange(1, 13);
            Console.WriteLine(historyRows.Count);

            IReadOnlyCollection<IWebElement> latestRowValues = b2BCatalogPackagingDataUploadPage.GetAuditHistoryRowValues(historyRows.ElementAt(0));
            latestRowValues.ElementAt(0).Text.Should().Be(fileToUpload);
            latestRowValues.ElementAt(1).Text.ToLowerInvariant().Should().Be(Environment.UserName.ToLowerInvariant());
            WaitForPageRefresh();
            Convert.ToDateTime(latestRowValues.ElementAt(2).Text).Should().BeAfter(timeBeforeUpload);
        }

        public void VerifyPackageUploadForNullAndInvalidValues(B2BEnvironment b2BEnvironment, string fileToUpload, string errorMessage)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Should().Be(errorMessage);
        }

        public void VerifyNewFieldsPackageUpload(B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.WaitForElementDisplayed(TimeSpan.FromSeconds(10));
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message).Should().BeTrue();
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking Published status in Auto CatalogList page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingPublishedcheckboxinAutoCatalogListPage(B2BEnvironment b2BEnvironment, CatalogStatus status, string regionName, string countryName)
        {
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status"); ;
            if (!Status.Equals(status.ToString()))
            {
                return false;
            }
            return true;
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking Published status and std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingPublishedandStdConfigcheckboxinAutoCatalogListPage(B2BEnvironment b2BEnvironment,
            string statusDropdown)
        {
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.SelectTheStatus(statusDropdown);
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return (b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected);
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingStdconfigcheckboxinAutoCatalogListPage(B2BEnvironment b2BEnvironment)
        {
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected;
        }
        ///<summary>
        /// Verifies Original/Delta catalog on clicking Test Harness checkbox and std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingTestHarnessandStdConfigcheckboxinAutoCatalogListPage(B2BEnvironment b2BEnvironment,
            string testHarnesscreated, string testHarnessFailed)
        {
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return (b2BAutoCatalogListPage.TestHarnessCheckbox.Selected && b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected);
        }
        public void VerifyRegionInGeneralAndAutoBHC(B2BEnvironment b2BEnvironment, string profileName)
        {
            B2BHomePage b2BHomePage = new B2BHomePage(webDriver);
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.ClickB2BProfileList();
            B2BCustomerProfileListPage b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", profileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName);
            B2BManageProfileIdentitiesPage b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            string regionName = b2BManageProfileIdentitiesPage.RegionName_Globalization.Text.Split(':')[1].Trim();
            regionName.Should().Be("US");
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            b2BBuyerCatalogPage.CatalogRegion.Text.Should().Be(regionName);
        }

        /// <summary>
        /// Verifies the search results on Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public bool VerifyCatalogSearchWithScheduledStatus(B2BEnvironment b2BEnvironment, CatalogStatus status, string regionName, string countryName)
        {
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            if (status == CatalogStatus.Scheduled)
                b2BAutoCatalogListPage.ScheduledCheckbox.Click();

            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            if (!(b2BAutoCatalogListPage.CatalogListTableRows.Count > 0))
                return false;

            if (
                 !b2BAutoCatalogListPage.CatalogListTableRows.All(
                     r =>
                         r.FindElements(By.TagName("td"))[3].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[0].FindElements(By.TagName("td"))[0].Text)))
                return false;

            try
            {
                var i = 1;
                var noOfPages = b2BAutoCatalogListPage.NoOfPages.Text.Split(' ');
                while (Convert.ToInt32(noOfPages[2]) > i)
                {
                    b2BAutoCatalogListPage.NextButton.Click();
                    if (
                        !b2BAutoCatalogListPage.CatalogListTableRows.All(
                            r =>
                                r.FindElements(By.TagName("td"))[3].Text.Equals(
                                    b2BAutoCatalogListPage.StatusTable[0].FindElements(By.TagName("td"))[0].Text)))
                        return false;

                    i++;
                }
            }
            catch
            {
                //ignored 
            }

            return true;
        }

        public void VerifyOriginalCatalogForConfig(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, ConfigRules configRules = ConfigRules.None,
           DefaultOptions defaultOptions=DefaultOptions.Off )
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            uxWorkflow.ValidateCatalogXML(catalogItemType, catalogType, identityName, filePath, beforeSchedTime, configRules, defaultOptions).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
            //uxWorkflow.ValidateCatalogEMails(identityName, beforeSchedTime, operation);
        }

        public void VerifyCatalogStatus(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment); 
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
        }

        public bool VerifyCatalogExpiresFieldValues(B2BEnvironment b2BEnvironment, string profileName, string expireDays)
        {
            var expireList = expireDays.Split(',');
            GoToBuyerCatalogTab(b2BEnvironment.ToString(), profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (expireList.Count() ==
                b2BBuyerCatalogPage.CatalogExpire.Select()
                    .Options.Count(o => !string.IsNullOrEmpty(o.GetAttribute("text"))))
            {
                return
                    b2BBuyerCatalogPage.CatalogExpire.Select()
                        .Options.Where(o => !string.IsNullOrEmpty(o.GetAttribute("text")))
                        .All(option => expireList.Contains(option.GetAttribute("text")));
            }

            Console.WriteLine("No expired days shown in Catalog Expires Field");
            return false;
        }

        public bool VerifyCatalogExpiresFieldDafaultValue(B2BEnvironment b2BEnvironment, string profileName, string expireDays)
        {
            GoToBuyerCatalogTab(b2BEnvironment.ToString(), profileName);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            SelectElement selectedValue = new SelectElement(b2BBuyerCatalogPage.CatalogExpire);
            var wantedText = selectedValue.SelectedOption.Text;
            return wantedText.Equals(expireDays);
        }

        public void ValidatePackagingInformation(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string mfgNumber, ConfigRules configRules = ConfigRules.None)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            //uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);

            CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ManufacturerPartNumber.ToString() == mfgNumber).FirstOrDefault();

            string excelQuery = @"select [Region],[Country],[MPN],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$] where [MPN] = '" + mfgNumber + "'";
            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            actualCatalogItem.PackageLength.Should().Be(excelTable.Rows[0]["Package Length"].RoundValue(), "Package Length mismatch");
            actualCatalogItem.PackageWidth.Should().Be(excelTable.Rows[0]["Package Width"].RoundValue(), "Package Width mismatch");
            actualCatalogItem.PackageHeight.Should().Be(excelTable.Rows[0]["Package Height"].RoundValue(), "Package Height mismatch");
            actualCatalogItem.PalletHeight.Should().Be(excelTable.Rows[0]["Pallet Height"].RoundValue(), "Pallet Height mismatch");
            actualCatalogItem.PalletLength.Should().Be(excelTable.Rows[0]["Pallet Length"].RoundValue(), "Pallet Length mismatch");
            actualCatalogItem.PalletWidth.Should().Be(excelTable.Rows[0]["Pallet Width"].RoundValue(), "Pallet Width mismatch");
            actualCatalogItem.PalletUnitsPerLayer.Should().Be(excelTable.Rows[0]["Pallet Units / Layer"].RoundValue(), "Pallet Units / Layer mismatch");
            actualCatalogItem.PalletLayerPerPallet.Should().Be(excelTable.Rows[0]["Pallet Layer / Pallet"].RoundValue(), "Pallet Layer / Pallet mismatch");
            actualCatalogItem.PalletUnitsPerPallet.Should().Be(excelTable.Rows[0]["Pallet Units / Pallet"].RoundValue(), "Pallet Units / Pallet mismatch");
        }

        public void ValidatePackagingInformationForNewFields(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string mfgNumber, ConfigRules configRules = ConfigRules.None)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);

            CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ManufacturerPartNumber.ToString() == mfgNumber).FirstOrDefault();

            string excelQuery = @"select [Region],[Country],[MPN],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$] where [MPN] = '" + mfgNumber + "'";
            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            actualCatalogItem.PalletLength.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Length"]),"Pallet Length not found");
            actualCatalogItem.PalletWidth.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Width"]), "Pallet Width not found");
            actualCatalogItem.PalletHeight.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Height"]), "Pallet Height not found");
            actualCatalogItem.PalletUnitsPerLayer.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Layer"]), "Pallet Units / Layer not found");
            actualCatalogItem.PalletLayerPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Layer / Pallet"]), "Pallet Layer / Pallet not found");
            actualCatalogItem.PalletUnitsPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Pallet"]), "Pallet Units / Pallet not found");

        }

        /// <summary>
        /// Below method verifies whether the BTS Order code exists or not in a Catlog file while Lead time > 3in BHC section
        /// If BTS Order code not exists then it retuns True
        /// </summary>
        public bool VerifyLeadTimeGreaterThanThreeBTSOrderCodesNotExistsInCatalog(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string itemOrderCode)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            if (uxWorkflow.VerifyOrderCodeExistsInCatalogFile(filePath, itemOrderCode))
            return false;
            else
            return true;
           
        }

        /// <summary>
        /// It Verify the SPL with SYS Config, SNP Config, SNP Auto CRT & SYS Auto CRT UI settings in B2B Profile->Buyer Catalog page, under under "Automated BHC Catalog-Processing Rules" section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profile"></param>
        /// <param name="splField"></param>
        /// <param name="snpField"></param>
        /// <param name="sysField"></param>
        /// <param name="snpCRTField"></param>
        /// <param name="sysCRTField"></param>
        /// <returns></returns>
        public bool VerifySPLEnabledSettingsValidations(string environment, string profileName, bool splField, bool snpField, bool sysField, bool snpCRTField, bool sysCRTField)
        {
            //Following will navigate to the page : Profile->Buyer Catalog->Automated BHC Catalog-Processing Rules
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            }
            else
            {
                b2BBuyerCatalogPage.EditScheduleButton.Click();
            }

            //Following will reset the fields by Turning Off for SYS Config, SNP Config, SPL, SYS CRT & SNP CRT fields, so that it makes every iteration execution go smooth
            if (b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
            }
            if (b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            }

            //If Parameter-"splField" is true, then folliwng will Turned On SPL field
            if (splField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                }
            }
            
            //If Parameter-"snpField" is true, then folliwng will Turned On SNP Config field
            if (snpField == true)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigSnP.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigSnP.Click();
                }
        }

            //If Parameter-"sysField" is true, then folliwng will Turned On SYS Config field
            if (sysField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                }
            }

            //If Parameter-"snpCRTField" is true, then folliwng will Turned On SNP Auto CRT field
            if (snpCRTField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
                }
            }

            //If Parameter-"sysCRTField" is true, then folliwng will Turned On SYS Auto CRT field
            if (sysCRTField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
                }
            }

            //Following will saves the profile with above settings
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
            {
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));
            
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            }
            b2BBuyerCatalogPage.UpdateButton.Click();

            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            //Following will verifies SPL with SYS Config, SNP Config, SNP Auto CRT & SYS Auto CRT UI settings are saved correctly or not
            bool matchFlag = true;

            if (splField == true && snpField == false && sysField == false)
            {
                matchFlag &= (UtilityMethods.CompareValues<bool>("SPL", b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected, false));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNP", b2BBuyerCatalogPage.CatalogConfigSnP.Selected, false));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYS", b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected, false));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNPCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected, false));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYSCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected, false));
            }
            else
            {
                matchFlag &= (UtilityMethods.CompareValues<bool>("SPL", b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected, splField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNP", b2BBuyerCatalogPage.CatalogConfigSnP.Selected, snpField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYS", b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected, sysField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNPCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected, snpCRTField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYSCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected, sysCRTField));
            }
            return matchFlag;
        }
        public void VerifyP2PValidationForEnableAutoBHCOFFNewProfile(B2BEnvironment b2BEnvironment, string customerSet, string accessGroup, string profileNameBase, CatalogType catalogType, string message)
        {
            ChannelCatalogWorkflow channelCatalogWorkflow = new ChannelCatalogWorkflow(webDriver);
            var newProfileName = channelCatalogWorkflow.CreateNewProfile(b2BEnvironment.ToString(), customerSet, accessGroup, profileNameBase);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateP2PMessage(b2BEnvironment, newProfileName, newProfileName, catalogType, message);
        }

        public void VerifyP2PValidationForEnableAutoBHCOFFExistingProfile(B2BEnvironment b2BEnvironment, string profileNameBase, CatalogType catalogType, string message)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateP2PMessage(b2BEnvironment, profileNameBase, profileNameBase, catalogType, message);
        }

        public Dictionary<int,string> GetPartViewerInformation(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, CatalogType type, CatalogStatus status, string profile, string identity)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheCustomer(profile);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            string filePath = uxWorkflow.DownloadCatalog(identity, beforeSchedTime);

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;
            
            DateTime catDate = DateTime.Parse(actualCatalogHeader.CatalogDate);
            DateTime expDate = DateTime.Parse(actualCatalogHeader.ExpirationDate);
            string ValueinSubHeaderRowOrigPub = null; int i = 2;
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var ValueinHeaderRowOrigPub = "" + "," + actualCatalogHeader.CatalogName + "," + actualCatalogHeader.IdentityUserName + "," + status.ToString() + "," + actualCatalogHeader.RequesterEmailId + "," + catDate.ToString("dd-MMM-yyyy") + "," + actualCatalogHeader.RequesterEmailId + "," + actualCatalogHeader.CountryCode + "," + expDate.ToString("dd-MMM-yyyy");
            dict.Add(1, ValueinHeaderRowOrigPub);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                IEnumerable<CatalogItem> actualCatalogItems = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == itemType);
                foreach(CatalogItem actualCatalogItem in actualCatalogItems)
                {
                   ValueinSubHeaderRowOrigPub = actualCatalogItem.ShortName + " ," + actualCatalogItem.ItemDescription + " ," + actualCatalogItem.UNSPSC + " ," + actualCatalogItem.UnitPrice.ToString().TrimEnd('0').TrimEnd('.') + " ," + actualCatalogItem.PartId + " ," + actualCatalogItem.QuoteId + " ," + actualCatalogItem.BaseSKUId + " ," + actualCatalogItem.ListPrice.ToString().TrimEnd('0').TrimEnd('.');
                   dict.Add(i, ValueinSubHeaderRowOrigPub); i++;
                }
            }
            return dict;
        }
        #region Unused Code
        /// <summary>
        /// Retrieve Delta Published Auto BHC Config Quote ID thru Part Viewer. Verify all required info in the table
        /// </summary>
        /// <returns></returns>
        //public bool VerifyRetrieveCatalogConfigAquoteId(B2BEnvironment b2BEnvironment, string quoteid, string Headervalue, string HeaderRowvalue, string SubHeadervalue, string SubRowvalue1, string subRowvalue2)
        //{
        //    webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoPartViewerUrl"] + ((environment == B2BEnvironment.Production.ToString()) ? "P" : "U"));
        //    b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
        //    b2BAutoCatalogListPage.PartViewerQuoteIdsLink.SendKeys(quoteid);
        //    b2BAutoCatalogListPage.PartViewerSearchButton.Click();
        //    b2BAutoCatalogListPage.PartViewerPlusButton.WaitForElementVisible(TimeSpan.FromSeconds(30));
        //    b2BAutoCatalogListPage.PartViewerPlusButton.Click();
        //    b2BAutoCatalogListPage.PartViewerSecondPlusButton.Click();
        //    string[] HeaderStringvalue = Headervalue.Split(',');
        //    string[] HeaderRowStringValue = HeaderRowvalue.Split(',');
        //    string[] SubHeaderStringValue = SubHeadervalue.Split(',');
        //    string[] SubRow1StringValue = SubRowvalue1.Split(',');
        //    string[] subRow2StringValue = subRowvalue2.Split(',');
        //    string TableXpath_First = "//*[@id='quoteTable']";
        //    string Table1FirstRow_End = "/tbody[1]/tr[1]/td";
        //    string Table1SubHeadingEnd = "/tbody[1]/tr[2]/td[2]/table/thead/tr/th";
        //    string Table1SubRow_End = "/tbody[1]/tr[2]/td[2]/table/tbody/tr/td";
        //    string Table2FirstRow_End = "/tbody[2]/tr[1]/td";
        //    string Table2SubHeading_End = "/tbody[2]/tr[2]/td[2]/table/thead/tr/th";
        //    string Table2SubRow_End = "/tbody[2]/tr[2]/td[2]/table/tbody/tr/td";
        //    for (int j = 0; j < SubHeaderStringValue.Length; j++)
        //    {
        //        var SubHeaderElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubHeadingEnd))[j];
        //        var subHeaderTable1fromLocator = SubHeaderElement.Text;
        //        var SubRowElemt = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubRow_End))[j];
        //        var subRowTable1fromLocator = SubRowElemt.Text;
        //        var SubHeadingTable2 = webDriver.FindElements(By.XPath(TableXpath_First + Table2SubHeading_End))[j];
        //        var subHeadingtable2fromLocator = SubHeadingTable2.Text;
        //        var SubRowTable2 = webDriver.FindElements(By.XPath(TableXpath_First + Table2SubRow_End))[j];
        //        var subRowTable2fromLocator = SubRowTable2.Text;
        //        var subHeaderTestdata = SubHeaderStringValue[j];
        //        var subRow1Testdata = SubRow1StringValue[j].Replace('_', ',');
        //        var subRow2Testdata = subRow2StringValue[j].Replace('_', ',');
        //        if (subHeaderTable1fromLocator.Equals(subHeaderTestdata) && subRowTable1fromLocator.Equals(subRow1Testdata) && subHeadingtable2fromLocator.Equals(subHeaderTestdata) && subRowTable2fromLocator.Equals(subRow2Testdata))
        //        {
        //            subHeaderRows++;
        //        }
        //    }
        //    //Header
        //    for (int i = 0; i < HeaderStringvalue.Length; i++)
        //    {
        //        var HeaderElement = b2BAutoCatalogListPage.PartViewerHeader.FirstOrDefault().FindElements(By.TagName("th"))[i];
        //        var HeaderTextfromLocator = HeaderElement.Text;
        //        var HeadTestdata = HeaderStringvalue[i];
        //        if (HeaderTextfromLocator.Equals(HeadTestdata))
        //        {
        //            Headercount++;
        //        }
        //    }
        //    //HeaderRows
        //    for (int z = 1; z < HeaderRowStringValue.Length; z++)
        //    {
        //        var HeaderRowElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1FirstRow_End))[z];
        //        var HeaderRowTextfromLocator = HeaderRowElement.Text;
        //        var HeaderRowtable2Element = webDriver.FindElements(By.XPath(TableXpath_First + Table2FirstRow_End))[z];
        //        var HeaderRowTable2TextfromLocator = HeaderRowtable2Element.Text;
        //        var HeaderRowTestData = HeaderRowStringValue[z];
        //        if (HeaderRowTextfromLocator.Equals(HeaderRowTestData) && HeaderRowTable2TextfromLocator.Equals(HeaderRowTestData))
        //        {
        //            HeaderRowsCount++;
        //        }
        //    }
        //    // Sub Header and Sub Rows Table1

        //    if (Headercount.Equals(8) && HeaderRowsCount.Equals(8) && subHeaderRows.Equals(8))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        #endregion


        /// <summary>
        /// Below method verifies ManufacturePartNumber and UP values for SPL Order code
        /// If ManufacturePartNumber and UP values are valid values then it retuns True
        /// </summary>
        public bool VerifyMpnAndUpcForSpl(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType,
                                          bool splUI, bool snpUI, bool sysUI, bool isSNP,
                                          string splItemOrderCode, string upcField, string upcValue,
                                          string mpnField, string mpnValue)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            VerifySPLEnabledSettingsValidations(b2BEnvironment.ToString(), profileName, splUI, snpUI, sysUI, false, false);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);

            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            //Below method verifies the ManufacturePartNumber and UPC values for SPL Item and if those values are valid values it returns True
            if (uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, splItemOrderCode, upcField, upcValue, isSNP) &&
                uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, splItemOrderCode, mpnField, mpnValue, isSNP))
                return true;
            else
                return false;
        }

    }

    /// <summary>
    /// Enum to restrict the types of frequencies used
    /// </summary>
    public enum FrequencyType
    {
        Days,
        Weeks
    }
}
