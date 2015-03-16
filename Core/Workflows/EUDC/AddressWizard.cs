using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;


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

        public bool BillToAddSearchOption(string accountId, string localChannelNumber)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.SearchByLocalChannelNumber(localChannelNumber);
            return AddressWizardPage.FindLocalChannel(localChannelNumber);
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

        public string CheckOmsAdd(string accountId, string localChannelNumber)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.SearchByLocalChannelNumber(localChannelNumber);
            string p = AddressWizardPage.OmsAdd(localChannelNumber);
            return p;
        }

        /// <summary>
        /// method to select Local Channel # from Drop Down and passing the value in search field 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="localChannelValue"></param>
        /// <returns>false,if local channel # is not displayed</returns>
        public bool CheckLocalChannelNumber(string accountId, string localChannelValue)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.SearchByLocalChannelNumber(localChannelValue);
            return AddressWizardPage.ChannelNumberColumnTextExist();
        }

        public bool SearchByAlphaNumericLocalChannelNumber(string accountId, string localChannelNumber)
        {
            HomePage.GoToCatalogAndPricingPage(accountId);
            CatalogAndPricingPage.GoToAdressWizardPage();
            webDriver.SwitchTo().Frame(0);
            AddressWizardPage.SelectLocalChannelOption();
            AddressWizardPage.SearchByLocalChannelNumber(localChannelNumber);
            return AddressWizardPage.CheckIfResultsTableIsAvailable();
        }
    }
}
