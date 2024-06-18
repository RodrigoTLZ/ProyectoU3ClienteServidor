using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using ProyectoU3ClienteServidor.Models.Validators;
using ProyectoU3ClienteServidor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.ViewModels
{
    public partial class DepartamentosViewModel:ObservableObject
    {
        Repositories.RepositoryGeneric<Departamentos> depaRepository = new();

        public ObservableCollection<Departamentos> ListadoDepartamentos { get; set; } = new();

        DepartamentosService service = new();
        DepartamentoValidator validador = new();
        LoginService loginservice { get; set; } = new();


        [ObservableProperty]
        public Departamentos departamentoSuperior;


        public Departamentos DepartamentoSeleccionado { get; set; }

        public DepartamentosViewModel()
        {
            ActualizarDepartamentos();
            service.ActualizarDatos += Service_ActualizarDatos;
        }

        private void Service_ActualizarDatos()
        {
            ActualizarDepartamentos();
        }

        [ObservableProperty]
        private DepartamentoDTO departamento;

        [ObservableProperty]
        public string error = "";

        [RelayCommand]
        public void Nuevo()
        {
            Departamento = new();
            ListadoDepartamentos.Clear();
            foreach (var item in depaRepository.GetAll().OrderBy(x => x.Nombre))
            {
                ListadoDepartamentos.Add(item);
            }

            Shell.Current.GoToAsync("//AgregarDepartamento");
        }


        [RelayCommand]
        public void VerDepartamentos()
        {
            ActualizarDepartamentos();
            Shell.Current.GoToAsync("//ListadoDepartamentos");
        }

        [RelayCommand]
        public void VerEliminarDepartamento(int id)
        {

            DepartamentoSeleccionado = depaRepository.Get(id);
            if (DepartamentoSeleccionado != null)
            {
                Shell.Current.GoToAsync("//EliminarDepartamento");
            }
        }


        [RelayCommand]
        public void VerEditarDepartamento(int id)
        {
            DepartamentoSeleccionado = depaRepository.Get(id);
            DepartamentoSuperior = depaRepository.Get(DepartamentoSeleccionado.IdSuperior);
            if (DepartamentoSeleccionado != null)
            {
                Error = "";
                Shell.Current.GoToAsync("//EditarDepartamento");
            }
        }


        [RelayCommand]
        public async Task Editar()
        {
            try
            {
                if (DepartamentoSeleccionado != null)
                {
                    DepartamentoSeleccionado.IdSuperior = departamentoSuperior.Id;
                    DepartamentoEditarValidator validator = new();

                    var departamentovalidado = await validator.ValidateAsync(DepartamentoSeleccionado);
                    if (departamentovalidado.IsValid)
                    {
                        var depa = new DepartamentoDTO()
                        {
                            Id = DepartamentoSeleccionado.Id,
                            IdSuperior = DepartamentoSeleccionado.IdSuperior,
                            Nombre = DepartamentoSeleccionado.Nombre,
                            Username = DepartamentoSeleccionado.Username,
                            Password = DepartamentoSeleccionado.Password
                        };
                        await service.EditarDepartamento(depa);
                       
                        Cancelar();
                    }
                    else
                    {
                        Error = string.Join("\n", departamentovalidado.Errors.Select(x => x.ErrorMessage));

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
                if (DepartamentoSeleccionado != null)
                {
                    await service.EliminarDepartamento(DepartamentoSeleccionado.Id);
                    Cancelar();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        [RelayCommand]
        public void Cancelar()
        {
            Departamento = null;
            DepartamentoSuperior = null;
            Error = "";
            VerDepartamentos();
        }


        [RelayCommand]
        public async Task Agregar()
        {
            try
            {
                if (Departamento != null)
                {
                    if(DepartamentoSuperior != null)
                    {
                        Departamento.IdSuperior = DepartamentoSuperior.Id;
                    }
                    var resultado = await validador.ValidateAsync(Departamento);
                    if (resultado.IsValid)
                    {
                        await service.Agregar(Departamento);
                        ActualizarDepartamentos();
                        Cancelar();
                    }
                    else
                    {
                        Error = string.Join("\n", resultado.Errors.Select(x => x.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }



        async void ActualizarDepartamentos()
        {
            int iddepa = await loginservice.GetDepartmentoId();
            ListadoDepartamentos.Clear();
            foreach (var item in depaRepository.GetAll().OrderBy(x=>x.Nombre).Where(x=> x.Id != iddepa))
            {
                ListadoDepartamentos.Add(item);
            }
        }

        
    }
}
