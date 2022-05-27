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
using System.Net.Http.Headers;

namespace MyCarsWhishlist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<CardModel> _content;
        private int[] ToDelete = new int[] { };

        HttpClient client = new HttpClient();


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Loader()
        {
                var responce = await client.GetAsync("http://localhost:54059/values");
                responce.EnsureSuccessStatusCode();

                if (responce.IsSuccessStatusCode)
                {
                    _content = JsonConvert.DeserializeObject<BindingList<CardModel>>(await responce.Content.ReadAsStringAsync());

                    NewCards.ItemsSource = _content;
                }
                else
                {
                    MessageBox.Show($"Responce : {responce.StatusCode} ");
                }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client.BaseAddress = new Uri("http://localhost:54059/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Loader();
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
                string Name = NameBox.Text;
                string Photo = PicUpload.Source + "";
                HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:54059/values/" + Name, Photo);
                if (!response.IsSuccessStatusCode)
                    MessageBox.Show($"Error: {response.StatusCode}");

                Loader();
            }

            CreateNew.Visibility = Visibility.Collapsed;
            Emptyfier();

        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var selectedId = (NewCards.Items.IndexOf(NewCards.SelectedItem)).ToString();
            HttpResponseMessage response = await client.DeleteAsync("http://localhost:54059/values/" + selectedId);
            if (!response.IsSuccessStatusCode)
                MessageBox.Show($"Error: {response.StatusCode}");

            Loader();
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync("http://localhost:54059/values", _content);
            if (!response.IsSuccessStatusCode)
                MessageBox.Show($"Error: {response.StatusCode}");
            else
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
            HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:54059/values", ToDelete);
            if (!response.IsSuccessStatusCode)
                MessageBox.Show($"Error: {response.StatusCode}");

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