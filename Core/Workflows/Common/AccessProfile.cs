using System;
using System.Threading;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    /// <summary>
    /// Gets instance of <see cref="AccessProfile"/> that takes base URL
    /// </summary>
    public class AccessProfile
    {
        private IWebDriver webDriver;
        private B2BHomePage b2BHomePage;
        private B2BCustomerProfileListPage b2BCustomerProfileListPage;
        private B2BManageProfileIdentitiesPage b2BManageProfileIdentitiesPage;
        private B2BProfileSettingsGeneralPage b2BProfileSettingsGeneralPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessProfile"/> class.
        /// </summary>
        /// <param name="webDriver"></param>
        public AccessProfile(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
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
        }

        /// <summary>
        /// Creates a new profile with the CustomerSet & AccessGroup provided 
        /// and navigates to the Buyer Catalog Tab
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="customerSet"></param>
        /// <param name="accessGroup"></param>
        /// <param name="profileNameBase"></param>
        public string CreateNewProfile(string environment, string customerSet, string accessGroup, string profileNameBase)
        {
            var newProfileName = profileNameBase + DateTime.Now.ToString("yyMMddHHmmss");

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

            Console.WriteLine("New profile created with Name: {0}", newProfileName);
            return newProfileName;
        }

        /// <summary>
        /// Waits for the Web Page to load
        /// </summary>
        public void WaitForPageRefresh()
        {
            var isloaded = string.Empty;
            do
            {
                Thread.Sleep(4000);

                try
                {
                    isloaded = ((IJavaScriptExecutor)webDriver).ExecuteScript("return window.document.readyState") as string;
                }
                catch
                {
                    // ignored
                }
            } while (isloaded != "complete");
        }
    }
}
