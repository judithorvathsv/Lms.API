using Lms.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Core.Dto
{
   public class CourseDto
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate 
        { 
          get { return StartDate.AddMonths(3); }
          set { } 
        }

        public ICollection<ModuleDto> Modules { get; set; }
    }
}
