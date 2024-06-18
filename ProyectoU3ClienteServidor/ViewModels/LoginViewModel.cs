using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using ProyectoU3ClienteServidor.Models.Validators;
using ProyectoU3ClienteServidor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoU3ClienteServidor.ViewModels
{
    public partial class LoginViewModel:ObservableObject
    {
        Repositories.RepositoryGeneric<Actividad> repositoryActividades = new();
        LoginService loginService = new();
        ActividadesService actividadesService = new();
        DepartamentosService departamentosService = new();

        [ObservableProperty]
        public string? username;

        [ObservableProperty]
        public string? password;

        [ObservableProperty]
        public string error = "";

        //[RelayCommand]
        //public async Task CerrarSesion()
        //{
        //    SecureStorage.Remove("tkn");
        //    await Task.Run(() => repositoryActividades.DeleteAll()); // Asegúrate de que DeleteAll termine
        //    Preferences.Clear();
        //    await Task.Delay(2500);
        //    Shell.Current.FlyoutIsPresented = false;
        //    await Shell.Current.GoToAsync("//Login");
        //}


        [RelayCommand]
        public async Task Login()
        {
                LoginDTO loginDTO = new()
                {
                    Username = username,
                    Password = password
                };
                LoginValidator validator = new();
                var loginvalidado = validator.Validate(loginDTO);
                if (loginvalidado.IsValid)
                {
                    var tokensito = await loginService.Login(loginDTO);
                    if (tokensito != null)
                    {
                        await SecureStorage.SetAsync("tkn", tokensito);
                        //await Task.Delay(6500);
                       // int departamentoid = await loginService.GetDepartmentoId();
                       // await actividadesService.GetActividades(departamentoid);
                        Error = "";
                        var rol = await loginService.GetRol();
                        if (rol == "Admin")
                        {
                            await departamentosService.GetDepartamentos();
                            await Shell.Current.GoToAsync("//ListadoActividadesAdmin");
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("//ListadoActividades");
                        }
                    }
                  else
                  {
                    Error = "Verifique haber ingresado el usuario y contraseña correctamente.";
                  }
                }
                else
                {
                      Error = string.Join("\n", loginvalidado.Errors.Select(x => x.ErrorMessage));
                }
        }


        async void ValidarToken()
        {
            var tokenazo = await SecureStorage.GetAsync("tkn");
            if (tokenazo != null)
            {
                var tokenvalido = await loginService.Validar(tokenazo);
                var rol = await loginService.GetRol();
                if (tokenvalido)
                {
                    if (rol == "Admin")
                    {
                        await Shell.Current.GoToAsync("//ListaActividadesAdmin");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//ListaActividades");
                    }

                }
                else
                {
                    SecureStorage.RemoveAll();
                }
            }
            else
            {
                return;
            }
        }

    }
}
