using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;

//Adept Framework 
using Dell.Adept.Core;
using Dell.Adept.UI;
using Dell.Adept.UI.Web;
using Dell.Adept.UI.Web.Pages;
using Dell.Adept.UI.Web.Support.Extensions.WebDriver;
using Dell.Adept.UI.Web.Support.Extensions.WebElement;
using Dell.Adept.UI.Web.Support.Locators;
using Dell.Adept.UI.Web.Support;
using Dell.Adept.Testing.DataGenerators.Primitive;

namespace Modules.Channel.B2B.Core.Workflows.ASN
{
    public class CreateProfile
    {
        private IWebDriver webDriver;

        private B2BHomePage b2bHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        private B2BCustomerProfileListPage b2bCustomerProfileListpage
        {
            get
            {
                return new B2BCustomerProfileListPage(webDriver);
            }
        }

        private B2BProfileSettingsGeneralPage profileSettingGeneralPage
        {
            get
            {
                return new B2BProfileSettingsGeneralPage(webDriver);
            }
        }
        public CreateProfile(IWebDriver Driver)
        {
            webDriver = Driver;
        }
        public string CreateNewProfile(string UserName, string CustomerSet, string AccessGroup) 
        {
        
            string profileName = UserName + Generator.RandomInt(0, 999);
            Console.WriteLine(profileName);
            b2bHomePage.ClickB2BProfileList();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            b2bCustomerProfileListpage.ClickCreateNewProfile();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            profileSettingGeneralPage.EnterUserId(profileName);
            profileSettingGeneralPage.EnterCustomerName(profileName);
            profileSettingGeneralPage.EnterIdentityName(profileName);
            profileSettingGeneralPage.EnterCustomerSet(CustomerSet);
            profileSettingGeneralPage.ClickSearch();
            webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            
            if (profileSettingGeneralPage.SelectAccessGroupMsgDisplayed())
            {
                profileSettingGeneralPage.EnterAccessGroup(AccessGroup);
                profileSettingGeneralPage.ClickCreateNewProfile();
                webDriver.WaitForPageLoad(TimeSpan.FromSeconds(5));
            }

            else
            {
                throw new ElementNotVisibleException();
            }

            return profileName;
        }
    }
}
