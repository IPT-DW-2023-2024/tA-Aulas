using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aulas.Models {

   [PrimaryKey(nameof(AlunoFK), nameof(UnidadeCurricularFK))] // PK para EF >= 7.0
   public class Inscricoes {
      // tabela do relacionamento N-M, COM atributos do relacionamento

      public DateTime DataInscricao { get; set; }


      // relacionamento N-M, COM atributos no relacionamento


      //  [Key, Column(Order = 1)] // PK para EF <= 6.0
      [ForeignKey(nameof(UnidadeCurricular))]
      public int UnidadeCurricularFK { get; set; }
      public UnidadesCurriculares UnidadeCurricular { get; set; }

      //  [Key, Column(Order = 2)] // PK para EF <= 6.0
      [ForeignKey(nameof(Aluno))]
      public int AlunoFK { get; set; }
      public Alunos Aluno { get; set; }

   }
}
