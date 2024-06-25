using System.Net.Http.Headers;

namespace Appointments.Infrastructure.Repositories;

public class DocumentsRepository
{
    public HttpClient HttpClient { get; set; }

    public DocumentsRepository(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://localhost:7208/api/documents");

        HttpClient = httpClient;
    }

    public async Task UploadPdfFileAsync(byte[] file, string fileName)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(file);

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

        form.Add(fileContent, "file", fileName);

        var response = await HttpClient.PostAsync(string.Empty, form);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePdfFileAsync(string fileName)
    {
        var response = await HttpClient.DeleteAsync($"/api/documents/{fileName}");

        response.EnsureSuccessStatusCode();
    }
}
