using E_Assignment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.Models
{
    public class SQLTeacherRepository : ITeacherRepository
    {
        private readonly E_AssignmentDbContext context;

        public SQLTeacherRepository(E_AssignmentDbContext context)
        {
            this.context = context;
        }
        public Teacher GetTeacher(int id)
        {
            throw new NotImplementedException();
        }

        public Teacher Update(Teacher teacherChanges)
        {
            var teacher = context.Teachers.Attach(teacherChanges);
            teacher.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return teacherChanges;
        }
    }
}
