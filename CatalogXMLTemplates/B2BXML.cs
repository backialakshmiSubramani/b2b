using Modules.Channel.B2B.Common;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Modules.Channel.B2B.CatalogXMLTemplates
{

    [Serializable, XmlRoot("B2BXML")]
    public partial class B2BXML
    {
        [XmlElement("BuyerCatalog")]
        public BuyerCatalog BuyerCatalog
        { get; set; }
    }

    [Serializable]
    public partial class BuyerCatalog
    {
        [XmlElement("CatalogHeader")]
        public CatalogHeader CatalogHeader
        { get; set; }

        [XmlElement("CatalogDetails")]
        public CatalogDetails CatalogDetails
        { get; set; }
    }

    [Serializable]
    public partial class CatalogHeader
    {
        [XmlElement("CatalogFormatType")]
        public string CatalogFormatType
        { get; set; }

        [XmlElement("CatalogName")]
        public string CatalogName
        { get; set; }

        [XmlElement("EffectiveDate")]
        public string EffectiveDate
        { get; set; }

        [XmlElement("ExpirationDate")]
        public string ExpirationDate
        { get; set; }

        [XmlElement("CountryCode")]
        public string CountryCode
        { get; set; }

        [XmlElement("SubLocationCode")]
        public string SubLocationCode
        { get; set; }

        [XmlElement("Buyer")]
        public string Buyer
        { get; set; }

        [XmlElement("RequesterEmailId")]
        public string RequesterEmailId
        { get; set; }

        [XmlElement("CatalogId")]
        public int CatalogId
        { get; set; }

        [XmlElement("ItemCount")]
        public int ItemCount
        { get; set; }

        [XmlElement("SupplierId")]
        public string SupplierId
        { get; set; }

        [XmlElement("Comments")]
        public string Comments
        { get; set; }

        [XmlElement("SNPEnabled")]
        public bool SNPEnabled
        { get; set; }

        [XmlElement("SYSConfigEnabled")]
        public bool SYSConfigEnabled
        { get; set; }

        [XmlElement("StdConfigEnabled")]
        public bool StdConfigEnabled
        { get; set; }

        [XmlElement("StdConfigUpSellDownSellEnabled")]
        public bool StdConfigUpSellDownSellEnabled
        { get; set; }

        [XmlElement("Region")]
        public string Region
        { get; set; }

        [XmlElement("GPEnabled")]
        public bool GPEnabled
        { get; set; }

        [XmlElement("GPShipToCurrency")]
        public string GPShipToCurrency
        { get; set; }

        [XmlElement("GPShipToCountry")]
        public string GPShipToCountry
        { get; set; }

        [XmlElement("GPShipToLanguage")]
        public string GPShipToLanguage
        { get; set; }

        [XmlElement("GPPurchaseOption")]
        public string GPPurchaseOption
        { get; set; }

        [XmlElement("CPFEnabled")]
        public bool CPFEnabled
        { get; set; }

        [XmlElement("IdentityUserName")]
        public string IdentityUserName
        { get; set; }

        [XmlElement("GracePeriod")]
        public int GracePeriod
        { get; set; }

        [XmlElement("ProfileId")]
        public int ProfileId
        { get; set; }

        [XmlElement("CustomerID")]
        public string CustomerID
        { get; set; }

        [XmlElement("AccessGroup")]
        public string AccessGroup
        { get; set; }

        [XmlElement("MessageType")]
        public string MessageType
        { get; set; }

        [XmlElement("CatalogType")]
        public string CatalogType
        { get; set; }

        [XmlElement("Sender")]
        public string Sender
        { get; set; }

        [XmlElement("Receiver")]
        public string Receiver
        { get; set; }

        [XmlElement("CatalogDate")]
        public string CatalogDate
        { get; set; }
    }

    [Serializable]
    public partial class CatalogDetails
    {
        [XmlElement("CatalogItem")]
        public CatalogItem[] CatalogItem
        { get; set; }
    }

    [Serializable]
    public partial class CatalogItem
    {
        [XmlAttribute("AutoCategory")]
        public string AutoCategory;

        [XmlElement("CatalogItemType")]
        public CatalogItemType CatalogItemType;

        [XmlElement("PrimaryCurrency")]
        public Currency PrimaryCurrency;

        [XmlElement("AlternateCurrency")]
        public Currency AlternateCurrency;

        [XmlElement("Modules")]
        public CatalogItemModules Modules;

        [XmlElement("Lease")]
        public CatalogItemLease Lease;

        [XmlElement("ShortName")]
        public string ShortName { get; set; }

        [XmlElement("ItemDescription")]
        public string ItemDescription { get; set; }

        [XmlElement("UNSPSC")]
        public string UNSPSC { get; set; }

        [XmlElement("UOM")]
        public string UOM { get; set; }

        [XmlElement("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [XmlElement("PartId")]
        public string PartId { get; set; }

        [XmlElement("SuplierPartAuxilaryId")]
        public string SuplierPartAuxilaryId { get; set; }

        [XmlElement("LeadTime")]
        public int LeadTime { get; set; }

        [XmlElement("SupplierURL")]
        public string SupplierURL { get; set; }

        [XmlElement("ImageURL")]
        public string ImageURL { get; set; }

        [XmlElement("ManufacturerPartNumber")]
        public string ManufacturerPartNumber { get; set; }

        [XmlElement("ManufacturerName")]
        public string ManufacturerName { get; set; }

        [XmlElement("LongDescription")]
        public string LongDescription { get; set; }

        [XmlElement("GrossWeight")]
        public string GrossWeight { get; set; }

        [XmlElement("Availability")]
        public string Availability { get; set; }

        [XmlElement("CategoryLevel1")]
        public string CategoryLevel1 { get; set; }

        [XmlElement("CategoryLevel2")]
        public string CategoryLevel2 { get; set; }

        [XmlElement("DomsQuote")]
        public string DomsQuote { get; set; }

        [XmlElement("ItemOrderCode")]
        public string ItemOrderCode { get; set; }

        [XmlElement("QuoteId")]
        public string QuoteId { get; set; }

        [XmlElement("BaseSKUId")]
        public string BaseSKUId { get; set; }

        [XmlElement("FGASKUId")]
        public string FGASKUId { get; set; }

        [XmlElement("ReplacementQuoteId")]
        public string ReplacementQuoteId { get; set; }

        [XmlElement("ItemType")]
        public string ItemType { get; set; }

        [XmlElement("ItemSKUinfo")]
        public string ItemSKUinfo { get; set; }

        [XmlElement("FGAModNumber")]
        public string FGAModNumber { get; set; }

        [XmlElement("InventoryQty")]
        public int InventoryQty { get; set; }

        [XmlElement("ListPrice")]
        public string ListPrice { get; set; }

        [XmlElement("UPC")]
        public string UPC { get; set; }

        [XmlElement("ManufacturerCode")]
        public string ManufacturerCode { get; set; }

        [XmlElement("VPNReplacement")]
        public string VPNReplacement { get; set; }

        [XmlElement("VPNEOLDate")]
        public string VPNEOLDate { get; set; }

        [XmlElement("DeltaComments")]
        public DeltaComments DeltaComments = new DeltaComments();

        [XmlElement("DeltaChange")]
        public DeltaStatus DeltaChange { get; set; }

        [XmlElement("PackageLength")]
        public string PackageLength { get; set; }

        [XmlElement("PackageWidth")]
        public string PackageWidth { get; set; }

        [XmlElement("PackageHeight")]
        public string PackageHeight { get; set; }

        [XmlElement("PalletLength")]
        public string PalletLength { get; set; }

        [XmlElement("PalletWidth")]
        public string PalletWidth { get; set; }

        [XmlElement("PalletHeight")]
        public string PalletHeight { get; set; }

        [XmlElement("PalletUnitsPerLayer")]
        public string PalletUnitsPerLayer { get; set; }

        [XmlElement("PalletLayerPerPallet")]
        public string PalletLayerPerPallet { get; set; }

        [XmlElement("PalletUnitsPerPallet")]
        public string PalletUnitsPerPallet { get; set; }

        //For SPL additional Nodes
        [XmlElement("AssociatedSNPSkus")]
        public string AssociatedSNPSkus { get; set; }

        [XmlElement("AssociatedServiceSkus")]
        public string AssociatedServiceSkus { get; set; }

        [XmlElement("BoxDimensions")]
        public string BoxDimensions { get; set; }

        [XmlElement("MinimumOrderQty")]
        public string MinimumOrderQty { get; set; }

        [XmlElement("PackSeparate")]
        public string PackSeparate { get; set; }

        [XmlElement("CasePackQuantity")]
        public string CasePackQuantity { get; set; }

        [XmlElement("MCQ")]
        public string MCQ { get; set; }

        [XmlElement("CustomsCode")]
        public string CustomsCode { get; set; }

        [XmlElement("ShippingInstructions")]
        public string ShippingInstructions { get; set; }

        [XmlElement("DropShip")]
        public string DropShip { get; set; }

        [XmlElement("CountryofOrigin")]
        public string CountryofOrigin { get; set; }

        [XmlElement("EPEAT")]
        public string EPEAT { get; set; }

        [XmlElement("ECCN")]
        public string ECCN { get; set; }

        [XmlElement("UHG")]
        public string UHG { get; set; }

        [XmlElement("Levy")]
        public string Levy { get; set; }

        [XmlElement("SerialScan")]
        public string SerialScan { get; set; }

        [XmlElement("Comments")]
        public string Comments { get; set; }

        [XmlElement("Promo")]
        public string Promo { get; set; }

        [XmlElement("CountryCode")]
        public string CountryCode { get; set; }

        [XmlElement("Model")]
        public string Model { get; set; }

        [XmlElement("EAN")]
        public string EAN { get; set; }

        [XmlElement("JAN")]
        public string JAN { get; set; }

        [XmlElement("LineofBusiness")]
        public string LineofBusiness { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("ProductCategory")]
        public string ProductCategory { get; set; }

        [XmlElement("ProductSubCategory")]
        public string ProductSubCategory { get; set; }

        [XmlElement("ShortDescription")]
        public string ShortDescription { get; set; }

        [XmlIgnore]
        public string ImageList;

        //[XmlElement("Images")]
        //public string Images { get; set; }
        //public string Images
        //{
        //    get { return new XmlDocument().CreateCDataSection(ImageList).Value; }
        //    set { ImageList = (value != null) ? value.ToString() : null; }
        //}

        [XmlElement("Aio")]
        public string Aio { get; set; }

        [XmlElement("EStar")]
        public string EStar { get; set; }

        [XmlElement("MfgPartNumber")]
        public string MfgPartNumber { get; set; }

        [XmlElement("ResellerPrice")]
        public string ResellerPrice { get; set; }

        [XmlElement("ResellerDeltaPrice")]
        public string ResellerDeltaPrice { get; set; }

        [XmlIgnore]
        public string TechSpecList;

        //[XmlElement("TechSpec")]
        //public string TechSpec { get; set; }

        //public string TechSpec
        //{
        //    //get { return new XmlDocument().CreateCDataSection(TechSpecList); }
        //    //set { TechSpecList = (value != null) ? value.Data : null; }


        //    get { return new XmlDocument().CreateCDataSection(TechSpecList).Value; }
        //    set { TechSpecList = (value != null) ? value.ToString() : null; }
        //}

        //SPL End


    }

    //[Serializable]
    //public enum CatalogGenerationType
    //{
    //    ConfigWithDefaultOptions = 0,
    //    ConfigWithUpsellDownsell = 1,
    //    SNP = 2,
    //    Systems = 3,
    //}

    [Serializable]
    public class Currency
    {
        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [XmlAttribute("HedgeRate")]
        public string HedgeRate { get; set; }
    }

    [Serializable]
    public class CatalogItemLease
    {
        [XmlElement("Disposition")]
        public string Disposition { get; set; }

        [XmlElement("ExpensedTotal")]
        public decimal ExpensedTotal { get; set; }

        [XmlElement("Frequency")]
        public string Frequency { get; set; }

        [XmlElement("LeaseTerms")]
        public string LeaseTerms { get; set; }

        [XmlElement("LRF")]
        public decimal LRF { get; set; }
    }

    [Serializable]
    public class CatalogItemModules
    {
        [XmlElement("Module")]
        public List<CatalogItemModule> Module;
    }

    [Serializable]
    public class CatalogItemModule
    {
        [XmlElement("Options")]
        public CatalogItemOptions Options;

        [XmlElement("DefaultOptionId")]
        public string DefaultOptionId { get; set; }

        [XmlElement("DefaultOptionPrice")]
        public decimal DefaultOptionPrice { get; set; }

        [XmlElement("IsHiddenOptionExists")]
        public string IsHiddenOptionExists { get; set; }

        [XmlElement("IsMultiQtyOptionExists")]
        public string IsMultiQtyOptionExists { get; set; }

        [XmlElement("IsRequiredOptionExists")]
        public string IsRequiredOptionExists { get; set; }

        [XmlElement("ModuleDesc")]
        public string ModuleDesc { get; set; }

        [XmlElement("ModuleId")]
        public int ModuleId { get; set; }

        [XmlElement("MultiSelect")]
        public bool MultiSelect { get; set; }

        [XmlElement("Required")]
        public bool Required { get; set; }
    }

    [Serializable]
    public class CatalogItemOptions
    {
        [XmlElement("Option")]
        public List<CatalogItemOption> Option;
    }

    [Serializable]
    public class CatalogItemOption
    {
        [XmlElement("MultiQtyDets")]
        public MultiQtyDets MultiQtyDets;

        [XmlElement("OptionSkuList")]
        public OptionSkuList OptionSkuList;

        [XmlElement("DeltaPrice")]
        public decimal DeltaPrice { get; set; }

        [XmlElement("FinalPrice")]
        public decimal FinalPrice { get; set; }

        [XmlElement("IsMultiQtyOption")]
        public string IsMultiQtyOption { get; set; }

        [XmlElement("OptionDesc")]
        public string OptionDesc { get; set; }

        [XmlElement("OptionId")]
        public string OptionId { get; set; }

        [XmlAttribute("Type")]
        public string OptionType { get; set; }

        [XmlElement("Selected")]
        public bool Selected { get; set; }
    }

    [Serializable]
    public class MultiQtyDets
    {
        [XmlElement("OptionValue")]
        public decimal OptionValue { get; set; }

        [XmlElement("Quantity")]
        public int Quantity { get; set; }

        [XmlElement("UnitPrice")]
        public decimal UnitPrice { get; set; }
    }

    [Serializable]
    public class OptionSkuList
    {
        [XmlElement("OptionSku")]
        public List<OptionSku> OptionSku;
    }

    [Serializable]
    public class OptionSku
    {
        [XmlElement("OptionId")]
        public string OptionId { get; set; }

        [XmlElement("SkuDescription")]
        public string SkuDescription { get; set; }

        [XmlElement("SkuId")]
        public string SkuId { get; set; }

        [XmlElement("SkuPrice")]
        public decimal SkuPrice { get; set; }
    }

    [Serializable]
    public enum DeltaStatus
    {
        [XmlEnum("")]
        NotSet = ' ',

        [XmlEnum("NC")]
        NoChange = 'N',

        [XmlEnum("A")]
        Add = 'A',

        [XmlEnum("M")]
        Modify = 'M',

        [XmlEnum("R")]
        Remove = 'R'
    }

    [Serializable]
    public class DeltaComments
    {
        [XmlElement("ChangedPrice")]
        public string ChangedPrice { get; set; }

        [XmlElement("ChangedModules")]
        public ChangedModules ChangedModules = new ChangedModules();

        [XmlAttribute("Operation")]
        public string Operation { get; set; }
    }

    [Serializable]
    public class ChangedModules
    {
        [XmlElement("Module")]
        public List<DeltaModules> DeltaModules = new List<DeltaModules>();
    }

    [Serializable]
    public class DeltaModules
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("Operation")]
        public string Operation { get; set; }

        [XmlElement("Options")]
        public OptionsClass Options = new OptionsClass();
    }

    [Serializable]
    public class OptionsClass
    {
        [XmlElement("Option")]
        public List<Option> OptionList = new List<Option>();
    }

    [Serializable]
    public class Option
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlAttribute("Operation")]
        public string Operation { get; set; }

        [XmlElement("OptionSkuList")]
        public DeltaOptionSkuList OptionSkuList = new DeltaOptionSkuList();
    }

    [Serializable]
    public class DeltaOptionSkuList
    {
        [XmlElement("OptionSku")]
        public List<DeltaOptionSku> OptionSkus = new List<DeltaOptionSku>();
    }

    [Serializable]
    public class DeltaOptionSku
    {
        [XmlAttribute("Operation")]
        public string Operation { get; set; }

        [XmlElement("SkuInfo")]
        public string SkuId { get; set; }

        [XmlElement("SkuDescription")]
        public string Description { get; set; }
    }
}
