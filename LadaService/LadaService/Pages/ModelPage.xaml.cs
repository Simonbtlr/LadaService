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

namespace LadaService.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModelPage : ContentPage
    {
        //My block
        string MyConStr, brandName, lineup, newLineup;
        StackLayout stcklt1 = new StackLayout();
        bool adminLogin;

        public ModelPage(string selectedBrandName, string selectedLineup, string mainConStr, bool adm)
        {
            MyConStr = mainConStr;
            brandName = selectedBrandName;
            lineup = selectedLineup;
            adminLogin = adm;

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if(selectedLineup != "Добавить модельный ряд")
            {
                ttle.Text = lineup;
                descr.Text = "Последний шаг. Выберите тип кузова";

                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();
                    //Находим IdBrand по полученным данным
                    string SQLQuery = $"Select IdBrand From Brands Where (BrandName = '{brandName}') And (Lineup = '{lineup}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    int idBrand = Convert.ToInt32(cmd.ExecuteScalar());

                    //Узнаем сколько моделей кузова у IdBrand
                    SQLQuery = $"Select Count(FK_IdModel) From Images Where (FK_IdBrand = {idBrand})";
                    cmd.CommandText = SQLQuery;
                    int[] idModel = new int[Convert.ToInt32(cmd.ExecuteScalar())];

                    //Массив в котором все модели кузова IdBrand
                    SQLQuery = $"Select Min(FK_IdModel) From Images Where (FK_IdBrand = {idBrand})";
                    for (int i = 0; i < idModel.Length; i++)
                    {
                        cmd.CommandText = SQLQuery;
                        idModel[i] = Convert.ToInt32(cmd.ExecuteScalar());
                        SQLQuery += $" And (FK_IdModel <> {idModel[i]})";
                    }

                    //Создаём массив modelName
                    string[] modelName = new string[idModel.Length];
                    for (int i = 0; i < modelName.Length; i++)
                    {
                        SQLQuery = $"Select ModelName From Models Where(IdModel = {idModel[i]})";
                        cmd.CommandText = SQLQuery;
                        modelName[i] = Convert.ToString(cmd.ExecuteScalar());
                    }

                    //Создаём кнопки
                    for (int i = 0; i < modelName.Length; i++)
                    {
                        //Находим картинку
                        SQLQuery = $"Select ImageURL From Images Where (FK_IdBrand = {idBrand}) And (FK_IdModel = {idModel[i]})";
                        cmd.CommandText = SQLQuery;
                        string imgbttn = Convert.ToString(cmd.ExecuteScalar());

                        Button bttn = new Button
                        {
                            //Text
                            TextColor = Color.FromHex("#FFFFFF"),
                            FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                            Text = modelName[i],
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
                        bttn.Clicked += Bttn_Clicked; ;
                        stcklt1.Children.Add(bttn);
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
                ttle.Text = "Добавление нового модельного ряда";
                descr.Text = "Внесите все данные";

                Entry newLineupEntry = new Entry
                {
                    Placeholder = "Введите имя модельного ряда",
                    TextColor = Color.FromHex("#FFFFFF"),
                    PlaceholderColor = Color.FromHex("FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    Margin = new Thickness(50)
                };
                newLineupEntry.Completed += BrandNameEntry_Completed;
                newLineupEntry.TextChanged += BrandNameEntry_TextChanged;

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

                stcklt1.Children.Add(newLineupEntry);
                stcklt1.Children.Add(bttn);


                scrlvw1.Content = stcklt1;
            
            }
        }

        private void AdminButton()
        {
            Button bttn = new Button
            {
                //Текст
                Text = "Добавить тип кузова",
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

        private void BrandNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            newLineup = ((Entry)sender).Text;
        }

        private void BrandNameEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    string SQLQuery = $"Select Min(IdBrand) From Brands Where (BrandName = '{brandName}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    int idBrand = Convert.ToInt32(cmd.ExecuteScalar());

                    SQLQuery = $"Select Lineup From Brands Where (IdBrand = {idBrand})";
                    cmd.CommandText = SQLQuery;
                    string lineup = Convert.ToString(cmd.ExecuteScalar());

                    if (lineup != "")
                    {
                        SQLQuery = $"Insert Into Brands (BrandName, Lineup) Values ('{brandName}', '{newLineup}')";
                        cmd.CommandText = SQLQuery;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLQuery = $"Update Brands Set Lineup = '{newLineup}' Where(IdBrand = {idBrand} )";
                        cmd.CommandText = SQLQuery;
                        cmd.ExecuteNonQuery();
                    }
                    
                    con.Close();
                }
            }
            catch
            {
                DisplayAlert("Ошибка","Не удалось добавить модельный ряд","Вернуться");
            }
            
        }

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
                CarOptionsPage carOptionsPage = new CarOptionsPage(brandName, lineup, ((Button)sender).Text, MyConStr, adminLogin);
                Navigation.PushAsync(carOptionsPage, true);
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