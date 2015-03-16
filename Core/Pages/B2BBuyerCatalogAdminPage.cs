// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/15/2014 2:42:13 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/15/2014 2:42:13 PM
// ***********************************************************************
// <copyright file="B2BBuyerCatalogAdminPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
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

namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BBuyerCatalogAdminPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BBuyerCatalogAdminPage(IWebDriver webDriver)
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
            return SalesRepresentativeList.IsElementVisible();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.ToLower().Contains("addprofilestocatalogmanager.aspx");
        }

        #region Elements

        private IWebElement SalesRepresentativeList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_cboSalesRepresentative"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.Id("ContentPageHolder_cboSalesRepresentative"));
            }
        }

        private IWebElement PaginationList
        {
            get
            {
                webDriver.WaitForElement(By.ClassName("ig_d1468a88_r10"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.ClassName("ig_d1468a88_r10"));
            }
        }

        private IWebElement SaveButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_imgbtnSave"));
            }
        }


        #endregion

        #region Reusable Methods

        public void AssociateProfileToSalesUser(string salesUser, string profileName)
        {
            //Selects Sales User
            SelectElement saleRepresentative = new SelectElement(SalesRepresentativeList);
            saleRepresentative.SelectByText(salesUser);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));

            //Gets the No. of pages available
            SelectElement pagination = new SelectElement(PaginationList);
            IList<IWebElement> pageOptions = pagination.Options;

            // Loop to verify whether Profile Name is available in any of the pages
            for (int i = 1; i <= pageOptions.Count; i++)
            {
                // System.Threading.Thread.Sleep(2000);
                // Due to Stale Element Exception - Page List Element is redefined again
                SelectElement pagesList = new SelectElement(webDriver.FindElement(By.ClassName("ig_d1468a88_r10"), TimeSpan.FromSeconds(30)));
                pagesList.SelectByText(i.ToString());
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));

                if (webDriver.ElementExists(By.XPath("//nobr[contains(text(),'" + profileName + "')]")))
                {
                    Console.WriteLine("Given Profile is found in Page - {0}", i);
                    IWebElement table = webDriver.FindElement(By.XPath("//table[contains(@id,'ProfileList')]"));
                    IList<IWebElement> rows = table.FindElements(By.TagName("tr"));

                    // Loop to Select Checkbox and save profile once profile name is identified in a page
                    for (int j = 3; j < rows.Count; j++)
                    {

                        if (rows[j].Text.Contains(profileName))
                        {
                            var currentElement =
                                webDriver.FindElement(
                                    By.XPath("//tr[" + (j - 2) + "][contains(@id,'ProfileList')]/td[1]/nobr/input"));
                            if (currentElement.GetAttribute("checked").Equals("true"))
                            {
                                ////currentElement.Click();
                                javaScriptExecutor.ExecuteScript("arguments[0].click();", currentElement);
                                ////SaveButton.Click();
                                javaScriptExecutor.ExecuteScript("arguments[0].click();", SaveButton);
                                webDriver.SwitchTo().Alert().Accept();
                                Console.WriteLine("Successfully selcted and saved profile to Sales User");
                            }
                            else
                            {
                                Console.WriteLine("Profile Already saved to Sales User");
                            }

                            break;
                        }
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("Given Profile is not found in Page - {0}", i);
                }
            }
        }

        #endregion

    }
}
