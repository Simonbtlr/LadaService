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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrandPage : ContentPage
    {
        //My block
        string MyConStr;
        StackLayout stcklt1 = new StackLayout();
        bool adminLogin;

        public BrandPage(string mainConStr, bool adm)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            MyConStr = mainConStr;
            adminLogin = adm;

            //Смотрю какие есть бренды
            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                //Запрос
                string SQLQuery = $"Select Count(Distinct(BrandName)) From Brands";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                int countBrandName = Convert.ToInt32(cmd.ExecuteScalar());

                string[] brandName = new string[countBrandName];
                string brandNameImage = "";

                //Создаём сетку
                Grid grd1 = new Grid
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                //Пропорции сетки
                int clm = 2, rw;
                if (countBrandName % clm != 0)
                {
                    rw = countBrandName / clm + 1;
                }
                else
                {
                    rw = countBrandName / clm;
                }

                //Размеры сеточки
                for (int j = 0; j < rw; j++)
                {
                    grd1.RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition
                            {
                                Height = new GridLength(1, GridUnitType.Star)
                            }
                        };
                    grd1.VerticalOptions = LayoutOptions.CenterAndExpand;
                    for (int z = 0; z < clm; z++)
                    {
                        grd1.ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star)
                            }
                        };
                        grd1.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    }
                }
                int i = 0;
                SQLQuery = $"Select Distinct(BrandName) From Brands Where (IdBrand > 0)";
                //Добовляем кнопку с машиной
                for (int j = 0; j < rw; j++)
                {
                    if (j == rw - 1 && countBrandName % 2 == 1)
                    {
                        clm = 1;
                    }
                    
                    for (int z = 0; z < clm; z++)
                    {
                        //Имя бренда
                        cmd.CommandText = SQLQuery;
                        brandName[i] = Convert.ToString(cmd.ExecuteScalar());

                        //Картинка
                        string SQLQuery1 = $"Select Distinct BrandNameImage From Brands Where (BrandName = '{brandName[i]}') And(BrandNameImage IS NOT NULL)";
                        cmd.CommandText = SQLQuery1;
                        brandNameImage = Convert.ToString(cmd.ExecuteScalar());

                        //Чтобы не повторялись бренды

                        for (int c = 0; c <= i; c++)
                        {
                            SQLQuery += $" And(BrandName <> '{brandName[i]}')";
                        }

                        Button bttn = new Button
                        {
                            //Текст
                            Text = brandName[i],
                            TextColor = Color.FromHex("#FFFFFF"),
                            FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),

                            //Картинка
                            ImageSource = brandNameImage,
                            ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 5),

                            //Кнопка
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BorderWidth = 1,
                            BackgroundColor = Color.FromHex("#200F39"),
                            BorderColor = Color.FromHex("#775F92"),
                            Margin = new Thickness(10, 4.5, 10, 4.5),
                        };
                        bttn.Clicked += Bttn_Clicked;
                        grd1.Children.Add(bttn, z, j);
                        i++;
                    }
                    
                }
                stcklt1.Children.Add(grd1);
                scrlvw1.Content = stcklt1;
                if (adminLogin == true)
                {
                    AdminButton();
                }
                con.Close();
            }
        }

        private void Bttn_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();
                    con.Close();
                }
                MenuPage menuPage = new MenuPage(((Button)sender).Text, MyConStr, adminLogin);
                Navigation.PushAsync(menuPage, true);
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

        private void AdminButton()
        {
            Button bttn = new Button
            {
                //Текст
                Text = "Добавить бренд",
                TextColor = Color.FromHex("#FFFFFF"),
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),

                //Кнопка
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BorderWidth = 1,
                BackgroundColor = Color.FromHex("#200F39"),
                BorderColor = Color.FromHex("#775F92"),
                Margin = new Thickness(10, 4.5, 10, 4.5)
            };
            bttn.Clicked += Bttn_Clicked;

            Button bttn1 = new Button
            {
                //Текст
                Text = "Просмотреть записи на Тест-Драйв",
                TextColor = Color.FromHex("#FFFFFF"),
                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),

                //Кнопка
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BorderWidth = 1,
                BackgroundColor = Color.FromHex("#200F39"),
                BorderColor = Color.FromHex("#775F92"),
                Margin = new Thickness(10, 4.5, 10, 4.5)
            };
            bttn1.Clicked += Bttn1_Clicked;

            stcklt1.Children.Add(bttn);
            stcklt1.Children.Add(bttn1);
            scrlvw1.Content = stcklt1;
        }

        private void Bttn1_Clicked(object sender, EventArgs e)
        {
            stcklt1.Children.Clear();

            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                string SQLQuery = $"Select Count(idRecord) From Records";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                string[] username = new string[Convert.ToInt32(cmd.ExecuteScalar())];

                StackLayout stcklt2 = new StackLayout
                {
                    BackgroundColor = Color.FromHex("#454545"),
                    Margin = new Thickness(0, 2.5),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(2.5, 0)
                };

                for (int i = 0; i <= username.Length - 1; i++)
                {

                    SQLQuery = $"Select Username From Records Where (idRecord = {i + 1})";
                    cmd.CommandText = SQLQuery;
                    username[i] = Convert.ToString(cmd.ExecuteScalar());

                    SQLQuery = $"Select DayRecord From Records Where (idRecord = {i + 1})";
                    cmd.CommandText = SQLQuery;
                    string dataTime = Convert.ToString(cmd.ExecuteScalar());

                    Label lbl = new Label
                    {
                        Text = username[i] + " " + dataTime,
                        TextColor = Color.FromHex("#FFFFFF"),
                        FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                        FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                    };

                    stcklt2.Children.Add(lbl);
                    stcklt1.Children.Add(stcklt2);
                }
                scrlvw1.Content = stcklt1;

                con.Close();
            }
        }
    }
}