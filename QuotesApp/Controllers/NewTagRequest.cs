using System.ComponentModel.DataAnnotations;

namespace QuotesApp.Controllers
{
    public class NewTagRequest
    {
        [Required(ErrorMessage = "Please enter the tag name.")]
        public string Name { get; set; }
    }
}
