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

        public CatalogAndPricing(IWebDriver Driver)
        {
            webDriver = Driver;
        }

        /// <summary>
        /// Find current Affinity ID, which we can use to match during verification
        /// </summary>
        public String FindCurrentAffinityId()
        {
            return CatalogAndPricingPage.AffinityId.GetAttribute("value");
        }

        /// <summary>
        /// Find current Affinity ID, which we can use to match during verification
        /// </summary>
        public void OpenCatalog(String accountId)
        {
            OstHomePage.GoToCatalogAndPricingPage(accountId);
        }

        /// <summary>
        /// Find Affinity ID after providing alphabets as affinity id
        /// </summary>
        public String AlphabetsAffinityIdNegativeTest(String alphaAffinity)
        {
            CatalogAndPricingPage.AlphabetAffinityNegative(alphaAffinity);
            String AffinityIdAfterProvidingAlphabet = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return AffinityIdAfterProvidingAlphabet;
        }

        /// <summary>
        /// Find Affinity ID after providing alpha numeric value as affinity id
        /// </summary>
        public String AlphaNumericAffinityIdNegativeTest(String alphaNumericAffinity)
        {
            CatalogAndPricingPage.AlphaNumericAffinityNegative(alphaNumericAffinity);
            String AffinityIdAfterProvidingAlphaNumeric = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return AffinityIdAfterProvidingAlphaNumeric;
        }

        /// <summary>
        /// Find Affinity ID after providing Zero as affinity id
        /// </summary>
        public void ZeroAffinityIdNegativeTest(String num)
        {
            CatalogAndPricingPage.ZeroAffinityNegative(num);
            // Try to update Affinity ID with zero value
            CatalogAndPricingPage.UpdateAffinityId();
        }

        /// </summary>
        /// Find Affinity ID after providing special character as affinity id
        /// </summary>
        public String SpecialCharAffinityIdNegativeTest(String SpecialChAffinity)
        {
            CatalogAndPricingPage.SpecialCharAffinityNegative(SpecialChAffinity);
            String AffinityIdAfterProvidingSpecialChar = CatalogAndPricingPage.AffinityId.GetAttribute("value");
            return AffinityIdAfterProvidingSpecialChar;
        }

        /// </summary>
        /// Find error message after updating affinity id with Zero
        /// </summary>
        public String ErrorMsg()
        {
            return CatalogAndPricingPage.ErrorLabel.Text;
        }
    }
}
