// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/1/2014 6:19:52 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/1/2014 6:19:52 PM
// ***********************************************************************
// <copyright file="B2BCustomerProfileListPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
using Modules.Channel.EUDC.Core.Pages;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCustomerProfileListPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCustomerProfileListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            //Name = "";
            //Url = "";
            //ProductUnit = "";
            Thread.Sleep(5000);
        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return SearchTextField.IsElementVisible();
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return (webDriver).ToString().Contains("(null)") ? false : true;
        }

        #region Elements

        public IWebElement CreateNewProfileLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'Create New Profile')]"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Create New Profile')]"));
            }
        }

        private IWebElement SearchLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Search"));
            }

        }
        private IWebElement AdvancedSearchLink
        {
            get
            {
                webDriver.WaitForElement(By.LinkText("Advance Search"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.LinkText("Advance Search"));
            }

        }

        private IWebElement SearchTextField
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txtBoxSearchText"));
            }
        }

        private SelectElement SearchCriteriaList
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_ddlSearchType"), TimeSpan.FromSeconds(30));
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_ddlSearchType")));

            }
        }

        private IWebElement PaginationArrow
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[@class='command'][contains(text(),'›')]"));
            }
        }

        private bool IsPaginationDisplayed
        {
            get
            {
                return webDriver.ElementExists(AdeptBy.XPath("//a[@class='command'][contains(text(),'›')]"));
            }
        }

        private IWebElement ChannelAsnCheckbox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_chkIsChannelASNEnabled"));
            }
        }

        private IWebElement ValueAfterSearch
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_grdCustomerProfilelists_hypCustomerName_0"));
            }
        }

        private IWebElement AdvanceSearchBtn
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[contains(@id,'lnkAdvanceSearch')]"));
            }
        }

        private IWebElement SearchedProfile
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(@id,'hypCustomerName')]"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.XPath("//a[contains(@id,'hypCustomerName')]"));
            }
        }

        #endregion

        #region Element Actions
        public void ClickCreateNewProfile()
        {
            ////CreateNewProfileLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", CreateNewProfileLink);
        }

        public void ClickAdvancedSearch()
        {
            ////AdvancedSearchLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AdvancedSearchLink);
        }

        public void EnableChannelASN()
        {
            if (ChannelAsnCheckbox.GetAttribute("checked") != "checked")
            {
                ////ChannelAsnCheckbox.Click();
                javaScriptExecutor.ExecuteScript("arguments[0].click();", ChannelAsnCheckbox);
            }
        }

        public void ClickSearchLink()
        {
            ////SearchLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchLink);
        }

        public void ClickSearchedProfile()
        {
            ////SearchedProfile.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchedProfile);
        }

        public void ClickValueAfterSearch()
        {
            ////ValueAfterSearch.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ValueAfterSearch);
            webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_ProfileHeader_hyp_PH_Authentication"), TimeSpan.FromSeconds(30));
        }

        public void ClickSearchedProfile(string linkText)
        {
           webDriver.WaitForElementDisplayed(By.LinkText(linkText), TimeSpan.FromSeconds(30));
            //webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_grdCustomerProfilelists_hypCustomerName_0"), TimeSpan.FromSeconds(30));
            
          //  webDriver.FindElement(By.LinkText(linkText)).Click();
//            Console.WriteLine(this.Validate());
            var elem = webDriver.FindElement(By.LinkText(linkText));
           // var elem = webDriver.FindElement(By.Id("ContentPageHolder_grdCustomerProfilelists_hypCustomerName_0"));
          //  Console.WriteLine("Element is visible" + elem.IsElementVisible());
            javaScriptExecutor.ExecuteScript("arguments[0].click();", elem);
            Thread.Sleep(1000);

           webDriver.WaitForPageLoad(new TimeSpan(0, 0, 60));
        }

        public B2BBuyerCatalogPage ClickB2BsearchedProfile()
        {
            webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_grdCustomerProfilelists_hypCustomerName_0"), TimeSpan.FromSeconds(30));
            var elem = webDriver.FindElement(By.Id("ContentPageHolder_grdCustomerProfilelists_hypCustomerName_0"));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", elem);
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 60));
            
            return new B2BBuyerCatalogPage(this.webDriver);

        }


        #endregion

        #region ReUsable Methods
        public void SearchProfile(string SearchCriteria, string ProfileName)
        {
            if (SearchCriteria != null)
            {
                if (!SearchCriteriaList.SelectedOption.Text.Equals(SearchCriteria))
                {
                    SearchCriteriaList.SelectByText(SearchCriteria);
                }
            }

            SearchTextField.Set(ProfileName);
            ////SearchLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SearchLink);
           // Thread.Sleep(5000);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }

        public void AdvanceSearchAsnEnabledProfile()
        {
            ////AdvancedSearchLink.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AdvancedSearchLink);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
            ////ChannelAsnCheckbox.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ChannelAsnCheckbox);
            ////AdvanceSearchBtn.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AdvanceSearchBtn);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
        }

        public bool VerifyProfileSearchResult(string ProfileName, string ClickProfile)
        {
            bool Flag = true;
            bool status = false;
            string locator = "//a[contains(text(),'" + ProfileName + "')]";  // Searched profile name is dynamic

            do
            {
                if (webDriver.ElementExists(By.XPath(locator)))
                {
                    status = webDriver.ElementExists(By.XPath(locator));
                    if (ClickProfile == "Yes")
                    {
                        ////webDriver.FindElement(By.XPath(locator)).Click();
                        javaScriptExecutor.ExecuteScript(
                            "arguments[0].click();",
                            webDriver.FindElement(By.XPath(locator)));
                    }

                    Flag = false;
                }
                else
                {
                    if (IsPaginationDisplayed)
                    {
                        ////PaginationArrow.Click();
                        javaScriptExecutor.ExecuteScript("arguments[0].click();", PaginationArrow);
                        webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
                    }
                    else
                    {
                        Flag = false;
                        status = false;
                    }
                }

            } while (Flag == true);

            return status;
        }

        #endregion


    }
}
