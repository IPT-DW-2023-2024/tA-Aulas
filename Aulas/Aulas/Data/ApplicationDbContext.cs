using Aulas.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aulas.Data {


   /// <summary>
   /// Esta classe representa a BD do nosso projeto
   /// </summary>
   public class ApplicationDbContext : IdentityDbContext {

      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options) {
      }

      /* ********************************************
       * definir as 'tabelas' da base de dados
       * ******************************************** */

      public DbSet<Utilizadores> Utilizadores { get; set; }
      public DbSet<Alunos> Alunos { get; set; }
      public DbSet<Professores> Professores { get; set; }
      public DbSet<Cursos> Cursos { get; set; }
      public DbSet<UnidadesCurriculares> UCs { get; set; }
      public DbSet<Inscricoes> Inscricoes { get; set; }

   }
}
