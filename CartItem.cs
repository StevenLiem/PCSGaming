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
        private int amount;

        public CartItem(string kodeGame, int amount)
        {
            this.kodeGame = kodeGame;
            this.amount = amount;
        }

        public string getKode()
        {
            return kodeGame;
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }
    }
}
