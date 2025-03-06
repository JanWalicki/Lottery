using Lottery.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lottery.Services
{
    public class FileService
    {
        private string ClassesFile = Path.Combine(FileSystem.AppDataDirectory, "classes.txt");
        private string AbsenceFile = Path.Combine(FileSystem.AppDataDirectory, "presence.txt");
        private string LuckyNumbersFile = Path.Combine(FileSystem.AppDataDirectory, "lucky_numbers.txt");

        public FileService()
        {
            InitializeFiles();
        }

        private void InitializeFiles()
        {
            if (!File.Exists(ClassesFile)) File.Create(ClassesFile).Close();
            if (!File.Exists(AbsenceFile)) File.Create(AbsenceFile).Close();
            if (!File.Exists(LuckyNumbersFile)) File.Create(LuckyNumbersFile).Close();
        }

        public void AddClass(Class classEntity)
        {
            if (classEntity.Id == 0)
            {
                classEntity.Id = GetNextClassId();
            }
            File.AppendAllText(ClassesFile, $"C|{classEntity.Id}|{classEntity.Name}|{classEntity.LotteryCount}\n");
        }

        public void AddStudent(Student student)
        {
            if (student.Id == 0)
            {
                student.Id = GetNextStudentId();
            }
            File.AppendAllText(ClassesFile, $"S|{student.Id}|{student.Name}|{student.ClassId}|{student.Number}|{student.LastPicked}\n");
        }

        public List<Class> GetAllClasses()
        {
            var classes = new Dictionary<int, Class>();
            var students = new List<Student>();

            foreach (var line in File.ReadAllLines(ClassesFile))
            {
                var parts = line.Split('|');
                if (parts[0] == "C")
                {
                    var classEntity = new Class(parts[2])
                    {
                        Id = int.Parse(parts[1]),
                        LotteryCount = int.Parse(parts[3])
                    };
                    classes[classEntity.Id] = classEntity;
                }
                else if (parts[0] == "S")
                {
                    var student = new Student(parts[2], int.Parse(parts[3]))
                    {
                        Id = int.Parse(parts[1]),
                        Number = int.Parse(parts[4]),
                        LastPicked = int.Parse(parts[5])
                    };
                    students.Add(student);
                }
            }

            foreach (var student in students)
            {
                if (classes.TryGetValue(student.ClassId, out var classEntity))
                {
                    classEntity.Students.Add(student);
                }
            }

            LoadAbsences(classes.Values.ToList());
            return classes.Values.ToList();
        }

        private void LoadAbsences(List<Class> classes)
        {
            var absences = new Dictionary<int, List<DateOnly>>();
            foreach (var line in File.ReadAllLines(AbsenceFile))
            {
                var parts = line.Split('|');
                var studentId = int.Parse(parts[0]);
                var date = DateOnly.Parse(parts[1]);

                if (!absences.ContainsKey(studentId))
                    absences[studentId] = new List<DateOnly>();

                absences[studentId].Add(date);
            }

            foreach (var classEntity in classes)
            {
                foreach (var student in classEntity.Students)
                {
                    if (absences.TryGetValue(student.Id, out var dates))
                    {
                        student.PresentDays.AddRange(dates);
                    }
                }
            }
        }

        public List<Student> GetAllStudents()
        {
            return GetAllClasses()
                .SelectMany(c => c.Students)
                .ToList();
        }

        public void UpdateStudent(Student student)
        {
            var lines = File.ReadAllLines(ClassesFile).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split('|');
                if (parts[0] == "S" && int.Parse(parts[1]) == student.Id)
                {
                    lines[i] = $"S|{student.Id}|{student.Name}|{student.ClassId}|{student.Number}|{student.LastPicked}";
                    break;
                }
            }
            File.WriteAllLines(ClassesFile, lines);
        }

        public void UpdateClass(Class classEntity)
        {
            var lines = File.ReadAllLines(ClassesFile).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split('|');
                if (parts[0] == "C" && int.Parse(parts[1]) == classEntity.Id)
                {
                    lines[i] = $"C|{classEntity.Id}|{classEntity.Name}|{classEntity.LotteryCount}";
                    break;
                }
            }
            File.WriteAllLines(ClassesFile, lines);

            foreach (var student in classEntity.Students)
                UpdateStudent(student);
        }

        public void DeleteStudent(int studentId)
        {
            var classLines = File.ReadAllLines(ClassesFile)
                .Where(line => !(line.StartsWith("S|") && int.Parse(line.Split('|')[1]) == studentId))
                .ToArray();
            File.WriteAllLines(ClassesFile, classLines);

            if (File.Exists(AbsenceFile))
            {
                var absenceLines = File.ReadAllLines(AbsenceFile)
                    .Where(line => int.Parse(line.Split('|')[0]) != studentId)
                    .ToArray();
                File.WriteAllLines(AbsenceFile, absenceLines);
            }
        }

        public void AddPresence(int studentId, DateOnly date)
        {
            File.AppendAllText(AbsenceFile, $"{studentId}|{date}\n");
        }

        public void RemovePresence(int studentId, DateOnly date)
        {
            var lines = File.ReadAllLines(AbsenceFile).ToList();
            string recordToRemove = $"{studentId}|{date}";
            lines = lines.Where(line => !line.StartsWith(recordToRemove)).ToList();
            File.WriteAllLines(AbsenceFile, lines);
        }

        public void AddLuckyNumber(int number, DateOnly date)
        {
            File.AppendAllText(LuckyNumbersFile, $"{number}|{date}\n");
        }

        public LuckyNumber? GetLuckyNumberByDate(DateOnly date)
        {
            foreach (var line in File.ReadAllLines(LuckyNumbersFile))
            {
                var parts = line.Split('|');
                var currentDate = DateOnly.Parse(parts[1]);
                if (currentDate == date)
                {
                    return new LuckyNumber(int.Parse(parts[0]), currentDate);
                }
            }
            return null;
        }

        private int GetNextClassId()
        {
            var classes = GetAllClasses();
            return classes.Count > 0 ? classes.Max(c => c.Id) + 1 : 1;
        }

        private int GetNextStudentId()
        {
            var students = GetAllStudents();
            return students.Count > 0 ? students.Max(s => s.Id) + 1 : 1;
        }
    }
}