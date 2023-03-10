using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace FireSharp.Library
{
   

    public partial class EdtAuthorization
    {

        IFirebaseConfig _config = new FirebaseConfig {
            AuthSecret = "sHh8QNpsRu3IadlZYiUYCNNZ8ZOHfudL6lV26r0w",
            BasePath = "https://electrical-design-tools-default-rtdb.firebaseio.com/",
        };

        IFirebaseClient _client;

        public UserAccount User { get; set; }



        public void Initialize()
        {
            try {

                _client = new FireSharp.FirebaseClient(_config);
                if (_client != null) {
                }
            }
            catch (Exception) {
                throw;
            }
        }

        
        public async void Insert(UserAccount account)
        {
            try {
                var response = await _client.SetAsync("UserAccounts/" + account.Email, account);
                UserAccount setResult = response.ResultAs<UserAccount>();
            }
            catch (Exception) {

            }
        }

        public async void Push(UserAccount account)
        {
            try {
                var response = await _client.PushAsync("UserAccounts", account);
                UserAccount setResult = response.ResultAs<UserAccount>();
            }
            catch (Exception) {

            }
        }


        public async Task<UserAccount> GetAccount()
        {
            try {
                var response = await _client.GetAsync("UserAccounts/Test1");
                var result = response.ResultAs<UserAccount>(); //The response will contain the data being retreived
                return result;
            }
            catch (Exception) {

                throw;
            }

        }


        public async Task<Dictionary<string, UserAccount>> GetAllAccounts()
        {
            try {
                FirebaseResponse response = _client.Get(@"UserAccounts");
                Dictionary<string, UserAccount> accounts = JsonConvert.DeserializeObject<Dictionary<string, UserAccount>>(response.Body.ToString());
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
