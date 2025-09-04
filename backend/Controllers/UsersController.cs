using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Équivalent à app.use('/api/users', userRoutes)
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users - Équivalent à router.get('/')
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users); // Équivalent à res.json(users)
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message }); // Équivalent à res.status(500).json()
            }
        }

        // GET api/users/{id} - Équivalent à router.get('/:id')
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" }); // Équivalent à res.status(404)
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST api/users - Équivalent à router.post('/')
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                // ModelState vérifie automatiquement les validations (comme express-validator)
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Équivalent à res.status(400).json(errors)
                }

                var user = await _userService.CreateUserAsync(createUserDto);
                
                // Retourne 201 avec l'URL de la ressource créée
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT api/users/{id} - Équivalent à router.put('/:id')
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE api/users/{id} - Équivalent à router.delete('/:id')
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "User not found" });
                }

                return NoContent(); // 204 No Content - Équivalent à res.status(204).send()
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}