using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    // private readonly IRecaptchaService _recaptchaService;
    private readonly IMailService _mailService;

    public AccountController(
        UserManager<User> userManager,
        IAccountService accountService,
        RoleManager<IdentityRole<int>> roleManager,
        SignInManager<User> signInManager,
        // IRecaptchaService recaptchaService,
        IMailService mailService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        // _recaptchaService = recaptchaService;
        _mailService = mailService;
        _accountService = accountService;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        // _recaptchaService = recaptchaService;
    }

    // Registro de usuarios
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserPostPutDTO model)
    {
        // // Verificar reCAPTCHA
        // var recaptchaValid = await _recaptchaService.VerifyRecaptchaAsync(model.RecaptchaToken);
        // if (!recaptchaValid)
        // {
        //     return BadRequest(new { Message = "reCAPTCHA inválido. Intenta nuevamente." });
        // }

        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "El usuario ya existe" });
        }

        var user = new User
        {
            UserName = model.Username,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            DNI = model.DNI,
            Address = model.Address,
            ProfilePhotoId = model.ProfilePhotoId
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "Error al crear usuario",
                Errors = result.Errors.Select(e => e.Description)
            });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action("ConfirmEmail", "Account", 
        new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

        var htmlMessage = $@"
        <html>
            <body style='font-family: Arial, sans-serif; text-align: center; background-color: #f9f9f9; padding: 50px;'>
                <h2 style='font-size: 24px; color: #333;'>Verificación de cuenta</h2>
                <p style='font-size: 18px; color: #555;'>Gracias por registrarte. Por favor, confirma tu cuenta haciendo clic en el botón de abajo:</p>
                <a href='{callbackUrl}' style='display: inline-block; padding: 15px 30px; color: white; background-color: #007bff; border-radius: 5px; text-decoration: none; font-size: 18px; margin-top: 20px;'>Confirmar cuenta</a>
            </body>
        </html>";

        await _mailService.SendEmailAsync(
            user.Email,
            "Confirmación de Registro",
            htmlMessage,
            "MercadoPlus Support <support@mercadoplus.xyz>"
        );

        return Ok(new { Message = "Usuario creado satisfactoriamente, por favor verifique la bandeja de entrada de su correo para validación" });
    }

    [AllowAnonymous]
    [HttpGet("confirmemail")]
    public async Task<IActionResult> ConfirmEmail(int userId, string token)
    {
        if (userId <= 0 || string.IsNullOrEmpty(token))
            return BadRequest(new { Message = "Token de verificación no válido" });

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound(new { Message = "Usuario no encontrado" });

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Error al confirmar el correo", Errors = result.Errors });

        return Ok(new { Message = "Correo confirmado exitosamente" });
    }


    // Actualizar información de usuario
    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UserPutDTO model)
    {
        try
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Forbid();
            }

            var updatedUser = await _accountService.Update(currentUserId, model);

            if (updatedUser == null)
            {
                return NotFound(new { Message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                Message = "Usuario actualizado satisfactoriamente",
                User = updatedUser
            });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    // Login y generación de JWT
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized(new { message = "Invalid username or password." });
    }

    // Asignar un rol a un usuario
    [Authorize(Roles = "admin")]
    [HttpPost("asignar-rol")]
    public async Task<IActionResult> AsignarRol([FromBody] RoleAssignmentDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }

        var roleExists = await _userManager.IsInRoleAsync(user, model.Role);
        if (roleExists)
        {
            return BadRequest(new { Message = "El usuario ya tiene este rol" });
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Rol asignado correctamente" });
        }

        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al asignar el rol" });
    }

    // Obtener la lista de roles (solo administradores)
    [Authorize(Roles = "admin")]
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return Ok(roles);
    }

    // Obtener lista de usuarios (solo administradores)
    [Authorize(Roles = "admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _accountService.GetAll();

            if (users == null || !users.Any())
            {
                return NotFound("No se encontraron usuarios en el sistema.");
            }

            return Ok(users);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("No tienes autorización para realizar esta operación.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor: " + ex.Message);
        }
    }

     // Obtener lista de usuarios (solo administradores)
    [Authorize]
    [HttpGet("getUserInfo")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _accountService.GetById(userId); // Usa await aquí
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }
            return Ok(user); // Devuelve el usuario encontrado
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("No tienes autorización para realizar esta operación.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor: " + ex.Message);
        }
    }



    // Obtener roles específicos de un usuario
    [Authorize(Roles = "admin, User")]
    [HttpGet("/users/{userId}/roles")]
    public async Task<IActionResult> GetRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        return BadRequest();
    }

    // Crear un rol (solo administradores)
    [Authorize(Roles = "admin")]
    [HttpPost("role")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest("El nombre del rol no puede estar vacío.");
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return Conflict($"El rol '{roleName}' ya existe.");
        }

        var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));

        if (result.Succeeded)
        {
            return Ok($"El rol '{roleName}' ha sido creado exitosamente.");
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();
        return BadRequest(new
        {
            Message = "Error al crear el rol.",
            Errors = errorMessages
        });
    }
}
