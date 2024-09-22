using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Core.Dto
{
    public class UpdateTaskDto
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; }
        [Required, MaxLength(250)]
        public string Description { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; }
    }
}
