using FluentValidation;
using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Validators
{
    public class LoginValidator:AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("- Ingrese un correo electrónico.");
            RuleFor(x => x.Username)
            .EmailAddress().WithMessage("- Ingrese un formato de correo válido para el usuario")
            .When(x => CorreoDepartamento(x.Username) == true);

            RuleFor(x => x.Username).Must(CorreoDepartamento).WithMessage("- Ese usuario no pertenece a esta área.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("- Ingrese la contraseña.");
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
    }
}
