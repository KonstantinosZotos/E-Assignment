﻿using E_Assignment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.ViewModels
{
    public class DiplomaViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Teacher Name")]
        public string TeacherName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        public string Status { get; set; }
        [Display(Name = "File")]
        public IFormFile File { get; set; }

        public static explicit operator DiplomaViewModel(Diploma d)
        {
            try
            {
                DiplomaViewModel diplomaVM = new DiplomaViewModel();
                diplomaVM.Id = d.Id;
                diplomaVM.Title = d.Title;
                diplomaVM.TeacherName = d.TeacherName;
                diplomaVM.Description = d.Description;
                diplomaVM.StudentName = d.StudentName;
                diplomaVM.Status = d.Status;
                return diplomaVM;
            }
            catch
            {
                throw new NotImplementedException();
            }
            
        }
    }
}
