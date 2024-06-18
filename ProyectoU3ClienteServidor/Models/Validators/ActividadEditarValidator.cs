using FluentValidation;
using ProyectoU3ClienteServidor.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Validators
{
    public class ActividadEditarValidator : AbstractValidator<Actividad>
    {
        public ActividadEditarValidator()
        {
            RuleFor(x => x.Titulo).MaximumLength(100).WithMessage("- El titulo debe tener un tamaño menor a 100 caracteres.");
            RuleFor(x => x.Descripcion).MaximumLength(250).WithMessage("- Ingrese una descripción menor a 250 caracteres.");
            RuleFor(x => x.FechaRealizacion).Must(ValidarFecha).WithMessage("- La fecha de realización no debe ser mayor a la fecha actual.");
        }
        private bool ValidarFecha(DateTime? date)
        {
            DateTime currentDate = DateTime.Now.Date;
            if (date.Value.Date <= currentDate.Date)
            {
                return true;
            }
            return false;
        }
    }
}
