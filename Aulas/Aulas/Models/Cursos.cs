namespace Aulas.Models {
   public class Cursos {

      public Cursos() {
         ListaUCs = new HashSet<UnidadesCurriculares>();
         ListaAlunos = new HashSet<Alunos>();
      }

      public int Id { get; set; }

      public string Nome { get; set; }

      public string Logotipo { get; set; }

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
