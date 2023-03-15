using Firebase.Auth;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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

    public User? CurrentUser
    {
        get
        {
            return _currentUser;
        }
        set
        {
            _currentUser = value;
            Username = _currentUser.Email;
        }
    }
    private User? _currentUser;


    public string Username
    {
        get { return _username; }
        set { _username = value; }
    }
    private string _username = "Anonymous";



    public async Task Login(string email, string password)
    {
        _currentFirebaseAuthLink = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
        CurrentUser = _currentFirebaseAuthLink?.User;

    }
}
