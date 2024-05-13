using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebooks_dotnet8_api
{
    public class Purchase
    {
        public int EbookId { get; set; }

        public int Quantity { get; set; }

        public int TotalPrice { get; set; }
    }
}