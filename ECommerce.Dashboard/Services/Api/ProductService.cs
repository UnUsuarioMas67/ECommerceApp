using System.Globalization;
using System.Net;
using System.Text;
using ECommerce.Dashboard.DTOs.Product;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class ProductService(ApiRequestService apiRequestService)
{
    private const string ProductsPath = "api/products";
    private const string AntiforgeryPath = "api/antiforgery";

    public async Task<PaginatedResponse<ProductResponse>> GetProducts(
        string? search = null,
        PaginationQuery? paginationQuery = null,
        string? category = null)
    {
        var route = category != null ? ProductsPath + "/categories/" + category : ProductsPath;

        var query = new StringBuilder($"?search={search ?? ""}");
        if (paginationQuery != null)
            query.Append($"&limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = route + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAlwaysSucceedAsync<PaginatedResponse<ProductResponse>>(options);
    }

    public async Task<Result<ProductResponse>> GetProductById(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = ProductsPath + "/" + id,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAsync<ProductResponse>(options);
    }

    public async Task<Result<ProductResponse>> CreateProduct(ProductCreate dto)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(dto.Category, Encoding.UTF8), "category");
        content.Add(new StringContent(dto.Sku, Encoding.UTF8), "sku");
        content.Add(new StringContent(dto.Name, Encoding.UTF8), "name");
        content.Add(new StringContent(dto.Description, Encoding.UTF8), "description");
        content.Add(new StringContent(dto.Price.ToString(CultureInfo.InvariantCulture)), "price");
        content.Add(new StringContent(dto.InitialStock.ToString(CultureInfo.InvariantCulture)), "initialStock");

        if (dto.ImageFile != null)
        {
            // Add image to content
            content.Add(new StreamContent(dto.ImageFile.OpenReadStream()), "imageFile", dto.ImageFile.FileName);
        }
        
        var antiforgery = await GetAntiforgeryToken();
        content.Add(new StringContent(antiforgery.RequestToken), "__RequestVerificationToken");

        var options = new ApiRequestOptionsMultipartForm
        {
            Path = ProductsPath,
            Method = HttpMethod.Post,
            Content = content
        };

        return await apiRequestService.SendAsync<ProductResponse>(options);
    }

    public async Task<Result<ProductResponse>> UpdateProduct(int id, ProductUpdate dto)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(dto.Category, Encoding.UTF8), "category");
        content.Add(new StringContent(dto.Name, Encoding.UTF8), "name");
        content.Add(new StringContent(dto.Description, Encoding.UTF8), "description");
        content.Add(new StringContent(dto.Price.ToString(CultureInfo.InvariantCulture)), "price");

        if (dto.ImageFile != null)
        {
            // Add image to content
            content.Add(new StreamContent(dto.ImageFile.OpenReadStream()), "imageFile", dto.ImageFile.FileName);
        }
        
        var antiforgery = await GetAntiforgeryToken();
        content.Add(new StringContent(antiforgery.RequestToken), "__RequestVerificationToken");

        var options = new ApiRequestOptionsMultipartForm
        {
            Path = ProductsPath + $"/{id}",
            Method = HttpMethod.Put,
            Content = content
        };

        return await apiRequestService.SendAsync<ProductResponse>(options);
    }

    public async Task<Result> DeleteProduct(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = ProductsPath + "/" + id,
            Method = HttpMethod.Delete,
        };

        return await apiRequestService.SendAsync(options);
    }

    public async Task<Result> RestockProduct(int id, int amount)
    {
        var options = new ApiRequestOptions
        {
            Path = ProductsPath + "/" + id + "/restock?amount=" + amount,
            Method = HttpMethod.Put,
        };

        return await apiRequestService.SendAsync<ProductResponse>(options);
    }

    private async Task<AntiforgeryToken> GetAntiforgeryToken()
    {
        var options = new ApiRequestOptions()
        {
            Path = AntiforgeryPath,
            Method = HttpMethod.Get
        };
        
        return await apiRequestService.SendAlwaysSucceedAsync<AntiforgeryToken>(options);
    }
}