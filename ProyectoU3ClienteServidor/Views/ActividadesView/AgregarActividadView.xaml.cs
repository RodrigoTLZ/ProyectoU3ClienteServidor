namespace ProyectoU3ClienteServidor.Views.ActividadesView;
using Microsoft.Maui.Storage;
using ProyectoU3ClienteServidor.ViewModels;
using System.IO;

public partial class AgregarActividadView : ContentPage
{
	public AgregarActividadView(ActividadesViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }
    private byte[] selectedImageBytes;

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Seleccione una imagen para la actividad",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
                return;

            using (var stream = await result.OpenReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    selectedImageBytes = memoryStream.ToArray();
                }
                myImage.Source = ImageSource.FromStream(() => new MemoryStream(selectedImageBytes));
                string base64Image = Convert.ToBase64String(selectedImageBytes);
                Imgbase64.Text = base64Image;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
;
        }
    }
}