using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace mvc_app.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context; 
 
     
 List<Category> listCategories = new List<Category>();

    public CategoryController(ApplicationDbContext context)
    {
       this._context = context;
    }

    [HttpGet]
    public IActionResult ReadCategory()
    { 
        listCategories = _context.Categories.ToList();
         ViewBag.SelectedCategory =  HttpContext.Session.GetString("SelectedCategory");
        return View(listCategories);
    }  
}
//  HttpContext.Session.SetString("SelectedCategoryName", )