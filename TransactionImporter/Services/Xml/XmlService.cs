using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TransactionImporter.Models;

namespace TransactionImporter.Services.Xml
{
    public class XmlImportResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public List<Transaction> Transactions { get; set; }
    }

    public class XmlService
    {
        public XmlImportResult ReadTransaction(IFormFile file)
        {
            var transactions = new List<Transaction>();
            var r = new XmlImportResult();

            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(TransactionCollection));
                var obj = (TransactionCollection)serializer.Deserialize(file.OpenReadStream());
                foreach (var item in obj.Transaction)
                {

                    if (!decimal.TryParse(item.PaymentDetails.Amount, out decimal amount))
                    {
                        throw new InvalidCastException($"Invalid amount value.");
                    }

                    if (!DateTime.TryParse(item.TransactionDate, out DateTime date))
                    {
                        throw new InvalidCastException($"Invalid datetime value.");
                    }

                    var status = item.Status.ToLower() switch
                    {
                        "approved" => Status.Approved,
                        "rejected" => Status.Failed,
                        "done" => Status.Finished,
                        _ => throw new InvalidCastException($"Invalid status value."),
                    };

                    var model = new Transaction()
                    {
                        TransactionId = item.TransactionId,
                        Amount = amount,
                        Currency = item.PaymentDetails.CurrencyCode,
                        TransactionDate = date,
                        Status = status,
                    };

                    transactions.Add(model);
                }

                r.Success = true;
                r.Message = "Success";
                r.Transactions = transactions;
            }
            catch (Exception ex)
            {
                r.Success = false;
                r.Message = ex.Message;
            }

            return r;
        }
    }
}
