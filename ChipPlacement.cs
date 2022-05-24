using System.Drawing;

namespace Domino
{
    public class ChipPlacement
    {
        public Chip Chip { get; set; }
        public bool IsLeft { get; set; }
        public RotateFlipType RotateFlipType { get; set; }
        public bool IsPlayerMove { get; set; }

        public ChipPlacement(Chip chip, bool isLeft, RotateFlipType rotateFlipType, bool isPlayerMove)
        {
            Chip = chip;
            IsLeft = isLeft;
            RotateFlipType = rotateFlipType;
            IsPlayerMove = isPlayerMove;
        }
    }
}
