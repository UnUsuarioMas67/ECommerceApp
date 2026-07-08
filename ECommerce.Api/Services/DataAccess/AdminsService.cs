using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.DTOs.User;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.DataAccess;

public interface IAdminsService
{
    Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto);
    Task<UserResponseDto?> GetByIdAsync(int adminId);
    Task<IEnumerable<UserResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null);
    Task<Result<UserResponseDto>> UpdateAsync(int adminId, UserUpdateDto dto);
    Task<UserResponseDto?> DeleteAsync(int adminId);
}

public class AdminsService(ECommerceContext context, IValidator<Admin> validator, AdminMapper mapper)
    : IAdminsService
{
    public async Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto)
    {
        var admin = mapper.MapToEntity(dto);

        var verification = await VerifyAdmin(admin);
        if (!verification.IsSuccess)
            return verification.Error;

        await context.Admins.AddAsync(admin);
        await context.SaveChangesAsync();

        return mapper.MapToDto(admin);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int adminId)
    {
        var admin = await context.Admins.FindAsync(adminId);
        return admin != null ? mapper.MapToDto(admin) : null;
    }

    public async Task<IEnumerable<UserResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        return await context.Admins
            .AsNoTracking()
            .Where(a => (a.FirstName + " " + a.LastName).Contains(search ?? ""))
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .Select(a => mapper.MapToDto(a))
            .ToListAsync();
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(int adminId, UserUpdateDto dto)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin == null)
            return new NotFoundError();
        
        if (dto.PasswordUpdate != null)
        {
            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.PasswordUpdate.OldPassword, admin.PasswordHash);
            if (!passwordValid)
                return new IncorrectPasswordError();
        }

        mapper.ApplyUpdate(admin, dto);

        var verification = await VerifyAdmin(admin);
        if (!verification.IsSuccess)
            return verification.Error;

        await context.SaveChangesAsync();

        return mapper.MapToDto(admin);
    }

    public async Task<UserResponseDto?> DeleteAsync(int adminId)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin == null)
            return null;

        context.Admins.Remove(admin);
        await context.SaveChangesAsync();

        return mapper.MapToDto(admin);
    }

    private async Task<Result> VerifyAdmin(Admin admin)
    {
        if (!await EmailIsUnique(admin))
            return new DuplicateEmailError(admin.Email, admin.Id > 0 ? admin.Id : null);

        if (!await PhoneNumberIsUnique(admin))
            return new DuplicatePhoneNumberError(admin.PhoneNumber, admin.Id > 0 ? admin.Id : null);

        var validation = await validator.ValidateAsync(admin);
        if (!validation.IsValid)
            return new ValidationError(validation.ToDictionary());

        return Result.Success();
    }

    private async Task<bool> EmailIsUnique(Admin admin)
        => !await context.Admins.AnyAsync(a => a.Email == admin.Email && a != admin);

    private async Task<bool> PhoneNumberIsUnique(Admin admin)
        => !await context.Admins.AnyAsync(a => a.PhoneNumber == admin.PhoneNumber && a != admin);
}
