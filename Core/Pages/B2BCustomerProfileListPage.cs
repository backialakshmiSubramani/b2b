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
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BCustomerProfileListPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCustomerProfileListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
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
            throw new NotImplementedException();
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

        private IWebElement CreateNewProfileLink
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//a[contains(text(),'Create New Profile')]"), TimeSpan.FromSeconds(30));
                return  webDriver.FindElement(By.XPath("//a[contains(text(),'Create New Profile')]"));
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

        private IWebElement SearchCriteriaList
        {
            get
            {
                 return webDriver.FindElement(By.Id("ContentPageHolder_ddlSearchType"));

            }
        }

        private IWebElement PaginationArrow
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[contains(text(),'�')]"));
            }
        }
            
                      
            
        private IWebElement ChannelASNChkBox
        {
            get
            {
             return webDriver.FindElement(By.XPath("//input[@id='ContentPageHolder_chkIsChannelASNEnabled']"));
            }
        }

        
       

        #endregion

        #region Element Actions
        public void ClickCreateNewProfile()
        {
            CreateNewProfileLink.Click();
        }

        public void ClickAdvancedSearch()
        {
            AdvancedSearchLink.Click();
        }

        public void EnableChannelASN()
        {
            if (ChannelASNChkBox.GetAttribute("checked") != "checked")
            {
                ChannelASNChkBox.Click();
            }       
        }

        public void ClickSearchLink()
        {
            SearchLink.Click();
        }

        public void ClickSearchedProfile()
        {
            
            webDriver.WaitForElement(By.XPath("//a[contains(@id,'hypCustomerName')]"), TimeSpan.FromSeconds(60));
            webDriver.FindElement(By.XPath("//a[contains(@id,'hypCustomerName')]")).Click();

        }
      
        #endregion

        #region ReUsable Methods
        public void SearchProfile(string SearchCriteria , string ProfileName)
        {
           
            if (SearchCriteria != null)
            {
                SelectElement criteria = new SelectElement(SearchCriteriaList);
                criteria.SelectByText(SearchCriteria);
            }

            SearchTextField.Set(ProfileName);
            SearchLink.Click();
        }

        public void AdvanceSearchAsnEnabledProfile()
        {
            AdvancedSearchLink.Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
            ChannelASNChkBox.Click();
            webDriver.FindElement(By.XPath("//a[contains(@id,'lnkAdvanceSearch')]")).Click();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }



        public bool VerifyProfileSearchResult(string ProfileName, string ClickProfile)
        {
            bool Flag = true;
            bool status = false;
            string locator = "//a[contains(text(),'" + ProfileName + "')]";

            do
            {
                if (webDriver.ElementExists(By.XPath(locator)))
                {
                    status = webDriver.ElementExists(By.XPath(locator));
                    if (ClickProfile == "Yes")
                    {
                        webDriver.FindElement(By.XPath(locator)).Click();
                    }
                    
                    Flag = false;                   
                }
                else
                {
                    if (webDriver.ElementExists(By.XPath("//a[contains(text(),'�')]")))
                    {
                        webDriver.FindElement(By.XPath("//a[contains(text(),'�')]")).Click();
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