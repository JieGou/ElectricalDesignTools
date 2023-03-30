using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System.Windows.Input;
using WpfUI._Authentication.Commands;

namespace WpfUI._Authentication.ViewModels;
public class RegisterViewModel: ViewModelBase
{
    public RegisterViewModel(Firebase.Auth.FirebaseAuthProvider firebaseAuthProvider, INavigationService loginNavigationService)
    {
        RegisterCommand = new RegisterCommand(this, firebaseAuthProvider, loginNavigationService);
        NavigateLoginCommand = new NavigateCommand((loginNavigationService));
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

	private string _username;
	public string FullName
	{
		get
		{
			return _username;
		}
		set
		{
			_username = value;
			OnPropertyChanged(nameof(FullName));
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


	private string _confirmPassword;
	public string ConfirmPassword
    {
		get
		{
			return _confirmPassword;
		}
		set
		{
			_confirmPassword = value;
			OnPropertyChanged(nameof(ConfirmPassword));
		}
	}

	public ICommand RegisterCommand { get; }
	public ICommand NavigateLoginCommand { get; }

    
}
