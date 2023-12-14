using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData;
using Newtonsoft.Json;

namespace Avalonia.PKI.Views
{
    public partial class ReceiverView : UserControl
    {
        private HttpClient _client;
        private ListBox receivedFiles;
        private List<string> absolutePaths;

        public ReceiverView()
        {
            InitializeComponent();
            contentControl.Content = new receiverViewHelp();
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionConfiguration.clientBaseAdress);
            receivedFiles = new ListBox();
            absolutePaths = new List<string>();
        }

        private void GoToHelpView(object sender, RoutedEventArgs args)
        {
            contentControl.Content = new receiverViewHelp();
        }
        
        
        private async void getAll(object sender, RoutedEventArgs args)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/Messages/getReceivedFileNames");
                response.EnsureSuccessStatusCode();

                var responseBody = response.Content.ReadAsStringAsync().Result;
                
                List<string> myList = JsonConvert.DeserializeObject<List<string>>(responseBody);
                
                for (int i = 0; i < myList.Count; ++i)
                {
                    absolutePaths.Add(myList[i]);
                    receivedFiles.Items.Add(Path.GetFileName(myList[i]));
                    

                }
                
                contentControl.Content = receivedFiles;

                
            }
            catch (Exception ex)
            {
                contentControl.Content = new errorView("Cannot receive data");
            }
            
        }

        private async void deleteAll(object sender, RoutedEventArgs args)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/Messages/DeleteAll");
                response.EnsureSuccessStatusCode();
                statusLabel.Content = "Deleted All files";
                Content = new ReceiverView();
                

            }
            catch (Exception ex)
            {
                contentControl.Content = new errorView("Cannot delete data");

            }
           
        }

        private async void validateSignature(object sender, RoutedEventArgs args)
        {
            try
            {
                if (receivedFiles.SelectedItems.Count == 0)
                {
                    
                    statusLabel.Content = "Please pick a file first.";
                }

                else
                { // receivedFiles.SelectedItems[0]
                    HttpResponseMessage response = await _client
                        .GetAsync($"api/Users/verifyDigitalSignature?message={absolutePaths[receivedFiles.SelectedIndex]}");

                    string validationStatus = await response.Content.ReadAsStringAsync();
                    
                    if (validationStatus == "VERIFIED")
                    {
                        
                        statusLabel.Content = "Digital Signature" + "\n" + "verified";

                    }

                    else
                    {
                        
                        statusLabel.Content = "Cannot validate" + "\n"+ "Digital Signature";
                    }
                
                }
                

            }
            catch (Exception ex)
            {
                Content = new errorView("");

            }
            
            
            
        }

        private async void decryptFile(object sender, RoutedEventArgs args)
        {
            
            try
            {
                if (receivedFiles.SelectedItems.Count == 0)
                {
                    
                    statusLabel.Content = "Please pick a file first.";
                }

                else
                {
                    HttpResponseMessage response = await _client
                        .GetAsync($"api/Users/symmetricDecryption?filePath={absolutePaths[receivedFiles.SelectedIndex]}");
                    response.EnsureSuccessStatusCode();

                    statusLabel.Content = "File Decrypted" + "\n" + "Saved to Deskop as:" + "\n" + "DecryptedFile";
                
                }


            }
            catch (Exception ex)
            {
                Content = new errorView("Decryption Error");

            }
           
        }

        private async void decryptAndVerify(object sender, RoutedEventArgs args)
        {
            
            try
            {
                if (receivedFiles.SelectedItems.Count == 0)
                {
                   
                    statusLabel.Content = "Please pick a file first.";
                }

                else
                {
                    HttpResponseMessage response = await _client
                        .GetAsync($"/api/Users/decryptAndVerify?filePath={absolutePaths[receivedFiles.SelectedIndex]}");
                    response.EnsureSuccessStatusCode();
                    
                    string status = await response.Content.ReadAsStringAsync();
                    
                    if (status == "SUCCESS")
                    {
                        statusLabel.Content = "Verified and Decrypted";

                    }
                    else
                    {
                        statusLabel.Content = "Not verified";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Content = new errorView("");

            }
            
        }
        
    }
}
