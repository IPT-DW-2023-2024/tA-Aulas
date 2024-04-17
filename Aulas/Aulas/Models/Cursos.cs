using System.ComponentModel.DataAnnotations;

namespace Aulas.Models {

   /// <summary>
   /// descrição do curso a que um Aluno se pode inscrever
   /// </summary>
   public class Cursos {

      public Cursos() {
         ListaUCs = new HashSet<UnidadesCurriculares>();
         ListaAlunos = new HashSet<Alunos>();
      }

      [Key] // PK
      public int Id { get; set; }

      /// <summary>
      /// nome do Curso
      /// </summary>
      [Required(ErrorMessage ="O {0} é de preenchimento obrigatório!")]
      [StringLength(100)]
      public string Nome { get; set; }

      /// <summary>
      /// nome do ficheiro que contém o logótipo do Curso
      /// </summary>
      [Display(Name ="Logótipo")] // define o nome a aparecer no ecrã
      [StringLength(50)]
      public string? Logotipo { get; set; } // o '?' vai tornar o atributo em preenchimento facultativo

      /* ****************************************
       * Construção dos Relacionamentos
       * *************************************** */

      // relacionamento 1-N

      /// <summary>
      /// Lista das UCs do Curso
      /// </summary>
      public ICollection<UnidadesCurriculares> ListaUCs { get; set; }

      /// <summary>
      /// lista dos Alunos 'matriculados' no Curso
      /// </summary>
      public ICollection<Alunos> ListaAlunos { get; set; }


   }
}
