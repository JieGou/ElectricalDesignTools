using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpfUI._Authentication
{

    [AddINotifyPropertyChangedInterface]
    public class EdtAuthorization
    {

        IFirebaseConfig _config = new FirebaseConfig {
            AuthSecret = "sHh8QNpsRu3IadlZYiUYCNNZ8ZOHfudL6lV26r0w",
            BasePath = "https://electrical-design-tools-default-rtdb.firebaseio.com/",
        };

        IFirebaseClient _client;

        public EdtUserAccount User { get; set; }



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
            catch (Exception) {

            }
        }

        public async void Push(EdtUserAccount account)
        {
            try {
                var response = await _client.PushAsync("UserAccounts", account);
                EdtUserAccount setResult = response.ResultAs<EdtUserAccount>();
            }
            catch (Exception) {

            }
        }


        public async Task<EdtUserAccount> GetAccount()
        {
            try {
                var response = await _client.GetAsync("UserAccounts/Test1");
                var result = response.ResultAs<EdtUserAccount>(); //The response will contain the data being retreived
                return result;
            }
            catch (Exception) {

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
            catch (Exception) {

                throw;
            }

        }



       
        public async void Update()
        {

        }
    }


}
