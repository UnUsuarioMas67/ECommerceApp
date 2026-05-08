using System.Net;
using System.Text;
using ECommerce.Dashboard.DTOs.Product;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class ProductService(ApiRequestService apiRequestService)
{
    private const string ProductsPath = "api/products";

    public async Task<Result<IEnumerable<ProductResponse>>> GetProducts(
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

        return await apiRequestService.SendAsync<IEnumerable<ProductResponse>>(options);
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
        var options = new ApiRequestOptions
        {
            Path = ProductsPath,
            Method = HttpMethod.Post,
            Body = dto
        };

        return await apiRequestService.SendAsync<ProductResponse>(options);
    }

    public async Task<Result<ProductResponse>> UpdateProduct(int id, ProductUpdate dto)
    {
        var options = new ApiRequestOptions
        {
            Path = ProductsPath + "/" + id,
            Method = HttpMethod.Put,
            Body = dto
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
}