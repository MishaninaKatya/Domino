using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Threading;

namespace Domino
{
    public partial class Form4 : Form
    {
        private const int MaxHorizontalCount = 5;
        private const int MaxHorizontalVerticalCount = 8;
        private List<Button> PlayerChips = new List<Button>();
        private List<Button> ComputerChips = new List<Button>();
        private List<Chip> chips = new List<Chip>();
        private List<ChipPlacement> placements = new List<ChipPlacement>();
        private Chip startChip = null;
        private Game game = new Game(winPlayer => MessageBox.Show(winPlayer ? "Победил игрок!" : "Победил компьютер!"));
        private bool isInited;

        private int BonesLeft = 0;
        private int BonesRight = 0;

        public Form4()
        {
            InitializeComponent();
        }

        public Form4(bool manualInit)
        {
            isInited = true;
            InitializeComponent();
        }

        public Form4 LoadGame(string save)
        {
            var lines = save.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var mode = 0;

            foreach (var line in lines)
            {
                if (line == "C" || line == "P")
                {
                    mode++;
                    continue;
                }

                var placement = line.Split(':');

                switch (mode)
                {
                    case 0:
                        var fc = lines[0].Split(':').Select(int.Parse).ToArray();
                        var firstChip = new Chip(fc[0], fc[1]);
                        DrawStartBone(firstChip);
                        mode++;
                        break;
                    case 1:
                        var chip = new Chip(int.Parse(placement[0]), int.Parse(placement[1]));
                        var isLeft = bool.Parse(placement[2]);
                        var rotation = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), placement[3]);

                        chips.Add(chip);
                        game.Deck.Remove(chip);
                        DrawBoneOnTable(chip, isLeft, rotation);
                        break;
                    case 2:
                        var pChip = new Chip(int.Parse(placement[0]), int.Parse(placement[1]));
                        game.Player.Deck.Add(pChip);
                        DrawPlayerBone(pChip);
                        break;
                    case 3:
                        var cChip = new Chip(int.Parse(placement[0]), int.Parse(placement[1]));
                        game.Computer.Deck.Add(cChip);
                        DrawComputerBone();
                        break;
                }
            }

            return this;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            if (isInited) { return; }

