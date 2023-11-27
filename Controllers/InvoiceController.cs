using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                cfg.CreateMap<AssignParty, AssignPartyDTO>()
                    .ForMember(dest => dest.PartyName, opt => opt.MapFrom(src => src.Party.PartyName))
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
                cfg.CreateMap<AssignPartyCreateDTO, AssignParty>();
            }).CreateMapper();
        }

        [HttpGet]
        public IActionResult GetInvoices(int? productId = null, DateTime? date = null, int? partyId = null)
        {
            try
            {
                var invoicesQuery = contex.Invoices
                    .FromSqlRaw("EXEC GetInvoices @productId, @Date, @partyId",
                        new SqlParameter("@productId", productId),
                        new SqlParameter("@Date", date),
                        new SqlParameter("@partyId", partyId))
                    .ToList();

                // Fetch related entities separately
                foreach (var invoice in invoicesQuery)
                {
                    contex.Entry(invoice)
                        .Reference(i => i.Party)
                        .Load();
                }

                var invoiceDTO = mapper.Map<InvoiceDTO>(invoicesQuery);

                return Ok(invoiceDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }





        [HttpPost]
        public async Task<ActionResult> Post([FromBody] InvoiceCreateDTO invoiceCreate)
        {
            var invoice = mapper.Map<Invoice>(invoiceCreate);
            contex.Add(invoice);
            try
            {
                await contex.SaveChangesAsync();
                var invoiceDTO = mapper.Map<InvoiceDTO>(invoice);
                return NoContent();
            }
            catch (Exception ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && sqlException.Number == 2627)
                {
                    return BadRequest("Party already exists.");
                }
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var exists = await contex.Invoices.AnyAsync(x => x.PartyId == Id);
            if (!exists)
            {
                return NotFound();
            }
            contex.Remove(new Invoice { InvoiceId = Id });
            await contex.SaveChangesAsync();
            return NoContent();
        }

    }
}
