using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Modules.Channel.B2B.CatalogXMLTemplates
{
    [Serializable]
    [XmlRoot("B2BXML")]
    public class CRTXML
    {
        [XmlElement("CrossReference")]
        public CrossReference CrossReference { get; set; }
    }

    public class CrossReference
    {
        [XmlElement("CRTValues")]
        public CRTValues CRTValues { get; set; }
    }

    public class CRTValues
    {
        [XmlAttribute("CRTId")]
        public string CRTId { get; set; }

        [XmlAttribute("CRTType")]
        public string CRTType { get; set; }

        [XmlElement("CRTValue")]
        public List<CRTValue> CRTValue { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
        public CRTValues() { }
    }

    public class CRTValue
    {
        [XmlElement("Item")]
        public List<CrtItem> Item { get; set; }

        [XmlAttribute("Id")]
        public string Id { get; set; }
    }

    public class CrtItem
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("Data")]
        public string Data { get; set; }
    }
}
