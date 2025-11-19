using Microsoft.Extensions.Configuration;
using Tp2.Models;

namespace Tp2.Models.Repositories
{
    public class CategoryRepository : ICategorieRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IList<Category> GetAll()
        {
            return _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        public Category? GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        public void Add(Category c)
        {
            _context.Categories.Add(c);
            _context.SaveChanges();
        }

        public Category? Update(Category c)
        {
            var c1 = _context.Categories.Find(c.CategoryId);
            if (c1 != null)
            {
                c1.CategoryName = c.CategoryName;
                _context.SaveChanges();
            }
            return c1;
        }

        public void Delete(int CategoryId)
        {
            var c1 = _context.Categories.Find(CategoryId);
            if (c1 != null)
            {
                _context.Categories.Remove(c1);
                _context.SaveChanges();
            }
        }
    }
}