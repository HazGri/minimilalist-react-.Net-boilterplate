using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            // Équivalent à User.find() en Mongoose
            var users = await _context.Users.ToListAsync();
            
            return users.Select(user => MapToResponseDto(user));
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            // Équivalent à User.findById(id)
            var user = await _context.Users.FindAsync(id);
            
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Vérifier l'unicité (comme une validation custom)
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == createUserDto.Email || u.Username == createUserDto.Username);
                
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username or Email already exists");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                CreatedAt = DateTime.UtcNow,
            };

            // Équivalent à user.save()
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            // Équivalent à User.findById(id)
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            // Vérifier l'unicité si email/username change
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id);
                if (emailExists) throw new InvalidOperationException("Email already exists");
            }

            if (!string.IsNullOrEmpty(updateUserDto.Username) && updateUserDto.Username != user.Username)
            {
                var usernameExists = await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username && u.Id != id);
                if (usernameExists) throw new InvalidOperationException("Username already exists");
            }

            // Mise à jour des champs (comme Object.assign ou spread operator)
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                user.Username = updateUserDto.Username;
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            if (updateUserDto.FirstName != null)
                user.FirstName = updateUserDto.FirstName;
            if (updateUserDto.LastName != null)
                user.LastName = updateUserDto.LastName;
                

            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            // Équivalent à User.findByIdAndDelete(id)
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Fonction helper pour mapper (comme toJSON() en Mongoose)
        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
            };
        }
    }
}