﻿using Microsoft.Maui.Controls.StyleSheets;

namespace Lottery
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
