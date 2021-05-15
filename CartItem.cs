using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS_Gaming
{
    class CartItem
    {
        private string kodeGame;
        private string namaGame;
        private int amount;

        public CartItem(string kodeGame, string namaGame, int amount)
        {
            this.kodeGame = kodeGame;
            this.namaGame = namaGame;
            this.amount = amount;
        }
    }
}
