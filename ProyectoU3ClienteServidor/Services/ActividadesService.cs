using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Services
{
    public class ActividadesService
    {
        HttpClient cliente;
        Repositories.RepositoryGeneric<Actividad> repository = new();
        Repositories.RepositoryGeneric<Departamentos> depasRepository = new();
        public event Action? ActualizarDatos;
        public event Action? CambiarListado;
        LoginService loginService = new();
        public ActividadesService()
        {
            cliente = new()
            {
                BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
            };
        }

        public async Task Agregar(ActividadDTO dto)
        {
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.PostAsJsonAsync("api/ActividadesAPI/Publicar", dto);
            var departamentoId = await loginService.GetDepartmentoId();
            if (response.IsSuccessStatusCode)
            {
                await GetActividades(departamentoId);
            }
            else
            {
                var errores = await response.Content.ReadAsStringAsync();
                throw new Exception(errores);
            }
        }

        public async Task AgregarBorrador(ActividadDTO dto)
        {
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.PostAsJsonAsync("api/ActividadesAPI/GuardarBorrador", dto);
            var departamentoId = await loginService.GetDepartmentoId();
            if (response.IsSuccessStatusCode)
            {
                await GetActividades(departamentoId);
            }
            else
            {
                var errores = await response.Content.ReadAsStringAsync();
                throw new Exception(errores);
            }
        }

        public async Task PublicarBorrador(ActividadDTO dto)
        {
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.PostAsJsonAsync("api/ActividadesAPI/PublicarBorrador", dto);
            var departamentoId = await loginService.GetDepartmentoId();
            if (response.IsSuccessStatusCode)
            {
                await GetActividades(departamentoId);
            }
            else
            {
                var errores = await response.Content.ReadAsStringAsync();
                throw new Exception(errores);
            }
        }

        public async Task Eliminar(int id)
        {
            string token = await loginService.GetToken();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await cliente.DeleteAsync($"api/ActividadesAPI/{id}");
            var departamentoId = await loginService.GetDepartmentoId();

            if (response.IsSuccessStatusCode)
            {
                await GetActividades(departamentoId);
            }
        }

        public async Task Editar(ActividadDTO dto)
        {
            var departamentoId = await loginService.GetDepartmentoId();

            if (departamentoId != 0)
            {
                string token = await loginService.GetToken();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await cliente.PutAsJsonAsync($"api/ActividadesAPI/{dto.Id}", dto);
                if (response.IsSuccessStatusCode)
                {
                    await GetActividades(departamentoId);
                }
            }
            else
            {
                throw new InvalidOperationException("No se pudo obtener el departamento ID.");
            }
        }


        public async Task GetActividades(int id)
        {
            try
            {
                var fecha = Preferences.Get($"FechaDeActividadesD{id}", DateTime.MinValue);
                bool aviso = false;
                string token = await loginService.GetToken();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await cliente.GetFromJsonAsync<List<ActividadDTO>>($"api/ActividadesAPI/{fecha:yyyy-MM-dd}/{fecha:HH}/{fecha:mm}/{id}");

                if (response != null)
                {
                    foreach (ActividadDTO actividad in response)
                    {
                        string depa = depasRepository.Get(actividad.IdDepartamento).Nombre;
                        var entidad = repository.Get(actividad.Id);

                        if (entidad == null)
                        {
                            entidad = new()
                            {
                                Id = actividad.Id,
                                Descripcion = actividad.Descripcion ?? "",
                                Estado = actividad.Estado,
                                FechaRealizacion = actividad.FechaRealizacion ?? DateTime.Now.Date,
                                FechaActualizacion = actividad.FechaActualizacion,
                                FechaCreacion = actividad.FechaCreacion,
                                IdDepartamento = actividad.IdDepartamento,
                                Titulo = actividad.Titulo,
                                Imagen = actividad.Imagen,
                                NombreDepartamento = depa
                                
                            };
                            repository.Insert(entidad);
                            aviso = true;
                        }

                        else
                        {
                            if (entidad != null)
                            {
                                if (actividad.Estado == 2)
                                {
                                    repository.Delete(entidad);
                                    aviso = true;
                                }
                                else
                                {
                                    if (actividad.Titulo != entidad.Titulo ||
                                        actividad.Estado != entidad.Estado ||
                                        actividad.FechaActualizacion != entidad.FechaActualizacion ||
                                        actividad.FechaRealizacion != entidad.FechaRealizacion ||
                                        actividad.Descripcion != entidad.Descripcion ||
                                        actividad.Imagen != entidad.Imagen)
                                           
                                    {
                                        entidad.Titulo = actividad.Titulo;
                                        entidad.Estado = actividad.Estado;
                                        entidad.FechaActualizacion = actividad.FechaActualizacion;
                                        entidad.FechaRealizacion = actividad.FechaRealizacion;
                                        entidad.Descripcion = actividad.Descripcion?? "";
                                        entidad.Imagen = actividad.Imagen;
                                        repository.Update(entidad);
                                        aviso = true;
                                    }
                                }
                            }
                        }
                    }

                    if (aviso)
                    {
                        _ = MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            ActualizarDatos?.Invoke();
                        });
                        Preferences.Set($"FechaDeActividadesD{id}", DateTime.UtcNow);
                    }

                }

                
            }
            catch (Exception)
            {

            }
        }
    }
}
