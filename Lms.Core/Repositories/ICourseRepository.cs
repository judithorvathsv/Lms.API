using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Core.Entities;


namespace Lms.Core.Repositories
{
  public interface ICourseRepository
    {
        public Task<IEnumerable<Course>> GetAllCourses();

        public Task<IEnumerable<Course>> GetAllCoursesWithOrWithoutModules(bool includeModules);

        //public Task<Course> GetCourse(int? id);

        public Task<Course> GetCourseAsync(string title);

        public Task<Course> FindAsync(int? id);

        public Task<Course> FindCourseAsync(string? title);

        public Task<bool> AnyAsync(int? id);

        public void Add(Course course);

        public void Update(Course course);

        public void Remove(Course course);

        public IEnumerable<Course> GetFilteredCourses(DateTime startDate);

        public bool CourseExists(int courseId);
    }
}
