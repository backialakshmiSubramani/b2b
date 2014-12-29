using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Microsoft.SharePoint.Client;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class CrtUpload
    {
        private IWebDriver webDriver;
        public CrtUpload(IWebDriver driver)
        {
            this.webDriver = driver;
        }

        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BCrossReferenceListPage B2BCrossReferenceListPage
        {
            get
            {
                return new B2BCrossReferenceListPage(webDriver);
            }
        }

        private B2BCrossReferenceMaintenancePage B2BCrossReferenceMaintenencePage
        {
            get
            {
                return new B2BCrossReferenceMaintenancePage(webDriver);
            }
        }

        public void UploadCrtFile(
            RunEnvironment environment,
            string crossReferenceType,
            string filePath,
            string description)
        {
            B2BHomePage.SelectEnvironment(environment.ToString());
            B2BHomePage.ClickCrossReferenceListLink();
            B2BCrossReferenceListPage.ClickNewCrossReference();
            ////selects CRT Type
            B2BCrossReferenceMaintenencePage.SelectCrossReferenceType(crossReferenceType);
            ////Sets the path for File Upload
            B2BCrossReferenceMaintenencePage.SetUploadFilePath(filePath);
            B2BCrossReferenceMaintenencePage.SetDescription(description);
            B2BCrossReferenceMaintenencePage.ClickSave();
        }

        public bool IsErrorMessageDisplayed()
        {
            return B2BCrossReferenceMaintenencePage.IsErrorMessageDisplayed();

        }

        public bool IsSuccessfulMessageDisplayed()
        {
            return B2BCrossReferenceMaintenencePage.IsSuccessfulMessageDisplayed();
        }
    }
}
