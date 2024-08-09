using Azure;
using Azure.Data.Tables;

namespace AppSDR
{
    // Class handles table entities when working with Table Storage
    public class TableEntityConfiguration : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string GraphName { get; set; }
        public string MaxCycles { get; set; }
        public string HighlightTouch { get; set; }
        public string XaxisTitle { get; set; }
        public string YaxisTitle { get; set; }
        public string MinRange { get; set; }
        public string MaxRange { get; set; }
        public string SavedName { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}