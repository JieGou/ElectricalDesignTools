using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.Helpers;

namespace WpfUI._Authentication
{

    [AddINotifyPropertyChangedInterface]
    public class EdtAuthDbManager
    {
        private static string _filePath = $@"{Environment.CurrentDirectory}/ContentFiles/EdtDbCon.sec";
        private static string DecryptSecret()
        {

            try {
                using (FileStream fileStream = new(_filePath, FileMode.Open)) {
                    using (Aes aes = Aes.Create()) {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0) {
                            int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        byte[] key =
                        {
                            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                        };

                        using (CryptoStream cryptoStream = new(
                           fileStream,
                           aes.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read)) {
                            // By default, the StreamReader uses UTF-8 encoding.
                            // To change the text encoding, pass the desired encoding as the second parameter.
                            // For example, new StreamReader(cryptoStream, Encoding.Unicode).
                            using (StreamReader decryptReader = new(cryptoStream)) {
                                string decryptedMessage = decryptReader.ReadToEnd();
                                decryptedMessage= decryptedMessage.Substring(0,decryptedMessage.Length-2);
                                return decryptedMessage;
                                // sHh8QNpsRu3IadlZYiUYCNNZ8ZOHfudL6lV26r0w
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
                return null;
            }
        }

        IFirebaseConfig _config = new FirebaseConfig {
            AuthSecret = DecryptSecret(),
            BasePath = "https://electrical-design-tools-default-rtdb.firebaseio.com/",
        };

        IFirebaseClient _client;


        public void Initialize()
        {
            try {
                _client = new FirebaseClient(_config);
                if (_client != null) {
                }
            }
            catch (Exception) {
                throw;
            }
        }

        
        public async void Insert(EdtUserAccount account)
        {
            try {
                var response = await _client.SetAsync("UserAccounts/" + account.CurrentUser.LocalId, account);
                EdtUserAccount setResult = response.ResultAs<EdtUserAccount>();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public async void Push(EdtUserAccount account)
        {
            try {
                var response = await _client.PushAsync("UserAccounts", account);
                EdtUserAccount setResult = response.ResultAs<EdtUserAccount>();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }


        public async Task<EdtUserAccount> GetAccount(string userId)
        {
            try {
                var response = await _client.GetAsync($"UserAccounts/{userId}");
                var result = response.ResultAs<EdtUserAccount>(); //The response will contain the data being retreived
                return result;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                throw;
            }

        }


        public async Task<Dictionary<string, EdtUserAccount>> GetAllAccounts()
        {
            try {
                FirebaseResponse response = _client.Get(@"UserAccounts");
                Dictionary<string, EdtUserAccount> accounts = JsonConvert.DeserializeObject<Dictionary<string, EdtUserAccount>>(response.Body.ToString());
                return accounts;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                throw;
            }

        }



       
        public async void Update()
        {

        }
    }


}
