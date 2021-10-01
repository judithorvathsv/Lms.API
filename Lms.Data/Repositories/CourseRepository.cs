using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Data.Data;
using Lms.Core.Entities;
using Lms.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Lms.Core.Dto;

namespace Lms.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        internal readonly LmsApiContext db;
        internal readonly DbSet<Course> dbSet;

        public CourseRepository(LmsApiContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            dbSet = db.Set<Course>();
        }

        
        public async Task<IEnumerable<Course>> GetAllCourses() {
            return await dbSet.Include(m=>m.Modules).ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesWithOrWithoutModules(bool includeModules)
        {
            return includeModules ?
             await dbSet.Include(m => m.Modules).ToListAsync() : await dbSet.ToListAsync();
        }


        //public async Task<Course> GetCourse(int? id) {
        //    return await dbSet.FirstOrDefaultAsync(m => m.Id == id);
        //}


        public async Task<Course> GetCourseAsync(string title)
        {        
            return await dbSet.Include(m => m.Modules).FirstOrDefaultAsync(e => e.Title == title);
        }


        public async Task<Course> FindAsync(int? id) {
            return await dbSet.FindAsync(id);
        }


        public async Task<Course> FindCourseAsync(string? title)        {
            return await dbSet.Include(m => m.Modules).FirstOrDefaultAsync(e => e.Title == title);
        }


        public async Task<bool> AnyAsync(int? id) {
            return await dbSet.AnyAsync(g => g.Id == id);
        }


        public void Add(Course course) {
            dbSet.Add(course);
        }


        public void Update(Course course) {
            dbSet.Update(course);
        }


        public void Remove(Course course) {
            dbSet.Remove(course);
        }



        public IEnumerable<Course> GetFilteredCourses(DateTime startDate) {
            return dbSet.Where(a => a.StartDate == startDate).ToList();
        }

        public bool CourseExists(int courseId)
        {
            if (courseId == 0)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return db.Course.Any(a => a.Id == courseId);
        }
    }
}
