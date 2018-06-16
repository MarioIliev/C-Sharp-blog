using System;
using System.Collections.Generic;
using System.Linq;
namespace CSharpBlog.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using CSharpBlog.Data;
    using CSharpBlog.Models;
    using Microsoft.AspNetCore.Authorization;

    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext database;

        public ArticleController(ApplicationDbContext context)
        {
            database = context;
        }

        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //GET: index of article
        public ActionResult List()
        {
            List<Article> articles = database.Articles
                .Include(a=> a.Author)
                .ToList();

            return View(articles);
        }

        // GET: Article/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = database.Articles
                .Include(a => a.Author)
                .First(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        // GET: Article/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(database.Users, "Id", "Id");
            return View();
        }


        // POST: Article/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                //TODO check if logged(Authorize)
                var authorId = database.Users.Where(u => u.UserName == User.Identity.Name)
                    .FirstOrDefault()
                    .Id;

                article.AuthorId = authorId;

                database.Articles.Add(article);
                database.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }


        // GET: Article/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = database.Articles
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .First();

            if (IsAuthorized(article))
            {
                return Forbid();
            }

            var model = new ArticleViewModel(article);


            return View(model);
        }


        // POST: Article/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,  ArticleViewModel model)
        {

            if (ModelState.IsValid)
            {
                var article = database.Articles
                    .Where(a => a.Id == model.Id)
                    .Include(a => a.Author)
                    .FirstOrDefault();

                article.Title = model.Title;
                article.Content = model.Content;

                database.Update(article);
                database.SaveChanges();

                return RedirectToAction("Index");
            }

            //If is invalid return same view
            return View(model);
        }


        // GET: Article/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await database.Articles
                .Include(a => a.Author)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await database.Articles.SingleOrDefaultAsync(m => m.Id == id);
            database.Articles.Remove(article);
            await database.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return database.Articles.Any(e => e.Id == id);
        }

        private bool IsAuthorized(Article article)
        {
            bool isAuthor = article.IsAuthor(User.Identity.Name);
            bool isAdmin = User.IsInRole("Admin");

            return isAdmin || isAuthor;
        }
    }
}
