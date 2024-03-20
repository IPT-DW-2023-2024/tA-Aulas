namespace Aulas.Models {
   public class Professores : Utilizadores {

      public Professores() {
         ListaUCs = new HashSet<UnidadesCurriculares>();
      }

      public ICollection<UnidadesCurriculares> ListaUCs { get; set; }

   }
}
