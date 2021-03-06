﻿using System;
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
            Console.WriteLine("Selecting Environment..");
            b2BHomePage.SelectEnvironment(environment);
            Console.WriteLine("Done!");
            Console.WriteLine("Clicking on B2B Profile List..");
            b2BHomePage.ClickB2BProfileList();
            Console.WriteLine("Done!");

            Console.WriteLine("Searching for Profile by Customer Name : {0}", profileName);
            b2BCustomerProfileListPage = new B2BCustomerProfileListPage(webDriver);
            b2BCustomerProfileListPage.SearchProfile("Customer Name", profileName);
            b2BCustomerProfileListPage.ClickSearchedProfile(profileName.ToUpper());

            b2BManageProfileIdentitiesPage = new B2BManageProfileIdentitiesPage(webDriver);
            Console.WriteLine("Opened Profile Page for profile: {0}", profileName);

            Console.WriteLine("Clicking on BuyerCatalogTab..");
            b2BManageProfileIdentitiesPage.BuyerCatalogTab.Click();
            WaitForPageRefresh();
            Console.WriteLine("Done!");
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
