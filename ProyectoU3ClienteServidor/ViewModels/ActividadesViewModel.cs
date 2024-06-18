using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using ProyectoU3ClienteServidor.Helper;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using ProyectoU3ClienteServidor.Models.Validators;
using ProyectoU3ClienteServidor.Services;
using ProyectoU3ClienteServidor.Views.ActividadesView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.ViewModels
{
    public partial class ActividadesViewModel : ObservableObject
    {

        HttpClient cliente = new()
        {
            BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
        };

        Repositories.RepositoryGeneric<Actividad> repositoryActividades = new();
        public ObservableCollection<Actividad> ListadoActividades { get; set; } = new();
        public ObservableCollection<Actividad> ListadoBorradores { get; set; } = new();

        [ObservableProperty]
        public string nuevaImagen = "";
        ActividadesService service { get; set; } = new();
        LoginService loginService { get; set; } = new();
        ActividadValidator validador = new();

        [ObservableProperty]
        public Actividad actividadSeleccionada;
        public int DepartamentoID { get; set; }

        public ActividadesViewModel()
        {
            App.ActividadesService.ActualizarDatos += ActividadesService_ActualizarDatos1;
        }


        private void ActividadesService_ActualizarDatos1()
        {
            ActualizarActividades();
            ActualizarBorradores();
        }


        [RelayCommand]
        public async Task CerrarSesion()
        {
            SecureStorage.Remove("tkn");
            await Task.Run(() => repositoryActividades.DeleteAll()); // Asegúrate de que DeleteAll termine
            Preferences.Clear();
            ListadoActividades.Clear();
            ListadoBorradores.Clear();
            await Task.Delay(2500);
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync("//Login");
        }


        [ObservableProperty]
        public ActividadDTO? actividad;

        [ObservableProperty]
        public string error = "";


        [RelayCommand]
        public async void VerListadoActividades()
        {
            ActualizarActividades();
            ActualizarBorradores();
            var rol = await loginService.GetRol();
            if (rol == "Admin")
            {
                await Shell.Current.GoToAsync("//ListadoActividadesAdmin");
            }
            else
            {
                await Shell.Current.GoToAsync("//ListadoActividades");
            }
        }


        [RelayCommand]
        public async void VerListadoBorradores()
        {
            ActualizarBorradores();
            await Shell.Current.GoToAsync("//ListadoBorradores");
        }


        [RelayCommand]
        public async void VerEditarActividad(int id)
        {
            int iddepa = await loginService.GetDepartmentoId();
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.GetFromJsonAsync<List<DepartamentoDTO>>($"api/DepartamentosAPI/ObtenerDepartamentos");
            if (response != null)
            {
                if (response.Any(x => x.Id == iddepa) == false)
                {
                    SecureStorage.Remove("tkn");
                    await Shell.Current.GoToAsync("//SesionCaducada");
                }
                else
                {

                    ActividadSeleccionada = repositoryActividades.Get(id);
                    if (ActividadSeleccionada != null)
                    {
                        if (ActividadSeleccionada.IdDepartamento == iddepa)
                        {
                            NuevaImagen = "";
                            Error = "";
                            if (ActividadSeleccionada.Estado == 0)
                            {   
                                var editarborradorview = new EditarBorradorView(this);
                                Shell.Current.Navigation.PushAsync(editarborradorview);
                            }
                            else
                            {
                                var editarview = new EditarActividadView(this);
                                Shell.Current.Navigation.PushAsync(editarview);
                            }
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("//AvisoEditar");
                        }

                    }
                }
            }
            

            
        }

        [RelayCommand]
        public async void VerEliminarActividad(int id)
        {

            int iddepa = await loginService.GetDepartmentoId();
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.GetFromJsonAsync<List<DepartamentoDTO>>($"api/DepartamentosAPI/ObtenerDepartamentos");
            if (response != null)
            {
                if (response.Any(x => x.Id == iddepa) == false)
                {
                    SecureStorage.Remove("tkn");
                    await Shell.Current.GoToAsync("//SesionCaducada");
                }
                else
                {
                    ActividadSeleccionada = repositoryActividades.Get(id);
                    if (ActividadSeleccionada != null)
                    {
                        Shell.Current.GoToAsync("//EliminarActividad");
                    }
                }
            }

            
        }

        [RelayCommand]
        public async void Nuevo()
        {
            int iddepa = await loginService.GetDepartmentoId();
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.GetFromJsonAsync<List<DepartamentoDTO>>($"api/DepartamentosAPI/ObtenerDepartamentos");
            if (response != null)
            {
                if (response.Any(x => x.Id == iddepa) == false)
                {
                    SecureStorage.Remove("tkn");
                    await Shell.Current.GoToAsync("//SesionCaducada");
                }
                else
                {
                    Error = "";
                    Actividad = new();
                    NuevaImagen = "";
                    var agregarview = new AgregarActividadView(this);
                    Shell.Current.Navigation.PushAsync(agregarview);
                }
            }
            
        }

        [RelayCommand]
        public void Cancelar()
        {
            Actividad = null;
            Error = "";
            VerListadoActividades();
        }

        [RelayCommand]
        public async void VerBorradores()
        {
            ActualizarBorradores();
            await Shell.Current.GoToAsync("//ListadoBorradores");
        }


        [RelayCommand]
        public async Task AgregarBorrador()
        {
            try
            {
                BorradorValidator validator = new();
                var borradorvalidado = validator.Validate(actividad);
                if(borradorvalidado.IsValid)
                {
                    actividad.IdDepartamento = await loginService.GetDepartmentoId();
                    if (Actividad != null)
                    {
                        await service.AgregarBorrador(actividad);
                        ActualizarActividades();
                        Actividad = null;
                        Error = "";
                        VerBorradores();
                    }
                }
                else
                {
                    Error = string.Join("\n", borradorvalidado.Errors.Select(x => x.ErrorMessage));
                }

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        [RelayCommand]
        public async Task PublicarBorrador()
        {
            try
            {
                if (ActividadSeleccionada != null)
                {
                    ActividadEditarValidator validator = new();
                    var actividadvalidada = validator.Validate(ActividadSeleccionada);
                    if (actividadvalidada.IsValid)
                    {
                        var actividad = new ActividadDTO()
                        {
                            Titulo = ActividadSeleccionada.Titulo,
                            Descripcion = ActividadSeleccionada.Descripcion,
                            FechaRealizacion = ActividadSeleccionada.FechaRealizacion,
                            Estado = ActividadSeleccionada.Estado,
                            FechaActualizacion = ActividadSeleccionada.FechaActualizacion,
                            FechaCreacion = ActividadSeleccionada.FechaCreacion,
                            Id = ActividadSeleccionada.Id,
                            IdDepartamento = ActividadSeleccionada.IdDepartamento,
                            Imagen = ActividadSeleccionada.Imagen
                        };
                        await service.PublicarBorrador(actividad);
                        ActualizarActividades();
                        ActualizarBorradores();
                        ActividadSeleccionada = null;
                        Error = "";
                        VerListadoBorradores();
                    }
                    else
                    {
                        Error = string.Join("\n", actividadvalidada.Errors.Select(x => x.ErrorMessage));
                    }
                }

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }


        [RelayCommand]
        public async Task Agregar()
        {
            try
            {
                if(Actividad != null)
                {
                    
                    Actividad.IdDepartamento = await loginService.GetDepartmentoId();
                    Actividad.Imagen = NuevaImagen;
                    if(Actividad.FechaRealizacion == null)
                        Actividad.FechaRealizacion = DateTime.Now;
                    if (Actividad != null)
                    {
                        var resultado = validador.Validate(Actividad);
                        if (resultado.IsValid)
                        {
                            await service.Agregar(Actividad);
                            ActualizarActividades();
                            ActualizarBorradores();
                            Cancelar();
                        }
                        else
                        {
                            Error = string.Join("\n", resultado.Errors.Select(x => x.ErrorMessage));
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        [RelayCommand]
        public async Task Eliminar()
        {
            try
            {
                if (ActividadSeleccionada != null)
                {
                    await service.Eliminar(ActividadSeleccionada.Id);
                    ActividadSeleccionada = null;
                    ActualizarActividades();
                    Cancelar();
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        [RelayCommand]
        public async Task Editar()
        {
            try
            {
                if (ActividadSeleccionada != null)
                {
                    ActividadEditarValidator validator = new();
                    var actividadvalidada = validator.Validate(ActividadSeleccionada);
                    ActividadSeleccionada.Imagen = NuevaImagen;
                    if (actividadvalidada.IsValid)
                    {
                        var actividad = new ActividadDTO()
                        {   
                            Titulo = ActividadSeleccionada.Titulo,
                            Descripcion = ActividadSeleccionada.Descripcion,
                            FechaRealizacion = ActividadSeleccionada.FechaRealizacion,
                            Estado = ActividadSeleccionada.Estado,
                            FechaActualizacion = ActividadSeleccionada.FechaActualizacion,
                            FechaCreacion = ActividadSeleccionada.FechaCreacion,
                            Id = ActividadSeleccionada.Id,
                            IdDepartamento = ActividadSeleccionada.IdDepartamento,
                            Imagen = ActividadSeleccionada.Imagen
                        };
                            await service.Editar(actividad);
                            ActualizarActividades();
                            ActualizarBorradores();
                            Cancelar();
                        NuevaImagen = "";
                    }
                    else
                    {
                        Error = string.Join("\n", actividadvalidada.Errors.Select(x => x.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }



        public void ActualizarActividades()
         {
           ListadoActividades.Clear();
           foreach (var item in repositoryActividades.GetAll().OrderByDescending(x => x.FechaCreacion).Where(x => x.Estado == 1))
           {
             ListadoActividades.Add(item);
           }
        }

        public async void ActualizarBorradores()
        {
            int id = await loginService.GetDepartmentoId();
            ListadoBorradores.Clear();
            foreach (var item in repositoryActividades.GetAll().OrderByDescending(x => x.FechaCreacion).Where(x => x.Estado == 0 && x.IdDepartamento == id))
            {
                ListadoBorradores.Add(item);
            }
        }
    }
}
