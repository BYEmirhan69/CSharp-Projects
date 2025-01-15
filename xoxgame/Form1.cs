using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xoxgame
{
    public partial class Form1 : Form
    {

        bool X_Sıra;
        bool O_Sıra;
        int islem_sayisi = 0;
        public Form1()
        {
            InitializeComponent();
            X_Sıra = true;
            sıraLabel.Text = "Sıra: X";
        }

        public void X_Checker()
        {
            // Yatay
            if (button1.Text == "X" && button2.Text == "X" && button3.Text == "X")
            {
                KazanmaMesaji("X");
            }
            if (button4.Text == "X" && button5.Text == "X" && button6.Text == "X")
            {
                KazanmaMesaji("X");
            }
            if (button7.Text == "X" && button8.Text == "X" && button9.Text == "X")
            {
                KazanmaMesaji("X");
            }

            // Dikey
            if (button1.Text == "X" && button4.Text == "X" && button7.Text == "X")
            {
                KazanmaMesaji("X");
            }
            if (button2.Text == "X" && button5.Text == "X" && button8.Text == "X")
            {
                KazanmaMesaji("X");
            }
            if (button3.Text == "X" && button6.Text == "X" && button9.Text == "X")
            {
                KazanmaMesaji("X");
            }

            // Çapraz
            if (button1.Text == "X" && button5.Text == "X" && button9.Text == "X")
            {
                KazanmaMesaji("X");
            }
            if (button3.Text == "X" && button5.Text == "X" && button7.Text == "X")
            {
                KazanmaMesaji("X");
            }


        }

        public void O_Checker()
        {

            // Yatay
            if (button1.Text == "O" && button2.Text == "O" && button3.Text == "O")
            {
                KazanmaMesaji("O");
            }
            if (button4.Text == "O" && button5.Text == "O" && button6.Text == "O")
            {
                KazanmaMesaji("O");
            }
            if (button7.Text == "O" && button8.Text == "O" && button9.Text == "O")
            {
                KazanmaMesaji("O");
            }

            // Dikey
            if (button1.Text == "O" && button4.Text == "O" && button7.Text == "O")
            {
                KazanmaMesaji("O");
            }
            if (button2.Text == "O" && button5.Text == "O" && button8.Text == "O")
            {
                KazanmaMesaji("O");
            }
            if (button3.Text == "O" && button6.Text == "O" && button9.Text == "O")
            {
                KazanmaMesaji("O");
            }

            // Çapraz
            if (button1.Text == "O" && button5.Text == "O" && button9.Text == "O")
            {
                KazanmaMesaji("O");
            }
            if (button3.Text == "O" && button5.Text == "O" && button7.Text == "O")
            {
                KazanmaMesaji("O");
            }



        }
        public void Temizle()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button button && button.Name.StartsWith("button"))
                {
                    button.Text = "";
                    button.Enabled = true;
                }
            }
        }


        public void KazanmaMesaji(String winner)
        {
            if(winner == "X") {
                MessageBox.Show("X kazandı", "Tebrikler");
   
            } else if (winner == "O")
            {
                MessageBox.Show("O kazandı", "Tebrikler");
         
            }

            Temizle();
            islem_sayisi = 0;


        }

        public void BeraberlikChecker()
        {
            if(islem_sayisi == 9)
            {
                Temizle();
                MessageBox.Show("Berabere bitti.", "Beraberlik Durumu");

            }
        }



        private void ClickEvent_All(object sender, EventArgs e)
        {
            islem_sayisi++;
            Button b = (Button)sender;
            if (X_Sıra)
            {
                X_Sıra = false;
                O_Sıra = true;
                b.Text = "X";
                b.Enabled = false;

                X_Checker();
                BeraberlikChecker();
                sıraLabel.Text = "Sıra: O";

            }
            else if (O_Sıra)
            {
                X_Sıra = true;
                O_Sıra = false;
                b.Text = "O";
                b.Enabled = false;
                O_Checker();
                BeraberlikChecker();
                sıraLabel.Text = "Sıra: X";


            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
