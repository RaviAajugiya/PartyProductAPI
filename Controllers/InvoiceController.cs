using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyProductAPI.DTOs;

namespace PartyProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly PartyProductApiContext contex;
        private readonly IMapper mapper;

        public InvoiceController(PartyProductApiContext contex, IMapper mapper)
        {
            this.contex = contex;
            this.mapper = mapper;

            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Invoice, InvoiceDTO>()
                    .ForMember(dest => dest.PartyName, opt => opt.MapFrom(src => src.Party.PartyName))
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
                cfg.CreateMap<InvoiceCreateDTO, Invoice>();
            }).CreateMapper();
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<List<InvoiceDTO>>> Get(int Id)
        {
            var invoice = await contex.Invoices.Include(x => x.Party).Include(x => x.Product).Where(x => x.PartyId == Id).ToListAsync();
            var invoiceDTO = mapper.Map<List<InvoiceDTO>>(invoice);
            return invoiceDTO;
        }

        [HttpPost]
    }
}
