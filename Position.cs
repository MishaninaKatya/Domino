namespace Domino
{
    //класс для работы с координатами изображения
    public struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int v1, int v2) : this()
        {
            this.X = v1;
            this.Y = v2;
        }
    }
}
