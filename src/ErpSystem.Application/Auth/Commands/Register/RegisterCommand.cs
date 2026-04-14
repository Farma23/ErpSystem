using MediatR;

namespace ErpSystem.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string FullName,
    string Password
) : IRequest<AuthResult>;

public record AuthResult(string Token, string Email, string FullName);
