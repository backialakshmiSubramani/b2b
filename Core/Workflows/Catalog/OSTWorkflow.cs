using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Common;

namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    public class OSTWorkflow
    {
        private IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor for ChannelUxWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public OSTWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
        }

        public void UpdateAccessGroup(string accountName, string accessGroupName, string orderCode, DeltaChange deltaChange)
        {
            OstHomePage ostHomePage = new OstHomePage(webDriver);
            ostHomePage.OpenOSTHomePage();
            ostHomePage.GoToCatalogAndPricingPage(accountName);
            ostHomePage.ManageAccessGroupsLink.SendKeys(Keys.Enter);
            OSTManageAccessGroupPage ostManageAccessGroupPage = new OSTManageAccessGroupPage(webDriver);
            ostManageAccessGroupPage.UpdateProduct(accessGroupName, orderCode, deltaChange);
        }

        public void ResetAccessGroup(string accountName, string accessGroupName, string orderCode, DeltaChange deltaChange)
        {
            OstHomePage ostHomePage = new OstHomePage(webDriver);
            ostHomePage.OpenOSTHomePage();
            ostHomePage.GoToCatalogAndPricingPage(accountName);
            ostHomePage.ManageAccessGroupsLink.SendKeys(Keys.Enter);
            OSTManageAccessGroupPage ostManageAccessGroupPage = new OSTManageAccessGroupPage(webDriver);
            ostManageAccessGroupPage.ResetProduct(accessGroupName, orderCode, deltaChange);
        }
    }
}
