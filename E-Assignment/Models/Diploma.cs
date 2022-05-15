using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.Models
{
    public class Diploma
    {
        public Diploma(){
            this.Status = "Pending";
        }
        public int Id { get; set; }
        [Required]     
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Teachers")]
        public List<Teacher> Teachers { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        public string Status { get; set; }
        [Display(Name = "FilePath")]
        public string FilePath { get; set; }
    }
}
