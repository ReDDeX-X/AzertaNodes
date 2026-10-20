using AutoMapper;
using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Mappings
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDto>();
        }
    }
}
