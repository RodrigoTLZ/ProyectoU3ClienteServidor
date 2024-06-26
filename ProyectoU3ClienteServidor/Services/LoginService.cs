﻿using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Services
{
    public class LoginService
    {

        HttpClient cliente = new()
        {
            BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
        };
        Repositories.RepositoryGeneric<Actividad> repositoryActividades = new();


        public async Task<string> Login(LoginDTO dto)
        {
            var dato = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync($"api/Login", dato);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return null;//implementar error
            }
        }
        public async Task<string> GetToken()
        {
            string token = await SecureStorage.GetAsync("tkn");
            return token;
        }

        public async Task<int> GetDepartmentoId()
        {
            var token = await GetToken();
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                var departamentoclaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "DepartamentoId");

                if (departamentoclaim != null)
                {
                    return int.Parse(departamentoclaim.Value);
                }
            }
            return 0;
        }

        public async Task<string> GetRol()
        {
            var token = await GetToken();
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                var roldepartamento = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                if (roldepartamento != null)
                {
                    return roldepartamento.Value;
                }
            }
            return "Este departamento no cuenta con ningun rol asignado.";
        }


        public async Task<bool> Validar(string token)
        {
            var response = await cliente.GetAsync($"api/login/{token}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
