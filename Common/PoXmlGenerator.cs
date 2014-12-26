using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Modules.Channel.B2B.Core.Workflows.Common;
using Microsoft.SharePoint.Client;

namespace Modules.Channel.B2B.Common
{
    public static class PoXmlGenerator
    {
        public static string GeneratePoXml(PoXmlFormat poXmlFormat, string identityName, string deploymentMode, string orderId, string unitPrice, string supplierPartId, string b2BCrtEndUserId)
        {
            var fileName = poXmlFormat + "Template.xml";
            if (poXmlFormat == PoXmlFormat.Cxml)
            {
                return GeneratePoCxml(
                    fileName,
                    identityName,
                    deploymentMode,
                    orderId,
                    unitPrice,
                    supplierPartId,
                    b2BCrtEndUserId);
            }

            return GeneratePoCbl(
                fileName,
                identityName,
                orderId,
                unitPrice,
                supplierPartId,
                b2BCrtEndUserId);
        }

        private static string GeneratePoCbl(string fileName, string identityName, string orderId, string unitPrice, string supplierPartId, string b2BCrtEndUserId)
        {
            XDocument doc = XDocument.Load(fileName);
            doc.XPathSelectElement("//BuyerRefNum/Reference/RefNum").SetValue(orderId);
            doc.XPathSelectElement("//BuyerParty/Party/ListOfIdentifier/Identifier/Agency").Attribute("AgencyOther").SetValue(identityName);
            doc.XPathSelectElement("//SupplierPartNum/PartNum/PartID").SetValue(supplierPartId);
            doc.XPathSelectElement("//SupplierPartNum/PartNum/PartIDExt").SetValue(supplierPartId);
            doc.XPathSelectElement("//BuyerExpectedUnitPrice/Price/UnitPrice").SetValue(unitPrice);
            doc.XPathSelectElement("//Attachment/Purpose").SetValue(b2BCrtEndUserId);

            var inputXml = "<?xml version='1.0' encoding='utf-8'?>" + doc.ToString();
            return inputXml;
        }

        public static string GeneratePoCxml(string fileName, string identityName, string deploymentMode, string orderId, string unitPrice, string supplierPartId, string b2BCrtEndUserId)
        {
            XDocument doc = XDocument.Load(fileName);
            doc.XPathSelectElements("//Identity").First().SetValue(identityName);
            doc.Descendants("Request").First().Attribute("deploymentMode").SetValue(deploymentMode);
            doc.XPathSelectElement("//Request/OrderRequest/OrderRequestHeader")
                .Attribute("orderID")
                .SetValue(orderId);
            doc.XPathSelectElement("//Request/OrderRequest/ItemOut/ItemDetail/UnitPrice/Money").SetValue(unitPrice);
            doc.XPathSelectElement("//Request/OrderRequest/ItemOut/ItemID/SupplierPartID").SetValue(supplierPartId);
            doc.XPathSelectElement("//Request/OrderRequest/ItemOut/ItemID/SupplierPartAuxiliaryID")
                .SetValue(supplierPartId);
            doc.XPathSelectElement("//Request/OrderRequest/OrderRequestHeader/Extrinsic").SetValue(b2BCrtEndUserId);

            var inputXml = "<?xml version='1.0' encoding='utf-8'?>" + doc.ToString();
            return inputXml;
        }
    }
}
