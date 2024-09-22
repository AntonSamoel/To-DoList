using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ToDoList.Core.Models.AuthModels;

namespace ToDoList.Core.Models
{
    
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(100)]
        public string Title { get; set; }
        [Required, MaxLength(250)]
        public string Description { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string? UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
}
