using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;

namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    using System.Data.Linq.Mapping;
    using System.Security.Policy;

    using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;

    using Microsoft.SharePoint.Client;

    public class FeatureSetup
    {
        private IWebDriver webDriver;

        public FeatureSetup(IWebDriver driver)
        {
            this.webDriver = driver;
        }

        private OstHomePage OstHomePage
        {
            get
            {
                return new OstHomePage(webDriver);
            }
        }

        private OstCatalogAndPricingPage OstCatalogAndPricingPage
        {
            get
            {
                return new OstCatalogAndPricingPage(webDriver);
            }
        }

        private OstFeatureSetupPage OstFeatureSetupPage
        {
            get
            {
                return new OstFeatureSetupPage(webDriver);
            }
        }

        private B2BPremierDashBoardPage B2BPremierDashBoardPage
        {
            get
            {
                return new B2BPremierDashBoardPage(webDriver);
            }
        }

        public bool MyCustomerTabInPremierDashboardPagePreview(string accountId, bool withUpdate = false)
        {
            OstHomePage.GoToCatalogAndPricingPage(accountId);
            OstCatalogAndPricingPage.GoToFeatureSetupPage();
            OstFeatureSetupPage.UnCheckPartnerCustomerTabCheckboxIfAlreadyChecked();
            OstFeatureSetupPage.CheckPartnerCustomerTabCheckbox();
            if (withUpdate)
            {
                if (!OstFeatureSetupPage.ClickUpdate())
                {
                    return false;
                }
            }

            OstFeatureSetupPage.PreviewPage();
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 30));
            var parentWindow = webDriver.CurrentWindowHandle;
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
            Console.WriteLine("Url after switching is: {0}", webDriver.Url);
            return B2BPremierDashBoardPage.CheckIfMyCustomersLinkIsAvailable();
        }
    }
}
