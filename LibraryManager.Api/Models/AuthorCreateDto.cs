using System;
using System.Collections.Generic;

namespace LibraryManager.Api.Models
{
    public class AuthorCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }

        public ICollection<BookCreateDto> Books { get; set; } = new List<BookCreateDto>();
    }
}