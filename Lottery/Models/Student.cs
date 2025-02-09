using CommunityToolkit.Mvvm.ComponentModel;
using Lottery.Services;
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
        public List<DateOnly> PresentDays { get; set; } = new List<DateOnly>();
        public int ClassId { get; set; }
        [ObservableProperty]
        public int number;

        public int LastPicked { get; set; }

        public bool IsPresentToday
        {
            get => PresentDays.Contains(DateOnly.FromDateTime(DateTime.Now));
            set
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (value)
                    AddPresence(today);
                else
                    RemovePresence(today);

                OnPropertyChanged(nameof(IsPresentToday));
            }
        }

        public Student(string name, int classId)
        {
            Name = name;
            ClassId = classId;
        }

        public void AddPresence(DateOnly date)
        {
            if (!PresentDays.Contains(date))
            {
                PresentDays.Add(date);
                FileService dbService = new FileService();
                dbService.AddPresence(this.Id, date);
            }
        }

        public void RemovePresence(DateOnly date)
        {
            if (PresentDays.Contains(date))
            {
                PresentDays.Remove(date);
                FileService dbService = new FileService();
                dbService.RemovePresence(this.Id, date);
            }
        }
    }
}
