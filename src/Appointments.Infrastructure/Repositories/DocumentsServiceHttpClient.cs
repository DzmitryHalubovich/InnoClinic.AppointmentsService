using System.Net.Http.Headers;

namespace Appointments.Infrastructure.Repositories;

public class DocumentsServiceHttpClient
{
    private readonly HttpClient _httpClient;

    public DocumentsServiceHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://localhost:7208/api/documents");

        _httpClient = httpClient;
    }

    public async Task UploadPdfFileAsync(byte[] file, string fileName)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(file);

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

        form.Add(fileContent, "file", fileName);

        var response = await _httpClient.PostAsync(string.Empty, form);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePdfFileAsync(string fileName)
    {
        var response = await _httpClient.DeleteAsync($"/api/documents/{fileName}");

        response.EnsureSuccessStatusCode();
    }
}
