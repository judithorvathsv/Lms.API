using AutoMapper;
using Bogus;
using Lms.Core.Dto;
using Lms.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Data.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {            
                CreateMap<Course, CourseDto>().ReverseMap();
                CreateMap<Module, ModuleDto>().ReverseMap();
                CreateMap<ModuleForCreationDto, Module>();
        }       
    }
}