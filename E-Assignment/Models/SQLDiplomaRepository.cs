using E_Assignment.Data;
using E_Assignment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Assignment.Models
{
    public class SQLDiplomaRepository : IDiplomaRepository
    {
        private readonly E_AssignmentDbContext context;
        public SQLDiplomaRepository(E_AssignmentDbContext context)
        {
            this.context = context;
        }
        public Diploma Add(Diploma diploma)
        {            
            context.Diplomas.Add(diploma);                                    
            context.SaveChanges();
            return diploma;
        }

        public Diploma Delete(int id)
        {
            Diploma diploma = context.Diplomas.Find(id);
            if(diploma != null)
            {
                context.Diplomas.Remove(diploma);
                context.SaveChanges();
            }
            return diploma;
        }
        
        public IEnumerable<Diploma> GetAllDiplomasForTeachers(string username)
        {
            Teacher t = new Teacher();
            t.Name = username;
            List<Teacher> teacherList = new List<Teacher>();
            teacherList.Add(t);
            var teachers = context.Teachers.Where(a => a.Name.Contains(t.Name)).Select(a => a.DiplomaId).ToArray();
            var query = context.Diplomas.Join(
                context.Teachers,
                diploma => diploma.Id,
                teacher => teacher.DiplomaId,
                (diploma, teacher) => new Diploma()
                {
                    Id = diploma.Id,
                    Title = diploma.Title,
                    Description = diploma.Description, 
                    Teachers = teacherList,
                    StudentName = diploma.StudentName,
                    Status = diploma.Status,
                    FilePath = diploma.FilePath
                }).Where(a => a.Teachers[0].Name.Contains(t.Name));   
            
            return query;
        }
        public IEnumerable<Diploma> GetAllDiplomasForStudents(string username)
        {
            return context.Diplomas.Where(b => b.StudentName == username);
        }

        public Diploma GetDiploma(int Id)
        {
            return context.Diplomas.Find(Id);
        }

        public Diploma Update(Diploma diplomaChanges)
        {
            var diploma = context.Diplomas.Attach(diplomaChanges);
            diploma.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return diplomaChanges;
        }
    }
}
