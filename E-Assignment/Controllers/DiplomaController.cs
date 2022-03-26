using E_Assignment.Areas.Identity.Data;
using E_Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using E_Assignment.ViewModels;

namespace E_Assignment.Controllers
{
    public class DiplomaController : Controller
    {        
        private readonly IDiplomaRepository _diplomaRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public DiplomaController(IDiplomaRepository diplomaRepository, IHostingEnvironment hostingEnvironment)
        {
            _diplomaRepository = diplomaRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.UserName = userName;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ViewResult ShowDiplomas()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var model = _diplomaRepository.GetAllDiplomasForTeachers(userName);
            if(model == null)
            {
                ViewBag.ErrorMessage = $"No diploma is created yet";                
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Student")]
        public ViewResult ShowDiplomasStudents()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var diploma = _diplomaRepository.GetAllDiplomasForStudents(userName);
            if (diploma == null)
            {
                ViewBag.ErrorMessage = $"No diploma is created yet";
            }            
            return View(diploma);
        }        

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public RedirectToActionResult Create(Diploma diploma)
        {
            Diploma newDiploma = _diplomaRepository.Add(diploma);
            return RedirectToAction("ShowDiplomas", new { id = newDiploma.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult EditDiploma(int id)
        {
            
            Diploma diploma = _diplomaRepository.GetDiploma(id);
            return View(diploma);
        }
        
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult EditDiploma(Diploma diploma)
        {
            _diplomaRepository.Update(diploma);
            return RedirectToAction("ShowDiplomas");
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult ViewDiploma(int id)
        {
            Diploma diploma = _diplomaRepository.GetDiploma(id);
            return View(diploma);            
        }

        [HttpPost]
        [Authorize(Roles = "Student")]       
        public IActionResult ViewDiploma(Diploma diploma)
        {
            /* 
            DiplomaViewModel model = new DiplomaViewModel();
            string fileName = null;
            if (model.FilePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "diplomas");
                fileName = Guid.NewGuid().ToString() + "_" + model.FilePath.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);
                model.FilePath.CopyTo(new FileStream(filePath, FileMode.Create));
                diploma.Status = 2;
            }
            diploma.FilePath = fileName;
            */
            _diplomaRepository.Update(diploma);
            return RedirectToAction("ShowDiplomasStudents");
        }

    }
}
