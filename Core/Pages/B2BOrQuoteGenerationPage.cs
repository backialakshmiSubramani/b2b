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
    public class B2BOrQuoteGenerationPage : DCSGPageBase
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

        # region Element
        private IWebElement CreatePoElement
        {
            get
            { return webDriver.FindElement(By.XPath("//div[@id='tabs']/form/table/tbody/tr[3]/td[1]/input")); }
        }

        private IWebElement Xml_Document
        {
            get
            { return webDriver.FindElement(By.Id("IncommingMessage")); }
        }
        # endregion

        # region Element Actions

        public String Create_PO_Button_Text()
        {
            return CreatePoElement.Text;
        }

        public void Find_OrQuote()
        {
            XDocument doc = XDocument.Parse(Xml_Document.Text);
            Console.WriteLine(doc.XPathSelectElement("//OrderRequest/ListOfOrderRequestDetail/OrderRequestDetail/BaseItemDetail/SupplierPartNum/PartNum/PartIDExt").Value);
            //var xElement = doc.Element("PartIDExt");
            //if (xElement != null) Console.WriteLine(xElement.Value);
        }

        # endregion
    }
}