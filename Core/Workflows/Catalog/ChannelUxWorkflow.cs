using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using FluentAssertions;
using Microsoft.BusinessData.MetadataModel;
using Modules.Channel.B2B.Core.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Modules.Channel.B2B.Core.Workflows.Catalog
{
    public class ChannelUxWorkflow
    {
        private IWebDriver webDriver;
        private B2BChannelUx B2BChannelUx;
        private IJavaScriptExecutor javaScriptExecutor;
        private B2BAutoCatalogListPage b2BAutoCatalogListPage;

        /// <summary>
        /// Constructor for ChannelUxWorkflow
        /// </summary>
        /// <param name="webDriver"></param>
        public ChannelUxWorkflow(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor) this.webDriver;

        }

        /// <summary>
        /// Verifies links on Channel UX page.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="profileName"></param>
        public bool Verifyandretrievelinks(string environment, string linksTextTestData)
        {
            string[] LinkTestStringValue = linksTextTestData.Split(',');
            B2BChannelUx = new B2BChannelUx(webDriver);
            B2BChannelUx.SelectEnvironment(environment);
            for (int j = 1; j <= LinkTestStringValue.Length; j++)
                {
                    string TestData = LinkTestStringValue[j-1];
                    var Link_Locator = webDriver.FindElement(By.XPath("//table/tbody/tr[" + j + "]/td/a/h4"));
                    string LinkLocatorText = Link_Locator.Text;
                    if (!LinkLocatorText.Contains(TestData))
                    {
                        return false;
                    }
                    else
                    {
                        Link_Locator.Click();
                        webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
                        if (webDriver.PageSource.Contains("Auto Catalog List") ||
                            webDriver.PageSource.Contains("Packaging Data") ||
                            webDriver.PageSource.Contains("Retrieve/Create Catalog") ||
                            webDriver.PageSource.Contains("Auto Catalog Part Viewer")
                            )
                        {
                            B2BChannelUx.Rightside_Menu.Click();
                            B2BChannelUx.Rightside_dropdown_Home.Click();
                        }
                        else
                        {
                            return false;
                         }
                      }
                }
            
            return true;
            }
    }
}
