using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Models;

namespace MyLibrary.Controllers {
    public class BooksController : Controller {
        private readonly MyLibraryContext _context;

        public BooksController(MyLibraryContext DbContext) {
            _context = DbContext;
        }

        public async Task<IActionResult> BookShelf(string sortKey, bool readFilter, string? searchString) {
            var query = _context.Books.AsQueryable();

            //filter through the books db and see if the search is in a title or author
            //checks if string has a value
            if (!String.IsNullOrEmpty(searchString)) {
                query = query.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
                //passing the searchstring back to the page so that the searchbar is sticky
                ViewBag.SearchString = searchString;
            }

            if (readFilter == true) {
                query = query.Where(b => b.Read == true);
            }

            switch (sortKey) {
                case "Title Asc.":
                default:
                    query = query.OrderBy(b => b.Title);
                    break;
                case "Title Desc.":
                    query = query.OrderByDescending(b => b.Title);
                    break;
                case "Author Asc.":
                    query = query.OrderBy(b => b.Author);
                    break;
                case "Author Desc.":
                    query = query.OrderByDescending(b => b.Author);
                    break;
                case "Rating Asc.":
                    query = query.OrderBy(b => b.Rating);
                    break;
                case "Rating Desc.":
                    query = query.OrderByDescending(b => b.Rating);
                    break;
            }

            List<Book> books = await query.ToListAsync();
            ViewBag.SortKey = sortKey;
            ViewBag.ReadFilter = readFilter;
            return View(books);
        }

        [HttpGet]
        public IActionResult Create() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title, Author, Rating, Read")] Book newBook) {
            if (ModelState.IsValid) {
                await _context.Books.AddAsync(newBook);
                await _context.SaveChangesAsync();
                return RedirectToAction("BookShelf");
            }
            Console.Write("Not Valid: " + newBook.Title + newBook.Author + newBook.Rating);
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            var book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book is not null) {
                return View(book);
            }

            return RedirectToAction("BookShelf");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id, Title, Author, Rating, Read")] Book newBook) {
            if (ModelState.IsValid) {
                _context.Update(newBook);
                await _context.SaveChangesAsync();
                return RedirectToAction("BookShelf");
            }
            return View(newBook.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book is not null) {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("BookShelf");
            }
            return RedirectToAction("Edit");
        }
    }
}
