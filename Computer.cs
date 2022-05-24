using System;
using System.Collections.Generic;
using System.Linq;

namespace Domino
{
    public class Computer
    {
        public List<Chip> Deck = new List<Chip>();
        // помогает отслеживать количество каждых половинок в игре
        public Dictionary<int, int> NumbersScore = new Dictionary<int, int>()
        {
            {0, 8},
            {1, 8},
            {2, 8},
            {3, 8},
            {4, 8},
            {5, 8},
            {6, 8}
        };

        private int CheckDoubles(List<Chip> chips)
        {
            foreach (var chip in chips)
            {
                if (chip.Right == chip.Left)
                {
                    return chips.IndexOf(chip);
                }
            }

            return -1;
        }

        // выбор фишки на основе анализа фишек в игре
        private int CheckPlayersDeck(List<Chip> chips, int leftChip, int rightChip)
        {

            foreach (var number in NumbersScore.OrderBy(x => x.Value))
            {
                var numberToFind = number.Key;
                foreach (var chip in chips)
                {
                    if (chip.Right == leftChip && chip.Left == numberToFind)
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Left == leftChip && chip.Right == numberToFind)
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Right == rightChip && chip.Left == numberToFind)
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Left == rightChip && chip.Right == numberToFind)
                    {
                        return chips.IndexOf(chip);
                    }
                }
            }

            throw new Exception("Невозможный исход, фишка должна быть найдена на третьем шаге");
        }
        // допустим игрок не смог сходить когда на концах было 1 и 5
        private List<int> MissedChips = new List<int>();
        //Проверяем можем ли сходить костяшками, которых нет у игрока
        private int CheckMissedPlayerChips(List<Chip> chips, int leftChip, int rightChip) // 3-4   1-2   2-6   5-1
        {
            // допускаем что на концах сейчас 6 и 5
            if (MissedChips.Any())
            {
                foreach (var chip in chips)
                {
                    if (chip.Left == leftChip && MissedChips.Contains(chip.Right))
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Right == leftChip && MissedChips.Contains(chip.Left))
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Left == rightChip && MissedChips.Contains(chip.Right))
                    {
                        return chips.IndexOf(chip);
                    }

                    if (chip.Right == rightChip && MissedChips.Contains(chip.Left))
                    {
                        return chips.IndexOf(chip);
                    }
                }
            }

            return -1;
        }

        // Возвращает индекс фишки которой компьютер сходит
        public Chip CalcNextChip(List<Chip> chips, int leftChip, int rightChip)
        {
            // 1 проверка на дубли
            var index = CheckDoubles(chips);

            if (index != -1)
            {
                return chips[index];
            }

            // 2 масти которые игрок не смог выложить
            index = CheckMissedPlayerChips(chips, leftChip, rightChip);

            if (index != -1)
            {
                return chips[index];
            }

            // 3 минимально-вероятные масти у игрока
            index = CheckPlayersDeck(chips, leftChip, rightChip);

            return chips[index];

        }
        //Понижаем колличества сторон определенной фишки 
        internal void LowScore(Chip chip)
        {
            NumbersScore[chip.Left]--;
            NumbersScore[chip.Right]--;
        }
        //Зафиксировать стороны, которых нет у игрока
        internal void FillMissedChips(int leftChip, int rightChip)
        {
            MissedChips.Add(leftChip);
            MissedChips.Add(rightChip);
        }
    }

}