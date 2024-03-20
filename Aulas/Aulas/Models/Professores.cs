namespace Aulas.Models {
   public class Professores {

      public Professores() {
         ListaUCs = new HashSet<UnidadesCurriculares>();
      }

      public ICollection<UnidadesCurriculares> ListaUCs { get; set; }

   }
}
