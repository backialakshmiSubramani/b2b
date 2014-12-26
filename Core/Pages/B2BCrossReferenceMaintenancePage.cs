// ***********************************************************************
// Author           : AMERICAS\Ygnashwaran_Sekar
// Created          : 12/11/2014 6:59:23 PM
//
// Last Modified By : AMERICAS\Ygnashwaran_Sekar
// Last Modified On : 12/11/2014 6:59:23 PM
// ***********************************************************************
// <copyright file="B2BCrossReferenceMaintenancePage.cs" company="Dell">
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
    public class B2BCrossReferenceMaintenancePage : DCSGPageBase
    {
        IWebDriver webDriver;

        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceMaintenancePage(IWebDriver webDriver)
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

        #region Elements

        private SelectElement CrossReferenceTypeList
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.XPath("//select[contains(@id,'CRTType')]"), TimeSpan.FromSeconds(60));
                return new SelectElement(webDriver.FindElement(By.XPath("//select[contains(@id,'CRTType')]")));
            }
        }

        private IWebElement DescriptionTxtBox
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_txtLookupDesc"));
            }
        }

        private IWebElement CrossReferneceTypeLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lblCRType"));
            }
        }
        private IWebElement DescriptionLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label3"));
            }
        }
        private IWebElement FileToUploadLabel
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label4"));
            }
        }

        private IWebElement FileToUpload
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ContentPageHolder_fileUpEx"), TimeSpan.FromSeconds(60));
                return webDriver.FindElement(By.Id("ContentPageHolder_fileUpEx"));
            }
        }

        private IWebElement SaveButton
        {
            get
            {
                return webDriver.FindElement(By.XPath("//a[contains(text(),'Save')]"));
            }
        }

        private IWebElement CrtUploadSuccessMsg
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'Cross Reference details saved successfully')]"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'Cross Reference details saved successfully')]"));
            }
        }

        private IWebElement CrtUploadErrorMsg
        {
            get
            {
                webDriver.WaitForElement(By.XPath("//span[contains(text(),'*Select valid .xls file for upload')]"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.XPath("//span[contains(text(),'*Select valid .xls file for upload')]"));
            }
        }

        #endregion

        #region Element Actions

        public void SelectCrossReferenceType(string crType)
        {
            CrossReferenceTypeList.SelectByText(crType);
        }

        public void SetDescription(string description)
        {
            DescriptionTxtBox.SendKeys(description);
        }

        public void SetUploadFilePath(string filePath)
        {
            Console.WriteLine("File path {0}", System.IO.Directory.GetCurrentDirectory());
            FileToUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + filePath);
        }

        public void ClickSave()
        {
            ////SaveButton.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", SaveButton);
        }

        public bool IsSuccessfulMessageDisplayed()
        {
            if (CrtUploadSuccessMsg.Displayed)
            {
                return true;
            }

            return false;
        }

        public bool IsErrorMessageDisplayed()
        {
            if (CrtUploadErrorMsg.Displayed)
            {
                return true;
            }

            return false;
        }

        public string crosstyperefText()
        {
            return CrossReferneceTypeLabel.Text;
        }

        public string FileUploadText()
        {
            return FileToUploadLabel.Text;
        }

        public string DescriptionText()
        {
            return DescriptionLabel.Text;
        }

        #endregion
    }
}
