
using System.ComponentModel.DataAnnotations;

namespace Battleship.Tracker.Features.StateTracker
{
    public class BattleshipRequest
    {
        [Required]
        [RegularExpression("^(vertical|horizontal)$")]
        public string Orientation { get; set; }
        [Required]
        public Coordinates Position { get; set; }
        [Required]
        [Range(1, 50, ErrorMessage = "'Length' should be greatter than 1")]
        public int Length { get; set; }
    }
}
