using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//My using block
using System.Data.SqlClient;
using LadaService.Pages;

namespace LadaService.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestDrivePage : ContentPage
    {
        StackLayout stcklt = new StackLayout { Padding = new Thickness(25, 0) };
        StackLayout stcklt2 = new StackLayout { Orientation = StackOrientation.Horizontal,
                                                HorizontalOptions = LayoutOptions.FillAndExpand};
        int idAuto;
        string MyConStr, userName, comments, data, time;
        DateTime dataRecord = new DateTime();

        public TestDrivePage(int selIdAuto, string selTtl, string selDscr, string mainConStr)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
            ttl.Text = selTtl;
            descr.Text = selDscr;
            MyConStr = mainConStr;
            idAuto = selIdAuto;

            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                //Картинка
                string SQLQuery = $"Select FK_IdImage From Autos Where (IdAuto = {idAuto})";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                int idImage = Convert.ToInt32(cmd.ExecuteScalar());
                SQLQuery = $"Select ImageURL From Images Where (IdImage = {idImage})";
                cmd.CommandText = SQLQuery;
                string imageSource = Convert.ToString(cmd.ExecuteScalar());
                imageCar.Source = imageSource;

                con.Close();
            }

            Entry userNameEntry = new Entry
            {
                TextColor = Color.FromHex("#FFFFFF"),
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Button)),
                Placeholder = "Введите ФИО",
                PlaceholderColor = Color.LightGray,
            };
            userNameEntry.TextChanged += UserNameEntry_TextChanged;

            TimePicker userTimePicker = new TimePicker
            {
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(TimePicker)),
                TextColor = Color.FromHex("#FFFFFF")
            };
            userTimePicker.PropertyChanged += UserTimePicker_PropertyChanged;

            DatePicker userDatePicker = new DatePicker
            {
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(DatePicker)),
                TextColor = Color.FromHex("#FFFFFF"),
                MinimumDate = DateTime.Now
            };
            userDatePicker.PropertyChanged += UserDatePicker_PropertyChanged;

            Label lbl = new Label
            {
                Text = "Выберите удобное для Вас время:",
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                TextColor = Color.FromHex("#FFFFFF"),
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Editor userEditor = new Editor
            {
                Placeholder = "Если у Вас остались какие-либо вопросы или пожелания, Вы можете написать их здесь.\n" +
                "Важно!\n" +
                "Если вы хотите получить ответ, укажите свою почту в этом блоке.",
                PlaceholderColor = Color.LightGray,
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Editor)),
                TextColor = Color.FromHex("#FFFFFF"),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                AutoSize = EditorAutoSizeOption.TextChanges,
                Keyboard = Keyboard.Default
            };
            userEditor.Keyboard = Keyboard.Create(KeyboardFlags.All);
            userEditor.TextChanged += UserEditor_TextChanged;

            Button bttn = new Button
            {
                //Text
                TextColor = Color.FromHex("#FFFFFF"),
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                Text = "Оставить заявку",

                //Button
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BorderWidth = 1,
                BorderColor = Color.FromHex("#775F92"),
                BackgroundColor = Color.FromHex("#200F39")
            };
            bttn.Clicked += Bttn_Clicked;

            stcklt2.Children.Add(lbl);
            stcklt2.Children.Add(userDatePicker);
            stcklt2.Children.Add(userTimePicker);
            stcklt.Children.Add(userNameEntry);
            stcklt.Children.Add(stcklt2);
            stcklt.Children.Add(userEditor);
            stcklt.Children.Add(bttn);

            scrlvw1.Content = stcklt;

        }

        private void UserDatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            data = "";
            data = ((DatePicker)sender).Date.ToString("yyyy/MM/dd");
        }

        private void UserTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            time = "";
            time = ((TimePicker)sender).Time.ToString();
        }

        private void UserNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            userName = "";
            userName = ((Entry)sender).Text;
        }

        private void UserEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            comments = "";
            comments = ((Editor)sender).Text;
        }

        private void Bttn_Clicked(object sender, EventArgs e)
        {
            dataRecord = Convert.ToDateTime(data + " " + time);
            try
            {
                if (userName != "")
                {
                    using (SqlConnection con = new SqlConnection(MyConStr))
                    {
                        con.Open();

                        string SQLQuery = $"INSERT INTO Records(Username, DayRecord, Comments, FK_IdAuto) " +
                            $"Values('{userName}', '{dataRecord}', '{comments}', {idAuto})";
                        SqlCommand cmd = new SqlCommand(SQLQuery, con);
                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                    DisplayAlert("Готово", $"Ваша заявка отправлена. Будем ждать вас {dataRecord} по какому-то адресу.", "ОК");
                }
                else
                {
                    DisplayAlert("Ошибка","Пожалюуйста, укажите ФИО", "Ок");
                }
            }
            catch
            {
                DisplayAlert("Ошибка", "Хм, похоже, что сервер упал.\nВы будете возвращены на главную страницу.", "Ок");
                MainPage mainPage = new MainPage();
                Navigation.PopToRootAsync();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Close();
                Navigation.PopAsync();
            }
        }
    }
}