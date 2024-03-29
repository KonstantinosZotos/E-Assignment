﻿using E_Assignment.Data;
using E_Assignment.ViewModels;
using Microsoft.EntityFrameworkCore;
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
            return context.Diplomas.Include(d => d.Teachers).Where(d => d.Teachers.Any(t => t.Name == username));
        }
        public IEnumerable<Diploma> GetAllDiplomasForStudents(string username)
        {
            return context.Diplomas.Where(b => b.StudentName == username);
        }

        public Diploma GetDiploma(int id)
        {            
            return context.Diplomas.Find(id);
        }        

        public IEnumerable<Diploma> GetDiplomaWithTeachers(int id)
        {
            return context.Diplomas.Include(t => t.Teachers).Where(d => d.Id == id);
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
