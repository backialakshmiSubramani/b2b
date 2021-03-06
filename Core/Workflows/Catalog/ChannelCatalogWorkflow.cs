﻿using System;
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
using Modules.Channel.Utilities;
using System.Security.Principal;
using System.IO;
using System.Text.RegularExpressions;
using System.ServiceModel;
using Modules.Channel.ATSServiceReferenceForSTDandSYS;
using System.Net;
using System.Text;
using Modules.Channel.B2B.CatalogXMLTemplates;

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
        private IJavaScriptExecutor javaScriptExecutor;
        private const string MMDDYYYY = "MM/dd/yyyy";
        private int HeaderRowsCount = 0, Headercount = 0, subHeaderRows = 0;
        private string windowsLogin;
        /// <summary>
        /// Constructor for ChannelCatalogWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelCatalogWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            b2BHomePage = new B2BHomePage(webDriver);
            this.webDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(120));
            windowsLogin = WindowsIdentity.GetCurrent().Name.Split('\\')[1].ToLowerInvariant();
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
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName.ToUpper());
            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("Opened Profile Page for profile: {0}", profileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Searches for the profile provided in B2B Profile List page and 
        /// navigates to the General Catalog Tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        public void GoToGeneralTab(string environment, string profileName)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.ClickB2BProfileList();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", profileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName.ToUpper());
            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("Opened Profile Page for profile: {0}", profileName);
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
            UtilityMethods.ClickElement(webDriver, b2BHomePage.B2BProfileListLink);
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BCustomerProfileListPage.CreateNewProfileLink);
            b2BProfileSettingsGeneralPage = new B2BProfileSettingsGeneralPage(webDriver);
            b2BProfileSettingsGeneralPage.EnterUserId(newProfileName);
            b2BProfileSettingsGeneralPage.EnterCustomerName(newProfileName);
            b2BProfileSettingsGeneralPage.EnterIdentityName(newProfileName);
            b2BProfileSettingsGeneralPage.EnterCustomerSet(customerSet);
            UtilityMethods.ClickElement(webDriver, b2BProfileSettingsGeneralPage.SearchLink);
            if (b2BProfileSettingsGeneralPage.SelectAccessGroupMsgDisplayed())
            {
                webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
                b2BProfileSettingsGeneralPage.EnterAccessGroup(accessGroup);
                UtilityMethods.ClickElement(webDriver, b2BProfileSettingsGeneralPage.CreateNewProfileButton);
            }
            else
            {
                throw new ElementNotVisibleException();
            }
            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("New profile created with Name: {0}", newProfileName);
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
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
                Thread.Sleep(10000);
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();

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
            var originalTimeOfSend = (centralTime.Hour + 2).ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(originalTimeOfSend);
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
            //var originalTimeOfSend = centralTime.Hour.ToString();
            originalTimeOfSend = (centralTime.Hour + 2).ToString();
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
            //b2BBuyerCatalogPage.CatalogConfigSnP.Click();
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

            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
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
        public bool VerifyThreadIdLinkInAutoCatalogListPage(string environment, string region, string country, CatalogStatus status, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
        public bool VerifyThreadIdLinkInAutoCatalogListPage(string environment, string profilename, string region, string country, CatalogStatus status, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            b2BHomePage.SelectEnvironment(environment);
            b2BHomePage.AutoCatalogInventoryListPageLink.Click();
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            webDriver.SwitchTo().Window(webDriver.WindowHandles.LastOrDefault());
            WaitForPageRefresh();
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.SelectTheCountry(country);
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename);
            b2BAutoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            b2BAutoCatalogListPage.SearchRecordsLink.Click();
            b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var Status = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Status");
            //var countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country\r\nCode");
            //var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region");
            //var Currencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency\r\nCode");
            var countryCode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Country").Trim();
            var regioncode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Region").Trim();
            var Currencycode = b2BAutoCatalogListPage.CatalogsTable.GetCellValue(1, "Currency").Trim();
            Console.WriteLine(Status + ", " + countryCode + " ," + regioncode + ", " + currencyCode);
            if (Status.Equals(status.ToString(), StringComparison.InvariantCultureIgnoreCase) && countryCode.Equals(CountryCode, StringComparison.InvariantCultureIgnoreCase) && regioncode.Equals(region, StringComparison.InvariantCultureIgnoreCase) && Currencycode.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;

        }

        ///<summary>
        /// Verified country region and currency fields for scheduled catalogs
        /// </summary>
        /// <returns></returns>
        public bool VerifyCountryCodeScheduledInAutoCatalogListPage(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status, string regionName, string countryName)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            if (status == CatalogStatus.Scheduled)
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
        public bool VerifyCountryCodeFailedInAutoCatalogListPage(B2BEnvironment b2BEnvironment, string regionName, string countryName, CatalogType type, CatalogStatus status)
        {
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(regionName);
            b2BAutoCatalogListPage.SelectTheCountry(countryName);
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            if (countryCode.Equals(countrycode, StringComparison.InvariantCultureIgnoreCase) && regioncode.Equals(regionCode) && cUrrencycode.Equals(currencycode, StringComparison.InvariantCultureIgnoreCase) && Status.Equals(status))
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Delta)
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

        public void VerifyCatalogDownload(B2BEnvironment environment, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, RequestorValidation requestor)
        {
            DateTime beforeSchedTime = DateTime.Now.AddDays(-90);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(environment);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime, requestor);
            string result = uxWorkflow.DownloadCatalogResponse(environment);
            //string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            string schemaPath = Path.Combine(System.Environment.CurrentDirectory, "CatalogSchema.xsd");
            //string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            string message = XMLSchemaValidator.ValidateSchemaNew(result, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            switch (type)
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
            b2BAutoCatalogListPage.ThreadId.SendKeys(profilename); b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.ThreadId.SendKeys(profile); b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.SelectTheCountry(country); b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheCustomer(profile);
            b2BAutoCatalogListPage.SelectTheIdentity(identity);
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
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
                b2BAutoCatalogListPage.SelectTheIdentity(identity); b2BAutoCatalogListPage.CatalogRadioButton.Click();
                b2BAutoCatalogListPage.SearchRecordsLink.Click();
                b2BAutoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
                catalogName = uxWorkflow.DownloadCatalog(identity, beforeSchedTime);
                catalogName = catalogName.Substring(catalogName.LastIndexOf("\\") + 1);
                catalogName = catalogName.Remove(catalogName.IndexOf('.'), 4);
                if (catalogName.Contains('('))
                {
                    catalogName = catalogName.Split(' ')[0];
                }
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
            Console.WriteLine(cOuntryCode + "," + regioncode);
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();

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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.UpdateButton);
                WaitForPageRefresh();
                return true;
            }
            else if (!b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected)
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.UpdateButton);
                WaitForPageRefresh();
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
            b2BAutoCatalogListPage.SelectOption(b2BAutoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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

            for (int i = 1; i < b2BAutoCatalogListPage.CatalogListTableRows.Count; i++)
            {
                for (int j = 0; j < b2BAutoCatalogListPage.StatusTable.Count; j++)
                {
                    if (b2BAutoCatalogListPage.CatalogsTable.GetCellValue(i, "Status").Equals(b2BAutoCatalogListPage.StatusTable[j].FindElements(By.TagName("td"))[0].Text))
                        break;

                    if(j == b2BAutoCatalogListPage.StatusTable.Count - 1)
                        return false;
                }
            }
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


            DateTime currentCstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            string originalOldHrs = (currentCstTime.Hour + 1).ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(originalOldHrs);
            var oldValue1 = b2BBuyerCatalogPage.OriginalTimeOfSend.GetAttribute("value").ToString();
            oldValue = "0" + oldValue1 + ":00:00";
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            string deltaOldHrs = (currentCstTime.Hour + 2).ToString();
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue(deltaOldHrs);

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

            string originalNewHrs = (currentCstTime.Hour + 3).ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(originalNewHrs);

            var newValue1 = b2BBuyerCatalogPage.OriginalTimeOfSend.GetAttribute("value").ToString();

            newValue = "0" + newValue1 + ":00:00";
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            string deltaNewHrs = (currentCstTime.Hour + 4).ToString();
            b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectByValue(deltaNewHrs);

            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();

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
            DateTime currentCstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            string hrs = (currentCstTime.Hour + 1).ToString();
            b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectByValue(hrs);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.UpdateButton);
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
        public bool VerifyRetrieveCatalogConfigAquoteId_Old(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, string Header, string SubHeader, CatalogType type, CatalogStatus status, string profile, string identity)
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
            string quoteid = SubRow1StringValue[4].Substring(4, quoteidlength - 4);

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

                if (subHeaderTable1fromLocator.Equals(subHeaderTestdata) && subRowTable1fromLocator.Equals(subRow1Testdata))
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
            if (Headercount.Equals(8) && HeaderRowsCount.Equals(8) && subHeaderRows.Equals(9))
            {
                return true;
            }
            Console.WriteLine("Headercount: " + Headercount + ",HeaderRowsCount: " + HeaderRowsCount + ",subHeaderRows: " + subHeaderRows);
            return false;
        }

        /// <summary>
        /// Retrieve Delta Published Auto BHC Config Quote ID thru Part Viewer. Verify all required info in the table
        /// </summary>
        /// <returns></returns>
        public bool VerifyRetrieveCatalogConfigAquoteId(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, string Header, string SubHeader, CatalogType type, CatalogStatus status, string profile, string identity)
        {
            Dictionary<int, string> dict = GetPartViewerInformation(b2BEnvironment, catalogItemType, region, country, type, status, profile, identity);
            bool matchFlag = true;
            dict.Count().ShouldBeEquivalentTo(2, "**** " + catalogItemType[0] + " Items are not found in catalog ****");
            var Headervalue = dict[1];
            var SubRowvalue1 = dict[2];

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPartViewerPage(b2BEnvironment);

            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);

            string[] HeaderRowStringValue = Header.Split(',');
            string[] SubHeaderStringValue = SubHeader.Split(',');
            string[] HeaderStringvalue = Headervalue.ToString().Split(',');
            string[] SubRow1StringValue = SubRowvalue1.ToString().Split(new string[] { "$" }, StringSplitOptions.None);

            int quoteidlength = SubRow1StringValue[5].Length;
            string quoteid = SubRow1StringValue[5].Substring(4, quoteidlength - 4);

            b2BAutoCatalogListPage.PartViewerQuoteIdsLink.SendKeys(quoteid);
            b2BAutoCatalogListPage.PartViewerSearchButton.Click();
            b2BAutoCatalogListPage.PartViewerPlusButton.WaitForElementVisible(TimeSpan.FromSeconds(120));
            b2BAutoCatalogListPage.PartViewerPlusButton.Click();

            string TableXpath_First = "//*[@id='quoteTable']";
            string Table1FirstRow_End = "/table/tbody/tr[1]/td";
            string Table1SubHeadingEnd = "/table/tbody/tr[2]/td[2]/table/thead/tr/th";
            string Table1SubRow_End = "/table/tbody/tr[2]/td[2]/table/tbody/tr/td";
            for (int j = 0; j < SubHeaderStringValue.Length; j++)
            {
                var SubHeaderElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubHeadingEnd))[j];
                var subHeaderTable1fromLocator = SubHeaderElement.Text;
                var SubRowElemt = webDriver.FindElements(By.XPath(TableXpath_First + Table1SubRow_End))[j];
                //var subRowTable1fromLocator = SubRowElemt.Text;
                var subHeaderTestdata = SubHeaderStringValue[j];
                var subRow1Testdata = SubRow1StringValue[j];

                matchFlag &= UtilityMethods.CompareValues<string>(SubHeaderStringValue[j], SubHeaderElement.Text, SubHeaderStringValue[j]);
                if (SubHeaderStringValue[j].Equals("Change Type"))
                    matchFlag &= UtilityMethods.CompareValues<string>(SubHeaderStringValue[j], SubRowElemt.Text, SubRow1StringValue[j].Trim() == "NoChange" ? "NC" : SubRow1StringValue[j].Trim().Substring(0, 1));
                else
                    matchFlag &= UtilityMethods.CompareValues<string>(SubHeaderStringValue[j], SubRowElemt.Text, SubRow1StringValue[j].Replace("  ", " "));
            }
            //Header
            for (int i = 0; i < HeaderRowStringValue.Length; i++)
            {
                var HeaderElement = b2BAutoCatalogListPage.PartViewerHeader.FirstOrDefault().FindElements(By.TagName("th"))[i + 1];
                var HeaderTextfromLocator = HeaderElement.Text;
                var HeadTestdata = HeaderRowStringValue[i];
                matchFlag &= UtilityMethods.CompareValues<string>(HeaderRowStringValue[i], HeaderElement.Text, HeaderRowStringValue[i]);
            }
            //HeaderRows
            for (int z = 1; z < HeaderStringvalue.Length; z++)
            {
                var HeaderRowElement = webDriver.FindElements(By.XPath(TableXpath_First + Table1FirstRow_End))[z];
                var HeaderRowTextfromLocator = HeaderRowElement.Text;
                var HeaderRowTestData = HeaderStringvalue[z];
                if (!string.IsNullOrEmpty(HeaderRowElement.Text) && !HeaderRowElement.Text.ToUpperInvariant().Contains("@DELL.COM"))
                    matchFlag &= UtilityMethods.CompareValues<string>(HeaderRowStringValue[z - 1], HeaderRowElement.Text, HeaderStringvalue[z]);
            }
            return matchFlag;
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
            if (b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled)
            {
                b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogEndDate, endDate);
            }
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
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AuditHistoryLink);
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
        /// Validates an Audit History entry base on provided property, old value & new value
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="auditHistoryProperty"></param>
        /// <returns></returns>
        private bool VerifyAuditHistoryRow(Dictionary<string, string> oldValues, Dictionary<string, string> newValues)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AuditHistoryLink);
            WaitForPageRefresh();
            List<string> fieldNames = b2BBuyerCatalogPage.AuditHistoryRows.Select(r => r.FindElements(By.TagName("td"))[0].Text).ToList();
            bool matchFlag = true;
            for (int i = 1; i < fieldNames.Count; i++)
            {
                if (fieldNames[i].Contains("Edited"))
                    break;
                var auditHistoryRow = b2BBuyerCatalogPage.AuditHistoryRows.FirstOrDefault(r => r.FindElements(By.TagName("td"))[0].Text.Contains(fieldNames[i].ToString()));
                var oldv = auditHistoryRow.FindElements(By.TagName("td"))[1].Text;

                var newv = auditHistoryRow.FindElements(By.TagName("td"))[2].Text;

                var oldValue = oldValues[fieldNames[i].ToString()];
                var newValue = newValues[fieldNames[i].ToString()];
                matchFlag &= (UtilityMethods.CompareValues<string>(fieldNames[i].ToString(), oldValue.ToUpperInvariant(), oldv.ToUpperInvariant()));
                matchFlag &= (UtilityMethods.CompareValues<string>(fieldNames[i].ToString(), newValue.ToUpperInvariant(), newv.ToUpperInvariant()));
            }
            return matchFlag;
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
                WaitForPageRefresh();
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
                WaitForPageRefresh();
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
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days);
            newValue = b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected.ToString();
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.UpdateButton);
            return VerifyAuditHistoryRow(oldValue, newValue, removeItemswithLtAuditHistoryProperty);

        }

        public void VerifyDefaultValueForExpiryDaysForSPL(B2BEnvironment b2BEnvironment, string profileName)
        {
            B2BHomePage b2BHomePage = new B2BHomePage(webDriver);
            b2BHomePage.OpenB2BHomePage(b2BEnvironment);

            GoToBuyerCatalogTab(b2BEnvironment.ToString(), profileName);

            string currentExpiryDays = b2BBuyerCatalogPage.CatalogExpireInDays.SelectedOption.Text.ToString();

            b2BBuyerCatalogPage.EditScheduleButton.Click();

            if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();

            b2BBuyerCatalogPage.CatalogExpireInDays.SelectedOption.Text.Should().Be("30", "Expiry days is not 30 when SPL is selected");

            if (b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();

            b2BBuyerCatalogPage.CatalogExpireInDays.SelectedOption.Text.Should().Be(currentExpiryDays, "Expiry days is not retverted back to " + currentExpiryDays + " when SPL is de-selected");
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
            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));

            b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            }

            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
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

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));

            b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
                    b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            }

            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
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

            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate,
                    DateTime.Now.AddDays(2).ToString(MMDDYYYY));

            b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                    b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
            }

            newValue = b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected.ToString();
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
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
                WaitForPageRefresh();
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
            WaitForPageRefresh();
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
            if (b2BBuyerCatalogPage.EditScheduleButton.Enabled)
            {
                b2BBuyerCatalogPage.EditScheduleButton.Click();
            }
            if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            }


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

            if (b2BBuyerCatalogPage.EditScheduleButton.Enabled)
            {
                b2BBuyerCatalogPage.EditScheduleButton.Click();
            }
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
            }
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
            WaitForPageRefresh();
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

        public bool VerifyLeadTimeAndInventoryByItemType(B2BEnvironment b2BEnvironment, CatalogItemType catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUXPage = new B2BChannelUx(webDriver);
            b2BChannelUXPage.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            List<CatalogItem> catalogItemList = null;
            CatalogItem catalogItem = null;
            SKUDetailsFromATS sKUDetailsFromATS;
            bool itemFound = false;
            bool errorFlag = false;

            if (catalogItemType == CatalogItemType.SNP)
            {
                // Get Dell Branded SnP SKU's
                Regex regex = new Regex(@"^\d{3}-", RegexOptions.IgnoreCase);
                catalogItem = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == CatalogItemType.SNP && regex.Match(ci.BaseSKUId).Success).FirstOrDefault();
                itemFound = UtilityMethods.CheckForNotNull(catalogItem, string.Format("Dell Branded SNP SKU not found in catalog {0}", actualCatalogHeader.CatalogName));
                sKUDetailsFromATS = GetLeadTimeAndInventoryForSNP("US", catalogItem.BaseSKUId);
                if (itemFound)
                {
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.LeadTime, Constant.DefaultLeadTime, string.Format("Lead time validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                        catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.LeadTime, Constant.DefaultLeadTime));
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                        catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                }
                errorFlag &= !itemFound;

                // Get Non Dell Branded SnP SKU's
                catalogItem = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == CatalogItemType.SNP && !regex.Match(ci.BaseSKUId).Success).FirstOrDefault();
                itemFound = UtilityMethods.CheckForNotNull(catalogItem, string.Format("Non Dell Branded SNP SKU not found in catalog {0}", actualCatalogHeader.CatalogName));
                sKUDetailsFromATS = GetLeadTimeAndInventoryForSNP("US", catalogItem.BaseSKUId);
                if (itemFound)
                {
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.LeadTime, Constant.DefaultLeadTime, string.Format("Lead time validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                        catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.LeadTime, Constant.DefaultLeadTime));
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                        catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                }
                errorFlag &= !itemFound;
            }
            else if (catalogItemType == CatalogItemType.ConfigWithDefaultOptions || catalogItemType == CatalogItemType.Systems)
            {
                // Get BTO order code
                catalogItem = actualCatalogDetails.CatalogItem.Where(ci => ci.ItemType == ProductType.BTO.ConvertToString()).FirstOrDefault();
                itemFound = UtilityMethods.CheckForNotNull(catalogItem, string.Format("Catalog {0} does not have BTO catalog item", actualCatalogHeader.CatalogName));
                sKUDetailsFromATS = GetLeadTimeAndInventoryForSTDandSYS(catalogItem.ItemSKUinfo, ProductType.BTO);
                if (itemFound)
                {
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.LeadTime, Constant.DefaultLeadTime, string.Format("Lead time validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                        catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.LeadTime, Constant.DefaultLeadTime));
                    errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                      catalogItem.ItemOrderCode, catalogItem.ItemType, catalogItem.ItemSKUinfo, catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                }
                errorFlag &= !itemFound;

                catalogItemList = actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == catalogItemType && ci.ItemType == ProductType.BTS.ConvertToString()).ToList();

                // Get BTS order code not tracked at ATS
                foreach (CatalogItem ci in catalogItemList)
                {
                    sKUDetailsFromATS = GetLeadTimeAndInventoryForSTDandSYS(ci.ItemSKUinfo, ProductType.BTS);
                    if (!sKUDetailsFromATS.IsTracked)
                    {
                        errorFlag &= UtilityMethods.CompareValuesAndLogError(ci.LeadTime, Constant.DefaultLeadTime, string.Format("Lead time validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                            ci.ItemOrderCode, ci.ItemType, ci.ItemSKUinfo, ci.LeadTime, Constant.DefaultLeadTime));
                        errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(ci.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                            ci.ItemOrderCode, ci.ItemType, ci.ItemSKUinfo, ci.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                        itemFound = true;
                        break;
                    }
                }
                errorFlag &= UtilityMethods.CompareValuesAndLogError<bool>(itemFound, true, string.Format("BTS order code not tracked in ATS is not found in catalog {0}", actualCatalogHeader.CatalogName));

                // Get BTS order code tracked at ATS
                itemFound = false;
                foreach (CatalogItem ci in catalogItemList)
                {
                    sKUDetailsFromATS = GetLeadTimeAndInventoryForSTDandSYS(ci.ItemSKUinfo, ProductType.BTS);
                    if (sKUDetailsFromATS.IsTracked)
                    {
                        errorFlag &= UtilityMethods.CompareValuesAndLogError(ci.LeadTime, sKUDetailsFromATS.LeadTime, string.Format("Lead time validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                            ci.ItemOrderCode, ci.ItemType, ci.ItemSKUinfo, ci.LeadTime, sKUDetailsFromATS.LeadTime));
                        errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(ci.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1} for SKU {2}. Actual: {3}, Expected: {4}",
                            ci.ItemOrderCode, ci.ItemType, ci.ItemSKUinfo, ci.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                        itemFound = true;
                        break;
                    }
                }
                errorFlag &= UtilityMethods.CompareValuesAndLogError(itemFound, true, string.Format("BTS order code tracked in ATS is not found in catalog {0}", actualCatalogHeader.CatalogName));

                if (catalogItemType == CatalogItemType.ConfigWithDefaultOptions)
                {
                    // Get DWC order code
                    catalogItem = actualCatalogDetails.CatalogItem.Where(ci => ci.BaseSKUId == ProductType.DWC.ConvertToString()).FirstOrDefault();
                    itemFound = UtilityMethods.CheckForNotNull(catalogItem, string.Format("Catalog {0} does not have DWC catalog item", actualCatalogHeader.CatalogName));
                    sKUDetailsFromATS = GetLeadTimeAndInventoryForSTDandSYS(catalogItem.ItemSKUinfo, ProductType.DWC);
                    if (itemFound)
                    {
                        errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.LeadTime, Constant.DefaultLeadTime, string.Format("Lead time validation failed for {0} of type {1}. Actual: {2}, Expected: {3}",
                            catalogItem.ItemOrderCode, catalogItem.BaseSKUId, catalogItem.LeadTime, Constant.DefaultLeadTime));
                        errorFlag &= UtilityMethods.CompareValuesAndLogError<int>(catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty, string.Format("Inventory Quantity validation failed for {0} of type {1}. Actual: {2}, Expected: {3}",
                            catalogItem.ItemOrderCode, catalogItem.BaseSKUId, catalogItem.InventoryQty, sKUDetailsFromATS.IsTracked ? sKUDetailsFromATS.InventoryQty : Constant.DefaultInventoryQty));
                    }
                    errorFlag &= !itemFound;
                }
            }

            return errorFlag;
        }

        public SKUDetailsFromATS GetLeadTimeAndInventoryForSTDandSYS(string skuId, ProductType productType)
        {
            string url = ConfigurationManager.AppSettings["ATSServiceURLForSTDandSYS"];

            var remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(url));

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.MaxBufferSize = 2147483647;
            basicHttpBinding.MaxReceivedMessageSize = 2147483647;

            AvailableToSellBySKUResponse availableToSellBySKUResponse = null;

            using (var inventoryServiceClient = new InventoryServiceClient(basicHttpBinding, remoteAddress))
            {
                AvailableToSellBySKURequest availableToSellBySKURequest = new AvailableToSellBySKURequest();
                availableToSellBySKURequest.SKU = skuId;
                availableToSellBySKURequest.SalesChannel = SalesChannelOptions.Online;
                availableToSellBySKURequest.LightWeightProfile = new LightWeightProfile { CountryCode = "US", LanguageCode = "EN" };

                availableToSellBySKUResponse = inventoryServiceClient.AvailableToSellBySKU(availableToSellBySKURequest);
            }

            SKUDetailsFromATS sKUDetailsFromATS = new SKUDetailsFromATS();
            if (string.IsNullOrWhiteSpace(availableToSellBySKUResponse.ProductATS.InventoryStatus))
            {
                sKUDetailsFromATS.IsTracked = false;
                sKUDetailsFromATS.LeadTime = Constant.DefaultLeadTime;
            }
            else
            {
                sKUDetailsFromATS.IsTracked = true;
                sKUDetailsFromATS.InventoryQty = availableToSellBySKUResponse.ProductATS.AvailableToSell;
                sKUDetailsFromATS.LeadTime = availableToSellBySKUResponse.ProductATS.LeadTime;
            }

            return sKUDetailsFromATS;
        }

        public SKUDetailsFromATS GetLeadTimeAndInventoryForSNP(string countryCode, string SkuId)
        {
            WebRequest request = WebRequest.Create(ConfigurationReader.GetValue("ATSServiceURLForSNP"));
            request.Method = "POST";
            string postData = "{\"CountryCode\":\"" + countryCode + "\", \"SkuList\": [{\"Sku\":\"" + SkuId + "\"}]}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            ATSResponseSNP responseObj = JsonHelper.DeserializeJsonObject<ATSResponseSNP>(responseFromServer);
            SkuList skuList = responseObj.SkuList.Where(sk => sk.Sku == SkuId).FirstOrDefault();
            skuList.Should().NotBeNull(string.Format("No data returned from ATS service for SKU {0}", SkuId));

            SKUDetailsFromATS sKUDetailsFromATS = new SKUDetailsFromATS();
            sKUDetailsFromATS.LeadTime = Convert.ToInt32(skuList.LeadTime);
            sKUDetailsFromATS.InventoryQty = Convert.ToInt32(skuList.Inventory);
            reader.Close();
            dataStream.Close();
            response.Close();

            return sKUDetailsFromATS;
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
            DateTime timeBeforeUpload = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoPackageUploadPage(b2BEnvironment);
            b2BCatalogPackagingDataUploadPage = new B2BCatalogPackagingDataUploadPage(webDriver);

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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName.ToUpper());
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
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
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
           DefaultOptions defaultOptions = DefaultOptions.Off, CRTStatus crtStatus = CRTStatus.OFF)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            //string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            string catalogResponse = uxWorkflow.DownloadCatalogResponse(b2BEnvironment);
            //uxWorkflow.ValidateCatalogXML(catalogItemType, catalogType, identityName, filePath, beforeSchedTime, configRules).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
            uxWorkflow.ValidateCatalogXMLNew(catalogItemType, catalogType, identityName, catalogResponse, beforeSchedTime, configRules).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");

            //if (crtStatus == CRTStatus.ON)
            //    uxWorkflow.ValidateCRT(b2BEnvironment, profileName, filePath).Should().BeTrue("Error: Data mismatch for CRT XML content with Catalog XML");
            if (crtStatus == CRTStatus.ON)
                uxWorkflow.ValidateCRTNew(b2BEnvironment, profileName, catalogResponse).Should().BeTrue("Error: Data mismatch for CRT XML content with Catalog XML");
        }

        public void VerifySetNewCatalogForConfig(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType,
            ConfigRules configRules = ConfigRules.None,
            DefaultOptions defaultOptions = DefaultOptions.Off)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType, catalogItemType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            uxWorkflow.ValidateCatalogXML(catalogItemType, catalogType, identityName, filePath, beforeSchedTime, configRules).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
        }

        public void VerifyCatalogDetails(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, DeltaChange[] deltaChanges)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            uxWorkflow.ValidateDeltaCatalog(catalogItemType, catalogType, identityName, filePath, beforeSchedTime, deltaChanges).Should().BeTrue("Error: Data mismatch for Delta Catalog XML content with expected values");
            //uxWorkflow.ValidateCatalogEMails(identityName, beforeSchedTime, operation);
        }

        public void GenerateAndValidateCatalog(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);
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

        public void VerifyCatalogFailureForNullMPN(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogType catalogType)
        {
            B2BHomePage b2BHomePage = new B2BHomePage(webDriver);
            b2BHomePage.OpenB2BHomePage(b2BEnvironment);
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, CatalogStatus.Failed);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, CatalogStatus.Failed, beforeSchedTime);
            string threadID = uxWorkflow.GetThreadID(catalogItemType, catalogType, CatalogStatus.Failed, beforeSchedTime);
            ChannelCatalogWorkflow channelCatalogWorkflow = new ChannelCatalogWorkflow(webDriver);
            channelCatalogWorkflow.VerifyErrorMessageInLogReport(b2BEnvironment, threadID, "Unexpected Error: Auto Catalog creation failed", "failed due to ManufacturerPartNumber being null or empty").Should().BeTrue("Incorrect error message displayed in log report");
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

        public void ValidatePackagingInformation(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, List<string> mfgNumbers, ConfigRules configRules = ConfigRules.None)
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

            foreach (string mfgNumber in mfgNumbers)
            {
                CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ManufacturerPartNumber.ToString() == mfgNumber).FirstOrDefault();

                string excelQuery = @"select [Region],[Country],[MPN],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$] where [MPN] = '" + mfgNumber + "'";
                DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

                actualCatalogItem.PackageLength.Should().Be(excelTable.Rows[0]["Package Length"].ToString(), "Package Length mismatch");
                actualCatalogItem.PackageWidth.Should().Be(excelTable.Rows[0]["Package Width"].ToString(), "Package Width mismatch");
                actualCatalogItem.PackageHeight.Should().Be(excelTable.Rows[0]["Package Height"].ToString(), "Package Height mismatch");
                actualCatalogItem.PalletHeight.Should().Be(excelTable.Rows[0]["Pallet Height"].RoundValue().ToString(), "Pallet Height mismatch");
                actualCatalogItem.PalletLength.Should().Be(excelTable.Rows[0]["Pallet Length"].RoundValue().ToString(), "Pallet Length mismatch");
                actualCatalogItem.PalletWidth.Should().Be(excelTable.Rows[0]["Pallet Width"].RoundValue().ToString(), "Pallet Width mismatch");
                actualCatalogItem.PalletUnitsPerLayer.Should().Be(excelTable.Rows[0]["Pallet Units / Layer"].RoundValue().ToString(), "Pallet Units / Layer mismatch");
                actualCatalogItem.PalletLayerPerPallet.Should().Be(excelTable.Rows[0]["Pallet Layer / Pallet"].RoundValue().ToString(), "Pallet Layer / Pallet mismatch");
                actualCatalogItem.PalletUnitsPerPallet.Should().Be(excelTable.Rows[0]["Pallet Units / Pallet"].RoundValue().ToString(), "Pallet Units / Pallet mismatch");
            }
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
            //string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalogResponse(b2BEnvironment);
            //B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            B2BXML actualCatalog = XMLDeserializer<B2BXML>.DeserializeFromXmlString(filePath);

            CatalogItem actualCatalogItem = actualCatalog.BuyerCatalog.CatalogDetails.CatalogItem.Where(ci => ci.ManufacturerPartNumber.ToString() == mfgNumber).FirstOrDefault();

            string excelQuery = @"select [Region],[Country],[MPN],LOB,[Config Name],[Ship Weight],[Package Length],[Package Width],[Package Height],[Pallet Length],[Pallet Width],[Pallet Height],[Pallet Units / Layer],[Pallet Layer / Pallet],[Pallet Units / Pallet] FROM [B2B_Catalog_matching_table$] where [MPN] = '" + mfgNumber + "'";
            DataTable excelTable = UtilityMethods.GetDataFromExcel(@"PackagingData.xlsx", excelQuery);

            actualCatalogItem.PalletLength.Should().Be(excelTable.Rows[0]["Pallet Length"].RoundValue().ToString(), "Pallet Length not found");
            actualCatalogItem.PalletWidth.Should().Be(excelTable.Rows[0]["Pallet Width"].RoundValue().ToString(), "Pallet Width not found");
            actualCatalogItem.PalletHeight.Should().Be(excelTable.Rows[0]["Pallet Height"].RoundValue().ToString(), "Pallet Height not found");
            actualCatalogItem.PalletUnitsPerLayer.Should().Be(excelTable.Rows[0]["Pallet Units / Layer"].RoundValue().ToString(), "Pallet Units / Layer not found");
            actualCatalogItem.PalletLayerPerPallet.Should().Be(excelTable.Rows[0]["Pallet Layer / Pallet"].RoundValue().ToString(), "Pallet Layer / Pallet not found");
            actualCatalogItem.PalletUnitsPerPallet.Should().Be(excelTable.Rows[0]["Pallet Units / Pallet"].RoundValue().ToString(), "Pallet Units / Pallet not found");

        }

        /// <summary>
        /// Below method verifies whether the BTS Order code exists or not in a Catlog file while Lead time > 3in BHC section
        /// If BTS Order code not exists then it retuns True
        /// </summary>
        public void VerifyLeadTimeGreaterThanThreeBTSOrderCodesNotExistsInCatalog(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string itemOrderCode, ConfigRules configRules)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);

            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            uxWorkflow.VerifyOrderCodeExistsInCatalogFile(filePath, catalogItemType, configRules).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
        }


        /// <summary>
        /// It Verify the SPL with SYS Config, SNP Config, SNP Auto CRT & SYS Auto CRT UI settings in B2B Profile->Buyer Catalog page, under under "Automated BHC Catalog-Processing Rules" section
        /// </summary>

        /// <returns></returns>
        public bool ExcludeUnChangedItemsElementExists(B2BEnvironment environment, string profileName)
        {
            //Following will navigate to the page : Profile->Buyer Catalog->Automated BHC Catalog-Processing Rules
            GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EnableCatalogAutoGeneration);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BuyerCatalogFirstIdentity);
            }
            else
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EditScheduleButton);
            }

            return (b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Displayed);
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
        public bool VerifyAutoScheduledUISettingsValidations(string environment, string profileName, bool splField, bool snpField, bool sysField, bool snpCRTField, bool sysCRTField, bool stdField = false, bool stdCRTField = false, bool excludeUnChangedItems = false, bool enableDeltaCatalog = false)
        {
            //Following will navigate to the page : Profile->Buyer Catalog->Automated BHC Catalog-Processing Rules
            GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EnableCatalogAutoGeneration);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BuyerCatalogFirstIdentity);
            }
            else
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EditScheduleButton);
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
            if (b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            }
            if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
            {
                b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
            }
            if (b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Selected)
            {
                b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Click();
            }

            //If Parameter-"splField" is true, then folliwng will Turned On SPL field
            if (splField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
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

            if (stdField == true)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
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

            //If Parameter-"sysCRTField" is true, then folliwng will Turned On SYS Auto CRT field
            if (stdCRTField == true)
            {
                if (!b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
                }
            }

            //Following will saves the profile with above settings
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate, DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
            {
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            }
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.DeltaCatalogStartDate, DateTime.Now.AddDays(2).ToString(MMDDYYYY));

            if (excludeUnChangedItems == true)
            {
                if (!b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Selected)
                {
                    b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Click();
                }
            }
            else if (excludeUnChangedItems == false && enableDeltaCatalog == false)
            {
                if (b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                {
                    b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
                }
            }


            if (splField == true && snpField == false && sysField == false && stdField == false)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                }
            }
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.UpdateButton);
            WaitForPageRefresh();
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
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
                matchFlag &= (UtilityMethods.CompareValues<bool>("excludeNonChangedItems", b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Selected, excludeUnChangedItems));
            }
            else
            {
                matchFlag &= (UtilityMethods.CompareValues<bool>("SPL", b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected, splField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNP", b2BBuyerCatalogPage.CatalogConfigSnP.Selected, snpField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYS", b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected, sysField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SNPCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected, snpCRTField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("SYSCRT", b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected, sysCRTField));
                matchFlag &= (UtilityMethods.CompareValues<bool>("excludeNonChangedItems", b2BBuyerCatalogPage.EnableExcludeNonChangedItems.Selected, excludeUnChangedItems));
            }
            return matchFlag;
        }

        public bool VerifyStdSysFinalPriceUISettingsValidations(B2BEnvironment environment, string profileName, bool isStdFinalPriceField, bool isSysFinalPriceField)
        {
            //Following will navigate to the page : Profile->Buyer Catalog->Automated BHC Catalog-Processing Rules
            GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EnableCatalogAutoGeneration);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BuyerCatalogFirstIdentity);
            }
            else
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EditScheduleButton);
            }

            if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
            }

            if (b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            }

            bool matchFlag = true;

            if (isStdFinalPriceField)
            {
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                matchFlag = !b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Enabled;
                b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();
                matchFlag = b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Enabled;
            }
            else if (isSysFinalPriceField)
            {
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();
                matchFlag = !b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Enabled;
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                matchFlag = b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Enabled;
            }
            return matchFlag;
        }


        public void VerifyP2PValidationForEnableAutoBHCOFFNewProfile(B2BEnvironment b2BEnvironment, string customerSet, string accessGroup, string profileNameBase, CatalogType catalogType)
        {
            ChannelCatalogWorkflow channelCatalogWorkflow = new ChannelCatalogWorkflow(webDriver);
            var newProfileName = channelCatalogWorkflow.CreateNewProfile(b2BEnvironment.ToString(), customerSet, accessGroup, profileNameBase);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateP2PMessage(b2BEnvironment, newProfileName, newProfileName, catalogType);
        }

        public void VerifyP2PValidationForEnableAutoBHCOFFExistingProfile(B2BEnvironment b2BEnvironment, string profileNameBase, CatalogType catalogType)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateP2PMessage(b2BEnvironment, profileNameBase, profileNameBase, catalogType);
        }
        public Dictionary<int, string> GetPartViewerInformation(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, CatalogType type, CatalogStatus status, string profile, string identity)
        {
            string filePath = DownloadAndReturnCatalogFilePath(b2BEnvironment, region, type, ref status, profile, identity);
            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogHeader actualCatalogHeader = actualcatalogXML.BuyerCatalog.CatalogHeader;

            DateTime catDate = DateTime.Parse(actualCatalogHeader.CatalogDate);
            DateTime expDate = DateTime.Parse(actualCatalogHeader.ExpirationDate);
            string ValueinSubHeaderRowOrigPub = null; int i = 2;
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var ValueinHeaderRowOrigPub = "" + "," + actualCatalogHeader.CatalogName + ","
                + actualCatalogHeader.IdentityUserName + ","
                + UtilityMethods.ConvertToString<CatalogStatus>(status)
                + "," + actualCatalogHeader.RequesterEmailId
                + "," + catDate.ToString("dd-MMM-yyyy") + ","
                + actualCatalogHeader.RequesterEmailId + ","
                + actualCatalogHeader.CountryCode + ","
                + expDate.ToString("dd-MMM-yyyy");
            dict.Add(1, ValueinHeaderRowOrigPub);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            foreach (CatalogItemType itemType in catalogItemType)
            {
                var actualCatalogItem = actualCatalogDetails.CatalogItem.Where(ci => (ci.CatalogItemType == itemType && ci.DeltaChange != DeltaStatus.Remove)).FirstOrDefault();
                if (actualCatalogItem != null)
                {
                    ValueinSubHeaderRowOrigPub = actualCatalogItem.DeltaChange + "$"
                        + actualCatalogItem.ShortName + "$"
                        + actualCatalogItem.ItemDescription + "$"
                        + actualCatalogItem.UNSPSC + "$"
                        + actualCatalogItem.UnitPrice.ToString().TrimEnd('0').TrimEnd('.') + "$"
                        + actualCatalogItem.PartId + "$"
                        + actualCatalogItem.QuoteId + "$"
                        + actualCatalogItem.BaseSKUId + "$"
                        + actualCatalogItem.ManufacturerPartNumber + "$"
                        + actualCatalogItem.ListPrice.ToString().TrimEnd('0').TrimEnd('.');
                    dict.Add(i, ValueinSubHeaderRowOrigPub); i++;
                }
            }
            return dict;
        }
        private string DownloadAndReturnCatalogFilePath(B2BEnvironment b2BEnvironment, string region, CatalogType type, ref CatalogStatus status, string profile, string identity, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime beforeSchedTime = DateTime.Now;
            string filename = string.Empty;

            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profile.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identity.ToUpper());
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.CatalogRadioButton.Click();
            if (autoCatalogListPage.OriginalCatalogCheckbox.Selected != (type == CatalogType.Original))
                autoCatalogListPage.OriginalCatalogCheckbox.Click();
            else if (autoCatalogListPage.DeltaCatalogCheckbox.Selected != (type == CatalogType.Delta))
                autoCatalogListPage.DeltaCatalogCheckbox.Click();
            string threadId = string.Empty; int row = 1;
            if (status.Equals(CatalogStatus.CreatedInstant) || status.Equals(CatalogStatus.CreatedWarningInstant))
            {
                autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(status));
                autoCatalogListPage.SearchRecordsLink.Click();
                autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(10));
                var catalogStatus = autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status");
                if (catalogStatus == null)
                {
                    autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(CatalogStatus.CreatedWarningInstant));
                    autoCatalogListPage.SearchRecordsLink.Click();
                    autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(10));
                }
                autoCatalogListPage.ShowhideCatalogMessage.Text.Should().BeNullOrEmpty("No Catalog records found");
                threadId = autoCatalogListPage.CatalogsTable.GetCellValue(row, "Thread");
            }
            else
            {
                autoCatalogListPage.SearchRecordsLink.Click();
                autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
                if (status == CatalogStatus.Created || status == CatalogStatus.CreatedWarning)
                {
                    while (!(UtilityMethods.ConvertToEnum<CatalogStatus>(autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status").Trim()).Equals(CatalogStatus.Created)
                        || UtilityMethods.ConvertToEnum<CatalogStatus>(autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status").Trim()).Equals(CatalogStatus.CreatedWarning)))
                    {
                        row++;
                    }
                }
                if (status == CatalogStatus.Published || status == CatalogStatus.PublishedWarning)
                {
                    while (!(UtilityMethods.ConvertToEnum<CatalogStatus>(autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status").Trim()).Equals(CatalogStatus.Published)
                    || UtilityMethods.ConvertToEnum<CatalogStatus>(autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status").Trim()).Equals(CatalogStatus.PublishedWarning)))
                    {
                        row++;
                    }
                }
                autoCatalogListPage.ShowhideCatalogMessage.Text.Should().BeNullOrEmpty("No Catalog records found");
                row.Should().BeLessOrEqualTo(14, "No matching status records found");
                threadId = autoCatalogListPage.CatalogsTable.GetCellValue(row, "Thread");
            }
            status = UtilityMethods.ConvertToEnum<CatalogStatus>(autoCatalogListPage.CatalogsTable.GetCellValue(row, "Status").Trim());
            var catalogName = autoCatalogListPage.CatalogsTable.GetCellElement(row, "Catalog/Inventory Name").GetAttribute("title");
            try
            {
                string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];
                return new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                    .Where(file => file.Name.Contains(catalogName))
                    .FirstOrDefault().FullName;
            }
            catch
            {
                ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
                return uxWorkflow.DownloadCatalog(identity, beforeSchedTime, row);
            }
        }

        public Dictionary<int, string> GetPartViewerInformation_Old(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string region, string country, CatalogType type, CatalogStatus status, string profile, string identity)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            b2BAutoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            b2BAutoCatalogListPage.SelectTheRegion(region);
            b2BAutoCatalogListPage.CatalogRadioButton.Click();
            if (type == CatalogType.Original)
                b2BAutoCatalogListPage.OriginalCatalogCheckbox.Click();
            else
                b2BAutoCatalogListPage.DeltaCatalogCheckbox.Click();

            b2BAutoCatalogListPage.SelectTheCustomer(profile);
            b2BAutoCatalogListPage.SelectTheIdentity(identity);
            b2BAutoCatalogListPage.SelectTheStatus(status.ToString());
            if (b2BAutoCatalogListPage.LiveChk.Selected)
                b2BAutoCatalogListPage.LiveChk.Click();
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
                foreach (CatalogItem actualCatalogItem in actualCatalogItems)
                {
                    ValueinSubHeaderRowOrigPub = string.Concat(actualCatalogItem.ShortName,
                        " ,",
                        actualCatalogItem.ItemDescription,
                        " ,",
                        actualCatalogItem.UNSPSC,
                        " ,",
                        actualCatalogItem.UnitPrice.ToString().TrimEnd('0').TrimEnd('.'),
                        " ,",
                        actualCatalogItem.PartId,
                        " ,",
                        actualCatalogItem.QuoteId,
                        " ,",
                        actualCatalogItem.BaseSKUId,
                        " ,",
                         actualCatalogItem.ManufacturerPartNumber,
                        " ,",
                        actualCatalogItem.ListPrice.ToString().TrimEnd('0').TrimEnd('.'));

                    dict.Add(i, ValueinSubHeaderRowOrigPub); i++;
                }
            }
            return dict;
        }
        public void VerifyErrorMessageIfNoConfigSelected(B2BEnvironment b2BEnvironment, string profileNameBase, string identityName,
            CatalogType catalogType, SetNewValidation setnew, CatalogItemType catalogItemType = CatalogItemType.ConfigWithDefaultOptions)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateErrorMessageNoConfigSelected(b2BEnvironment, profileNameBase, identityName, catalogType, setnew, catalogItemType);
        }

        public void VerifyErrorMessageWhileCreatingDeltaIfOriginalNotExists(B2BEnvironment b2BEnvironment, string profileNameBase, CatalogType catalogType)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateErrorMessageWhileCreatingDeltaCatalog(b2BEnvironment, profileNameBase, profileNameBase, catalogType);
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
        public bool VerifyMpnAndUpcForSpl(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType,
                                          bool splUI, bool snpUI, bool sysUI, bool isSNP,
                                          string splItemOrderCode, string upcField, string upcValue,
                                          string mpnField, string mpnValue)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            VerifyAutoScheduledUISettingsValidations(b2BEnvironment.ToString(), profileName, splUI, snpUI, sysUI, false, false);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            //Below method verifies the ManufacturePartNumber and UPC values for SPL Item and if those values are valid values it returns True
            if (uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, splItemOrderCode, upcField, upcValue, isSNP) &&
                uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, splItemOrderCode, mpnField, mpnValue, isSNP))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Below method verifies Inventory and Lead Time values of Order code
        /// If Inventory and Lead Time values are valid values then it retuns True
        /// </summary>
        public bool VerifyInventoryAndLtForStdOrderCodes(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType,
                                          string stdBtoOrderCode, string inventoryField, string inventoryValue,
                                          string leadTimeField, string leadTimeValue)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);

            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);

            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            //Below method verifies the Inventory and Lead Time values of Order Codes and if those values are valid values it returns True
            if (uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, stdBtoOrderCode, inventoryField, inventoryValue, false) &&
                uxWorkflow.VerifyFieldValueforAnOrderCode(filePath, stdBtoOrderCode, leadTimeField, leadTimeValue, false))
                return true;
            else
                return false;
        }
        public void VerifyUOMFieldValue(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string orderCode)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            uxWorkflow.ValidateUOMValue(filePath, orderCode, catalogType, catalogItemType).Should().BeTrue("Error: UOM Data mismatch for Catalog XML content with expected values");
        }

        /// <summary>
        /// Below method verifies error "No Records Found with LT < 3" in Log report
        /// If error message is correct then it retuns True
        /// </summary>
        public bool VerifyLeadTimeErrorinLogreport(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType,
                                          bool splUI, bool snpUI, bool sysUI, bool stdUI, bool snpCRTUI, bool sysCRTUI, bool stdCRTUI)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            VerifyAutoScheduledUISettingsValidations(b2BEnvironment.ToString(), profileName, splUI, snpUI, sysUI, snpCRTUI, sysCRTUI, stdUI, stdCRTUI);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["AutoCatalogListPageUrl"] + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string threadID = uxWorkflow.GetThreadID(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            return VerifyErrorMessageInLogReport(b2BEnvironment, threadID, "Unexpected Error: Auto Catalog creation failed", "No Records Found with LT < 3");
        }

        public bool VerifyErrorMessageInLogReport(B2BEnvironment b2BEnvironment, string threadId, string logError, string lodDetailError)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["logReportUrl"] + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
            B2BLogReportPage B2BLogReportPage;
            B2BLogReportPage = new B2BLogReportPage(webDriver);
            B2BLogReportPage.SearchThreadIdNumber(threadId);
            bool error = B2BLogReportPage.FindErrorMessageInLogDetailPage(logError, lodDetailError);

            return error;
        }

        public void VerifyRequestorName(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);

            if (!(catalogStatus == CatalogStatus.Failed))
            {
                string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
                uxWorkflow.ValidateRequestorEmailIdInCatalogHeaderXML(filePath, windowsLogin).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
            }
        }

        /// <summary>
        /// Verifies RemoveItemsLt Checkbox defaultValue for new profile
        /// </summary>
        /// <returns></returns>
        public bool VerifyRemoveItemsLtCheckboxStatus(bool chkBoxStatus)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            }
            return chkBoxStatus == b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Enabled;
        }

        /// <summary>
        /// Searches for the profile provided in B2B Profile List page with filter options
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        public bool VerifyAdvanceSearchForProfile(B2BEnvironment b2BEnvironment, string customerName, string customerID = "", string region = "", bool isMigrated = false, bool isBHCAutoEnabled = false)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            b2BHomePage.SelectEnvironment(b2BEnvironment.ToString());
            b2BHomePage.ClickB2BProfileList();
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.AdvanceSearchProfileWithFilterOptions(customerName, customerID, region, isMigrated, isBHCAutoEnabled);
            return b2BCustomerProfileListPage.VerifyProfileSearchResult(customerName, "No");
        }

        public string UpdateScheduleInformationForNewProfile(ExpireDays expireDays)
        {
            if (!b2BBuyerCatalogPage.BcpCatalogEnabled.Selected)
                b2BBuyerCatalogPage.BcpCatalogEnabled.Click();
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            b2BBuyerCatalogPage.BuyerCatalogFirstIdentity.Click();
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

            if (b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected)
                b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            b2BBuyerCatalogPage.SetTextBoxValue(b2BBuyerCatalogPage.OriginalCatalogStartDate,
                DateTime.Now.AddDays(1).ToString(MMDDYYYY));
            SelectElement selectedValue = new SelectElement(b2BBuyerCatalogPage.CatalogExpire);
            switch (expireDays)
            {
                case (ExpireDays.Thirty):
                    selectedValue.SelectByIndex(0);
                    break;
                case (ExpireDays.Ninty):
                    selectedValue.SelectByIndex(1);
                    break;
                default:
                    selectedValue.SelectByIndex(2);
                    break;
            }
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            return b2BBuyerCatalogPage.BuyerCatalogProfileName.Text.Trim();
        }

        public CatalogItem GetCatalogItem(B2BEnvironment b2BEnvironment, CatalogItemType catalogItemType, CatalogType type, CatalogStatus status, Region region, string profile, string identity)
        {
            string filePath = DownloadAndReturnCatalogFilePath(b2BEnvironment, type, status, region, profile, identity);
            B2BXML actualcatalogXML = XMLDeserializer<B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            return actualCatalogDetails.CatalogItem.Where(ci => ci.CatalogItemType == catalogItemType).FirstOrDefault();
        }
        private string DownloadAndReturnCatalogFilePath(B2BEnvironment b2BEnvironment, CatalogType type, CatalogStatus status, Region region, string profile, string identity, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime beforeSchedTime = DateTime.Now;
            string filename = string.Empty;

            B2BChannelUx b2bChannelUx = new B2BChannelUx(webDriver);
            b2bChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString().ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profile.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identity.ToUpper());
            autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(status));
            autoCatalogListPage.SelectCatalogTestOrLive(catalogTestOrLive);
            autoCatalogListPage.CatalogRadioButton.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            var threadId = autoCatalogListPage.CatalogsTable.GetCellValue(1, "Thread");
            try
            {
                string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];
                return new DirectoryInfo(downloadPath).GetFiles().AsEnumerable()
                    .Where(file => file.Name.Contains(identity.ToUpper()) && file.Name.Contains(threadId.ToUpper()))
                    .FirstOrDefault().FullName;
            }
            catch
            {
                ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
                return uxWorkflow.DownloadCatalog(identity, beforeSchedTime);
            }
        }
        public bool AuditHistoryForChangesWhileAutoBHCEnabled(string environment, string profile)
        {
            GoToBuyerCatalogTab(environment, profile);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                WaitForPageRefresh();
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            }
            Dictionary<string, string> oldValues = GetElementValues();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Click();
            }
            b2BBuyerCatalogPage.EditScheduleButton.Click();
            if (b2BBuyerCatalogPage.CatalogOperationCreate.Selected)
                b2BBuyerCatalogPage.CatalogOperationCreatePublish.Click();
            else
                b2BBuyerCatalogPage.CatalogOperationCreate.Click();
            b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Click();
            UpdateConfigurations();
            b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();

            UpdateShedule(oldValues);

            SelectElement selectedValue = new SelectElement(b2BBuyerCatalogPage.CatalogExpire);
            switch (oldValues["AUTO BHC: Items in Catalog Expires in"])
            {
                case "30":
                    selectedValue.SelectByIndex(2);
                    break;
                case "90":
                    selectedValue.SelectByIndex(0);
                    break;
                default:
                    selectedValue.SelectByIndex(1);
                    break;
            }
            b2BBuyerCatalogPage.CheckExcludeNonChangedItems.Click();
            if (b2BBuyerCatalogPage.InternalEMail.Text == "abc@dell.com")
            {
                b2BBuyerCatalogPage.InternalEMail.Clear();
                b2BBuyerCatalogPage.InternalEMail.SendKeys("xyz@dell.com");
            }
            else
            {
                b2BBuyerCatalogPage.InternalEMail.Clear();
                b2BBuyerCatalogPage.InternalEMail.SendKeys("abc@dell.com");
            }
            if (b2BBuyerCatalogPage.InternalEMail.Text == "abc@dell.com")
            {
                b2BBuyerCatalogPage.CustomerEmail.Clear();
                b2BBuyerCatalogPage.CustomerEmail.SendKeys("xyz@dell.com");
            }
            else
            {
                b2BBuyerCatalogPage.CustomerEmail.Clear();
                b2BBuyerCatalogPage.CustomerEmail.SendKeys("xyz@dell.com");
            }
            b2BBuyerCatalogPage.UpdateButton.Click();
            WaitForPageRefresh();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            Dictionary<string, string> newValues = GetElementValues();
            webDriver.Navigate().Back(); webDriver.Navigate().Forward();
            WaitForPageRefresh();
            return VerifyAuditHistoryRow(oldValues, newValues);
        }
        private void UpdateConfigurations()
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Click();
            b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Click();
            b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                b2BBuyerCatalogPage.CatalogConfigStandard.Click();
            if (!b2BBuyerCatalogPage.CatalogConfigSnP.Selected)
                b2BBuyerCatalogPage.CatalogConfigSnP.Click();
            if (!b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
                b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Click();

            b2BBuyerCatalogPage.CatalogConfigUpsellDownSell.Click();
            if (b2BBuyerCatalogPage.CatalogConfigUpsellDownSell.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigIncludeOptionType.Click();
                b2BBuyerCatalogPage.CatalogConfigIncludeAbsolutePrice.Click();
            }
            b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Click();
            b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Click();
            b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Click();

            if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Click();
                b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Click();
            }
        }
        private void UpdateShedule(Dictionary<string, string> oldValues)
        {
            if (oldValues["AUTO BHC: Original Start Date"] == DateTime.Now.AddDays(10).ToString(MMDDYYYY))
            {
                var originalStartDate = DateTime.Now.AddDays(11).ToString(MMDDYYYY);
                var originalFrequencyDays = "3";
                var originalEndDate = DateTime.Now.AddDays(30).ToString(MMDDYYYY);
                var originalTimeOfSend = "9";
                SetOriginalSchedule(originalStartDate, originalFrequencyDays, FrequencyType.Days, originalEndDate, originalTimeOfSend);
            }
            else
            {
                var originalStartDate = DateTime.Now.AddDays(10).ToString(MMDDYYYY);
                var originalFrequencyDays = "2";
                var originalEndDate = DateTime.Now.AddDays(29).ToString(MMDDYYYY);
                var originalTimeOfSend = "8";
                SetOriginalSchedule(originalStartDate, originalFrequencyDays, FrequencyType.Days, originalEndDate, originalTimeOfSend);
            }

            if (oldValues["AUTO BHC: Delta Start Date"] == DateTime.Now.AddDays(12).ToString(MMDDYYYY))
            {
                var deltaStartDate = DateTime.Now.AddDays(13).ToString(MMDDYYYY);
                var deltaFrequencyDays = "2";
                var deltaEndDate = DateTime.Now.AddDays(29).ToString(MMDDYYYY);
                var deltaTimeOfSend = "8";
                SetDeltaSchedule(deltaStartDate, deltaFrequencyDays, FrequencyType.Days, deltaEndDate, deltaTimeOfSend);
            }
            else
            {
                var deltaStartDate = DateTime.Now.AddDays(12).ToString(MMDDYYYY);
                var deltaFrequencyDays = "1";
                var deltaEndDate = DateTime.Now.AddDays(28).ToString(MMDDYYYY);
                var deltaTimeOfSend = "6";
                SetDeltaSchedule(deltaStartDate, deltaFrequencyDays, FrequencyType.Days, deltaEndDate, deltaTimeOfSend);
            }
        }
        private Dictionary<string, string> GetElementValues()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            fields.Add("AUTO BHC: Enabled", b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected.ToString());
            if (b2BBuyerCatalogPage.CatalogOperationCreatePublish.Selected)
                fields.Add("AUTO BHC: Catalog Operation", "Create & Publish");
            else if (b2BBuyerCatalogPage.CatalogOperationCreate.Selected)
                fields.Add("AUTO BHC: Catalog Operation", "Create");
            fields.Add("AUTO BHC: Remove Items With LT > 3 Days", b2BBuyerCatalogPage.BcpchkRemoveItemsWithLTAbove3Days.Selected.ToString());
            fields.Add("AUTO BHC: Cross Reference Update STD Enabled", b2BBuyerCatalogPage.BcpchkCrossRefernceStdUpdate.Selected.ToString());
            fields.Add("AUTO BHC: Cross Reference Update SNP Enabled", b2BBuyerCatalogPage.BcpchkCrossRefernceSnpUpdate.Selected.ToString());
            fields.Add("AUTO BHC: Cross Reference Update SYS Enabled", b2BBuyerCatalogPage.BcpchkCrossRefernceSysUpdate.Selected.ToString());
            fields.Add("AUTO BHC: Standard Configurations", b2BBuyerCatalogPage.CatalogConfigStandard.Selected.ToString());
            fields.Add("AUTO BHC: Upsell and Downsell", b2BBuyerCatalogPage.CatalogConfigUpsellDownSell.Selected.ToString());
            fields.Add("AUTO BHC: SNP", b2BBuyerCatalogPage.CatalogConfigSnP.Selected.ToString());
            fields.Add("AUTO BHC: Include Default Options", b2BBuyerCatalogPage.CatalogConfigIncludeDefaultOptions.Selected.ToString());
            fields.Add("AUTO BHC: Include Absolute Price", b2BBuyerCatalogPage.CatalogConfigIncludeAbsolutePrice.Selected.ToString());
            fields.Add("AUTO BHC: Include Option Type", b2BBuyerCatalogPage.CatalogConfigIncludeOptionType.Selected.ToString());
            fields.Add("AUTO BHC: Include Final Price", b2BBuyerCatalogPage.CatalogConfigIncludeFinalPrice.Selected.ToString());
            fields.Add("AUTO BHC: Include Sku Details", b2BBuyerCatalogPage.CatalogConfigIncludeSkuDetails.Selected.ToString());
            fields.Add("AUTO BHC: Sytem Catalog", b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected.ToString());
            if (b2BBuyerCatalogPage.BcpchkSysCatalogCheckbox.Selected)
            {
                fields.Add("AUTO BHC: SYS: Default Options", b2BBuyerCatalogPage.CatalogConfigSysDefaultOptionsCheckbox.Selected.ToString());
                fields.Add("AUTO BHC: SYS: Final Price", b2BBuyerCatalogPage.CatalogConfigSysFinalPriceCheckbox.Selected.ToString());
                fields.Add("AUTO BHC: SYS: Sku Details", b2BBuyerCatalogPage.CatalogConfigSysSkuDetailsCheckbox.Selected.ToString());
            }
            else
            {
                fields.Add("AUTO BHC: SYS: Default Options", "False");
                fields.Add("AUTO BHC: SYS: Final Price", "False");
                fields.Add("AUTO BHC: SYS: Sku Details", "False");
            }
            fields.Add("EnableOriginalCatalog", b2BBuyerCatalogPage.EnableOriginalCatalog.Selected.ToString());
            fields.Add("AUTO BHC: Original Start Date", b2BBuyerCatalogPage.OriginalCatalogStartDate.GetAttribute("value"));
            fields.Add("AUTO BHC: Original Frequency", b2BBuyerCatalogPage.OriginalFrequencyDays.Select().SelectedOption.GetAttribute("value") + " Day(s)");
            //fields.Add("OriginalFrequencyWeeks", b2BBuyerCatalogPage.OriginalFrequencyWeeks.Select().SelectedOption.GetAttribute("value"));
            fields.Add("AUTO BHC: Original End Date", b2BBuyerCatalogPage.OriginalCatalogEndDate.GetAttribute("value"));
            fields.Add("AUTO BHC: Original Time Of Send", "0" + b2BBuyerCatalogPage.OriginalTimeOfSend.Select().SelectedOption.GetAttribute("value") + ":00:00");
            fields.Add("AUTO BHC: Delta Scheduled", b2BBuyerCatalogPage.EnableDeltaCatalog.Selected.ToString());
            fields.Add("AUTO BHC: Delta Start Date", b2BBuyerCatalogPage.DeltaCatalogStartDate.GetAttribute("value"));
            fields.Add("AUTO BHC: Delta Frequency", b2BBuyerCatalogPage.DeltaFrequencyDays.Select().SelectedOption.GetAttribute("value") + " Day(s)");
            //fields.Add("DeltaFrequencyWeeks", b2BBuyerCatalogPage.DeltaFrequencyWeeks.Select().SelectedOption.GetAttribute("value"));
            fields.Add("AUTO BHC: Delta End Date", b2BBuyerCatalogPage.DeltaCatalogEndDate.GetAttribute("value"));
            fields.Add("AUTO BHC: Delta Time Of Send", "0" + b2BBuyerCatalogPage.DeltaTimeOfSend.Select().SelectedOption.GetAttribute("value") + ":00:00");
            fields.Add("AUTO BHC: Items in Catalog Expires in", b2BBuyerCatalogPage.CatalogExpire.Select().SelectedOption.GetAttribute("value"));
            fields.Add("AUTO BHC: Exclude Unchanged Items", b2BBuyerCatalogPage.CheckExcludeNonChangedItems.Selected.ToString());
            fields.Add("AUTO BHC: Internal Email", b2BBuyerCatalogPage.InternalEMail.GetAttribute("value"));
            fields.Add("AUTO BHC: External Email", b2BBuyerCatalogPage.CustomerEmail.GetAttribute("value"));
            fields.Add("AUTO BHC: SPL", b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected.ToString());
            return fields;
        }

        /// <summary>
        /// Below method verifies Auto BHC Settings Update
        /// </summary>
        public bool B2BProfileAutoBHCSettingsUpdate(B2BEnvironment b2BEnvironment, string profileName,
                                          bool splUI, bool snpUI, bool sysUI, bool snpCRTField,
                                          bool sysCRTField, bool stdField, bool stdCRTField, bool excludeUnChangedItems, bool enableDeltaCatallog)
        {
            webDriver.Navigate().GoToUrl(ConfigurationManager.AppSettings["B2BBaseURL"]);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            return VerifyAutoScheduledUISettingsValidations(b2BEnvironment.ToString(), profileName, splUI, snpUI, sysUI, snpCRTField, sysCRTField, stdField, stdCRTField, excludeUnChangedItems, enableDeltaCatallog);
        }


        public void VerifyInstantCatalogSearch(B2BEnvironment environment, Region region, string profileName, string identityName, CatalogType catalogType, CatalogStatus catalogStatus, CatalogItemType[] catalogItemType)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            DateTime anyTimeAfter = DateTime.Now.AddDays(-180);
            uxWorkflow.SearchInstantCatalog(environment, region, profileName, identityName, catalogType, catalogStatus, catalogItemType, anyTimeAfter);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, anyTimeAfter, RequestorValidation.Off);
            uxWorkflow.DownloadCatalog(identityName, anyTimeAfter).Should().NotBeNull("Unable to download the file");
        }

        public bool VerifyCatlogEndDateGrayOutValidations(B2BEnvironment environment, string profileName, bool isTestProfile, bool isSTD, bool isSNP, bool isSYS, bool isSPL)
        {
            GoToGeneralTab(environment.ToString(), profileName);

            b2BProfileSettingsGeneralPage = new B2BProfileSettingsGeneralPage(webDriver);
            if (isTestProfile)
            {
                if (!b2BProfileSettingsGeneralPage.TestProfileCheckbox.Selected)
                {
                    b2BProfileSettingsGeneralPage.TestProfileCheckbox.Click();
                    b2BProfileSettingsGeneralPage.UpdateProfileButton.Click();
                }
            }
            else
            {
                if (b2BProfileSettingsGeneralPage.TestProfileCheckbox.Selected)
                {
                    b2BProfileSettingsGeneralPage.TestProfileCheckbox.Click();
                    b2BProfileSettingsGeneralPage.UpdateProfileButton.Click();
                }
            }

            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            b2BBuyerCatalogPage.EnableCatalogAutoGeneration.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            if (!b2BBuyerCatalogPage.EnableCatalogAutoGeneration.Selected)
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EnableCatalogAutoGeneration);
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.BuyerCatalogFirstIdentity);
            }
            else
            {
                UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.EditScheduleButton);
            }

            if (isSTD)
            {
                if (!b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.CatalogConfigStandard.Selected)
                {
                    b2BBuyerCatalogPage.CatalogConfigStandard.Click();
                }
            }

            if (isSNP)
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

            if (isSYS)
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

            if (isSPL)
            {
                if (!b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                }
            }
            else
            {
                if (b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Selected)
                {
                    b2BBuyerCatalogPage.BcpchkSPLFlagCheckbox.Click();
                }
            }

            if (!b2BBuyerCatalogPage.EnableOriginalCatalog.Selected)
            {
                b2BBuyerCatalogPage.EnableOriginalCatalog.Click();
            }

            if (!b2BBuyerCatalogPage.EnableDeltaCatalog.Selected)
            {
                b2BBuyerCatalogPage.EnableDeltaCatalog.Click();
            }

            bool matchFlag = true;
            try
            {
                if (isTestProfile)
                {
                    matchFlag = b2BBuyerCatalogPage.OriginalCatalogEndDate.Enabled && b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled;
                }
                else
                {
                    matchFlag = !b2BBuyerCatalogPage.OriginalCatalogEndDate.Enabled && !b2BBuyerCatalogPage.DeltaCatalogEndDate.Enabled;
                }
            }
            catch (Exception)
            {
                matchFlag = false;
            }
            return matchFlag;
        }

        public void VerifyLogReportOnFailedCatalog(B2BEnvironment environment, Region region, string profileName, string identityName, CatalogItemType[] catalogItemType, CatalogType catalogType, CatalogStatus catalogStatus)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(environment, profileName, identityName, catalogType);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(environment);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            var threadId = autoCatalogListPage.CatalogsTable.GetCellValue(1, "Thread");
            VerifyMessageInLogReport(threadId, environment);
        }
        internal void VerifyMessageInLogReport(string threadId, B2BEnvironment environment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("B2BBaseURL"));// + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("logReportUrl") + ((environment == B2BEnvironment.Production) ? "P" : "U"));
            B2BLogReportPage b2bLogReport = new B2BLogReportPage(webDriver);
            b2bLogReport.SearchThreadIdNumber(threadId);
            b2bLogReport.WaitForElementVisible();
            b2bLogReport.GetCellValueFromLogTable(3, "Message").Should().Contain("failed as Exclude Unchanged Items flag returned no records", "Delta Catalog creation is failed not due to Exclude Unchanged Items");
        }
        public void VerifyHardcodedInvQtyLTValuesForDellBrandedSnP(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType, string profileName, string identityName,
            CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            if (catalogStatus == CatalogStatus.Published)
                uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
            else if (catalogStatus == CatalogStatus.CreatedInstant)
                uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType, catalogItemType);

            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, false);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, beforeSchedTime);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            uxWorkflow.VerifyHardcodedInvQtyLTValuesForDellBrandedSnP(catalogItemType, catalogType, identityName, filePath, beforeSchedTime).Should().BeTrue("Error: Data mismatch for Catalog XML content with expected values");
        }

        public void VerifyCatalogCreationByOperation(B2BEnvironment b2BEnvironment, CatalogItemType catalogItemType,
           string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);

            B2BChannelUx b2BChannelUXPage = new B2BChannelUx(webDriver);
            b2BChannelUXPage.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogTestOrLive);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);
        }
        public void VerifySetNewCatalogCreationByOperation(B2BEnvironment b2BEnvironment, CatalogItemType[] catalogItemType,
           string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime beforeSchedTime = DateTime.Now;

            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType, catalogItemType);

            B2BChannelUx b2BChannelUXPage = new B2BChannelUx(webDriver);
            b2BChannelUXPage.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);

            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogTestOrLive);
            uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);
        }
        public void VerifyLogReportNavigation(B2BEnvironment environment, Region region, string profileName, string identityName, CatalogItemType[] catalogItemType,
            CatalogType catalogType, CatalogStatus catalogStatus, CatalogTestOrLive catalogTestOrLive = CatalogTestOrLive.None)
        {
            DateTime anyTimeAfter = DateTime.Now.AddDays(-180);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(environment);
            uxWorkflow.SearchCatalog(profileName, identityName, anyTimeAfter, catalogStatus, catalogType, catalogTestOrLive);
            uxWorkflow.ValidateCatalogSearchResult(catalogItemType, catalogType, catalogStatus, anyTimeAfter);
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            var threadIdValue = autoCatalogListPage.CatalogsTable.GetCellValue(1, "Thread");
            IWebElement threadId = autoCatalogListPage.CatalogsTable.GetCellElement(1, "Thread");
            threadId.Click();
            WaitForPageRefresh(); threadId.Click();
            IReadOnlyCollection<string> windowHandles = webDriver.WindowHandles;
            webDriver.SwitchTo().Window(webDriver.WindowHandles.Last()); WaitForPageRefresh();
            webDriver.Url.Should().Contain(threadIdValue, "User navigated to Logdetails page");
        }

        public void VerifyInstantCatalogErrorMessage(B2BEnvironment b2BEnvironment, string profileName, string identityName, CatalogType catalogType, CatalogItemType catalogItemType
            , string accessGroup, ErrorMessages errorMessages, bool isSetNew)
        {
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.ValidateInstantCatalogErrorMessage(b2BEnvironment, profileName, identityName, catalogType, catalogItemType, accessGroup, errorMessages, isSetNew);
        }

        public void VerifyWarningMessageInCPTPage(B2BEnvironment b2BEnvironment,string region, string profileName, string identityName, string customerSet,
            CatalogType catalogType, CatalogStatus catalogStatus, CatalogItemType[] catalogItemType, ErrorMessages errorMessages,
            string accessGroup = null, bool instantCatalog = false, string orderCodes = null)
        {
            DateTime beforeSchedTime = DateTime.Now;
            B2BChannelUx b2BChannelUx = new B2BChannelUx(webDriver);
            b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            if (!uxWorkflow.ValidateRecentCatalogExists(region, profileName, identityName, catalogType, catalogStatus, beforeSchedTime))
            {
                if (instantCatalog)
                {
                    if (catalogItemType.Count() == 1)
                    {
                        if (catalogItemType[0] == CatalogItemType.ConfigWithDefaultOptions)
                            uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType,
                                new CatalogItemType[] { CatalogItemType.ConfigWithDefaultOptions, CatalogItemType.Systems });
                        if (catalogItemType[0] == CatalogItemType.Systems)
                            uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType,
                                new CatalogItemType[] { CatalogItemType.ConfigWithDefaultOptions, CatalogItemType.Systems });
                        if (catalogItemType[0] == CatalogItemType.SNP)
                            uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType,
                                new CatalogItemType[] { CatalogItemType.ConfigWithDefaultOptions, CatalogItemType.SNP });
                    }
                    else
                        uxWorkflow.CreateInstantCatalogSetNew(b2BEnvironment, profileName, identityName, catalogType, new CatalogItemType[] { CatalogItemType.ConfigWithDefaultOptions, CatalogItemType.SNP, CatalogItemType.Systems }/*catalogItemType*/);
                }
                else
                    uxWorkflow.PublishCatalogByClickOnce(b2BEnvironment, profileName, identityName, catalogType);
                b2BChannelUx.OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
                uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None);//.Should().NotBeNullOrEmpty("No Catalog records found"); ;
                uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime);
            }
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.CatalogsTable.GetStatusMessageIconForCatalog(1, "Status").Click();
            Thread.Sleep(5000); string allConifgs = string.Empty;

            if (catalogItemType.Count() > 1)
            {
                if (catalogItemType.Count() == 2)
                {
                    if (catalogItemType.Contains(CatalogItemType.ConfigWithDefaultOptions) && catalogItemType.Contains(CatalogItemType.Systems))
                        allConifgs = "STD and SYS";
                    if (catalogItemType.Contains(CatalogItemType.ConfigWithDefaultOptions) && catalogItemType.Contains(CatalogItemType.SNP))
                        allConifgs = "STD and SNP";
                    if (catalogItemType.Contains(CatalogItemType.Systems) && catalogItemType.Contains(CatalogItemType.SNP))
                        allConifgs = "SYS and SNP";
                }
                if (catalogItemType.Count() == 3)
                {
                    if (errorMessages == ErrorMessages.AccessGroupNotAssociated)
                        allConifgs = "STD, SYS and SNP";
                    if (errorMessages == ErrorMessages.PageLevelSettingsOff)
                        allConifgs = "STD, SNP and SYS";
                }
                switch (errorMessages)
                {
                    case ErrorMessages.AccessGroupNotAssociated:
                        string allAccessGroupError = string.Concat("For the Customer Set " + customerSet + " and Access Group " + accessGroup + ": " + allConifgs + " are not enabled at Access Group level in OST."/*\r\nPlease fix the OST configuration to rectify the error."*/);
                        if (autoCatalogListPage.FailureReason.Trim().Contains("service cards"))
                            autoCatalogListPage.FailureReason.Trim().Should().Contain(allAccessGroupError);
                        else
                            autoCatalogListPage.FailureReason.Should().Contain(allAccessGroupError);
                        break;
                    case ErrorMessages.ZeroCatalogItems:
                        break;
                    case ErrorMessages.PageLevelSettingsOff:
                        string allPageLevelError = string.Concat("For the Customer Set " + customerSet + ", " + allConifgs + " configuration at OST Page level is turned off."/*\r\nPlease fix the OST configuration to rectify the error."*/);
                        autoCatalogListPage.FailureReason.Should().Contain(allPageLevelError);
                        break;
                    case ErrorMessages.AccessGroupDeletedInOST:
                        string accessGroupDeletedError = string.Concat("For the Customer Set " + customerSet + " and" + " Access Group " + accessGroup + " is inactive hence catalog failed."/*\r\nPlease fix the OST configuration to rectify the error."*/);
                        autoCatalogListPage.FailureReason.Should().Contain(accessGroupDeletedError);
                        break;
                    default:
                        break;
                }
            }
            else if (catalogItemType.Count() == 1)
            {
                switch (errorMessages)
                {
                    case ErrorMessages.AccessGroupNotAssociated:
                        if (catalogItemType[0] == CatalogItemType.ConfigWithDefaultOptions || catalogItemType[0] == CatalogItemType.ConfigWithUpsellDownsell)
                        {
                            string stdError = string.Concat("For the Customer Set " + customerSet + " and Access Group " + accessGroup + ": STD is not enabled at Access Group level in OST."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            if (autoCatalogListPage.FailureReason.Trim().Contains("service cards"))
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(stdError);
                            else
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(stdError);

                        }
                        if (catalogItemType[0] == CatalogItemType.SNP)
                        {
                            string snpError = string.Concat("For the Customer Set " + customerSet + " and Access Group " + accessGroup + ": SNP is not enabled at Access Group level in OST."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            if (autoCatalogListPage.FailureReason.Trim().Contains("service cards"))
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(snpError);
                            else
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(snpError);
                        }
                        if (catalogItemType[0] == CatalogItemType.Systems)
                        {
                            string sysError = string.Concat("For the Customer Set " + customerSet + " and Access Group " + accessGroup + ": SYS is not enabled at Access Group level in OST."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            if (autoCatalogListPage.FailureReason.Trim().Contains("service cards"))
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(sysError);
                            else
                                autoCatalogListPage.FailureReason.Trim().Should().Contain(sysError);
                        }
                        break;
                    case ErrorMessages.ZeroCatalogItems:
                        autoCatalogListPage.FailureReason.Trim().Should().Contain("Error while generating Auto Catalog XML. Auto Catalog creation failed due to zero items in the OST Store.");
                        break;
                    case ErrorMessages.PageLevelSettingsOff:
                        if (catalogItemType[0] == CatalogItemType.ConfigWithDefaultOptions || catalogItemType[0] == CatalogItemType.ConfigWithUpsellDownsell)
                        {
                            string stdPageError = string.Concat("For the Customer Set " + customerSet + ", STD configuration at OST Page level is turned off."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            autoCatalogListPage.FailureReason.Trim().Should().Contain(stdPageError);
                        }
                        else if (catalogItemType[0] == CatalogItemType.Systems)
                        {
                            string sysPageError = string.Concat("For the Customer Set " + customerSet + ", SYS configuration at OST Page level is turned off."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            autoCatalogListPage.FailureReason.Trim().Should().Contain(sysPageError);
                        }
                        else if (catalogItemType[0] == CatalogItemType.SNP)
                        {
                            string snpPageError = string.Concat("For the Customer Set " + customerSet + ", SNP configuration at OST Page level is turned off."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                            autoCatalogListPage.FailureReason.Trim().Should().Contain(snpPageError);
                        }
                        break;
                    case ErrorMessages.AccessGroupDeletedInOST:
                        string accessGroupDeletedError = string.Concat("For the Customer Set " + customerSet + "," + " Access Group " + accessGroup + "  is inactive hence catalog failed."/*\r\n\r\nPlease fix the OST configuration to rectify the error."*/);
                        autoCatalogListPage.FailureReason.Should().Contain(accessGroupDeletedError);
                        break;
                    case ErrorMessages.WarningOrderCodes:
                        foreach (string oCode in orderCodes.Split(','))
                            autoCatalogListPage.FailureReason.IndexOf(oCode, StringComparison.InvariantCultureIgnoreCase).Should().BeGreaterOrEqualTo(0);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool VerifyPartHistoryTable(B2BEnvironment b2BEnvironment, string region, string country, string profileName, string identityName, string[] status, bool instant)
        {
            Dictionary<int, string> dict = GetPartViewerInformation(b2BEnvironment,
                new CatalogItemType[] { CatalogItemType.ConfigWithDefaultOptions },
                region, country, CatalogType.Original, CatalogStatus.Created, profileName, identityName);
            string mpn = string.Empty;
            dict.Count().ShouldBeEquivalentTo(2, "Expected product types are not available in catalog generated &downloaded");
            mpn = dict[2].Split('$')[8].ToString();
            B2BChannelUx b2BChannelUXPage = new B2BChannelUx(webDriver);
            b2BChannelUXPage.OpenAutoPartViewerPage(b2BEnvironment);
            B2BQuoteViewerPage autoBHCQuoteViewerPage = new B2BQuoteViewerPage(webDriver);
            WaitForPageRefresh();
            if (!autoBHCQuoteViewerPage.HistoryCheckbox.Selected)
                autoBHCQuoteViewerPage.HistoryCheckbox.Click();
            autoBHCQuoteViewerPage.SelectOption(autoBHCQuoteViewerPage.SelectRegionSpan, region);
            autoBHCQuoteViewerPage.SelectTheCountry(country);
            autoBHCQuoteViewerPage.SelectOption(autoBHCQuoteViewerPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoBHCQuoteViewerPage.SelectOption(autoBHCQuoteViewerPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoBHCQuoteViewerPage.SelectTheStatus(status);
            if (instant)
                autoBHCQuoteViewerPage.InstantCheckbox.Click();
            autoBHCQuoteViewerPage.MPNTextBox.SendKeys(mpn);
            autoBHCQuoteViewerPage.PartViewerSearchButton.Click();
            autoBHCQuoteViewerPage.QuoteHistoryTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoBHCQuoteViewerPage.QuoteHistoryTable.Displayed.Should().BeTrue("No Data Found for '" + mpn + "'");

            Convert.ToString(dict[1].Split(',')[1].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "CatalogName").Trim(), "Catalog Name mismatch");
            Convert.ToString(dict[1].Split(',')[2].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Identity").Trim(), "Identity Name mismatch");
            Convert.ToString(dict[1].Split(',')[3].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Status").Trim(), "Catalog Status mismatch");
            Convert.ToString(dict[2].Split('$')[0].Trim()).Should().Contain(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Change Type").Trim().Substring(0, 1), "ChangeType Name mismatch");
            Convert.ToString(dict[2].Split('$')[2].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Part Description").Trim(), "Part Description mismatch");
            Convert.ToString(dict[2].Split('$')[4].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Unit Price").Trim(), "Unit Price mismatch");
            Convert.ToString(dict[2].Split('$')[6].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "BHC Q ID").Trim(), "BHC Q ID mismatch");
            Convert.ToString(dict[2].Split('$')[8].Trim()).Should().BeEquivalentTo(autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "MPN(VPN)").Trim(), "Manufacturer Number mismatch");
            string cDate = autoBHCQuoteViewerPage.QuoteHistoryTable.GetCellValueFromMPNHistoryTable(1, "Date Of Change").Trim().ToString();
            string[] d = cDate.Split(' ')[0].Split('-');
            Convert.ToString(dict[1].Split(',')[5].Trim()).Should().BeEquivalentTo((d[1] + "-" + d[0] + "-" + d[2]).ConvertToDateTime("dd-MMM-yyyy"), "Date Of Change mismatch");

            return true;
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
