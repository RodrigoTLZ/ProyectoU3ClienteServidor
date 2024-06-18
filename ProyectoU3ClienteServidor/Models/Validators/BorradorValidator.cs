using FluentValidation;
using ProyectoU3ClienteServidor.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Validators
{
    public class BorradorValidator:AbstractValidator<ActividadDTO>
    {
        public BorradorValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("- Debe ingresar como mínimo el título antes de guardar como borrador.");
        }
    }
}
