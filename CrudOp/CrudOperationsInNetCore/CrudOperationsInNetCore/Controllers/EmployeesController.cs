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
                .FirstOrDefaultAsync(x => x.EmployeeEmail == empObj.EmployeeEmail && x.Password == empObj.Password);
            if(user == null)
                return NotFound(new {Message = "User Not Found!"});

            return Ok(new { Message = "Login Success" });
        }

        //Ananth Register Method Post: api/Employees/authenticate
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Employee empObj)
        {

        
            if (empObj == null)
                return BadRequest();

            // Assign EmployeeEmail to CreatedBy
            empObj.CreatedBy = empObj.EmployeeEmail;

            // Set default values for columns
            empObj.CreatedDateTime = DateTime.Now;
            empObj.IsDeleted = 0;

            await _context.Employees.AddAsync(empObj);
            await _context.SaveChangesAsync();
            return           
               CreatedAtAction("GetEmployee", new { id = empObj.EmployeeKey }, empObj);
        
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
    }
}
