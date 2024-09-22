using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Core.Constants;
using ToDoList.Core.Dto;
using ToDoList.Core.Interfaces;
using M = ToDoList.Core.Models;

namespace ToDoList.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
      
        #endregion


        #region Constructor
        public TaskController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion


        #region Actions

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task is null)
                return NotFound($"There is no task with id = {id}");
            return Ok(task);
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetTasksOfUser(string userId)
        {
            var user = await _unitOfWork.ApplicationUsers.FindAsync(u=>u.Id== userId);

            if(user is null)
                return NotFound($"There is no user with id = {userId}");

            var tasks = await _unitOfWork.Tasks.FindAllAsync(t=>t.UserId==userId);

            return Ok(tasks);
        }


        [HttpGet("List")]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            return Ok(tasks);
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto taskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var applicationUser = await _unitOfWork.ApplicationUsers.FindAsync(u => u.UserName == userName);

            M.Task task = new M.Task()
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status= Status.Pending,
                UserId = applicationUser.Id
            };
           

            await _unitOfWork.Tasks.AddAsync(task);

            if (_unitOfWork.Complete() > 0)
                return Created("Created Successfully", task);

            return BadRequest("Error While Saving");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateTaskDto taskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var taskDb = await _unitOfWork.Tasks.GetByIdAsync(taskDto.Id);

            if (taskDb is null)
                return NotFound($"There is no task with id = {taskDto.Id}");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var applicationUser = await _unitOfWork.ApplicationUsers.FindAsync(u => u.UserName == userName);

            if (taskDb.UserId != applicationUser.Id)
                return BadRequest("This is not your own task to Update It");

            // Update 
            taskDb.Title = taskDto.Title;
            taskDb.Description = taskDto.Description;
            taskDb.Status = taskDto.Status;

            _unitOfWork.Tasks.Update(taskDb);

            if (_unitOfWork.Complete() > 0)
                return Ok(taskDb);

            return BadRequest("Error While Saving");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task is null)
                return NotFound($"There is no category with id = {id}");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var applicationUser = await _unitOfWork.ApplicationUsers.FindAsync(u => u.UserName == userName);

            if (task.UserId != applicationUser.Id)
                return BadRequest("This is not your own task to Delete It");

            _unitOfWork.Tasks.Delete(task);

            if (_unitOfWork.Complete() > 0)
                return Ok("Deleted Successfully");

            return BadRequest("Error While Saving");

        }

        #endregion
    }
}
