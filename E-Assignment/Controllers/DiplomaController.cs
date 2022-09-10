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
        public IActionResult CreateDiploma()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.UserName = userName;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public RedirectToActionResult CreateDiploma (Diploma diploma)
        {
            Diploma newDiploma = _diplomaRepository.Add(diploma);
            return RedirectToAction("ShowDiplomas", new { id = newDiploma.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ViewResult ShowDiplomas()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var diploma = _diplomaRepository.GetAllDiplomasForTeachers(userName);            
            IEnumerable<DiplomaViewModel> diplomaVM = diploma.Select(diplomaVM => new DiplomaViewModel { Id = diplomaVM.Id, Title = diplomaVM.Title, Teachers = diplomaVM.Teachers, Description = diplomaVM.Description, StudentName = diplomaVM.StudentName, Status = diplomaVM.Status });
            if (diploma == null)
            {
                ViewBag.ErrorMessage = $"No diploma is created yet";                
            }
            return View(diplomaVM);
        }
        [HttpGet]
        [Authorize(Roles = "Student")]
        public ViewResult ShowDiplomasStudents()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var diploma = _diplomaRepository.GetAllDiplomasForStudents(userName);            
            IEnumerable<DiplomaViewModel> diplomaVM = diploma.Select(diplomaVM => new DiplomaViewModel { Id = diplomaVM.Id, Title = diplomaVM.Title, Teachers = diplomaVM.Teachers, Description = diplomaVM.Description, StudentName = diplomaVM.StudentName, Status = diplomaVM.Status});
            if (diploma == null)
            {
                ViewBag.ErrorMessage = $"No diploma is created yet";
            }            
            return View(diplomaVM);
        }        
        

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult EditDiploma(int id)
        {
            
            Diploma diploma = _diplomaRepository.GetDiploma(id);                        
            return View(diploma);
        }        
        public FileResult DownloadFile(string fileName)
        {
            string filePath = Path.Combine(this.hostingEnvironment.WebRootPath, "diplomas/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);        
            return File(bytes, "application/octet-stream", fileName);
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
            var diploma = _diplomaRepository.GetDiploma(id);            
            DiplomaViewModel diplomaVM = new DiplomaViewModel();            
            diplomaVM = (DiplomaViewModel)diploma;                     
            return View(diplomaVM);            
        }

        //Enable the students to upload their diplomas
        [HttpPost]
        [Authorize(Roles = "Student")]       
        public IActionResult ViewDiploma(DiplomaViewModel diplomaVM)
        {   
            /*
            //Deletes previous file
            int id = diplomaVM.Id;
            Diploma diplomaFile = _diplomaRepository.GetDiploma(id);
            
            if (diplomaFile.FilePath != null)
            {
                String rootFolder = @"C:\Users\konst\source\repos\E-Assignment\E-Assignment\wwwroot\diplomas\";                              
                if (System.IO.File.Exists(Path.Combine(rootFolder, diplomaFile.FilePath)))
                {
                    System.IO.File.Delete(Path.Combine(rootFolder, diplomaFile.FilePath));
                }
            }
*/
            if(ModelState.IsValid)
            {
                string fileName = null;
                string filePath = null;
                if (diplomaVM.File != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "diplomas");
                    fileName = Guid.NewGuid().ToString() + "_" + diplomaVM.File.FileName;
                    filePath = Path.Combine(uploadsFolder, fileName);
                    diplomaVM.File.CopyTo(new FileStream(filePath, FileMode.Create));
                    diplomaVM.Status = "Assigned";
                }
                Diploma diploma = new Diploma
                {
                    Id = diplomaVM.Id,
                    Title = diplomaVM.Title,
                    Teachers = diplomaVM.Teachers,
                    Description = diplomaVM.Description,
                    StudentName = diplomaVM.StudentName,
                    Status = diplomaVM.Status,
                    FilePath = filePath
                };
                diploma.FilePath = fileName;
                _diplomaRepository.Update(diploma);
            }            
            return RedirectToAction("ShowDiplomasStudents");
        }
        
        [Authorize(Roles = "Teacher")]
        public RedirectToActionResult DeleteDiploma(int id)
        {
            String rootFolder = @"C:\Users\konst\source\repos\E-Assignment\E-Assignment\wwwroot\diplomas\";
            Diploma diploma = _diplomaRepository.GetDiploma(id);
            _diplomaRepository.Delete(id);
            if (System.IO.File.Exists(Path.Combine(rootFolder, diploma.FilePath)))
            {
                System.IO.File.Delete(Path.Combine(rootFolder, diploma.FilePath));
            }
            
            return RedirectToAction("ShowDiplomas");
        }

    }
}
