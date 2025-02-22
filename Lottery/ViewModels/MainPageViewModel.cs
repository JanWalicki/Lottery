﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        FileService dbService = new FileService();

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
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            LuckyNumber dbLuckyNumber = dbService.GetLuckyNumberByDate(today);

            if (dbLuckyNumber == null && LuckyNumber == 0)
                luckyNumber = GenerateLuckyNumber(today);
            else
                luckyNumber = dbLuckyNumber.Number;
        }

        private int GenerateLuckyNumber(DateOnly today)
        {
            if(Classes.Count <= 0)
                return 0;

            int max = Classes.Max(c=>c.Students.Count);
            if(max <= 0) return 0;
            Random random = new Random();
            int luckyNumber = random.Next(1, max);

            dbService.AddLuckyNumber(luckyNumber, today);
            return luckyNumber;
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


            if (!string.IsNullOrEmpty(NewClassName))
            {
                dbService.AddClass(new Class(NewClassName));
                Refresh();

                NewClassName = String.Empty;
                IsButtonVisible = !IsButtonVisible;
                IsFormVisible = !IsFormVisible;
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
