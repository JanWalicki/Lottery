using Lottery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Services
{
    public class DatabaseService
    {
        private readonly SchoolContext _context;

        public DatabaseService()
        {
            _context = new SchoolContext();
            _context.Database.EnsureCreated();
        }

        public void AddClass(Class classEntity)
        {
            _context.Classes.Add(classEntity);
            _context.SaveChanges();
        }

        public void AddStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public List<Class> GetAllClasses()
        {
            return _context.Classes.Include(c => c.Students).ToList();
        }

        public List<Student> GetAllStudents()
        {
            return _context.Students.ToList();
        }

        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }
        public void UpdateClass(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            _context.SaveChanges();
        }

        public void DeleteStudent(int studentId)
        {
            var student = _context.Students.Find(studentId);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public void AddAbsence(int studentId, DateOnly date)
        {
            var student = _context.Students.Find(studentId);
            if (student != null)
            {
                student.AddAbsence(date);
                _context.SaveChanges();
            }
        }

        public void AddLuckyNumber(int number, DateOnly date)
        {
            var luckyNumber = new LuckyNumber { Number = number, Date = date };
            _context.LuckyNumbers.Add(luckyNumber);
            _context.SaveChanges();
        }

        public LuckyNumber? GetLuckyNumberByDate(DateOnly date)
        {
            return _context.LuckyNumbers.FirstOrDefault(ln => ln.Date == date);
        }


    }
}
