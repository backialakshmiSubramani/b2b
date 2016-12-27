// ***********************************************************************
// Author           : AMERICAS\Dushyant_Kotecha
// Created          : 11/25/2016 3:12:50 PM
//
// Last Modified By : AMERICAS\Shaukat_Saleem
// Last Modified On : 10/13/2016 3:12:50 PM
// ***********************************************************************
// <copyright file="OstFlowPage.cs" company="Dell">
//     Copyright (c) Dell 2016. All rights reserved.
// </copyright>
// <summary>Provide a summary of the page class here.</summary>
// ***********************************************************************

using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Modules.Channel.B2B.Core.NewPages
{
    public class OstSNPPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor _javaScriptExecutor;

        public OstSNPPage(IWebDriver webDriver) : base(ref webDriver)
        {
            this.webDriver = webDriver;
            _javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
        }

        public override bool IsActive()
        {
            throw new NotImplementedException();
        }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
       
        private IWebElement LeftIframe
        {
            get
            {
                return webDriver.FindElement(By.Id("leftFrame"), TimeSpan.FromSeconds(30));
            }
        }

        /// <summary>
        /// Gets the account id.
        /// </summary>
        private IWebElement AccountId
        {
            get
            {
                webDriver.WaitForElementDisplayed(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_txtbxAutoComplete"), new TimeSpan(0, 0, 30));
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_txtbxAutoComplete"));
            }
        }

        /// <summary>
        /// Gets the go button.
        /// </summary>
        private IWebElement GoButton
        {
            get
            {
                return webDriver.FindElement(By.Id("ctl00_topHeaderControl_tbContrHeader_tbpnlPgSetting_imgbtnGo"));
            }
        }

        private IWebElement CutomizeCategoriesAndManufacturers
        {
            get
            {
                return webDriver.FindElement(By.XPath("//div[@id='treeContainer']/div[3]/img[1]"), TimeSpan.FromSeconds(30));
            }
        }

        private IWebElement AudioSubtreeOption
        {
            get
            {
                return webDriver.FindElement(By.XPath("//div[@id='COCategory2999']/div/img[2]"), TimeSpan.FromSeconds(30));
            }
        }        

        private IWebElement AudioSpeakers
        {
            get
            {
                return webDriver.FindElement(By.Id("TISubclass7564_192"));
            }
        }

        private IWebElement Link_ViewSkus
        {
            get
            {
                return webDriver.FindElement(By.Id("__tab_SubclassManager_TabPanel3"));
            }
        }

        private IWebElement DropDown_Status_ViewSKU
        {
            get
            {
                return webDriver.FindElement(By.XPath("//tr[@class='Cat_Mgmt_Top']/following-sibling::tr[2]//select[@class='Cat_Mgmt_Text_White']"));
            }
        }

        private IWebElement StatusDropDown
        {
            get
            {
                return webDriver.FindElement(By.Id("SubclassManager_TabPanel3_SkuListControl1_ProductsGrid_ctl02_StatusDropDown"));
            }
        }

        private IWebElement UpdateButton
        {
            get
            {
                return webDriver.FindElement(By.Id("SubclassManager_TabPanel3_SkuListControl1_Update"));
            }
        }

        private IWebElement IsVerifyClassicViewLink
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='ctl00_MainContentPlaceHolder_lnkBtnSwitch']"));
            }
        }
        private IWebElement IsVerifyClassicViewLinks
        {
            get
            {
                return webDriver.FindElement(By.XPath("//*[@id='ctl00_MainContentPlaceHolder_lnkBtnSwitch']"));
            }
        }


        private IWebElement SoftwareAndPeripheralsLink
        {
            get
            {
                return webDriver.FindElement(By.LinkText("Software and Peripherals"));
            }
        }

    

        public void OpenOSTHomePage()
        {
            webDriver.Navigate().GoToUrl(ConfigurationReader.GetValue("OSTHomePageUrl"));
        }

        public void SearchStoreInOST(string accountId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            AccountId.SendKeys(accountId);            
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", GoButton);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
            webDriver.WaitForElementDisplayed(By.Id("ctl00_brdcrbControl_lbl_PageMigrationinfo"), TimeSpan.FromSeconds(60));
        }
        
        public void GotoSNPPage()
        {
            SoftwareAndPeripheralsLink.SendKeys(Keys.Enter);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(120));
        }

        public void ClickOnCutomizeCategoriesAndManufacturers()
        {
            SwitchToLeftIFrame();
            webDriver.SwitchTo().Frame(0);
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", CutomizeCategoriesAndManufacturers);           
        }

        public void ClickOnAudioSubtreeOption()
        {
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", AudioSubtreeOption);            
        }

        public void ClickOnAudioSpeakers()
        {
            AudioSpeakers.SendKeys(Keys.Enter);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
        }

        public void SelectSKUViewStatus(string status)
        {
            webDriver.SwitchTo().DefaultContent();
            webDriver.SwitchTo().Frame(0);
            webDriver.SwitchTo().Frame(1);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(30));
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", Link_ViewSkus);
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
            SelectElement pickList = new SelectElement(DropDown_Status_ViewSKU);
            pickList.SelectByText(status);
           
          //  ChangeStatus();
            UpdateStatus();
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
            webDriver.SwitchTo().DefaultContent().SwitchTo().DefaultContent();
        }
        public void ChangeStatus()
        {
            SelectElement selector = new SelectElement(StatusDropDown);
            selector.SelectByIndex(0);
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", selector);            
        }

        public void UpdateStatus()
        {

          //  UpdateButton.SendKeys(Keys.Enter);
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", UpdateButton);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(60));
        }

        private void SwitchToLeftIFrame()
        {
            webDriver.SwitchTo().Frame(0);
            //webDriver.SwitchTo().Frame(LeftIframe);
         
           // _javaScriptExecutor.ExecuteScript()
        }

        public void SwtichToClassicView()
        {
            IsVerifyClassicViewLinks.Click();
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
        }

        public bool IsVerifyClassicView()
        {
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(60));
            //SelectElement link = new SelectElement(IsVerifyClassicViewLinks);
            WebDriverWait _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
            bool isElementVisible = false;
            try
            {
                IWebElement _webElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='ctl00_MainContentPlaceHolder_lnkBtnSwitch']")));
                isElementVisible = _webElement.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                isElementVisible = false;
            }

            return isElementVisible;
        }
    }
}
