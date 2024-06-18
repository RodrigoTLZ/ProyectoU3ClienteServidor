using FluentValidation;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ProyectoU3ClienteServidor.Services;

namespace ProyectoU3ClienteServidor.Models.Validators
{
    public class DepartamentoEditarValidator:AbstractValidator<Departamentos>
    {
        LoginService loginService = new();
        HttpClient cliente = new HttpClient()
        {
            BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
        };

        public DepartamentoEditarValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("- El nombre del departamento no debe estar vacío.");
            RuleFor(x => x.Nombre).MustAsync((Departamento, Nombre, cancellationToken) => ValidarNombreExistente(Nombre, Departamento.Id, cancellationToken)).WithMessage("- Ya se encuentra registrado un departamento con ese nombre.");

            RuleFor(x => x.Username).Must(CorreoDepartamento).WithMessage("- Ingrese un correo perteneciente a esta área.");
            RuleFor(x => x.Username).NotEmpty().WithMessage("- Ingrese el nombre de usuario.");

            RuleFor(x => x.Username)
            .EmailAddress().WithMessage("- Ingrese un formato de correo válido para el usuario")
            .When(x => CorreoDepartamento(x.Username) == true);

            RuleFor(x => x.Username).MustAsync((Departamento, Username, cancellationToken) => ValidarCorreoExistente(Username, Departamento.Id, cancellationToken)).WithMessage("El nombre de usuario ya se encuentra registrado.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("- Ingrese una contraseña.");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }



        private async Task<bool> ValidarCorreoExistente(string correo, int id, CancellationToken cancellationToken)
        {
            if (correo != null)
            {
                string token = await loginService.GetToken();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await cliente.GetAsync("api/DepartamentosAPI/ObtenerDepartamentos");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var listadousuarios = JsonConvert.DeserializeObject<IEnumerable<DepartamentoDTO>>(data);
                    if (listadousuarios != null)
                    {
                        if (listadousuarios.Any(u => u.Username.ToUpper() == correo.ToUpper() && u.Id != id))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CorreoDepartamento(string username)
        {
            if (username != null)
            {
                if (username.Contains("@equipo7"))
                {
                    return true;
                }
                return false;
            }
            return true;
        }


        private async Task<bool> ValidarNombreExistente(string nombre, int id, CancellationToken cancellationToken)
        {
            if (nombre != null)
            {
                string token = await loginService.GetToken();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await cliente.GetAsync("api/DepartamentosAPI/ObtenerDepartamentos");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var listadousuarios = JsonConvert.DeserializeObject<IEnumerable<DepartamentoDTO>>(data);
                    if (listadousuarios != null)
                    {
                        if (listadousuarios.Any(u => u.Nombre.ToUpper() == nombre.ToUpper() && u.Id != id))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


    }
}
