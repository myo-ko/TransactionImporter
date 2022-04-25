using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionImporter.DbContexts;
using TransactionImporter.Models;
using TransactionImporter.Services.Csv;
using TransactionImporter.Services.Xml;
using TransactionImporter.ViewModels;

namespace TransactionImporter.Controllers.Api
{

    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionDbContext dbContext;

        public TransactionController(TransactionDbContext _context1)
        {
            dbContext = _context1;
        }

        [HttpPost]
        [Route("api/transactions/currency")]
        public IActionResult GetTransactionsByCurrency([FromBody] string currency)
        {
            #region Validation
            if (string.IsNullOrEmpty(currency))
            {
                return BadRequest(new
                {
                    Message = "Currency is required."
                });
            }
            #endregion

            var transactions = dbContext.Transactions.Where(x => x.Currency.ToLower() == currency.ToLower())
                .Select(x => TransactionViewModel.FromTransactionModel(x))
                .ToList();

            return ComposeResult(transactions);
        }

        [HttpPost]
        [Route("api/transactions/date-range")]
        public IActionResult GetTransactionsByDateRange([FromBody] DateTime? start, [FromBody] DateTime? end)
        {
            #region Validation
            if (!start.HasValue)
            {
                return BadRequest(new
                {
                    Message = "Start date is required."
                });
            }

            if (!end.HasValue)
            {
                return BadRequest(new
                {
                    Message = "End date is required."
                });
            }
            #endregion

            var transactions = dbContext.Transactions.Where(x => x.TransactionDate >= start.Value && x.TransactionDate <= end.Value)
                .Select(x => TransactionViewModel.FromTransactionModel(x))
                .ToList();

            return ComposeResult(transactions);
        }

        [HttpPost]
        [Route("api/transactions/status")]
        public IActionResult GetTransactionsByStatus([FromBody] int? status)
        {
            #region Validation
            if (!status.HasValue)
            {
                return BadRequest(new
                {
                    Message = "Status is required."
                });
            }
            #endregion

            var transactions = dbContext.Transactions.Where(x => x.Status == (Status)status)
                .Select(x => TransactionViewModel.FromTransactionModel(x))
                .ToList();

            return ComposeResult(transactions);
        }

        [HttpPost]
        [Route("api/transactions")]
        public IActionResult ImportFromFile(IFormFile file, [FromServices] CsvService csvService, [FromServices] XmlService xmlService)
        {
            int insertCount = 0;
            var transactions = new List<Transaction>();

            #region Validation
            if (file == null)
            {
                return BadRequest(new
                {
                    Message = "No file uploaded"
                });
            }

            if (file.Length > 1024)
            {
                return BadRequest(new
                {
                    Message = "File cannot larger than 1MB."
                });
            }

            string[] _extensions = new string[] { ".csv", ".xml" };
            var extension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(extension.ToLower()))
            {
                return BadRequest(new
                {
                    Message = "Unknown format."
                });
            }
            #endregion

            #region Read CSV
            if (extension.ToLower() == ".csv")
            {
                var readResult = csvService.ReadTransaction(file);
                if (!readResult.Success)
                {
                    return BadRequest(new
                    {
                        Message = readResult.Message
                    });
                }

                transactions = readResult.Transactions;
            }
            #endregion

            #region Read Xml
            if (extension.ToLower() == ".xml")
            {
                var readResult = xmlService.ReadTransaction(file);
                if (!readResult.Success)
                {
                    return BadRequest(new
                    {
                        Message = readResult.Message
                    });
                }

                transactions = readResult.Transactions;
            }
            #endregion

            #region Save into DB
            foreach (var item in transactions)
            {
                if (!dbContext.Transactions.Any(x => x.TransactionId == item.TransactionId))
                {
                    dbContext.Transactions.Add(item);
                    dbContext.SaveChanges();
                    insertCount++;
                }
            }
            #endregion

            return ComposeResult(null, $"{insertCount} record(s) inserted.");
        }


        protected JsonResult ComposeResult(object data = null, string message = "Success")
        {
            return new JsonResult(new ApiResponse(data, message));
        }

    }
}
