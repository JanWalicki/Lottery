using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Models
{
    public partial class Student : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Present { get; set; } = true;
        public List<DateOnly> AbsenceDays { get; set; } = new List<DateOnly>();
        public int ClassId { get; set; }

        public int LastPicked { get; set; }

        public Student(string name, int classId)
        {
            Name = name;
            ClassId = classId;
        }

        public void AddAbsence(DateOnly date)
        {
            AbsenceDays.Add(date);
        }
    }
}
