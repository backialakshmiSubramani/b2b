// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/2/2014 11:09:45 AM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/2/2014 11:09:45 AM
// ***********************************************************************
// <copyright file="CatalogAndPricing.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Describe what is being tested in this test class here.</summary>
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using Modules.Channel.B2B.Core.Pages;


namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    public class CatalogAndPricing
    {
        private IWebDriver webDriver;

        /// <summary>
        /// Page object creation to use its methods
        /// </summary>
        private OstCatalogAndPricingPage CatalogAndPricingPage
        {
            get { return new OstCatalogAndPricingPage(webDriver); }
        }

        /// <summary>
        /// Gets page object to use its methods
        /// </summary>
        private OstHomePage OstHomePage
        {
            get { return new OstHomePage(webDriver); }
        }

        private B2BHomePage b2BHomePage
        {
            get { return new B2BHomePage(webDriver); }
        }

        private B2BCustomerProfileListPage b2BCustomerProfileListPage
        {
            get { return new B2BCustomerProfileListPage(webDriver); }
        }

        private B2BProfileSettingsGeneralPage b2BProfileSettingsGeneralPage
        {
            get { return new B2BProfileSettingsGeneralPage(webDriver); }
        }

        public CatalogAndPricing(IWebDriver Driver)
        {
            webDriver = Driver;
        }

        /// <summary>
        /// Find current Affinity ID, which we can use to match during verification
        /// </summary>
        public string FindCurrentAffinityId()
        {
            return CatalogAndPricingPage.AffinityId.GetAttribute("value");
        }

        public string FindCurrentAffinityAccountName()
        {
            return CatalogAndPricingPage.AffinityAccountName.Text;
        }

        /// <summary>
        /// Open catalog page with an account name
        /// </summary>
        public void OpenCatalog(string accountId)
        {
            OstHomePage.GoToCatalogAndPricingPage(accountId);
            webDriver.WaitForElementVisible(By.Id("ctl00_ContentPageHolder_txt_AffAccountID"), TimeSpan.FromSeconds(30));
            //webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Find Affinity ID after providing alphabets as affinity id
        /// </summary>
        public string AlphabetsAffinityIdNegativeTest(string alphaAffinity)
        {
            CatalogAndPricingPage.AlphabetAffinityNegative(alphaAffinity);
            string affinityIdAfterProvidingAlphabet = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return affinityIdAfterProvidingAlphabet;
        }

        /// <summary>
        /// Find Affinity ID after providing alpha numeric value as affinity id
        /// </summary>
        public string AlphaNumericAffinityIdNegativeTest(string alphaNumericAffinity)
        {
            CatalogAndPricingPage.AlphaNumericAffinityNegative(alphaNumericAffinity);
            string affinityIdAfterProvidingAlphaNumeric = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return affinityIdAfterProvidingAlphaNumeric;
        }

        /// <summary>
        /// Find Affinity ID after providing Zero as affinity id
        /// </summary>
        public void ZeroAffinityIdNegativeTest(string num)
        {
            CatalogAndPricingPage.ZeroAffinityNegative(num);
            // Try to update Affinity ID with zero value
            CatalogAndPricingPage.UpdateAffinityId();
        }

        /// <summary>
        /// Find Affinity ID after providing special character as affinity id
        /// </summary>
        public String SpecialCharAffinityIdNegativeTest(string SpecialChAffinity)
        {
            CatalogAndPricingPage.SpecialCharAffinityNegative(SpecialChAffinity);
            string affinityIdAfterProvidingSpecialChar = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return affinityIdAfterProvidingSpecialChar;
        }

        /// <summary>
        /// Find Affinity ID after providing alphabets as affinity id
        /// </summary>
        public string NumericAffinityId(string NumericVal)
        {
            webDriver.WaitForElementVisible(By.Id("ctl00_ContentPageHolder_txt_AffAccountID"), TimeSpan.FromSeconds(20));
            CatalogAndPricingPage.NumericAffinityPositive(NumericVal);
            string affinityIdAfterProvidingNumeric = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return affinityIdAfterProvidingNumeric;
        }

        /// <summary>
        /// Find error message after updating affinity id with Zero
        /// </summary>
        public string Msg()
        {
            return CatalogAndPricingPage.ErrorLabel.Text;
        }

        public void UpdateAffinity()
        {
            CatalogAndPricingPage.UpdateAffinityId();
        }

        // searchCriteria - "Customer Name"
        public string CheckProduction(string url, string searchCriteria, string custName)
        {
            webDriver.Navigate().GoToUrl(url);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            b2BHomePage.ClickB2BProfileList();
            webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_ImgBtnUpdate_Top"), TimeSpan.FromSeconds(30));
            b2BCustomerProfileListPage.SearchProfile(searchCriteria, custName);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
            b2BCustomerProfileListPage.ClickValueAfterSearch();
            return b2BProfileSettingsGeneralPage.FindAffinityAccountId();
        }
    }
}
