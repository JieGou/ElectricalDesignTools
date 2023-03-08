﻿using Authentication.WPF.ViewModels;
using Firebase.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using MVVMEssentials.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Authentication.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    private readonly IHost _host;

    public App()
    {
        _host =  Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, service) => 
            {
                string firebaseApiKey = context.Configuration.GetValue<string>("FIREBASE_API_KEY");
                
                service.AddSingleton(new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey)));

                service.AddSingleton<NavigationStore>();
                service.AddSingleton<ModalNavigationStore>();

                service.AddSingleton<NavigationService<RegisterViewModel>>(
                    (services)=> new NavigationService<RegisterViewModel>(
                        services.GetRequiredService<NavigationStore>(),
                        ()=>new RegisterViewModel(
                            services.GetRequiredService<FirebaseAuthProvider>(),
                            services.GetRequiredService<NavigationService<LoginViewModel>>())));

                service.AddSingleton<NavigationService<LoginViewModel>>(
                    (services) => new NavigationService<LoginViewModel>(
                        services.GetRequiredService<NavigationStore>(),
                        () => new LoginViewModel(
                            services.GetRequiredService<FirebaseAuthProvider>(),
                            services.GetRequiredService<NavigationService<RegisterViewModel>>())));

                service.AddSingleton<MainViewModel>();

                service.AddSingleton<MainWindow>((services) => new MainWindow() {
                    DataContext = services.GetRequiredService<MainViewModel>()

                });
            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {

        INavigationService navigationService = _host.Services.GetRequiredService<NavigationService<LoginViewModel>>();
        navigationService.Navigate();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show(); 

        var firebaseAuthProvider = _host.Services.GetRequiredService<FirebaseAuthProvider>();

        firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync("pdeault@gmail.com", "asdfasdf", sendVerificationEmail: true);
        
        base.OnStartup(e);
    }
}
