
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Battleship.Tracker.Features.StateTracker
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackerController : ControllerBase
    {
        private readonly ITrackerService trackerService;
        public TrackerController(ITrackerService trackerService)
        {
            this.trackerService = trackerService;
        }

        [HttpPost("createboard")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateBoard(BattleBoardRequest battleBoard)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = trackerService.CreateBoard(battleBoard.Size);
            if (string.IsNullOrWhiteSpace(response))
            {
                return CreatedAtAction("createboard", null);
            }

            return Ok(response);
        }

        [HttpPost("addship")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PlaceShipOnBoard(BattleshipRequest battleship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

           var response= trackerService.PlaceShipOnBoard(battleship);
            return Ok(response);
        }

        [HttpPost("attack")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Attack(Coordinates coordinates)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = trackerService.Attack(coordinates.X, coordinates.Y);
            return response;
        }
    }
}