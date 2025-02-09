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

        [ObservableProperty]
        private Student selectedStudentForRenaming;

        [ObservableProperty]
        private bool isEditFormVisible = false;

        [ObservableProperty]
        private string newStudentNameForEdit;


        public ClassPageViewModel(int selectedClassId)
        {
            Refresh(selectedClassId);
        }


        [RelayCommand]
        public void RenameStudent(Student student)
        {
            if (student == null) return;

            SelectedStudentForRenaming = student;
            NewStudentNameForEdit = student.Name; // Pre-fill with current name
            IsEditFormVisible = true;
        }

        [RelayCommand]
        public void SaveRenamedStudent()
        {
            if (SelectedStudentForRenaming == null || string.IsNullOrWhiteSpace(NewStudentNameForEdit))
                return;

            SelectedStudentForRenaming.Name = NewStudentNameForEdit;
            dbService.UpdateStudent(SelectedStudentForRenaming); // Assuming this method exists

            // Reset fields
            SelectedStudentForRenaming = null;
            NewStudentNameForEdit = string.Empty;
            IsEditFormVisible = false;
           
            
            
            Refresh(SelectedClass.Id);
            AllocateNumbers();

            var prev = SelectedClass;
            SelectedClass = null;
            SelectedClass = prev;
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
                AllocateNumbers();

                NewStudentName = String.Empty;
                IsButtonVisible = !IsButtonVisible;
            }
        }

        [RelayCommand]
        public void StartLottery()
        {
            int max = SelectedClass.Students.Where(s=> s.IsPresentToday == true).Count();

            if (max <= 1)
                return; //ADD NOTIFICATION THAT THERE ARE NOT ENOUGH STUDENTS

            int rePickingFrequency = 3;

            if (max == 4)
                rePickingFrequency = 2;
            if (max == 3)
                rePickingFrequency = 1;
            if (max == 2)
                rePickingFrequency = 0;

            List<Student> possibleStudents = SelectedClass.Students
                                            .Where(s => (s.LastPicked <= SelectedClass.LotteryCount - rePickingFrequency 
                                                        || s.LastPicked == 0) 
                                                        && s.IsPresentToday == true)
                                            .ToList();

            if (possibleStudents.Count == 0)
                return; //ADD NOTIFICATION THAT THERE ARE NO STUDENTS TO PICK


            //LATER CREATIVE ANIMATION
            Random random = new Random();
            int lotteryWinnerId = random.Next(0, possibleStudents.Count);
            LotteryWinner = possibleStudents[lotteryWinnerId];


            SelectedClass.LotteryCount++;
            LotteryWinner.LastPicked = SelectedClass.LotteryCount;
            dbService.UpdateClass(SelectedClass);
        }

        private void AllocateNumbers() 
        {
            foreach (var student in SelectedClass.Students)
            {
                student.Number = SelectedClass.Students.IndexOf(student) + 1;
            }
        }

        private void Refresh(int id)
        {
            SelectedClass = dbService.GetAllClasses().Find(c => c.Id == id)!;
        }

        [RelayCommand]
        public void DeleteStudent(Student student)
        {
            if (student != null)
            {
                dbService.DeleteStudent(student.Id);  // Assuming this method exists
                Refresh(SelectedClass.Id);
                AllocateNumbers();
            }
        }

    }
}
