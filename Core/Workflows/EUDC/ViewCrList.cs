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


namespace Modules.Channel.B2B.Core.Workflows.EUDC
{
    public class ViewCrList
    {
        private readonly IWebDriver _webDriver;

        private B2BHomePage B2BhomePage
        {
            get { return new B2BHomePage(_webDriver); }
        }

        private B2BCrossReferenceListPage B2BCrossReferenceListPage
        {
            get { return new B2BCrossReferenceListPage(_webDriver); }
        }

        public ViewCrList(IWebDriver driver)
        {
            _webDriver = driver;
        }

        public void OpenCrossReferenceList()
        {
            B2BhomePage.ClickCrossReferenceListLink();
        }

        public string OpenCrAssociationList()
        {
            B2BhomePage.ClickCrAssociationList();
            return _webDriver.Url;
        }

        public void DispalyCrList(string crossReferenceType)
        {
            B2BCrossReferenceListPage.SelectCrTypeList(crossReferenceType);
            B2BCrossReferenceListPage.ClickViewCrList();
            _webDriver.WaitForPageLoad(TimeSpan.FromSeconds(20));
        }

        public bool CheckTypeColumnValue(string columnText)
        {
           return B2BCrossReferenceListPage.GetTypeColumnValues().All(e => e.Text.Contains(columnText));
        }
    }
}
