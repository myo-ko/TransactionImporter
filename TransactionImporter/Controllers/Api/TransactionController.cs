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

        [Route("api/transactions/currency")]
        public IActionResult GetTransactions()
        {
            return ComposeResult();
        }

        [HttpPost]
        [Route("api/transactions")]
        public IActionResult ImportFromFile(IFormFile file, [FromServices] CsvService csvService)
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

        protected JsonResult ComposeError(object data = null)
        {
            return new JsonResult(new ApiResponse(data));
        }
    }
}
