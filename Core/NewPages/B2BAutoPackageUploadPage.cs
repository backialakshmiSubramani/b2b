// ***********************************************************************
// Author           : AMERICAS\Shaukat_Saleem
// Created          : 12/9/2016 11:13:53 AM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 12/9/2016 11:13:53 AM
// ***********************************************************************
// <copyright file="B2BAutoPackageUploadPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace Modules.Channel.B2B.Core.NewPages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BAutoPackageUploadPage : PageBase

    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BAutoPackageUploadPage(IWebDriver webDriver) : base(ref webDriver)
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

        /// <summary>
        /// Field for selecting the file to upload
        /// </summary>
        public IWebElement FileUpload
        {
            get { return webDriver.FindElement(By.Id("file")); }
        }

        /// <summary>
        /// Upload button
        /// </summary>
        public IWebElement UploadButton
        {
            get { return webDriver.FindElement(By.Id("btnUpload")); }
        }

        /// <summary>
        /// Message displayed on Upload
        /// </summary>
        public IWebElement UploadMessage
        {
            get
            {
                webDriver.WaitForElementVisible(By.Id("divMessage"), new TimeSpan(0, 0, 30));
                return webDriver.FindElement(By.Id("divMessage"));
            }
        }

        #endregion Elements

        #region Public methods

        public void OpenAutoPackageUploadPage(B2BEnvironment b2BEnvironment)
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("AutoPackageUploadUrl") + ((b2BEnvironment == B2BEnvironment.Production) ? "P" : "U"));
        }

        public void UploadExcelFile(string fileToUpload)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            FileUpload.SendKeys(System.IO.Directory.GetCurrentDirectory() + @"\" + fileToUpload);
            UploadButton.Submit();
        }

        /// <summary>
        /// Waits for the page to refresh after navigation
        /// </summary>
        public void WaitForPageRefresh()
        {
            var isloaded = string.Empty;
            do
            {
                Thread.Sleep(4000);
                try
                {
                    isloaded = javaScriptExecutor.ExecuteScript("return window.document.readyState") as string;
                }
                catch
                {
                    // ignored
                }
            } while (isloaded != "complete");
        }

        public string MessageText()
        {
            return UploadMessage.Text.Trim();
        }

        #endregion Public methods
    }
}
