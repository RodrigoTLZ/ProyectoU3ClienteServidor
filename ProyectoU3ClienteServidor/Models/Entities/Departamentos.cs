using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Models.Entities
{
    [Table("Departamentos")]
    public class Departamentos
    {
        [PrimaryKey]
        public int Id { get; set; }

        [NotNull]
        public string Nombre { get; set; } = null!;

        [NotNull]
        public string Username { get; set; } = null!;

        [NotNull]
        public string Password { get; set; } = null!;

        public int IdSuperior { get; set; }
    }
}
