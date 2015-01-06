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
        private PoOperations poOperations;

        public CreateQuote(IWebDriver Driver)
        {
            this.webDriver = Driver;
            poOperations = new PoOperations(webDriver);
        }

        /// <summary>
        /// Page object creation to use its methods
        /// </summary>
        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BQaToolsPage B2BQaToolsPage
        {
            get
            {
                return new B2BQaToolsPage(webDriver);
            }
        }

        private B2BPremierDashboardPage B2BPremierDashboardPage
        {
            get
            {
                return new B2BPremierDashboardPage(webDriver);
            }
        }

        private B2BStandardConfigurationPage B2BStandardConfigurationPage
        {
            get
            {
                return new B2BStandardConfigurationPage(webDriver);
            }
        }

        private B2BShopingCartPage ShopingCartPage
        {
            get
            {
                return new B2BShopingCartPage(webDriver);
            }
        }

        private B2BEQuoteDetailsPage eQuoteDetailsPage
        {
            get
            {
                return new B2BEQuoteDetailsPage(webDriver);
            }
        }

        private B2BEQuoteSummaryPage eQuoteSummaryPage
        {
            get
            {
                return new B2BEQuoteSummaryPage(webDriver);
            }
        }

        private B2BFinalEquoteSummaryPage finalEquoteSummaryPage
        {
            get
            {
                return new B2BFinalEquoteSummaryPage(webDriver);
            }
        }

        private B2BEQuoteGenerationPage eQuoteGenerationPage
        {
            get
            {
                return new B2BEQuoteGenerationPage(webDriver);
            }
        }

        private B2BSecureCheckoutPage B2BSecureCheckoutPage
        {
            get
            {
                return new B2BSecureCheckoutPage(webDriver);
            }
        }

        private B2BTermsOfSalesPage B2BTermsOfSalesPage
        {
            get
            {
                return new B2BTermsOfSalesPage(webDriver);
            }
        }

        private B2BOrQuoteGenerationPage B2BOrQuoteGenerationPage
        {
            get
            {
                return new B2BOrQuoteGenerationPage(webDriver);
            }
        }

        private B2BCatalogViewerPage B2BCatalogViewerPage
        {
            get
            {
                return new B2BCatalogViewerPage(webDriver);
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
            string quoteNumber = "0";
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
            B2BQaToolsPage.ClickCxml();
            B2BQaToolsPage.ClickCxmlMainCreate();
            B2BQaToolsPage.IdentityForProfileCorrelator(profileId);
            B2BQaToolsPage.IdentityForUserId(profileId);
            B2BQaToolsPage.ClickApplyParameter();
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
                B2BPremierDashboardPage.WaitForTitle();

                B2BPremierDashboardPage.OpenShop();
                B2BPremierDashboardPage.ClickStandardConfiguration();

                B2BStandardConfigurationPage.SelectFirstConfiguration();
                B2BStandardConfigurationPage.ClickAddSelectedToCartButton();

                Console.WriteLine("Shoping Cart Page Title is :- " + ShopingCartPage.ReturnShopingCartTitle());

                ShopingCartPage.ClickSaveQuote(quoteType);

                // Starting EQuote Generation
                if (quoteType.Equals(eQuoteType))
                {
                    Console.WriteLine("EQuote Page Details Page Title is :-" + eQuoteDetailsPage.ReturnTitle());
                    eQuoteDetailsPage.EquoteNameSetting(name);
                    eQuoteDetailsPage.SavedBySetting(email);
                    eQuoteDetailsPage.ClickContinueButton();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    eQuoteSummaryPage.ClickContinueButton();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    finalEquoteSummaryPage.ClickSaveButton();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));

                    quoteNumber = eQuoteGenerationPage.ReturnNumber();
                    price = eQuoteGenerationPage.ReturnPrice().Replace("$", "");
                    quoteNumber = "EQ:" + quoteNumber;
                    Console.WriteLine("Your Equote Number is :- " + quoteNumber);

                    Console.WriteLine("Price is :- " + price);
                }

                // Starting OrQuote Generation 
                if (quoteType.Equals(orType))
                {
                    B2BSecureCheckoutPage.ClickExportOption();
                    B2BSecureCheckoutPage.ClickContinueButton();
                    price = B2BTermsOfSalesPage.FindPrice().Replace("$", "");
                    B2BTermsOfSalesPage.ClickSubmitButton();
                    //String poQuoteText = b2BOrQuoteGenerationPage.Create_PO_Button_Text();
                    //Console.WriteLine("Po Button text is :-" + poQuoteText);
                    Console.WriteLine("Price is :- " + price);
                    B2BOrQuoteGenerationPage.FindOrQuote();
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
                quoteNumber,
                endUserId);

            // Submits PO
            webDriver.SwitchTo().Window(parentWindow);
            B2BCatalogViewerPage.GoToHomePage();

            //string poNumber = "B2BAuto20141226031458"; //preview
            //string poNumber ="B2BAuto20141226050246"; // Prod
            string poNumber;

            B2BCatalogViewerPage.ClickQaTools3();
            if (!poOperations.SubmitXmlForPoCreation(poXml, env, poTargetUrl, out poNumber))
            {
                return false;
            }

            // verifies all validation after submiting PO
            webDriver.SwitchTo().Window(parentWindow);
            System.Threading.Thread.Sleep(1000);
            B2BCatalogViewerPage.GoToHomePage();
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
