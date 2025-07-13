using crud_api.Common;
using crud_api.DTOs.Auth;

namespace crud_api.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult> Login(UserLoginDto userLoginDto);
    Task<AuthServiceResult> Register(UserRegisterDto userRegisterDto);
}
