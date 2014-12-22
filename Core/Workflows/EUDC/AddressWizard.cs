﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Page;

using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    public class AddressWizard
    {
        private IWebDriver webDriver;

        private OstHomePage HomePage
        {
            get
            {
                return new OstHomePage(webDriver);
            }
        }

        private OstCatalogAndPricingPage CatalogAndPricingPage
        {
            get
            {
                return new OstCatalogAndPricingPage(webDriver);
            }
        }

        private OstAddressWizardPage AddressWizardPage
        {
            get { return new OstAddressWizardPage(webDriver); }
        }

        private OstShipToAddressPage ShipToAddressPage
        {
            get
            {
                return new OstShipToAddressPage(webDriver);
            }
        }

        public AddressWizard(IWebDriver Driver)
        {
            webDriver = Driver;
        }



        public bool BillToAddSearchBy(string accountId)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            return AddressWizardPage.CheckLocalChannelNumber();
        }

        public bool BillToAddSearchOption(string accountId, string localchannelvalue)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.LocalChannelNumberValue(localchannelvalue);
            return AddressWizardPage.FindLocalChannel(localchannelvalue);
        }

        public bool CustomerAndLocalChnNumDisplay(string accountId)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            return (AddressWizardPage.CustomerNumberColumnText().Contains("Customer #") && AddressWizardPage.ChannelNumberColumnText().Contains("Local Channel #"));
        }

        public bool ShipToAddressCheck(string accountId)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.ShipToAddress();
            return ShipToAddressPage.SelectShipToAddOptions();
        }

        public string BillToAddressRefreshHyperlink(string accountId)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.BillToAddHyperLinkClick();
            return AddressWizardPage.BillToAddUpdateTextCValidate();
        }

        public string CheckOmsAdd(string accountId, string localchannelvalue)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.LocalChannelNumberValue(localchannelvalue);
            string p = AddressWizardPage.OmsAdd(localchannelvalue);
            return p;
        }

        /// <summary>
        /// method to select Local Channel # from Drop Down and passing the value in search field 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="localchannelvalue"></param>
        /// <returns>false,if local channel # is not displayed</returns>
        public bool CheckLocalChannelNumber(string accountId, string localchannelvalue)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.LocalChannelNumberValue(localchannelvalue);
            return AddressWizardPage.ChannelNumberColumnTextExist();

        }
    }
}
