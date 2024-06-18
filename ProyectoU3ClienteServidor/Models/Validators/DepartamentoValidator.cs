using FluentValidation;
using Newtonsoft.Json;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Validators
{
    public class DepartamentoValidator:AbstractValidator<DepartamentoDTO>
    {
        LoginService loginService = new();


        HttpClient client = new()
        {
            BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
        };

        public DepartamentoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("- El nombre del departamento no debe estar vacío.");
            RuleFor(x => x.Nombre).MustAsync(ValidarNombreExistente).WithMessage("Ya se encuentra registrado un departamento con ese nombre.");


            RuleFor(x => x.Username).NotEmpty().WithMessage("- Ingrese el nombre de usuario.");
            RuleFor(x => x.Username)
            .EmailAddress().WithMessage("- Ingrese un formato de correo válido para el usuario")
            .When(x => CorreoDepartamento(x.Username) == true);
            RuleFor(x => x.Username).MustAsync(Comparar).WithMessage("- El nombre de usuario ya se encuentra registrado.");
            RuleFor(x => x.Username).Must(CorreoDepartamento).WithMessage("- Ingrese un correo perteneciente a esta área.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("- Ingrese una contraseña.");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("- La contraseña debe contar con al menos 6 caracteres.");
        }


        private async Task<bool> Comparar(string username, CancellationToken cancellationToken)
        {
            if (username != null)
            {
                string token = await loginService.GetToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/DepartamentosAPI/ObtenerDepartamentos");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var listadousuarios = JsonConvert.DeserializeObject<IEnumerable<DepartamentoDTO>>(data);
                    if (listadousuarios != null)
                    {
                        if (listadousuarios.Any(u => u.Username.ToUpper() == username.ToUpper()))
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

        private async Task<bool> ValidarNombreExistente(string nombre, CancellationToken cancellationToken)
        {
            if (nombre != null)
            {
                string token = await loginService.GetToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/DepartamentosAPI/ObtenerDepartamentos");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var listadousuarios = JsonConvert.DeserializeObject<IEnumerable<DepartamentoDTO>>(data);
                    if (listadousuarios != null)
                    {
                        if (listadousuarios.Any(u => u.Nombre.ToUpper() == nombre.ToUpper()))
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
