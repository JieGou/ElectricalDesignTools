using Authentication.WPF.Commands;
using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Authentication.WPF.ViewModels;
public class LoginViewModel : ViewModelBase
{
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

    public LoginViewModel(Firebase.Auth.FirebaseAuthProvider firebaseAuthProvider, INavigationService registerNavigationService)
    {
        LoginCommand = new LoginCommand(this, firebaseAuthProvider);
        NavigateRegisterCommand = new NavigateCommand((registerNavigationService));

    }
}
