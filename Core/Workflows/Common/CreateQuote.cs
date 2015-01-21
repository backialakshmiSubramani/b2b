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

        /* How to create 'E-Quote' or 'OrQuote'
         * Call CompleteEQuoteGeneration() with 5 arguments -
         * i) quoteType(equote/OrQuote)     <-- Note pass only "equote" for E-Quote and "OrQuote" for OR-Quote
         * ii) environment - "Preview" or "Prod"
         * iii) profileId - In String 
         * iv) name - any name      <- will be used in equote
         * v) email - any email     <- will be used in equote        
         */

        public bool CompleteQuoteGeneration(
             string profileId,
             string name,
             string email,
             RunEnvironment environment,
             Workflow workflow,
             PoXmlFormat format,
             string deploymentMode,
             string orderIdBase,
             string poTargetUrl,
             string configFilterVal,
            List<QuoteDetail> listOfQuoteDetail,
             out string poNumber,
             out List<QuoteDetail> quoteDetail)
        {
            string responseCode = "0";

            var parentWindow = webDriver.CurrentWindowHandle;
            B2BHomePage.SelectEnvironment(environment.ToString());
            B2BHomePage.ClickQaTools3();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
            webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
            var QaToolsWindow = webDriver.CurrentWindowHandle;
            B2BQaToolsPage.ClickLocationEnvironment(environment.ToString());
            B2BQaToolsPage.ClickLocationEnvironmentLink(environment.ToString());
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
                string temp = B2BQaToolsPage.GetStoreLinkText();
                Console.WriteLine("Link value is " + temp);
                B2BQaToolsPage.ClickStoreLink();
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                webDriver.Close();
                string newwindow = webDriver.WindowHandles.LastOrDefault();
                webDriver.SwitchTo().Window(newwindow);
                //webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
                var premierWindow = webDriver.CurrentWindowHandle;

                webDriver.Manage().Window.Maximize();
                B2BPremierDashboardPage.WaitForTitle();
                B2BPremierDashboardPage.OpenShop();
                B2BPremierDashboardPage.ClickStandardConfiguration();
                if (workflow == Workflow.Asn)
                {
                    B2BStandardConfigurationPage.SelectSpecificConfiguration(configFilterVal, listOfQuoteDetail.FirstOrDefault().ItemDescription);
                }
                else
                {
                    B2BStandardConfigurationPage.SelectFirstConfiguration();
                }
                webDriver.WaitForPageLoad(new TimeSpan(0, 0, 20));
                B2BStandardConfigurationPage.ClickAddSelectedToCartButton();
                Console.WriteLine("Shoping Cart Page Title is :- " + ShopingCartPage.ReturnShopingCartTitle());
                ShopingCartPage.ClickSaveQuote(listOfQuoteDetail.FirstOrDefault().QuoteType);

                // Starting EQuote Generation
                if (listOfQuoteDetail.FirstOrDefault().QuoteType == QuoteType.EQuote)
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
                    listOfQuoteDetail.FirstOrDefault().SupplierPartId = "EQ:" + eQuoteGenerationPage.ReturnNumber();
                    listOfQuoteDetail.FirstOrDefault().Price = eQuoteGenerationPage.ReturnPrice().Replace("$", "");
                    Console.WriteLine("Your Equote Number is :- " + listOfQuoteDetail.FirstOrDefault().SupplierPartId);
                    Console.WriteLine("Price is :- " + listOfQuoteDetail.FirstOrDefault().Price);
                }

                    // Starting OrQuote Generation 
                else if (listOfQuoteDetail.FirstOrDefault().QuoteType == QuoteType.OrQuote)
                {
                    B2BSecureCheckoutPage.EnterContactAndBillingInfo();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    listOfQuoteDetail.FirstOrDefault().Price = B2BTermsOfSalesPage.FindPrice().Replace("$", "");
                    B2BTermsOfSalesPage.ClickSubmitButton();
                    webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
                    Console.WriteLine("Price is :- " + listOfQuoteDetail.FirstOrDefault().Price);
                    listOfQuoteDetail.FirstOrDefault().SupplierPartId = B2BOrQuoteGenerationPage.FindOrQuote();
                }

                else
                {
                    Console.WriteLine("Quote Type is not Specified");
                    poNumber = string.Empty;
                    quoteDetail = listOfQuoteDetail;
                    return false;
                }
            }


            // Generates PO Template
            var orderId = orderIdBase + DateTime.Today.ToString("yyMMdd") + DateTime.Now.ToString("HHmmss");
            string poXml;
            if (workflow == Workflow.Eudc)
            {
                poXml = PoXmlGenerator.GeneratePoCxmlCblForEudc(
                    format,
                    profileId,
                    deploymentMode,
                    orderId,
                    listOfQuoteDetail.FirstOrDefault().Price,
                    listOfQuoteDetail.FirstOrDefault().SupplierPartId,
                    listOfQuoteDetail.FirstOrDefault().CrtId);
            }
            else
            {
                poXml = PoXmlGenerator.GeneratePoCblForAsn(format, orderId, profileId, listOfQuoteDetail);
            }

            // Submits PO
            webDriver.SwitchTo().Window(parentWindow);
            B2BHomePage.ClickQaTools3();
            if (!poOperations.SubmitXmlForPoCreation(poXml, environment.ToString(), poTargetUrl, out poNumber))
            {
                quoteDetail = listOfQuoteDetail;
                return false;
            }

            // verifies all validation after submiting PO
            webDriver.SwitchTo().Window(parentWindow);
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(40));
            quoteDetail = listOfQuoteDetail;
            return true;
        }
    }
}
