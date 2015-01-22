
// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/8/2014 4:48:16 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/8/2014 4:48:16 PM
// ***********************************************************************
// <copyright file="StandardConfigurationPage.cs" company="Dell">
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
    public class B2BStandardConfigurationPage : DCSGPageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BStandardConfigurationPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
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
            throw new NotImplementedException();
        }

        #region Element
        private IWebElement FirstConfigurationCheckbox
        {
            get
            {
                // return webDriver.FindElement(By.XPath("//table[@class='uif_table gsd_bodyCopyMedium']/tbody/tr[1]/td[2]"));
                return webDriver.FindElement(By.Id("SelectedResults"));
            }
        }

        private IWebElement AddSelectedToCartButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[@id='btnAddSelectedToCart']/span"));
            }
        }

        private IWebElement FilterList
        {
            get
            {
                return webDriver.FindElement(By.Id("action-selection-list"));
            }
        }

        private IList<IWebElement> StandardConfigTableRows
        {
            get
            {
                return webDriver.FindElement(By.Id("dataTableGridForm")).FindElements(By.XPath("//tr/td[3]"));
            }
        }

        private IList<IWebElement> StandardConfigTableCheckBox
        {
            get
            {
                return webDriver.FindElement(By.Id("dataTableGridForm")).FindElements(By.Id("SelectedResults"));
            }
        }

        #endregion

        #region Element Actions

        public void SelectFirstConfiguration()
        {
            ////FirstConfigurationCheckbox.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", FirstConfigurationCheckbox);
        }

        public void SelectSpecificConfiguration(string filterValue, string ItemName)
        {

            bool status = false;

            SelectElement filter = new SelectElement(FilterList);
            filter.SelectByText(filterValue);
            System.Threading.Thread.Sleep(2000);
            for (int i = 0; i < StandardConfigTableRows.Count; i++)
            {
                if (StandardConfigTableRows[i].Text.Contains(ItemName))
                {
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", StandardConfigTableCheckBox[i]);
                    status = true;
                    break;
                }
            }

            if (status == true)
            {
                Console.WriteLine("Found Item of Name, {0}", ItemName);
            }
            else
            {
                Console.WriteLine("Not Found Item of Name, {0}", ItemName);
            }
        }

        public void ClickAddSelectedToCartButton()
        {
            ////AddSelectedToCartButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AddSelectedToCartButton);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(50));
        }

        #endregion
    }
}
