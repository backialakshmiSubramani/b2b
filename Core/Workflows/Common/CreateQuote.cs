using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using DCSG.ADEPT.Framework;
using DCSG.ADEPT.Framework.Core;
using DCSG.ADEPT.Framework.Core.Extensions.WebDriver;
using DCSG.ADEPT.Framework.Core.Extensions.WebElement;
using DCSG.ADEPT.Framework.Core.Extensions.Locators;
using DCSG.ADEPT.Framework.Core.Page;
using Modules.Channel.B2B.Core.Pages;
using Modules.Channel.B2B.Common;
using Modules.Channel.B2B.Core.Workflows.Common;
using Modules.Channel.B2B.Core.Workflows.EUDC;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class CreateQuote
    {
        private IWebDriver webDriver;
        private IJavaScriptExecutor javaScriptExecutor;
        private PoOperations poOperation;
        private BuyerCatalog buyerCatalog;

        public CreateQuote(IWebDriver Driver)
        {
            webDriver = Driver;
        }

        /// <summary>
        /// Page object creation to use its methods
        /// </summary>
        private B2BHomePage B2BHomePage
        {
            get { return new B2BHomePage(webDriver); }
        }

        private B2BQaToolsPage B2BQaToolsPage
        {
            get { return new B2BQaToolsPage(webDriver); }
        }

        private B2BPremierDashBoardPage B2BPremierDashBoardPage
        {
            get { return new B2BPremierDashBoardPage(webDriver); }
        }

        private B2BStandardConfigurationPage StandardConfigurationPage
        {
            get { return new B2BStandardConfigurationPage(webDriver); }
        }

        private B2BShopingCartPage ShopingCartPage
        {
            get { return new B2BShopingCartPage(webDriver); }
        }

        private B2BEQuoteDetailsPage eQuoteDetailsPage
        {
            get { return new B2BEQuoteDetailsPage(webDriver); }
        }

        private B2BEQuoteSummaryPage eQuoteSummaryPage
        {
            get { return new B2BEQuoteSummaryPage(webDriver); }
        }

        private B2BFinalEquoteSummaryPage finalEquoteSummaryPage
        {
            get
            { return new B2BFinalEquoteSummaryPage(webDriver);}
        }

        private B2BEQuoteGenerationPage eQuoteGenerationPage
        {
            get
            { return new B2BEQuoteGenerationPage(webDriver); }
        }

        private B2BSecureCheckoutPage b2BSecureCheckoutPage
        {
            get
            { return new B2BSecureCheckoutPage(webDriver); }
        }

        private B2BTermsOfSalesPage b2BTermsOfSalesPage
        {
            get
            { return new B2BTermsOfSalesPage(webDriver); }
        }

        private B2BOrQuoteGenerationPage b2BOrQuoteGenerationPage
        {
            get
            { return new B2BOrQuoteGenerationPage(webDriver); }
        }

        private B2BCatalogViewer b2bCatalogViewerPage
        {
            get
            {
                return new B2BCatalogViewer(webDriver);
            }
        }

        private PoOperations poOperations
        {
            get
            {
                return new PoOperations(webDriver);
            }
        }

        private B2BCatalogViewer b2bCatalogViewer
        {
            get
            {
                return new B2BCatalogViewer(webDriver);
            }
        }

        /* How to create 'E-Quote' or 'OrQuote'
         * Call CompleteEQuoteGeneration() with 5 arguments -
         * i) quoteType(equote/OrQuote)     <-- Note pass only "equote" for E-Quote and "OrQuote" for OR-Quote
         * ii) environment - "Preview" or "Prod"
         * iii) profileId - In String 
         * iv) name - any name      <- will be used in equote
         * v) email - any email     <- will be used in equote        
         */

        public bool CompleteEQuoteGeneration(string quoteType,
            string env,
            string profileId, 
            string name, 
            string email,
            RunEnvironment environment,
            Workflow workflow,
            PoXmlFormat format,
            string deploymentMode,
            string orderIdBase,
            string crtFilePath,
            string poTargetUrl,
            string gcmUrl,
            string endUserId)
        {
            string eQuoteType = "eQuote";
            string orType = "OrQuote";
            string responseCode = "0";
            string quote_num = "0";
            string price = "0";


            // Creates Equote / OR Quote
            var parentWindow = webDriver.CurrentWindowHandle;
            Console.WriteLine("parent window {0}", parentWindow);

            B2BHomePage.SelectEnvironment(env);
            B2BHomePage.ClickQaTools3();
            System.Threading.Thread.Sleep(3000);

            webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
            var QaToolsWindow = webDriver.CurrentWindowHandle;
            Console.WriteLine("Qa Tools window {0}", QaToolsWindow);

            B2BQaToolsPage.ClickLocationEnvironment(env);
            B2BQaToolsPage.ClickLocationEnvironmentLink(env);
            B2BQaToolsPage.ClickPunchoutCreate();
            B2BQaToolsPage.Click_Cxml();
            B2BQaToolsPage.ClickCxmlMainCreate();
            B2BQaToolsPage.Profile_For_ProfileCorrelator(profileId);
            B2BQaToolsPage.Profile_For_UserIdIdentity(profileId);
            B2BQaToolsPage.Click_ApplyParameter();
            B2BQaToolsPage.ClickSubmitMessage();
            responseCode = B2BQaToolsPage.GetSubmissionResult();
            Console.WriteLine("Response Code is :- " + responseCode);

            if (responseCode.Contains("200"))
            {
                String temp = B2BQaToolsPage.GetStoreLinkText();
                Console.WriteLine("Link value is " + temp);
                B2BQaToolsPage.ClickStoreLink();

                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                webDriver.Close();
                //String newWindow = webDriver.WindowHandles.LastOrDefault();
                //webDriver.SwitchTo().Window(newWindow);
                webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
                var premierWindow = webDriver.CurrentWindowHandle;
                Console.WriteLine("premier window {0}", premierWindow);

                webDriver.Manage().Window.Maximize();
                B2BPremierDashBoardPage.Wait_For_Title();

                B2BPremierDashBoardPage.Open_Shop();
                B2BPremierDashBoardPage.Click_Standard_Config();

                StandardConfigurationPage.Select_First_Config();
                StandardConfigurationPage.ClickAddSelectedToCartButton();

                Console.WriteLine("Shoping Cart Page Title is :- " + ShopingCartPage.Return_Shoping_Cart_Title());

                ShopingCartPage.Click_SaveQuote(quoteType);

                // Starting EQuote Generation
                if (quoteType.Equals(eQuoteType))
                {
                    Console.WriteLine("EQuote Page Details Page Title is :-" + eQuoteDetailsPage.Return_Title());
                    eQuoteDetailsPage.Equote_Name_Setting(name);
                    eQuoteDetailsPage.Saved_By_Setting(email);
                    eQuoteDetailsPage.Click_Continue_Button();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    eQuoteSummaryPage.Click_Continue_Button();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    finalEquoteSummaryPage.Click_Save_Button();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));

                    quote_num = eQuoteGenerationPage.Return_Number();
                    price = eQuoteGenerationPage.Return_Price().Replace("$", "");
                    quote_num = "EQ:" + quote_num;
                    Console.WriteLine("Your Equote Number is :- " + quote_num);

                    Console.WriteLine("Price is :- " + price);
                }

                // Starting OrQuote Generation 
                if (quoteType.Equals(orType))
                {
                    b2BSecureCheckoutPage.Click_ExportOption();
                    b2BSecureCheckoutPage.Click_Continue_Button();
                    price = b2BTermsOfSalesPage.Find_Price().Replace("$", "");
                    b2BTermsOfSalesPage.Click_Submit_Button();
                    //String poQuoteText = b2BOrQuoteGenerationPage.Create_PO_Button_Text();
                    //Console.WriteLine("Po Button text is :-" + poQuoteText);
                    Console.WriteLine("Price is :- " + price);
                    b2BOrQuoteGenerationPage.Find_OrQuote();
                }
            }
            webDriver.Close();

            //quote_num ="EQ:1000017642306"; // preview
           // quote_num = "EQ:1000017643106"; // prod
            price = "1,668.99";

            // Generates PO cXml Template
            var orderId = orderIdBase + DateTime.Today.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss");
            var poXml = PoXmlGenerator.GeneratePoXml(
                format,
                profileId,
                deploymentMode,
                orderId,
                price,
                quote_num,
                endUserId);

            // Submits PO
            webDriver.SwitchTo().Window(parentWindow);
            b2bCatalogViewer.GoToHomePage();

            //string poNumber = "B2BAuto20141226031458"; //preview
            //string poNumber ="B2BAuto20141226050246"; // Prod
            string poNumber;

            b2bCatalogViewerPage.ClickQaTools3();
            if (!poOperations.SubmitXmlForPoCreation(poXml, env, poTargetUrl, out poNumber))
            {
                return false;
            }

            // verifies all validation after submiting PO
            webDriver.SwitchTo().Window(parentWindow);
            System.Threading.Thread.Sleep(1000);
            b2bCatalogViewer.GoToHomePage();
            Console.WriteLine(parentWindow);
            Console.WriteLine(webDriver.CurrentWindowHandle);
            if (!poOperations.AllOperations(poNumber, workflow, environment, crtFilePath, endUserId, gcmUrl, price))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
