using ECommerce.Api.Infrastructure.EF;

namespace ECommerce.Api.Application.Services.Mapping;

public interface IEntityDtoMapper<TEntity, out TResponseDto, in TCreateDto, in TUpdateDto>
{
    TResponseDto ToDto(TEntity entity);
    TEntity ToEntity(TCreateDto dto);
    TEntity UpdateEntity(TEntity entity, TUpdateDto dto);
}

public interface IEntityDtoAsyncMapper<TEntity, TResponseDto, in TCreateDto, in TUpdateDto>
{
    TResponseDto ToDto(TEntity entity);
    Task<TEntity> ToEntityAsync(TCreateDto dto, ECommerceContext context);
    Task<TEntity> UpdateEntityAsync(TEntity entity, TUpdateDto dto, ECommerceContext context);
}