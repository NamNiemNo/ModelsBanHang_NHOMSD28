using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace API.Models.DTO
{
    public class CustomAuthenticationStateProvider
        : AuthenticationStateProvider
    {
        private static readonly ClaimsPrincipal AnonymousUser =
            new(new ClaimsIdentity());

        private readonly IJSRuntime _jsRuntime;

        private readonly SemaphoreSlim _loadLock =
            new(1, 1);

        private ClaimsPrincipal _currentUser =
            AnonymousUser;

        private bool _hasLoaded;

        public CustomAuthenticationStateProvider(
            IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState>
            GetAuthenticationStateAsync()
        {
            if (_hasLoaded)
            {
                return new AuthenticationState(
                    _currentUser);
            }

            await _loadLock.WaitAsync();

            try
            {
                if (_hasLoaded)
                {
                    return new AuthenticationState(
                        _currentUser);
                }

                var token = await GetTokenAsync();

                if (string.IsNullOrWhiteSpace(token))
                {
                    SetAnonymous();

                    return new AuthenticationState(
                        _currentUser);
                }

                var user = ReadUserFromToken(token);

                if (user == null)
                {
                    await RemoveAuthenticationDataAsync();
                    SetAnonymous();

                    return new AuthenticationState(
                        _currentUser);
                }

                _currentUser = CreatePrincipal(
                    user.Value.Username,
                    user.Value.Role,
                    user.Value.UserId);

                _hasLoaded = true;

                return new AuthenticationState(
                    _currentUser);
            }
            catch
            {
                SetAnonymous();

                return new AuthenticationState(
                    _currentUser);
            }
            finally
            {
                _loadLock.Release();
            }
        }

        public void MarkUserAsAuthenticated(
            string userName,
            string role)
        {
            _currentUser = CreatePrincipal(
                userName,
                role,
                Guid.Empty);

            _hasLoaded = true;

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(
                        _currentUser)));
        }

        public void MarkUserAsLoggedOut()
        {
            SetAnonymous();

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(
                        _currentUser)));
        }

        private static ClaimsPrincipal CreatePrincipal(
            string userName,
            string role,
            Guid userId)
        {
            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.Name,
                    userName),

                new(
                    ClaimTypes.Role,
                    role)
            };

            if (userId != Guid.Empty)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        userId.ToString()));
            }

            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "jwt");

            return new ClaimsPrincipal(identity);
        }

        private static (
            Guid UserId,
            string Username,
            string Role)? ReadUserFromToken(
                string token)
        {
            try
            {
                var handler =
                    new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    return null;

                var jwt = handler.ReadJwtToken(token);

                // Kiểm tra thời gian hết hạn của JWT.
                var expiresClaim = jwt.Claims
                    .FirstOrDefault(
                        claim =>
                            string.Equals(
                                claim.Type,
                                JwtRegisteredClaimNames.Exp,
                                StringComparison.OrdinalIgnoreCase))
                    ?.Value;

                if (!long.TryParse(
                    expiresClaim,
                    out var expiresUnix))
                {
                    return null;
                }

                var expiresAt =
                    DateTimeOffset.FromUnixTimeSeconds(
                        expiresUnix);

                if (expiresAt <= DateTimeOffset.UtcNow)
                    return null;

                // Lấy ID tài khoản hoặc nhân viên.
                var idValue = GetClaimValue(
                    jwt,
                    ClaimTypes.NameIdentifier,
                    JwtRegisteredClaimNames.Sub,
                    "nameid");

                // Lấy tên đăng nhập.
                var username = GetClaimValue(
                    jwt,
                    ClaimTypes.Name,
                    JwtRegisteredClaimNames.UniqueName,
                    "unique_name",
                    "name");

                // Lấy quyền.
                var role = GetClaimValue(
                    jwt,
                    ClaimTypes.Role,
                    "role",
                    "roles");

                if (!Guid.TryParse(
                        idValue,
                        out var userId) ||
                    string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(role))
                {
                    return null;
                }

                return (
                    userId,
                    username,
                    role);
            }
            catch
            {
                return null;
            }
        }

        private static string? GetClaimValue(
            JwtSecurityToken token,
            params string[] claimTypes)
        {
            foreach (var claimType in claimTypes)
            {
                var value = token.Claims
                    .FirstOrDefault(
                        claim =>
                            string.Equals(
                                claim.Type,
                                claimType,
                                StringComparison.OrdinalIgnoreCase))
                    ?.Value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private void SetAnonymous()
        {
            _currentUser = AnonymousUser;
            _hasLoaded = true;
        }

        private async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                "authToken");
        }

        private async Task RemoveAuthenticationDataAsync()
        {
            await RemoveLocalStorageItemAsync(
                "authToken");

            await RemoveLocalStorageItemAsync(
                "currentId");

            await RemoveLocalStorageItemAsync(
                "currentRole");

            await RemoveLocalStorageItemAsync(
                "currentUserName");
        }

        private async Task RemoveLocalStorageItemAsync(
            string key)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.removeItem",
                    key);
            }
            catch
            {
                // JavaScript chưa sẵn sàng.
            }
        }
    }

    /*
     * Tự động lấy JWT trong localStorage và gắn vào
     * tất cả request gửi sang API.
     */
    public sealed class ApiAuthorizationHandler
        : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public ApiAuthorizationHandler(
            IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage>
            SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            string? token = null;

            try
            {
                token = await _jsRuntime
                    .InvokeAsync<string?>(
                        "localStorage.getItem",
                        cancellationToken,
                        "authToken");
            }
            catch (InvalidOperationException)
            {
                // JavaScript chưa sẵn sàng.
            }
            catch (JSDisconnectedException)
            {
                // Circuit Blazor đã ngắt.
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            var response = await base.SendAsync(
                request,
                cancellationToken);

            /*
             * Nếu API trả 401 thì token đã hết hạn
             * hoặc không còn hợp lệ.
             */
            if (response.StatusCode ==
                    HttpStatusCode.Unauthorized &&
                !string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        cancellationToken,
                        "authToken");

                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        cancellationToken,
                        "currentId");

                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        cancellationToken,
                        "currentRole");

                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.removeItem",
                        cancellationToken,
                        "currentUserName");
                }
                catch (InvalidOperationException)
                {
                    // JavaScript chưa sẵn sàng.
                }
                catch (JSDisconnectedException)
                {
                    // Circuit Blazor đã ngắt.
                }
            }

            return response;
        }
    }
}