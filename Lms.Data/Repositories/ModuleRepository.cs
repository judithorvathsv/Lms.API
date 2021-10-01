using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Data.Data;
using Lms.Core.Entities;
using Lms.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lms.Data.Repositories
{

   public class ModuleRepository : IModuleRepository
    {
        internal readonly LmsApiContext db;
        internal readonly DbSet<Module> dbSet;
        internal readonly DbSet<Course> dbSetCourse;

        public ModuleRepository(LmsApiContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            dbSet = db.Set<Module>();
            dbSetCourse = db.Set<Course>();
        }


        public async Task<IEnumerable<Module>> GetAllModule()
        {     
            return await dbSet.ToListAsync();
        }
            

        public async Task<Module> GetModule(int? id)
        {
            return await dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }


        public async Task<Module> GetModuleAsync(string title)
        {       
            var query = dbSet.Include(c=>c.Course).AsQueryable();
            return await query.FirstOrDefaultAsync(e => e.Title == title);
        }


        public async Task<Module> FindAsync(int? id)
        {
            return await dbSet.FindAsync(id);
        }


        //public async Task<Module> FindModuleAsync(string? courseTitle, string? title)
        //{
        //    return await dbSet.Include(m => m.Course).Where(c => c.Title == courseTitle).FirstOrDefaultAsync();
        //}


        public async Task<Module> FindModulToCourseAsync(string? moduleTitle, string? courseTitle)
        {     
            var modulesAndCourses= dbSet.Where(m => m.Title == moduleTitle).Include(m => m.Course);
            return  await modulesAndCourses.Where(x=>x.Course.Title == courseTitle).FirstOrDefaultAsync();         
        }


        public async Task<bool> AnyAsync(int? id)
        {
            return await dbSet.AnyAsync(g => g.Id == id);
        }


        public void AddModule(int courseId, Module module)
        {
            if (courseId == 0)
            {
                throw new ArgumentNullException(nameof(courseId));
            }
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            module.CourseId = courseId;
            dbSet.Add(module);
        }


        public void Update(Module module)
        {
            dbSet.Update(module);
        }


        public void Remove(Module module)
        {
            dbSet.Remove(module);
        }
    }
}
