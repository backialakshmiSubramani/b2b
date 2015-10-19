﻿using System;
using System.Data;
using System.Linq;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Core.Workflows.Common;
using Modules.Channel.B2B.DAL.Inventory;
using System.Collections.Generic;
using FluentAssertions;
using System.IO;
using System.Globalization;

namespace Modules.Channel.B2B.Core.Workflows.Inventory
{
    /// <summary>
    /// Class for Channel Inventory Workflows
    /// </summary>
    public class ChannelInventoryWorkflow
    {
        private IWebDriver webDriver;
        private AccessProfile accessProfile;
        private B2BBuyerCatalogPage b2BBuyerCatalogPage;
        private B2BEntities b2BEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelInventoryWorkflow"/> class.
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelInventoryWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            accessProfile = new AccessProfile(webDriver);
        }

        /// <summary>
        /// Accesses the B2B Profile & validates the presence of  
        /// required fields in 'Inventory Feed - Processing Rules' section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="clickToRunOnceButtonLabelText"></param>
        /// <param name="clickToRunOnceButtonText"></param>
        /// <param name="enableAutoInventoryLabelText"></param>
        /// <param name="autoInventoryRefreshIntervalLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyFieldsInInventoryFeedProcessingRulesSection(string environment, string profileName,
            string clickToRunOnceButtonLabelText, string clickToRunOnceButtonText, string enableAutoInventoryLabelText,
            string autoInventoryRefreshIntervalLabelText)
        {
            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.ClickToRunOnceButton.WaitForElementDisplayed(TimeSpan.FromSeconds(30));

            return b2BBuyerCatalogPage.VerifyInventoryFeedSectionFields(clickToRunOnceButtonLabelText,
                clickToRunOnceButtonText, enableAutoInventoryLabelText, autoInventoryRefreshIntervalLabelText);
        }

        /// <summary>
        /// Accesses the B2B Profile & verifies if the 
        /// 'Automated Inventory Feed Failure Notification Email' textbox is present
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="failureNotificationEmailLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyPresenceOfEmailField(string environment, string profileName,
            string failureNotificationEmailLabelText)
        {
            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            return b2BBuyerCatalogPage.VerifyPresenceOfEmailField(failureNotificationEmailLabelText);
        }

        /// <summary>
        /// Accesses the B2B Profile & verifies if the checking/unchecking of the 
        /// 'Enable Automated Inventory Feed' checkbox enables/disables
        /// the control below, which are Days, Hours & Minutes dropdowns
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyEnableDisableAutoInventoryCheckbox(string environment, string profileName)
        {
            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            if (!b2BBuyerCatalogPage.VerifyRefreshIntervalDropdownsAreDisabled())
            {
                return false;
            }

            b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();

            return b2BBuyerCatalogPage.VerifyRefreshIntervalDropdownsAreEnabled();
        }

        /// <summary>
        /// Verifies the last_mod_date of the newly created profile is the time of profile creation
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateOfNewProfile(string environment, string customerSet, string accessGroup, string profileNameBase)
        {
            var timeBeforeProfileCreation = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            // Create new profile and verify the last modified date
            var newProfileName = accessProfile.CreateNewProfile(environment, customerSet, accessGroup, profileNameBase);
            var lastModifiedDate = GetLastModifiedDate(newProfileName);

            return lastModifiedDate >= timeBeforeProfileCreation;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while enabling or disabling Auto Inventory
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForEnableDisableAutoInventory(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // If Auto Inventory Checkbox is checked, uncheck and save.
            if (b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            }

            // Check Auto Inventory Checkbox, save changes & verify last modified date
            b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            // Uncheck Auto Inventory Checkbox, save changes & verify last modified date
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while changing the value in Days Dropdown
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForChangeInDaysDropdown(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // If Auto Inventory Checkbox is unchecked, check and save.
            if (!b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            }

            // Select a value in Days dropdown and save.
            b2BBuyerCatalogPage.AutoInventoryDaysDropdown.Select().SelectByValue("1");
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();

            // Change the value in Days dropdown, save and verify last modified date.
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutoInventoryDaysDropdown.Select().SelectByValue("2");
            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while changing the value in Hours Dropdown
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForChangeInHoursDropdown(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // If Auto Inventory Checkbox is unchecked, check and save.
            if (!b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            }

            // Select a value in Hours dropdown and save.
            b2BBuyerCatalogPage.AutoInventoryHoursDropdown.Select().SelectByValue("11");
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();

            // Change the value in Hours dropdown, save and verify last modified date.
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutoInventoryHoursDropdown.Select().SelectByValue("12");
            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while changing the value in Minutes Dropdown
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForChangeInMinutesDropdown(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // If Auto Inventory Checkbox is unchecked, check and save.
            if (!b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            }

            // Select a value in Minutes dropdown and save.
            b2BBuyerCatalogPage.AutoInventoryMinutesDropdown.Select().SelectByValue("0");
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();

            // Change the value in Minutes dropdown, save and verify last modified date.
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutoInventoryMinutesDropdown.Select().SelectByValue("30");
            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while changing a field in Auto BHC Section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForChangeInAutoBhcSection(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules.Click();

            // Change the Catalog Operation selected, save and verify last modified date.
            switch (b2BBuyerCatalogPage.CatalogOperationSelected.GetAttribute("value"))
            {
                case "1":
                    b2BBuyerCatalogPage.CatalogOperationCreatePublish.Click();
                    break;
                case "2":
                    b2BBuyerCatalogPage.CatalogOperationCreate.Click();
                    break;
            }

            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the last_mod_date is updated while changing a field in Buyer Catalog tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyLastModifiedDateForChangeInBuyerCatalogTab(string environment, string profileName)
        {
            DateTime? lastModifiedDateBeforeSave;
            DateTime timeBeforeSave;
            DateTime? lastModifiedDate;

            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // Check/Uncheck the Catalog Enabled checkbox, save and verify last modified date.
            b2BBuyerCatalogPage.BcpCatalogEnabled.Click();

            timeBeforeSave = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
               TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            lastModifiedDateBeforeSave = GetLastModifiedDate(profileName);
            b2BBuyerCatalogPage.UpdateButton.Click();
            accessProfile.WaitForPageRefresh();
            lastModifiedDate = GetLastModifiedDate(profileName);

            if (lastModifiedDate < timeBeforeSave && lastModifiedDateBeforeSave == lastModifiedDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the last_mod_date of the profile provided
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns>The <see cref="DateTime"/></returns>
        private DateTime? GetLastModifiedDate(string profileName)
        {
            b2BEntities = new B2BEntities();

            return (from cm in b2BEntities.b2b_profile
                    where cm.user_id == profileName
                    select cm.last_mod_date)
                .FirstOrDefault();
        }

        public bool ClickToRunOnce(DataRow ClickToRunOncetestData, RunEnvironment Env)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            string profileName = ClickToRunOncetestData["ProfileName"].ToString();
            string statusMsg = ClickToRunOncetestData["inventoryFeedRequestStatusMsg"].ToString();
            string atsFeedLocation = ClickToRunOncetestData["AtsFeedLocation"].ToString();
            string atsFeedPrefix = ClickToRunOncetestData["AtsFeedPrefix"].ToString();
            string atsFeedExtension = ClickToRunOncetestData["AtsFeedExtension"].ToString();
            string atsFileTimeDiff = ClickToRunOncetestData["inventoryFileGenerationTimeDifference"].ToString();

            //Search for the profile and go to Buyer Catalog tab
            accessProfile.GoToBuyerCatalogTab(Env.ToString(), profileName);

            // Click on ClickToRunOnce button
            b2BBuyerCatalogPage.ClickToRunOnce().Should().BeTrue();

            //Verufy ClickToRunOnce request status message
            b2BBuyerCatalogPage.VerifyClickToRunOnceRequestStatus(statusMsg).Should().BeTrue();

            // Verify Inventory Feeds generated for the enabled identites in the selected profile
            VerifyFeedGeneration(profileName, PoOperations.getEnvCodes(Env), atsFeedLocation, atsFeedPrefix, atsFeedExtension, atsFileTimeDiff).Should().BeTrue();
            return true;
        }

        /// <summary>
        /// Verify the feed generation after clicking on ClickToRunOnce button
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyFeedGeneration(string profileName, string environment, string atsFeedLocation, string atsFeedPrefix, string atsFeedExtn, string atsFileTimeDiff)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            string fileName = string.Empty;
            var identities = b2BBuyerCatalogPage.GetIdentities();
            if (identities != null)
            {
                foreach (string identity in identities)
                {
                    Console.WriteLine("Searching for Inventory file for identity : {0}", identity);
                    return getLatestInventoryFileGenerated(atsFeedLocation + @"\" + environment, atsFeedPrefix + "_" + identity + "_", atsFileTimeDiff);
                }
                return false;
            }
            else
            {
                Console.WriteLine("No Enabled Identites Found for the profile !!");
                return true;
            }
        }

        /// <summary>
        ///Get the latest Inventory File Generated
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool getLatestInventoryFileGenerated(string atsFeedLocation, string feedFileName, string timeDifference)
        {
            var directory = new DirectoryInfo(atsFeedLocation);
            string fileFound = string.Empty;
            DateTime inventoryFileTimeStamp;

            Console.WriteLine("Searching for the Inventory file in the Directory : {0} ", atsFeedLocation);
            Console.WriteLine("Searching for the Inventory file Name [Without TimeStamp] : {0} ", feedFileName);

            if (directory.EnumerateFiles(feedFileName + "*" + ".csv").Any())
            {
                fileFound = (directory.EnumerateFiles(feedFileName + "*" + ".csv")
                        .OrderByDescending(f => f.CreationTimeUtc)
                        .First()
                        .FullName);
                Console.WriteLine("Inventory File found for the given Identity : " + fileFound);

                inventoryFileTimeStamp = DateTime.ParseExact(fileFound.Split('_')[2].Split('.')[0], "MMddHHmmss", CultureInfo.CreateSpecificCulture("en-US"));

                //if (DateTimeOffset.UtcNow.Subtract(inventoryFileTimeStamp).TotalMinutes < Convert.ToDouble(timeDifference))
                DateTime CSTNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                if (CSTNow.Subtract(inventoryFileTimeStamp).TotalMinutes < Convert.ToDouble(timeDifference))
                {
                    Console.WriteLine("The Latest Inventory File found is created within {0} minutes.", timeDifference);
                    return true;
                }
                else
                {
                    Console.WriteLine("The Latest Inventory File found is created much before the time difference [ {0} minutes ] specified.", timeDifference);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("No inventory file found for the selected identity.");
                return false;
            }
        }

    }
}