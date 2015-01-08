// ***********************************************************************
// Author           : AMERICAS\Naveen_Kumar31
// Created          : 12/30/2014 12:13:51 PM
//
// Last Modified By : AMERICAS\Naveen_Kumar31
// Last Modified On : 12/30/2014 4:13:51 PM
// ***********************************************************************
// <copyright file="CrtAssociation.cs" company="Dell">
//     Copyright (c) Dell 2014. All rights reserved.
// </copyright>
// <summary>Contains CrtAssociation workflow methods.</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Modules.Channel.B2B.Core.Pages;
using Microsoft.SharePoint.Client;
using Modules.Channel.B2B.Core.Workflows.Common;
using FluentAssertions;

namespace Modules.Channel.B2B.Core.Workflows.Common
{
    public class CrtAssociation
    {
        private IWebDriver webDriver;
        public CrtAssociation(IWebDriver driver)
        {
            this.webDriver = driver;
        }

        #region Page objects
        /// <summary>
        /// B2BHome Page instance
        /// </summary>
        private B2BHomePage B2BHomePage
        {
            get
            {
                return new B2BHomePage(webDriver);
            }
        }

        /// <summary>
        /// B2BCrossReferenceList Page instance
        /// </summary>
        private B2BCrossReferenceListPage B2BCrossReferenceListPage
        {
            get
            {
                return new B2BCrossReferenceListPage(webDriver);
            }
        }

        /// <summary>
        /// B2BCrossReferenceMaintenance Page Instance
        /// </summary>
        private B2BCrossReferenceMaintenancePage B2BCrossReferenceMaintenencePage
        {
            get
            {
                return new B2BCrossReferenceMaintenancePage(webDriver);
            }
        }

        /// <summary>
        /// B2BCustomerProfileList Page instance
        /// </summary>
        private B2BCustomerProfileListPage B2BCustomerProfileListPage
        {
            get
            {
                return new B2BCustomerProfileListPage(webDriver);
            }
        }

        /// <summary>
        /// B2BManageProfileIdentities Page instance 
        /// </summary>
        private B2BManageProfileIdentitiesPage B2BManageProfileIdentitiesPage
        {
            get
            {
                return new B2BManageProfileIdentitiesPage(webDriver);
            }
        }
        
        /// <summary>
        /// B2BCrossReferenceAssociation Page instance
        /// </summary>
        private B2BCrossReferenceAssociationPage B2BCrossReferenceAssociationPage
        {
            get
            {
                return new B2BCrossReferenceAssociationPage(webDriver);
            }
        }

        /// <summary>
        /// CRT Upload instance
        /// </summary>
        private CrtUpload crtUpload
        {
            get
            {
                return new CrtUpload(webDriver);
            }
        }
        #endregion

        #region workflow method
        /// <summary>
        /// Associate CRT
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="crossReferenceType"></param>
        /// <param name="filePath"></param>
        /// <param name="description"></param>
        /// <param name="profilename"></param>
        public void AssociateCrt(
           RunEnvironment environment,
           string crossReferenceType,
           string filePath,
           string description,
            string profilename)
        {
            if(environment.Equals(RunEnvironment.Preview))
                //Upload CRT file
                crtUpload.UploadCrtFile(environment, crossReferenceType, filePath, description);
                //Click B2B Profile List
                B2BHomePage.ClickB2BProfileList();
                //Search given profile and associate
                B2BCustomerProfileListPage.SearchProfile(null, profilename);
                B2BCustomerProfileListPage.ClickSearchedProfile();
                //Associate CR
                B2BManageProfileIdentitiesPage.CRAssociationLink.Click();
                B2BManageProfileIdentitiesPage.AssociateCrossReferenceLink.Click();
                B2BCrossReferenceAssociationPage.FilterCRT(crossReferenceType);
                B2BCrossReferenceAssociationPage.CrtTableCheckBox.Click();
                B2BCrossReferenceAssociationPage.AssociateLink.Click();
        }


        #endregion
    }
}
