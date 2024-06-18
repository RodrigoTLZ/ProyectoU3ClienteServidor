using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Entities
{
    public class Actividad
    {
        [PrimaryKey]
        public int Id { get; set; }

        [NotNull]
        public string Titulo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;
        public string? Imagen { get; set; }
        public string NombreDepartamento { get; set; } = null!;

        public DateTime? FechaRealizacion { get; set; }

        [NotNull]
        public int IdDepartamento { get; set; }

        [NotNull]
        public DateTime FechaCreacion { get; set; }

        [NotNull]
        public DateTime FechaActualizacion { get; set; }

        [NotNull]
        public int Estado { get; set; }




        public ImageSource ImagenSource
        {
            get
            {
                if (string.IsNullOrEmpty(Imagen))
                    return null;

                byte[] imageBytes = Convert.FromBase64String(Imagen);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
        }
    }
}
