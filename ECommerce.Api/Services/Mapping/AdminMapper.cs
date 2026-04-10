using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Entities;

namespace ECommerce.Api.Services.Mapping;

public class AdminMapper
{
    public UserResponseDto MapToDto(Admin admin)
    {
        return new UserResponseDto
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Email = admin.Email,
            PhoneNumber = admin.PhoneNumber,
            BirthDate = admin.BirthDate,
            CreatedAt = admin.CreatedAt,
        };
    }

    public Admin MapToEntity(UserCreateDto dto)
        => new Admin
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            BirthDate = DateOnly.Parse(dto.BirthDate),
            CreatedAt = DateTime.UtcNow,
        };

    public void ApplyUpdate(Admin toUpdate, UserUpdateDto dto)
    {
        if (dto.FirstName != null && dto.FirstName != toUpdate.FirstName)
            toUpdate.FirstName = dto.FirstName;

        if (dto.LastName != null && dto.LastName != toUpdate.LastName)
            toUpdate.LastName = dto.LastName;

        if (dto.PhoneNumber != null && dto.PhoneNumber != toUpdate.PhoneNumber)
            toUpdate.PhoneNumber = dto.PhoneNumber;

        if (DateOnly.TryParseExact(dto.BirthDate, "yyyy-MM-dd", out var birthDate) && birthDate != toUpdate.BirthDate)
            toUpdate.BirthDate = birthDate;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (newPasswordHash != toUpdate.PasswordHash)
                toUpdate.PasswordHash = newPasswordHash;
        }
    }
}
