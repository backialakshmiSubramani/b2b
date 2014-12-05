// ***********************************************************************
// Author           : AMERICAS\Vinay_Chand
// Created          : 12/3/2014 3:53:48 PM
//
// Last Modified By : AMERICAS\Vinay_Chand
// Last Modified On : 12/3/2014 3:53:48 PM
// ***********************************************************************
// <copyright file="CrossReferenceListPage.cs" company="Dell">
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
    public class B2BCrossReferenceListPage : DCSGPageBase
    {
        IWebDriver webDriver;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceListPage(IWebDriver webDriver)
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

        #region Elements
        private IWebElement ChooseCrossReferenceTypeList
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType"));

            }
        }




        private IWebElement ViewCrList
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));

            }
        }


        public IWebElement CrossReferenceTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList"));

            }
        }

        public IList<IWebElement> CrossReferenceTypeColumn
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath(
                            "//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr/td[3]"));
            }
        }

        #endregion

        # region Element Actions

        public void SelectCrTypeList(string CrossReferenceTypeList)
        {
            SelectElement crossReferenceTypeList = new SelectElement(ChooseCrossReferenceTypeList);
            crossReferenceTypeList.SelectByValue(CrossReferenceTypeList);
        }

        public void ClickViewCrList()
        {
            ViewCrList.Click();
        }

        public IList<IWebElement> GetTypeColumnValues()
        {
            return CrossReferenceTypeColumn;
        }

        # endregion
    }
}
