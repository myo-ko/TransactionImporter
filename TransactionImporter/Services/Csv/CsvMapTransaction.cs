using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TransactionImporter.Models;

namespace TransactionImporter.Services.Csv
{
    public class CsvMapTransaction : ClassMap<Transaction>
    {
        public CsvMapTransaction()
        {
            Map(m => m.TransactionId).Index(0);
            Map(m => m.Amount).Index(1).Convert(r =>
            {
                if (decimal.TryParse(r.Row.GetField(1), out var value))
                {
                    return value;
                }
                return 0;
            });
            Map(m => m.Currency).Index(2);
            Map(m => m.TransactionDate).Index(3).Convert(r =>
            {
                var value = r.Row.GetField(3);
                if (DateTime.TryParseExact(value, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
                throw new InvalidCastException($"Invalid value to DateTime.");
            });
            Map(m => m.Status).Index(4).TypeConverter<StatusConverter<Status>>();
        }
    }

    public class StatusConverter<T> : EnumConverter where T : struct
    {
        public StatusConverter() : base(typeof(T))
        { }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            switch (text.ToLower())
            {
                case "approved":
                    return Status.Approved;

                case "failed":
                    return Status.Failed;

                case "finished":
                    return Status.Finished;

                default:
                    throw new InvalidCastException($"Invalid status value.");
            }
        }

    }
}
