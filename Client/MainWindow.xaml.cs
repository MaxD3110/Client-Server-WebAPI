using Newtonsoft.Json;
using Client.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using System.Windows.Controls;

namespace MyCarsWhishlist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<CardModel> _content;
        private int[] ToDelete = new int[] { };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Loader()
        {
            using (var client = new HttpClient())
            {
                var responce = await client.GetAsync("http://localhost:54059/values");
                responce.EnsureSuccessStatusCode();

                if (responce.IsSuccessStatusCode)
                {
                    _content = JsonConvert.DeserializeObject<BindingList<CardModel>>(await responce.Content.ReadAsStringAsync());

                    if (_content == null)
                        MessageBox.Show("cardDataList.json empty!");
                    else
                    _content.ListChanged += _content_ListChanged;

                    NewCards.ItemsSource = _content;
                }
                else
                {
                    MessageBox.Show($"Responce : {responce.StatusCode} ");
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loader();
        }

        private async void _content_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemChanged)
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync("http://localhost:54059/values", _content);
                    if (!response.IsSuccessStatusCode)
                        MessageBox.Show($"Error: {response.StatusCode}");
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateNew.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CreateNew.Visibility = Visibility.Collapsed;
            Emptyfier();
        }

        private void Emptyfier()
        {
            NameBox.Text = null;
            PicUpload.Source = null;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image files|*.jpg;*.png";
            openFile.FilterIndex = 1;

            if (openFile.ShowDialog() == true)
            {
                PicUpload.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NameBox.Text) == true || PicUpload.Source == null)
            {
                MessageBox.Show("Choose image & description");
            }
            else
            {

                using (var client = new HttpClient())
                {
                    //_content.Add(card);
                    string Name = NameBox.Text;
                    string Photo = PicUpload.Source + "";
                    HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:54059/values/" + Name, Photo);
                    if (!response.IsSuccessStatusCode)
                        MessageBox.Show($"Error: {response.StatusCode}");
                }

                Loader();
            }

            CreateNew.Visibility = Visibility.Collapsed;
            Emptyfier();

        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {

            var selectedId = (NewCards.Items.IndexOf(NewCards.SelectedItem)).ToString();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync("http://localhost:54059/values/" + selectedId);
                if (!response.IsSuccessStatusCode)
                    MessageBox.Show($"Error: {response.StatusCode}");
            }
            Loader();
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync("http://localhost:54059/values", _content);
                if (!response.IsSuccessStatusCode)
                    MessageBox.Show($"Error: {response.StatusCode}");
            }

            MessageBox.Show("Everything is saved in cardDataList.json!");

            Loader();

            
        }

        public string Selectable()
        {
            if (ToDelete.Length > 0)
            {
                return "false";
            }

            return "true";
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            ToDelete = ToDelete.Concat(new int[] { Convert.ToInt32((sender as CheckBox).Content) }).ToArray();

            SaveButton.Visibility = Visibility.Collapsed;
            DeleteAllButton.Visibility = Visibility.Visible;

            Selectable();
        }

        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:54059/values", ToDelete);
                if (!response.IsSuccessStatusCode)
                    MessageBox.Show($"Error: {response.StatusCode}");
            }

            DeleteAllButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            ToDelete = new int[] { };

            Loader();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToDelete = ToDelete.Except(new int[] { Convert.ToInt32((sender as CheckBox).Content) }).ToArray();

            if (ToDelete.Length <= 0)
            {
                DeleteAllButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
            }

            Selectable();
        }

    }
}