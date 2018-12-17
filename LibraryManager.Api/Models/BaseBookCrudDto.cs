using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Api.Models
{
    public abstract class BaseBookCrudDto
    {
        [Required(ErrorMessage = "Please provide a title for the book.")]
        [MaxLength(100, ErrorMessage = "A book's title cannot be more than 100 characters long.")]
        public virtual string Title { get; set; }

        [MaxLength(500, ErrorMessage = "A book's description cannot be more than 500 characters long.")]
        public virtual string Description { get; set; }
    }
}