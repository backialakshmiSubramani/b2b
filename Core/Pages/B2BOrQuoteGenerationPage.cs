// ***********************************************************************
// Author           : AMERICAS\Gaurav_Bhardwaj3
// Created          : 12/10/2014 2:57:03 PM
//
// Last Modified By : AMERICAS\Gaurav_Bhardwaj3
// Last Modified On : 12/10/2014 2:57:03 PM
// ***********************************************************************
// <copyright file="B2BOrQuoteGenerationPage.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
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
    /// <summary>
    /// This base class is the where all specific page classes will be derived.
    /// </summary>
    public class B2BOrQuoteGenerationPage : PageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BOrQuoteGenerationPage(IWebDriver webDriver)
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

        #region Element
        private IWebElement CreatePoElement
        {
            get
            { return webDriver.FindElement(By.XPath("//div[@id='tabs']/form/table/tbody/tr[3]/td[1]/input")); }
        }

        private IWebElement XmlDocument
        {
            get
            {
                return webDriver.FindElement(By.Id("IncommingMessage"));
            }
        }
        #endregion

        #region Element Actions

        public string GetCreatePoButtonText()
        {
            return CreatePoElement.Text;
        }

        public string FindOrQuote()
        {
            XDocument doc = XDocument.Parse(XmlDocument.Text);
            Console.WriteLine(doc.XPathSelectElement("//OrderRequest/ListOfOrderRequestDetail/OrderRequestDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value);
            return doc.XPathSelectElement("//OrderRequest/ListOfOrderRequestDetail/OrderRequestDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value;
            //var xElement = doc.Element("PartIDExt");
            //if (xElement != null) Console.WriteLine(xElement.Value);
        }

        #endregion
    }
}
