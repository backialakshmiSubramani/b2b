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
        
            string profileName = UserName + DCSG.ADEPT.Framework.Data.Generator.RandomInt(0, 999);
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
