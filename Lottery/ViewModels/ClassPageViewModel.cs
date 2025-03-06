using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Models;
using Lottery.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Lottery.ViewModels
{
    public partial class ClassPageViewModel : ObservableObject
    {
        FileService dbService = new FileService();
        LuckyNumberService luckyNumService = new LuckyNumberService();

        [ObservableProperty]
        public Class selectedClass = new Class("CLASS NOT FOUND");



        [ObservableProperty]
        public bool isFormVisible = false;

        [ObservableProperty]
        public bool isButtonVisible = true;

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

        private int luckyNumber;
        public ClassPageViewModel(int selectedClassId)
        {
            Refresh(selectedClassId);
        }
        private void UpdateLuckyNumber()
        {
            luckyNumber = luckyNumService.GetLuckyNumber();
        }

        [RelayCommand]
        public void RenameStudent(Student student)
        {
            if (student == null) return;

            SelectedStudentForRenaming = student;
            NewStudentNameForEdit = student.Name;
            IsEditFormVisible = true;
        }

        [RelayCommand]
        public async Task SaveRenamedStudent()
        {
            if (SelectedStudentForRenaming == null || string.IsNullOrWhiteSpace(NewStudentNameForEdit) || !Regex.IsMatch(NewStudentNameForEdit, @"^[A-Za-z0-9ęółśążźćńĘÓŁŚĄŻŹĆŃ\s]+$"))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz Nazwisko i Imię ucznia", "OK");
                return;
            }

            SelectedStudentForRenaming.Name = NewStudentNameForEdit;
            dbService.UpdateStudent(SelectedStudentForRenaming);

            // Reset fields
            SelectedStudentForRenaming = null;
            NewStudentNameForEdit = string.Empty;
            IsEditFormVisible = false;



            Refresh(SelectedClass.Id);


            var prev = SelectedClass;
            SelectedClass = null;
            SelectedClass = prev;
        }

        [RelayCommand]
        public async Task AddStudent()
        {
            if (IsButtonVisible)
            {
                IsButtonVisible = !IsButtonVisible;
                IsFormVisible = !IsFormVisible;
                return;
            }


            if (!string.IsNullOrEmpty(NewStudentName))
            {
                if (Regex.IsMatch(NewStudentName, @"^[A-Za-z0-9ęółśążźćńĘÓŁŚĄŻŹĆŃ\s]+$"))
                {
                    dbService.AddStudent(new Student(NewStudentName, SelectedClass.Id));
                    Refresh(SelectedClass.Id);

                    NewStudentName = String.Empty;
                    IsButtonVisible = !IsButtonVisible;
                    IsFormVisible = !IsFormVisible;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Dozwolone są tylko litery i cyfry", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz Nazwisko i Imię ucznia", "OK");
            }
        }

        [RelayCommand]
        public async Task StartLottery()
        {
            UpdateLuckyNumber();

            int max = SelectedClass.Students.Where(s => s.IsPresentToday == true && s.Number != luckyNumber).Count();

            if (max <= 1)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Zbyt mało uczniów aby przeprowadzić losowanie", "OK");
                return;
            }

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
                                                        && s.IsPresentToday == true
                                                        && s.Number != luckyNumber)
                                            .ToList();

            if (possibleStudents.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Zbyt mało uczniów aby przeprowadzić losowanie", "OK");
                return;
            }

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
            Class dbClass = dbService.GetAllClasses().Find(c => c.Id == id)!;
            dbClass.Students = new ObservableCollection<Student>(dbClass.Students.OrderBy(s => s.Name).ToList());
            SelectedClass = dbClass;
            AllocateNumbers();
        }

        [RelayCommand]
        public void DeleteStudent(Student student)
        {
            if (student != null)
            {
                dbService.DeleteStudent(student.Id);
                Refresh(SelectedClass.Id);
            }
        }

        [RelayCommand]
        public static async Task GoBack()
        {
            Shell.Current.Items.Clear();
            Application.Current!.MainPage = new AppShell();
            await Shell.Current.GoToAsync("//mainPage");
        }

    }
}
