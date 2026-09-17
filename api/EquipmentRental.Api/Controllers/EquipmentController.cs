using EquipmentRental.Api.Dtos;
using EquipmentRental.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    [Produces("application/json")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipment;

        public EquipmentController(IEquipmentService equipment)
        {
            _equipment = equipment;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentDto>>> GetAll()
        {
            return Ok(await _equipment.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EquipmentDto>> Get(int id)
        {
            var dto = await _equipment.GetAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}
