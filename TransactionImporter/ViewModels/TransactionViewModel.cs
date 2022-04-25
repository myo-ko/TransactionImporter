using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionImporter.Models;

namespace TransactionImporter.ViewModels
{
    public class TransactionViewModel
    {
        public string TransactionId { get; set; }

        public string Payment { get; set; }

        public string Status { get; set; }

        public static TransactionViewModel FromTransactionModel(Transaction model)
        {
            string status = string.Empty;

            switch (model.Status)
            {
                case Models.Status.Approved:
                    status = "A";
                    break;
                case Models.Status.Failed:
                    status = "R";
                    break;
                case Models.Status.Finished:
                    status = "D";
                    break;
                default:
                    break;
            }

            var vm = new TransactionViewModel
            {
                TransactionId = model.TransactionId,
                Payment = string.Format("{0:N2} {1}", model.Amount, model.Currency),
                Status = status,
            };

            return vm;
        }
    }
}
