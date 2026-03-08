using ECommerce.Api.Infrastructure.EF;

namespace ECommerce.Api.Application.Services.Mapping;

public interface IEntityDtoMapper<TEntity, out TResponseDto, in TCreateDto, in TUpdateDto>
{
    TResponseDto ToDto(TEntity entity);
    TEntity ToEntity(TCreateDto dto);
    TEntity GetUpdatedEntity(TEntity entity, TUpdateDto dto);
}

public interface IEntityDtoAsyncMapper<TEntity, TResponseDto, in TCreateDto, in TUpdateDto>
{
    TResponseDto ToDto(TEntity entity);
    Task<TEntity> ToEntityAsync(TCreateDto dto, ECommerceContext context);
    Task<TEntity> GetUpdatedEntityAsync(TEntity entity, TUpdateDto dto, ECommerceContext context);
}