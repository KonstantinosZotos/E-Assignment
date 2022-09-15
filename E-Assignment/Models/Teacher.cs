using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        [Display(Name = "TeacherName")]
        public string Name { get; set; }
        public int DiplomaId { get; set; }
        public Diploma Diploma { get; set; }
        public bool Sign { get; set; }
    }
}