            // Инициализируем игру 
            game.StartGame(DrawStartBone, DrawPlayerBone, DrawComputerBone);
            chips.Add(game.startChip);
        }
        //Рисуем кость на игровом столе
        private void DrawBoneOnTable(Chip chip, bool left, RotateFlipType flipType)
        {
            chips.Add(chip);
            using (var g = Graphics.FromImage(pictureBox1.Image))
            {
                using (var chipImage = ImageClipper.GetImage(chip))
                {
                    chipImage.RotateFlip(flipType);

                    // Нужно для того чтобы на поле фишки побольше выглядели
                    var newWidth = chipImage.Width /2 ;
                    var newHeight = chipImage.Height/2 ;

                    var startX = pictureBox1.Image.Width / 2 - newWidth / 2;
                    var startY = pictureBox1.Image.Height / 2 - newHeight / 2;

                    if (left)
                    {
                        if (BonesLeft >= MaxHorizontalCount)
                        {
                            if (BonesLeft >= MaxHorizontalVerticalCount)
                            {
                                chipImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                startX -= MaxHorizontalCount * newWidth;
                                startY -= (MaxHorizontalVerticalCount - MaxHorizontalCount) * newWidth;
                                startX += newHeight;
                                startX += (BonesLeft - MaxHorizontalVerticalCount) * newWidth;
                            }
                            else
                            {
                                chipImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                startX -= MaxHorizontalCount * newWidth;

                                var temp = newWidth;
                                newWidth = newHeight;
                                newHeight = temp;

                                var moveUp = BonesLeft - MaxHorizontalCount + 1;
                                startY -= moveUp * newHeight;
                            }
                        } else
                        {
                            startX -= (BonesLeft + 1) * newWidth;
                        }

                        BonesLeft++;
                    }
                    else
                    {
                        if (BonesRight >= MaxHorizontalCount)
                        {
                            if (BonesRight >= MaxHorizontalVerticalCount)
                            {
                                chipImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                startX += (MaxHorizontalCount + 1) * newWidth;
                                startY -= (MaxHorizontalVerticalCount - MaxHorizontalCount) * newWidth;
                                startX -= newHeight;
                                startX -= (BonesRight - MaxHorizontalVerticalCount + 1) * newWidth;
                            } else
                            {
                                chipImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                startX += MaxHorizontalCount * newWidth + 1;

                                var temp = newWidth;
                                newWidth = newHeight;
                                newHeight = temp;

                                startX += newWidth;

                                var moveUp = BonesRight - MaxHorizontalCount + 1;
                                startY -= moveUp * newHeight;
                            }
                        } else
                        {
                            startX += (BonesRight + 1) * newWidth;
                        }

                        BonesRight++;
                    }

                    // Прямоугольная область в которую мы поместим наше изображение на фон
                    RectangleF destinationRect = new RectangleF(startX, startY, newWidth, newHeight);

                    // Прямоугольная область которую мы "откусим" от изображения нашей фишки домино
                    RectangleF sourceRect = new RectangleF(0, 0, chipImage.Width, chipImage.Height);

                    g.DrawImage(chipImage, destinationRect, sourceRect, GraphicsUnit.Pixel);
                }
            }
            Refresh();
        }
        //Рисуем стартовую кость
        private void DrawStartBone(Chip chip)
        {
            var img = pictureBox1.Image;
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                var chipImage = ImageClipper.GetImage(chip);
                chipImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

                // Установка размеров костяшек
                var newWidth = chipImage.Width / 2  ;
                var newHeight = chipImage.Height / 2 ;

                var startX = img.Width / 2 - newWidth / 2;
                var startY = img.Height / 2 - newHeight / 2;

                // Прямоугольная область в которую мы поместим наше изображение на фон
                RectangleF destinationRect = new RectangleF(startX, startY, newWidth, newHeight);

                // Прямоугольная область которую мы "откусим" от изображения нашей фишки домино
                RectangleF sourceRect = new RectangleF(0, 0, chipImage.Width, chipImage.Height);

                g.DrawImage(chipImage, destinationRect, sourceRect, GraphicsUnit.Pixel);
            }
        }
        //Рисуем кость компьютера
        private void DrawComputerBone()
        {
            var chipImage = ImageClipper.GetUndefinedImage();

            var b = new Button();

            b.Image = chipImage;
            b.Size = new Size(chipImage.Width, chipImage.Height);
            b.Location = new Point(Width - chipImage.Width / 2 - ((ComputerChips.Count + 1) * chipImage.Width), Height - (int)(chipImage.Height * 1.5f));

            ComputerChips.Add(b);

            this.Controls.Add(b);
            Invalidate();
        }
        //рисуем кость игрока
        private void DrawPlayerBone(Chip chip)
        {
            var chipImage = ImageClipper.GetImage(chip);

            var b = new Button();

            b.Click += (sen, ev) =>
            {
                var isOk = game.PlayerMove(chip, (_, isLeft, flipType) =>
                {
                    DrawPlayerMove(chip, isLeft, flipType, b);
                }, chipToSelect =>
                {
                    var result = MessageBox.Show($"Нажмите\nДа чтобы выбрать {chipToSelect.Left}\nИли Нет чтобы выбрать {chipToSelect.Right}", "Выбор стороны домино", MessageBoxButtons.YesNo);
                    return result.ToString() == "Yes";
                });

                if (isOk)
                {
                    game.ComputerMove(DrawComputerMove, DrawComputerBone);
                }
            };

            b.Image = chipImage;
            b.Size = new Size(chipImage.Width, chipImage.Height);
            b.Location = new Point(PlayerChips.Count * (int)(chipImage.Width), Height - (int)(chipImage.Height * 1.5f));

            PlayerChips.Add(b);

            Controls.Add(b);

            Invalidate();
        }
        //Функция отвечает за ход игрока
        private void DrawPlayerMove(Chip chip, bool isLeft, RotateFlipType flipType, Button b)
        {
            PlayerChips.Remove(b);
            Controls.Remove(b);
            placements.Add(new ChipPlacement(chip, isLeft, flipType, true));
            DrawBoneOnTable(chip, isLeft, flipType); // TO DO передать нормально влево или вправо пойти передвинуть остальное влево
            var counter = 0;
            foreach (var pb in PlayerChips)
            {
                pb.Location = new Point(counter++ * (int)(ImageClipper.NewWidth), Height - (int)(ImageClipper.NewHeight * 1.5f));
            }
        }
        //Функция отвечает за ход компьютера
        private void DrawComputerMove(Chip chip, bool isLeft, RotateFlipType flipType)
        {
            var computerChip = ComputerChips.Last();
            ComputerChips.Remove(computerChip);
            Controls.Remove(computerChip);
            placements.Add(new ChipPlacement(chip, isLeft, flipType, false));
            DrawBoneOnTable(chip, isLeft, flipType); // TO DO передать нормально влево или вправо пойти
                                                     // передвинуть остальное влево
        }

        //Взять из базара
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            var result = game.PlayerTake(DrawPlayerBone);
            if (!result)
            {
                MessageBox.Show("Базар пуст!");
                game.CheckEndOfGame();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        //Пропустить ход игроку
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            game.ComputerMove(DrawComputerMove, DrawComputerBone);
        }
        //Запуск новый игры
        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
            //new Form4();
            //this.Close();
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            if (chips[0] == null)
            {
                MessageBox.Show("Нечего сохранять!");
                return;
            }
            Form6 form6 = new Form6();
            form6.ShowDialog();
            if (form6.DialogResult != DialogResult.OK) return;
            string name = "save" + Convert.ToString(form6.numericUpDown1.Value) + ".txt";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            string dir = Path.Combine(Environment.CurrentDirectory, "saves");
            string fileName = Path.Combine(dir, name);
            Directory.CreateDirectory(dir);
            File.WriteAllText(fileName, string.Empty);

            using (var sw = new StreamWriter(fileName))
            {
                sw.WriteLine(game.startChip.Left + ":" + game.startChip.Right);
                sw.WriteLine();

                foreach (var p in placements)
                {
                    sw.WriteLine($"{p.Chip.Left}:{p.Chip.Right}:{p.IsLeft}:{p.RotateFlipType}:{p.IsPlayerMove}");
                }
                sw.WriteLine("P");
                foreach (var p in game.Player.Deck)
                {
                    sw.WriteLine($"{p.Left}:{p.Right}");
                }
                sw.WriteLine("C");
                foreach (var p in game.Computer.Deck)
                {
                    sw.WriteLine($"{p.Left}:{p.Right}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.ShowDialog();
            if (form5.DialogResult != DialogResult.OK) return;
            string name = "save" + Convert.ToString(form5.numericUpDown1.Value) + ".txt";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "saves", name);
            if (File.Exists(fileName) == false)
            {
                MessageBox.Show("Слот сохранения пуст!");
                return;
            }
            string save = File.ReadAllText(fileName);
            var newForm = new Form4(true).LoadGame(save);
            newForm.Show();

            save.ToCharArray();

        }
    }
}
