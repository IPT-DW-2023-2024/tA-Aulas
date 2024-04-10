using System.ComponentModel.DataAnnotations;

namespace Aulas.Models {
   public class Cursos {

      public Cursos() {
         ListaUCs = new HashSet<UnidadesCurriculares>();
         ListaAlunos = new HashSet<Alunos>();
      }

      [Key] // PK
      public int Id { get; set; }

      [Required(ErrorMessage ="O {0} é de preenchimento obrigatório!")]
      [StringLength(100)]
      public string Nome { get; set; }


      [Display(Name ="Logótipo")] // define o nome a aparecer no ecrã
      [StringLength(50)]
      public string? Logotipo { get; set; } // o '?' vai tornar o atributo em preenchimento facultativo

      /* ****************************************
       * Construção dos Relacionamentos
       * *************************************** */

      // relacionamento 1-N

      // Lista das UCs que um Curso tem
      public ICollection<UnidadesCurriculares> ListaUCs { get; set; }

      // lista dos Alunos 'matriculados' num Curso
      public ICollection<Alunos> ListaAlunos { get; set; }


   }
}
