using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aulas.Models {
   public class Alunos : Utilizadores {
      // um Aluno é um objeto do tipo Utilizadores
      // um Aluno é um caso particular de Utlizadores

      public Alunos() {
         ListaInscricoes = new HashSet<Inscricoes>();
      }


      public int NumAluno { get; set; }

      /// <summary>
      /// atributo auxiliar para recolher os dados da Propina
      /// </summary>
      [NotMapped] // informa a EF para ignorar este atributo
      [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
      [Display(Name = "Propina")]
      [StringLength(9)]
      [RegularExpression("[0-9]{1,6}([,.][0-9]{1,2})?", ErrorMessage = "Escreva um número com, no máximo 2 casas decimais, separadas por . ou ,")]
      public string PropinasAux { get; set; }

      /// <summary>
      /// Propina paga pelo Aluno aquando da matrícula no Curso
      /// </summary>
      public decimal Propinas { get; set; }

      [Display(Name ="Data Matrícula")]
      [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
      [DataType(DataType.Date)]
      public DateTime DataMatricula { get; set; }


      /* ****************************************
      * Construção dos Relacionamentos
      * *************************************** */

      // relacionamento 1-N

      // esta anotação informa a EF
      // que o atributo 'CursoFK' é uma FK em conjunto
      // com o atributo 'Curso'
      [ForeignKey(nameof(Curso))]
      [Display(Name ="Curso")]
      public int CursoFK { get; set; } // FK para o Curso
      public Cursos Curso { get; set; } // FK para o Curso



      // relacionamento N-M, com atributos no relacionamento
      public ICollection<Inscricoes> ListaInscricoes { get; set; }
   }
}
