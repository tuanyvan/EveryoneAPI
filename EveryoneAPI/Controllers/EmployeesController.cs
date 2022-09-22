using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EveryoneAPI.Models;
using static EveryoneAPI.Models.Employee;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EveryoneAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly EveryoneDBContext _context;

        public EmployeesController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Employees
        [HttpGet]
        public async Task<IActionResult> Index(string uuid)
        {
            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("The user sending the request is invalid.");
            }

            var json = Array.Empty<object>().ToList();

            var employees = _context.Employees.Where(e => e.EmployerId == user.EmployerId).ToList();

            foreach (var employee in employees)
            {
                var departmentName = _context.Departments.Where(d => d.DepartmentId == employee.DepartmentId).SingleOrDefault();
                var podName = _context.Pods.Where(p => p.PodId == employee.PodId).SingleOrDefault();

                var employeeData = new
                {
                    EmployeeId = employee.EmployeeId,
                    Name = employee.Name,
                    GenderIdentity = _context.GenderIdentities.Where(gi => gi.GenderId == employee.GenderIdentity).SingleOrDefault().Name,
                    SexualOrientation = _context.SexualOrientations.Where(s => s.OrientationId == employee.SexualOrientation).SingleOrDefault().Name,
                    Ethnicity = _context.Ethnicities.Where(e => e.EthnicityId == employee.Ethnicity).SingleOrDefault().Name,
                    EmployerId = employee.EmployerId,
                    DepartmentId = employee.DepartmentId,
                    DepartmentName = departmentName == null ? null : departmentName.Name,
                    PodId = employee.PodId,
                    PodName = podName == null ? null : podName.Name,
                    Pronoun = _context.Pronouns.Where(e => e.PronounId == employee.Pronoun).SingleOrDefault().Name,
                };

                json.Add(employeeData);
            }

            return Json(json);
        }

        // GET: Employees/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int id, string uuid)
        {
            if (id == null)
            {
                return NotFound("The provided parameters are missing: id");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("An invalid user has sent the request.");
            }

            var employee = _context.Employees.Where(e => e.EmployeeId == id && e.EmployerId == user.EmployerId).SingleOrDefault();

            if (employee == null)
            {
                return NotFound("The employee requested was not found.");
            }

            var departmentName = _context.Departments.Where(d => d.DepartmentId == employee.DepartmentId).SingleOrDefault();
            var podName = _context.Pods.Where(p => p.PodId == employee.PodId).SingleOrDefault();

            var employeeData = new
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                GenderIdentity = _context.GenderIdentities.Where(gi => gi.GenderId == employee.GenderIdentity).SingleOrDefault().Name,
                SexualOrientation = _context.SexualOrientations.Where(s => s.OrientationId == employee.SexualOrientation).SingleOrDefault().Name,
                Ethnicity = _context.Ethnicities.Where(e => e.EthnicityId == employee.Ethnicity).SingleOrDefault().Name,
                EmployerId = employee.EmployerId,
                DepartmentId = employee.DepartmentId,
                DepartmentName = departmentName == null ? null : departmentName.Name,
                PodId = employee.PodId,
                PodName = podName == null ? null : podName.Name,
                Pronoun = _context.Pronouns.Where(e => e.PronounId == employee.Pronoun).SingleOrDefault().Name,
            };

            return Json(employeeData);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] EmployeeFormModel employee, string uuid)
        {
            try
            {
                var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
                if (user == null)
                {
                    return BadRequest("The user requesting the employee creation does not exist.");
                }
                Employee newEmployee = new Employee();
                newEmployee.Name = employee.Name;
                newEmployee.GenderIdentity = employee.GenderIdentity;
                newEmployee.SexualOrientation = employee.SexualOrientation;
                newEmployee.Ethnicity = employee.Ethnicity;
                newEmployee.EmployerId = user.EmployerId;
                newEmployee.DepartmentId = employee.DepartmentId;
                newEmployee.PodId = employee.PodId;
                newEmployee.Pronoun = employee.Pronoun;

                _context.Add(newEmployee);
                await _context.SaveChangesAsync();

                return Ok("The employee was successfully created.");
            }
            catch (Exception e)
            {
                return BadRequest("There was an error with the creation of the employee.");
            }
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, string uuid, [FromBody] EmployeeEditModel employee)
        {
            try
            {

                Employer user = _context.Employers.Where(e => e.Uuid.Equals(uuid)).SingleOrDefault();

                if (user != null)
                {

                    Employee selectedEmployee = _context.Employees.Where(e => e.EmployeeId == id && e.EmployerId == user.EmployerId).SingleOrDefault();
                    
                    if (selectedEmployee != null)
                    {
                        selectedEmployee.Name = employee.Name;
                        selectedEmployee.GenderIdentity = employee.GenderIdentity;
                        selectedEmployee.SexualOrientation = employee.SexualOrientation;
                        selectedEmployee.Ethnicity = employee.Ethnicity;
                        selectedEmployee.DepartmentId = employee.DepartmentId;
                        selectedEmployee.PodId = employee.PodId;
                        selectedEmployee.Pronoun = employee.Pronoun;
                        _context.Update(selectedEmployee);
                        await _context.SaveChangesAsync();
                        return Ok("Employee was successfully edited.");
                    }

                    else
                    {
                        return BadRequest("The requested employee for edit could not be found.");
                    }

                }

                else
                {
                    return BadRequest("Request was made from an invalid user.");
                }
            }
            catch (Exception e)
            {
                return BadRequest("The form to edit the employee has a malformed field.");
            }
        }


        // POST: Employees/Delete/5
        [HttpDelete]
        [Route("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id, string uuid)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            if (employee != null && user != null)
            {
                if (user.EmployerId == employee.EmployerId)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    return Ok("Employee was successfully deleted.");
                }
                else
                {
                    return BadRequest("The employee requested for deletion does not belong to the user.");
                }
            }
            else
            {
                return BadRequest("The employee or user used for the deletion process could not be found.");
            }

        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
