using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aulas.Data;
using Aulas.Models;

namespace Aulas.Controllers {
   public class CursosController : Controller {
      /// <summary>
      /// referência à BD do projeto
      /// </summary>
      private readonly ApplicationDbContext _context;

      /// <summary>
      /// objecto que contém os dados do Servidor
      /// </summary>
      private readonly IWebHostEnvironment _webHostEnvironment;

      public CursosController(
         ApplicationDbContext context,
         IWebHostEnvironment webHostEnvironment) {
         _context = context;
         _webHostEnvironment = webHostEnvironment;
      }

      // GET: Cursos
      public async Task<IActionResult> Index() {
         return View(await _context.Cursos.ToListAsync());
      }

      // GET: Cursos/Details/5
      public async Task<IActionResult> Details(int? id) {
         if (id == null) {
            return NotFound();
         }

         var cursos = await _context.Cursos
             .FirstOrDefaultAsync(m => m.Id == id);
         if (cursos == null) {
            return NotFound();
         }

         return View(cursos);
      }

      // GET: Cursos/Create
      [HttpGet]
      public IActionResult Create() {
         return View();
      }

      // POST: Cursos/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("Nome")] Cursos curso, IFormFile ImagemLogo) {
         // a anotação [Bind] informa o servidor de quais os atributos
         // que devem ser lidos do objeto que vem do browser

         /* Guardar a imagem no disco rígido do Servidor
          * Algoritmo
          * 1- há ficheiro?
          *    1.1 - não
          *          devolvo controlo ao browser
          *          com mensagem de erro
          *    1.2 - sim
          *          Será imagem (JPG,JPEG,PNG)?
          *          1.2.1 - não
          *                  uso logótipo pre-definido
          *          1.2.2 - sim
          *                  - determinar o nome da imagem
          *                  - guardar esse nome na BD
          *                  - guardar o ficheir no disco rígido
          */

         // avalia se os dados recebido do browser estão
         // de acordo com o Model
         if (ModelState.IsValid) {

            // vars auxiliares
            string nomeImagem = "";
            bool haImagem = false;

            // há ficheiro?
            if (ImagemLogo == null) {
               // não há
               // crio msg de erro
               ModelState.AddModelError("",
                  "Deve fornecer um logótipo");
               // devolver controlo à View
               return View(curso);
            }
            else {
               // há ficheiro, mas é uma imagem?
               if (!(ImagemLogo.ContentType == "image/png" ||
                    ImagemLogo.ContentType == "image/jpeg"
                  )) {
                  // não
                  // vamos usar uma imagem pre-definida
                  curso.Logotipo = "logoCurso.jpg";
               }
               else {
                  // há imagem
                  haImagem = true;
                  // gerar nome imagem
                  Guid g = Guid.NewGuid();
                  nomeImagem = g.ToString();
                  string extensaoImagem = Path.GetExtension(ImagemLogo.FileName).ToLowerInvariant();
                  nomeImagem += extensaoImagem;
                  // guardar o nome do ficheiro na BD
                  curso.Logotipo = nomeImagem;
               }
            }


            // adiciona à BD os dados vindos da View
            _context.Add(curso);
            // Commit
            await _context.SaveChangesAsync();

            // guardar a imagem do logótipo
            if (haImagem) {
               // encolher a imagem ao tamanho certo --> fazer pelos alunos
               // procurar no NuGet

               // determinar o local de armazenamento da imagem
               string localizacaoImagem = _webHostEnvironment.WebRootPath;
               // adicionar à raiz da parte web, o nome da pasta onde queremos guardar as imagens
               localizacaoImagem = Path.Combine(localizacaoImagem, "Imagens");

               // será que o local existe?
               if (!Directory.Exists(localizacaoImagem)) {
                  Directory.CreateDirectory(localizacaoImagem);
               }

               // atribuir ao caminho o nome da imagem
               localizacaoImagem = Path.Combine(localizacaoImagem, nomeImagem);

               // guardar a imagem no Disco Rígido
               using var stream = new FileStream(
                  localizacaoImagem, FileMode.Create
                  );
               await ImagemLogo.CopyToAsync(stream);
            }




            // redireciona o utilizador para a página de 'início'
            // dos Cursos
            return RedirectToAction(nameof(Index));
         }
         // se cheguei aqui é pq alguma coisa correu mal
         // devolve controlo à View, apresentando os dados recebidos
         return View(curso);
      }

      // GET: Cursos/Edit/5
      public async Task<IActionResult> Edit(int? id) {
         if (id == null) {
            return NotFound();
         }

         var cursos = await _context.Cursos.FindAsync(id);
         if (cursos == null) {
            return NotFound();
         }
         return View(cursos);
      }

      // POST: Cursos/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Logotipo")] Cursos cursos) {
         if (id != cursos.Id) {
            return NotFound();
         }

         if (ModelState.IsValid) {
            try {
               _context.Update(cursos);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
               if (!CursosExists(cursos.Id)) {
                  return NotFound();
               }
               else {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         return View(cursos);
      }

      // GET: Cursos/Delete/5
      public async Task<IActionResult> Delete(int? id) {
         if (id == null) {
            return NotFound();
         }

         var cursos = await _context.Cursos
             .FirstOrDefaultAsync(m => m.Id == id);
         if (cursos == null) {
            return NotFound();
         }

         return View(cursos);
      }

      // POST: Cursos/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id) {
         var cursos = await _context.Cursos.FindAsync(id);
         if (cursos != null) {
            _context.Cursos.Remove(cursos);
         }

         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

      private bool CursosExists(int id) {
         return _context.Cursos.Any(e => e.Id == id);
      }
   }
}
