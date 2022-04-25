using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionImporter.Models;

namespace TransactionImporter.Services.Csv
{
    public class CsvImportResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public List<Transaction> Transactions { get; set; }
    }

    public class CsvService
    {
        public CsvImportResult ReadTransaction(IFormFile file)
        {
            var transactions = new List<Transaction>();
            var r = new CsvImportResult();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<Services.Csv.CsvMapTransaction>();
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<Transaction>();
                        transactions.Add(record);
                    }
                }

                r.Success = true;
                r.Message = "Read success";
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
