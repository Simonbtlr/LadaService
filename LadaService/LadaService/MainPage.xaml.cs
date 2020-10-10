using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// My using block
using System.Data.SqlClient;
using LadaService.Pages;

namespace LadaService
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //My block
        public string MyStrConUser = @"Server = tcp:Simon-PC,1433; " +
                                      "Persist Security Info = False; " +
                                      "Initial Catalog = LadaService; " +
                                      "Timeout = 5; ";
        public bool adminLogin = false;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (adminToggle.IsToggled == true)
            {
                MyStrConUser += $"User ID = {userEntry.Text}; Password = {passEntry.Text}";
                adminLogin = true;
            }
            else
            {
                MyStrConUser += "User ID = User; Password = 123456";
                adminLogin = false;
            }
            try
            {
                //Проверяем соединение с сервером
                using (SqlConnection con = new SqlConnection(MyStrConUser))
                {
                    con.Open();
                    con.Close();
                }
                BrandPage brandPage = new BrandPage(MyStrConUser, adminLogin);
                await Navigation.PushAsync(brandPage, false);
            }
            catch
            {
                await DisplayAlert("Ошибка", "Не удалось подключиться к серверу", "Отмена");
            }
        }

        private async void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (adminToggle.IsToggled == true)
            {
                bool disAlrt = await DisplayAlert("Внимание",
                   "Желаете подключиться с правами администратора? \n" +
                   "Вам потребуется ввести логин и пароль.",
                   "Войти как Администратор",
                   "Отмена");
                if (disAlrt == true)
                {
                    adminToggle.IsToggled = true;
                    passEntry.IsVisible = true;
                    userEntry.IsVisible = true;
                }
                else
                {
                    adminToggle.IsToggled = false;
                    passEntry.IsVisible = false;
                    userEntry.IsVisible = false;
                }
            }
            else
            {
                passEntry.IsVisible = false;
                userEntry.IsVisible = false;
            }
        }

        private void userEntry_Completed(object sender, EventArgs e)
        {
            passEntry.Focus();
        }

        private void passEntry_Completed(object sender, EventArgs e)
        {
            Button_Clicked(sender, e);
        }
    }
}