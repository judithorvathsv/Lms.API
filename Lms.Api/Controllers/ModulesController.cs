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
    public class ModulesController : ControllerBase
    {
        private readonly LmsApiContext _context;
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;



        public ModulesController(LmsApiContext context, IUnitOfWork iUnitOfWork, IMapper mapper)
        {
            _context = context;
            uow = iUnitOfWork;
            this.mapper = mapper;
        }





        /*
        1.) To get Modules in a list
        // GET: api/Modules
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Module>>> GetModule()
        public async Task<IEnumerable<Module>> GetModule()
        {
            //return await _context.Module.ToListAsync();
            return await uow.ModuleRepository.GetAllModule();
        }
        */

        // GET: api/Modules
        [HttpGet]
         
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules()
        {     
            var module = await uow.ModuleRepository.GetAllModule();
            var moduleDto = mapper.Map<IEnumerable<ModuleDto>>(module);
            return Ok(moduleDto);
        }







        /*
        1.) To get one module by id
        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Module>> GetModule(int id)
        {
            //var @module = await _context.Module.FindAsync(id);
            var @module = await uow.ModuleRepository.FindAsync(id);

            if (@module == null)
            {
                return NotFound();
            }

            return @module;
        }
        */

        /*
        2.) To get one module by title 
        // GET: api/Modules/5
        [HttpGet("{title}")]
        public async Task<ActionResult<ModuleDto>> GetModule(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();
           
            var @module = await uow.ModuleRepository.FindModuleAsync(title);

            if (@module == null)
            {
                return NotFound();
            }

            var moduleDto = mapper.Map<ModuleDto>(@module);
            return moduleDto;
        }
        */

        // Find a modul by course name and module name together
        [HttpGet("{courseTitle}", Name = "GetModuleToCreateItToACourse")]
        public async Task<ActionResult<ModuleDto>> GetModule(string moduleTitle, string courseTitle)
        {
            if (string.IsNullOrWhiteSpace(moduleTitle) || string.IsNullOrWhiteSpace(courseTitle)  ) return StatusCode(400);

            var course = await uow.CourseRepository.FindCourseAsync(courseTitle);
            if (course == null) return StatusCode(404);

            var @module = await uow.ModuleRepository.FindModulToCourseAsync(moduleTitle, courseTitle);
            if (@module == null) return StatusCode(404);
            

            var moduleDto = mapper.Map<ModuleDto>(@module);
            return Ok(moduleDto);
        }







        /*
         1.) Modify one module
         // PUT: api/Modules/5
         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
         [HttpPut("{id}")]
         public async Task<IActionResult> PutModule(int id, Module @module)
         {
             if (id != @module.Id)
             {
                 return BadRequest();
             }

             _context.Entry(@module).State = EntityState.Modified;

             try
             {
                 //await _context.SaveChangesAsync();
                 await uow.CompleteAsync();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!ModuleExists(id))
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

        //Modify one module
        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{title}")]
        public async Task<IActionResult> PutModule(string title, ModuleDto moduleDto)
        {
            if (string.IsNullOrWhiteSpace(title)) return StatusCode(400);

            var module = await uow.ModuleRepository.GetModuleAsync(title);

            if (module is null) return StatusCode(404);

            mapper.Map(moduleDto, module);

            if (await uow.CompleteAsync()) return Ok(mapper.Map<ModuleDto>(module));
          
            else return StatusCode(500);         
        }







        /*
         1.) Add a module
         // POST: api/Modules
         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
         [HttpPost]
         public async Task<ActionResult<Module>> PostModule(Module @module)
         {
             //_context.Module.Add(@module);
              uow.ModuleRepository.Add(@module);

             //await _context.SaveChangesAsync();
             await uow.CompleteAsync();

             return CreatedAtAction("GetModule", new { id = @module.Id }, @module);
         }
        */

        /*
         2.) Add a module
        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Module>> PostModule(ModuleDto moduleDto)
        {
            var @module = mapper.Map<Module>(moduleDto);

            uow.ModuleRepository.Add(@module);
            
            await uow.CompleteAsync();

            return CreatedAtAction("GetModule", new { id = @module.Id }, @module);
        }
        */

        [HttpPost]   
        public async Task<ActionResult<ModuleDto>> CreateModuleForCourseAsync(int courseId, ModuleForCreationDto moduleForCreationDto)
        {
            if(courseId == 0) return StatusCode(400);

            if (!uow.CourseRepository.CourseExists(courseId)) return StatusCode(404);

            var moduleEntity = mapper.Map<Module>(moduleForCreationDto);
            uow.ModuleRepository.AddModule(courseId, moduleEntity);

            //uow.CompleteAsync();

            if (await uow.CompleteAsync())
            {
                var moduleToReturn = mapper.Map<ModuleDto>(moduleEntity);
                return Ok(CreatedAtRoute("GetModuleToCreateItToACourse", new { courseId = courseId }, moduleToReturn));
            }
            else
            {
                return StatusCode(500);
            }

            //var moduleToReturn = mapper.Map<ModuleDto>(moduleEntity);
           
            //return CreatedAtRoute("GetModuleToCreateItToACourse", new { courseId = courseId}, moduleToReturn);        
        }







        /*
        1.) Delete one module by id
        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            //var @module = await _context.Module.FindAsync(id);
            var @module = await uow.ModuleRepository.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            //_context.Module.Remove(@module);
            uow.ModuleRepository.Remove(@module);

            //await _context.SaveChangesAsync();
            await uow.CompleteAsync();

            return NoContent();
        }
        */

        // DELETE: api/Modules/5
        [HttpDelete("{title}")]
        public async Task<IActionResult> DeleteModule(string title, ModuleDto moduleDto)
        {
            if (string.IsNullOrWhiteSpace(title)) return StatusCode(400);

            var @module = await uow.ModuleRepository.GetModuleAsync(title);

            if (@module == null)  return StatusCode(404);

            uow.ModuleRepository.Remove(@module);

            mapper.Map(moduleDto, @module);


            if (await uow.CompleteAsync())
            {
                return Ok(mapper.Map<ModuleDto>(@module));
            }
            else
            {
                return StatusCode(500);
            }
        }







        private bool ModuleExists(int id)
        {
            return _context.Module.Any(e => e.Id == id);
        }







        //Patch
        [HttpPatch("{moduleName}")]
        public async Task<ActionResult<ModuleDto>> PatchModule(string moduleName, JsonPatchDocument<ModuleDto> patchDocument)
        {
            var module = await uow.ModuleRepository.GetModuleAsync(moduleName);

            if (module is null) return StatusCode(404);

            var dto = mapper.Map<ModuleDto>(module);

            patchDocument.ApplyTo(dto, ModelState);

            if (!TryValidateModel(dto)) return BadRequest(ModelState);

            mapper.Map(dto, module);

            if (await uow.CompleteAsync())
                return Ok(mapper.Map<ModuleDto>(module));
            else
                return StatusCode(500);

        }
    }
}
