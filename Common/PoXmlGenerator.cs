using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Modules.Channel.B2B.Common
{
    public static class PoXmlGenerator
    {
        public static IEnumerable<string> GeneratorPoCXml(string fileName, string identityName, string deploymentMode, string orderId, string unitPrice, string supplierPartId, string customerId)
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
            doc.XPathSelectElement("//Request/OrderRequest/OrderRequestHeader/Extrinsic").SetValue(customerId);

            var inputXml = "<?xml version='1.0' encoding='utf-8'?>" + doc.ToString();
            if (inputXml.Length > 2000)
            {
                return inputXml.SplitByLength(2000);
            }

            return new[] { inputXml };
        }

        public static IEnumerable<string> SplitByLength(this string stringToBeSplit, int maximunLength)
        {
            for (int index = 0; index < stringToBeSplit.Length; index += maximunLength)
            {
                yield return stringToBeSplit.Substring(index, Math.Min(maximunLength, stringToBeSplit.Length - index));
            }
        }
    }

    //public enum Quote
    //{
    //    Bhc,
    //    eQuote,
    //    OrQuote,
    //    Cif,
    //    Doms
    //}
}
