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
    
    public async Task<Result<CategoryResponse>> GetCategoryById(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + id,
            Method = HttpMethod.Get,
            ExpectedFailCodes = [HttpStatusCode.NotFound]
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }

    public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest dto)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath,
            Method = HttpMethod.Post,
            ExpectedFailCodes = [HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity],
            Body = dto
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }

    public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest dto)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + id,
            Method = HttpMethod.Put,
            ExpectedFailCodes =
            [
                HttpStatusCode.BadRequest,
                HttpStatusCode.UnprocessableEntity,
                HttpStatusCode.NotFound
            ],
            Body = dto
        };

        return await apiRequestService.SendAsync<CategoryResponse>(options);
    }

    public async Task<Result> DeleteCategory(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = CategoryPath + "/" + id,
            Method = HttpMethod.Delete,
            ExpectedFailCodes = [HttpStatusCode.NotFound]
        };

        return await apiRequestService.SendAsync(options);
    }
}