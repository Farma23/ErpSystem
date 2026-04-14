using ErpSystem.Application.Auth.Commands.Register;
using MediatR;

namespace ErpSystem.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
