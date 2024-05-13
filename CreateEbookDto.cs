using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ebooks_dotnet8_api
{
    public class CreateEbookDto
    {
        [StringLength(256, MinimumLength = 3)]
        public required string Title { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public required string Author { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public required string Genre { get; set; }
        
        [StringLength(256, MinimumLength = 3)]
        public required string Format { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, int.MaxValue)]
        public required int Price { get; set; }

        public int Stock { get; set; } = 0;
    }
}