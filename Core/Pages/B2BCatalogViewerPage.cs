// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/12/2014 11:46:04 AM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/12/2014 11:46:04 AM
// ***********************************************************************
// <copyright file="B2BCatalogViewer.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using System.Linq;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Workflows.Common;

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCatalogViewerPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCatalogViewerPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return CatalogDetailsTableRows.Any();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("b2bcatalogviewer.aspx");
        }

        #region Elements

        private ReadOnlyCollection<IWebElement> CatalogDetailsTableRows
        {
            get
            {
                return webDriver.FindElements(
                                            By.XPath(
                                                "//table[@id='G_ContentPageHolderxuwGrdCatlogDetailsxuwGrdCatlogDetails']/tbody/tr"));
            }
        }

        private IWebElement QaTools3
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[normalize-space(.)='QA Tools 3.0']"));
            }
        }

        private IWebElement DellImageLink
        {
            get
            {
                return webDriver.FindElement(By.Id("HyplnkDellLogo"));
            }
        }

        #endregion

        /// <summary>
        /// Fetches the quote details used for PO processing
        /// </summary>
        /// <param name="crtId"></param>
        /// <param name="quantity"></param>
        /// <param name="quoteType"></param>
        /// <returns>Quote details</returns>
        public List<QuoteDetail> GetQuoteDetails(string crtId, string quantity, QuoteType quoteType, List<string> configTitles = null)
        {
            var listOfQuoteDetail = new List<QuoteDetail>();

            if (configTitles == null)
            {
                var firstRow = this.CatalogDetailsTableRows.FirstOrDefault();
                if (firstRow != null)
                {
                    listOfQuoteDetail.Add(
                        new QuoteDetail()
                            {
                                CrtId = crtId,
                                ItemDescription =
                                    firstRow.FindElements(By.TagName("td"))[8].Text.Trim().Split(',')[0],
                                Price =
                                    firstRow.FindElements(By.TagName("td"))[9].Text.Trim().Split(' ')[0],
                                Quantity = quantity,
                                QuoteType = quoteType,
                                SupplierPartId = "BHC:" + firstRow.FindElements(By.TagName("td"))[1].Text.Trim()
                            });
                }
                else
                {
                    Console.WriteLine("No items found in Catalog Viewer page");
                }
            }
            else
            {
                foreach (var configTitle in configTitles)
                {
                    var configRow = CatalogDetailsTableRows.FirstOrDefault(e => e.FindElements(By.TagName("td"))[7].Text.Contains(configTitle));

                    if (configRow != null)
                    {
                        listOfQuoteDetail.Add(
                            new QuoteDetail()
                                {
                                    CrtId = crtId,
                                    ItemDescription =
                                        configRow.FindElements(By.TagName("td"))[8].Text.Trim().Split(',')[0],
                                    Price =
                                        configRow.FindElements(By.TagName("td"))[9].Text.Trim().Split(' ')[0],
                                    Quantity = quantity,
                                    QuoteType = quoteType,
                                    SupplierPartId =
                                        "BHC:" + configRow.FindElements(By.TagName("td"))[1].Text.Trim()
                                });
                    }
                    else
                    {
                        Console.WriteLine("Catalog Item with title **{0}** not found in Catalog Viewer page", configTitle);
                    }
                }
            }
            return listOfQuoteDetail;
        }

        public void ClickQaTools3()
        {
            ////QaTools3.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", QaTools3);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
            Console.WriteLine("Url after switching is: {0}", webDriver.Url);
            webDriver.Manage().Window.Maximize();
        }

        public void GoToHomePage()
        {
            javaScriptExecutor.ExecuteScript("arguments[0].click();", DellImageLink);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
        }
    }
}
