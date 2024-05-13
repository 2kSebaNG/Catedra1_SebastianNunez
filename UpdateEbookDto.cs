using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ebooks_dotnet8_api
{
    public class UpdateEbookDto
    {
        
        [StringLength(256, MinimumLength = 3)]
        public string? Title { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public string? Author { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public string? Genre { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public string? Format { get; set; }

        [Range(1, int.MaxValue)]
        public int? Price { get; set; }

    }
}