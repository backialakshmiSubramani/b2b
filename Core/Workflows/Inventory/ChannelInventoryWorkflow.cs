using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using FluentAssertions;
using System.Linq;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Core.Workflows.Common;
using System.IO;
using Modules.Channel.ATSServiceReferenceForSTDandSYS;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Workflows.Catalog;
using Modules.Channel.Utilities;
using Modules.Channel.B2B.CatalogXMLTemplates;
using Modules.Channel.B2B.InventoryXMLTemplates;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Text;
using System.ServiceModel;

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
        private B2BHomePage b2BHomePage;
        private CPTAutoCatalogInventoryListPage cPTAutoCatalogInventoryListPage;
        private readonly InventoryServiceClient _inventoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelInventoryWorkflow"/> class.
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelInventoryWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            accessProfile = new AccessProfile(webDriver);
            b2BHomePage = new B2BHomePage(webDriver);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            _inventoryService = new InventoryServiceClient();
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

        public bool ClickToRunOnce(string environment, string profileName, string statusMessage, string atsFeedLocation,
            string atsFeedPrefix, string atsFeedExtension, int atsFileTimeDifference)
        {
            DateTime utcTime;

            //Search for the profile and go to Buyer Catalog tab
            accessProfile.GoToBuyerCatalogTab(environment, profileName);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            // Click on ClickToRunOnce button
            if (!b2BBuyerCatalogPage.ClickToRunOnce(out utcTime))
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

            // Check if the result table is visible

            if (!cPTAutoCatalogInventoryListPage.CatalogsTable.Displayed)
            {
                Console.WriteLine("Records are not loaded");
                return true;
            }

            Console.WriteLine("Records are loaded");
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
        /// <param name="region"></param>
        /// <returns></returns>
        public bool VerifyClearAllLink(RunEnvironment environment, Region region)
        {
            SearchInventoryRecords(environment, region);
            cPTAutoCatalogInventoryListPage.ClearAllLink.Click();
            return !cPTAutoCatalogInventoryListPage.CatalogListTableHeader.Displayed;
        }

        private void SearchInventoryRecords(RunEnvironment environment, Region region, string profileName = "", string identityName = "")
        {
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.SearchInventoryRecords(region, profileName, identityName);
        }
        
        /// <summary>
        /// Verifies if all the records displayed in multiple pages are Inventory records
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool VerifyInventorySearchRecords(RunEnvironment environment, Region region)
        {
            SearchInventoryRecords(environment, region);

            if (!cPTAutoCatalogInventoryListPage.VerifyColumnValue(2, "Inventory"))
            {
                return false;
            }
            int count = 0;
            if (!cPTAutoCatalogInventoryListPage.IsPagingEnabled()) return true;
            if (cPTAutoCatalogInventoryListPage.CatalogListTableRows.Count() != 14) return false;
            
            do
            {
                if (count >= 2) return true;
                
                cPTAutoCatalogInventoryListPage.NextButton.Click();
                if (!cPTAutoCatalogInventoryListPage.VerifyColumnValue(2, "Inventory"))
                {
                    return false;
                }
                count++;
            } while (Convert.ToInt32(cPTAutoCatalogInventoryListPage.PageNumberTextbox.GetAttribute("value")) <
                     Convert.ToInt32(cPTAutoCatalogInventoryListPage.PagingSpan.Text.Split(' ').Last()));

            return true;
        }

        /// <summary>
        /// Verify the number of records displayed per page on CPT UI
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public int VerifyNumberOfMostRecentRecords(RunEnvironment environment, Region region)
        {
            SearchInventoryRecords(environment, region);
            return cPTAutoCatalogInventoryListPage.CatalogListTableRows.Count();
        }

        /// <summary>
        /// Verify the 'Help - Auto Inventory' Section
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="autoInventoryHelpText"></param>
        /// <returns></returns>
        public bool VerifyAutoInventoryHelpSection(RunEnvironment environment, string profileName,
            string autoInventoryHelpText)
        {
            accessProfile.GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            return b2BBuyerCatalogPage.VerifyAutoInventoryHelpSection(autoInventoryHelpText);
        }

        /// <summary>
        /// Verify if the inventory feeds are filtered by region
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool VerifyAutoInventoryFilteringByRegion(RunEnvironment environment, Region region)
        {
            SearchInventoryRecords(environment, region);

            if (cPTAutoCatalogInventoryListPage.ShowhideInventoryMessage.Displayed)
                return true;
            if (!cPTAutoCatalogInventoryListPage.CatalogsTable.GetCellValueForInventory(1, "Region").Equals(region.ToString()))
                return false; 
            /*
            if (!cPTAutoCatalogInventoryListPage.VerifyColumnValue(15, region.ToString()))
            {
                return false;
            }
            */
            if (!cPTAutoCatalogInventoryListPage.IsPagingEnabled()) return true;
            int count = 1;
            do
            {
                cPTAutoCatalogInventoryListPage.NextButton.Click();
                if (count >= 2) return true;
                if (!cPTAutoCatalogInventoryListPage.CatalogsTable.GetCellValueForInventory(1, "Region").Equals(region.ToString()))
                { return false; }
                /*if (!cPTAutoCatalogInventoryListPage.VerifyColumnValue(14, region.ToString()))
                {
                    return false;
                }*/
                count++;
            } while (Convert.ToInt32(cPTAutoCatalogInventoryListPage.PageNumberTextbox.GetAttribute("value")) <
                     Convert.ToInt32(cPTAutoCatalogInventoryListPage.PagingSpan.Text.Split(' ').Last()));

            return true;
        }

        /// <summary>
        /// Verifies if changes to Auto Inventory are tracked in Audit History
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="windowsLogin"></param>
        /// <param name="oldInterval"></param>
        /// <param name="newInterval"></param>
        /// <param name="oldOffset"></param>
        /// <param name="newOffset"></param>
        /// <returns></returns>
        public bool VerifyAuditHistoryForAutoInventory(RunEnvironment environment, string profileName,
            string windowsLogin, string oldInterval, string newInterval, string oldOffset, string newOffset)
        {
            var expectedEditedBy = "Edited By " + windowsLogin;
            accessProfile.GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            //Verify changes to 'Enable Automated Inventory Feed' are tracked
            var oldValue = b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected;
            var newValue = !oldValue;
            b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            b2BBuyerCatalogPage.UpdateButton.Click();

            if (
                !VerifyAuditHistoryRow(oldValue.ToString(), newValue.ToString(),
                    "Enable Automated Inventory Feed", expectedEditedBy))
            {
                return false;
            }

            //Verify changes to 'Automated Inventory Refresh Interval' are tracked
            var oldIntervalValue = SetAutoInventoryRefreshInterval(oldInterval);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            var newIntervalValue = SetAutoInventoryRefreshInterval(newInterval);

            if (
                !VerifyAuditHistoryRow(oldIntervalValue, newIntervalValue, "Automated Inventory Refresh Interval",
                    expectedEditedBy))
            {
                return false;
            }

            //Verify the changes to 'Minimum Delay post AutoCatalog' are tracked
            SetAutoInventoryOffset(oldOffset);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            SetAutoInventoryOffset(newOffset);


            if (!VerifyAuditHistoryRow(oldOffset, newOffset, "TimeOffset", expectedEditedBy))
            {
                return false;
            }

            //Verify click of 'Click to Run Once' is tracked'
            b2BBuyerCatalogPage.ClickToRunOnceButton.Click();
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            webDriver.Navigate().Refresh();

            return VerifyAuditHistoryRow("False", "True", "Enable Manual Inventory Feed", expectedEditedBy);
        }

        private string SetAutoInventoryRefreshInterval(string interval)
        {
            var splitInterval = interval.Split('/');

            if (!b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            }

            if (!splitInterval[0].Equals("0"))
            {
                b2BBuyerCatalogPage.AutoInventoryDaysDropdown.Select().SelectByValue(splitInterval[0]);
            }

            if (!splitInterval[1].Equals("0"))
            {
                b2BBuyerCatalogPage.AutoInventoryHoursDropdown.Select().SelectByValue(splitInterval[1]);
            }

            if (!splitInterval[2].Equals("0"))
            {
                b2BBuyerCatalogPage.AutoInventoryMinutesDropdown.Select().SelectByValue(splitInterval[2]);
            }

            b2BBuyerCatalogPage.UpdateButton.Click();

            return splitInterval[0] + " days " + splitInterval[1] + " hours " + splitInterval[2] + " minutes";
        }

        private void SetAutoInventoryOffset(string offset)
        {
            if (!b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Selected)
            {
                b2BBuyerCatalogPage.EnableAutoInventoryCheckbox.Click();
            }

            b2BBuyerCatalogPage.AutoInventoryOffset.Select().SelectByValue(offset);
            b2BBuyerCatalogPage.UpdateButton.Click();
        }

        private bool VerifyAuditHistoryRow(string expectedOldValue, string expectedNewValue, string auditHistoryProperty,
            string expectedEditedBy)
        {
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            b2BBuyerCatalogPage.AuditHistoryLink.Click();

            var editedBy = b2BBuyerCatalogPage.AuditHistoryRows[0].FindElements(By.TagName("td"))[0].Text;

            //Skip the first timestamp row & take only the field change rows before the next timestamp row
            IList<IWebElement> immediateRows = new List<IWebElement>();

            foreach (var row in b2BBuyerCatalogPage.AuditHistoryRows.Skip(1))
            {
                if (!row.FindElement(By.TagName("td")).GetAttribute("class").Equals("auditHeader"))
                {
                    //Take only field change rows
                    immediateRows.Add(row);
                }
                else
                {
                    //Skip timestamp rows
                    break;
                }
            }

            var auditHistoryRow =
                immediateRows.FirstOrDefault(r => r.FindElements(By.TagName("td"))[0].Text.Equals(auditHistoryProperty));

            if (auditHistoryRow == null)
            {
                Console.WriteLine("No latest audit history row found for {0}", auditHistoryProperty);
            }

            var oldValue = auditHistoryRow.FindElements(By.TagName("td"))[1].Text;
            Console.WriteLine(oldValue);
            var newValue = auditHistoryRow.FindElements(By.TagName("td"))[2].Text;
            Console.WriteLine(newValue);


            return editedBy.Contains(expectedEditedBy) && oldValue.Equals(expectedOldValue) &&
                   newValue.Equals(expectedNewValue);

        }

        public bool VerifyAutoInventoryMinimumDelayPostAutoCatalogField(RunEnvironment environment, string profileName, string autoInventoryMinimumDelayPostAutoCatalogText)
        {
            accessProfile.GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            return
                b2BBuyerCatalogPage.VerifyAutoInventoryMinimumDelayPostAutoCatalog(
                    autoInventoryMinimumDelayPostAutoCatalogText);

        }

        public bool VerifyRestrictionOfInventoryIntervalToOneType(RunEnvironment environment, string profileName)
        {
            accessProfile.GoToBuyerCatalogTab(environment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);

            return b2BBuyerCatalogPage.VerifyRestrictionOfInventoryIntervalToOneType();
        }

        /// <summary>
        /// Verifies the Display order of the EMEA countries in the dropdown.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="expectedCountries"></param>
        /// <param name="region"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyDisplayOrderOfCountriesForEMEA(RunEnvironment environment, string profileName,
            string expectedCountries, Region region)
        {
            return VerifySearchResultsInCountryDropdown(environment, profileName, expectedCountries, region);
        }

        /// <summary>
        /// Verifies the search result if a search string is given
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="expectedSearchResults"></param>
        /// <param name="region"></param>
        /// <param name="searchString"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifySearchResultsInCountryDropdown(RunEnvironment environment, string profileName,
            string expectedSearchResults, Region region, string searchString = "")
        {
            var expectedSearchResult = expectedSearchResults.Split('/').ToList();

            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.SelectTheRegion(region.ToString());
            var actualSearchResult = cPTAutoCatalogInventoryListPage.GetCountryText(searchString);

            if (actualSearchResult.Count() > expectedSearchResult.Count())
            {
                Console.WriteLine("Expected {0} countries. Found {1} countries.", expectedSearchResult.Count(), actualSearchResult.Count());
                return false;
            }
            return true;
            //return actualSearchResult.SequenceEqual(expectedSearchResult);
        }

        /// <summary>
        /// Verifies the Inventory Records deleted over 60 days
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        /// <param name="maximumNoOfDays"></param>
        /// <param name="region"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyInventoryRecordsDeletedOverSixtyDays(RunEnvironment environment, string profileName,
            int maximumNoOfDays, Region region)
        {
            //Get the date that is 60 days old. Get only the date part as the job runs at 12:00 am CST
            var dateBeforeInterval = DateTime.Now.AddDays(-1 * maximumNoOfDays).Date;

            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.SearchInventoryRecords(Region.US);

            if (cPTAutoCatalogInventoryListPage.IsPagingEnabled())
            {
                var totalPages = cPTAutoCatalogInventoryListPage.PagingSpan.Text.Split(' ').Last();

                cPTAutoCatalogInventoryListPage.PageNumberTextbox.Clear();
                cPTAutoCatalogInventoryListPage.PageNumberTextbox.SendKeys(totalPages);
            }

            Console.WriteLine(cPTAutoCatalogInventoryListPage.CatalogListTableRows.Last());
            Console.WriteLine(cPTAutoCatalogInventoryListPage.CatalogListTableRows.Last().FindElements(By.TagName("td"))[6].Text);
            var oldestRecordDate =
                Convert.ToDateTime(
                    cPTAutoCatalogInventoryListPage.CatalogListTableRows.Last().FindElements(By.TagName("td"))[6].Text);

            return oldestRecordDate >= dateBeforeInterval;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="retryAndscheduleRunMessage"></param>
        /// <param name="region"></param>
        /// <param name="failed"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyRetryInformationInPopUp(RunEnvironment environment, string retryAndscheduleRunMessage,
            Region region, CatalogStatus failed)
        {
            var retryMessageList = retryAndscheduleRunMessage.Split('/').ToList();
            b2BHomePage.SelectEnvironment(environment.ToString());
            b2BHomePage.OpenAutoCatalogInventoryListPage();
            cPTAutoCatalogInventoryListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            cPTAutoCatalogInventoryListPage.SelectTheStatus(failed.ToString());
            cPTAutoCatalogInventoryListPage.SearchInventoryRecords(region);
            IList<IWebElement> rowsPerPage = cPTAutoCatalogInventoryListPage.CatalogListTableRows;

            if (rowsPerPage.Count() > 1)
            {
                for (var i = 0; i <= rowsPerPage.Count(); i++)
                {
                    cPTAutoCatalogInventoryListPage.FailureReasonTooltip.Click();
                    var actualPopUpText =
                        cPTAutoCatalogInventoryListPage.FailureReason.Remove(19);

                    if (!retryMessageList.Contains(actualPopUpText))
                    {
                        return false;
                    }
                    cPTAutoCatalogInventoryListPage.SubmitInPopUp.Click();
                }
                return true;
            }

            Console.WriteLine("No Inventory records");
            return false;
        }

        
        /// <summary>
        /// Retrieve Restock Info.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="restockDate"></param>
        /// <param name="restockQuantity"></param>
        private static void RetrieveRestockInfo(InventoryInfoResponseProduct product, out string restockDate,
            out string restockQuantity)
        {
            // Set default values for Restock information when there is Error.
            restockDate = string.Empty;
            restockQuantity = string.Empty;

            // Set Restock information values when there is no Error. 
            if (string.IsNullOrEmpty(product.ErrorMessage) &&
                (product.RestockInfoList != null && product.RestockInfoList.Length > 0))
            {
                DateTime restockDateOutput;
                var earliestProductInfo = GetEarliestProductInfo(product);

                if (earliestProductInfo != null &&
                    DateTime.TryParse(earliestProductInfo.EstimatedArrivalDate, out restockDateOutput))
                {
                    restockDate = restockDateOutput.ToString("MM/dd/yyyy HH:mm");
                    restockQuantity = earliestProductInfo.Quantity.ToString();
                }
            }
        }

        /// <summary>
        /// Retrieve Inventory Info.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="availableQuantity"></param>
        /// <param name="leadTime"></param>
        private static void RetrieveInventoryInfo(InventoryInfoResponseProduct product, out string availableQuantity,
            out string leadTime)
        {
            // Set default Inventory info values when there is Error.
            availableQuantity = "0";
            leadTime = "10";

            var productType = product.ProductType != null ? product.ProductType.ToUpper() : "";

            // Default value for BTO when there is Error.
            if (!string.IsNullOrEmpty(product.ErrorMessage) && productType == "BTO")
            {
                availableQuantity = "9999";
            }

            // Set Inventory info values when there is no Error.
            if (string.IsNullOrEmpty(product.ErrorMessage) && product.InventoryInformation != null)
            {
                if (product.InventoryInformation.AvailableQuantity.HasValue)
                {
                    if (product.InventoryInformation.AvailableQuantity.Value > 0)
                    {
                        availableQuantity = product.InventoryInformation.AvailableQuantity.Value.ToString();
                    }
                    else if (product.InventoryInformation.AvailableQuantity.Value <= 0 && productType == "BTO")
                    {
                        availableQuantity = "9999";
                    }
                }

                leadTime = "0";
                if (product.InventoryInformation.LeadTime.HasValue)
                {
                    leadTime = product.InventoryInformation.LeadTime.Value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the earliest product info from present and future restock dates.  
        /// </summary>
        /// <param name="product"></param>
        /// <returns><see cref="InventoryInfoResponseReStockDetail"/></returns>
        private static InventoryInfoResponseReStockDetail GetEarliestProductInfo(InventoryInfoResponseProduct product)
        {
            var curreDateTime =
                Convert.ToDateTime(string.Format("{0} 12:00:00 AM", DateTime.Now.ToShortDateString()));

            var restockEarliestDateTimeOutput = DateTime.MinValue;

            // Considering only present and future dates and ignoring past dates.
            var validReStockDetail = (from reStockDetail in product.RestockInfoList
                                      where DateTime.TryParse(reStockDetail.EstimatedArrivalDate, out restockEarliestDateTimeOutput)
                                            && restockEarliestDateTimeOutput >= curreDateTime
                                      select reStockDetail).OrderBy(x => Convert.ToDateTime(x.EstimatedArrivalDate));

            return validReStockDetail.Any() ? validReStockDetail.First() : null;
        }

        /// <summary>
        /// Returns downloaded catalogName
        /// </summary>
        /// <param name="b2BEnvironment"></param>
        /// <param name="region"></param>
        /// <param name="profileName"></param>
        /// <param name="identityName"></param>
        /// <param name="catalogStatus"></param>
        /// <param name="catalogType"></param>
        /// <returns></returns>
        public IEnumerable<CatalogItem> GetCatalogItems(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            DateTime beforeSchedTime = DateTime.Now.AddDays(-365);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, true);
            //uxWorkflow.ValidateCatalogSearchResult(catalogType, catalogStatus, beforeSchedTime, RequestorValidation.Off);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);

            Modules.Channel.B2B.CatalogXMLTemplates.B2BXML actualcatalogXML = XMLDeserializer<Modules.Channel.B2B.CatalogXMLTemplates.B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;

            return actualCatalogDetails.CatalogItem.Where(ci => ci.DeltaChange != DeltaStatus.Remove);
        }

        /// <summary>
        /// Returns downloaded catalogName
        /// </summary>
        /// <param name="b2BEnvironment"></param>
        /// <param name="region"></param>
        /// <param name="profileName"></param>
        /// <param name="identityName"></param>
        /// <param name="catalogStatus"></param>
        /// <param name="catalogType"></param>
        /// <returns></returns>
        public IEnumerable<CatalogItem> GetDeltaCatalogItems(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            DateTime beforeSchedTime = DateTime.Now.AddDays(-365);
            ChannelUxWorkflow uxWorkflow = new ChannelUxWorkflow(webDriver);
            uxWorkflow.SearchCatalog(profileName, identityName, beforeSchedTime, catalogStatus, catalogType, CatalogTestOrLive.None, true);
            uxWorkflow.ValidateCatalogSearchResult(CatalogType.Delta, catalogStatus, beforeSchedTime, RequestorValidation.Off);
            string filePath = uxWorkflow.DownloadCatalog(identityName, beforeSchedTime);
            Modules.Channel.B2B.CatalogXMLTemplates.B2BXML actualcatalogXML = XMLDeserializer<Modules.Channel.B2B.CatalogXMLTemplates.B2BXML>.DeserializeFromXmlFile(filePath);
            CatalogDetails actualCatalogDetails = actualcatalogXML.BuyerCatalog.CatalogDetails;
            return actualCatalogDetails.CatalogItem.Where(ci => ci.DeltaChange == DeltaStatus.Remove);
        }

        public IEnumerable<Item> GetInventoryFeedItems(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ClickToRunOnce(b2BEnvironment, profileName).Should().BeTrue("Inventory feed publsih unsuccessful");
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            ValidateInvenotryFeedSearchResult(catalogType, catalogStatus, beforeSchedTime);
            string filePath = DownloadInvenotryFeed(identityName, beforeSchedTime);
            Modules.Channel.B2B.InventoryXMLTemplates.B2BXML actualInventoryXML = XMLDeserializer<Modules.Channel.B2B.InventoryXMLTemplates.B2BXML>.DeserializeFromXmlFile(filePath);
            InventoryDetails actualInventoryDetails = actualInventoryXML.Inventory.InventoryDetails;
            
            return actualInventoryDetails.Item;
        }

        public string CreateOnDemandInventoryFeed(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ClickToRunOnce(b2BEnvironment, profileName).Should().BeTrue("Inventory feed publsih unsuccessful");
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            ValidateInvenotryFeedSearchResult(catalogType, catalogStatus, beforeSchedTime);
            return DownloadInvenotryFeed(identityName, beforeSchedTime);
        }
        public bool VerifyInventoryInformation(B2BEnvironment b2BEnvironment, Region region, CatalogItemType[] catalogItemType, ProductType[] productType, string profileName, 
            string identityName, CatalogStatus catalogStatus, CatalogType catalogType, string countryCode, string sender, string requestUrl)
        {
            bool matchFlag = true;
            var inventoryItems = GetInventoryFeedItems(b2BEnvironment, region, profileName, identityName, catalogStatus, catalogType);
            var catalogItems = GetCatalogItems(b2BEnvironment, region, profileName, identityName, catalogStatus, catalogType);

            inventoryItems.Count().ShouldBeEquivalentTo(catalogItems.Count(), "Items in Inventory Feed file: " + inventoryItems.Count() + " is not matching with Items in Catalog File: "+ catalogItems.Count());

            foreach (CatalogItemType itemType in catalogItemType)
            {
                if (itemType.Equals(CatalogItemType.SNP))
                {
                    matchFlag = ValidateInventoryInfoForSnPItems(countryCode, requestUrl, inventoryItems, catalogItems, matchFlag);
                }
                else if (itemType.Equals(CatalogItemType.ConfigWithDefaultOptions) || itemType.Equals(CatalogItemType.ConfigWithUpsellDownsell) || itemType.Equals(CatalogItemType.Systems))
                {
                    foreach (ProductType prod in productType)
                    {
                        if (prod == ProductType.DWC)
                        {
                            //Validate all DWC Items
                            IEnumerable<CatalogItem> DWCCatalogItems = catalogItems.Where(item => (item.BaseSKUId == ProductType.DWC.ToString() && item.CatalogItemType == itemType)).ToList();
                            if (DWCCatalogItems.Count() > 0)
                            {
                                IEnumerable<Item> DWCInventoryItems = inventoryItems.Where(item => item.SKU == ProductType.DWC.ToString()).ToList();
                                foreach (Item item in DWCInventoryItems)
                                {
                                    matchFlag &= UtilityMethods.CompareValues<string>("LeadTime for MPN & SKU: '" + item.ManufacturerPartNumber + "," + item.SKU + "':", item.LeadTime, "10");
                                    matchFlag &= UtilityMethods.CompareValues<string>("Inventory Quantity for MPN & SKU: '" + item.ManufacturerPartNumber + "," + item.SKU + "':", item.InventoryQty, "0");
                                    matchFlag &= UtilityMethods.CompareValues<string>("Retsock Date for SKU: " + item.SKU + "':", item.RestockDate, string.Empty);
                                    matchFlag &= UtilityMethods.CompareValues<string>("Restock Quantity for SKU: " + item.SKU + "':", item.RestockQuantity, string.Empty);
                                }
                            }
                        }
                        else if (prod == ProductType.BTO)
                        {//Validate all BTO Items
                            IEnumerable<CatalogItem> BTOCatalogItems = catalogItems.Where(item => item.ItemType == ProductType.BTO.ToString()).ToList();
                            IEnumerable<Item> BTOInventoryItems = inventoryItems.Where(Cat => BTOCatalogItems.Any(Inv => Cat.ManufacturerPartNumber == Inv.ManufacturerPartNumber)).ToList();
                            foreach (Item item in BTOInventoryItems)
                            {
                                matchFlag &= UtilityMethods.CompareValues<string>("LeadTime for MPN & SKU: '" + item.ManufacturerPartNumber + "," + item.SKU + "':", item.LeadTime, "10");
                                matchFlag &= UtilityMethods.CompareValues<string>("Inventory Quantity for MPN & SKU: '" + item.ManufacturerPartNumber + "," + item.SKU + "':", item.InventoryQty, "9999");
                                matchFlag &= UtilityMethods.CompareValues<string>("Retsock Date for SKU: " + item.SKU + "':", item.RestockDate, string.Empty);
                                matchFlag &= UtilityMethods.CompareValues<string>("Restock Quantity for SKU: " + item.SKU + "':", item.RestockQuantity, string.Empty);
                            }
                        }
                        else if (prod == ProductType.BTS || prod == ProductType.BTP)
                        {
                            //IEnumerable<CatalogItem> BTSBTPCatalogItems = catalogItems.Where(item => item.CatalogItemType.ToString() == CatalogItemType.Systems.ToString()
                            //                                                                || item.CatalogItemType.ToString() == CatalogItemType.ConfigWithDefaultOptions.ToString()
                            //                                                                || item.CatalogItemType.ToString() == CatalogItemType.ConfigWithUpsellDownsell.ToString())
                            //                                                          .Where(item => (item.ItemType == ProductType.BTS.ToString() || item.ItemType == ProductType.BTP.ToString()));
                            IEnumerable<CatalogItem> BTSBTPCatalogItems = catalogItems.Where(item => item.CatalogItemType == itemType)
                                                                                      .Where(item => item.ItemType.ToString() == prod.ToString());

                            Dictionary<string, string> mpnSkuDict = BTSBTPCatalogItems.GroupBy(j => j.ItemSKUinfo)
                                                                    .Where(g => g.Count() >= 1)
                                                                    .Select(g => g.First()).ToDictionary(j => j.ItemSKUinfo, k => k.ManufacturerPartNumber);

                            var batchesOfSkus = BTSBTPCatalogItems.Select((p, index) => new { p, index })
                                                                                      .GroupBy(a => a.index / 300)
                                                                                      .Select((grp => grp.Select(g => g.p).ToList())).ToArray();
                            
                            string url = ConfigurationManager.AppSettings["ATSServiceURLForSTDandSYS"];
                            var remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(url));
                            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                            basicHttpBinding.MaxBufferSize = 2147483647;
                            basicHttpBinding.MaxReceivedMessageSize = 2147483647;

                            return batchesOfSkus.All(batchOfSkus =>
                            {
                                List<InventoryInfoRequestProduct> products = new List<InventoryInfoRequestProduct>();
                                batchOfSkus.ForEach(item =>
                                    {
                                        InventoryInfoRequestProduct t = new InventoryInfoRequestProduct();
                                        t.SKU = item.ItemSKUinfo;
                                        t.CountryCode = countryCode;
                                        t.ProductType = item.ItemType;
                                        t.Tag = item.BaseSKUId;
                                        products.Add(t);
                                    });

                                var inventoryInfoRequest = new InventoryInfoRequest()
                                {
                                    ProductList = products.ToArray(),
                                    Sender = sender
                                };
                                InventoryInfoResponse inventoryInfoResponse = new InventoryInfoResponse();
                                using (var inventoryServiceClient = new InventoryServiceClient(basicHttpBinding, remoteAddress))
                                {
                                    inventoryInfoResponse = inventoryServiceClient.GetInventoryInfo(inventoryInfoRequest);
                                }
                                //if (inventoryInfoRequest != null)
                                //{
                                //    inventoryInfoResponse = _inventoryService.GetInventoryInfo(inventoryInfoRequest);
                                //}
                                if (inventoryInfoResponse == null)
                                {
                                    Console.WriteLine("Inventory Service did not return any response.");
                                    return false;
                                }
                                // For each product/item verify the values received from ATS Inventory Service
                                foreach (var product in inventoryInfoResponse.ProductList.Products)
                                {
                                    // Get the ManufacturerPartNumber corresponding to the SKU
                                    string manufact = mpnSkuDict[product.SKU];

                                    //Retrieve LT, IQ, RD and RQ from inventoryFeed file
                                    string leadTime = string.Empty;
                                    string inventoryQty = string.Empty;
                                    string restockDate = string.Empty;
                                    string restockQuantity = string.Empty;
                                    CatalogItem baseSKU = null;
                                    Item actualItem = null;
                                    if (string.IsNullOrEmpty(manufact))
                                    {
                                        baseSKU = BTSBTPCatalogItems.Where(item => (item.ItemSKUinfo == product.SKU && string.IsNullOrEmpty(item.ManufacturerPartNumber))).FirstOrDefault();
                                        actualItem = inventoryItems.Where(item => (item.SKU == baseSKU.BaseSKUId && string.IsNullOrEmpty(item.ManufacturerPartNumber))).FirstOrDefault();
                                    }
                                    else
                                    {
                                        baseSKU = BTSBTPCatalogItems.Where(item => (item.ManufacturerPartNumber == manufact && item.ItemSKUinfo == product.SKU)).FirstOrDefault();
                                        actualItem = inventoryItems.Where(item => (item.ManufacturerPartNumber == manufact && item.SKU == baseSKU.BaseSKUId)).FirstOrDefault();
                                    }

                                    if (actualItem != null)
                                    {
                                        leadTime = actualItem.LeadTime;
                                        inventoryQty = actualItem.InventoryQty;
                                        restockDate = actualItem.RestockDate;
                                        restockQuantity = actualItem.RestockQuantity;
                                    }

                                    // Retrieve Restock Info & Inventory Info from the response
                                    string expectedRestockDate; string expectedRestockQuantity; string expectedInventoryQuantity; string expectedLeadTime;
                                    RetrieveRestockInfo(product, out expectedRestockDate, out expectedRestockQuantity);
                                    RetrieveInventoryInfo(product, out expectedInventoryQuantity, out expectedLeadTime);

                                    if (baseSKU.ItemType == ProductType.BTP.ToString())
                                    { expectedInventoryQuantity = "0"; }

                                    matchFlag &= UtilityMethods.CompareValues<string>("LeadTime for MPN & SKU: '" + actualItem.ManufacturerPartNumber + "," + actualItem.SKU + "':", leadTime, expectedLeadTime);
                                    matchFlag &= UtilityMethods.CompareValues<string>("Inventory Quantity for MPN & SKU: '" + actualItem.ManufacturerPartNumber + "," + actualItem.SKU + "':", inventoryQty, expectedInventoryQuantity);
                                    matchFlag &= UtilityMethods.CompareValues<string>("Retsock Date for MPN & SKU'" + actualItem.ManufacturerPartNumber + "," + actualItem.SKU + "':", restockDate, expectedRestockDate);
                                    matchFlag &= UtilityMethods.CompareValues<string>("Restock Quantity for MPN & SKU'" + actualItem.ManufacturerPartNumber + "," + actualItem.SKU + "':", restockQuantity, expectedRestockQuantity);
                                }
                                return matchFlag;
                            });
                        }
                    }
                }
            }
            return matchFlag;
        }

        private static bool ValidateInventoryInfoForSnPItems(string countryCode, string requestUrl, IEnumerable<Item> inventoryItems, IEnumerable<CatalogItem> catalogItems, bool matchFlag)
        {
            Console.WriteLine("-----: SnP Validation Started :----");
            IEnumerable<string> SnPSkuList = catalogItems.Where(item => item.CatalogItemType.ToString() == "SNP").Select(s => s.BaseSKUId).ToList();
            var batchesOfSkus = SnPSkuList.Select((p, index) => new { p, index })
                                .GroupBy(a => a.index / 300)
                                .Select((grp => grp.Select(g => g.p).ToList()))
                                .ToArray();
            return batchesOfSkus.All(batchOfSkus =>
            {
                var requestString = new SkuDetailsRequest
                {
                    CountryCode = countryCode,
                    SkuList = new List<SkuInfoRequest>()
                };

                batchOfSkus.ForEach(
                    snpSkuList =>
                    {
                        requestString.SkuList.Add(new SkuInfoRequest()
                        {
                            Sku = snpSkuList
                        });
                    });

                var serializer = new DataContractJsonSerializer(typeof(SkuDetailsRequest));
                var stream = new MemoryStream();
                serializer.WriteObject(stream, requestString);
                var jsonString = Encoding.UTF8.GetString(stream.ToArray());
                stream.Close();

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    var jsonSerializer = new DataContractJsonSerializer(typeof(SkuDetailsResponse));
                    var skuDetailsResponse =
                        jsonSerializer.ReadObject(response.GetResponseStream()) as SkuDetailsResponse;

                    skuDetailsResponse.SkuList.AsParallel().ForAll(info =>
                    {
                        info.LeadtTime = info.LeadtTime ?? 10;
                        info.Inventory = (info.Inventory < 0) ? 0 : info.Inventory;
                    });

                    foreach (var skuInfoResponse in skuDetailsResponse.SkuList)
                    {
                        foreach (var row in inventoryItems.Where(q => q.SKU.Equals(skuInfoResponse.Sku)))
                        {
                            if (!row.InventoryQty.Equals(skuInfoResponse.Inventory.ToString()))
                            {
                                matchFlag &= UtilityMethods.CompareValues<string>("Inventory Quantity for SKU '" + row.SKU + "':", row.InventoryQty, skuInfoResponse.Inventory.ToString());
                            }
                            if (!row.LeadTime.Equals(skuInfoResponse.LeadtTime.ToString()))
                            {
                                matchFlag &= UtilityMethods.CompareValues<string>("LeadTime for SKU'" + row.SKU + "':", row.LeadTime, skuInfoResponse.LeadtTime.ToString());
                            }
                            matchFlag &= UtilityMethods.CompareValues<string>("Retsock Date for SKU'" + row.SKU + "':", row.RestockDate, string.Empty);
                            matchFlag &= UtilityMethods.CompareValues<string>("Restock Quantity for SKU'" + row.SKU + "':", row.RestockQuantity, string.Empty);
                        }
                    }
                }
                return matchFlag;
            });
        }
        
        public bool ClickToRunOnce(B2BEnvironment b2BEnvironment, string profileName)
        {
            DateTime utcTime;
            //Search for the profile and go to Buyer Catalog tab
            accessProfile.GoToBuyerCatalogTab(b2BEnvironment.ToString(), profileName);

            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            return b2BBuyerCatalogPage.ClickToRunOnce(out utcTime);
        }

        public string ClickToRunOnceWithRequestedBy(B2BEnvironment b2BEnvironment, string profileName)
        {
            DateTime utcTime;
            //Search for the profile and go to Buyer Catalog tab
            accessProfile.GoToBuyerCatalogTab(b2BEnvironment.ToString(), profileName);
            b2BBuyerCatalogPage = new B2BBuyerCatalogPage(webDriver);
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            string requestedByName = b2BBuyerCatalogPage.RequestedBy.Text;
            UtilityMethods.ClickElement(webDriver, b2BBuyerCatalogPage.AutomatedBhcCatalogProcessingRules);
            b2BBuyerCatalogPage.ClickToRunOnce(out utcTime).Should().BeTrue("Inventory feed publsih unsuccessful");
            return requestedByName;
        }

        /// <summary>
        /// Search for a InvenotryFeed in Auto Catalog List & Inventory page 
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <param name="operation">Catalog operation i.e. 'Create' or 'Create & Publish'</param>
        public void SearchInvenotryFeedRecords(string profileName, string identityName, Region region, DateTime anyTimeAfter, CatalogStatus catalogStatus)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.SelectTheStatus(UtilityMethods.ConvertToString(catalogStatus));
            autoCatalogListPage.InventoryCheckbox.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoCatalogListPage.WaitForInventoryInSearchResult(anyTimeAfter.ConvertToUtcTimeZone(), catalogStatus);
        }

        /// <summary>
        /// Search for a InvenotryFeed in Auto Catalog List & Inventory page 
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="identityName">Identity name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <param name="operation">Catalog operation i.e. 'Create' or 'Create & Publish'</param>
        public void ShowNoInvenotryFeedRecordsMessage(string profileName, string identityName, Region region)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.InventoryCheckbox.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.ShowhideInventoryMessage.WaitForElementVisible(TimeSpan.FromSeconds(30));
            autoCatalogListPage.ShowhideInventoryMessage.Displayed.Should().BeTrue();
        }

        /// <summary>
        /// Validate InvenotryFeed details in Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="catalogItemType">Catalog Item Type</param>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="catalogOperation">Catalog Operation</param>
        /// <param name="anyTimeAfter">Time after which catalog was generated</param>
        public void ValidateInvenotryFeedSearchResult(CatalogType catalogType, CatalogStatus catalogStatus, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.CatalogsTable.GetCellValueForInventory(1, "Last Status Date").Trim().ConvertToDateTime().AddMinutes(1).Should().BeAfter(anyTimeAfter.ConvertToUtcTimeZone(), "InventoryFeed is not displayed in Search Result");
            autoCatalogListPage.CatalogsTable.GetCellValueForInventory(1, "Type").Should().Be(catalogType.ConvertToString(), "Expected InventoryFeed type is incorrect");
            autoCatalogListPage.CatalogsTable.GetCellValueForInventory(1, "Status").Should().Be(catalogStatus.ConvertToString(), "InventoryFeed status is not as expected");
        }

        /// <summary>
        /// Download catalog from Auto Catalog List & Inventory page
        /// </summary>
        /// <param name="identityName">Identity Name</param>
        /// <param name="anyTimeAfter">Time after which the catalog is processed</param>
        /// <returns>File name for the downloaded catalog</returns>
        public string DownloadInvenotryFeed(string identityName, DateTime anyTimeAfter)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            UtilityMethods.ClickElement(webDriver, autoCatalogListPage.GetInventoryDownloadButton(1));
            string downloadPath = ConfigurationManager.AppSettings["CatalogDownloadPath"];

            webDriver.WaitForDownLoadToComplete(downloadPath, "Inventory_" + identityName, anyTimeAfter, TimeSpan.FromMinutes(1));
            string fileName = new DirectoryInfo(downloadPath).GetFiles().OrderByDescending(p => p.CreationTime).AsEnumerable()
                .Where(file => file.Name.Contains("Inventory_" + identityName.ToUpper()) && file.CreationTime > anyTimeAfter)
                .FirstOrDefault().FullName;

            return fileName;
        }
        
        public void OpenAutoCatalogAndInventoryListPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoCatalogListPageUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void VerifyNodeNamesInInventoryFeed(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName,
                                                    CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            ClickToRunOnce(b2BEnvironment, profileName).Should().BeTrue("Inventory feed publsih unsuccessful");
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            ValidateInvenotryFeedSearchResult(catalogType, catalogStatus, beforeSchedTime);
            string filePath = DownloadInvenotryFeed(identityName, beforeSchedTime);
            string schemaPath = string.Empty;
            schemaPath = Path.Combine(System.Environment.CurrentDirectory, "InventorySchema.xsd");
            string message = XMLSchemaValidator.ValidateSchema(filePath, schemaPath);
            message.Should().Be(string.Empty, "Error: One or more tags failed scehma validation. Please check the log for complete details");
        }

        public void VerifyInvenotryFeedTableColumnHeaders(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, string columnNames)
        {
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            InvenotryFeedTableColumnHeaders(region, profileName, identityName, columnNames);
        }

        public void InvenotryFeedTableColumnHeaders(Region region, string profileName, string identityName, string columnNames)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectRegionSpan, region.ToString());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectCustomerNameSpan, profileName.ToUpper());
            autoCatalogListPage.SelectOption(autoCatalogListPage.SelectIdentityNameSpan, identityName.ToUpper());
            autoCatalogListPage.InventoryCheckbox.Click();
            autoCatalogListPage.SearchRecordsLink.Click();
            autoCatalogListPage.CatalogsTable.WaitForElementVisible(TimeSpan.FromSeconds(30));
            string[] columns = columnNames.Replace('_', '&').Split(',');
            bool matchFlag = true;
            for (int i = 0; i < columns.Length; i++)
            {
                matchFlag &= UtilityMethods.CompareValues<bool>("'" + columns[i] + "' Column not present: ", autoCatalogListPage.VerifyInventoryFeedCoulmnNames(columns[i]), true);
            }
        }

        public void VerifyInventoryFeedDownloadButtonColor(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName,
                                                    CatalogStatus catalogStatus, string buttonColor)
        {
            DateTime beforeSchedTime = DateTime.Now.AddDays(-180);
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            this.ShowNoInvenotryFeedRecordsMessage(profileName, identityName, region);
        }

        public void ValidateNoInventoryFeedForProfile(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName)
        {
            DateTime beforeSchedTime = DateTime.Now.AddDays(-180);
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            ShowNoInvenotryFeedRecordsMessage(profileName, identityName, region);
        }

        private void DownloadButtonColor(string color)
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            string download = autoCatalogListPage.GetInventoryDownloadButton(1).GetAttribute("src");
            download.Should().Contain(color, "Download button color is not bule");
        }
        public bool VerifyFixedTextFields(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName,
                                                    CatalogStatus catalogStatus, CatalogType catalogType, string expectedFixedTextFieldValues)
        {
            DateTime beforeSchedTime = DateTime.Now; bool matchFlag = true;
            ClickToRunOnce(b2BEnvironment, profileName).Should().BeTrue("Inventory feed publsih unsuccessful");
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            ValidateInvenotryFeedSearchResult(catalogType, catalogStatus, beforeSchedTime);
            string filePath = DownloadInvenotryFeed(identityName, beforeSchedTime);
            Modules.Channel.B2B.InventoryXMLTemplates.B2BXML actualInventoryXML = XMLDeserializer<Modules.Channel.B2B.InventoryXMLTemplates.B2BXML>.DeserializeFromXmlFile(filePath);
            InventoryHeader actualInventoryHeader = actualInventoryXML.Inventory.InventoryHeader;
            var expectedValues = expectedFixedTextFieldValues.Split(',').ToList();

            matchFlag &= UtilityMethods.CompareValues<string>("MeesageType", actualInventoryHeader.MessageType.ToString(), expectedValues[0]);
            matchFlag &= UtilityMethods.CompareValues<string>("CatalogType", actualInventoryHeader.CatalogType.ToString(), expectedValues[1]);

            InventoryDetails actualInventoryDetails = actualInventoryXML.Inventory.InventoryDetails;
            foreach (Item item in actualInventoryDetails.Item)
            {
                matchFlag &= UtilityMethods.CompareValues<string>("UOM", item.UOM.ToString(), expectedValues[2]);
                matchFlag &= UtilityMethods.CompareValues<string>("Location", item.Location.ToString(), expectedValues[3]);
                matchFlag &= UtilityMethods.CompareValues<string>("ProductAction", item.ProductAction.ToString(), expectedValues[4]);
                matchFlag &= UtilityMethods.CompareValues<string>("TransactionSet", item.TransactionSet.ToString(), expectedValues[5]);
            }
            return true;
        }

        /// <summary>
        /// Verifies that the DeltaChange items with value as 'R', are not present in ATSFeed
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUriProduction"></param>
        /// <param name="atsFeedLocationProduction"></param>
        /// <param name="atsFeedPrefix"></param>
        /// <param name="atsFeedExtension"></param>
        /// <param name="b2BBaseUrl"></param>
        /// <param name="b2BRelativeUriProduction"></param>
        /// <param name="identityNameIdPair"></param>
        /// <param name="isLocal"></param>
        /// <param name="isPreview"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyDeltaRemoveItemsAbsentInInventoryFeed(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName,
                                                    CatalogStatus catalogStatus, CatalogType catalogType)
        {
            var inventoryItems = GetInventoryFeedItems(b2BEnvironment, region, profileName, identityName, catalogStatus, catalogType);
            var catalogItems = GetCatalogItems(b2BEnvironment, region, profileName, identityName, catalogStatus, catalogType);
            
            //For catalog items with DeltaChange Value 'R', add the same to a list - 'deltaMPNs'
            //var deltaMPNs = catalogItems.Select(c => c.ManufacturerPartNumber).ToList();
            Dictionary<string, string> mpnSkuDictCatalog = catalogItems.GroupBy(j => j.ItemSKUinfo)
                                                                    .Where(g => g.Count() >= 1)
                                                                    .Select(g => g.First()).ToDictionary(j => j.ItemSKUinfo, k => k.ManufacturerPartNumber);
            if (!mpnSkuDictCatalog.Any())
            {
                Console.WriteLine("There are no Delta products in the catalog. Use correct test data");
                return false;
            }
            Dictionary<string, string> mpnSkuDictInventory = catalogItems.GroupBy(j => j.ItemSKUinfo)
                                                                    .Where(g => g.Count() >= 1)
                                                                    .Select(g => g.First()).ToDictionary(j => j.ItemSKUinfo, k => k.ManufacturerPartNumber);
            if (inventoryItems  == null)
            {
                Console.WriteLine("No feed created");
                return false;
            }

            //Verify that none of the deltaMPNs is present in the feed.
            return mpnSkuDictCatalog.All(d => !mpnSkuDictInventory.Contains(d));
        }
        public void VerifyRequestorName(B2BEnvironment b2BEnvironment, Region region, string profileName, string identityName, CatalogStatus catalogStatus, CatalogType catalogType)
        {
            DateTime beforeSchedTime = DateTime.Now;
            string requestorName = ClickToRunOnceWithRequestedBy(b2BEnvironment, profileName);
            OpenAutoCatalogAndInventoryListPage(b2BEnvironment);
            SearchInvenotryFeedRecords(profileName, identityName, region, beforeSchedTime, catalogStatus);
            ValidateInvenotryFeedSearchResult(catalogType, catalogStatus, beforeSchedTime);
            requestorName.ShouldBeEquivalentTo(GetInvenotryFeedRequestorName(),"Requestor Name is incorrectly displayed in CPT UI");
        }

        public string GetInvenotryFeedRequestorName()
        {
            CPTAutoCatalogInventoryListPage autoCatalogListPage = new CPTAutoCatalogInventoryListPage(webDriver);
            return autoCatalogListPage.CatalogsTable.GetCellValueForInventory(1, "Requester");
        }
    }

    /// <summary>
    /// SKU Info Request sent to SNP Inventory Service.
    /// </summary>
    public class SkuInfoRequest
    {
        /// <summary>
        /// Stock keeping unit item.
        /// </summary>
        public string Sku { get; set; }
    }

    /// <summary>
    /// SKU Details request sent to SNP Inventory service.
    /// </summary>
    public class SkuDetailsRequest
    {
        /// <summary>
        /// Country Code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Stock keeping unit items.
        /// </summary>
        public IList<SkuInfoRequest> SkuList { get; set; }
    }


    /// <summary>
    /// SKU Details response received from SNP Inventory service.
    /// </summary>
    public class SkuDetailsResponse
    {
        /// <summary>
        /// Country Code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Stock keeping unit items.
        /// </summary>
        public IList<SkuInfoResponse> SkuList { get; set; }
    }

    /// <summary>
    /// SKU Info Response received from SNP Inventory Service.
    /// </summary>
    public class SkuInfoResponse
    {
        /// <summary>
        /// Stock keeping unit item.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Lead time.
        /// </summary>
        public int? LeadtTime { get; set; }

        /// <summary>
        /// Inventory count.
        /// </summary>
        public int? Inventory { get; set; }
    }

    /// <summary>
    /// Catalog Product Type
    /// </summary>
    public enum ProductType
    {
        BTS = 0,
        BTO = 1,
        SNP = 2,
        BTP = 3,
        DWC = 4,
        All = 5
    }
}
