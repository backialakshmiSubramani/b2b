// ***********************************************************************
// Author           : AMERICAS\Vinay_Chand
// Created          : 12/9/2014 2:20:52 PM
//
// Last Modified By : AMERICAS\Vinay_Chand
// Last Modified On : 12/9/2014 2:20:52 PM
// ***********************************************************************
// <copyright file="B2BMaintenanceCrossReference.cs" company="Dell">
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


namespace Modules.Channel.B2B.Core.Pages
{
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BMaintenanceCrossReferencePage : DCSGPageBase
    {
        IWebDriver webDriver;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BMaintenanceCrossReferencePage(IWebDriver webDriver)
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
            throw new NotImplementedException();
        }

        # region Elements
        private IWebElement CrossReferneceType
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lblCRType"));
            }
        }
        private IWebElement FileToUpload
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label4"));
            }
        }
        private IWebElement Description
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_Label3"));
            }
        }
        # endregion
        # region Element Actions
        public string crosstyperefText()
        {
            const string crosstyperefPath = "ContentPageHolder_lblCRType";
            return webDriver.FindElement(By.Id(crosstyperefPath)).Text;
        }
        public string FileUploadText()
        {
            const string fileUploadTextPath = "ContentPageHolder_Label4";
            return webDriver.FindElement(By.Id(fileUploadTextPath)).Text;
        }
        public string DescriptionText()
        {
            const string descriptionPath = "ContentPageHolder_Label3";
            return webDriver.FindElement(By.Id(descriptionPath)).Text;
        }
        # endregion
    }
}
