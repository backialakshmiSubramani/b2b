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
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

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
    public class B2BCrossReferenceListPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;

        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public B2BCrossReferenceListPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            //populate the following variables with the appropriate value
            Name = "B2B Preview Cross Reference List Page";
            Url = webDriver.Url;
            ProductUnit = "Channel";
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));

        }

        /// <summary>
        /// Treat this like a BVT of the page. If Validate does not pass, throw exception and console.writeline a return message into Test Class
        /// </summary>
        /// <returns>validated</returns>
        public override bool Validate()
        {
            return PageTitleHeader.Displayed;
        }

        /// <summary>
        /// determines whether or not the driver is active on this page. Must be overriden with each subclass.
        /// </summary>
        /// <returns>active</returns>
        public override bool IsActive()
        {
            return webDriver.Url.Contains("/B2BToolsCE");
        }

        #region Elements


        private IWebElement _pageTitleHeader;
        private IWebElement PageTitleHeader
        {
            get
            {
                if (_pageTitleHeader == null)
                    _pageTitleHeader = webDriver.FindElement(By.Id("ucBreadCrumb_lblPageTitle"), new TimeSpan(0, 0, 10));
                return _pageTitleHeader;
            }
        }
        private SelectElement ChooseCrossReferenceTypeList
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType")));
            }
        }

        private IWebElement ViewCrList
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
            }
        }

        private IEnumerable<IWebElement> CrossReferenceTableRows
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath("//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr"))
                        .Skip(1);
            }
        }

        private IList<IWebElement> CrossReferenceTypeColumn
        {
            get
            {
                return
                    webDriver.FindElements(
                        By.XPath(
                            "//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr/td[3]"));
            }
        }

        private IWebElement NewCrossReference
        {
            get
            {
                webDriver.WaitForElement(By.Id("ContentPageHolder_lnk_btnNew"), TimeSpan.FromSeconds(30));
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnNew"));
            }
        }

        /// <summary>
        /// Crt Reference List Table Rows
        /// </summary>
        private IList<IWebElement> CrtReferenceListTableRows
        {
            get
            {
                return
                   webDriver.FindElements(
                       By.XPath("//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr"));

            }
        }

        /// <summary>
        /// Cross reference type dropdown
        /// </summary>
        private string _crossReferenceTypeDropDown;
        private string crossReferenceTypeDropDown
        {
            get
            {
                if (string.IsNullOrEmpty(_crossReferenceTypeDropDown))
                    _crossReferenceTypeDropDown = "Channel_Cross_Segment_Booking";
                return _crossReferenceTypeDropDown;
            }
        }

        /// <summary>
        /// CRT DropDown
        /// </summary>
        private IWebElement _selectCRTDropDown;
        private IWebElement selectCRTDropDown
        {
            get
            {
                if (_selectCRTDropDown == null)
                    _selectCRTDropDown = webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType"));
                return _selectCRTDropDown;
            }
        }

        /// <summary>
        /// Search CRT List link
        /// </summary>
        private IWebElement _searchCRTList;
        private IWebElement searchCRTList
        {
            get
            {
                if (_searchCRTList == null)
                    _searchCRTList = webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
                return _searchCRTList;
            }
        }

        public SelectElement AccountName
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRT_Profiles")));
            }
        }

        public SelectElement CrossReferenceType
        {
            get
            {
                return new SelectElement(webDriver.FindElement(By.Id("ContentPageHolder_drp_CRTType")));
            }
        }

        public IWebElement Search
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_lnk_btnSearch"));
            }
        }

        public IWebElement CRTResultTable
        {
            get
            {
                return webDriver.FindElement(By.Id("ContentPageHolder_CRTGridAssoList_grdVwCrossReferenceAssociationsList"));
            }
        }

        #endregion

        # region Element Actions

        public void SelectCrTypeList(string CrossReferenceTypeList)
        {
            ChooseCrossReferenceTypeList.SelectByValue(CrossReferenceTypeList);
        }

        public void ClickViewCrList()
        {
            ////ViewCrList.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", ViewCrList);
        }

        public IList<IWebElement> GetTypeColumnValues()
        {
            return CrossReferenceTypeColumn;
        }
        public bool CheckChannelSegmentBookingDropDown(string ListOptions)
        {
            return ChooseCrossReferenceTypeList.Options.Any(e => e.Text.Contains(ListOptions));
        }
        public void ClickNewCrossReference()
        {
            ////NewCrossReference.Click();
            javaScriptExecutor.ExecuteScript("arguments[0].click();", NewCrossReference);

            // Both CrossReferenceList page and CrossReferenceMaintenance page have same element Cross Reference Type List
            // hence wait for page load after click
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(10));
        }

        public void ViewXmlForCrId(string crId)
        {
            var rowWithCrId = CrossReferenceTableRows.FirstOrDefault(e => e.FindElements(By.TagName("td"))[1].Text.Trim().Equals(crId));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", rowWithCrId.FindElement(By.XPath("//td[6]/a")));
            webDriver.WaitForPageLoad(new TimeSpan(0, 0, 10));
            var newWindow = webDriver.WindowHandles.LastOrDefault();
            webDriver.SwitchTo().Window(newWindow);
            Console.WriteLine("Url after switching windows is: {0}", webDriver.Url);
        }

        /// <summary>
        /// click on description for given CR ID in CR List
        /// </summary>
        /// <param name="crId"></param>
        /// <returns></returns>
        public B2BCrossReferenceMaintenancePage clickonDescriptionforCrID(string crId)
        {
            // var rowWithCrID = CrossReferenceTableRows.FirstOrDefault(e => e.FindElements(By.TagName("td"))[1].Text.Trim().Equals(crId));
            for (int j = 0; j < CrtReferenceListTableRows.Count; j++)
            {
                if (CrtReferenceListTableRows[j].Text.Contains(crId))
                {
                    var currentElement =
                        CrtReferenceListTableRows[j].FindElement(By.TagName("a"));
                    // CrtReferenceListTableRows[j].FindElement(By.TagName("/a[contains(@href,'#//CrossReferenceMaintenance.aspx?CRTID=')"));
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", currentElement);
                }
            }
            this.webDriver.SwitchTo().DefaultContent();
            return new B2BCrossReferenceMaintenancePage(this.webDriver);
        }

        /// <summary>
        /// Get CRT ID from Cross Reference List Table
        /// </summary>
        public string CrossReferenceListTableCrtId
        {
            get
            {
                const string rowPath = "//table[@id='ContentPageHolder_CRTGridCRTList_grdVwCrossReferenceList']/tbody/tr[2]/td[2]";
                return webDriver.FindElement(By.XPath(rowPath)).Text;
            }
        }

        /// <summary>
        ///Search based on given crt Type, by default  Channel_Cross_Segment_Booking will get filtered 
        /// </summary>
        /// <param name="crtType"></param>
        public void FilterCRT(string crtType)
        {
            this._crossReferenceTypeDropDown = crtType;
            selectCRTDropDown.Select().SelectByText(crossReferenceTypeDropDown);
            searchCRTList.Click();
        }
        # endregion

        public void OpenCRTXML(string userId)
        {
            CRTResultTable.FindElement(By.XPath(string.Format("//td[text()='{0}']/..//a[text()='View Xml']", userId))).Click();
        }

    }
}
