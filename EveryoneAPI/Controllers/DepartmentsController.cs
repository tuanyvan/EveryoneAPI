using Microsoft.AspNetCore.Mvc;
using EveryoneAPI.Models;

namespace EveryoneAPI.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentsController : Controller
    {
        private readonly EveryoneDBContext _context;

        public DepartmentsController(EveryoneDBContext context)
        {
            _context = context;
        }

        // GET: Departments
        [HttpGet]
        public async Task<IActionResult> Index(string uuid)
        {

            if (uuid == null)
            {
                return BadRequest("The user could not be identified at the beginning of this request.");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();
            if (user == null)
            {
                return BadRequest("The user sending the request is invalid.");
            }

            var json = Array.Empty<object>().ToList();

            var departments = _context.Departments.Where(d => d.EmployerId == user.EmployerId).ToList();

            foreach (var department in departments)
            {
                var departmentData = new
                {
                    DepartmentId = department.DepartmentId,
                    Name = department.Name,
                };
                json.Add(departmentData);
            }

            return Json(json);
        }

        // GET: Departments/Details/5
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(int id, string uuid)
        {
            if (id == null || uuid == null)
            {
                return NotFound("The provided parameters must be filled out: id, uuid");
            }

            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            if (user == null)
            {
                return BadRequest("An invalid user has sent the request.");
            }

            var department = _context.Departments.Where(d => d.DepartmentId == id && d.EmployerId == user.EmployerId).SingleOrDefault();

            if (department == null)
            {
                return NotFound("The department requested could not be found.");
            }
            else
            {
                var employeeJson = Array.Empty<object>().ToList();
                var departmentEmployees = _context.Employees.Where(e => e.DepartmentId == department.DepartmentId).ToList();
                var departmentPods = _context.Pods.Where(p => p.DepartmentId == department.DepartmentId).ToList();

                foreach (var employee in departmentEmployees)
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
                    employeeJson.Add(employeeData);
                }

                var podJson = Array.Empty<object>().ToList();

                foreach (var pod in departmentPods)
                {
                    var podData = new
                    {
                        PodId = pod.PodId,
                        Name = pod.Name,
                        DepartmentId = pod.DepartmentId
                    };
                    podJson.Add(podData);
                }

                var returnJson = new
                {
                    Employees = employeeJson,
                    Pods = podJson
                };

                return Json(returnJson);
            }

        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        
        public async Task<IActionResult> Create(string uuid, [FromBody] DepartmentCRUDModel department)
        {
            try
            {

                if (uuid == null)
                {
                    return BadRequest("The user could not be identified at the beginning of this request.");
                }

                var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

                if (user == null)
                {
                    return BadRequest("The user making the create department request is invalid.");
                }

                var newDepartment = new Department();

                newDepartment.Name = department.Name;
                newDepartment.EmployerId = user.EmployerId;

                _context.Add(newDepartment);

                await _context.SaveChangesAsync();
                return Ok("The department was successfully created.");
            }
            catch (Exception e)
            {
                return StatusCode(500, "The department could not be created due to a server error.");
            }
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Edit")]
        
        public async Task<IActionResult> Edit(int id, string uuid, [FromBody] DepartmentCRUDModel department)
        {
            try
            {

                if (uuid == null)
                {
                    return BadRequest("The user could not be identified at the beginning of this request.");
                }

                var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

                if (user != null)
                {
                    Department selectedDepartment = _context.Departments.Where(d => d.DepartmentId == id && d.EmployerId == user.EmployerId).SingleOrDefault();
                    if (selectedDepartment != null)
                    {

                        selectedDepartment.Name = department.Name;

                        _context.Update(selectedDepartment);
                        await _context.SaveChangesAsync();
                        return Ok("Department was successfully edited.");
                    }
                    else
                    {
                        return BadRequest("The requested department for editing could not be found.");
                    }
                }

                else
                {
                    return BadRequest("Request was made from an invalid user.");
                }

            }
            catch (Exception e)
            {
                return BadRequest("The form to edit the department has a malformed field.");
            }
        }

        // POST: Departments/Delete/5
        [HttpDelete]
        [Route("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id, string uuid)
        {

            if (uuid == null)
            {
                return BadRequest("The user could not be identified at the beginning of this request.");
            }

            if (_context.Departments == null)
            {
                return Problem("Entity set 'EveryoneDBContext.Departments'  is null.");
            }

            var department = await _context.Departments.FindAsync(id);
            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            if (department != null && user != null)
            {
                if (department.EmployerId == user.EmployerId)
                {

                    var departmentEmployees = _context.Employees.Where(e => e.DepartmentId == department.DepartmentId).ToList();
                    var pods = _context.Pods.Where(p => p.DepartmentId == department.DepartmentId).ToList();

                    foreach (var employee in departmentEmployees)
                    {
                        employee.PodId = null;
                        employee.DepartmentId = null;
                        _context.Update(employee);
                    }
                    await _context.SaveChangesAsync();

                    foreach (var pod in pods)
                    {
                        _context.Remove(pod);
                    }
                    await _context.SaveChangesAsync();

                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                    return Ok("The department was successfully deleted.");
                }
                else
                {
                    return BadRequest("The department requested for deletion does not belong to the user.");
                }
            }
            else
            {
                return BadRequest("The department or user used for the deletion process could not be found.");
            }
        }

        [HttpPatch]
        [Route("SortDepartment")]
        public async Task<IActionResult> SortDepartment(int departmentId, string uuid)
        {
            // NUll check departmentId, uuid.
            if (departmentId == null|| uuid == null)
            {
                return BadRequest("Please specify out the departmentId and uuid parameters.");
            }

            // Check department belongs to user.
            var department = _context.Departments.Where(d => d.DepartmentId == departmentId).SingleOrDefault();
            var user = _context.Employers.Where(e => e.Uuid == uuid).SingleOrDefault();

            // If the department belongs to the user...
            if (user.EmployerId == department.EmployerId)
            {
                var pods = _context.Pods.Where(p => p.DepartmentId == department.DepartmentId).ToList();
                var remainingEmployees = _context.Employees.Where(e => e.DepartmentId == department.DepartmentId).ToList();

                // Seed the pods with a random employee and remove them from the list of employees.
                foreach (var pod in pods)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(0, remainingEmployees.Count);

                    remainingEmployees[randomNumber].PodId = pod.PodId;
                    remainingEmployees.Remove(remainingEmployees[randomNumber]);
                }

                _context.Update(remainingEmployees);
                await _context.SaveChangesAsync();

                // While there are still employees to sort, go through each pod and add an employee based on uniqueness score.
                /**
                 * Rules
                 * 1. Any employee with score of 3 is automatically added, their entry in the employees list is then removed.
                 * 2. For tiebreakers, the first employee with the highest score is given the position in the pod.
                 * 3. Iterate through each pod one by one, determine the highest diversity scoring employee, change their pod to that PodId, then save the changes on DB.
                 */
                while (remainingEmployees.Count > 0)
                {
                    for (int i = 0; i < pods.Count; i++)
                    {
                        // As long as while condition is true...
                        if (remainingEmployees.Count > 0)
                        {
                            var targetedPod = pods[i];
                            var podEmployees = _context.Employees.Where(e => e.PodId == targetedPod.PodId).ToList();
                            
                            // Scores for remainingEmployees stored here...
                            List<int> employeeScores = new List<int>();

                            // Qualitative field list below...
                            HashSet<int> existingGenders = new HashSet<int>();
                            HashSet<int> existingEthnicities = new HashSet<int>();
                            HashSet<int> existingOrientation = new HashSet<int>();

                            // Get list of existing genders, ethnicities, orientations in that pod
                            foreach (var employee in podEmployees)
                            {
                                existingGenders.Add(employee.GenderIdentity);
                                existingEthnicities.Add(employee.Ethnicity);
                                existingOrientation.Add(employee.SexualOrientation);
                            }

                            // Calculate the scores for each remainingEmployee using the existing qualitative fields
                            foreach (var employee in remainingEmployees)
                            {
                                int score = 0;
                                if (existingGenders.Contains(employee.GenderIdentity)) score++;
                                if (existingEthnicities.Contains(employee.Ethnicity)) score++;
                                if (existingOrientation.Contains(employee.SexualOrientation)) score++;
                                employeeScores.Add(score);
                            }

                            // Get the employee with the highest score, let the first highest scoring employee into the pod, remove the employee from remainingEmployees.
                            int highestScore = employeeScores.Max();
                            int indexOfNewEmployee = employeeScores.IndexOf(highestScore);
                            var newEmployee = remainingEmployees[indexOfNewEmployee];
                            remainingEmployees.Remove(newEmployee);

                            // Save the changes to the database.
                            newEmployee.PodId = targetedPod.PodId;
                            _context.Update(newEmployee);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return Ok("All employees were successfully organized into their respective pods.");
            }
            else
            {
                return BadRequest("The user specified does not have ownership over the specified department.");
            }

        }

        private bool DepartmentExists(int id)
        {
          return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
