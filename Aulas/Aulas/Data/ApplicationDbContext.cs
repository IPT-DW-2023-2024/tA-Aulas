using Aulas.Models;

using Microsoft.AspNetCore.Identity;
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



      protected override void OnModelCreating(ModelBuilder builder) {
         /* Esta instrução importa tudo o que está pre-definido
          * na super classe
          */
         base.OnModelCreating(builder);

         /* Adição de dados à Base de Dados
          * Esta forma é PERSISTENTE, pelo que apenas deve ser utilizada em 
          * dados que perduram da fase de 'desenvolvimento' para a fase de 'produção'.
          * Implica efetuar um 'Add-Migration'
          * 
          * Atribuir valores às ROLES
          */
         builder.Entity<IdentityRole>().HasData(
             new IdentityRole { Id = "p", Name = "Professor", NormalizedName = "PROFESSOR" },
             new IdentityRole { Id = "adm", Name = "Administrativo", NormalizedName = "ADMINISTRATIVO" }
             );

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
