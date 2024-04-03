using System.ComponentModel.DataAnnotations.Schema;

namespace Aulas.Models {
   public class Alunos : Utilizadores {
      // um Aluno é um objeto do tipo Utilizadores
      // um Aluno é um caso particular de Utlizadores

      public Alunos() {
         ListaInscricoes = new HashSet<Inscricoes>();
      }


      public int NumAluno { get; set; }

      public decimal Propinas { get; set; }

      public DateTime DataMatricula { get; set; }


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



      // relacionamento N-M, com atributos no relacionamento
      public ICollection<Inscricoes> ListaInscricoes { get; set; }
   }
}
