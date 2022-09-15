using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.Models
{
    public interface ITeacherRepository
    {
        Teacher GetTeacher(int id);
        Teacher Update(Teacher teacherChanges);
    }
}
