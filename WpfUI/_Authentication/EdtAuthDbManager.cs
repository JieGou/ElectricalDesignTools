using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI._Authentication
{

    [AddINotifyPropertyChangedInterface]
    public class EdtAuthDbManager
    {

        IFirebaseConfig _config = new FirebaseConfig {
            AuthSecret = "sHh8QNpsRu3IadlZYiUYCNNZ8ZOHfudL6lV26r0w",
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
