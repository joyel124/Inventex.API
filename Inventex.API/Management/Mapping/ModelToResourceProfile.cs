using AutoMapper;
using Inventex.API.Management.Domain.Models;
using Inventex.API.Management.Resources;

namespace Inventex.API.Management.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile(){
        CreateMap<User, UserResource>();
        CreateMap<Machine, MachineResource>();
    }
}