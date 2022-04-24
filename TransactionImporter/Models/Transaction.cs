using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionImporter.Models
{
    public enum Status
    {
        Approved = 0,
        Failed = 1,
        Finished = 2,
    }

    public class Transaction
    {
        public string TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTime TransactionDate { get; set; }

        public Status Status { get; set; }

    }
}
