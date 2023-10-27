using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeSheetAuthAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TimeSheetAuthAPI.Helpers;
using System.Text;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TimeSheetAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class EmployeesController : ControllerBase
    {
        private readonly DevContext _context;

        public EmployeesController(DevContext context)
        {
            _context = context;
        }

        //Ananth Login Method Post: api/Employees/authenticate

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Employee empObj)
        {
            if (empObj == null)
                return BadRequest();

            var user = await _context.Employees
                .FirstOrDefaultAsync(x => x.EmployeeEmail == empObj.EmployeeEmail);
            if(user == null)
                return NotFound(new {Message = "User Not Found!"});
            if(!PasswordHasher.VerifyPassword(empObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            var token = CreateJwt(user);

            return Ok(new 
            { 
                Token = token,
                Message = "Login Success",
                //EmployeeFirstName = user.EmployeeFirstName 
            });
        }

        //Ananth Register Method Post: api/Employees/authenticate
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Employee empObj)
        {

        
            if (empObj == null)
                return BadRequest();

            //Check Email
            if (await CheckEmailExistAsync(empObj.EmployeeEmail))
                return BadRequest(new { Message = " Email Already Exist!" });

            //Check password Strength
            var pass = CheckPasswordStrength(empObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            empObj.Password = PasswordHasher.HashPassword(empObj.Password);
            
            // Assign EmployeeEmail to CreatedBy
            empObj.CreatedBy = empObj.EmployeeEmail;

            // Set default values for columns
            empObj.CreatedDateTime = DateTime.Now;
            empObj.IsDeleted = 0;

            await _context.Employees.AddAsync(empObj);
            await _context.SaveChangesAsync();
            return           
               CreatedAtAction("GetEmployee", new { id = empObj.EmployeeKey }, new { Message = "User Registered", Data = empObj });
           
        
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeKey)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'DevContext.Employees'  is null.");
            }
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeKey }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeKey == id)).GetValueOrDefault();
        }

        private Task<bool> CheckEmailExistAsync(string EmployeeEmail) 
            => _context.Employees.AnyAsync(x => x.EmployeeEmail == EmployeeEmail);

        private string CheckPasswordStrength(string Password)
        {
            StringBuilder sb = new StringBuilder();
            if(Password.Length<8)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);

            if (!(Regex.IsMatch(Password, "[a-z]") && Regex.IsMatch(Password, "[A-Z]")
                && Regex.IsMatch(Password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if(!(Regex.IsMatch(Password, "[!,@,#,$,%,^,&,*,_,-,+,=,(,),\\[,\\],{,},/,,|,:,;,,,.,<,>,?]")))
                sb.Append("Password should contain special chars"+ Environment.NewLine);
            return sb.ToString();
        }
        // Creating JWT token 
        private string CreateJwt(Employee user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.EmployeeFirstName} {user.EmployeeLastName}")

            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        //[HttpGet]
        //public async Task<ActionResult<Employee>> GetAllUsers()
        //{
        //    return Ok(await _context.Employees.ToListAsync());
        //}
    }
}
