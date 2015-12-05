﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium.Support.UI;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using System.Data;
using Modules.Channel.B2B.DAL.ChannelCatalog;
using Modules.Channel.B2B.DAL;

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
        public bool VerifyOkClickOnUploadAlert(ConstantObjects.B2BEnvironment environment, string fileToBeUploaded, string uploadMessage)
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
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            return b2BAutoCatalogListPage.PageHeader.Text.Contains(pageHeaderText);
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
            var firstThreadIdElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[8];
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

        ///<summary>
        /// Verifies country region and currency fields for original/delta created/published catalogs in Auto Cat List Page
        /// </summary>
        public bool VerifyCountryCodepublishedcreatedInAutoCatListPage(string environment, string profilename, string status,
            string CountryCode, string region, string currencyCode)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[11];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[13];
            var Currencycode = firstcurrencycode.Text;
            if (Status.Equals(status) && countryCode.Equals(CountryCode) && regioncode.Equals(region) && Currencycode.Equals(currencyCode))
            {
                return true;
            }
            return false;

        }

        ///<summary>
        /// Verified country region and currency fields for scheduled catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeScheduledInAutoCatalogListPage(string environment, string status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.ScheduledCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[12];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[14];
            var cUrrencycode = firstcurrencycode.Text;
            if (Status.Equals(status) && countryCode.Equals("") && regioncode.Equals("") && cUrrencycode.Equals(""))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies the presence and functionality of country region and currency fields in Auto Cataog List Page for failed catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeFailedInAutoCatalogListPage(string environment, string profilename, string status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[10];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[12];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[14];
            var currencyCode = firstcurrencycode.Text;
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
            string profilename, string countrycode, string regionCode, string currencycode, string status)
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
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[11];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[13];
            var cUrrencycode = firstcurrencycode.Text;
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
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstCountryCodeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[9];
            var countryCode = firstCountryCodeElement.Text;
            var firstregioncode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[11];
            var regioncode = firstregioncode.Text;
            var firstcurrencycode = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[13];
            var cUrrencycode = firstcurrencycode.Text;
            if (countryCode.Equals("") && regioncode.Equals("") && cUrrencycode.Equals("") && Status.Equals(status))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies the presence of Download link for original/delta published/Created catalogs in Auto Cat List page
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
            var firstTypeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
            if (Type.Equals(type) && Status.Equals(status))
            {
                DownloadLinkElement.Click();
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies the presence of Download link for Failed catalogs in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkFailedOrigInAutoCatListPage(string environment, string profilename, string type, string status)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firstTypeElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var firststatusElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var DownloadLinkElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
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
        ///// Verifies the presence of Download link for original scheduled catalogs in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkScheduledOrigInAutoCatListPage(string environment, string status, string type)
        {

            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ScheduledCheckbox.Click();
            b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firstTypeElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var firststatusElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;

            var DownloadLinkElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
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
        ///// Verifies the presence of Download link for Delta scheduled catalogs in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkScheduledDeltaInAutoCatListPage(string environment, string status, string type)
        {

            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ScheduledCheckbox.Click();
            b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firstTypeElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var firststatusElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;

            var DownloadLinkElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
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
        ///// Verifies the presence of Download link for Created through Test harness in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyDownloadLinkInAutoCatListPageCreatedTestHarness(string environment, string profilename, string type, string status)
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
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var firstTypeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var DownloadLinkElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
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
        public bool VerifyDownloadLinkFailedThrutestHarnessInAutoCatListPage(string environment, string profile, string type, string status)
        {

            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var firstTypeElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[2];
            var Type = firstTypeElement.Text;
            var firststatusElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            var DownloadLinkElement =
                b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[18];
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
        public bool VerifyCatalogNameInAutoCatListPage(string environment, string profile, string catalogName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var catalogNameElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[0];
            var CatalogNamefromLocator = catalogNameElement.Text;
            if (CatalogNamefromLocator.Equals(catalogName))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies Status Time in Auto Cat List page
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyStatusTimeInAutoCatListPage(string environment, string profile, string statusTime)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            webDriver.WaitForElement(b2BAutoCatalogListPage.NextButton);
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            webDriver.WaitForTableRowCount(b2BAutoCatalogListPage.CatalogsTable, 1);
            var statusTimeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[4];
            if (statusTimeElement.Text.Equals(statusTime))
                return true;
            return false;
        }

        ///// <summary>
        ///// Verifies Status Time in Auto Cat List page for Test Harness
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyStatusTimeforTestHarnessInAutoCatListPage(string environment, string profile, string statusTime)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            webDriver.WaitForElement(b2BAutoCatalogListPage.NextButton);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            webDriver.WaitForTableRowCount(b2BAutoCatalogListPage.CatalogsTable,1);
            var statusTimeElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[4];
            if (statusTimeElement.Text.Equals(statusTime))
                return true;
            return false;
        }

        ///// <summary>
        ///// Verifies Profile Name for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyProfileNameAutoCatPage(string environment, string profile, string profileName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            webDriver.WaitForElement(b2BAutoCatalogListPage.NextButton);
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            webDriver.WaitForTableRowCount(b2BAutoCatalogListPage.CatalogsTable, 1);
            var profileNameElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[1];
            var profileNamefromLocator = profileNameElement.Text;
            if (profileNamefromLocator.Equals(profileName))
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// Verifies Profile Name for Test Harness for existing profile
        ///// </summary>
        ///// <returns></returns>
        public bool VerifyProfileNameforTestHarnessAutoCatPage(string environment, string profile)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile);
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();
            var profileNameElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[1];
            var profileNamefromLocator = profileNameElement.Text;
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
            else return false;

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
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.SearchCatalogLink.Click();
            WaitForPageRefresh();

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
        public bool VerifyRetrieveCatalogConfigAquoteId(string environment, string quoteid, string Headervalue, string HeaderRowvalue, string SubHeadervalue, string SubRowvalue1, string subRowvalue2)
        {

            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogPartViewerLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            WaitForPageRefresh();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(5));
            b2BAutoCatalogListPage.PartViewerQuoteIdsLink.SendKeys(quoteid);
            b2BAutoCatalogListPage.PartViewerSearchButton.Click();
            WaitForPageRefresh();
            b2BAutoCatalogListPage.PartViewerPlusButton.Click();
            b2BAutoCatalogListPage.PartViewerSecondPlusButton.Click();
            string[] HeaderStringvalue = Headervalue.Split(',');
            string[] HeaderRowStringValue = HeaderRowvalue.Split(',');
            string[] SubHeaderStringValue = SubHeadervalue.Split(',');
            string[] SubRow1StringValue = SubRowvalue1.Split(',');
            string[] subRow2StringValue = subRowvalue2.Split(',');
            string TableXpath_First = "//*[@id='quoteTable']";
            string Table1FirstRow_End = "/tbody[1]/tr[1]/td";
            string Table1SubHeadingEnd = "/tbody[1]/tr[2]/td[2]/table/thead/tr/th";
            string Table1SubRow_End = "/tbody[1]/tr[2]/td[2]/table/tbody/tr/td";
            string Table2FirstRow_End = "/tbody[2]/tr[1]/td";
            string Table2SubHeading_End = "/tbody[2]/tr[2]/td[2]/table/thead/tr/th";
            string Table2SubRow_End = "/tbody[2]/tr[2]/td[2]/table/tbody/tr/td";
            for (int j = 0; j < SubHeaderStringValue.Length; j++)
            {
                var SubHeaderElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubHeadingEnd))[j];
                var subHeaderTable1fromLocator = SubHeaderElement.Text;
                var SubRowElemt = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubRow_End))[j];
                var subRowTable1fromLocator = SubRowElemt.Text;
                var SubHeadingTable2 = webDriver.FindElements(By.XPath(TableXpath_First + Table2SubHeading_End))[j];
                var subHeadingtable2fromLocator = SubHeadingTable2.Text;
                var SubRowTable2 = webDriver.FindElements(By.XPath(TableXpath_First + Table2SubRow_End))[j];
                var subRowTable2fromLocator = SubRowTable2.Text;
                var subHeaderTestdata = SubHeaderStringValue[j];
                var subRow1Testdata = SubRow1StringValue[j];
                var subRow2Testdata = subRow2StringValue[j];
                if (subHeaderTable1fromLocator.Equals(subHeaderTestdata) && subRowTable1fromLocator.Equals(subRow1Testdata) && subHeadingtable2fromLocator.Equals(subHeaderTestdata) && subRowTable2fromLocator.Equals(subRow2Testdata))
                {
                    subHeaderRows++;
                }
            }
            //Header
            for (int i = 0; i < HeaderStringvalue.Length; i++)
            {
                var HeaderElement = b2BAutoCatalogListPage.PartViewerHeader.FirstOrDefault().FindElements(By.TagName("th"))[i];
                var HeaderTextfromLocator = HeaderElement.Text;
                var HeadTestdata = HeaderStringvalue[i];
                if (HeaderTextfromLocator.Equals(HeadTestdata))
                {
                    Headercount++;
                }
            }
            //HeaderRows
            for (int z = 1; z < HeaderRowStringValue.Length; z++)
            {
                var HeaderRowElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1FirstRow_End))[z];
                var HeaderRowTextfromLocator = HeaderRowElement.Text;
                var HeaderRowtable2Element = webDriver.FindElements(By.XPath(TableXpath_First + Table2FirstRow_End))[z];
                var HeaderRowTable2TextfromLocator = HeaderRowtable2Element.Text;
                var HeaderRowTestData = HeaderRowStringValue[z];
                if (HeaderRowTextfromLocator.Equals(HeaderRowTestData) && HeaderRowTable2TextfromLocator.Equals(HeaderRowTestData))
                {
                    HeaderRowsCount++;
                }
            }
            // Sub Header and Sub Rows Table1

            if (Headercount.Equals(9) && HeaderRowsCount.Equals(9) && subHeaderRows.Equals(8))
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
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
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

        /// <summary>
        /// Verifies Sys checkbox is present and non editable in Auto Cat List page. 
        /// </summary>
        public bool VerifySysCheckboxinAutoCatListPage(string environment)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            webDriver.WaitForElement(b2BAutoCatalogListPage.NextButton);
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
        /// Verifies SPL Flag checkbox is present in Auto BHC section. 
        /// </summary>
        public bool VerifySplFlaginAutoBhcSection(string environment, string profileName)
        {
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
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
            b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
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
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                {
                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
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
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
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
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                {
                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
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
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected &&
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
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
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigSnP.Click();
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    b2BBuyerCatalogPage.UpdateButton.Click();
                    b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                    b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                    b2BBuyerCatalogPage.EditScheduleButton.Click();
                    b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                        DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                    if (b2BBuyerCatalogPage.CatalogConfigSnP.Selected &&
                        b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            if (!b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
                b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();
                b2BBuyerCatalogPage.EditScheduleButton.Click();
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                    DateTime.Now.AddDays(1).ToString(MMDDYYYY));
                if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected &&
                    b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        public void VerifyRoundOffValuesPackageUploadForAllFieldsProd(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Should().Be(message, "Incorrect message for packaging file upload");

            string excelQuery = @"select [Order code],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$]";

            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            for (int iteration = 0; iteration < 10; iteration++)
            {
                int index = new Random().Next(0, excelTable.Rows.Count - 1);
                Channel_Catalog_PackagingData dbData = ChannelCatalogProdDataAccess.GetPackagingDetails(excelTable.Rows[index]["Order code"].ToString());

                Console.WriteLine("Data validation for Order Code: " + excelTable.Rows[index]["Order code"].ToString());
                dbData.ShipWeight.Should().Be(excelTable.Rows[index]["Ship Weight"].RoundValue(), "Ship Weight mismatch");
                dbData.PackageLength.Should().Be(excelTable.Rows[index]["Package Length"].RoundValue(), "Package Length mismatch");
                dbData.PackageWidth.Should().Be(excelTable.Rows[index]["Package Width"].RoundValue(), "Package Width mismatch");
                dbData.PackageHeight.Should().Be(excelTable.Rows[index]["Package Height"].RoundValue(), "Package Height mismatch");
                dbData.PalletLength.Should().Be(excelTable.Rows[index]["Pallet Length"].RoundValue(), "Pallet Length mismatch");
                dbData.PalletWidth.Should().Be(excelTable.Rows[index]["Pallet Width"].RoundValue(), "Pallet Width mismatch");
                dbData.PalletHeight.Should().Be(excelTable.Rows[index]["Pallet Height"].RoundValue(), "Pallet Height mismatch");
                dbData.PalletUnitsPerLayer.Should().Be(excelTable.Rows[index]["Pallet Units / Layer"].RoundValue(), "Pallet Units / Layer mismatch");
                dbData.PalletLayerPerPallet.Should().Be(excelTable.Rows[index]["Pallet Layer / Pallet"].RoundValue(), "Pallet Layer / Pallet mismatch");
                dbData.PalletUnitsPerPallet.Should().Be(excelTable.Rows[index]["Pallet Units / Pallet"].RoundValue(), "Pallet Units / Pallet mismatch");
            }
        }

        public void VerifyRoundOffValuesPackageUploadForAllFieldsPrev(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message);

            string excelQuery = @"select [Order code],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$]";

            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            for (int iteration = 0; iteration < 10; iteration++)
            {
                int index = new Random().Next(0, excelTable.Rows.Count - 1);
                Channel_Catalog_PackagingData dbData = ChannelCatalogPrevDataAccess.GetPackagingDetails(excelTable.Rows[index]["Order code"].ToString());

                Console.WriteLine("Data validation for Order Code: " + excelTable.Rows[index]["Order code"].ToString());
                dbData.ShipWeight.Should().Be(excelTable.Rows[index]["Ship Weight"].RoundValue(), "Ship Weight mismatch");
                dbData.PackageLength.Should().Be(excelTable.Rows[index]["Package Length"].RoundValue(), "Package Length mismatch");
                dbData.PackageWidth.Should().Be(excelTable.Rows[index]["Package Width"].RoundValue(), "Package Width mismatch");
                dbData.PackageHeight.Should().Be(excelTable.Rows[index]["Package Height"].RoundValue(), "Package Height mismatch");
                dbData.PalletLength.Should().Be(excelTable.Rows[index]["Pallet Length"].RoundValue(), "Pallet Length mismatch");
                dbData.PalletWidth.Should().Be(excelTable.Rows[index]["Pallet Width"].RoundValue(), "Pallet Width mismatch");
                dbData.PalletHeight.Should().Be(excelTable.Rows[index]["Pallet Height"].RoundValue(), "Pallet Height mismatch");
                dbData.PalletUnitsPerLayer.Should().Be(excelTable.Rows[index]["Pallet Units / Layer"].RoundValue(), "Pallet Units / Layer mismatch");
                dbData.PalletLayerPerPallet.Should().Be(excelTable.Rows[index]["Pallet Layer / Pallet"].RoundValue(), "Pallet Layer / Pallet mismatch");
                dbData.PalletUnitsPerPallet.Should().Be(excelTable.Rows[index]["Pallet Units / Pallet"].RoundValue(), "Pallet Units / Pallet mismatch");
            }
        }

        public void VerifyAuditHistoryRecordsForPackageUpload(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.WaitForElementDisplayed(TimeSpan.FromSeconds(10));
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message);

            b2BCatalogPackagingDataUploadPage.AuditHistoryTable.WaitForTableLoadComplete(60);
            IReadOnlyCollection<IWebElement> historyRows = b2BCatalogPackagingDataUploadPage.AuditHistoryRecords;
            historyRows.Count.Should().BeInRange(1, 13);
            Console.WriteLine(historyRows.Count);

            IReadOnlyCollection<IWebElement> latestRowValues = b2BCatalogPackagingDataUploadPage.GetAuditHistoryRowValues(historyRows.ElementAt(0));
            latestRowValues.ElementAt(0).Text.Should().Be(fileToUpload);
            latestRowValues.ElementAt(1).Text.Should().Be(Environment.UserName);
            Convert.ToDateTime(latestRowValues.ElementAt(2).Text).Should().BeAfter(timeBeforeUpload);
        }

        public void VerifyPackageUploadForNullAndInvalidValues(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string errorMessage)
        {
            b2BHomePage = new B2BHomePage(webDriver);
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Should().Be(errorMessage);
        }

        public void VerifyNewFieldsPackageUploadProd(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.WaitForElementDisplayed(TimeSpan.FromSeconds(10));
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message);

            string excelQuery = @"select [Order code],LOB,[Config Name],ROUND([Ship Weight],0) as [Ship Weight],ROUND([Package Length],0) as [Package Length],ROUND([Package Width],0) as [Package Width],ROUND([Package Height],0) as [Package Height],ROUND([Pallet Length],0) as [Pallet Length],ROUND([Pallet Width],0) as [Pallet Width],ROUND([Pallet Height],0) as [Pallet Height],ROUND([Pallet Units / Layer],0) as [Pallet Units / Layer],ROUND([Pallet Layer / Pallet],0) as [Pallet Layer / Pallet],ROUND([Pallet Units / Pallet],0) as [Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$]";
            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            Channel_Catalog_PackagingData dbData = ChannelCatalogProdDataAccess.GetPackagingDetails(excelTable.Rows[0]["Order code"].ToString());

            dbData.PalletLength.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Length"]));
            dbData.PalletWidth.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Width"]));
            dbData.PalletHeight.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Height"]));
            dbData.PalletUnitsPerLayer.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Layer"]));
            dbData.PalletLayerPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Layer / Pallet"]));
            dbData.PalletUnitsPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Pallet"]));
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking Published status in Auto CatalogList page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingPublishedcheckboxinAutoCatalogListPage(string environment, string statusDropdown)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));
            b2BAutoCatalogListPage.SelectTheStatus(statusDropdown);
            b2BAutoCatalogListPage.SearchCatalogLink.Click(); webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            var firststatusElement = b2BAutoCatalogListPage.CatalogListTableRows.FirstOrDefault().FindElements(By.TagName("td"))[3];
            var Status = firststatusElement.Text;
            if (!Status.Equals(statusDropdown))
            {
                return false;
            }
            return true;
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking Published status and std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingPublishedandStdConfigcheckboxinAutoCatalogListPage(string environment,
            string statusDropdown)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.SelectTheStatus(statusDropdown);
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return (b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected);
        }

        ///<summary>
        /// Verifies Original/Delta catalog on clicking std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingStdconfigcheckboxinAutoCatalogListPage(string environment)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected;
        }
        ///<summary>
        /// Verifies Original/Delta catalog on clicking Test Harness checkbox and std config in Auto Catalog List page.
        /// </summary>
        public bool VerifyOriginalDeltaCatonclickingTestHarnessandStdConfigcheckboxinAutoCatalogListPage(string environment,
            string testHarnesscreated, string testHarnessFailed)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogListPageLink.Click();
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            b2BAutoCatalogListPage = new B2BAutoCatalogListPage(webDriver);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            b2BAutoCatalogListPage.TestHarnessCheckbox.Click();
            b2BAutoCatalogListPage.StdConfigTypeCheckbox.Click();
            return (b2BAutoCatalogListPage.TestHarnessCheckbox.Selected && b2BAutoCatalogListPage.StdConfigTypeCheckbox.Selected);
        }
        public void VerifyNewFieldsPackageUploadPrev(ConstantObjects.B2BEnvironment b2BEnvironment, string fileToUpload, string message)
        {
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.OpenPackageUploadPage();

            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);
            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            b2BCatalogPackagingDataUploadPage.UploadExcelFile(fileToUpload);
            b2BCatalogPackagingDataUploadPage.UploadMessage.WaitForElementDisplayed(TimeSpan.FromSeconds(10));
            b2BCatalogPackagingDataUploadPage.UploadMessage.Text.Trim().Equals(message);

            string excelQuery = @"select [Order code],LOB,[Config Name],ROUND([Ship Weight],0) as [Ship Weight],ROUND([Package Length],0) as [Package Length],ROUND([Package Width],0) as [Package Width],ROUND([Package Height],0) as [Package Height],ROUND([Pallet Length],0) as [Pallet Length],ROUND([Pallet Width],0) as [Pallet Width],ROUND([Pallet Height],0) as [Pallet Height],ROUND([Pallet Units / Layer],0) as [Pallet Units / Layer],ROUND([Pallet Layer / Pallet],0) as [Pallet Layer / Pallet],ROUND([Pallet Units / Pallet],0) as [Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$]";
            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            Channel_Catalog_PackagingData dbData = ChannelCatalogPrevDataAccess.GetPackagingDetails(excelTable.Rows[0]["Order code"].ToString());

            dbData.PalletLength.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Length"]));
            dbData.PalletWidth.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Width"]));
            dbData.PalletHeight.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Height"]));
            dbData.PalletUnitsPerLayer.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Layer"]));
            dbData.PalletLayerPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Layer / Pallet"]));
            dbData.PalletUnitsPerPallet.Should().Be(Convert.ToInt32(excelTable.Rows[0]["Pallet Units / Pallet"]));
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
