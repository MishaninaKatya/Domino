using System;
using System.Collections.Generic;

namespace Domino
{
    //Класс для хранения колоды игрока
    public class Player
    {
        public List<Chip> Deck = new List<Chip>();
        private Chip chipToMove;

        public Player(Action predicate)
        {
            TransferMovePredicate = predicate;
        }

        public Action TransferMovePredicate { get; }

    }
}