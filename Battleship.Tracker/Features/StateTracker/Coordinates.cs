using System;
using System.ComponentModel.DataAnnotations;

namespace Battleship.Tracker.Features.StateTracker
{
    public class Coordinates
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "'X' value should be greater than 0")]
        public int X { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "'Y' value should be greater than 0")]
        public int Y { get; set; }
    }
}
