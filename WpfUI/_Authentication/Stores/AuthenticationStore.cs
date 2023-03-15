using Firebase.Auth;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI._Authentication.Stores;
[AddINotifyPropertyChangedInterface]
public class AuthenticationStore
{
    private readonly FirebaseAuthProvider _firebaseAuthProvider;

    private FirebaseAuthLink _currentFirebaseAuthLink;

    public AuthenticationStore(FirebaseAuthProvider firebaseAuthProvider)
    {
        _firebaseAuthProvider = firebaseAuthProvider;
    }

   
   
    private EdtAuthorization _edtAuth = new EdtAuthorization();
    public EdtUserAccount EdtUser { get; set; } = new EdtUserAccount();
    

    public async Task Login(string email, string password)
    {
        _currentFirebaseAuthLink = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
        EdtUser.CurrentUser = _currentFirebaseAuthLink?.User;

        {
            _edtAuth.Initialize();
            var accounts = _edtAuth.GetAllAccounts().Result;

            foreach (var account in accounts) {
               
                if (account.Value.Email == EdtUser.CurrentUser.Email) {
                    MessageBox.Show($"User: {account.Value.Email} is subscribed until {account.Value.Subscription_End}");
                }
            }

        }
    }
}
