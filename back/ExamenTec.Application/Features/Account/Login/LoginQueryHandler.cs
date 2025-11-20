using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Auth;
using ExamenTec.Application.Interfaces;
using MediatR;

namespace ExamenTec.Application.Features.Account.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseDto>
{
    private readonly IUserService _userService;

    public LoginQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<LoginResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var loginRequest = new LoginRequestDto
        {
            Email = request.Email,
            Password = request.Password
        };

        try
        {
            return await _userService.LoginAsync(loginRequest);
        }
        catch (UnauthorizedException)
        {
            throw;
        }
    }
}

