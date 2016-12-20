using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Modules.Channel.B2B.InventoryXMLTemplates
{
    [Serializable, XmlRoot("B2BXML")]
    public partial class B2BXML
    {
        [XmlElement("Inventory")]
        public Inventory Inventory
        { get; set; }
    }

    [Serializable]
    public partial class Inventory
    {
        [XmlElement("InventoryHeader")]
        public InventoryHeader InventoryHeader
        { get; set; }

        [XmlElement("InventoryDetails")]
        public InventoryDetails InventoryDetails
        { get; set; }
    }
    [Serializable]
    public partial class InventoryHeader
    {
        [XmlElement("MessageType")]
        public string MessageType
        { get; set; }

        [XmlElement("Buyer")]
        public string Buyer
        { get; set; }

        [XmlElement("CatalogType")]
        public string CatalogType
        { get; set; }

        [XmlElement("CatalogName")]
        public string CatalogName
        { get; set; }
    }
    [Serializable]
    public partial class InventoryDetails
    {
        [XmlElement("Item")]
        public Item[] Item
        { get; set; }
    }
    [Serializable]
    public partial class Item
    {
        [XmlElement("ManufacturerPartNumber")]
        public string ManufacturerPartNumber { get; set; }

        [XmlElement("ManufacturerName")]
        public string ManufacturerName { get; set; }

        [XmlElement("ItemDescription")]
        public string ItemDescription { get; set; }

        [XmlElement("ListPrice")]
        public decimal ListPrice { get; set; }

        [XmlElement("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [XmlElement("UOM")]
        public string UOM { get; set; }

        [XmlElement("GrossWeight")]
        public string GrossWeight { get; set; }

        [XmlElement("PackageLength")]
        public int PackageLength { get; set; }

        [XmlElement("PackageWidth")]
        public int PackageWidth { get; set; }

        [XmlElement("PackageHeight")]
        public int PackageHeight { get; set; }

        [XmlElement("Location")]
        public string Location { get; set; }

        [XmlElement("LeadTime")]
        public string LeadTime { get; set; }

        [XmlElement("InventoryQty")]
        public string InventoryQty { get; set; }

        [XmlElement("RestockDate")]
        public string RestockDate { get; set; }

        [XmlElement("RestockQuantity")]
        public string RestockQuantity { get; set; }

        [XmlElement("CategoryLevel1")]
        public string CategoryLevel1 { get; set; }

        [XmlElement("BuyerPartNumber")]
        public string BuyerPartNumber { get; set; }

        [XmlElement("ProductAction")]
        public string ProductAction { get; set; }

        [XmlElement("TransactionSet")]
        public string TransactionSet { get; set; }

        [XmlElement("UPC")]
        public string UPC { get; set; }

        [XmlElement("ExpirationDate")]
        public string ExpirationDate { get; set; }

        [XmlElement("SKU")]
        public string SKU { get; set; }
    }
}
