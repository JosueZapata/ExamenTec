using ExamenTec.Application.DTOs.Auth;
using MediatR;

namespace ExamenTec.Application.Features.Account.Login;

public record LoginQuery(string Email, string Password) : IRequest<LoginResponseDto>;

