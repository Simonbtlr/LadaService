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
    public partial class MenuPage : ContentPage
    {
        //My block
        string MyConStr, brandName, newBrandName;
        StackLayout stcklt1 = new StackLayout();
        bool adminLogin;

        public MenuPage(string selectedBrand, string mainConStr, bool adm)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if (selectedBrand != "Добавить бренд")
            {
                //Данные с другой формы
                MyConStr = mainConStr;
                brandName = selectedBrand;
                ttl.Text = brandName;
                adminLogin = adm;

                //Смотрим модельный ряд выбранного бренда
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    //Запрос
                    string SQLQuery = $"Select Count(IdBrand) From Brands Where (BrandName = '{brandName}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    int countLineup = Convert.ToInt32(cmd.ExecuteScalar());

                    //Начало расположения бренда
                    SQLQuery = $"Select Min(IdBrand) From Brands Where (BrandName = '{brandName}')";
                    cmd.CommandText = SQLQuery;
                    int startIdLineup = Convert.ToInt32(cmd.ExecuteScalar());

                    //Первая модель бренда
                    int[] idLineup = new int[countLineup];
                    idLineup[0] = startIdLineup;
                    SQLQuery = $"Select Lineup From Brands Where (BrandName = '{brandName}') AND(IdBrand = {startIdLineup})";
                    string[] lineupName = new string[countLineup];
                    cmd.CommandText = SQLQuery;
                    lineupName[0] = Convert.ToString(cmd.ExecuteScalar());

                    if (lineupName[0] != "")
                    {
                        //Конец расположения бренда
                        SQLQuery = $"Select Max(IdBrand) From Brands Where (BrandName = '{brandName}')";
                        cmd.CommandText = SQLQuery;
                        int endIdLineup = Convert.ToInt32(cmd.ExecuteScalar());

                        SQLQuery = $"Select Min(IdBrand) " +
                                   $"From Brands " +
                                   $"Where(BrandName = '{brandName}') " +
                                   $"And(Lineup <> '{lineupName[0]}')";
                        for (int i = 1; i < countLineup; i++)
                        {
                            //id модельного ряда
                            cmd.CommandText = SQLQuery;
                            idLineup[i] = Convert.ToInt32(cmd.ExecuteScalar());
                            //Название модельного ряда
                            string SQLQuery1 = $"Select Lineup " +
                                               $"From Brands " +
                                               $"Where BrandName = '{brandName}'" +
                                               $"And(IdBrand = {idLineup[i]})";
                            cmd.CommandText = SQLQuery1;
                            lineupName[i] = Convert.ToString(cmd.ExecuteScalar());

                            SQLQuery += $"And(Lineup <> '{lineupName[i]}')";
                        }
                        //Создаю кнопки на каждую модель
                        for (int i = 0; i < countLineup; i++)
                        {
                            //Картинка
                            string imgbttn;
                            SQLQuery = $"Select LineupImage From Brands Where (IdBrand = {idLineup[i]})";
                            cmd.CommandText = SQLQuery;
                            imgbttn = Convert.ToString(cmd.ExecuteScalar());

                            Button bttn = new Button
                            {
                                //Text
                                TextColor = Color.FromHex("#FFFFFF"),
                                FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                                Text = lineupName[i],
                                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),

                                //Image
                                ImageSource = imgbttn,
                                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 0),

                                //Button
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BorderWidth = 1,
                                BorderColor = Color.FromHex("#775F92"),
                                BackgroundColor = Color.FromHex("#200F39"),
                                Margin = new Thickness(10, 4.5)
                            };
                            bttn.Clicked += Bttn_Clicked;
                            stcklt1.Children.Add(bttn);
                        }
                    }
                    else
                    {
                        Label label = new Label
                        {
                            Text = "Здесь пусто :)",
                            TextColor = Color.FromHex("#FFFFFF"),
                            FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                            FontSize = Device.GetNamedSize(NamedSize.Caption, typeof(Label))
                        };
                        stcklt1.HorizontalOptions = LayoutOptions.Center;
                        stcklt1.Children.Add(label);
                    }
                    if (adminLogin == true)
                    {
                        AdminButton();
                    }
                    scrlvw1.Content = stcklt1;
                    con.Close();
                }
            }
            else
            {
                MyConStr = mainConStr;
                ttl.Text = "Добавление нового бренда";
                descr.Text = "Внесите все данные";
                
                Entry brandNameEntry = new Entry
                {
                    Placeholder = "Введите имя бренда",
                    TextColor = Color.FromHex("#FFFFFF"),
                    PlaceholderColor = Color.FromHex("FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    Margin = new Thickness(50)
                };
                brandNameEntry.Completed += BrandNameEntry_Completed;
                brandNameEntry.TextChanged += BrandNameEntry_TextChanged;

                Button bttn = new Button
                {
                    //Text
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                    Text = "Добавить",

                    //Button
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BorderWidth = 1,
                    BorderColor = Color.FromHex("#775F92"),
                    BackgroundColor = Color.FromHex("#200F39"),
                    Margin = new Thickness(10, 4.5)
                };
                bttn.Clicked += BrandNameEntry_Completed;

                stcklt1.Children.Add(brandNameEntry);
                stcklt1.Children.Add(bttn);


                scrlvw1.Content = stcklt1;
            }
        }

        //Дополнительная кнопка для Админимстратора
        private void AdminButton()
        {
            Button bttn = new Button
            {
                //Текст
                Text = "Добавить модельный ряд",
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
            stcklt1.Children.Add(bttn);
            scrlvw1.Content = stcklt1;
        }

        //Название бренда
        private void BrandNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            newBrandName = ((Entry)sender).Text;
        }

        //Добавление бренда
        private void BrandNameEntry_Completed(object sender, EventArgs e)
        {
            string SQLQuery = $"Insert Into Brands (BrandName) Values ('{newBrandName}')";
            try
            {
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
                Navigation.PopAsync();
            }
            catch
            {
                DisplayAlert("Ошибка", "Не удалось добавить бренд", "Назад");
            }
        }

        //Нажатие на кнопку выбора
        private void Bttn_Clicked(object sender, EventArgs e)
        {
            //Проверяем есть ли связь с сервером
            try
            {
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();
                    con.Close();
                }
                ModelPage modelPage = new ModelPage(brandName, ((Button)sender).Text, MyConStr, adminLogin);
                Navigation.PushAsync(modelPage, true);
            }
            catch
            {
                DisplayAlert("Ошибка", "Хм, похоже, что сервер упал.\nВы будете возвращены на главную страницу.", "Ок");
                MainPage mainPage = new MainPage();
                Navigation.PopToRootAsync();
            }
        }

        //Возвращение назад
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