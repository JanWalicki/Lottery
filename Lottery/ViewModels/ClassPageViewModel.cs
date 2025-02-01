using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Models;
using Lottery.Services;

namespace Lottery.ViewModels
{
    public partial class ClassPageViewModel : ObservableObject
    {
        DatabaseService dbService = new DatabaseService();

        [ObservableProperty]
        public Class selectedClass = new Class("CLASS NOT FOUND");



        [ObservableProperty]
        public bool isFormVisible = false;
        public bool IsButtonVisible
        {
            get { return !IsFormVisible; }
            set { IsFormVisible = !value; }
        }

        [ObservableProperty]
        public string newStudentName = String.Empty;

        [ObservableProperty]
        public Student lotteryWinner = new Student("", 0);

        public ClassPageViewModel(int selectedClassId)
        {
            SelectedClass = dbService.GetAllClasses().Find(c => c.Id == selectedClassId)!;
        }

        [RelayCommand]
        public void AddStudent()
        {
            if (IsButtonVisible)
                IsButtonVisible = !IsButtonVisible;


            if (!string.IsNullOrEmpty(NewStudentName))
            {
                dbService.AddStudent(new Student(NewStudentName, SelectedClass.Id));
                Refresh(SelectedClass.Id);

                NewStudentName = String.Empty;
                IsButtonVisible = !IsButtonVisible;
            }
        }


        [RelayCommand]
        public void StartLottery()
        {
            int max = SelectedClass.Students.Count;

            if (max <= 1)
                return;

            int rePickingFrequency = 3;

            if (max == 4)
                rePickingFrequency = 2;

            List<Student> possibleStudents = SelectedClass.Students
                                            .Where(s => s.LastPicked <= SelectedClass.LotteryCount - rePickingFrequency || s.LastPicked == 0)
                                            .ToList();

            if (possibleStudents.Count == 0)
                return;

            Random random = new Random();
            int lotteryWinnerId = random.Next(0, possibleStudents.Count);
            LotteryWinner = possibleStudents[lotteryWinnerId];


            SelectedClass.LotteryCount++;
            LotteryWinner.LastPicked = SelectedClass.LotteryCount;
            dbService.UpdateClass(SelectedClass);
        }


        private void Refresh(int id)
        {
            SelectedClass = dbService.GetAllClasses().Find(c => c.Id == id)!;
        }
    }
}
