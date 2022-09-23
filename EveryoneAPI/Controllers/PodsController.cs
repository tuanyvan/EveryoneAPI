using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EveryoneAPI.Models;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis;
using MessagePack;
using Newtonsoft.Json.Linq;
using static EveryoneAPI.Models.Pod;

namespace EveryoneAPI.Controllers
{
    [Route("api/[controller]")]
    public class PodsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public PodsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Pods
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> Index(int departmentId, string uuid)
        {

            if (uuid == null)
            {
                return StatusCode(401, "The user could not be identified at the beginning of the request.");
            }

            var department = _context.Departments.Where(d => d.DepartmentId == departmentId).SingleOrDefault();
            if (department == null)
            {
                return BadRequest("The requested department id for the pod index does not exist.");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            if (user == null)
            {
                return StatusCode(401, "The user making the request is invalid.");
            }

            if (department.EmployerId != user.EmployerId)
            {
                return StatusCode(401, "The user requesting the department pod index does not have ownership over this department.");
            }

            var json = Array.Empty<object>().ToList();
            var departmentPods = _context.Pods.Where(p => p.DepartmentId == department.DepartmentId).ToList();

            foreach (var pod in departmentPods)
            {
                var podData = new
                {
                    PodId = pod.PodId,
                    Name = pod.Name,
                    DepartmentId = pod.DepartmentId
                };
                json.Add(podData);
            }

            return Json(json);
        }

        /**
         * Get list of employees belonging to the pod.
         */
        // GET: Pods/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int id, string uuid)
        {
            if (id == null || _context.Pods == null)
            {
                return NotFound("The specified pod could not be found because the id supplied is null.");
            }

            if (uuid == null)
            {
                return StatusCode(401, "The user could not be identified at the beginning of this request.");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            var pod = _context.Pods.Where(p => p.PodId == id).SingleOrDefault();

            if (pod == null)
            {
                return BadRequest("The requested pod could not be found as its id does not match any existing pods.");
            }

            var podDepartment = _context.Departments.Where(d => d.DepartmentId == pod.DepartmentId).SingleOrDefault();

            if (podDepartment.EmployerId != user.EmployerId)
            {
                return StatusCode(401, "The user does not have ownership over the specified pod.");
            }

            var json = Array.Empty<object>().ToList();
            var podEmployees = _context.Employees.Where(e => e.PodId == pod.PodId).ToList();

            foreach (var employee in podEmployees)
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

        // POST: Pods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        
        public async Task<IActionResult> Create(string uuid, [FromBody] PodFormModel pod)
        {

            if (uuid == null)
            {
                return StatusCode(401, "The user could not be identified at the beginning of this request.");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            if (user == null)
            {
                return StatusCode(401, "The user making the request is invalid.");
            }

            var department = _context.Departments.Where(d => d.DepartmentId == pod.DepartmentId).SingleOrDefault();

            if (department == null)
            {
                return BadRequest("The department which the pod is being created for does not exist.");
            }
            if (department.EmployerId != user.EmployerId)
            {
                return StatusCode(401, "The user does not own the department where the pod is being created.");
            }

            Pod newPod = new Pod();

            newPod.Name = pod.Name;
            newPod.DepartmentId = pod.DepartmentId;

            _context.Add(newPod);
            await _context.SaveChangesAsync();

            return Ok("The pod was successfully created.");
        }

        // POST: Pods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        
        public async Task<IActionResult> Edit(int id, string uuid, [FromBody] PodEditModel pod)
        {
            if (uuid == null)
            {
                return StatusCode(401, "The user could not be identified at the beginning of this request.");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            
            if (user == null)
            {
                return StatusCode(401, "The user making the pod edit request is invalid.");
            }

            var targetedPod = _context.Pods.Where(p => p.PodId == id).SingleOrDefault();

            if (targetedPod == null)
            {
                return BadRequest("The pod targeted for editing could not be found with the supplied id.");
            }

            // Check that the pod's department belongs to the user employer.
            Department podDepartment = _context.Departments.Where(d => d.DepartmentId == targetedPod.DepartmentId).SingleOrDefault();
            if (podDepartment.EmployerId != user.EmployerId)
            {
                return BadRequest("The pod targeted for editing does not belong to the user.");
            }

            // Pod will only have its name editable.
            targetedPod.Name = pod.Name;
            
            // Try to save the changes.
            try
            {
                _context.Update(targetedPod);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A critical error occured while trying to edit the specified pod.");
            }

            return Ok("The pod was successfully edited.");
        }

        // POST: Pods/Delete/5
        [HttpDelete]
        [Route("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id, string uuid)
        {
            if (uuid == null)
            {
                return StatusCode(401, "The user could not be identified at the beginning of the request.");
            }

            if (id == null)
            {
                return BadRequest("Please supply a value for the parameter: id");
            }

            if (_context.Pods == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Pods'  is null.");
            }

            var pod = await _context.Pods.FindAsync(id);

            if (pod == null)
            {
                return BadRequest("The pod request for deletion does not exist.");
            }

            // Verify that the pod belongs to the employer.
            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            Department podDepartment= _context.Departments.Where(d => d.DepartmentId == pod.DepartmentId).SingleOrDefault();
            if (podDepartment.EmployerId != user.EmployerId)
            {
                return BadRequest("The department belonging to the deleted pod does not belong to the user.");
            }

            if (pod != null)
            {

                // Cascade null employee PodId fields belonging to the pod.
                var podEmployees = _context.Employees.Where(e => e.PodId == pod.PodId).ToList();

                foreach (var employee in podEmployees)
                {
                    employee.PodId = null;
                    _context.Update(employee);
                }
                await _context.SaveChangesAsync();

                // Remove the pod.
                _context.Pods.Remove(pod);
                await _context.SaveChangesAsync();
                return Ok($"The department was successfully deleted. {podEmployees.Count} employees no longer have a pod.");
            }
            else
            {
                return BadRequest("The pod requested for deletion does not exist.");
            }
        }

        private bool PodExists(int id)
        {
          return _context.Pods.Any(e => e.PodId == id);
        }
    }
}
