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

namespace Lottery.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        DatabaseService dbService = new DatabaseService();

        [ObservableProperty]
        ObservableCollection<Class> classes = new();


        [ObservableProperty]
        public bool isFormVisible = false;

        public bool IsButtonVisible 
        { 
            get { return !IsFormVisible; } 
            set { IsFormVisible = !value; } 
        }

        [ObservableProperty]
        public string newClassName = String.Empty;

        public MainPageViewModel()
        {
            Classes = new ObservableCollection<Class>(dbService.GetAllClasses());
        }

        [RelayCommand]
        public void AddClass()
        {
            if (IsButtonVisible)
                IsButtonVisible = !IsButtonVisible;


            if (!string.IsNullOrEmpty(NewClassName))
            {
                dbService.AddClass(new Class(NewClassName));
                Refresh();

                NewClassName = String.Empty;
                IsButtonVisible = !IsButtonVisible;
            }

        }

        private void Refresh()
        {
            Classes = new ObservableCollection<Class>(dbService.GetAllClasses());
        }

        [RelayCommand]
        private static async Task OnClassSelected(Class selectedClass)
        {
            await Shell.Current.GoToAsync($"///classPage?classId={selectedClass.Id}");
        }
    }
}
