using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionImporter.Models
{
    public class ApiResponse
    {
        public string Version { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public ApiResponse(object data, string message = "Success", string version = "1.0")
        {
            Data = data;
            Version = version;
            Message = message;
        }

    }
}
