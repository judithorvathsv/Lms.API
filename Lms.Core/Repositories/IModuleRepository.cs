using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Core.Entities;

namespace Lms.Core.Repositories
{
    public interface IModuleRepository
    {
        public Task<IEnumerable<Module>> GetAllModule();      

        public Task<Module> GetModule(int? id);

        public Task<Module> GetModuleAsync(string title);

        public Task<Module> FindAsync(int? id);

        //public Task<Module> FindModuleAsync(string? title);

        public Task<Module> FindModulToCourseAsync(string? courseTitle, string? title);

        public Task<bool> AnyAsync(int? id);

        //public void Add(Module module);

        public void AddModule(int courseId, Module module);

        public void Update(Module module);

        public void Remove(Module module);
    }
}
