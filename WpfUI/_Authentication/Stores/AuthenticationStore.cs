using Firebase.Auth;
using PropertyChanged;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.Protection.PlayReady;
using WpfUI._Authentication.ViewModels;

namespace WpfUI._Authentication.Stores;
[AddINotifyPropertyChangedInterface]
public class AuthenticationStore
{
    private readonly FirebaseAuthProvider _firebaseAuthProvider;
    private readonly AuthenticationMainWindow _authWindow;
    private FirebaseAuthLink _currentFirebaseAuthLink;

    public AuthenticationStore(FirebaseAuthProvider firebaseAuthProvider, AuthenticationMainWindow authWindow)
    {
        _firebaseAuthProvider = firebaseAuthProvider;
        _authWindow = authWindow;
    }

   
   
    private EdtAuthorization _edtAuth = new EdtAuthorization();
    public EdtUserAccount EdtUser { get; set; } = new EdtUserAccount();
    

    public async Task<FirebaseAuthLink> Login(LoginViewModel loginViewModel, string email, string password)
    {
        _currentFirebaseAuthLink = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
        if (_currentFirebaseAuthLink.User.IsEmailVerified == true) {
            EdtUser.CurrentUser = _currentFirebaseAuthLink?.User;
            _edtAuth.Initialize();
            var accounts = _edtAuth.GetAllAccounts().Result;

            foreach (var account in accounts) {

                if (account.Value.Email == EdtUser.CurrentUser.Email) {
                    MessageBox.Show($"User: {account.Value.Email} is subscribed until {account.Value.Subscription_End}");
                }
            }
            loginViewModel.SerializUserInfo();
            _authWindow._isLoggedIn = true;
            _authWindow.Close();
        }
        else {
            MessageBox.Show($"The email {email} has not been verified. Check your inbox for a verification email and click the link.");
           
        }
        return _currentFirebaseAuthLink;
    }

    public void Logout()
    {
        _currentFirebaseAuthLink = null;
    }

    public async Task SendVerificationEmail()
    {
        try {

            await _firebaseAuthProvider.SendEmailVerificationAsync(_currentFirebaseAuthLink.FirebaseToken);
        }
        catch (Exception ex) {
            if (_currentFirebaseAuthLink == null) {
                MessageBox.Show("Please try logging in first so we can confirm who to send the email to.",
                                "Unknown Email",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else {
                MessageBox.Show(ex.Message,
                                "Email Verification Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }

    public async Task GetFreshAuthAsync()
    {
        await _currentFirebaseAuthLink.GetFreshAuthAsync();
        EdtUser.CurrentUser = _currentFirebaseAuthLink?.User;
    }
}
