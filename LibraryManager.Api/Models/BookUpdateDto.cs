using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Api.Models
{
    public class BookUpdateDto : BaseBookCrudDto
    {
        [Required(ErrorMessage = "Please provide a description for the book.")]
        public override string Description
        {
            get { return base.Description; }
            set { base.Description = value;}
        }
        
    }
}