using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lms.Data.Data;
using Lms.Core.Entities;
using Lms.Core.Repositories;
using AutoMapper;
using Lms.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace Lms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly LmsApiContext _context;
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;


        public CoursesController(LmsApiContext context, IUnitOfWork iUnitOfWork, IMapper mapper)
        {
            _context = context;
            uow = iUnitOfWork;
            this.mapper = mapper;
        }







        /*
        1.) To get Courses  in list 
        // GET: api/Courses
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        public async Task<IEnumerable<Course>> GetCourse()
        {
            //return await _context.Course.ToListAsync();
            return await uow.CourseRepository.GetAllCourses();
        }
        */
        // GET: api/Courses
        
         [HttpGet]
         public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
         {
             var course = await uow.CourseRepository.GetAllCourses();
             var courseDto = mapper.Map<IEnumerable<CourseDto>>(course);
             return Ok(courseDto); 
         }







        //Get courses with or withour modules
        /*
        [HttpGet]
  
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(bool includeModules)
        {
            var result = await uow.CourseRepository.GetAllCoursesWithOrWithoutModules(includeModules);
            if (result is null) return StatusCode(404);
            var courseDto = mapper.Map<IEnumerable<CourseDto>>(result);
            return Ok(courseDto);
        }
        */







        /*
        1.) To get one course by id 
        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            //var course = await _context.Course.FindAsync(id);
            var course = await uow.CourseRepository.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            return course;
        }
        */

        // GET: api/Courses/5
        //get a course by title
        [HttpGet("{title}")]
        public async Task<ActionResult<CourseDto>> GetCourseAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return StatusCode(400);

            var course = await uow.CourseRepository.FindCourseAsync(title);            

            if (course == null)
            {
                return StatusCode(404);
            }
            var courseDto = mapper.Map<CourseDto>(course);

            return Ok(courseDto);      
        }







        /*
        1.) To modify one course    
        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                // await _context.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        */

        //Modify one course found by title
        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{title}")]
        public async Task<IActionResult> PutCourse(string title, CourseDto courseDto)
        {
            if (string.IsNullOrWhiteSpace(title)) return StatusCode(400);

            var course = await uow.CourseRepository.GetCourseAsync(title);

            if (course is null) return StatusCode(404);            

            mapper.Map(courseDto, course);            

            if (await uow.CompleteAsync()) return Ok(mapper.Map<CourseDto>(course));            
            
            else return StatusCode(500);           
        }







        /*
        1.) Add a course 
         // POST: api/Courses
         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
         [HttpPost]
         public async Task<ActionResult<Course>> PostCourse(Course course)
         {
             //_context.Course.Add(course);
             uow.CourseRepository.Add(course);

             //await _context.SaveChangesAsync();
             await uow.CompleteAsync();

             return CreatedAtAction("GetCourse", new { id = course.Id }, course);
         }
        */

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseDto>> PostCourse(CourseDto courseDto)
        {
           
            if (CourseTitleExists(courseDto.Title)) {
                ModelState.AddModelError("Name", "This name is in use");
               return BadRequest(ModelState);
            }

            var course = mapper.Map<Course>(courseDto);

            uow.CourseRepository.Add(course);

            if (await uow.CompleteAsync())
            {
                return Ok(CreatedAtAction("GetCourse", new { id = course.Id }, course));
            }
            else
            {
                return StatusCode(500);
            }
          //  await uow.CompleteAsync();
         //   return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }







        /*
         1.) Delete a course 
            // DELETE: api/Courses/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteCourse(int id)
            {
                //var course = await _context.Course.FindAsync(id);
                var course = await uow.CourseRepository.FindAsync(id);

                if (course == null)
                {
                    return NotFound();
                }

                //_context.Course.Remove(course);
                uow.CourseRepository.Remove(course);

                //await _context.SaveChangesAsync();
                await uow.CompleteAsync();

                return NoContent();
            }
        */

        // DELETE: api/Courses/5
        [HttpDelete("{title}")]
        public async Task<IActionResult> DeleteCourse(string title, CourseDto courseDto)
        {      
            var course = await uow.CourseRepository.GetCourseAsync(title);

            if (course is null) return StatusCode(404);

            uow.CourseRepository.Remove(course);

            mapper.Map(courseDto, course);

            if (await uow.CompleteAsync())
            {
                return Ok(mapper.Map<CourseDto>(course));
            }
            else
            {
                return StatusCode(500);            
            }           
        }







        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }







        private bool CourseTitleExists(string title)
        {
            return _context.Course.Any(e => e.Title == title);
        }







        // Filtering courses list by startDate
        //[HttpGet()]
        //public ActionResult<IEnumerable<CourseDto>> GetFilteredCourses(DateTime startDate)
        //{
        //    var courses = uow.CourseRepository.GetFilteredCourses(startDate);

        //    if (courses.Count()==0) { return StatusCode(404); }
        //    else { 
        //        return Ok(mapper.Map<IEnumerable<CourseDto>>(courses));
        //    }
        //}






        //Patch
        [HttpPatch("{courseName}")]
        public async Task<ActionResult<CourseDto>> PatchCourse (string courseName, JsonPatchDocument<CourseDto> patchDocument)
        {
            var course = await uow.CourseRepository.GetCourseAsync(courseName);

            if (course is null) return StatusCode(404);

            var dto = mapper.Map<CourseDto>(course);

            patchDocument.ApplyTo(dto, ModelState);

            if (!TryValidateModel(dto)) return BadRequest(ModelState);

            mapper.Map(dto, course);

            if (await uow.CompleteAsync())
                return Ok(mapper.Map<CourseDto>(course));
            else
                return StatusCode(500);

        }

    }
}
