using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace E_Assignment.Models
{
    public interface IDiplomaRepository
    {
        Diploma GetDiploma(int id);
        IEnumerable<Diploma> GetDiplomaWithTeachers(int id);
        IEnumerable<Diploma> GetAllDiplomasForTeachers(string username);
        IEnumerable<Diploma> GetAllDiplomasForStudents(string username);
        Diploma Add(Diploma diploma);
        Diploma Update(Diploma diplomaChanges);
        Diploma Delete(int id);
    }
}
