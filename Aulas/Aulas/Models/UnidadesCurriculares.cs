using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aulas.Models {

   public class UnidadesCurriculares {
      // Vamos usar a Entity Framework para a construção do Model
      // https://learn.microsoft.com/en-us/ef/

      public UnidadesCurriculares() {
         ListaProfessores = new HashSet<Professores>();
         ListaInscricoes = new HashSet<Inscricoes>();
      }

      [Key] // PK
      public int Id { get; set; }

      public string Nome { get; set; }

      public int AnoCurricular { get; set; }

      public int Semestre { get; set; }

      /* ****************************************
       * Construção dos Relacionamentos
       * *************************************** */

      // relacionamento 1-N

      // esta anotação informa a EF
      // que o atributo 'CursoFK' é uma FK em conjunto
      // com o atributo 'Curso'
      [ForeignKey(nameof(Curso))]
      public int CursoFK { get; set; } // FK para o Curso
      public Cursos Curso { get; set; } // FK para o Curso


      // relacionamento M-N, SEM atributos no relacionamento
      public ICollection<Professores> ListaProfessores { get; set; }


      // relacionamento N-M, COM atributos no relacionamento
      public ICollection<Inscricoes> ListaInscricoes { get; set; }

   }
}
