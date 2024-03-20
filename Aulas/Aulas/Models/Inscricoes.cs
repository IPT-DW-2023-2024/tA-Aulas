using System.ComponentModel.DataAnnotations.Schema;

namespace Aulas.Models {
   public class Inscricoes {
      // tabela do relacionamento N-M, COM atributos do relacionamento

      public DateTime DataInscricao { get; set; }


      // relacionamento N-M, COM atributos no relacionamento

      [ForeignKey(nameof(UnidadeCurricular))]
      public int UnidadeCurricularFK { get; set; }
      public UnidadesCurriculares UnidadeCurricular { get; set; }

      [ForeignKey(nameof(Aluno))]
      public int AlunoFK { get; set; }
      public Alunos Aluno { get; set; }

   }
}
