using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private B2BAutoCatalogListPage b2BAutoCatalogListPage;
        private IJavaScriptExecutor javaScriptExecutor;
        const string MMDDYYYY = "MM/dd/yyyy";

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
        /// <param name="profileName"></param>
        public void GoToBuyerCatalogTab(string profileName)
        {
            b2BHomePage.ClickB2BProfileList();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", profileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName);
            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("Opened Profile Page for profile: {0}", profileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Creates a new profile with the CustomerSet & AccessGroup provided 
        /// and navigates to the Buyer Catalog Tab
        /// </summary>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        public void CreateNewProfileAndGoToBuyerCatalogTab(string customerSet, string accessGroup, string profileNameBase)
        {
            var newProfileName = profileNameBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            Console.WriteLine("Profile creation start with name: {0}", newProfileName);
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
            Console.WriteLine("New profile created with Name: {0}", newProfileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
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
        public string VerifyCancelClickOnUploadAlert(string fileToUpload)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Dismiss();
            return b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim();
        }

        /// <summary>
        /// Returns the count of Audit History Records on Packaging Data Upload Page
        /// </summary>
        /// <param name="packagingDataFile"></param>
        /// <returns></returns>
        public int VerifyCountOfRecordsInAuditHistory(string packagingDataFile)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
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
        /// <param name="fileToBeUploaded"></param>
        /// <param name="uploadMessage"></param>
        /// <returns></returns>
        public bool VerifyOkClickOnUploadAlert(string fileToBeUploaded, string uploadMessage)
        {
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
            return UploadAndCheckMessageAndValidate(fileToBeUploaded, uploadMessage);
        }

        /// <summary>
        /// Validates the messages received on uploading packaging data files 
        /// with Invalid Values in OrderCode, LOB & ConfigName columns in Packaging Data Upload Page
        /// </summary>
        /// <param name="invalidValueInOrderCodeFile"></param>
        /// <param name="invalidValueInOrderCodeErrorMessage"></param>
        /// <param name="invalidValueInLobFile"></param>
        /// <param name="invalidValueInLobErrorMessage"></param>
        /// <param name="invalidValueInConfigNameFile"></param>
        /// <param name="invalidValueInConfigNameErrorMessage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validates the messages received on uploading packaging data files with Invalid Values
        /// in PackageHeight, PackageLength, PackageWidth & ShipWeight columns in Packaging Data Upload Page
        /// </summary>
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

        /// <summary>
        /// Verifies default delta scheduling options for a new profile
        /// </summary>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDeltaSchedulingDefaultOptionsForNewProfile(string customerSet, string accessGroup,
            string profileNameBase, string defaultDeltaTimeOfSend)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(customerSet, accessGroup, profileNameBase);
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            return VerifyDeltaCatalogSchedulingOptions(DateTime.Now.AddDays(1).ToString(MMDDYYYY), "0", "1",
                DateTime.Now.AddDays(59).ToString(MMDDYYYY), defaultDeltaTimeOfSend);
        }

        /// <summary>
        /// Verifies default delta scheduling options for an existing profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDeltaSchedulingDefaultOptionsForExistingProfile(string profileName,
            string defaultDeltaTimeOfSend)
        {
            GoToBuyerCatalogTab(profileName);
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
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool ChangeDeltaSchedulingDefaultOptionsForExistingProfile(string profileName)
        {
            var startDate = DateTime.Now.AddDays(5).ToString(MMDDYYYY);
            var frequencyDays = "2";
            var endDate = DateTime.Now.AddDays(30).ToString(MMDDYYYY);
            var timeOfSend = "9";
            GoToBuyerCatalogTab(profileName);
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
        /// <param name="autoCatalogStatus"></param>
        /// <param name="autoCatalogStatusDescription"></param>
        /// <returns></returns>
        public bool VerifyStatusOnAutoCatalogListPage(string[] autoCatalogStatus, string[] autoCatalogStatusDescription)
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            var statusDictionary = b2BAutoCatalogListPage.GetStatusDictionary();
            return CheckDictionary(statusDictionary, autoCatalogStatus, autoCatalogStatusDescription);
        }

        /// <summary>
        /// Verifies both Default Original Time Of Send and 
        /// Default Delta Time Of Send for a new profile
        /// </summary>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDefaultTimeOfSendForNewProfile(string customerSet, string accessGroup, string profileNameBase,
            string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(customerSet, accessGroup, profileNameBase);
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
        /// <param name="profileName"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <returns></returns>
        public bool VerifyDefaultTimeOfSendForExistingProfile(string profileName, string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend)
        {
            GoToBuyerCatalogTab(profileName);
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
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="autoBhcSaveMessage"></param>
        /// <param name="windowsLogin"></param>
        /// <returns></returns>
        public bool VerifyRequestedByAutopopulationForNewProfile(string customerSet, string accessGroup,
            string profileNameBase, string autoBhcSaveMessage, string windowsLogin)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(customerSet, accessGroup, profileNameBase);
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
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <param name="windowsLogin"></param>
        /// <returns></returns>
        public bool VerifyRequestedByAutopopulationForExistingProfile(string profileName, string scheduleSaveConfirmation, string windowsLogin)
        {
            GoToBuyerCatalogTab(profileName);

            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            if (!b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Selected)
                b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();
            b2BBuyerCatalogPage.BuyerCatalogTab.Click();
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
            b2BBuyerCatalogPage.UncheckAllConfigTypes();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.UpdateButton.Click();
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
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForCatalogOperationChange(string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(profileName);
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
        /// <param name="profileName"></param>
        /// <param name="defaultOriginalTimeOfSend"></param>
        /// <param name="defaultDeltaTimeOfSend"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForConfigChange(string profileName, string defaultOriginalTimeOfSend, string defaultDeltaTimeOfSend, string scheduleSaveConfirmation)
        {
            var originalStartDate = DateTime.Now.AddDays(10).ToString(MMDDYYYY);
            var originalFrequencyDays = "5";
            var originalEndDate = DateTime.Now.AddDays(30).ToString(MMDDYYYY);
            var originalTimeOfSend = "8";

            var deltaStartDate = DateTime.Now.AddDays(11).ToString(MMDDYYYY);
            var deltaFrequencyDays = "5";
            var deltaEndDate = DateTime.Now.AddDays(29).ToString(MMDDYYYY);
            var deltaTimeOfSend = "9";


            GoToBuyerCatalogTab(profileName);
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
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningMessageForMultipleIdentity(string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(profileName);

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
        /// <param name="profileName"></param>
        /// <param name="internalEmailAdress"></param>
        /// <param name="customerEmailAdress"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponEmailFieldChange(string profileName, string internalEmailAdress,
            string customerEmailAdress, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(profileName);

            b2BBuyerCatalogPage.InternalEMail.SendKeys(string.Empty);
            b2BBuyerCatalogPage.CustomerEmail.SendKeys(string.Empty);
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            b2BBuyerCatalogPage.InternalEMail.SendKeys(internalEmailAdress);
            b2BBuyerCatalogPage.CustomerEmail.SendKeys(customerEmailAdress);

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

            if (!b2BBuyerCatalogPage.InternalEMail.Text.Equals(internalEmailAdress))
                return false;

            if (!b2BBuyerCatalogPage.CustomerEmail.Text.Equals(customerEmailAdress))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if warning message is displayed when Auto BHC is turned off - Existing Profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponTurningAutoBhcOff(string profileName, string scheduleSaveConfirmation)
        {
            GoToBuyerCatalogTab(profileName);
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
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <param name="scheduleSaveConfirmation"></param>
        /// <returns></returns>
        public bool VerifyWarningUponTurningAutoBhcOnForNewProfile(string customerSet, string accessGroup, string profileNameBase, string scheduleSaveConfirmation)
        {
            CreateNewProfileAndGoToBuyerCatalogTab(customerSet, accessGroup, profileNameBase);

            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();

            b2BBuyerCatalogPage.Identities[0].Click();
            b2BBuyerCatalogPage.CatalogConfigStandard.Click();

            SetOriginalSchedule(DateTime.Now.AddDays(10).ToString(MMDDYYYY), "5", FrequencyType.Days,
                DateTime.Now.AddDays(30).ToString(MMDDYYYY), "8");

            b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            SetDeltaSchedule(DateTime.Now.AddDays(11).ToString(MMDDYYYY), "5", FrequencyType.Days,
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
        /// <param name="profileName"></param>
        /// <param name="pastTimeErrorStart"></param>
        /// <returns></returns>
        public bool VerifyEditingErrorNotLockingScheduleSection(string profileName, string pastTimeErrorStart)
        {
            var centralTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            GoToBuyerCatalogTab(profileName);
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

            WaitForPageRefresh();
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

            return VerifyConfigScheduleSectionEnabled();
        }

        /// <summary>
        /// Checks if navigation to Auto Catalog List page is successful
        /// </summary>
        /// <param name="pageHeaderText"></param>
        /// <returns></returns>
        public bool VerifyNavigationToAutoCatalogListPage(string pageHeaderText)
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            return b2BAutoCatalogListPage.PageHeader.Text.Equals(pageHeaderText);
        }

        /// <summary>
        /// Checks if the Clear All link removes the previously set search criteria
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyClearAllLink(string profileName, string identities)
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();

            b2BAutoCatalogListPage.SelectTheCustomer(profileName);
            WaitForPageRefresh();
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
        /// <returns></returns>
        public bool VerifyThreadIdLinkInAutoCatalogListPage()
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            var firstThreadIdElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[6];
            var threadId = firstThreadIdElement.Text;

            if (!firstThreadIdElement.ElementExists(By.TagName("a")))
            {
                Console.WriteLine("Thread ID column with value **{0}** is not a hyperlink ", threadId);
                return false;
            }

            var threadIdLink = firstThreadIdElement.FindElement(By.TagName("a"));
            threadId = threadIdLink.Text;
            threadIdLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            return webDriver.Url.ToUpperInvariant().Contains("b2blogreportb.aspx?threadid=" + threadId);
        }

        /// <summary>
        /// Verifies the Select Customer drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool VerifySelectCustomerFieldOnAutoCatalogListPage(string profileName)
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            return b2BAutoCatalogListPage.SelectCustomer.Select().Options.Count() > 1 &&
                   b2BAutoCatalogListPage.SelectCustomer.Select().Options.Any(o => o.GetAttribute("text").Equals(profileName));
        }

        /// <summary>
        /// Verifies the Identity drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyIdentityFieldOnAutoCatalogListPage(string profileName, string identities)
        {
            var identityList = identities.Split(',');
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheCustomer(profileName);
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
            if (!b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value").Equals(startDate))
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

            if (!b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value").Equals(endDate))
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
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
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
            b2BCatalogPackagingDataUploadPage.FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToBeUploaded);
            b2BCatalogPackagingDataUploadPage.UploadButton.Click();
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
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
            var uploadAlert = webDriver.SwitchTo().Alert();
            uploadAlert.Accept();
            WaitForPageRefresh();
            Console.WriteLine("Message received on upload: **{0}**",
                b2BCatalogPackagingDataUploadPage.UploadMessage.Text);
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
