using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lms.Core;
using Lms.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Lms.Data.Data
{
  public  class SeedData
    {
        private static LmsApiContext db;
        private static Faker fake;

        //public static async Task InitAsync(LmsApiContext context, IServiceProvider services)        
        public static async Task InitAsync(IServiceProvider services)
        {
            using (var db = services.GetRequiredService<LmsApiContext>())
            {
                //if (context is null) throw new NullReferenceException(nameof(LmsApiContext));
                //db = context;

                if (db.Course.Any()) return;

                fake = new Faker("sv");

                var courses = GetCourses();
                await db.AddRangeAsync(courses);

                var modules = GetModules(courses);
                await db.AddRangeAsync(modules);

                await db.SaveChangesAsync();
            }
        }


        private static ICollection<Module> GetModules(ICollection<Course> courses)
        {
            var modules = new List<Module>();
            var faker = new Faker("sv");

      
            foreach (var course in courses)           
            {     
                for(int i=0; i < 4; i++) { 
                    var module = new Module
                        {
                            Course = course,
                            Title = faker.Hacker.Verb(),
                            StartDate = DateTime.Now.AddDays(faker.Random.Int(-5, 5))
                        };
                    modules.Add(module);
                }
            }                
            return modules;
            
        }


        private static ICollection<Course> GetCourses()
        {
            var faker = new Faker("sv");

            var courses = new List<Course>();

            for (int i = 0; i < 20; i++)
            {
                var temp = new Course
                {            
                    Title = faker.Hacker.Verb(),                 
                    StartDate = DateTime.Now.AddDays(faker.Random.Int(-5, 5))
                };

                courses.Add(temp);
            }
            return courses;
        }
    }
}
