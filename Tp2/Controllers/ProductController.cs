using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tp2.Models;
using Tp2.Models;
using Tp2.Models.Repositories;
using Tp2.Models.Repositories;
using Tp2.ViewModels;

namespace Tp2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICategorieRepository CategRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        public ProductController(IProductRepository prodRepository, ICategorieRepository categRepository,
       IWebHostEnvironment hostingEnvironment)
        {
            ProductRepository = prodRepository;
            CategRepository = categRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: ProductController
        // Show all products
        [AllowAnonymous]

        public ActionResult Index()
        {
            var products = ProductRepository.GetAll();
            return View(products);
        }

        // GET: ProductController/Details/5
        // Show details of a specific product
        public ActionResult Details(int id)
        {
            var product = ProductRepository.GetById(id);  // ✅ use GetById
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: ProductController/Create
        // Display the create product form
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Create
        // Handle the form submission for creating a new product
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // Check if user selected an image
                if (model.ImagePath != null)
                {
                    // Path to wwwroot/images folder
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    // Make sure file name is unique
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy file to wwwroot/images
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImagePath.CopyTo(fileStream);
                    }
                }

                // Create new Product object
                Product newProduct = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    QteStock = model.QteStock,
                    CategoryId = model.CategoryId,
                    Image = uniqueFileName,  // Save file name to database
                    //Category = model.Category // Initialisation obligatoire
                };

                // Add product to repository
                ProductRepository.Add(newProduct);

                // Redirect to details page of the new product
                return RedirectToAction("Details", new { id = newProduct.ProductId });
            }

            // If validation fails, reload categories for dropdown
            ViewBag.CategoryId = new SelectList(
                CategRepository.GetAll(),
                "CategoryId",
                "CategoryName",
                model.CategoryId);

            return View(model);
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            Product product = ProductRepository.GetById(id);
            var productEditViewModel = new Tp2.ViewModels.EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.Image ?? string.Empty,
                //Category = product.Category ?? new Category { CategoryName = string.Empty, Products = new List<Product>() },
                ImagePath = null!
            };
            return View(productEditViewModel);
        }
     
        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(), "CategoryId", "CategoryName");
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the product being edited from the database
                Product product = ProductRepository.GetById(model.ProductId);
                // Update the product object with the data in the model object
                product.Name = model.Name;
                product.Price = model.Price;
                product.QteStock = model.QteStock;
                product.CategoryId = model.CategoryId;
                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.ImagePath != null)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the product object which will be
                    // eventually saved in the database
                    product.Image = ProcessUploadedFile(model);
                }

                // Call update method on the repository service passing it the
                // product object to update the data in the database table
                Product updatedProduct = ProductRepository.Update(product);
                if (updatedProduct != null)
                    return RedirectToAction("Index");
                else
                    return NotFound();
            }
            return View(model);
        }
        [NonAction]
        private string ProcessUploadedFile(EditViewModel model)
        {
            string uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // GET: ProductController/Delete/5
        // Show confirmation page before deleting a product
        public ActionResult Delete(int id)
        {
            var product = ProductRepository.GetById(id); // ✅ use GetById
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: ProductController/Delete/5
        // Confirm and delete the product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Search(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                // Retourner tous les produits si recherche vide
                return View("Index", ProductRepository.GetAll());
            }

            // Utiliser la nouvelle méthode de recherche
            var results = ProductRepository.FindByNameOrCategory(val);

            return View("Index", results);
        }
    }
}