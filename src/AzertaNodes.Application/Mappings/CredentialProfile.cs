using AutoMapper;
using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Mappings
{
    public class CredentialProfile : Profile
    {
        public CredentialProfile()
        {
            CreateMap<Credential, CredentialDto>();
        }
    }
}