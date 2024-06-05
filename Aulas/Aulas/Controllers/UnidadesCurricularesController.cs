using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aulas.Data;
using Aulas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Aulas.Controllers {


   [Authorize(Roles = "Professor,Administrativo")]
   /*
    *  [Authorize(Roles = "Professor")] --> apenas pessoas com o 'perfil' Professor consegue aceder
    *  
    *  [Authorize(Roles = "Professor,Administrativo")] --> apenas pessoas com o 'perfil' Professor OU com o 'perfil' Administrativo consegue aceder
    *  
    *  [Authorize(Roles = "Professor")] ------->
    *  [Authorize(Roles = "Administrativo")] -->  apenas pessoas com o 'perfil' Professor E com o 'perfil' Administrativo consegue aceder
    */

   public class UnidadesCurricularesController : Controller {

      private readonly ApplicationDbContext _context;

      /// <summary>
      /// objeto para interagir com os dados da pessoa autenticada
      /// </summary>
      private readonly UserManager<IdentityUser> _userManager;

      public UnidadesCurricularesController(
         ApplicationDbContext context,
         UserManager<IdentityUser> userManager) {
         _context = context;
         _userManager = userManager;
      }

      // GET: UnidadesCurriculares
      public async Task<IActionResult> Index() {

         var applicationDbContext = _context.UCs.Include(u => u.Curso);
         return View(await applicationDbContext.ToListAsync());


      }

      // GET: UnidadesCurriculares/Details/5
      public async Task<IActionResult> Details(int? id) {
         if (id == null) {
            return NotFound();
         }



         // Esta pesquisa aqui feita em LINQ
         // é equivalente a esta, em SQL:
         // SELECT *
         // FROM UnidadesCurriculares uc INNER JOIN Cursos c ON uc.CursoFK = c.Id
         //                              INNER JOIN ProfessoresUnidadesCurriculares puc
         //                                         ON puc.UnidadeFK = uc.Id
         //                              INNER JOIN Professores p ON puc.ProfessorFK = p.Id
         // WHERE uc.Id=id
         var unidadeCurricular = await _context.UCs
                                               .Include(u => u.Curso)
                                               .Include(u => u.ListaProfessores)
                                               .FirstOrDefaultAsync(m => m.Id == id);

         if (unidadeCurricular == null) {
            return NotFound();
         }

         return View(unidadeCurricular);
      }

      // GET: UnidadesCurriculares/Create
      public IActionResult Create() {
         ViewData["CursoFK"] = new SelectList(_context.Cursos.OrderBy(c => c.Nome), "Id", "Nome");

         // obter a lista de professores existentes na BD
         ViewData["ListaProfs"] = _context.Professores.OrderBy(p => p.Nome).ToList();

         return View();
      }

      // POST: UnidadesCurriculares/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("Nome,AnoCurricular,Semestre,CursoFK")] UnidadesCurriculares uc, int[] listaIdsProfessores) {

         // VALIDAR SE FOI ESCOLHIDO UM CURSO E, PELO MENOS, UM PROFESSOR

         // PQ HÁ PROFESSOR(ES)
         var listaProfessores = new List<Professores>();
         foreach (var profId in listaIdsProfessores) {
            var prof = _context.Professores.FirstOrDefault(p => p.Id == profId);
            //var prof = _context.Professores.Where(p => p.Id == profId).FirstOrDefault();

            if (prof != null) {
               listaProfessores.Add(prof);
            }
         }


         if (listaProfessores != null) {
            uc.ListaProfessores = listaProfessores;
         }
         else {
            // se chego aqui, houve tratalhada feita no browser
            // gerar mensagem de erro
            // notificar utilizador
            // enviar controlo à VIEW
         }

         if (ModelState.IsValid) {

            _context.Add(uc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
         }

         ViewData["CursoFK"] = new SelectList(_context.Cursos.OrderBy(c=>c.Id), "Id", "Nome", uc.CursoFK);
         // obter a lista de professores existentes na BD
         ViewData["ListaProfs"] = _context.Professores.OrderBy(p => p.Nome).ToList();

         return View(uc);
      }






      // GET: UnidadesCurriculares/Edit/5
      public async Task<IActionResult> Edit(int? id) {
         if (id == null) {
            return NotFound();
         }


         if (User.IsInRole("Administrativo")) {
            var uc = await _context.UCs.FindAsync(id);
            if (uc == null) {
               return NotFound();
            }

            ViewData["CursoFK"] = new SelectList(_context.Cursos.OrderBy(c => c.Nome), "Id", "Nome", uc.CursoFK);

            // falta fazer a lista de Professores, como no método da criação

            return View(uc);
         }

         // se chego aqui é pq sou professor
         // será que tenho autorização de editar a UC?

         // obter ID da pessoa autenticada
         var userId =  _userManager.GetUserId(User);

         // ID do Utilizador autenticado
         var idProf=_context.Professores
                            .Where(p=> p.UserID == userId)
                            .FirstOrDefault()
                            .Id;

         // Investigar se o Professor está associado à UC
         var unidadeCurricularComProf = _context.UCs
                                 .Where(uc => uc.Id == id &&
                                        uc.ListaProfessores.Any(p => p.Id == idProf))
                                 .FirstOrDefault();

         // se a UC não é nula, é pq o Professor está associado à UC
         if (unidadeCurricularComProf != null) {
            ViewData["CursoFK"] = new SelectList(_context.Cursos.OrderBy(c => c.Nome), "Id", "Nome", unidadeCurricularComProf.CursoFK);

            // falta fazer a lista de Professores, como no método da criação

            // enviar UC para a View
            return View(unidadeCurricularComProf);
         }
         else {
            // O professor naão está associado
            // gerar Mensagem de erro
            // notificar utilizador
            // etc.

            return NotFound();
         }

      }

      // POST: UnidadesCurriculares/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,AnoCurricular,Semestre,CursoFK")] UnidadesCurriculares unidadesCurriculares) {
         if (id != unidadesCurriculares.Id) {
            return NotFound();
         }

         if (ModelState.IsValid) {
            try {
               _context.Update(unidadesCurriculares);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
               if (!UnidadesCurricularesExists(unidadesCurriculares.Id)) {
                  return NotFound();
               }
               else {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         ViewData["CursoFK"] = new SelectList(_context.Cursos, "Id", "Nome", unidadesCurriculares.CursoFK);
         return View(unidadesCurriculares);
      }

      // GET: UnidadesCurriculares/Delete/5
      public async Task<IActionResult> Delete(int? id) {
         if (id == null) {
            return NotFound();
         }

         var unidadesCurriculares = await _context.UCs
             .Include(u => u.Curso)
             .FirstOrDefaultAsync(m => m.Id == id);
         if (unidadesCurriculares == null) {
            return NotFound();
         }

         return View(unidadesCurriculares);
      }

      // POST: UnidadesCurriculares/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id) {
         var unidadesCurriculares = await _context.UCs.FindAsync(id);
         if (unidadesCurriculares != null) {
            _context.UCs.Remove(unidadesCurriculares);
         }

         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

      private bool UnidadesCurricularesExists(int id) {
         return _context.UCs.Any(e => e.Id == id);
      }
   }
}
