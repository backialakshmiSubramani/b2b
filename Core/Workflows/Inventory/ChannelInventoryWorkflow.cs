﻿using System;
using System.Configuration;
using System.Data;
using System.Linq;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Core.Workflows.Common;
using Modules.Channel.B2B.DAL.Inventory;
using System.IO;

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
        private B2BHomePage b2BHomePage;
        private CPTAutoCatalogInventoryListPage cPTAutoCatalogInventoryListPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelInventoryWorkflow"/> class.
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelInventoryWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            accessProfile = new AccessProfile(webDriver);
            b2BHomePage = new B2BHomePage(webDriver);
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

            if (b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
                b2BBuyerCatalogPage.UpdateButton.Click();
                accessProfile.WaitForPageRefresh();
                b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            }

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
        /// <param name="isPreview"></param>
        /// <returns>The <see cref="DateTime"/></returns>
        private DateTime? GetLastModifiedDate(string profileName, bool isPreview = false)
        {
            b2BEntities =
                new B2BEntities(isPreview
                    ? ConfigurationManager.ConnectionStrings["B2BPrevEntities"].ToString()
                    : ConfigurationManager.ConnectionStrings["B2BProdEntities"].ToString());

            return (from cm in b2BEntities.b2b_profile
                    where cm.user_id == profileName
                    select cm.last_mod_date)
                .FirstOrDefault();
        }

        public bool ClickToRunOnce(string environment, string profileName, string statusMessage, string atsFeedLocation,
            string atsFeedPrefix, string atsFeedExtension, int atsFileTimeDifference)
        {
            DateTime utcTime;

            //Search for the profile and go to Buyer Catalog tab
            accessProfile.GoToBuyerCatalogTab(environment, profileName);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // Click on ClickToRunOnce button
            if (!b2BBuyerCatalogPage.ClickToRunOnce(statusMessage, out utcTime))
            {
                return false;
            }

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // Verify Inventory Feeds generated for the enabled identites in the selected profile
            return VerifyFeedGeneration(environment, atsFeedLocation, atsFeedPrefix, atsFeedExtension, utcTime, atsFileTimeDifference);
        }

        /// <summary>
        /// Verify the feed generation after clicking on ClickToRunOnce button
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        private bool VerifyFeedGeneration(string environment, string atsFeedLocation, string atsFeedPrefix, string atsFeedExtension, DateTime utcTime, int atsFileTimeDifference)
        {
            var centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime,
                TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            var noOfFilesGenerated = 0;
            var directory = new DirectoryInfo(atsFeedLocation);

            var identities = b2BBuyerCatalogPage.GetIdentities();

            if (identities.Any())
            {
                identities.ForEach(identity =>
                {
                    Console.WriteLine("Searching for Inventory Feed for identity : {0}", identity);
                    var newFileName = atsFeedPrefix + "_" + identity + "_" + centralTime.ToString("MMddHHmmss") + "." +
                                      atsFeedExtension;

                    if (directory.EnumerateFiles(newFileName).Any())
                    {
                        noOfFilesGenerated++;
                    }
                    else
                    {
                        Console.WriteLine("No file found with name {0}", newFileName);
                        Console.WriteLine("Getting the latest file for identity: {0}", identity);

                        var filesForIdentity = directory.EnumerateFiles(atsFeedPrefix + "_" + identity + "_" + "*" + atsFeedExtension);

                        if (filesForIdentity.Any())
                        {
                            if (
                                filesForIdentity.OrderByDescending(f => f.CreationTimeUtc)
                                    .First()
                                    .CreationTimeUtc.Subtract(utcTime)
                                    .TotalSeconds <= atsFileTimeDifference)
                            {
                                noOfFilesGenerated++;
                            }
                            else
                            {
                                Console.WriteLine(
                                    "No Inventory Feed created within {0} seconds of 'Click to Run Once' for Identity: {1} in environment: {2}",
                                    atsFileTimeDifference, identity, environment);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Inventory Feed found for Identity: {0} in environment: {1}", identity, environment);
                        }
                    }
                });
            }

            if (identities.Count() == noOfFilesGenerated)
            {
                Console.WriteLine("Inventory Feeds generated for all identities");
                return true;
            }

            Console.WriteLine("No Enabled Identites Found for the profile !!");
            return false;
        }

        public bool VerifyCPTChangesForAutoInventory(RunEnvironment environment, string pageHeader, string inventoryLabel, string searchRecordsLinkText, string autoInventoryDisclaimer)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);

            // Verify the header is changed to 'Auto Catalog & Inventory List'
            if (!cPTAutoCatalogInventoryListPage.PageHeader.Text.Equals(pageHeader))
            {
                return false;
            }

            // Verify the link in the top right corner drop menu is reads 'Auto Catalog & Inventory List'
            cPTAutoCatalogInventoryListPage.DropdownMenuLink.Click();

            if (!cPTAutoCatalogInventoryListPage.DropdownMenuItems.ElementAt(1).Text.Equals(pageHeader))
            {
                return false;
            }

            // Verify the checkbox with Inventory Label is available
            try
            {
                if (!cPTAutoCatalogInventoryListPage.InventoryCheckbox.FindElement(By.XPath("..")).Text.Contains(inventoryLabel))
                {
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Inventory Checkbox not available");
                return false;
            }

            // Verify the search link text is changed to 'Search Records'
            if (!cPTAutoCatalogInventoryListPage.SearchRecordsLink.Text.Equals(searchRecordsLinkText))
            {
                Console.WriteLine("Search Link Text is not: <{0}>", searchRecordsLinkText);
                return false;
            }

            // Verify the Inventory Disclaimer text mactches 'Disclaimer: Not all capability is valid for Inventory record.'
            try
            {
                if (!cPTAutoCatalogInventoryListPage.InventoryDisclaimer.Text.Equals(autoInventoryDisclaimer))
                {
                    Console.WriteLine("Inventory Disclaimer text does not match: <{0}>", autoInventoryDisclaimer);
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Disclaimer not available");
                return false;
            }

            return true;
        }

        public bool VerifyAutoCatalogListPageDoesNotAutoLoad(RunEnvironment environment)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);

            // Verify that default search is not returning any records(Prev and next buttons won't b available)

            try
            {

                if (!cPTAutoCatalogInventoryListPage.WebDriver.FindElement(By.Id("nextUpButton")).IsElementVisible())
                {
                    Console.WriteLine("Records are not loaded");
                    return true;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Records are loaded");
                return false;
            }

            return false;

        }

        public bool VerifyAutoCatalogListPageLinkText(RunEnvironment environment)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            try
            {
                if (!b2BHomePage.AutoCatalogInventoryListPageLink.IsElementVisible())
                {
                    Console.WriteLine("Auto Catalog & Inventory List Page link is not available");
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Auto Catalog & Inventory List Page link is not available");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Use this method to verify if the 'Inventory' checkbox on 'Auto Catalog & Inventory List Page' is selectable
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool IsInventoryCheckboxSelectable(RunEnvironment environment)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.InventoryCheckbox.Click();

            return cPTAutoCatalogInventoryListPage.InventoryCheckbox.Selected;
        }

        /// <summary>
        /// Use this method to verify if the Clear All link clears the results table
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool VerifyClearAllLink(RunEnvironment environment)
        {
            SearchInventoryRecords(environment);
            cPTAutoCatalogInventoryListPage.ClearAllLink.Click();
            return !cPTAutoCatalogInventoryListPage.CatalogListTableHeader.Displayed;
        }

        private void SearchInventoryRecords(RunEnvironment environment)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.InventoryCheckbox.Click();
            cPTAutoCatalogInventoryListPage.SearchRecordsLink.Click();
            PageUtility.WaitForPageRefresh(webDriver);
        }

        /// <summary>
        /// Verifies if all the records displayed in multiple pages are Inventory records
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool VerifyInventorySearchRecords(RunEnvironment environment)
        {
            SearchInventoryRecords(environment);

            if (!cPTAutoCatalogInventoryListPage.AreAllRowsInventory())
            {
                return false;
            }

            if (!cPTAutoCatalogInventoryListPage.PagingSpan.Displayed) return true;

            do
            {
                cPTAutoCatalogInventoryListPage.NextButton.Click();

                if (!cPTAutoCatalogInventoryListPage.AreAllRowsInventory())
                {
                    return false;
                }
            } while (Convert.ToInt32(cPTAutoCatalogInventoryListPage.PageNumberTextbox.GetAttribute("value")) <
                     Convert.ToInt32(cPTAutoCatalogInventoryListPage.PagingSpan.Text.Split(' ').Last()));

            return true;
        }

        /// <summary>
        /// Accesses the B2B Profile & verifies if the 
        /// 'Number of Automated Inventory Feeds' textbox is present
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="noOfOccurrenceLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyPresenceOfNumberOfOccurrenceField(string environment, string profileName,
            string noOfOccurrenceLabelText)
        {
            accessProfile.GoToBuyerCatalogTab(environment, profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            return b2BBuyerCatalogPage.VerifyPresenceOfNumberOfOccurrenceField(noOfOccurrenceLabelText);
        }

        public int VerifyNumberOfMostRecentRecords(RunEnvironment environment)
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.SearchRecordsLink.Click();
            return cPTAutoCatalogInventoryListPage.CatalogListTableRows.Count();
        }


    }

}
