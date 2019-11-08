using System.ComponentModel.DataAnnotations;

namespace Battleship.Tracker
{
    public class BattleBoardRequest
    {
        // Square Board size.
        [Required]
        [Range(1, 50, ErrorMessage = "'Size' should be between 1 and 50")]
        public int Size { get; set; }
    }
}
