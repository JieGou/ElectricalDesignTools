using Firebase.Auth;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public User? CurrentUser => _currentFirebaseAuthLink?.User;
    public string Username => CurrentUser?.Email ?? "Anonymous";

    public async Task Login(string email, string password)
    {
        _currentFirebaseAuthLink = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
    }
}
