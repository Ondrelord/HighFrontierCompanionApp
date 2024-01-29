using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLitePCL;
using SQLite;
using System;
using System.IO;

namespace HFCA
{
    public partial class App : Application
    {
        static CardDatabase database;
        public static CardDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CardDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CardsDatabase.db3"));
                }
                return database;
            }
        }


        public App()
        {
            InitializeComponent();
            
            MainPage = new NavigationPage(new Page1());
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
