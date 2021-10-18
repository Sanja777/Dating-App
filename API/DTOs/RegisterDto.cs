using System.ComponentModel.DataAnnotations;

namespace API.dtoS
{
    public class RegisterDto
    {
       [System.ComponentModel.DataAnnotations.Required]

        public string Username { get; set; } 

[Required]
[StringLength(8, MinimumLength =4)]

      public string Password { get; set; } 

    }
}