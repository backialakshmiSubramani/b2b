using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Modules.Channel.B2B.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace Modules.Channel.B2B.Core.NewPages
{
    public class OstStandardConfigurationPage : PageBase
    {
        IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        /// <summary>
        /// Constructor to hand off webDriver
        /// </summary>
        /// <param name="webDriver"></param>
        public OstStandardConfigurationPage(IWebDriver webDriver)
            : base(ref webDriver)
        {
            this.webDriver = webDriver;
            javaScriptExecutor = (IJavaScriptExecutor)this.webDriver;
            
            this.webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(2));
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
        
        private IWebElement TxtDiscount
        {
            get
            {
                 return webDriver.FindElement(By.Id("txtDiscount"), TimeSpan.FromSeconds(30));                
            }
        }
        
        private IWebElement ContentPageHolderFrame
        {
            get
            {               
                    return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ifrmRedirect"), TimeSpan.FromSeconds(30));                
            }
        }

        public IWebElement BtnApplyChanges
        {
            get
            {
                return webDriver.FindElement(By.Id("btnApplyChanges"));  
            }
        }

        public IWebElement BtnUpdateNow
        {
            get
            {
                return webDriver.FindElement(By.Id("btnUpdateNow"));
            }
        }

        public string CurrentConfigPrice
        {
            get
            {
                return webDriver.FindElement(By.Id("lblConfigPrice")).Text;
            }
        }

        private IWebElement StandardConfigFrame
        {
            get
            {                
                return webDriver.FindElement(By.Id("ctl00_ContentPageHolder_ifrmRedirect"), TimeSpan.FromSeconds(30));
            }
        }
       
        public void SelectLiveConfig(string orderCode)
        {
            webDriver.SwitchTo().Frame(StandardConfigFrame);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));

            javaScriptExecutor.ExecuteScript("arguments[0].click();", webDriver.FindElement(By.XPath("//td[text() = '" + orderCode + "']")));
            webDriver.WaitForPageLoadNew(TimeSpan.FromSeconds(120));
        }

        public void OpenPriceChangeDialog(string linkId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
            webDriver.FindElement(By.Id(linkId)).SendKeys(Keys.Enter);
        }        

        public void SwitchWindowToPricingPopupToApplyDiscount(string discount)
        {
            string oldWindow = webDriver.CurrentWindowHandle;
            string popWindowHandle = null;
            WebDriverWait wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 5));
            wait.Until((d) => webDriver.WindowHandles.Count == 2);
            var windowHandles = webDriver.WindowHandles;
            ReadOnlyCollection<string> handles = new ReadOnlyCollection<string>(windowHandles);
            foreach (string handle in handles)
            {
                if (handle != oldWindow)
                {
                    popWindowHandle = handle;
                }
            }
            webDriver.SwitchTo().Window(popWindowHandle);
            TxtDiscount.Clear();
            TxtDiscount.SendKeys(discount);
            BtnApplyChanges.SendKeys(Keys.Enter);
            webDriver.SwitchTo().Window(oldWindow);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
        }

        private IAlert alertBox;
        public bool UpdateConfigrationByUpdateNow()
        {
            BtnUpdateNow.SendKeys(Keys.Enter);

            bool isAlertExist = false;

            while (!isAlertExist)
            {
                isAlertExist = IsAlertExists();
            }

             
            string alertMsg = alertBox.Text;
            alertBox.Accept();

            if (alertMsg.Contains("Pinpoint refresh call is successful."))
            {
                return true;
            }

            return false;            
        }

        public void SwitchToFrame()
        {
            webDriver.SwitchTo().Frame(ContentPageHolderFrame);
        }

        private bool IsAlertExists()
        {
            try
            {                
                alertBox = webDriver.SwitchTo().Alert();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SwitchToDefaultFrame()
        {
            webDriver.SwitchTo().DefaultContent();
        }
        public void SwitchToContentPageHolderFrame()
        {
            webDriver.SwitchTo().Frame(ContentPageHolderFrame);
        }
        public void UpdateSKULevelChanges()
        {
            IWebElement btnUpdate = webDriver.FindElement(By.XPath("//div[@id='floatMenuDiv']//*[@id='btnUpdateNow']"));
            javaScriptExecutor.ExecuteScript("arguments[0].click();", btnUpdate);
           // Thread.Sleep(20000);
        }

        public void HandleConfirmationAlert()
        {
            try
            {
                WebDriverWait _wait1 = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
                IAlert alertDialog = _wait1.Until<IAlert>(ExpectedConditions.AlertIsPresent());
                if (alertDialog != null)
                    alertDialog.Accept();
                webDriver.SwitchTo().DefaultContent();
            }
            catch (UnhandledAlertException)
            {

                try { webDriver.SwitchTo().Alert().Dismiss(); } catch (NoAlertPresentException) { }
                webDriver.SwitchTo().DefaultContent();
            }
            catch
            {
                webDriver.SwitchTo().DefaultContent();
            }


            try
            {
              
                webDriver.SwitchTo().Window(webDriver.WindowHandles.ToList().Last()).Close();


            }
            catch (UnhandledAlertException)
            {

                try { webDriver.SwitchTo().Alert().Dismiss(); } catch (NoAlertPresentException) { }
                webDriver.SwitchTo().DefaultContent();
            }
            catch
            {
                webDriver.SwitchTo().DefaultContent();
            }

        }

        public void HandleBSTAlert()
        {
            try
            {
                WebDriverWait _wait1 = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
                IAlert alertDialog = _wait1.Until<IAlert>(ExpectedConditions.AlertIsPresent());
                if (alertDialog != null)
                    alertDialog.Dismiss();
                webDriver.SwitchTo().DefaultContent();
            }
            catch (UnhandledAlertException)
            {

                try { webDriver.SwitchTo().Alert().Dismiss(); } catch (NoAlertPresentException) { }
                webDriver.SwitchTo().DefaultContent();
            }
            catch
            {
                webDriver.SwitchTo().DefaultContent();
            }

            webDriver.SwitchTo().Frame(StandardConfigFrame);
        }

        public void AddOrRemoveSkusToperformDelatOperations() //  SKULevelModifications
        {
            try
            {

                //Below are preconditions for generating the original Catalog and evaluating the followed Delta


                //**Module ID: 74
                //**SKU :412-1065
                //**Expected condition:Removing 412-1065 SKU checkbox from 74 Module

                IWebElement chkInclude_74_UPREMT_1 = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'UPREMT')]/parent::tr//input[contains(@id,'chkInclude_74_UPREMT')]"));
                chkInclude_74_UPREMT_1.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_UPREMT_1);

                //**Module ID: 74
                //**SKU :A3014456
                //**Expected condition "ON":Adding A3014456 SKU to 74 Module

                IWebElement chkInclude_74_3014456_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'3014456')]/parent::tr//input[contains(@id,'chkInclude_74_3014456')]"));
                chkInclude_74_3014456_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_3014456_On);
                //**Module ID: 74
                //**SKU :A3014456
                //**Expected condition "Default":Adding A3014456 SKU to 74 Module
                IWebElement chkDefault_74_3014456_Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'3014456')]/parent::tr//input[contains(@id,'chkDefault_74_3014456')]"));
                chkDefault_74_3014456_Def.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkDefault_74_3014456_Def);

                //**Module ID: 77
                //**SKU :331-2359
                //**Expected condition "ON":Adding 331-2359 SKU to 77 Module
                IWebElement chkInclude_77_3312359_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_77']//td[contains(text(),'3312359')]/parent::tr//input[contains(@id,'chkInclude_77_3312359')]"));
                chkInclude_77_3312359_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_77_3312359_On);

                //**Module ID: 77
                //**SKU :331-2359
                //**Expected condition "Default":Adding 331-2359 SKU to 77 Module
                IWebElement chkInclude_77_3312359_Default = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_77']//td[contains(text(),'3312359')]/parent::tr//input[contains(@id,'chkDefault_77_3312359')]"));
                chkInclude_77_3312359_Default.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_77_3312359_Default);


                //**Module ID: 107
                //**SKU :310-4729
                //**Expected condition :Removing 310-4729 SKU to 107 Module
                IWebElement chkInclude_107_3104729_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_107']//td[contains(text(),'50VGA')]/parent::tr//input[contains(@id,'chkInclude_107_50VGA')]"));
                chkInclude_107_3104729_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_107_3104729_On);

                //**Module ID: 132
                //**SKU :982-8638
                //**Expected condition :Removing 982-8638 SKU to 132 Module
                IWebElement chkInclude_132_9828638_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_132']//td[contains(text(),'PROJSPK')]/parent::tr//input[contains(@id,'chkInclude_132_PROJSPK')]"));
                chkInclude_132_9828638_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_132_9828638_On);

                //**Module ID: 185
                //**SKU :311-8288
                //**Expected condition "ON" :Adding 311-8288 SKU to 185 Module
                IWebElement chkInclude_185_3118288_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_185']//td[contains(text(),'3118288')]/parent::tr//input[contains(@id,'chkInclude_185_3118288')]"));
                chkInclude_185_3118288_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_185_3118288_On);

                //**Module ID: 185
                //**SKU :311-8288
                //**Expected condition "Default" :Adding 311-8288 SKU to 185 Module
                IWebElement chkInclude_185_3118288_Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_185']//td[contains(text(),'3118288')]/parent::tr//input[contains(@id,'chkDefault_185_3118288')]"));
                chkInclude_185_3118288_Def.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_185_3118288_Def);
                
            }
            catch (Exception)
            {
                IWebElement testabc = webDriver.FindElement(By.Id("updPanelConfigHeader"));
            }
        }

        public void FindByXPathAndPerformClick(string xPath)
        {
            IWebElement elem = webDriver.FindElement(By.XPath(xPath));
            if (elem != null)
                javaScriptExecutor.ExecuteScript("arguments[0].click()", elem);
        }

        public void ResetModuleSkus()
        {
            try
            {

                //**Module ID: 74
                //**SKU :412-1065
                //**Expected condition "ON":Adding 412-1065 SKU checkbox from 74 Module

                IWebElement chkInclude_74_UPREMT_1_On_reset = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'UPREMT')]/parent::tr//input[contains(@id,'chkInclude_74_UPREMT')]"));
                chkInclude_74_UPREMT_1_On_reset.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_UPREMT_1_On_reset);

                //**Module ID: 74
                //**SKU :412-1065
                //**Expected condition "Default":Adding 412-1065 SKU checkbox from 74 Module

                IWebElement chkInclude_74_UPREMT_1_reset_Default = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'UPREMT')]/parent::tr//input[contains(@id,'chkDefault_74_UPREMT')]"));
                chkInclude_74_UPREMT_1_reset_Default.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_UPREMT_1_reset_Default);

                //**Module ID: 74
                //**SKU :A3014456
                //**Expected condition :Removing  A3014456 SKU checkbox from 74 Module

                IWebElement chkInclude_74_3014456_1_reset_Default = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'3014456')]/parent::tr//input[contains(@id,'chkInclude_74_3014456')]"));
                chkInclude_74_3014456_1_reset_Default.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_3014456_1_reset_Default);


                //**Module ID: 77
                //**SKU :331-2359
                //**Expected condition :Removing  331-2359 SKU checkbox from 77 Module

                IWebElement chkInclude_77_3312359_1_reset_Default = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_77']//td[contains(text(),'3312359')]/parent::tr//input[contains(@id,'chkInclude_77_3312359')]"));
                chkInclude_77_3312359_1_reset_Default.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_77_3312359_1_reset_Default);


                //**Module ID: 107
                //**SKU :310-4729
                //**Expected condition "ON" :Adding  310-4729 SKU checkbox from 1077 Module

                IWebElement chkInclude_107_50VGA_1_reset_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_107']//td[contains(text(),'50VGA')]/parent::tr//input[contains(@id,'chkInclude_107_50VGA')]"));
                chkInclude_107_50VGA_1_reset_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_107_50VGA_1_reset_On);


                //**Module ID: 107
                //**SKU :310-4729
                //**Expected condition "Default" :Adding  310-4729 SKU checkbox from 107 Module

                IWebElement chkInclude_107_50VGA_1_reset_Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_107']//td[contains(text(),'50VGA')]/parent::tr//input[contains(@id,'chkDefault_107_50VGA')]"));
                chkInclude_107_50VGA_1_reset_Def.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_107_50VGA_1_reset_Def);

                //**Module ID: 132
                //**SKU :982-8638
                //**Expected condition "ON" :Adding  982-8638 SKU checkbox from 132 Module

                IWebElement chkInclude_132_9828638_1_reset_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_132']//td[contains(text(),'PROJSPK')]/parent::tr//input[contains(@id,'chkInclude_132_PROJSPK')]"));
                chkInclude_132_9828638_1_reset_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_132_9828638_1_reset_On);

                //**Module ID: 132
                //**SKU :982-8638
                //**Expected condition "Default" :Adding  982-8638 SKU checkbox from 132 Module

                IWebElement chkInclude_132_9828638_1_reset_Default = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_132']//td[contains(text(),'PROJSPK')]/parent::tr//input[contains(@id,'chkDefault_132_PROJSPK')]"));
                chkInclude_132_9828638_1_reset_Default.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_132_9828638_1_reset_Default);

                //**Module ID: 185
                //**SKU :311-8288
                //**Expected condition  :Removing  982-8638 SKU checkbox from 185 Module

                IWebElement chkInclude_185_3118288_1_reset_On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_185']//td[contains(text(),'3118288')]/parent::tr//input[contains(@id,'chkInclude_185_3118288')]"));
                chkInclude_185_3118288_1_reset_On.SendKeys(Keys.Enter);
                javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_185_3118288_1_reset_On);
              

            }
            catch (Exception)
            {
                IWebElement testabc = webDriver.FindElement(By.Id("updPanelConfigHeader"));
            }

        }
        public void SelectOrderCode(string orderCode)
        {
            webDriver.SwitchTo().Frame(StandardConfigFrame);
            WebDriverWait _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
            _wait.Until<bool>(w =>
            {
                return ((IJavaScriptExecutor)w).ExecuteScript("return document.readyState").ToString().Equals("complete", StringComparison.OrdinalIgnoreCase);
            });

            WebDriverWait _wait1 = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
            _wait1.PollingInterval = TimeSpan.FromSeconds(5);
            IWebElement firstRowOrder = _wait1.Until<IWebElement>(w =>
            {
                try
                {
                    return webDriver.FindElement(By.XPath("//*[@id='" + orderCode + "']"));
                }
                catch (NoSuchElementException)
                {
                    webDriver.SwitchTo().DefaultContent();
                    webDriver.SwitchTo().Frame(StandardConfigFrame);
                    return null;
                }
            });

            javaScriptExecutor.ExecuteScript("arguments[0].click()", firstRowOrder);
        }

        /// <summary>
        /// Clicks on Go button and navigates to Catalog and Pricing Page
        /// </summary>
        /// <param name="accountId"></param>
        public void GoToCatalogAndPricingPage(string accountId)
        {
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(30));
          //  AccountId.SendKeys(accountId);
            ////GoButton.Click();
          //  javaScriptExecutor.ExecuteScript("arguments[0].click();", GoButton);
            webDriver.WaitForElementDisplayed(By.Id("ctl00_brdcrbControl_lbl_PageMigrationinfo"), TimeSpan.FromSeconds(60));
        }


        public void CheckPreConditionsModuleSkus()
        {
            try
            {
                //Below are preconditions for generating the original Catalog and evaluating the followed Delta
                

                //**Module ID: 74
                //**SKU :412-1065
                //**Expected pre-condition:"ON" checkbox To be selected
               
                IWebElement chkInclude_74_UPREMT = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'UPREMT')]/parent::tr//input[contains(@id,'chkInclude_74_UPREMT')]"));

                //**Module ID: 74
                //**SKU :412-1065
                //checking the status of ON checkbox , if not selected , changing it to selected. 
                if (chkInclude_74_UPREMT.Selected == false)
                {
                    chkInclude_74_UPREMT.SendKeys(Keys.Enter);
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_UPREMT);
                }

                //**Module ID: 74
                //**SKU :412-1065
                //**Expected pre-condition:"Default" checkbox To be selected

                IWebElement chkDefault_74_UPREMT = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'UPREMT')]/parent::tr//input[contains(@id,'chkDefault_74_UPREMT')]"));
                //**Module ID: 74
                //**SKU :412-1065
                //checking the status of Default checkbox , if not selected , changing it to selected.
                if (chkDefault_74_UPREMT.Selected == false)
                {
                    chkDefault_74_UPREMT.SendKeys(Keys.Enter);
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", chkDefault_74_UPREMT);

                }

                //**Module ID: 74
                //**SKU :A3014456
                //**Expected pre-condition:"ON" checkbox To be selected

                IWebElement chkInclude_74_3014456 = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'3014456')]/parent::tr//input[contains(@id,'chkInclude_74_3014456')]"));
                if (chkInclude_74_3014456.Selected == true)
                {
                    chkInclude_74_3014456.SendKeys(Keys.Enter);
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", chkInclude_74_3014456);
                }
                //**Module ID: 74
                //**SKU :A3014456
                //checking the status of Default checkbox .
                IWebElement addRemoveSkuDefault = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_74']//td[contains(text(),'3014456')]/parent::tr//input[contains(@id,'chkDefault_74_3014456')]"));
                if (addRemoveSkuDefault.Selected == true)
                {

                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", addRemoveSkuDefault);
                }

               

                //**Module ID: 77
                //**SKU :331-2359
                //**Expected pre-condition:"ON" checkbox


                IWebElement Sku2359On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_77']//td[contains(text(),'3312359')]/parent::tr//input[contains(@id,'chkInclude_77_3312359')]"));

                if (Sku2359On.Selected == true)
                {
                    Sku2359On.SendKeys(Keys.Enter);
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", Sku2359On);
                }
                //**Module ID: 77
                //**SKU :331-2359
                //checking the status of Default checkbox .
                IWebElement Sku2359OnDefault = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_77']//td[contains(text(),'3312359')]/parent::tr//input[contains(@id,'chkDefault_77_3312359')]"));
                if (Sku2359OnDefault.Selected == true)
                {
                    Sku2359OnDefault.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", Sku2359OnDefault);
                }

                

                //**Module ID: 107
                //**SKU :310-4729
                //**Expected pre-condition:"ON" checkbox

                IWebElement skuOn3104729 = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_107']//td[contains(text(),'50VGA')]/parent::tr//input[contains(@id,'chkInclude_107_50VGA')]"));

              
                if (skuOn3104729.Selected == false)
                {
                    skuOn3104729.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", skuOn3104729);
                }
                //**Module ID: 107
                //**SKU :310-4729
                //**Expected pre-condition:"Default" checkbox
                IWebElement skuOn3104729Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_107']//td[contains(text(),'50VGA')]/parent::tr//input[contains(@id,'chkDefault_107_50VGA')]"));
                if (skuOn3104729Def.Selected == false)
                {
                    skuOn3104729Def.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", skuOn3104729Def);
                }

              
                //**Module ID: 132
                //**SKU :982-8638
                //**Expected pre-condition:"ON" checkbox

                IWebElement sku9828638On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_132']//td[contains(text(),'PROJSPK')]/parent::tr//input[contains(@id,'chkInclude_132_PROJSPK')]"));
                if (sku9828638On.Selected == false)
                {
                    sku9828638On.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", sku9828638On);
                }
                //**Module ID: 132
                //**SKU :982-8638
                //**Expected pre-condition:"DEF" checkbox
                IWebElement sku9828638Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_132']//td[contains(text(),'PROJSPK')]/parent::tr//input[contains(@id,'chkDefault_132_PROJSPK')]"));
                if (sku9828638Def.Selected == false)
                {
                    sku9828638Def.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", sku9828638Def);
                }

             
                //**Module ID: 185
                //**SKU :311-8288
                //**Expected pre-condition:"ON" checkbox

                IWebElement chkInclude_185_3118288On = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_185']//td[contains(text(),'3118288')]/parent::tr//input[contains(@id,'chkInclude_185_3118288')]"));
                if (chkInclude_185_3118288On.Selected == true)
                {
                    chkInclude_185_3118288On.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", chkInclude_185_3118288On);
                }

                //**Module ID: 185
                //**SKU :311-8288
                //**Expected pre-condition:"Def" checkbox

                IWebElement chkInclude_185_3118288Def = webDriver.FindElement(By.XPath("//div[@id='module_collection']//div[@id='divAll']//div[@id='module_detail_panel_185']//td[contains(text(),'3118288')]/parent::tr//input[contains(@id,'chkDefault_185_3118288')]"));
                if (chkInclude_185_3118288Def.Selected == true)
                {
                    chkInclude_185_3118288Def.SendKeys(Keys.Enter);
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", chkInclude_185_3118288Def);
                }
            }
            catch (Exception)
            {
                IWebElement testabc = webDriver.FindElement(By.Id("updPanelConfigHeader"));
            }
        }




    }
}
