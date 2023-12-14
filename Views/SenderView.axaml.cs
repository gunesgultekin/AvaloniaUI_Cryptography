using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using System.Net.Http;
using Avalonia.Controls.Shapes;

namespace Avalonia.PKI.Views
{
    public partial class SenderView : UserControl
    {

        private readonly HttpClient _client;
        
        public string pickedFilePath;
        
        public SenderView()
        {
            InitializeComponent();
            
            contentControl.Content = new senderHelpView();
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionConfiguration.clientBaseAdress);

        }

        private void GoToHelpView(object sender, RoutedEventArgs args)
        {
            contentControl.Content = new senderHelpView();

        }
        private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select File",
                AllowMultiple = false,
            });


            if (files.Count != 0)
            {
                pickedFilePath = files[0].TryGetLocalPath();
                myLabel.Content = "File selected";
                
                if (files.Count >= 1)
                {
                    // Open reading stream from the first file.
                    await using var stream = await files[0].OpenReadAsync();
                    using var streamReader = new StreamReader(stream);
                    // Reads all the content of file as a text.
                    var fileContent = await streamReader.ReadToEndAsync();
                }
            }

            else
            {
                myLabel.Content = "No files selected";

            }
            
            
        }

        private async void createDigitalSignature(object sender, RoutedEventArgs args)
        {
            if (pickedFilePath == null)
            {
                myLabel.Content = "Please pick a file first.";
            }
            
            else
            {
                try
                {
                    HttpResponseMessage response =
                        await _client.GetAsync($"api/Users/createDigitalSignature?filePath={pickedFilePath}");
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    myLabel.Content = "Signature created";
                }
                catch (Exception e)
                {
                    Content = new errorView("Cannot create Digital Signature");

                }
            }
        }

        private async void encryptFile(object sender, RoutedEventArgs args)
        {
            if (pickedFilePath == null)
            {
                myLabel.Content = "Please pick a file first.";
            }

            else
            {
                
                try
                {
                    HttpResponseMessage response = await _client
                        .GetAsync($"api/Users/symmetricEncryption?filePath={pickedFilePath}");
                    response.EnsureSuccessStatusCode();
                    
                    myLabel.Content = "File encrypted" + "\n" + "Saved to desktop as EncryptedFile";

                }
                catch (Exception ex)
                {
                    Content = new errorView("Encryption Error");
                }
            }
        }

        private async void signAndEncrypt(object sender, RoutedEventArgs args)
        {
            if (pickedFilePath == null)
            {
                myLabel.Content = "Please pick a file first.";
            }

            else
            {
                try
                {
                    HttpResponseMessage response = await _client
                        .GetAsync($"/api/Users/signAndEncrypt?filePath={pickedFilePath}");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (responseBody == "SUCCESS")
                    {
                        myLabel.Content = "Signed and Encrypted.";

                    }
                    else
                    {
                        myLabel.Content = "Failed.";
                    }

                }
                catch (Exception ex)
                {
                    Content = new errorView("");
                }
                
            }

            
        }
    }
}
