using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Models;
using Lottery.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lottery.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        FileService dbService = new FileService();
        LuckyNumberService luckyNumService = new LuckyNumberService();

        [ObservableProperty]
        ObservableCollection<Class> classes = new();


        [ObservableProperty]
        public bool isFormVisible = false;

        [ObservableProperty]
        public bool isButtonVisible = true;

        [ObservableProperty]
        public string newClassName = String.Empty;


        [ObservableProperty]
        public int luckyNumber;

        public MainPageViewModel()
        {
            Refresh();

        }

        private void UpdateLuckyNumber()
        {
            LuckyNumber = luckyNumService.GetLuckyNumber();
        }

        [RelayCommand]
        public async Task AddClass()
        {
            if (IsButtonVisible)
            {
                IsButtonVisible = !IsButtonVisible;
                IsFormVisible = !IsFormVisible;
                return;
            }


            if (!string.IsNullOrWhiteSpace(NewClassName))
            {
                if (Regex.IsMatch(NewClassName, @"^[A-Za-z0-9\s]+$"))
                {
                    dbService.AddClass(new Class(NewClassName));
                    Refresh();

                    NewClassName = String.Empty;
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
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz oznaczenie klasy", "OK");
            }

        }

        private void Refresh()
        {
            Classes = new ObservableCollection<Class>(dbService.GetAllClasses().OrderBy(c=>c.Name).ToList());
            UpdateLuckyNumber();
        }

        [RelayCommand]
        private static async Task OnClassSelected(Class selectedClass)
        {
            await Shell.Current.GoToAsync($"///classPage?classId={selectedClass.Id}");
        }
    }
}
