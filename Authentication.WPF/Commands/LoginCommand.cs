using Authentication.WPF.ViewModels;
using Firebase.Auth;
using MVVMEssentials.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Authentication.WPF.Commands;
public class LoginCommand : AsyncCommandBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly FirebaseAuthProvider _firebaseAuthProvider;

    public LoginCommand(LoginViewModel loginViewModel, FirebaseAuthProvider firebaseAuthProvider)
    {
        _loginViewModel = loginViewModel;
        _firebaseAuthProvider = firebaseAuthProvider;
    }

    protected override async Task ExecuteAsync(object parameter)
    {
        try {
            await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(
                _loginViewModel.Email,
                _loginViewModel.Password);

    

        }
        catch (Exception ex) {
            if (ex.Message.Contains("EMAIL_NOT_FOUND")) {
                MessageBox.Show("This email is not registered. Confirm email and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            if (ex.Message.Contains("INVALID_PASSWORD")) {
                MessageBox.Show("Invalid Password. Confirm password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            MessageBox.Show("Could not login. Confirm eamil and password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
