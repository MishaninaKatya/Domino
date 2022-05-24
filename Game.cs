using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Domino
{

    internal class Game
    {
        public Game(Action<bool> eogAction)
        {
            EndOfGameAction = eogAction;
        }

        // private FormDrawer _drawer;
        //Список костяшек
        public List<Chip> Deck = new List<Chip>()
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
 
        private int leftChip = 0;
        private int rightChip = 0;
        public Chip startChip;
        //Создание игрока
        public Player Player { get; set; } = new Player(() => { playerMove = false;  });
        //Создание компьютера
        public Computer Computer { get; set; } = new Computer();
        //Действие по завершению игры
        public Action<bool> EndOfGameAction { get; private set; }

        private static bool playerMove = false;

        private bool endOfGame = false;

        private Random Randomizer = new Random();
         //Логика хода компьютера    
        internal void ComputerMove(Action<Chip, bool, RotateFlipType> computerMove, Action drawComputerBone)
        {
            var result = ReturnActualChips(Computer.Deck);

            if (result.Any()) // если есть фишки которыми мы можем сходить
            {
                var chip = Computer.CalcNextChip(result, leftChip, rightChip);
                var isOk = TryDraw(chip, computerMove);

                if (isOk)
                {
                    Computer.Deck.Remove(chip);
                    Computer.LowScore(chip);
                }

                if (Computer.Deck.Count == 0)
                {
                    EndOfGameAction(false);
                }

                playerMove = true;
            }
            else
            {
                if (Deck.Count == 0)
                {
                    EndOfGame();
                }
                else
                {
                    TakeFromDeck(Computer.Deck, false);
                    drawComputerBone();
                }
            }
        }
        //Игрок берет кость
        internal bool PlayerTake(Action<Chip> drawChip)
        {
            var chip = TakeFromDeck(Player.Deck, true);

            if (chip != null)
            {
                drawChip(chip);
                return true;
            }

            return false;
        }
        //Рассчет хода игрока
        internal bool PlayerMove(Chip chip, Action<Chip, bool, RotateFlipType> drawAction, Func<Chip, bool> ChoosePlayerBoneSide)
        {
            var exists = Player.Deck.Contains(chip);

            if (!exists)
            {
                throw new Exception("у игрока нет такой фишки");
            }

            var isOk = TryDrawPlayer(chip, drawAction, ChoosePlayerBoneSide);

            if (!isOk)
            {
                CheckEndOfGame();
                return false;
            }

            Player.Deck.Remove(chip);
            Computer.LowScore(chip);

            if (Player.Deck.Count == 0)
            {
                EndOfGameAction(true);
            }

            playerMove = false;
            return true;
        }
        //Рассчитываем , как будет нарисована кость на столе
        private bool TryDrawPlayer(Chip chip, Action<Chip, bool, RotateFlipType> drawAction, Func<Chip, bool> ChoosePlayerBoneSide)
        {
            RotateFlipType rotating;
            bool toLeft;

            if (chip.Left == leftChip && chip.Right == rightChip)
            {
                var isLeft = ChoosePlayerBoneSide(chip);
                if (isLeft)
                {
                    rotating = RotateFlipType.Rotate90FlipNone;
                    leftChip = chip.Right;
                    toLeft = true;
                }
                else
                {
                    rotating = RotateFlipType.Rotate90FlipNone;
                    rightChip = chip.Left;
                    toLeft = false;
                }

                drawAction(chip, toLeft, rotating);
                return true;
            }
            else if (chip.Right == leftChip && chip.Left == rightChip)
            {
                var isLeft = ChoosePlayerBoneSide(chip);
                if (isLeft)
                {
                    rotating = RotateFlipType.Rotate270FlipNone;
                    leftChip = chip.Left;
                    toLeft = true;
                }
                else
                {
                    rotating = RotateFlipType.Rotate270FlipNone;
                    rightChip = chip.Right;
                    toLeft = false;
                }

                drawAction(chip, toLeft, rotating);
                return true;
            }

            return TryDraw(chip, drawAction);
        }
        //Рассчитываем положение кости на столе для компьютера 
        private bool TryDraw(Chip chip, Action<Chip, bool, RotateFlipType> drawAction)
        {
            RotateFlipType rotating;
            bool toLeft;

            if (chip.Left == leftChip)
            {
                rotating = RotateFlipType.Rotate90FlipNone;
                leftChip = chip.Right;
                toLeft = true;
            }
            else if (chip.Right == rightChip)
            {
                rotating = RotateFlipType.Rotate90FlipNone;
                rightChip = chip.Left;
                toLeft = false;
            }
            else if (chip.Left == rightChip)
            {
                rotating = RotateFlipType.Rotate270FlipNone;
                rightChip = chip.Right;
                toLeft = false;
            }
            else if (chip.Right == leftChip)
            {
                rotating = RotateFlipType.Rotate270FlipNone;
                leftChip = chip.Left;
                toLeft = true;
            }
            else
            {
                return false;
                // не можем сходить никуда, не делаем ничего 
            }


            drawAction(chip, toLeft, rotating);
            return true;
        }

        // Инициализируем игру
        public void StartGame(Action<Chip> drawStartBone, Action<Chip> drawPlayerBone, Action drawComputerBone)
        {
            Computer.Deck = GetDeck();
            Player.Deck = GetDeck();

            startChip = Deck[Randomizer.Next(Deck.Count)];
            leftChip = startChip.Left;
            rightChip = startChip.Right;
            drawStartBone(startChip);
            Computer.LowScore(startChip);

            foreach (var chip in Player.Deck)
            {
                drawPlayerBone(chip);
            }

            for (int i = 0; i < Computer.Deck.Count; i++)
            {
                drawComputerBone();
            }

            ChooseFirstMove(); 
        }

        // определить кто первый ходит
        private void ChooseFirstMove()
        {
            playerMove = Randomizer.NextDouble() < 0.5;
        }

        // возвращаем список фишек которыми можем сходить
        private List<Chip> ReturnActualChips(List<Chip> deck)
        {
            return deck.Where(phishka => phishka.Left == leftChip || phishka.Left == rightChip || phishka.Right == leftChip ||
                                  phishka.Right == rightChip).ToList();
        }
        //Пробуем взять кость из списка
        private Chip TakeFromDeck(List<Chip> deck, bool playersMove)
        {
            if (playersMove)
            {
                Computer.FillMissedChips(leftChip, rightChip);
                // TO:DO если игрок берёт фишку, то заполняем коллекцию отсутствующих фишек - MissedChips
            }

            var chip = GetChip(deck);
            return chip;
        }
        //Проверка конца игры
        private void EndOfGame()
        {
            if (ReturnActualChips(Computer.Deck).Any() || ReturnActualChips(Player.Deck).Any())
            {
                return;
            }

            if (Deck.Count == 0)
            {
                var playerVictory = Player.Deck.Count == 0 || Deck.Count == 0 && Player.Deck.Count < Computer.Deck.Count;
                EndOfGameAction(playerVictory);
            }
        }

        // Получение колоды из общего базара
        public List<Chip> GetDeck()
        {
            var newDeck = new List<Chip>();

            for (var i = 0; i < 7; i++)
            {
                GetChip(newDeck);
            }

            return newDeck;
        }
        //Получение кости из общего базара
        private Chip GetChip(List<Chip> newDeck)
        {
            Chip c = null;
            if (Deck.Count > 0)
            {
                var index = Randomizer.Next(Deck.Count - 1);
                c = Deck[index];
                newDeck.Add(c);
                Deck.RemoveAt(index);
            }

            return c;
        }

        internal void CheckEndOfGame()
        {
            EndOfGame();
        }
    }

}