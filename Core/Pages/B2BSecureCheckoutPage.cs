// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/10/2014 12:50:12 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/10/2014 12:50:12 PM
// ***********************************************************************
// <copyright file="SecureCheckoutPage.cs" company="Dell">
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
using OpenQA.Selenium.Interactions;


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BSecureCheckoutPage : DCSGPageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BSecureCheckoutPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)webDriver;
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
        private IWebElement ExportOption
        {
            get
            {
                return webDriver.FindElement(By.Id("exportNo"), new TimeSpan(0, 0, 10));
            }
        }

        private IWebElement ShippingContinueButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ShippingContinue"));
            }
        }

        private IWebElement FirstName
        {
            get
            {
                return webDriver.FindElement(By.Id("OrderContactInformation_Contact_FirstName"));
            }
        }

        private IWebElement LastName
        {
            get
            {
                return webDriver.FindElement(By.Id("OrderContactInformation_Contact_LastName"));
            }
        }

        private IWebElement CompanyName
        {
            get
            {
                return webDriver.FindElement(By.Id("OrderContactInformation_Contact_CompanyName"));
            }
        }

        private IWebElement Email
        {
            get
            {
                return webDriver.FindElement(By.Id("OrderContactInformation_Contact_Email"));
            }
        }

        private IWebElement PhoneNumber
        {
            get
            {
                return webDriver.FindElement(By.Id("OrderContactInformation_Contact_PhoneNumber"));
            }
        }

        private IWebElement AddressBookLink
        {
            get
            {
                return webDriver.FindElement(By.ClassName("address_book"));
            }
        }

        private IWebElement SelectBtn
        {
            get
            {
                // No unique Id is present hence Xpath is used.
                return webDriver.FindElement(By.XPath("//form[@id='addressBookForm']/table/tbody/tr[1]/td[5]/a"));
            }
        }

        private IWebElement PurchaseOrderSelectBtn
        {
            get
            {
                return webDriver.FindElement(By.Id("btnAddPayment_PURCHASEORDER"));
            }
        }

        private IWebElement EquoteContactContinueBtn
        {
            get
            {
                return webDriver.FindElement(By.Id("EQuoteContactContinue"));
            }
        }

        private IWebElement PaymentContinueBtn
        {
            get
            {
                return webDriver.FindElement(By.Id("PaymentContinue"));
            }
        }

        #endregion

        #region Element Actions

        public void ClickExportOption()
        {
            ////ExportOption.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ExportOption);
        }

        public void ClickContinueButton()
        {
            ////ShippingContinueButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ShippingContinueButton);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
        }

        public void EnterContactAndBillingInfo()
        {
            string firstName = DCSG.ADEPT.Framework.Data.Generator.RandomString(5, 0);
            string lastName = DCSG.ADEPT.Framework.Data.Generator.RandomString(5, 0);
            string companyName = DCSG.ADEPT.Framework.Data.Generator.RandomString(5, 0);
            string phoneNumber = DCSG.ADEPT.Framework.Data.Generator.RandomInt(0, 999999).ToString() + DCSG.ADEPT.Framework.Data.Generator.RandomInt(0, 999999).ToString();
            string email = DCSG.ADEPT.Framework.Data.Generator.RandomString(5, 0) + "@test.com";

            FirstName.SendKeys(firstName);
            LastName.SendKeys(lastName);
            CompanyName.SendKeys(companyName);
            Email.SendKeys(email);
            PhoneNumber.SendKeys("0");
            PhoneNumber.Clear();
            PhoneNumber.Set(phoneNumber);

            //select Billing address;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", AddressBookLink);
            System.Threading.Thread.Sleep(3000);
            webDriver.SwitchTo().Frame(webDriver.FindElement(By.Id("billing_address_wizard_modal_iframe")));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SelectBtn);
            webDriver.SwitchTo().DefaultContent();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", EquoteContactContinueBtn);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ExportOption);
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ShippingContinueButton);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PurchaseOrderSelectBtn);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", PaymentContinueBtn);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));

        }

        #endregion

    }
}
