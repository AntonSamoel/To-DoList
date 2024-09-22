using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ToDoList.Core.Models.AuthModels;

namespace ToDoList.Core.Dto
{
    public class CreateTaskDto
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }
        [Required, MaxLength(250)]
        public string Description { get; set; }
      
       
    }
}
