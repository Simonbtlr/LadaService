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
    public partial class CarOptionsPage : ContentPage
    {
        //My block
        string brandName, lineup, modelName, MyConStr, newModel;
        StackLayout stcklt1 = new StackLayout();
        int selIdBrand, selIdModel, selIdEquipment, selIdEngine;
        bool adminLogin;

        public CarOptionsPage(string selectedBrandName, string selectedLineup, 
                              string selectedModelName, string mainConStr, bool adm)
        {

            //Данные с прошлой формы
            brandName = selectedBrandName;
            lineup = selectedLineup;
            modelName = selectedModelName;
            MyConStr = mainConStr;
            adminLogin = adm;
            string toTitle = brandName + " " + lineup;

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);


            if (selectedModelName != "Добавить тип кузова")
            { 
                ttl.Text = toTitle;
                descr.Text = modelName;

                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    //idBrand
                    string SQLQuery = $"Select IdBrand From Brands Where (BrandName = '{brandName}') And (Lineup = '{lineup}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    int idBrand = Convert.ToInt32(cmd.ExecuteScalar());

                    //idModel
                    SQLQuery = $"Select IdModel From Models Where (ModelName = '{modelName}')";
                    cmd.CommandText = SQLQuery;
                    int idModel = Convert.ToInt32(cmd.ExecuteScalar());

                    //Картинка
                    SQLQuery = $"Select ImageURL From Images Where (FK_IdModel = {idModel}) And (FK_IdBrand = {idBrand})";
                    cmd.CommandText = SQLQuery;
                    string imageSource = Convert.ToString(cmd.ExecuteScalar());
                    imageCar.Source = imageSource;

                    //Выбор комплектации + л.с.
                    SQLQuery = $"Select Count(FK_IdEquipment) From Autos Where (FK_IdBrand = {idBrand}) " +
                        $"And (FK_IdModel = {idModel})";
                    cmd.CommandText = SQLQuery;
                    int[] idEquipment = new int[Convert.ToInt32(cmd.ExecuteScalar())];

                    //Элементы пикера
                    //Информация о машине
                    int[] idAuto = new int[idEquipment.Length];
                    int[] idEngine = new int[idEquipment.Length];
                    SQLQuery = $"Select Min(IdAuto) From Autos Where (FK_IdBrand = {idBrand}) And (FK_IdModel = {idModel})";
                    string saveQuery = SQLQuery;

                    for (int i = 0; i < idEquipment.Length; i++)
                    {
                        cmd.CommandText = saveQuery;
                        idAuto[i] = Convert.ToInt32(cmd.ExecuteScalar());

                        SQLQuery = $"Select FK_IdEquipment From Autos Where (idAuto = {idAuto[i]})";
                        cmd.CommandText = SQLQuery;
                        idEquipment[i] = Convert.ToInt32(cmd.ExecuteScalar());

                        SQLQuery = $"Select FK_IdEngine From Autos Where (idAuto = {idAuto[i]})";
                        cmd.CommandText = SQLQuery;
                        idEngine[i] = Convert.ToInt32(cmd.ExecuteScalar());

                        SQLQuery = $"Select EquipmentName From Equipments Where (IdEquipment = {idEquipment[i]})";
                        cmd.CommandText = SQLQuery;
                        string pckrItem;

                        pckrItem = Convert.ToString(cmd.ExecuteScalar()) + " /";

                        SQLQuery = $"Select Volume From Engines Where (IdEngine = {idEngine[i]})";
                        cmd.CommandText = SQLQuery;
                        pckrItem += " " + Convert.ToString(cmd.ExecuteScalar());

                        SQLQuery = $"Select Horsepower From Engines Where (IdEngine = {idEngine[i]})";
                        cmd.CommandText = SQLQuery;
                        pckrItem += " (" + Convert.ToString(cmd.ExecuteScalar()) + "), ";

                        SQLQuery = $"Select Gearbox From Engines Where (IdEngine = {idEngine[i]})";
                        cmd.CommandText = SQLQuery;
                        pckrItem += Convert.ToString(cmd.ExecuteScalar());

                        pckr.Items.Add(pckrItem);

                        saveQuery += $" And (IdAuto <> {idAuto[i]})";
                    }
                    con.Close();
                }
            }
            else
            {
                ttl.Text = "Добавление типа кузова";
                descr.Text = "Внесите все данные";

                Entry newModelEntry = new Entry
                {
                    Placeholder = "Введите название кузова",
                    TextColor = Color.FromHex("#FFFFFF"),
                    PlaceholderColor = Color.FromHex("FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    Margin = new Thickness(50)
                };
                newModelEntry.Completed += NewModelEntry_Completed;
                newModelEntry.TextChanged += NewModelEntry_TextChanged;

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
                bttn.Clicked += NewModelEntry_Completed;

                stcklt1.Children.Add(newModelEntry);
                stcklt1.Children.Add(bttn);


                scrlvw1.Content = stcklt1;
            }
        }

        private void NewModelEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            newModel = ((Entry)sender).Text;
        }

        private void NewModelEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    string SQLQuery = $"Select IdModel From Models Where (ModelName = '{newModel}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    int idModel = Convert.ToInt32(cmd.ExecuteScalar());

                    if (idModel != 0)
                    {
                        DisplayAlert("Внимание","Данный тип кузова уже есть в базе. \nПожалуйста воспользуйтесь страницей для связки","Назад");
                    }
                    else
                    {
                        SQLQuery = $"Insert Into Models (ModelName) Values ('{newModel}')";
                        cmd.CommandText = SQLQuery;
                        cmd.ExecuteNonQuery();
                        DisplayAlert("Готово", "Данный тип кузова добавлен в базу. \nПожалуйста воспользуйтесь страницей для связки", "Ок");
                        Button_Clicked(sender, e);
                    }

                    con.Close();
                }
            }
            catch
            {
                DisplayAlert("Ошибка", "Не удалось добавить модельный ряд", "Вернуться");
            }

        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Close();
                Navigation.PopAsync();
            }
        }

        private void pckr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Очистка
            stcklt1.Children.Clear();
            scrlvw1.Content = null;
            
            
            //Разбиваем текст
            string selectedEquipment = pckr.SelectedItem.ToString();
            string equipmentName = "", volume = "", horsepower = "", gearbox = "";
            for (int i = 0; i < selectedEquipment.Length; i++)
            {
                if (selectedEquipment[i] != '/')
                {
                    equipmentName += selectedEquipment[i];
                }
                else
                {
                    for(int j = i + 1; j < selectedEquipment.Length; j++)
                    {
                        if (selectedEquipment[j] != '(')
                        {
                            volume += selectedEquipment[j];
                        }
                        else
                        {
                            for (int k = j + 1; k < selectedEquipment.Length; k++)
                            {
                                if (selectedEquipment[k] != ')')
                                {
                                    horsepower += selectedEquipment[k];
                                }
                                else
                                {
                                    for (int l = k + 1; l < selectedEquipment.Length; l++)
                                    {
                                        if(selectedEquipment[l] != ',')
                                        {
                                            gearbox += selectedEquipment[l];
                                        }
                                    }
                                    break;
                                }
                                
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            //Удаляю лишние знаки
            equipmentName = equipmentName.Substring(0, equipmentName.Length - 1);
            volume = volume.Substring(1, volume.Length - 2);
            gearbox = gearbox.Substring(1, gearbox.Length - 1);
            
            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                //Информация о комплекте
                Label lbl1 = new Label
                {
                    Text = "Безопасность:\n",
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    Padding = 15
                };
                
                string SQLQuery = $"Select SecDes From Equipments Where (EquipmentName = '{equipmentName}')";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                lbl1.Text += Convert.ToString(cmd.ExecuteScalar()) + "\n\nИнтерьер:\n";

                SQLQuery = $"Select Interior From Equipments Where (EquipmentName = '{equipmentName}')";
                cmd.CommandText = SQLQuery;
                lbl1.Text += Convert.ToString(cmd.ExecuteScalar()) + "\n\nКомфорт:\n";

                SQLQuery = $"Select Comfort From Equipments Where (EquipmentName = '{equipmentName}')";
                cmd.CommandText = SQLQuery;
                lbl1.Text += Convert.ToString(cmd.ExecuteScalar()) + "\n\nЭкстерьер:\n";

                SQLQuery = $"Select Exterior From Equipments Where (EquipmentName = '{equipmentName}')";
                cmd.CommandText = SQLQuery;
                lbl1.Text += Convert.ToString(cmd.ExecuteScalar());

                StackLayout total = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };

                //idBrand
                SQLQuery = $"Select IdBrand From Brands Where(BrandName = '{brandName}') And (Lineup = '{lineup}')";
                cmd.CommandText = SQLQuery;
                int idBrand = Convert.ToInt32(cmd.ExecuteScalar());

                //idModel
                SQLQuery = $"Select IdModel From Models Where(ModelName = '{modelName}')";
                cmd.CommandText = SQLQuery;
                int idModel = Convert.ToInt32(cmd.ExecuteScalar());

                //idEquipment 
                int idEquipment = GetIdEquipment(pckr);

                //idEngine
                /*SQLQuery = $"Select IdEngine From Engines Where(Volume = '{volume}') " +
                    $"And (Horsepower = '{horsepower}') And (Gearbox = '{gearbox}')";
                cmd.CommandText = SQLQuery;*/
                int idEngine = GetIdEngine(pckr);

                //Итого
                Label lbl2 = new Label
                {
                    Text = "Цена:\n",
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                    Padding = 15
                };

                SQLQuery = $"Select Price From Autos Where (FK_IdBrand = {idBrand}) " +
                    $"And (FK_IdModel = {idModel}) And (FK_IdEquipment = {idEquipment})" +
                    $"And (FK_IdEngine = {idEngine})";
                cmd.CommandText = SQLQuery;
                int price = Convert.ToInt32(cmd.ExecuteScalar());
                lbl2.Text += price + " руб.";

                Button bttn = new Button
                {
                    //Text
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontFamily = "Fonts/pragmaticalightc.otf#PragmaticaLightC",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                    Text = "Записаться на тест-драйв",

                    //Button
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BorderWidth = 1,
                    BorderColor = Color.FromHex("#775F92"),
                    BackgroundColor = Color.FromHex("#200F39"),
                    Margin = new Thickness(10, 4.5)
                };
                bttn.Clicked += Bttn_Clicked;

                total.Children.Add(lbl2);
                total.Children.Add(bttn);

                stcklt1.Children.Add(lbl1);
                stcklt1.Children.Add(total);

                scrlvw1.Content = stcklt1;

                con.Close();
            }
        }

        private void Bttn_Clicked(object sender, EventArgs e)
        {
            try
            {
                int selIdAuto;
                string SQLQuery;
                using (SqlConnection con = new SqlConnection(MyConStr))
                {
                    con.Open();

                    //selIdBrand
                    SQLQuery = $"Select idBrand From Brands Where (BrandName = '{brandName}') And (Lineup = '{lineup}')";
                    SqlCommand cmd = new SqlCommand(SQLQuery, con);
                    selIdBrand = Convert.ToInt32(cmd.ExecuteScalar());

                    //selIdModel
                    SQLQuery = $"Select idModel From Models Where (ModelName = '{modelName}')";
                    cmd.CommandText = SQLQuery;
                    selIdModel = Convert.ToInt32(cmd.ExecuteScalar());

                    //selIdEquipment
                    selIdEquipment = GetIdEquipment(pckr);

                    //selIdEngine
                    selIdEngine = GetIdEngine(pckr);

                    SQLQuery = $"Select idAuto From Autos Where (FK_IdBrand = {selIdBrand}) And (FK_IdModel = {selIdModel}) " +
                                      $"And (FK_IdEquipment = {selIdEquipment}) And (FK_IdEngine = {selIdEngine})";
                    cmd.CommandText = SQLQuery;
                    selIdAuto = Convert.ToInt32(cmd.ExecuteScalar());

                    con.Close();
                }

                TestDrivePage testDrivePage = new TestDrivePage(selIdAuto, ttl.Text, descr.Text, MyConStr);
                Navigation.PushAsync(testDrivePage);
            }
            catch
            {
                DisplayAlert("Ошибка", "Хм, похоже, что сервер упал.\nВы будете возвращены на главную страницу.", "Ок");
                MainPage mainPage = new MainPage();
                Navigation.PopToRootAsync();
            }
        }

        private int GetIdEquipment(Picker pckr)
        {
            string selectedEquipment = pckr.SelectedItem.ToString();
            string equipmentName = "";
            int idEquipment;


            for (int i = 0; i < selectedEquipment.Length; i++)
            {
                if(selectedEquipment[i] != '/')
                {
                    equipmentName += selectedEquipment[i];
                }
                else
                {
                    break;
                }
            }

            equipmentName = equipmentName.Substring(0, equipmentName.Length - 1);


            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                string SQLQuery = $"Select idEquipment from Equipments Where(EquipmentName = '{equipmentName}')";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                idEquipment = Convert.ToInt32(cmd.ExecuteScalar());

                con.Close();
            }

            return idEquipment;
        }

        private int GetIdEngine(Picker pckr)
        {
            string selectedEquipment = pckr.SelectedItem.ToString();
            string equipmentName = "", volume = "", horsepower = "", gearbox = "";
            for (int i = 0; i < selectedEquipment.Length; i++)
            {
                if (selectedEquipment[i] != '/')
                {
                    equipmentName += selectedEquipment[i];
                }
                else
                {
                    for (int j = i + 1; j < selectedEquipment.Length; j++)
                    {
                        if (selectedEquipment[j] != '(')
                        {
                            volume += selectedEquipment[j];
                        }
                        else
                        {
                            for (int k = j + 1; k < selectedEquipment.Length; k++)
                            {
                                if (selectedEquipment[k] != ')')
                                {
                                    horsepower += selectedEquipment[k];
                                }
                                else
                                {
                                    for (int l = k + 1; l < selectedEquipment.Length; l++)
                                    {
                                        if (selectedEquipment[l] != ',')
                                        {
                                            gearbox += selectedEquipment[l];
                                        }
                                    }
                                    break;
                                }

                            }
                            break;
                        }
                    }
                    break;
                }
            }

            //Удаляю лишние знаки
            equipmentName = equipmentName.Substring(0, equipmentName.Length - 1);
            volume = volume.Substring(1, volume.Length - 2);
            gearbox = gearbox.Substring(1, gearbox.Length - 1);

            int idEngine;
            using (SqlConnection con = new SqlConnection(MyConStr))
            {
                con.Open();

                string SQLQuery = $"Select IdEngine From Engines Where(Volume = '{volume}') " +
                        $"And (Horsepower = '{horsepower}') And (Gearbox = '{gearbox}')";
                SqlCommand cmd = new SqlCommand(SQLQuery, con);
                idEngine = Convert.ToInt32(cmd.ExecuteScalar());

                con.Close();
            }
            return idEngine;
        }
    }
}