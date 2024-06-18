using ProyectoU3ClienteServidor.Models.DTOs;
using ProyectoU3ClienteServidor.Models.Entities;
using ProyectoU3ClienteServidor.Services;
using ProyectoU3ClienteServidor.ViewModels;
using ProyectoU3ClienteServidor.Views;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoU3ClienteServidor
{
    public partial class App : Application
    {

        public static ActividadesService ActividadesService { get; set; } = new();
        Repositories.RepositoryGeneric<Actividad> repository = new();
        Repositories.RepositoryGeneric<Departamentos> deparepository = new();
        public static ActividadesViewModel ActividadesViewModel { get; set; } = new();


        public LoginService loginService { get; set; } = new();
        HttpClient cliente;

        public App()
        {

            Preferences.Clear();
            repository.DeleteAll();
            SecureStorage.RemoveAll();

            cliente = new()
            {
                BaseAddress = new Uri("https://actividadesteam7.websitos256.com/")
            };
            InitializeComponent();
            Thread thread = new Thread(Sincronizador) { IsBackground = true };
            thread.Start();
            MainPage = new AppShell();
        }

        async void Sincronizador()
        {
            while (true)
            {
                try
                {
                    string token = await SecureStorage.GetAsync("tkn");
                    if(token != null)
                    {
                        int id = await loginService.GetDepartmentoId();
                        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        await ActividadesService.GetActividades(id);
                        Thread.Sleep(TimeSpan.FromSeconds(15));
                    }
                }
                catch (Exception)
                {

                }
                
            }
        }
    }
}
