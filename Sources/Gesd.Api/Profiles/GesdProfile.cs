using AutoMapper;
using Gesd.Api.Dtos;

using File = Gesd.Entite.File;

namespace Gesd.Api.Profiles
{
    public class GesdProfile : Profile
    {
        public GesdProfile()
        {
            CreateMap<File, FileToAddDto>().ReverseMap();
            CreateMap<File, FileAddedDto>().ReverseMap();
            CreateMap<File, FileDto>().ReverseMap();
        }
    }
}
