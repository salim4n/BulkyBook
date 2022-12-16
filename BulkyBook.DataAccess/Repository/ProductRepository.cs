using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Product obj)
        {
            var objDb = _db.Products.FirstOrDefault(p => p.Id == obj.Id);
            if(objDb == null)
            {
                objDb.Title = obj.Title;
                objDb.ISBN = obj.ISBN;
                objDb.Price = obj.Price;
                objDb.Price50 = obj.Price50;
                objDb.ListPrice = obj.ListPrice;
                objDb.Price100 = obj.Price100;
                objDb.Description = obj.Description;
                objDb.CategoryId = obj.CategoryId;
                objDb.Author = obj.Author;
                objDb.CoverTypeId = obj.CoverTypeId;
                if(obj.ImageUrl != null)
                {
                    objDb.ImageUrl = obj.ImageUrl;
                }


            }

        }
    }
}
