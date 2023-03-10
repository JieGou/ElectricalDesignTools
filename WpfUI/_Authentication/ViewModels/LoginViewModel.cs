﻿using MVVMEssentials.Commands;
using MVVMEssentials.Services;
using MVVMEssentials.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using WpfUI._Authentication.Commands;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.ViewModels.Home;

namespace WpfUI._Authentication.ViewModels;
public class LoginViewModel : ViewModelBase
{
    public LoginViewModel(Firebase.Auth.FirebaseAuthProvider firebaseAuthProvider, INavigationService registerNavigationService, AuthenticationMainWindow authWindow)
    {
        LoginCommand = new LoginCommand(this, firebaseAuthProvider, authWindow);
        NavigateRegisterCommand = new NavigateCommand((registerNavigationService));
        DeserializeUserInfo();
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

    private UserInfo _userInfo;

    [Serializable]
    private class UserInfo
    {
        public UserInfo(string email, byte[] entropy, byte[] cypherText)
        {
            Email = email;
            Entropy = entropy;
            CypherText = cypherText;
        }

        public string Email { get; set; }
        public byte[] Entropy { get; set; }
        public byte[] CypherText { get; set; }
    }

    private byte[] _plainTextPassword;
    public void SaveLoginInfo()
    {
        byte[] _plainTextPassword = Encoding.UTF8.GetBytes(_password);
        byte[] entropy = new byte[20];

        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
            rng.GetBytes(entropy);
        }

        byte[] cypherText = ProtectedData.Protect(_plainTextPassword, entropy,
            DataProtectionScope.CurrentUser);

        _userInfo = new UserInfo(_email, entropy, cypherText);
    }

    public void SerializUserInfo()
    {
        SaveLoginInfo();
        using (Stream stream = File.Open("UserLogin.bin", FileMode.Create)) {
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(stream, _userInfo);
        }
    }

    private void DeserializeUserInfo()
    {
        try {
            var previousProjectDtoList = new ObservableCollection<PreviousProjectDto>();
            string userInfo = "UserLogin.bin";

            //Deserialize RecentProject to DTO's
            if (File.Exists(userInfo)) {
                using (Stream stream = File.Open(userInfo, FileMode.Open)) {
                    if (stream.Length != 0) {
                        BinaryFormatter bin = new BinaryFormatter();
                        _userInfo = (UserInfo)bin.Deserialize(stream);
                        _plainTextPassword = ProtectedData.Unprotect(_userInfo.CypherText, _userInfo.Entropy, DataProtectionScope.CurrentUser);
                        _email = _userInfo.Email;
                        _password = Encoding.Default.GetString(_plainTextPassword);
                    }
                   
                }
            }
        }
        catch (IOException ex) {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }

    public ICommand LoginCommand { get; }
	public ICommand NavigateRegisterCommand { get; }

    
}
