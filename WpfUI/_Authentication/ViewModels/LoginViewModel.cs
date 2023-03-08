using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System.Windows.Input;
using WpfUI._Authentication.Commands;

namespace WpfUI._Authentication.ViewModels;
public class LoginViewModel : ViewModelBase
{
    public LoginViewModel(Firebase.Auth.FirebaseAuthProvider firebaseAuthProvider, INavigationService registerNavigationService, AuthenticationMainWindow authWindow)
    {
        LoginCommand = new LoginCommand(this, firebaseAuthProvider, authWindow);
        NavigateRegisterCommand = new NavigateCommand((registerNavigationService));

    }

    private string _email;
	public string Email
	{
		get
		{
			return _email;
		}
		set
		{
			_email = value;
			OnPropertyChanged(nameof(Email));
		}
	}

	
	private string _password;
	public string Password
	{
		get
		{
			return _password;
		}
		set
		{
			_password = value;
			OnPropertyChanged(nameof(Password));
		}
	}


	public ICommand LoginCommand { get; }
	public ICommand NavigateRegisterCommand { get; }

    
}
