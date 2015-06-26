﻿using System;
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
            WaitForPageRefresh();
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
        public bool VerifyOkClickOnUploadAlert(string environment, string fileToBeUploaded, string uploadMessage)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ChannelCatalogUxLink.Click();
            WaitForPageRefresh();
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
        public bool VerifyStatusOnAutoCatalogListPage(string environment, string autoCatalogStatus, string autoCatalogStatusDescription)
        {
            var autoCatStatus = autoCatalogStatus.Split(',');
            var autoCatStatusDescription = autoCatalogStatusDescription.Split(',');
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
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
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            b2BBuyerCatalogPage.CatalogConfigUpsellDownSell.Click();
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
        /// <param name="environment"></param>
        /// <param name="pageHeaderText"></param>
        /// <returns></returns>
        public bool VerifyNavigationToAutoCatalogListPage(string environment, string pageHeaderText)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            return b2BAutoCatalogListPage.PageHeader.Text.Equals(pageHeaderText);
        }

        /// <summary>
        /// Checks if the Clear All link removes the previously set search criteria
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyClearAllLink(string environment, string profileName, string identities)
        {
            b2BHomePage.SelectEnvironment(environment);
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
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool VerifyThreadIdLinkInAutoCatalogListPage(string environment)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            var firstThreadIdElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[7];
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
            return webDriver.Url.ToLowerInvariant().Contains("b2blogreportb.aspx?threadid=" + threadId);
        }



        /// <summary>
        /// Verifies the presence and functionality of Country code for Published in Auto Cataog List Page
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeInAutoCatalogListPage(string environment, string profileName, string status, string countryCode, string region, string currencycode)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profileName);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[8];
            var countryCodea = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var currencyCode = firstcurrencycode.Text;
            if (status.Equals(Status) && countryCode.Equals(countryCodea) && region.Equals(regioncode) && currencycode.Equals(currencyCode))
            {
                return true;
            }

            return false;
            }

        ///<summary>
        /// Verified country region and currency fields for scheduled catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeScheduledInAutoCatalogListPage(string status)
        {
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ScheduledCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();

            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[8];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var cUrrencycode = firstcurrencycode.Text;
            if (Status.Equals(status) && countryCode.Equals("") && regioncode.Equals("") && cUrrencycode.Equals(""))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence and functionality of Country code in Auto Cataog List Page for failed catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeFailedInAutoCatalogListPage(string environment, string profilename, string status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[8];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var currencyCode = firstcurrencycode.Text;
            if (Status.Equals(status) && countryCode.Equals("") && regioncode.Equals("") && currencyCode.Equals(""))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// Verifies the presence and functionality of Country code in Auto Cataog List Page.
        /// </summary>
        /// <returns></returns>
        public bool VerifyTestHarnessCheckboxInAutoCatalogListPage(string environment, string profilename, string status, string countrycode, string regionCode, string currencycode)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[8];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var cUrrencycode = firstcurrencycode.Text;
            if (Status.Equals(status) && countryCode.Equals(countrycode) && regioncode.Equals(regionCode) && cUrrencycode.Equals(currencycode))
            {
                return true;
            }
            return false;
        }
        
        ///// <summary>
        ///// Verifies the presence of Download link for original published catalogs in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkInAutoCatListPage(string environment, string profilename, string type, string status)
        {

            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firstTypeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[1];
            var Type = firstTypeElement.Text;
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Status = firststatusElement.Text;
            if (Type.Equals(type) && Status.Equals(status))
            {
                if (!b2BAutoCatalogListPage.DownloadButton.Displayed)
                {
                    return false;
                }
                b2BAutoCatalogListPage.DownloadButton.Click();
                return true;
            }


            return false;

        }

        /// <summary>
        /// Verifies the Select Customer drop down on the Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool VerifySelectCustomerFieldOnAutoCatalogListPage(string environment, string profileName)
        {
            b2BHomePage.SelectEnvironment(environment);
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
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identities"></param>
        /// <returns></returns>
        public bool VerifyIdentityFieldOnAutoCatalogListPage(string environment, string profileName, string identities)
        {
            var identityList = identities.Split(',');
            b2BHomePage.SelectEnvironment(environment);
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
        /// Verifies the search results on Auto Catalog List Page
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public bool VerifySearchResultsOnAutoCatalogListPage(string environment, string profileName, string identity)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheCustomer(profileName);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheIdentity(identity);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r =>
                        r.FindElements(By.TagName("td"))[0].Text.ToLowerInvariant()
                            .StartsWith(identity.ToLowerInvariant())))
                return false;

            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r => r.FindElements(By.TagName("td"))[0].ElementExists(By.TagName("a"))))
                return false;

            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r =>
                        r.FindElements(By.TagName("td"))[1].Text.ToLowerInvariant().Equals("delta") ||
                        r.FindElements(By.TagName("td"))[1].Text.ToLowerInvariant().Equals("original")))
                return false;

            if (
                !b2BAutoCatalogListPage.CatalogListTableRows.All(
                    r => r.FindElements(By.TagName("td"))[0].Text.EndsWith(r.FindElements(By.TagName("td"))[7].Text)))
                return false;

            if (
                 !b2BAutoCatalogListPage.CatalogListTableRows.All(
                     r =>
                         r.FindElements(By.TagName("td"))[2].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[0].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[2].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[1].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[2].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[2].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[2].Text.Equals(
                             b2BAutoCatalogListPage.StatusTable[3].FindElements(By.TagName("td"))[0].Text) ||
                         r.FindElements(By.TagName("td"))[2].Text.Equals(
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
