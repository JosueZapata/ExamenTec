using ExamenTec.Application.DTOs.Auth;

namespace ExamenTec.Application.Interfaces;

public interface IUserService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}

