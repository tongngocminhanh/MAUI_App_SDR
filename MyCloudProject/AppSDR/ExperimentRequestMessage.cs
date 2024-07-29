using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSDR
{
    public class ExperimentRequestMessage
    {
        public string StorageConnectionString { get; set; }
        public string DownloadBlobStorage { get; set; }
    }
}
