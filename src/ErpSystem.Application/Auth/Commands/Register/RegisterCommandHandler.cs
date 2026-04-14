using ErpSystem.Application.Common.Interfaces;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using MediatR;

namespace ErpSystem.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenGenerator _jwt;

    public RegisterCommandHandler(IUserRepository users, IUnitOfWork uow, IJwtTokenGenerator jwt)
        => (_users, _uow, _jwt) = (users, uow, jwt);

    public async Task<AuthResult> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await _users.GetByEmailAsync(cmd.Email, ct);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        var hash = BCrypt.Net.BCrypt.HashPassword(cmd.Password);
        var user = User.Create(cmd.Email, cmd.FullName, hash);

        await _users.AddAsync(user, ct);
        await _uow.CommitAsync(ct);

        var token = _jwt.GenerateToken(user.Id, user.Email, new List<string>());
        return new AuthResult(token, user.Email, user.FullName);
    }
}
