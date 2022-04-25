using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TransactionImporter.Models
{
    [XmlRoot("TransactionCollection")]
    public class TransactionCollection
    {
        [XmlArray("Transactions")]
        [XmlArrayItem("Transaction", typeof(XmlTransaction))]
        public XmlTransaction[] Transaction { get; set; }
    }

    public class XmlTransaction
    {
        [XmlAttribute("id")]
        public string TransactionId { get; set; }

        [XmlElement("TransactionDate")]
        public string TransactionDate { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }
    }

    public class PaymentDetails
    {
        [XmlElement("Amount")]
        public string Amount { get; set; }

        [XmlElement("CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}
