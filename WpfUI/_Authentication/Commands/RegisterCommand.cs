using Firebase.Auth;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using WpfUI._Authentication.ViewModels;

namespace WpfUI._Authentication.Commands;
public class RegisterCommand : AsyncCommandBase
{
    private readonly RegisterViewModel _registerViewModel;
    private readonly FirebaseAuthProvider _firebaseAuthProvider;
    private readonly INavigationService _loginNavigationService;

    public RegisterCommand(RegisterViewModel registerViewModel, FirebaseAuthProvider firebaseAuthProvider, INavigationService loginNavigationService)
    {
        _registerViewModel = registerViewModel;
        _firebaseAuthProvider = firebaseAuthProvider;
        _loginNavigationService = loginNavigationService;
    }


    EdtAuthorization edtAuth = new EdtAuthorization();
    protected override async Task ExecuteAsync(object parameter)
    {
        string password = _registerViewModel.Password;
        string confirmPassword = _registerViewModel.ConfirmPassword;

        if (password != confirmPassword) {
            MessageBox.Show("Passwords don't match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try {
            var firebaseAuthLink = await _firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(
                                        _registerViewModel.Email,
                                        password,
                                        _registerViewModel.Username,
                                        sendVerificationEmail:true);


            {
                var userAccount = new EdtUserAccount { CurrentUser=firebaseAuthLink.User, Email = _registerViewModel.Email };

                edtAuth.Initialize();
                edtAuth.Insert(userAccount);
                //edtAuth.Push(userAccount);
            }

            _loginNavigationService.Navigate();
        }
        catch (Exception ex) {
            if (ex.Message.Contains("INVALID_EMAIL")) {
                MessageBox.Show("Password must be minimum 6 characters.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (ex.Message.Contains("WEAK_PASSWORD")) {
                MessageBox.Show("Password must be minimum 6 characters.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (ex.Message.Contains("EMAIL_EXISTS")) {
                MessageBox.Show("An account with this email is already registered.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            else {
                MessageBox.Show("Missing registration details.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }
}
