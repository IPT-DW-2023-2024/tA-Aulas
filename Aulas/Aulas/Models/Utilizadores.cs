using System.ComponentModel.DataAnnotations;

namespace Aulas.Models {
   public class Utilizadores {

      [Key] // PK
      public int Id { get; set; }

      public string Nome { get; set; }

      [Display(Name ="Data Nascimento")]
      public DateOnly DataNascimento { get; set; }

      [Display(Name ="Telemóvel")]
      public string Telemovel { get; set; }

   }
}
