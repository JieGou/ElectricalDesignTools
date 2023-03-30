using EDTLibrary.A_Helpers;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using Portable.Licensing.Validation;
using Syncfusion.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace WpfUI._Authentication.OfflineLicense;
internal class OfflineLicenseManager
{
    private static License _license = null;

    private static string _licenseFilePath;
    private static FileInfo _licenseFile;

    private static string _privateKeyFilePath = $"{Environment.CurrentDirectory}\\PrivateKey.txt";
    private static FileInfo _privateKeyFile = new FileInfo(_privateKeyFilePath);

    private static string _publicKeyFilePath = $"{Environment.CurrentDirectory}\\PublicKey.txt";
    private static FileInfo _publicKeyFile = new FileInfo(_publicKeyFilePath);

    private static string _privateKey = "";
    private static string _publicKey = "";

    private const string _offlineLicenseDefaultPassword = "asdf";
    private static int _offlineLicenseDays = 7;



    private static void GenerateKeys( string licenseEncryptionPassword = _offlineLicenseDefaultPassword)
    {
        KeyGenerator keyGenerator = KeyGenerator.Create();
        KeyPair keyPair = keyGenerator.GenerateKeyPair();

        _privateKey = keyPair.ToEncryptedPrivateKeyString(licenseEncryptionPassword);
        File.WriteAllText(_privateKeyFilePath, _privateKey.ToString(), Encoding.UTF8);

        _publicKey = keyPair.ToPublicKeyString();
        File.WriteAllText(_publicKeyFilePath, _publicKey.ToString(), Encoding.UTF8);
    }
    internal static void GenerateLicense(EdtUserAccount edtUserAccount, string edtUserAccountPassword, string licenseEncryptionPassword = _offlineLicenseDefaultPassword)
    {

        GenerateKeys();

        try {

            string ComputerGuid = ComputerInfo.GetComputerGuid();

            _license = License.New().WithUniqueIdentifier(Guid.NewGuid())
                                    .As(LicenseType.Trial)
                                    .ExpiresAt(DateTime.Now.AddDays(_offlineLicenseDays))
                                    .WithMaximumUtilization(1)
                                    .WithProductFeatures(new Dictionary<string, string>
                                                                    {
                                                                        {"All Modules", "yes"},
                                                                        {"Duration", "30 Days"},
                                                                    })
                                    .WithAdditionalAttributes(new Dictionary<string, string> {
                                                                        {"Email", edtUserAccount.Email},
                                                                        {"Password", edtUserAccountPassword},
                                                                        {"UserId", edtUserAccount.UserId},
                                                                        {"Name", edtUserAccount.FullName},

                                                                        {"SubscriptionStatus", edtUserAccount.SubscriptionStatus},
                                                                        {"Subscription_Start", edtUserAccount.Subscription_Start.ToString()},
                                                                        {"Subscription_End", DateTime.Now.AddDays(7).ToString()},
                                                                        {"ComputerName", Environment.MachineName.ToString()},
                                                                        {"ComputerGuid", ComputerGuid},
                                                                    })
                                    .LicensedTo(edtUserAccount.FullName, edtUserAccount.Email)
                                    .CreateAndSignWithPrivateKey(File.ReadAllText(_privateKeyFilePath), licenseEncryptionPassword);


            _licenseFilePath = $@"{Environment.CurrentDirectory}_{edtUserAccount.Email}_License.lic";
            _licenseFile = new FileInfo(_licenseFilePath);
            _licenseFile.Directory.Create();

            File.WriteAllText(_licenseFilePath, _license.ToString(), Encoding.UTF8);

            AppSettings.Default.LicenseFileCreated = true;
            AppSettings.Default.Save();
           

        }
        catch (Exception ex) {
                    MessageBox.Show("Could not create a new offline license. You will not be able to run the application without the internet. \n\n" +
                    "Contact DCS Inc. and provide the below error code. \n\n" + 
                    "Error Code: \n\n"  + ex.Message, 
                    "Offline License Creation Failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }
    }


    private static bool GetPrivateKey()
    {
        if (_privateKeyFile.Exists) {
            _privateKey = File.ReadAllText(_privateKeyFilePath);
            return true;
        }
        return false;
       
    }
    private static bool GetPublicKey()
    {
        if (_publicKeyFile.Exists) {
            _publicKey = File.ReadAllText(_publicKeyFilePath);
            return true;
        }
        return false;
    }

    private static bool GetLicenseFile(string email)
    {
        _licenseFilePath = $@"{Environment.CurrentDirectory}_{email}_License.lic";
        _licenseFile = new FileInfo(_licenseFilePath);

        if (_licenseFile.Exists) {
            _license = License.Load(File.OpenText(_licenseFilePath));
            return true;
        }
        else return false;
    }
    internal static Tuple<bool,EdtUserAccount> ValidateLicense(string email, string password)
    {

        try {

            if (GetLicenseFile(email) == false || GetPublicKey() == false) {
                MessageBox.Show("There is no license file created for this account. You must first login via the internet to create an offline license. " +
                    "You must use the same email you used to login online.",
                    "Offline License Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return Tuple.Create(false, new EdtUserAccount());
            }



            else {
                var validationFailures = _license.Validate()
                                                    .ExpirationDate()
                                                    .When(lic => lic.Type == LicenseType.Trial)
                                                    .And()
                                                    .Signature(_publicKey)
                                                    .And()
                                                    .AssertThat(lic => lic.AdditionalAttributes.Get("Password") == password,
                                                                                                        new GeneralValidationFailure() {
                                                                                                            Message = "Incorrect user account password.",
                                                                                                            HowToResolve = "Verify login email and password."
                                                                                                        })
                                                    .And()
                                                    .AssertThat(lic => lic.AdditionalAttributes.Get("ComputerName") == Environment.MachineName.ToString(),
                                                                                                        new GeneralValidationFailure() {
                                                                                                            Message = "The license file is not registered for this machine. This can be caused If you changed your computer name or re-install Windows.",
                                                                                                            HowToResolve = "Contact administrator"
                                                                                                        })
                                                    .And()
                                                    .AssertThat(lic => lic.AdditionalAttributes.Get("ComputerGuid") == ComputerInfo.GetComputerGuid(),
                                                                                                        new GeneralValidationFailure() {
                                                                                                            Message = "The license file is not registered to this machine.",
                                                                                                            HowToResolve = "Contact administrator"
                                                                                                        })
                                                    .AssertValidLicense();
                bool isValidated = true;

                // Validation Failure Messages
                foreach (var failure in validationFailures) {
                    if (failure.Message.Contains("password")) {
                        MessageBox.Show($"{failure.Message}",
                            "Offline License Validation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (failure.Message.Contains("error")) {
                        MessageBox.Show($"The offline license file and/or the encryption key file are corrupt or have been modified. " +
                            "If the files have not been modified and you still possess a valid registration, contact DCS Inc. to resolve. " +
                            $"Logging in via the internet automatically generates an offline license that is valid for {_offlineLicenseDays} " +
                            $"days from your last online login. \n\n" +
                            $"Error Code: \n\n" +
                            $"{failure.Message}",
                            "Offline License Validation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (failure.Message.Contains("expired")) {
                        MessageBox.Show($"The offline license file for this account has expired. " +
                            $"You must login via the internet to create a new offline license that will be " +
                            $"valid for {_offlineLicenseDays} days from your last online login. \n\n" +
                            "Contact DCS Inc. to renew or extend your registration and get a new license \n\n" +
                            $"Error Code: \n\n" +
                            $"{failure.Message}",
                            "Offline License Expired", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    isValidated = false;

                }

                if (isValidated == false) {
                    return Tuple.Create(false, new EdtUserAccount());
                }

                var edtUserAccount = new EdtUserAccount { 
                    FullName = _license.AdditionalAttributes.Get("Name"), 
                    Email = _license.AdditionalAttributes.Get("Email"),
                    UserId = _license.AdditionalAttributes.Get("UserId"),

                    SubscriptionStatus = _license.AdditionalAttributes.Get("SubscriptionStatus"),
                    Subscription_Start= _license.AdditionalAttributes.Get("Subscription_Start").ToDateTime(),
                    Subscription_End= _license.AdditionalAttributes.Get("Subscription_End").ToDateTime(),
                };

                return Tuple.Create(true, edtUserAccount);
            }
        }
        catch (Exception ex) {
                MessageBox.Show("Public Key or License file is corrupt or has been modified." + "\n\n" +
                     $"Error Code: \n\n" + 
                     $"{ex.Message}", 
                    "Offline License Validation Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            return Tuple.Create(false, new EdtUserAccount());
        }
    }

}

