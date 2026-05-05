using System.Net;
using System.Text;
using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class CategoryService(ApiRequestService apiRequestService)
{
    private const string CategoryPath = "api/categories";

    public async Task<Result<IEnumerable<CategoryResponse>>> GetCategories(
        string? search = null,
        PaginationQuery? paginationQuery = null)
    {
        var query = new StringBuilder($"?search={search ?? ""}");
        if (paginationQuery != null)
            query.Append($"&limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = CategoryPath + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAsync<IEnumerable<CategoryResponse>>(options);
    }

    public async Task<Result<CategoryResponse>> GetCategory(int id)
    {
        return await GetCategoryInner(id.ToString());
    }
    
    public async Task<Result<CategoryResponse>> GetCategory(string slug)
    {
        return await GetCategoryInner(slug);
    }
    
    private async Task<Result<CategoryResponse>> GetCategoryInner(string category)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + category,
            Method = HttpMethod.Get,
            ExpectedFailCodes = [HttpStatusCode.NotFound]
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }

    public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath,
            Method = HttpMethod.Post,
            ExpectedFailCodes = [HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity],
            Body = request
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }
    
    public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
    {
        return await UpdateCategoryInner(id.ToString(), request);
    }
    
    public async Task<Result<CategoryResponse>> UpdateCategory(string slug, CategoryRequest request)
    {
        return await UpdateCategoryInner(slug, request);
    }

    private async Task<Result<CategoryResponse>> UpdateCategoryInner(string category, CategoryRequest request)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + category,
            Method = HttpMethod.Put,
            ExpectedFailCodes =
            [
                HttpStatusCode.BadRequest,
                HttpStatusCode.UnprocessableEntity,
                HttpStatusCode.NotFound
            ],
            Body = request
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }
    
    public async Task<Result> DeleteCategory(int id)
    {
        return await DeleteCategoryInner(id.ToString());
    }
    
    public async Task<Result> DeleteCategory(string slug)
    {
        return await DeleteCategoryInner(slug);
    }

    private async Task<Result> DeleteCategoryInner(string category)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + category,
            Method = HttpMethod.Delete,
            ExpectedFailCodes = [HttpStatusCode.NotFound]
        };

        return await apiRequestService.SendAsync(options);
    }
}