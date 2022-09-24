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
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using Syncfusion.Drawing;
using Microsoft.AspNetCore.Http;

namespace E_Assignment.Controllers
{
    public class DiplomaController : Controller
    {        
        private readonly IDiplomaRepository _diplomaRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IHostingEnvironment hostingEnvironment;        

        public DiplomaController(IDiplomaRepository diplomaRepository, IHostingEnvironment hostingEnvironment, ITeacherRepository teacherRepository)
        {
            _diplomaRepository = diplomaRepository;
            _teacherRepository = teacherRepository;
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
            
            if (ModelState.IsValid)
            {                
                string filePath = null;
                if (diplomaVM.File != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "diplomas");                    
                    filePath = Path.Combine(uploadsFolder, diplomaVM.File.FileName);
                    FileStream stream = new FileStream(filePath, FileMode.Create);
                    diplomaVM.File.CopyTo(stream);
                    diplomaVM.Status = "Assigned";
                    stream.Close();
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
                diploma.FilePath = diplomaVM.File.FileName;
                _diplomaRepository.Update(diploma);
            }            
            return RedirectToAction("ShowDiplomasStudents");
        }
        
        [Authorize(Roles = "Teacher")]
        public RedirectToActionResult DeleteDiploma(int id)
        {
            String rootFolder = hostingEnvironment.WebRootPath + @"\diplomas\";
            Diploma diploma = _diplomaRepository.GetDiploma(id);
            _diplomaRepository.Delete(id);
            if (System.IO.File.Exists(Path.Combine(rootFolder, diploma.FilePath)))
            {
                System.IO.File.Delete(Path.Combine(rootFolder, diploma.FilePath));
            }
            
            return RedirectToAction("ShowDiplomas");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult PreviewDiploma(int id)
        {
            var diploma = _diplomaRepository.GetDiplomaWithTeachers(id);
            var username = User.FindFirstValue(ClaimTypes.Name);
            ViewData["Username"] = username;
            return View(diploma);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult SignDiploma(string Browser, int id, string password, string Reason, string Location, string Contact, IFormFile certificate)
        {
            var diploma = _diplomaRepository.GetDiploma(id);
            //Updates database that teacher has signed the diploma
            var diplomaTeachers = _diplomaRepository.GetDiplomaWithTeachers(id);
            var diplomas = diplomaTeachers.ToArray();
            var signInUser = User.FindFirstValue(ClaimTypes.Name);            
            String rootFolder = hostingEnvironment.WebRootPath+ @"\diplomas\";
            String diplomaPath = rootFolder+diploma.FilePath;
            var pdfdocument = System.IO.File.OpenRead(diplomaPath);           

            if (pdfdocument != null && pdfdocument.Length > 0 && certificate != null && certificate.Length > 0 && certificate.FileName.Contains(".pfx") && password != null && Location != null && Reason != null && Contact != null)
            {
                PdfLoadedDocument ldoc = new PdfLoadedDocument(pdfdocument);                
                PdfCertificate pdfCert = new PdfCertificate(certificate.OpenReadStream(), password);
                PdfPageBase page = ldoc.Pages[0];
                PdfSignature signature = new PdfSignature(ldoc, page, pdfCert, "Signature");
                signature.Bounds = new RectangleF(new PointF(5, 5), new SizeF(200, 200));
                signature.ContactInfo = Contact;
                signature.LocationInfo = Location;
                signature.Reason = Reason;
                signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA256;
                MemoryStream stream = new MemoryStream();                
                ldoc.Save(stream);                
                ldoc.Close(true);
                stream.Position = 0;                                         
                FileStream streamFile = new FileStream(diplomaPath, FileMode.Create);
                stream.CopyTo(streamFile);
                stream.Close();
                streamFile.Close();

                int signedNumber = 0;
                //Changes the status to signed for the specific teacher
                foreach (var teacher in diplomas[0].Teachers)
                {
                    if (teacher.Name.Equals(signInUser))
                    {
                        teacher.Sign = true;
                        _teacherRepository.Update(teacher);

                    }
                    if (teacher.Sign)
                    {
                        signedNumber++;
                    }
                }
                int teachersNumber = diplomas[0].Teachers.Count;
                //Changes status to Signed when all signatures are signed
                if(signedNumber == teachersNumber)
                {
                    diploma.Status = "Signed";
                    _diplomaRepository.Update(diploma);
                }

            }
            else
            {
                ViewData["error"] = "Something wrong with the file or signature";
                return RedirectToAction("PreviewDiploma", new {id = diploma.Id });
            }            
            return RedirectToAction("ShowDiplomas");
        }
    }
}
