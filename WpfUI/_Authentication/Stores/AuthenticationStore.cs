using Firebase.Auth;
using Portable.Licensing;
using PropertyChanged;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.Protection.PlayReady;
using WpfUI._Authentication.OfflineLicense;
using WpfUI._Authentication.ViewModels;
using WpfUI.PopupWindows;
using WpfUI.Services;

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

   
   
    private EdtAuthDbManager _edtDbAuthManager = new EdtAuthDbManager();
    public EdtUserAccount EdtUserAccount { get; set; } = new EdtUserAccount();
    

    public async Task<FirebaseAuthLink> Login(LoginViewModel loginViewModel, string email, string password)
    {
        _currentFirebaseAuthLink = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
        if (_currentFirebaseAuthLink.User.IsEmailVerified == true) {
            

            _edtDbAuthManager.Initialize();

            EdtUserAccount = await _edtDbAuthManager.GetAccount(_currentFirebaseAuthLink?.User.LocalId);
            
            
            OfflineLicenseManager.GenerateLicense(EdtUserAccount, password);

            loginViewModel.SerializUserInfo();
            _authWindow._isLoggedIn = true;
            _authWindow.Close();

            PopupService.ShowPopupNotificationAsync(
                $"{EdtUserAccount.Email} is subscribed until {EdtUserAccount.Subscription_End} " +
                $"under a {EdtUserAccount.SubscriptionStatus} subscription");

            await PopupService.ClosePopupNotificationAsync(3000);

        }
        else {
            MessageBox.Show($"The email {email} has not been verified. Check your inbox for a verification email and click the link.");
           
        }
        return _currentFirebaseAuthLink;
    }

    public void LoginOffline(EdtUserAccount edtUserAccount)
    {
        EdtUserAccount = edtUserAccount;

        MessageBox.Show($"A temporary offline license has been used to login. \n\n" +
            $"The offline license for {EdtUserAccount.Email} is valid until {EdtUserAccount.Subscription_End}",
            "Offline Login Successful",
            MessageBoxButton.OK,
            MessageBoxImage.Information); 
        
        _authWindow._isLoggedIn = true;
        _authWindow.Close();
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

    public async Task ResetPassword(string email)
    {

        var result = 
            MessageBox.Show($"A password reset link will be sent to {email}.", 
            "Reset Password", 
            MessageBoxButton.OKCancel, 
            MessageBoxImage.Information);
   

        if (result == MessageBoxResult.OK) {
            await _firebaseAuthProvider.SendPasswordResetEmailAsync(email);
        }


    }

    public async Task GetFreshAuthAsync()
    {
        await _currentFirebaseAuthLink.GetFreshAuthAsync();
        EdtUserAccount.CurrentUser = _currentFirebaseAuthLink?.User;
    }
}
