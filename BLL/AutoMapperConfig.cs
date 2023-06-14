using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using InShapeModels;

namespace BLL
{
    public class AutoMapperConfig: Profile
    {
        protected override void Configure()
        {
            CreateMap<ProfileTracking, UserProfileModel>().ReverseMap();
            CreateMap<GetWild.Models.ProfileViewModel, UserProfileModel>().ReverseMap();
            
        }
    }
}
