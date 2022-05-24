namespace Domino
{
    // List<Chip> класс описывающий кость
    public class Chip
    {
        public int Left { get; set; }
        public int Right { get; set; }

        public Chip(int left, int right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var chip = obj as Chip;

            return chip.Left == this.Left && chip.Right == this.Right;
        }
    }
}