// ***********************************************************************
// Author           : AMERICAS\Nethra_Pandappilav
// Created          : 12/29/2014 12:15:19 PM
//
// Last Modified By : AMERICAS\Nethra_Pandappilav
// Last Modified On : 12/29/2014 12:15:19 PM
// ***********************************************************************
// <copyright file="B2BViewCrossReferenceXmlPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

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
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BViewCrossReferenceXmlPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BViewCrossReferenceXmlPage(IWebDriver webDriver)
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

        private IWebElement XmlHeader
        {
            get
            {
                return webDriver.FindElement(By.XPath("//body/div[1]"));
            }
        }

        public bool ParsePageSourceToXml()
        {
            return XmlHeader.Text.Replace(" ", string.Empty).ToUpper().Contains("<?XMLVERSION=\"1.0\"ENCODING=\"UTF-8\"?>");
        }

        public XElement GetCrtValuesFromXml()
        {
            XDocument pageSourceXml;

            try
            {
                pageSourceXml =
                    XDocument.Parse(
                        webDriver.FindElement(By.TagName("body"))
                            .Text.Trim()
                            .Replace("- ", string.Empty)
                            .Replace("\n", string.Empty)
                            .Replace("\r", string.Empty));
            }
            catch
            {
                return null;
            }

            return pageSourceXml.XPathSelectElements("//CRTValues/CRTValue").FirstOrDefault();
        }
    }
}
