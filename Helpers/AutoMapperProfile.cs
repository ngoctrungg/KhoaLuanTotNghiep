using AutoMapper;
using KLTN_E.Data;
using KLTN_E.ViewModels;

namespace KLTN_E.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, KhachHang>();
        }

    }
}
