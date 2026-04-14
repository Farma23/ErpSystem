using ErpSystem.Application.Auth.Commands.Register;
using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Application.Common.Interfaces;
using ErpSystem.Domain.Interfaces;
using MediatR;

namespace ErpSystem.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;

    public LoginCommandHandler(IUserRepository users, IJwtTokenGenerator jwt)
        => (_users, _jwt) = (users, jwt);

    public async Task<AuthResult> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(cmd.Email, ct)
            ?? throw new NotFoundException("User", cmd.Email);

        if (!BCrypt.Net.BCrypt.Verify(cmd.Password, user.PasswordHash))
            throw new NotFoundException("User", cmd.Email);

        var token = _jwt.GenerateToken(user.Id, user.Email, new List<string>());
        return new AuthResult(token, user.Email, user.FullName);
    }
}
