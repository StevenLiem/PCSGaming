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
        private int hargaGame;
        private string namaGame;

        public CartItem(string kodeGame, int amount)
        {
            this.kodeGame = kodeGame;
            this.amount = amount;
            if (!kodeGame.Contains("BDL"))
            {
                this.namaGame = MainWindow.ambilstring($"SELECT NAME FROM GAME WHERE GAME_ID ='{kodeGame}'");
                this.hargaGame = Int32.Parse(MainWindow.ambilstring($"SELECT PRICE FROM GAME WHERE GAME_ID ='{kodeGame}'"));
            }
            else
            {
                this.namaGame = MainWindow.ambilstring($"SELECT NAME FROM BUNDLE WHERE BUNDLE_ID ='{kodeGame}'");
                this.hargaGame = Int32.Parse(MainWindow.ambilstring($"SELECT PRICE * ((100-DISCOUNT)/100) FROM BUNDLE WHERE BUNDLE_ID ='{kodeGame}'"));
            }
            
        }

        public string getKode()
        {
            return kodeGame;
        }
        public int getHarga()
        {
            return hargaGame;
        }
        public int getJumlah()
        {
            return amount;
        }

        public string getNama()
        {
            return namaGame;
        }

        public void setAmount(int amount)
        {
            this.amount = amount;
        }
    }
}
