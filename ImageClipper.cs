using System;
using System.Collections.Generic;
using System.Drawing;

namespace Domino
{
    //Класс для работы с изображением
    public class ImageClipper
    {
        public static List<Chip> Deck = new List<Chip>()
        {
            new Chip(0, 0),
            new Chip(0, 1),
            new Chip(0, 2),
            new Chip(0, 3),
            new Chip(0, 4),
            new Chip(0, 5),
            new Chip(0, 6),
            new Chip(1, 1),
            new Chip(1, 2),
            new Chip(1, 3),
            new Chip(1, 4),
            new Chip(1, 5),
            new Chip(1, 6),
            new Chip(2, 2),
            new Chip(2, 3),
            new Chip(2, 4),
            new Chip(2, 5),
            new Chip(2, 6),
            new Chip(3, 3),
            new Chip(3, 4),
            new Chip(3, 5),
            new Chip(3, 6),
            new Chip(4, 4),
            new Chip(4, 5),
            new Chip(4, 6),
            new Chip(5, 5),
            new Chip(5, 6),
            new Chip(6, 6)
        };

        private static readonly Position StartPosition = new Position(0, 0);
        public const int Width = 78;
        public const int Height = 154;
        public const int HorizontalSpace = 0;
        public const int VerticalSpace = 54;

        private const int Rows = 7;
        private const int Columns = 4;

        public const int NewWidth = Width / 2;
        public const int NewHeight = Height / 2;
        //Получение изображения для костей компьютера 
        public static Bitmap GetUndefinedImage()
        {
            var cropRect = new Rectangle(0, 0, Width, Height);
            Bitmap src = Image.FromFile(Environment.CurrentDirectory + "\\Sprites\\undefined_domino.png") as Bitmap;
            Bitmap target = new Bitmap(NewWidth, NewHeight);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }
        //Получить изображение нужной костяшки
        public static Bitmap GetImage(Chip chip)
        {
            var index = Deck.IndexOf(chip);

            int row = 0;
            int col = 0;

            if (index < 7)
            {
                row = 0;
                col = index;                
            } else if (index < 14)
            {
                row = 1;
                col = index - 7;
            } else if (index < 21)
            {
                row = 2;
                col = index - 14;
            } else
            {
                row = 3;
                col = index - 21;
            }

            var x = StartPosition.X + Width * col;
            var y = StartPosition.Y + (Height + VerticalSpace) * row;
            var cropRect = new Rectangle(x, y, Width, Height);
            Bitmap src = Image.FromFile(Environment.CurrentDirectory + "\\Sprites\\dominos.png") as Bitmap;
            Bitmap target = new Bitmap(NewWidth, NewHeight);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}
