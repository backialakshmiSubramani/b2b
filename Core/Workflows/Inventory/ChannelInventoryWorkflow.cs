using System;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Core.Workflows.Common;
using OpenQA.Selenium;

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
        /// <returns>The <see cref="bool"/>/></returns>
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
        /// <returns>The <see cref="bool"/>/></returns>
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
        /// <returns>The <see cref="bool"/>/></returns>
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
    }
}
