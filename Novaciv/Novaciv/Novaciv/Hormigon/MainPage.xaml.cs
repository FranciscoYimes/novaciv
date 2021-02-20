using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Novaciv.Hormigon
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        void Losa_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Losa(), true);
        }

        void Viga_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Viga(), true);
        }
    }
}
