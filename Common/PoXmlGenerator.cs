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
        /// <summary>
        /// Use this method to generate the PO CBL for B2B ASN
        /// </summary>
        /// <param name="poXmlFormat"></param>
        /// <param name="orderId"></param>
        /// <param name="identityName"></param>
        /// <param name="quoteDetails"></param>
        /// <returns></returns>
        public static string GeneratePoCblForAsn(
            PoXmlFormat poXmlFormat,
            string orderId,
            string identityName,
            IList<QuoteDetail> quoteDetails)
        {
            var fileName = poXmlFormat + "Template.xml";
            var doc = XDocument.Load(fileName);
            doc.XPathSelectElement("//BuyerRefNum/Reference/RefNum").SetValue(orderId);
            doc.XPathSelectElement("//BuyerParty/Party/ListOfIdentifier/Identifier/Agency")
                .Attribute("AgencyOther")
                .SetValue(identityName);

            var orderDetailNode = doc.XPathSelectElement("//ListOfOrderDetail/OrderDetail");
            var newDoc = new XDocument(orderDetailNode);
            newDoc.Save("OrderDetailNode.xml");
            orderDetailNode.Remove();

            for (var i = quoteDetails.Count() - 1; i > -1; i--)
            {
                if (string.IsNullOrEmpty(quoteDetails[i].SupplierPartId) || string.IsNullOrEmpty(quoteDetails[i].CrtId)
                    || string.IsNullOrEmpty(quoteDetails[i].Quantity) || string.IsNullOrEmpty(quoteDetails[i].Price))
                {
                    continue;
                }

                var orderDetailDoc = XDocument.Load("OrderDetailNode.xml");
                orderDetailDoc.XPathSelectElement("//BaseItemDetail/LineItemNum").SetValue((i + 1).ToString("D2"));
                switch (quoteDetails[i].QuoteType)
                {
                    case QuoteType.EQuote:
                    case QuoteType.Doms:
                        orderDetailDoc.XPathSelectElement("//SupplierPartNum/PartNum/PartID")
                            .SetValue(quoteDetails[i].SupplierPartId);
                        break;
                    case QuoteType.OrQuote:
                    case QuoteType.Bhc:
                        orderDetailDoc.XPathSelectElement("//SupplierPartNum/PartNum/PartIDExt")
                            .SetValue(quoteDetails[i].SupplierPartId);
                        break;
                    case QuoteType.Cif:
                        orderDetailDoc.XPathSelectElement("//SupplierPartNum/PartNum/PartID")
                            .SetValue(quoteDetails[i].SupplierPartId);
                        orderDetailDoc.XPathSelectElement("//SupplierPartNum/PartNum/PartIDExt")
                            .SetValue(quoteDetails[i].SupplierPartId);
                        break;
                    default:
                        throw new ArgumentException("Quote type not specified");
                }

                orderDetailDoc.XPathSelectElement("//BuyerPartNum/PartNum/PartID").SetValue(quoteDetails[i].CrtId);
                orderDetailDoc.XPathSelectElement("//ManufacturerPartNum/PartNum/PartID").SetValue(quoteDetails[i].CrtId);

                orderDetailDoc.XPathSelectElement("//BaseItemDetail/Quantity/Qty").SetValue(quoteDetails[i].Quantity);
                orderDetailDoc.XPathSelectElement("//BuyerExpectedUnitPrice/Price/UnitPrice").SetValue(quoteDetails[i].Price);

                doc.XPathSelectElement("//ListOfOrderDetail").AddFirst(orderDetailDoc.XPathSelectElement("//OrderDetail"));
            }

            doc.Save(fileName);
            var inputXml = "<?xml version='1.0' encoding='utf-8'?>" + doc.ToString();
            return inputXml;
        }

        public static string GeneratePoCxmlCblForEudc(PoXmlFormat poXmlFormat, string identityName, string deploymentMode, string orderId, string unitPrice, string supplierPartId, string b2BCrtEndUserId)
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

        private static string GeneratePoCxml(string fileName, string identityName, string deploymentMode, string orderId, string unitPrice, string supplierPartId, string b2BCrtEndUserId)
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

    public class QuoteDetail
    {
        public QuoteType QuoteType { get; set; }
        public string SupplierPartId { get; set; }
        public string CrtId { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string ItemDescription { get; set; }
    }
}
