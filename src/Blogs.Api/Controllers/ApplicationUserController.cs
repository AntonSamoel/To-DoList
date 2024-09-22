using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Core.Dto;
using ToDoList.Core.Interfaces;

namespace ToDoList.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        #endregion


        #region Constructor
        public ApplicationUserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Actions
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {

            var user = await _unitOfWork.ApplicationUsers.FindAsync(u => u.Id == id);

            if (user is null)
                return NotFound($"There is no user with id={id}");

            GetApplicationUserDto getApplicationUserDto = new GetApplicationUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };

            return Ok(getApplicationUserDto);

        }
        #endregion
    }
}
