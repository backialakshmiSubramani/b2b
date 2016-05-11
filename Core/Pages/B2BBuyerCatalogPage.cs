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
using System.IO;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BBuyerCatalogPage : PageBase
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
            //Thread.Sleep(5000);
            PageUtility.WaitForPageRefresh(webDriver);
        }

        #endregion

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return AutomatedBhcCatalogProcessingRules.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLowerInvariant().Contains("buyercatalog.aspx");
        }

        #region Elements

        /// <summary>
        /// Selected Catalog Operaton - Create / Create & Publish
        /// </summary>
        private IWebElement _catalogOperationSelected;
        public IWebElement CatalogOperationSelected
        {
            get
            {
                if (_catalogOperationSelected == null)
                {
                    _catalogOperationSelected =
                        webDriver.FindElements(By.Name("ctl00$ContentPageHolder$AutoBhc$rbOrgpub")).FirstOrDefault(e => e.Selected);
                }
                return _catalogOperationSelected;
            }
        }

        /// <summary>
        /// Catalog Operation - Create
        /// </summary>
        private IWebElement _catalogOperationCreate;
        public IWebElement CatalogOperationCreate
        {
            get
            {
                if (_catalogOperationCreate == null)
                {
                    _catalogOperationCreate = webDriver.FindElement(By.Id("rbOrgpub_0"));
                }
                return _catalogOperationCreate;
            }
        }

        /// <summary>
        /// Catalog Operation - Create & Publish
        /// </summary>
        private IWebElement _catalogOperationCreatePublish;
        public IWebElement CatalogOperationCreatePublish
        {
            get
            {
                if (_catalogOperationCreatePublish == null)
                {
                    _catalogOperationCreatePublish = webDriver.FindElement(By.Id("rbOrgpub_1"));
                }
                return _catalogOperationCreatePublish;
            }
        }

        /// <summary>
        /// Automated BHC Catalog - Processing Rules collapse/ expand
        /// </summary>
        private IWebElement _automatedBhcCatalogProcessingRules;
        public IWebElement AutomatedBhcCatalogProcessingRules
        {
            get
            {
                if (_automatedBhcCatalogProcessingRules == null)
                    _automatedBhcCatalogProcessingRules = webDriver.FindElement(By.LinkText("Automated BHC Catalog - Processing Rules"), new TimeSpan(0, 0, 30));
                return _automatedBhcCatalogProcessingRules;
            }
        }

        /// <summary>
        /// Catalog Enabled checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpCatalogEnabled;
        public IWebElement BcpCatalogEnabled
        {
            get
            {
                if (_bcpCatalogEnabled == null)
                    _bcpCatalogEnabled = webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_BuyerCatalogCheck"), new TimeSpan(0, 0, 30));
                return _bcpCatalogEnabled;
            }
        }

        /// <summary>
        /// Catalog Enabled checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkRemoveItemsWithLTAbove3Days;
        public IWebElement BcpchkRemoveItemsWithLTAbove3Days
        {
            get
            {
                if (_bcpchkRemoveItemsWithLTAbove3Days == null)
                    _bcpchkRemoveItemsWithLTAbove3Days = webDriver.FindElement(By.Id("chkRemoveItemsWithLTAbove3Days"), new TimeSpan(0, 0, 30));
                return _bcpchkRemoveItemsWithLTAbove3Days;
            }
        }

        /// <summary>
        /// Catalog Enabled checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkSysCatalogCheckbox;
        public IWebElement BcpchkSysCatalogCheckbox
        {
            get
            {
                if (_bcpchkSysCatalogCheckbox == null)
                    _bcpchkSysCatalogCheckbox = webDriver.FindElement(By.XPath("//*[@id='chkSystemCatalog']"), new TimeSpan(0, 0, 30));
                return _bcpchkSysCatalogCheckbox;
            }
        }

        /// <summary>
        /// Catalog Configuration SYS:Default Options
        /// </summary>
        private IWebElement _catalogConfigSysDefaultOptionsCheckbox;
        public IWebElement CatalogConfigSysDefaultOptionsCheckbox
        {
            get
            {
                if (_catalogConfigSysDefaultOptionsCheckbox == null)
                    _catalogConfigSysDefaultOptionsCheckbox = webDriver.FindElement(By.Id("chk_BC_IsDefaultOptionEnabledForSystem"), new TimeSpan(0, 0, 30));
                return _catalogConfigSysDefaultOptionsCheckbox;
            }
        }

        /// <summary>
        /// Catalog Configuration SYS:Final Price
        /// </summary>
        private IWebElement _catalogConfigSysFinalPriceCheckbox;
        public IWebElement CatalogConfigSysFinalPriceCheckbox
        {
            get
            {
                if (_catalogConfigSysFinalPriceCheckbox == null)
                    _catalogConfigSysFinalPriceCheckbox = webDriver.FindElement(By.Id("chk_BC_IsFinalPriceEnabledForSystem"), new TimeSpan(0, 0, 30));
                return _catalogConfigSysFinalPriceCheckbox;
            }
        }

        /// <summary>
        /// Catalog Configuration SYS:Final Price
        /// </summary>
        private IWebElement _catalogConfigSysSkuDetailsCheckbox;
        public IWebElement CatalogConfigSysSkuDetailsCheckbox
        {
            get
            {
                if (_catalogConfigSysSkuDetailsCheckbox == null)
                    _catalogConfigSysSkuDetailsCheckbox = webDriver.FindElement(By.Id("chk_BC_IsSkuDetailsEnabledForSystem"), new TimeSpan(0, 0, 30));
                return _catalogConfigSysSkuDetailsCheckbox;
            }
        }

        /// <summary>
        /// Catalog Enabled checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkSPLFlagCheckbox;
        public IWebElement BcpchkSPLFlagCheckbox
        {
            get
            {
                if (_bcpchkSPLFlagCheckbox == null)
                    _bcpchkSPLFlagCheckbox = webDriver.FindElement(By.XPath("//*[@id='chkSPL']"), new TimeSpan(0, 0, 30));
                return _bcpchkSPLFlagCheckbox;
            }
        }
        /// <summary>
        /// Cross reference checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkCrossRefernceUpdate;
        public IWebElement BcpchkCrossRefernceUpdate
        {
            get
            {
                if (_bcpchkCrossRefernceUpdate == null)
                    _bcpchkCrossRefernceUpdate = webDriver.FindElement(By.Id("chkCrossRefUpdate"), new TimeSpan(0, 0, 30));
                return _bcpchkCrossRefernceUpdate;
            }
        }

        /// <summary>
        /// Cross reference Std config checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkCrossRefernceStdUpdate;
        public IWebElement BcpchkCrossRefernceStdUpdate
        {
            get
            {
                if (_bcpchkCrossRefernceStdUpdate == null)
                    _bcpchkCrossRefernceStdUpdate = webDriver.FindElement(By.Id("chkCrossRefSTDUpdate"), new TimeSpan(0, 0, 30));
                return _bcpchkCrossRefernceStdUpdate;
            }
        }

        /// <summary>
        /// Cross reference Snp checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkCrossRefernceSnpUpdate;
        public IWebElement BcpchkCrossRefernceSnpUpdate
        {
            get
            {
                if (_bcpchkCrossRefernceSnpUpdate == null)
                    _bcpchkCrossRefernceSnpUpdate = webDriver.FindElement(By.Id("chkCrossRefSNPUpdate"), new TimeSpan(0, 0, 30));
                return _bcpchkCrossRefernceSnpUpdate;
            }
        }

        /// <summary>
        /// Cross reference Snp checkbox under Buyer Catalog - Processing Rules section
        /// </summary>
        private IWebElement _bcpchkCrossRefernceSysUpdate;
        public IWebElement BcpchkCrossRefernceSysUpdate
        {
            get
            {
                if (_bcpchkCrossRefernceSysUpdate == null)
                    _bcpchkCrossRefernceSysUpdate = webDriver.FindElement(By.Id("chkCrossRefSYSUpdate"), new TimeSpan(0, 0, 30));
                return _bcpchkCrossRefernceSysUpdate;
            }
        }
        /// <summary>
        /// First Identity from the Automated BHC Catalog - Processing Rules section
        /// </summary>
        private IWebElement _buyerCatalogFirstIdentity;
        public IWebElement BuyerCatalogFirstIdentity
        {
            get
            {
                if (_buyerCatalogFirstIdentity == null)
                    _buyerCatalogFirstIdentity = webDriver.FindElement(By.Id("chklistIdenties_0"), new TimeSpan(0, 0, 30));
                return _buyerCatalogFirstIdentity;
            }
        }

        /// <summary>
        /// Buyer Catalog Error Message Label
        /// </summary>
        private IWebElement _buyerCatalogError;
        public IWebElement BuyerCatalogError
        {
            get
            {
                if (_buyerCatalogError == null)
                    _buyerCatalogError = webDriver.FindElement(By.Id("ContentPageHolder_lbl_Auto_BhcUI_Error"), new TimeSpan(0, 0, 30));
                return _buyerCatalogError;
            }
        }

        /// <summary>
        /// List of Configuration checkboxes
        /// </summary>
        public ReadOnlyCollection<IWebElement> ConfigElements
        {
            get
            {
                return webDriver.FindElements(By.XPath("//table[@id='mytable']/tbody/tr[3]/td[2]/table/tbody/tr/td/input"), new TimeSpan(0, 0, 30));
            }
        }

        /// <summary>
        /// Catalog Configuration Standard Checkbox
        /// </summary>
        private IWebElement _catalogConfigStandard;
        public IWebElement CatalogConfigStandard
        {
            get
            {
                if (_catalogConfigStandard == null)
                    _catalogConfigStandard = webDriver.FindElement(By.Id("chkStandardConfigurations"), new TimeSpan(0, 0, 30));
                return _catalogConfigStandard;
            }
        }

        /// <summary>
        /// Catalog Configuration UpDn Sell checkbox
        /// </summary>
        private IWebElement _catalogConfigUpsellDownSell;
        public IWebElement CatalogConfigUpsellDownSell
        {
            get
            {
                if (_catalogConfigUpsellDownSell == null)
                    _catalogConfigUpsellDownSell = webDriver.FindElement(By.Id("chkUpAndDownSell"), new TimeSpan(0, 0, 30));
                return _catalogConfigUpsellDownSell;
            }
        }

        /// <summary>
        /// Catalog Configuration SNP checkbox
        /// </summary>
        private IWebElement _catalogConfigSnP;
        public IWebElement CatalogConfigSnP
        {
            get
            {
                if (_catalogConfigSnP == null)
                    _catalogConfigSnP = webDriver.FindElement(By.Id("chkSNP"), new TimeSpan(0, 0, 30));
                return _catalogConfigSnP;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Default Options
        /// </summary>
        private IWebElement _catalogConfigIncludeDefaultOptions;
        public IWebElement CatalogConfigIncludeDefaultOptions
        {
            get
            {
                if (_catalogConfigIncludeDefaultOptions == null)
                    _catalogConfigIncludeDefaultOptions = webDriver.FindElement(By.Id("chk_BC_IncDefOptions"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncludeDefaultOptions;
            }
        }

        /// <summary>
        /// Catalog Configuration Show Absolute Configuration Checkbox
        /// </summary>
        private IWebElement _catalogConfigIncludeAbsolutePrice;
        public IWebElement CatalogConfigIncludeAbsolutePrice
        {
            get
            {
                if (_catalogConfigIncludeAbsolutePrice == null)
                    _catalogConfigIncludeAbsolutePrice = webDriver.FindElement(By.Id("chk_BC_ShowAbsPrice"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncludeAbsolutePrice;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Option Type CheckBox
        /// </summary>
        private IWebElement _catalogConfigIncludeOptionType;
        public IWebElement CatalogConfigIncludeOptionType
        {
            get
            {
                if (_catalogConfigIncludeOptionType == null)
                    _catalogConfigIncludeOptionType = webDriver.FindElement(By.Id("chk_BC_IncOptionType"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncludeOptionType;
            }
        }

        /// <summary>
        /// Catalog Configuration Include Show Final Price Checkbox
        /// </summary>
        private IWebElement _catalogConfigIncludeFinalPrice;
        public IWebElement CatalogConfigIncludeFinalPrice
        {
            get
            {
                if (_catalogConfigIncludeFinalPrice == null)
                    _catalogConfigIncludeFinalPrice = webDriver.FindElement(By.Id("chk_BC_ShowFinPrice"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncludeFinalPrice;
            }
        }

        /// <summary>
        /// Catalog config include sku details checkbox
        /// </summary>
        private IWebElement _catalogConfigIncludeSkuDetails;
        public IWebElement CatalogConfigIncludeSkuDetails
        {
            get
            {
                if (_catalogConfigIncludeSkuDetails == null)
                    _catalogConfigIncludeSkuDetails = webDriver.FindElement(By.Id("chk_BC_SkuDetail"), new TimeSpan(0, 0, 30));
                return _catalogConfigIncludeSkuDetails;
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
        public IWebElement OriginalFrequencyWeeks
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
        /// Enable BHC Catalog Auto Generation Checkbox
        /// </summary>
        private IWebElement _enableCatalogAutoGeneration;
        public IWebElement EnableCatalogAutoGeneration
        {
            get
            {
                if (_enableCatalogAutoGeneration == null)
                {
                    _enableCatalogAutoGeneration = webDriver.FindElement(By.Id("chk_BC_BuyerCatalogCheck"),
                        new TimeSpan(0, 0, 30));
                }
                return _enableCatalogAutoGeneration;
            }
        }

        /// <summary>
        /// List of all identities in Automated BHC Catalog - Processing Rules section
        /// </summary>
        private ReadOnlyCollection<IWebElement> _identities;
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
        /// Buyer Catalog Tab on the Page
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

        /// <summary>
        /// Requested By field in Automated BHC Catalog - Processing Rules section
        /// </summary>
        private IWebElement _requestedBy;
        public IWebElement RequestedBy
        {
            get
            {
                if (_requestedBy == null)
                {
                    _requestedBy = webDriver.FindElement(By.Id("txtRequestedBy"), new TimeSpan(0, 0, 10));
                }

                return _requestedBy;
            }
        }

        /// <summary>
        /// Edit button under Automated BHC Catalog - Processing Rules section
        /// </summary>
        private IWebElement _editScheduleButton;
        public IWebElement EditScheduleButton
        {
            get
            {
                if (_editScheduleButton == null)
                {
                    _editScheduleButton = webDriver.FindElement(By.Id("btnEditSchedule"), new TimeSpan(0, 0, 10));
                }

                return _editScheduleButton;
            }
        }

        /// <summary>
        /// Audit History Section on Buyer Catalog Page
        /// </summary>
        private IWebElement _AuditHistoryLink;
        public IWebElement AuditHistoryLink
        {

            get
            {
                if (_AuditHistoryLink == null)
                {
                    _AuditHistoryLink = webDriver.FindElement(By.LinkText("Audit History"), new TimeSpan(0, 0, 30));
                }
                return _AuditHistoryLink;
            }
        }

        /// <summary>
        /// Audit History Records
        /// </summary>
        private List<IWebElement> _AuditHistoryRows;
        public List<IWebElement> AuditHistoryRows
        {
            get
            {
                if (_AuditHistoryRows == null)
                {
                    _AuditHistoryRows = webDriver.FindElements(
                        By.XPath("//div[@id='audithistory']/table/tbody/tr/td/div/table/tbody/tr"),
                        new TimeSpan(0, 0, 30)).ToList();
                }
                return _AuditHistoryRows;
            }
        }

        private IWebElement _inventoryFeedProcessingRules;

        /// <summary>
        /// 'Inventory Feed - Processing Rules' section - Collapse/Expand link
        /// </summary>
        public IWebElement InventoryFeedProcessingRules
        {
            get
            {
                return _inventoryFeedProcessingRules ??
                       (_inventoryFeedProcessingRules =
                           webDriver.FindElement(By.LinkText("Inventory Feed - Processing Rules"),
                               new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _inventoryFeedSectionTable;

        /// <summary>
        /// 'Inventory Feed - Processing Rules' section
        /// </summary>
        public IWebElement InventoryFeedSectionTable
        {
            get
            {
                return _inventoryFeedSectionTable ??
                       (_inventoryFeedSectionTable = webDriver.FindElement(By.Id("ContentPageHolder_Table6"),
                           new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _clickToRunOnceButton;

        /// <summary>
        /// 'Click to Run Once' button under 'Inventory Feed - Processing Rules' section
        /// Click to Run Once Button - to perform catalog Inventory Feed manually
        /// </summary>
        public IWebElement ClickToRunOnceButton
        {
            get
            {
                return _clickToRunOnceButton ??
                       (_clickToRunOnceButton = webDriver.FindElement(By.Id("ContentPageHolder_btnClicktoRunOnce"),
                           new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _enableAutoInventoryCheckbox;

        /// <summary>
        /// Enable Automated Inventory Feed Checkbox
        /// </summary>
        public IWebElement EnableAutoInventoryCheckbox
        {
            get
            {
                return _enableAutoInventoryCheckbox ??
                       (_enableAutoInventoryCheckbox =
                           webDriver.FindElement(By.Id("ContentPageHolder_chk_BC_AutomatedATSCheck"),
                               new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _autoInventoryDaysDropdown;

        /// <summary>
        /// Auto Inventory Refresh Interval - Days Dropdown
        /// </summary>
        public IWebElement AutoInventoryDaysDropdown
        {
            get
            {
                return _autoInventoryDaysDropdown ??
                       (_autoInventoryDaysDropdown =
                           webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervaldays"),
                               new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _autoInventoryHoursDropdown;

        /// <summary>
        /// Auto Inventory Refresh Interval - Hours Dropdown
        /// </summary>
        public IWebElement AutoInventoryHoursDropdown
        {
            get
            {
                return _autoInventoryHoursDropdown ??
                       (_autoInventoryHoursDropdown =
                           webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervalhr"),
                               new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _autoInventoryMinutesDropdown;

        /// <summary>
        /// Auto Inventory Refresh Interval - Minutes Dropdown
        /// </summary>
        public IWebElement AutoInventoryMinutesDropdown
        {
            get
            {
                return _autoInventoryMinutesDropdown ??
                       (_autoInventoryMinutesDropdown =
                           webDriver.FindElement(By.Id("ContentPageHolder_dd_AutomatedATSIntervalmn"),
                               new TimeSpan(0, 0, 10)));
            }
        }

        private IWebElement _autoInventoryOffset;

        /// <summary>
        /// Minimum Delay post AutoCatalog for Channel Inventory
        /// </summary>
        public IWebElement AutoInventoryOffset
        {
            get
            {
                return _autoInventoryOffset ?? (_autoInventoryOffset =
                           webDriver.FindElement(By.Id("ContentPageHolder_dd_MinimumDelayPostAutoCatalog"),
                               new TimeSpan(0, 0, 10)));
            }
        }


        private List<IWebElement> _checkedIdentityList;
        public List<IWebElement> CheckedIdentityList
        {
            get
            {
                _checkedIdentityList = webDriver.FindElements(By.XPath("//label[contains(@for,'chklistIdenties_')]"), new TimeSpan(0, 0, 30)).ToList();
                return _checkedIdentityList;
            }
        }

        /// <summary>
        /// Enable BHC Catalog Auto Generation Checkbox
        /// </summary>
        private IWebElement _catalogExpires;
        public IWebElement CatalogExpire
        {
            get
            {
                if (_catalogExpires == null)
                {
                    _catalogExpires = webDriver.FindElement(By.Id("ddlCatalogExpiresInDays"),
                        new TimeSpan(0, 0, 30));
                }
                return _catalogExpires;
            }
        }

        /// <summary>
        /// Expand or the '+' button for 'Help - Auto Inventory' section
        /// </summary>
        private IWebElement _autoInventoryHelpPlus;
        public IWebElement AutoInventoryHelpPlus
        {
            get
            {
                return _autoInventoryHelpPlus ??
                       (_autoInventoryHelpPlus =
                           webDriver.FindElement(By.Id("helpautoinventoryplus")));
            }
        }

        /// <summary>
        /// Collapse or the '-' button for 'Help - Auto Inventory' section
        /// </summary>
        private IWebElement _autoInventoryHelpMinus;
        public IWebElement AutoInventoryHelpMinus
        {
            get
            {
                return _autoInventoryHelpMinus ??
                       (_autoInventoryHelpMinus =
                           webDriver.FindElement(By.Id("helpautoinventoryminus")));
            }
        }

        /// <summary>
        /// 'Help - Auto Inventory' link
        /// </summary>
        private IWebElement _autoInventoryHelpLink;
        public IWebElement AutoInventoryHelpLink
        {
            get
            {
                return _autoInventoryHelpLink ??
                       (_autoInventoryHelpLink =
                           webDriver.FindElement(By.LinkText("Help - Auto Inventory"), new TimeSpan(0, 0, 30)));
            }
        }

        /// <summary>
        /// 'Help - Auto Inventory' content
        /// </summary>
        private IWebElement _autoInventoryHelp;
        public IWebElement AutoInventoryHelp
        {
            get
            {
                return _autoInventoryHelp ??
                       (_autoInventoryHelp =
                           webDriver.FindElement(By.Id("helpautoinventory")));
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets the text for a textbox
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IWebElement SetTextBoxValue(IWebElement element, string value)
        {
            element.WaitForElementDisplayed(TimeSpan.FromSeconds(30));
            if (!element.Displayed) return element;
            javaScriptExecutor.ExecuteScript("arguments[0].value=arguments[1]", element, String.Empty);
            javaScriptExecutor.ExecuteScript("arguments[0].value=arguments[1]", element, value);
            return element;
        }

        /// <summary>
        /// Unchecks all the config & option checkboxes
        /// </summary>
        public void UncheckAllConfigTypes()
        {
            foreach (var configElement in ConfigElements.Where(configElement => !configElement.Selected))
            {
                configElement.Click();
            }
        }

        /// <summary>
        /// Validates the presence of the required fields in 'Inventory Feed - Processing Rules' section
        /// </summary>
        /// <param name="clickToRunOnceButtonLabelText"></param>
        /// <param name="clickToRunOnceButtonText"></param>
        /// <param name="enableAutoInventoryLabelText"></param>
        /// <param name="autoInventoryRefreshIntervalLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyInventoryFeedSectionFields(string clickToRunOnceButtonLabelText,
            string clickToRunOnceButtonText, string enableAutoInventoryLabelText,
            string autoInventoryRefreshIntervalLabelText)
        {
            return VerifyClickToRunOnceButton(clickToRunOnceButtonLabelText, clickToRunOnceButtonText) &&
                   (VerifyEnableAutoInventoryCheckbox(enableAutoInventoryLabelText) &&
                    VerifyAutoInventoryRefreshIntervalDropdowns(autoInventoryRefreshIntervalLabelText));
        }


        /// <summary>
        /// Verifies if the 'Automated Inventory Feed Failure Notification Email' textbox is present
        /// </summary>
        /// <param name="failureNotificationEmailLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyPresenceOfEmailField(string failureNotificationEmailLabelText)
        {
            return
                InventoryFeedSectionTable.ElementExists(
                    By.XPath("tbody/tr[4]/td[2]/input[@id='ContentPageHolder_txt_AutomatedATSNotification']")) ||
                InventoryFeedSectionTable.Text.Contains(failureNotificationEmailLabelText);
        }

        /// <summary>
        /// Verify if all the dropdowns of Refresh Interval are disabled
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyRefreshIntervalDropdownsAreDisabled()
        {
            return !AutoInventoryDaysDropdown.Enabled && !AutoInventoryHoursDropdown.Enabled &&
                   !AutoInventoryMinutesDropdown.Enabled;
        }

        /// <summary>
        /// Verify if all the dropdowns of Refresh Interval are enabled
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyRefreshIntervalDropdownsAreEnabled()
        {
            return AutoInventoryDaysDropdown.Enabled && AutoInventoryHoursDropdown.Enabled &&
                   AutoInventoryMinutesDropdown.Enabled;
        }

        private bool VerifyClickToRunOnceButton(string clickToRunOnceButtonLabelText, string clickToRunOnceButtonText)
        {
            var firstRow = InventoryFeedSectionTable.FindElement(By.XPath("tbody/tr[1]"));

            if (
                !firstRow.FindElement(By.XPath("td[1]/span[@id='ContentPageHolder_lblManualATSEnabled']"))
                    .Text.Equals(clickToRunOnceButtonLabelText))
            {
                return false;
            }

            return firstRow.FindElement(By.XPath("td[2]/input[@id='ContentPageHolder_btnClicktoRunOnce']"))
                .GetAttribute("value")
                .Equals(clickToRunOnceButtonText);
        }

        private bool VerifyEnableAutoInventoryCheckbox(string enableAutoInventoryLabelText)
        {
            var secondRow = InventoryFeedSectionTable.FindElement(By.XPath("tbody/tr[2]"));

            if (
                !secondRow.FindElement(By.XPath("td[1]/span[@id='ContentPageHolder_lblAutomatedATSEnabled']"))
                    .Text.Equals(enableAutoInventoryLabelText))
            {
                return false;
            }

            return secondRow.FindElement(By.XPath("td[2]/input[@id='ContentPageHolder_chk_BC_AutomatedATSCheck']"))
                .GetAttribute("type")
                .Equals("checkbox");
        }

        private bool VerifyAutoInventoryRefreshIntervalDropdowns(string autoInventoryRefreshIntervalLabelText)
        {
            var thirdRow = InventoryFeedSectionTable.FindElement(By.XPath("tbody/tr[3]"));

            if (
                !thirdRow.FindElement(By.XPath("td[1]/span[@id='ContentPageHolder_lblAutomatedATSInterval']"))
                    .Text.Equals(autoInventoryRefreshIntervalLabelText))
            {
                return false;
            }

            var intervalDropdowns = thirdRow.FindElements(By.XPath("td[2]/select"));

            if (intervalDropdowns.Count() != 3)
            {
                return false;
            }

            var daysOptions = Enumerable.Range(0, 7);

            if (intervalDropdowns[0].Select().Options.Count() != daysOptions.Count() ||
                intervalDropdowns[0].Select().Options.Any(o => !daysOptions.Contains(Convert.ToInt32(o.Text))))
            {
                return false;
            }

            var hoursOptions = new[] { "0", "1", "2", "3", "4", "6", "8", "12" };

            if (intervalDropdowns[1].Select().Options.Count() != hoursOptions.Count() ||
                intervalDropdowns[1].Select().Options.Any(o => !hoursOptions.Contains(o.Text)))
            {
                return false;
            }

            var minutesOptions = new[] { "0", "30" };

            if (intervalDropdowns[2].Select().Options.Count() != minutesOptions.Count() ||
                intervalDropdowns[2].Select().Options.Any(o => !minutesOptions.Contains(o.Text)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies the 'Minimum Delay Post AutoCatalog' label & dropdown
        /// </summary>
        /// <param name="autoInventoryMinimumDelayPostAutoCatalogText"></param>
        /// <returns></returns>
        public bool VerifyAutoInventoryMinimumDelayPostAutoCatalog(string autoInventoryMinimumDelayPostAutoCatalogText)
        {
            var fourthRow = InventoryFeedSectionTable.FindElement(By.XPath("tbody/tr[4]"));

            if (
                !fourthRow.FindElement(By.XPath("td[1]/span[@id='ContentPageHolder_lblMinimumDelayPostAutoCatalog']"))
                    .Text.Equals(autoInventoryMinimumDelayPostAutoCatalogText))
            {
                return false;
            }

            var offsetHoursDropdown = fourthRow.FindElement(By.XPath("td[2]/select"));

            var offsetOptions = Enumerable.Range(1, 3);

            if (offsetHoursDropdown.Select().Options.Count() != offsetOptions.Count() ||
                offsetHoursDropdown.Select().Options.Any(o => !offsetOptions.Contains(Convert.ToInt32(o.Text))))
            {
                return false;
            }

            if (!EnableAutoInventoryCheckbox.Selected == AutoInventoryOffset.Enabled)
            {
                return false;
            }

            EnableAutoInventoryCheckbox.Click();

            if (!EnableAutoInventoryCheckbox.Selected == AutoInventoryOffset.Enabled)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Click on Click to Run once button
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool ClickToRunOnce(string statusMessage, out DateTime utcTime)
        {
            Console.WriteLine("Clicking on ClickToRunOnce Button..");
            ClickToRunOnceButton.Click();
            utcTime = DateTime.UtcNow;
            PageUtility.WaitForPageRefresh(webDriver);
            Console.WriteLine("Done!");
            Console.WriteLine("Inventory Feed Request Status : {0}", ConfirmationLabel.Text);
            return string.Equals(ConfirmationLabel.Text.Trim(), statusMessage);
        }

        /// <summary>
        /// to get all the identites for the profile name provided
        /// </summary>
        /// <returns>List of enabled Identity Names</returns>
        public List<string> GetIdentities()
        {
            //Expand Auto BHC Section
            AutomatedBhcCatalogProcessingRules.Click();
            // Now coding for only enabled Identites. Which are picked from Auto BHC catalog panel.
            return CheckedIdentityList.Select(e => e.Text).ToList();
        }

        /// <summary>
        /// Verifies if the 'Number of Automated Inventory Feeds' textbox is present
        /// </summary>
        /// <param name="noOfOccurrenceLabelText"></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerifyPresenceOfNumberOfOccurrenceField(string noOfOccurrenceLabelText)
        {
            return InventoryFeedSectionTable.ElementExists(By.Id("ContentPageHolder_txt_NumberOfInvFeedPreview")) ||
                   InventoryFeedSectionTable.Text.Contains(noOfOccurrenceLabelText);
        }

        /// <summary>
        /// Verify the 'Help - Auto Inventory' is collapsed while the page loads
        /// and verify the text while expanded
        /// </summary>
        /// <param name="autoInventoryHelpText"></param>
        /// <returns></returns>
        public bool VerifyAutoInventoryHelpSection(string autoInventoryHelpText)
        {
            if (!AutoInventoryHelpPlus.IsElementVisible() || AutoInventoryHelpMinus.IsElementVisible() ||
                AutoInventoryHelp.IsElementVisible())
            {
                return false;
            }

            AutoInventoryHelpLink.Click();

            if (AutoInventoryHelpPlus.IsElementVisible() || !AutoInventoryHelpMinus.IsElementVisible() ||
                !AutoInventoryHelp.IsElementVisible())
            {
                return false;
            }

            return RemoveSpaces(AutoInventoryHelp.Text).Equals(RemoveSpaces(autoInventoryHelpText));
        }

        public bool VerifyRestrictionOfInventoryIntervalToOneType()
        {
            // If 'Enable Automated Inventory Feed Checkbox' is checked, uncheck it to make sure the interval is defaulted
            if (EnableAutoInventoryCheckbox.Selected)
            {
                EnableAutoInventoryCheckbox.Click();
            }

            EnableAutoInventoryCheckbox.Click();

            //Select a value in Days dropdown
            AutoInventoryDaysDropdown.Select().SelectByText("3");

            if (!AutoInventoryHoursDropdown.Select().SelectedOption.Text.Equals("0") ||
                !AutoInventoryMinutesDropdown.Select().SelectedOption.Text.Equals("0"))
            {
                return false;
            }

            //Select a value in Hours dropdown
            AutoInventoryHoursDropdown.Select().SelectByText("4");

            if (!AutoInventoryDaysDropdown.Select().SelectedOption.Text.Equals("0") ||
                !AutoInventoryMinutesDropdown.Select().SelectedOption.Text.Equals("0"))
            {
                return false;
            }

            //Select '30 minutes' in Minutes dropdown
            AutoInventoryMinutesDropdown.Select().SelectByText("30");

            if (!AutoInventoryHoursDropdown.Select().SelectedOption.Text.Equals("0") ||
                !AutoInventoryDaysDropdown.Select().SelectedOption.Text.Equals("0"))
            {
                return false;
            }

            return true;
        }

        private static string RemoveSpaces(string value)
        {
            return value.Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty)
                .Replace(" ", string.Empty);
        }

        #endregion

    }
}
