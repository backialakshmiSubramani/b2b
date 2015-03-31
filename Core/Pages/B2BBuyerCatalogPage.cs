// ***********************************************************************
// Author           : AMERICAS\Kaushik_Ganguly
// Created          : 12/24/2014 6:02:07 PM
//
// Last Modified By : AMERICAS\Kaushik_Ganguly
// Last Modified On : 12/24/2014 6:02:07 PM
// ***********************************************************************
// <copyright file="ManageProfileIdentities.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using Channel.PartnerDirect.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BBuyerCatalogPage: PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;


        #region Constructors
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BBuyerCatalogPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            Thread.Sleep(5000);
        }

        #endregion

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return BuyerCatalogTab.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.Contains("ManageProfileIdentities.aspx");
        }


        #region Elements

        private IWebElement _catalogOperation;
        public IWebElement CatalogOperation
        {
            get
            {
                if (_catalogOperation == null)
                {
                    _catalogOperation =
                        webDriver.FindElements(By.Name("ctl00$ContentPageHolder$AutoBhc$rbOrgpub")).FirstOrDefault(e => e.Selected);
                }
                return _catalogOperation;
            }
        }

        private IWebElement _automatedBHCCatalogProcessingRules;
        public IWebElement AutomatedBHCCatalogProcessingRules
        {
            get
            {
                if (_automatedBHCCatalogProcessingRules == null)
                    _automatedBHCCatalogProcessingRules = webDriver.FindElement(By.LinkText("Automated BHC Catalog - Processing Rules"), new TimeSpan(0, 0, 30));
                return _automatedBHCCatalogProcessingRules;
            }
        }
        /// <summary>
        /// BHC Catalog Config BCP Buyer Catalog Checkbox
        /// </summary>
        private IWebElement _catalogConfigBCPBuyerCatalog;
        public IWebElement BCP_BuyerCatalog
        {
            get
            {
                if (_catalogConfigBCPBuyerCatalog == null)
                    _catalogConfigBCPBuyerCatalog = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_BuyerCatalogCheck"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPBuyerCatalog;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP Standard Config Checkbox
        /// </summary>
        private IWebElement _catalogConfigBCPStandardConfig;
        public IWebElement BCP_StandardConfig
        {
            get
            {
                if (_catalogConfigBCPStandardConfig == null)
                    _catalogConfigBCPStandardConfig = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_StandardConfigurations"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPStandardConfig;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP UpSell DownSell checkbox
        /// </summary>
        public IWebElement _catalogConfigBCPUpDnSell;
        public IWebElement BCP_UpSellDownSell
        {
            get
            {
                if (_catalogConfigBCPUpDnSell == null)
                    _catalogConfigBCPUpDnSell = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_UpsellDownsell"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPUpDnSell;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP include option type check box
        /// </summary>
        private IWebElement _catalogConfigBCPIncOptionType;
        public IWebElement BCP_IncludeOptionType
        {
            get
            {
                if (_catalogConfigBCPIncOptionType == null)
                    _catalogConfigBCPIncOptionType = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_IncOptionType"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPIncOptionType;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP Show Final Price Checkbox
        /// </summary>
        private IWebElement catalogConfigBCP_ShowFinalPrice;
        public IWebElement BCP_ShowFinalPrice
        {
            get
            {
                if (catalogConfigBCP_ShowFinalPrice == null)
                    catalogConfigBCP_ShowFinalPrice = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_ShowFinPrice"), new TimeSpan(0, 0, 30));
                return catalogConfigBCP_ShowFinalPrice;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP SNP Checkbox
        /// </summary>
        private IWebElement _catalogConfigBCPSNP;
        public IWebElement BCP_SNP
        {
            get
            {
                if (_catalogConfigBCPSNP == null)
                    _catalogConfigBCPSNP = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_SNP"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPSNP;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP Include Default Options
        /// </summary>
        private IWebElement _catalogConfigBCPIncDefOPtions;
        public IWebElement BCP_IncludeDefaultOptions
        {
            get
            {
                if (_catalogConfigBCPIncDefOPtions == null)
                    _catalogConfigBCPIncDefOPtions = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_IncDefOptions"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPIncDefOPtions;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP Show Absolute Price Checkbox
        /// </summary>
        private IWebElement _catalogConfigBCPShowAbsPrice;
        public IWebElement BCP_ShowAbsPrice
        {
            get
            {
                if (_catalogConfigBCPShowAbsPrice == null)
                    _catalogConfigBCPShowAbsPrice = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_ShowAbsPrice"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCPShowAbsPrice;
            }
        }

        /// <summary>
        /// BHC Catalog Config BCP Skudetail Checkbox
        /// </summary>
        private IWebElement _catalogConfigBCSkuDetail;
        public IWebElement BCP_SkuDetail
        {
            get
            {
                if (_catalogConfigBCSkuDetail == null)
                    _catalogConfigBCSkuDetail = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_SkuDetail"), new TimeSpan(0, 0, 30));
                return _catalogConfigBCSkuDetail;
            }
        }

        /// <summary>
        /// Catalog Configuration SNP checkbox
        /// </summary>
        private IWebElement _catalogConfigSNP;
        public IWebElement CatalogConfigurationSNP
        {
            get
            {
                if (_catalogConfigSNP == null)
                    _catalogConfigSNP = webDriver.FindElement(By.Id("chkSNP"), new TimeSpan(0, 0, 30));
                return _catalogConfigSNP;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Default Options
        /// </summary>
        private IWebElement _catalogConfigIncDefOptions;
        public IWebElement CatalogConfigurationIncludeDefaultOption
        {
            get
            {
                if (_catalogConfigIncDefOptions == null)
                    _catalogConfigIncDefOptions = webDriver.FindElement(By.Id("chk_BC_IncDefOptions"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncDefOptions;
            }
        }

        /// <summary>
        /// Catalog Configuration Show Absolute Configuration Checkbox
        /// </summary>
        private IWebElement _catalogConfigShowAbsPrice;
        public IWebElement CatalogConfigurationShowAbsolutePrice
        {
            get
            {
                if (_catalogConfigShowAbsPrice == null)
                    _catalogConfigShowAbsPrice = webDriver.FindElement(By.Id("chk_BC_ShowAbsPrice"), new TimeSpan(0, 0, 30));
                return _catalogConfigShowAbsPrice;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Option Type CheckBox
        /// </summary>
        private IWebElement _catalogConfigIncOptionType;
        public IWebElement CatalogConfigIncludeOptionType
        {
            get
            {
                if (_catalogConfigIncOptionType == null)
                    _catalogConfigIncOptionType = webDriver.FindElement(By.Id("chk_BC_IncOptionType"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncOptionType;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Show Final Price Checkbox
        /// </summary>
        private IWebElement _catalogConfigShowFinalPrice;
        public IWebElement CatalogConfigShowFinalPrice
        {
            get
            {
                if (_catalogConfigShowFinalPrice == null)
                    _catalogConfigShowFinalPrice = webDriver.FindElement(By.Id("chk_BC_ShowFinPrice"), new TimeSpan(0, 0, 30));
                return _catalogConfigShowFinalPrice;
            }
        }

        /// <summary>
        /// Catalog config include sku details checkbox
        /// </summary>
        private IWebElement _catalogSkuDeail;
        public IWebElement CatalogConfigurationIncludeSkuDetail
        {
            get
            {
                if (_catalogSkuDeail == null)
                    _catalogSkuDeail = webDriver.FindElement(By.Id("chk_BC_SkuDetail"), new TimeSpan(0, 0, 30));
                return _catalogSkuDeail;
            }
        }
        /// <summary>
        /// Buyer Catalog First Identity
        /// </summary>
        private IWebElement _firstIdentity;
        public IWebElement BuyerCatalogFirstIdentity
        {
            get
            {
                if (_firstIdentity == null)
                    _firstIdentity = webDriver.FindElement(By.Id("chklistIdenties_0"), new TimeSpan(0, 0, 30));
                return _firstIdentity;
            }
        }

        /// <summary>
        /// Buyer Catalog Error Message Label
        /// </summary>
        private IWebElement _bhcUIError;
        public IWebElement BuyerCatalogError
        {
            get
            {
                if (_bhcUIError == null)
                    _bhcUIError = webDriver.FindElement(By.Id("ContentPageHolder_lbl_Auto_BhcUI_Error"), new TimeSpan(0, 0, 30));
                return _bhcUIError;
            }
        }

        /// <summary>
        /// Catalog Configuration Standard Checkbox
        /// </summary>
        private IWebElement _catalogConfigStd;
        public IWebElement CatalogConfigurationStandard
        {
            get
            {
                if (_catalogConfigStd == null)
                    _catalogConfigStd = webDriver.FindElement(By.Id("chkStandardConfigurations"), new TimeSpan(0, 0, 30));
                return _catalogConfigStd;
            }
        }

        /// <summary>
        /// Catalog Configuration UpDn Sell checkbox
        /// </summary>
        private IWebElement _catalogConfigUpDnSell;
        public IWebElement CatalogConfigurationUpAndDownSell
        {
            get
            {
                if (_catalogConfigUpDnSell == null)
                    _catalogConfigUpDnSell = webDriver.FindElement(By.Id("chkUpAndDownSell"), new TimeSpan(0, 0, 30));
                return _catalogConfigUpDnSell;
            }
        }
        /// <summary>
        /// Delta Catalog End Date TextBox
        /// </summary>
        private IWebElement _deltaCatalogEnd;
        public IWebElement DeltaCatalogEndDate
        {
            get
            {
                if (_deltaCatalogEnd == null)
                    _deltaCatalogEnd = webDriver.FindElement(By.Id("txt_DeltaCatalogEnd"), new TimeSpan(0, 0, 30));
                return _deltaCatalogEnd;
            }
        }
        /// <summary>
        /// Delta Catalog Start Date TextBox
        /// </summary>
        private IWebElement _deltaCatalogStart;
        public IWebElement DeltaCatalogStartDate
        {
            get
            {
                if (_deltaCatalogStart == null)
                    _deltaCatalogStart = webDriver.FindElement(By.Id("txt_DeltaCatalogStart"), new TimeSpan(0, 0, 30));
                return _deltaCatalogStart;
            }
        }

        /// <summary>
        /// Original Catalog End Date Textbox
        /// </summary>
        private IWebElement _originalCatalogEndDate;
        public IWebElement OriginalCatalogEndDate
        {
            get
            {
                if (_originalCatalogEndDate == null)
                    _originalCatalogEndDate = webDriver.FindElement(By.Id("txt_OrgCatalogEnd"), new TimeSpan(0, 0, 30));
                return _originalCatalogEndDate;
            }
        }

        /// <summary>
        /// Original Catalog Startdate Textbox
        /// </summary>
        private IWebElement _originalCatalogStartDate;
        public IWebElement OriginalCatalogStartDate
        {
            get
            {
                if (_originalCatalogStartDate == null)
                {
                    _originalCatalogStartDate = webDriver.FindElement(By.Id("txt_OrgCatalogStart"));
                }
                return _originalCatalogStartDate;
            }
        }

        /// <summary>
        /// Internal Email Textbox
        /// </summary>
        private IWebElement _internalEmail;
        public IWebElement InternalEMail
        {
            get
            {
                if (_internalEmail == null)
                {
                    _internalEmail = webDriver.FindElement(By.Id("txtInterEmail"));
                }

                return _internalEmail;
            }
        }

        /// <summary>
        /// Customer Email Textbox
        /// </summary>
        private IWebElement _customerEmail;
        public IWebElement CustomerEmail
        {
            get
            {
                if (_customerEmail == null)
                {
                    _customerEmail = webDriver.FindElement(By.Id("txtCustEmail"), new TimeSpan(0, 0, 30));
                }
                return _customerEmail;
            }
        }
        /// <summary>
        /// Catalog Type Label
        /// </summary>
        private IWebElement _catalogType;
        public IWebElement CatalogType
        {
            get
            {
                if (_catalogType == null)
                {
                    _catalogType = webDriver.FindElement(By.Id("lblCatalogType"), new TimeSpan(0, 0, 30));
                }
                return _catalogType;
            }
        }

        /// <summary>
        /// Delta Frequency Weeks Dropdown
        /// </summary>
        private IWebElement _deltaFreqWeeks;
        public IWebElement DeltaFrequencyWeeks
        {
            get
            {
                if (_deltaFreqWeeks == null)
                {
                    _deltaFreqWeeks = webDriver.FindElement(By.Id("ddlDeltaWeeks"), new TimeSpan(0, 0, 30));
                }
                return _deltaFreqWeeks;
            }
        }

        /// <summary>
        /// Original Frequency Weeks Dropdown
        /// </summary>
        private IWebElement _originalFreqWeeks;
        public IWebElement OriginaFrequencyWeeks
        {
            get
            {
                if (_originalFreqWeeks == null)
                {
                    _originalFreqWeeks = webDriver.FindElement(By.Id("ddlOrgWeeks"), new TimeSpan(0, 0, 30));
                }
                return _originalFreqWeeks;
            }
        }

        /// <summary>
        /// Delta Frequency Months Dropdown
        /// </summary>
        private IWebElement _deltaFreqMonths;
        public IWebElement DeltaFrequencyMonths
        {
            get
            {
                if (_deltaFreqMonths == null)
                {
                    _deltaFreqMonths = webDriver.FindElement(By.Id("ddlDeltaMonths"), new TimeSpan(0, 0, 30));
                }
                return _deltaFreqMonths;
            }
        }

        /// <summary>
        /// Original Catalog Frequency Months Dropdown
        /// </summary>
        private IWebElement _originalFreqMonths;
        public IWebElement OriginaFrequencyMonths
        {
            get
            {
                if (_originalFreqMonths == null)
                {
                    _originalFreqMonths = webDriver.FindElement(By.Id("ddlOrgMonths"), new TimeSpan(0, 0, 30));
                }
                return _originalFreqMonths;
            }
        }

        /// <summary>
        /// Delta Catalog Frequency Days Dropdown
        /// </summary>
        private IWebElement _deltaFreqDays;
        public IWebElement DeltaFrequencyDays
        {
            get
            {
                if (_deltaFreqDays == null)
                {
                    _deltaFreqDays = webDriver.FindElement(By.Id("ddlDeltaDays"), new TimeSpan(0, 0, 30));
                }
                return _deltaFreqDays;
            }
        }

        /// <summary>
        /// Original Catalog Frequency Days Dropdown
        /// </summary>
        private IWebElement _originalFreqDays;
        public IWebElement OriginalFrequencyDays
        {
            get
            {
                if (_originalFreqDays == null)
                {
                    _originalFreqDays = webDriver.FindElement(By.Id("ddlOrgDays"), new TimeSpan(0, 0, 30));
                }

                return _originalFreqDays;
            }
        }

        /// <summary>
        /// Catalog Region Label
        /// </summary>
        private IWebElement _region;
        public IWebElement CatalogRegion
        {
            get
            {
                if (_region == null)
                    _region = webDriver.FindElement(By.Id("lblregion"), new TimeSpan(0, 0, 30));
                return _region;
            }
        }

        /// <summary>
        /// Delta Time of Send Dropdown
        /// </summary>
        private IWebElement _deltaTimeOfSend;
        public IWebElement DeltaTimeOfSend
        {
            get
            {
                if (_deltaTimeOfSend == null)
                    _deltaTimeOfSend = webDriver.FindElement(By.Id("ddlDelTime"), new TimeSpan(0, 0, 30));
                return _deltaTimeOfSend;
            }
        }

        /// <summary>
        /// Delta Day Of Send Dropdown
        /// </summary>
        private IWebElement _deltaDayOfSend;
        public IWebElement DeltaDayOfSend
        {
            get
            {
                if (_deltaDayOfSend == null)
                    _deltaDayOfSend = webDriver.FindElement(By.Id("ddlDelDaySend"), new TimeSpan(0, 0, 30));
                return _deltaDayOfSend;
            }
        }

        /// <summary>
        /// Original Time of Send Dropdown
        /// </summary>
        private IWebElement _originalTimeOfSend;
        public IWebElement OriginalTimeOfSend
        {
            get
            {
                if (_originalTimeOfSend == null)
                    _originalTimeOfSend = webDriver.FindElement(By.Id("ddlOrgTime"), new TimeSpan(0, 0, 30));
                return _originalTimeOfSend;
            }
        }

        /// <summary>
        /// Original Day of Send Dropdown
        /// </summary>
        private IWebElement _originalDayOfSend;
        public IWebElement OriginalDayOfSend
        {
            get
            {
                if (_originalDayOfSend == null)
                {
                    _originalDayOfSend = webDriver.FindElement(By.Id("ddlOrgDaySend"), new TimeSpan(0, 0, 30));
                }

                return _originalDayOfSend;
            }
        }

        /// <summary>
        /// Enable Delta Catalog Checkbox
        /// </summary>
        private IWebElement _enableDeltaCatalog;
        public IWebElement EnableDeltaCatalog
        {
            get
            {
                if (_enableDeltaCatalog == null)
                {
                    _enableDeltaCatalog = webDriver.FindElement(By.Id("chkDelCatlog"), new TimeSpan(0, 0, 30));
                }
                return _enableDeltaCatalog;
            }
        }
        /// <summary>
        /// Enable Original Catalog Checkbox
        /// </summary>
        private IWebElement _enableOriginalCatalogChk;
        public IWebElement EnableOriginalCatalog
        {
            get
            {
                if (_enableOriginalCatalogChk == null)
                {
                    _enableOriginalCatalogChk = webDriver.FindElement(By.Id("chkOrgCatlog"), new TimeSpan(0, 0, 30));
                }
                return _enableOriginalCatalogChk;
            }
        }
        /// <summary>
        /// Update Cross Reference Checkbox
        /// </summary>
        private IWebElement _updateCrossReferenceChk;
        public IWebElement CrossReferenceUpdate
        {
            get
            {
                if (_updateCrossReferenceChk == null)
                    _updateCrossReferenceChk = webDriver.FindElement(By.Id("chkCrossRefUpdate"), new TimeSpan(0, 0, 30));
                return _updateCrossReferenceChk;
            }
        }
        /// <summary>
        /// Automatically create and Publish Checkbox
        /// </summary>
        private IWebElement _autoCreatePublishChk;
        public IWebElement AutoCreatePublish
        {
            get
            {
                if (_autoCreatePublishChk == null)
                {
                    _autoCreatePublishChk = webDriver.FindElement(By.Id("chk_BC_BuyerCatalogCheck"),
                        new TimeSpan(0, 0, 30));
                }
                return _autoCreatePublishChk;
            }
        }

        public ReadOnlyCollection<IWebElement> _identities;
        public ReadOnlyCollection<IWebElement> Identities
        {
            get
            {
                if (_identities == null)
                {
                    _identities = webDriver.FindElements(By.XPath("//input[contains(@id, 'chklistIdenties_')]"),
                        new TimeSpan(0, 0, 30));
                }
                return _identities;
            }
        }
        /// <summary>
        /// Buyer Catalog Tab on Manage Profile Identities Page
        /// </summary>
        private IWebElement _buyerCatalogTab;
        public IWebElement BuyerCatalogTab
        {
            get
            {
                if (_buyerCatalogTab == null)
                {
                    //Thread.Sleep(1000);
                    _buyerCatalogTab =
                        webDriver.FindElement(By.Id("ContentPageHolder_ProfileHeader_hyp_PH_BuyerCatalog"),
                            new TimeSpan(0, 0, 30));
                }
                return _buyerCatalogTab;
            }
        }

        /// <summary>
        /// Label that displays confirmation messages for BuyerCatalogTab
        /// </summary>
        private IWebElement _confirmationLabel;
        public IWebElement ConfirmationLabel
        {
            get
            {
                if (_confirmationLabel == null)
                    _confirmationLabel = webDriver.FindElement(By.Id("ContentPageHolder_lbl_BC_OKmsg"), new TimeSpan(0, 0, 60));
                return _confirmationLabel;
            }
        }

        /// <summary>
        /// Update button for Buyer Catalog Tab
        /// </summary>
        private IWebElement _updateButton;
        public IWebElement UpdateButton
        {
            get
            {
                if (_updateButton == null)
                {
                    _updateButton = webDriver.FindElement(By.Id("ContentPageHolder_lnk_BC_Save"), new TimeSpan(0, 0, 30));
                }
                return _updateButton;
            }
        }


        #endregion

        #region Helper Methods

        public IWebElement SetTextBoxValue(IWebElement element, string value)
        {
            element.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!element.Displayed) return element;
            javaScriptExecutor.ExecuteScript("arguments[0].value=arguments[1]", element, String.Empty);
            javaScriptExecutor.ExecuteScript("arguments[0].value=arguments[1]", element, value);
            return element;
        }

        #endregion
    }
}
