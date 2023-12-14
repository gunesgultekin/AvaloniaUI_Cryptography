using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System;
using System.Net.Http;

namespace Avalonia.PKI.Views
{
    public partial class selectUserPageControl : Window
    {

        public Button button;
        public TextBlock myText;

        private readonly HttpClient _client;
        public selectUserPageControl()
        {
            InitializeComponent();
            contentControl.Content = new helpView(); 
            button = this.FindControl<Button>("button1");
            myText = this.FindControl<TextBlock>("textBlock");
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:44347");

        }

        public async void ButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Handle button click event here
            // For example, show a message box
            HttpResponseMessage response = await _client.GetAsync("api/MessagesContoller/getAll");

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            myText.Text = "Waiting for API connecion..." ;
            
        }

        private void GoToSenderView(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new SenderView(); 
        }

        private void GoToReceiverView(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ReceiverView(); 
        }

        private void GoToHelpView(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new helpView();
        }






    }
}