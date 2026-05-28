using Core.DTOs.Usage;
using System.Data;

namespace Core.CommonFunctions
{
    public static class CommonFunctions
    {
        public static string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // returns a 6-digit string
        }

        public static DataTable CreateUsageEventDataTable(IEnumerable<UsageEventIngestItemDto> events)
        {
            var table = new DataTable();
            table.Columns.Add("CustomerId", typeof(long));
            table.Columns.Add("MonitorId", typeof(long));
            table.Columns.Add("RequestAtUtc", typeof(DateTime));
            table.Columns.Add("StatusCode", typeof(int));
            table.Columns.Add("LatencyMs", typeof(int));
            table.Columns.Add("IsSuccess", typeof(bool));
            table.Columns.Add("RequestUnits", typeof(int));
            table.Columns.Add("TraceId", typeof(string));

            foreach (var item in events)
            {
                var row = table.NewRow();
                row["CustomerId"] = item.CustomerId;
                row["MonitorId"] = item.MonitorId.HasValue ? item.MonitorId.Value : DBNull.Value;
                row["RequestAtUtc"] = item.RequestAtUtc;
                row["StatusCode"] = item.StatusCode;
                row["LatencyMs"] = item.LatencyMs;
                row["IsSuccess"] = item.IsSuccess;
                row["RequestUnits"] = item.RequestUnits;
                row["TraceId"] = item.TraceId ?? (object)DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
