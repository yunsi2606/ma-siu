using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

/// <summary>
/// Main authentication service handling Google Sign-In and token management.
/// </summary>
public class AuthenticationService
{
    private readonly IGoogleAuthProvider _googleAuthProvider;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticationService(
        IGoogleAuthProvider googleAuthProvider,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _googleAuthProvider = googleAuthProvider;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    /// <summary>
    /// Authenticates user with Google ID token.
    /// </summary>
    public async Task<AuthResponse?> GoogleSignInAsync(
        GoogleSignInRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate Google ID token
        var googleUser = await _googleAuthProvider.ValidateIdTokenAsync(
            request.IdToken, cancellationToken);

        if (googleUser == null)
            return null;

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(
            googleUser.GoogleId,
            googleUser.Email,
            googleUser.Name);

        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(googleUser.GoogleId, refreshTokenValue);

        // Store refresh token
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            _tokenService.GetAccessTokenExpiry(),
            new UserInfo(
                googleUser.GoogleId,
                googleUser.Email,
                googleUser.Name,
                googleUser.Picture
            )
        );
    }

    /// <summary>
    /// Refreshes access token using refresh token.
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken, cancellationToken);

        if (existingToken == null || !existingToken.IsActive)
            return null;

        // Revoke old token and create new one (token rotation)
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
        existingToken.Revoke(newRefreshTokenValue);
        await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);

        var newRefreshToken = RefreshToken.Create(existingToken.UserId, newRefreshTokenValue);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        // Generate new access token
        var accessToken = _tokenService.GenerateAccessToken(existingToken.UserId, "");

        return new AuthResponse(
            accessToken,
            newRefreshTokenValue,
            _tokenService.GetAccessTokenExpiry(),
            new UserInfo(existingToken.UserId, "", null, null)
        );
    }

    /// <summary>
    /// Revokes refresh token.
    /// </summary>
    public async Task<bool> RevokeTokenAsync(
        RevokeTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken, cancellationToken);

        if (token == null || !token.IsActive)
            return false;

        token.Revoke();
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
        return true;
    }
}
