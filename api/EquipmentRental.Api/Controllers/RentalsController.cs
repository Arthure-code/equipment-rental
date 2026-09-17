using EquipmentRental.Api.Dtos;
using EquipmentRental.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers
{
    // A machine's rentals: one to open, the current one to cancel.
    [ApiController]
    [Route("api/equipment/{id:int}/rentals")]
    [Produces("application/json")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentals;

        public RentalsController(IRentalService rentals)
        {
            _rentals = rentals;
        }

        [HttpPost]
        public async Task<ActionResult<RentalDto>> Rent(int id, RentalRequest request)
        {
            var (outcome, rental) = await _rentals.RentAsync(id, request.Days);
            return outcome switch
            {
                RentalOutcome.NotFound => NotFound(),
                RentalOutcome.AlreadyRented => Conflict(new { message = "This machine is already rented." }),
                _ => CreatedAtAction("Get", "Equipment", new { id }, rental),
            };
        }

        [HttpDelete("current")]
        public async Task<IActionResult> Cancel(int id)
        {
            return await _rentals.CancelAsync(id) ? NoContent() : NotFound();
        }
    }
}
