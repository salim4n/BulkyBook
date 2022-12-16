using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypesList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypesList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Cover type created successfylly";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var coverTypeDb = _unitOfWork.CoverType.GetFirstOrDefault(ct => ct.Id == id);
            if(coverTypeDb == null)
            {
                return NotFound();
            }

            return View(coverTypeDb);
        }

        [HttpPost] [ValidateAntiForgeryToken]
       
        public IActionResult Edit(CoverType obj)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.CoverType.update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Cover type updated successfully";
                return RedirectToAction("index");
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var coverTypeDb = _unitOfWork.CoverType.GetFirstOrDefault(cv => cv.Id == id);
            if(coverTypeDb == null)
            {
                return NotFound();
            }
            return View(coverTypeDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(cv => cv.Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            _unitOfWork.CoverType.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Cover type deleted successfully";
            return RedirectToAction("Index");
        }

         
    }
}
