using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Novaciv
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Hormigon.MainPage());
            //else MainPage = new Hormigon.MainPage();
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
