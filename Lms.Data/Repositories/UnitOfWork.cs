using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Core.Entities;
using Lms.Core.Repositories;
using Lms.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Lms.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly LmsApiContext db;
        public ICourseRepository CourseRepository { get; }
        public IModuleRepository ModuleRepository { get; }

        public UnitOfWork(LmsApiContext db)
        {
            this.db = db;
            CourseRepository = new CourseRepository(db);
            ModuleRepository = new ModuleRepository(db);
        }

        public async Task<bool> CompleteAsync()
        {
            return (await db.SaveChangesAsync()) >= 0;
        }
    }
}
