using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Agregator.Api.Mappings
{
    using Resources;
    using Data;
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSignUpResource, AgregatorUser>().ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
            CreateMap<UserLoginResource, AgregatorUser>().ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
            Console.WriteLine("Mapping Finished");

        }
    }
}
