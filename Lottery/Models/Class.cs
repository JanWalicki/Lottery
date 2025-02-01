using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Models
{

    public partial class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Student> Students { get; set; } = new();
        public int LotteryCount { get; set; }

        public Class(string name)
        {
            Name = name;
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }
    }
}
