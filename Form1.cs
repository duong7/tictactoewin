using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToeWin
{
    public partial class Form1 : Form
    {
        //0: empty; 1: user; 2: computer
        private byte[,] table = new byte[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        private readonly Random Rand = new Random();
        //enum Is { Empty, True, False }

        //Computer always moves first
        private byte[] player_first_move = new byte[2] { 0, 0 };
        private byte[] first_move = new byte[2] { 0, 0 };
        private byte[] latest_move = new byte[2] { 0, 0 };

        private byte player_middle = 0; //1: yes; 2: no
        private void Reset()
        {
            table = new byte[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            player_first_move = new byte[2] { 0, 0 };
            first_move = new byte[2] { 0, 0 };
            latest_move = new byte[2] { 0, 0 };
            player_middle = 0;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            //label2.Text = "";
            button1.Text = "";
            button2.Text = "";
            button3.Text = "";
            button4.Text = "";
            button5.Text = "";
            button6.Text = "";
            button7.Text = "";
            button8.Text = "";
            button9.Text = "";

            //button1.Text = "O";
            //table[0, 0] = 2;
            switch (Rand.Next(0, 3))
            {
                case 0:
                    button1.Text = "O";
                    table[0, 0] = 2;
                    //first_move = {0, 0};
                    break;
                case 1:
                    button3.Text = "O";
                    table[0, 2] = 2;
                    first_move[1] = 2;
                    latest_move = new byte[2] { 0, 2 };
                    break;
                case 2:
                    button7.Text = "O";
                    table[2, 2] = 2;
                    first_move = new byte[2] { 2, 2 };
                    latest_move = new byte[2] { 2, 2 };
                    break;
                default:
                    button9.Text = "O";
                    table[2, 0] = 2;
                    first_move[0] = 2;
                    latest_move = new byte[2] { 2, 0 };
                    break;
            }
        }

        private double[] RelPos(byte x1, byte y1, byte x2, byte y2, bool euclidean_distance = false)
        {
            double[] distance = new double[2] { 0, 0 };
            distance[0] = Math.Abs(x2 - x1);
            distance[1] = Math.Abs(y2 - y1);
            Array.Sort(distance);
            if (!euclidean_distance)
            {
                return distance;
            }
            else
            {
                return new double[1] { Math.Round(Math.Sqrt(Math.Pow(distance[0] + 1, 2) + Math.Pow(distance[1] + 1, 2)), 2) };
            }
        }

        private Button ButtonId(byte row, byte col)
        {
            switch (row)
            {
                case 0:
                    switch (col)
                    {
                        case 0:
                            return button1;
                        case 1:
                            return button2;
                        default:
                            return button3;
                    }
                case 1:
                    switch (col)
                    {
                        case 0:
                            return button6;
                        case 1:
                            return button5;
                        default:
                            return button4;
                    }
                default:
                    switch (col)
                    {
                        case 0:
                            return button9;
                        case 1:
                            return button8;
                        default:
                            return button7;
                    }
            }
        }
        private bool OnClick(byte row, byte col)
        {
            if (table[row, col] == 0)
            {
                if (player_first_move.SequenceEqual(new byte[] { 0, 0 }))
                {
                    player_first_move = new byte[] { row, col };
                }
                table[row, col] = 1;
                if (row == 1 && col == 1 && player_middle == 0) //dist[0] == 1 && dist[1] == 1
                {
                    player_middle = 1;
                }
                else
                {
                    if (player_middle == 0) player_middle = 2;
                }
                if (player_middle == 1)
                {
                    bool success = FinishOrBlock();
                    if (!success)
                    {
                        for (byte i = 0; i < 3; i++)
                        {
                            for (byte j = 0; j < 3; j++)
                            {
                                if (RelPos(i, j, first_move[0], first_move[1])[0] == 2
                                    && RelPos(i, j, first_move[0], first_move[1])[1] == 2
                                    && table[i, j] == 0)
                                {
                                    table[i, j] = 2;
                                    first_move = new byte[] { i, j };
                                    ButtonId(i, j).Text = "O";
                                }
                            }
                        }
                    }
                }
                else if (player_middle == 2)
                {
                    bool success = FinishOrBlock();
                    if (!success)
                    {
                        //double[] dist = ;[0] == 1 && dist[1] == 2
                        if (RelPos(row, col, first_move[0], first_move[1]).SequenceEqual(new double[2] { 1, 2 }))
                        {
                            if (first_move.SequenceEqual(latest_move))
                            {
                                for (byte i = 0; i < 3; i++)
                                {
                                    for (byte j = 0; j < 3; j++)
                                    {
                                        if ((first_move[0] == i || first_move[1] == j) && RelPos(row, col, i, j).SequenceEqual(new double[2] { 0, 1 }))
                                        {
                                            table[i, j] = 2;
                                            latest_move[0] = i;
                                            latest_move[1] = j;
                                            ButtonId(i, j).Text = "O";
                                            break;
                                        }
                                    }
                                }
                                CheckWin();
                                return true;
                            }
                        }
                        //find farthest corner from user
                        byte[,] corners = new byte[4, 2] { { 0, 0 }, { 0, 2 }, { 2, 0 }, { 2, 2 } };
                        double max_euclid_dist = 0;
                        byte farthest_index = 5;
                        for (byte i = 0; i < 4; i++)
                        {
                            if (table[corners[i, 0], corners[i, 1]] == 0)
                            { //same line/col
                                if (max_euclid_dist < RelPos(row, col, corners[i, 0], corners[i, 1], true)[0])
                                {
                                    max_euclid_dist = RelPos(row, col, corners[i, 0], corners[i, 1], true)[0];
                                    farthest_index = i;
                                }
                                else if (max_euclid_dist == RelPos(row, col, corners[i, 0], corners[i, 1], true)[0])
                                {
                                    if (RelPos(player_first_move[0], player_first_move[1], corners[i, 0], corners[i, 1], true)[0] >
                                        RelPos(player_first_move[0], player_first_move[1], corners[farthest_index, 0], corners[farthest_index, 1], true)[0])
                                    {
                                        farthest_index = i;
                                    }
                                    else if (RelPos(player_first_move[0], player_first_move[1], corners[i, 0], corners[i, 1], true)[0] ==
                                             RelPos(player_first_move[0], player_first_move[1], corners[farthest_index, 0], corners[farthest_index, 1], true)[0])
                                    {
                                        if (corners[i, 0] == latest_move[0] || corners[i, 1] == latest_move[1]
                                         || corners[i, 0] == first_move[0] || corners[i, 1] == first_move[1]) farthest_index = i;
                                    }
                                }
                            }
                        }
                        if (farthest_index != 5)
                        {
                            table[corners[farthest_index, 0], corners[farthest_index, 1]] = 2;
                            latest_move[0] = corners[farthest_index, 0];
                            latest_move[1] = corners[farthest_index, 1];
                            ButtonId(corners[farthest_index, 0], corners[farthest_index, 1]).Text = "O";
                        }
                    }
                }
                //strategies
                //Debug purpose 
                /*for (byte i = 0; i < 3; i++)
                {
                    for (byte j = 0; j < 3; j++)
                    {
                        Console.Write(table[i, j]);
                    }
                    Console.WriteLine();
                }*/
                CheckWin();
                return true;
            }
            return false;
        }

        private void CheckWin()
        {
            //true: Computer wins
            //return true;
            //label2.Text = "Congratulations!";
            //label2.Text = "Game over!";
            bool empty = false;
            for (byte i = 0; i < 3; i++)
            {
                for (byte j = 0; j < 3; j++)
                {
                    if (table[i, j] == 0) empty = true;
                }
            }
            if (
                new byte[] { table[0, 0], table[0, 1], table[0, 2] }.All(x => x == 1) ||
                new byte[] { table[1, 0], table[1, 1], table[1, 2] }.All(x => x == 1) ||
                new byte[] { table[2, 0], table[2, 1], table[2, 2] }.All(x => x == 1) ||
                new byte[] { table[0, 0], table[1, 0], table[2, 0] }.All(x => x == 1) ||
                new byte[] { table[0, 1], table[1, 1], table[2, 1] }.All(x => x == 1) ||
                new byte[] { table[0, 2], table[1, 2], table[2, 2] }.All(x => x == 1) ||
                new byte[] { table[0, 0], table[1, 1], table[2, 2] }.All(x => x == 1) ||
                new byte[] { table[2, 0], table[1, 1], table[0, 2] }.All(x => x == 1)
                )
            {
                MessageBox.Show("Congratulations!", "Tic Tac Toe");
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                //Reset();
            }
            else if (
              new byte[] { table[0, 0], table[0, 1], table[0, 2] }.All(x => x == 2) ||
              new byte[] { table[1, 0], table[1, 1], table[1, 2] }.All(x => x == 2) ||
              new byte[] { table[2, 0], table[2, 1], table[2, 2] }.All(x => x == 2) ||
              new byte[] { table[0, 0], table[1, 0], table[2, 0] }.All(x => x == 2) ||
              new byte[] { table[0, 1], table[1, 1], table[2, 1] }.All(x => x == 2) ||
              new byte[] { table[0, 2], table[1, 2], table[2, 2] }.All(x => x == 2) ||
              new byte[] { table[0, 0], table[1, 1], table[2, 2] }.All(x => x == 2) ||
              new byte[] { table[2, 0], table[1, 1], table[0, 2] }.All(x => x == 2)
              )
            {
                MessageBox.Show("Game Over!", "Tic Tac Toe");
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                //Reset();
            }
            else if (!empty)
            {
                MessageBox.Show("Tie!", "Tic Tac Toe");
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
            }
        }

        private bool FinishOrBlock()
        {
            byte[] index = new byte[2] { 3, 3 }; //3: nothing changed
            //Computer almost win? (2)
            //Diagonal
            if (table[0, 0] == table[1, 1] && table[0, 0] == 2 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[2, 2] == table[1, 1] && table[1, 1] == 2 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[2, 0] == table[1, 1] && table[2, 0] == 2 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 2] == table[1, 1] && table[0, 2] == 2 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 0] == table[2, 2] && table[0, 0] == 2 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[2, 0] == table[0, 2] && table[0, 2] == 2 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            //Horizontal
            if (table[0, 0] == table[0, 1] && table[0, 0] == 2 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 1] == table[0, 2] && table[0, 1] == 2 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[1, 0] == table[1, 1] && table[1, 0] == 2 && table[1, 2] == 0 && index[0] == 3) { index = new byte[2] { 1, 2 }; }
            if (table[1, 2] == table[1, 1] && table[1, 2] == 2 && table[1, 0] == 0 && index[0] == 3) { index = new byte[2] { 1, 0 }; }
            if (table[2, 0] == table[2, 1] && table[2, 0] == 2 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[2, 1] == table[2, 2] && table[2, 1] == 2 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 0] == table[0, 2] && table[0, 0] == 2 && table[0, 1] == 0 && index[0] == 3) { index = new byte[2] { 0, 1 }; }
            if (table[1, 0] == table[1, 2] && table[1, 0] == 2 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[2, 0] == table[2, 2] && table[2, 0] == 2 && table[2, 1] == 0 && index[0] == 3) { index = new byte[2] { 2, 1 }; }
            //Vertical
            if (table[0, 0] == table[1, 0] && table[0, 0] == 2 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 1] == table[1, 1] && table[0, 1] == 2 && table[2, 1] == 0 && index[0] == 3) { index = new byte[2] { 2, 1 }; }
            if (table[0, 2] == table[1, 2] && table[0, 2] == 2 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[1, 0] == table[2, 0] && table[1, 0] == 2 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[1, 1] == table[2, 1] && table[1, 1] == 2 && table[0, 1] == 0 && index[0] == 3) { index = new byte[2] { 0, 1 }; }
            if (table[1, 2] == table[2, 2] && table[1, 2] == 2 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 0] == table[2, 0] && table[0, 0] == 2 && table[1, 0] == 0 && index[0] == 3) { index = new byte[2] { 1, 0 }; }
            if (table[0, 1] == table[2, 1] && table[0, 1] == 2 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[0, 2] == table[2, 2] && table[0, 2] == 2 && table[1, 2] == 0 && index[0] == 3) { index = new byte[2] { 1, 2 }; }
            //Player? (1)
            //Diagonal
            if (table[0, 0] == table[1, 1] && table[0, 0] == 1 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[2, 2] == table[1, 1] && table[1, 1] == 1 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[2, 0] == table[1, 1] && table[2, 0] == 1 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 2] == table[1, 1] && table[0, 2] == 1 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 0] == table[2, 2] && table[0, 0] == 1 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[2, 0] == table[0, 2] && table[0, 2] == 1 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            //Horizontal
            if (table[0, 0] == table[0, 1] && table[0, 0] == 1 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 1] == table[0, 2] && table[0, 1] == 1 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[1, 0] == table[1, 1] && table[1, 0] == 1 && table[1, 2] == 0 && index[0] == 3) { index = new byte[2] { 1, 2 }; }
            if (table[1, 2] == table[1, 1] && table[1, 2] == 1 && table[1, 0] == 0 && index[0] == 3) { index = new byte[2] { 1, 0 }; }
            if (table[2, 0] == table[2, 1] && table[2, 0] == 1 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[2, 1] == table[2, 2] && table[2, 1] == 1 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 0] == table[0, 2] && table[0, 0] == 1 && table[0, 1] == 0 && index[0] == 3) { index = new byte[2] { 0, 1 }; }
            if (table[1, 0] == table[1, 2] && table[1, 0] == 1 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[2, 0] == table[2, 2] && table[2, 0] == 1 && table[2, 1] == 0 && index[0] == 3) { index = new byte[2] { 2, 1 }; }
            //Vertical
            if (table[0, 0] == table[1, 0] && table[0, 0] == 1 && table[2, 0] == 0 && index[0] == 3) { index = new byte[2] { 2, 0 }; }
            if (table[0, 1] == table[1, 1] && table[0, 1] == 1 && table[2, 1] == 0 && index[0] == 3) { index = new byte[2] { 2, 1 }; }
            if (table[0, 2] == table[1, 2] && table[0, 2] == 1 && table[2, 2] == 0 && index[0] == 3) { index = new byte[2] { 2, 2 }; }
            if (table[1, 0] == table[2, 0] && table[1, 0] == 1 && table[0, 0] == 0 && index[0] == 3) { index = new byte[2] { 0, 0 }; }
            if (table[1, 1] == table[2, 1] && table[1, 1] == 1 && table[0, 1] == 0 && index[0] == 3) { index = new byte[2] { 0, 1 }; }
            if (table[1, 2] == table[2, 2] && table[1, 2] == 1 && table[0, 2] == 0 && index[0] == 3) { index = new byte[2] { 0, 2 }; }
            if (table[0, 0] == table[2, 0] && table[0, 0] == 1 && table[1, 0] == 0 && index[0] == 3) { index = new byte[2] { 1, 0 }; }
            if (table[0, 1] == table[2, 1] && table[0, 1] == 1 && table[1, 1] == 0 && index[0] == 3) { index = new byte[2] { 1, 1 }; }
            if (table[0, 2] == table[2, 2] && table[0, 2] == 1 && table[1, 2] == 0 && index[0] == 3) { index = new byte[2] { 1, 2 }; }
            if (index[0] != 3 || index[1] != 3) //&& == 3
            {
                table[index[0], index[1]] = 2;
                ButtonId(index[0], index[1]).Text = "O";
                return true;
            }
            else return false;
        }

        public Form1()
        {
            InitializeComponent();
            Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool success = OnClick(0, 2);
            if (success)
            {
                button3.Text = "X";
                //CheckWin();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool success = OnClick(0, 0);
            if (success)
            {
                button1.Text = "X";
                //ButtonId(0, 0).Text = "X";
                //CheckWin();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool success = OnClick(0, 1);
            if (success)
            {
                button2.Text = "X";
                //CheckWin();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bool success = OnClick(1, 0);
            if (success)
            {
                button6.Text = "X";
                //CheckWin();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool success = OnClick(1, 1);
            if (success) button5.Text = "X";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool success = OnClick(1, 2);
            if (success) button4.Text = "X";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bool success = OnClick(2, 0);
            if (success) button9.Text = "X";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bool success = OnClick(2, 1);
            if (success) button8.Text = "X";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool success = OnClick(2, 2);
            if (success) button7.Text = "X";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:gg78qvf6m@mozmail.com?subject=TicTacToe_Suggestion(s)");
        }
    }
}
//nothing here
/*if (strategy == "none")
{
    switch (dist[0])
    {
        case 0:
            switch(dist[1])
            {
                case 1:
                    strategy = "next_to";
                    break;
                case 2:
                    strategy = "corner_line";
                    break;
            }
            break;
        case 1:
            switch(dist[1])
            {
                case 1:
                    strategy = "middle";
                    break;
                case 2:
                    strategy = "other";
                    break;
            }
            break;
        case 2:
            strategy = "corner_diagonal";
            break;
    }
}
switch (strategy)
{
    case "next_to":
        FinishOrBlock();
        break;
    case "corner_line":
        break;
    case "corner_diagonal":
        break;
    case "middle":
        break;
    case "other":
        break;
}
switch (turns)
{
    case 0:
        switch(dist[0])
        {
            case 0:
                if (dist[1] == 1)
                {
                    for (byte i = 0; i < 3; i++)
                    {
                        for (byte j = 0; j < 3; j++)
                        {
                            if (new int[] { 0, 2 }.SequenceEqual(RelPos(i, j, first_move[0], first_move[1]))
                                && new int[] { 1, 2 }.SequenceEqual(RelPos(i, j, row, col)))
                            {
                                table[i, j] = 2;
                                ButtonId(i, j).Text = "O";
                                break;
                            }
                        }
                    }
                } else if (dist[1] == 2)
                {
                    for (byte i = 0; i < 3; i++)
                    {
                        for (byte j = 0; j < 3; j++)
                        {
                            if (new int[] { 0, 2 }.SequenceEqual(RelPos(i, j, first_move[0], first_move[1]))
                                && new int[] { 2, 2 }.SequenceEqual(RelPos(i, j, row, col)))
                            {
                                table[i, j] = 2;
                                ButtonId(i, j).Text = "O";
                                break;
                            }
                        }
                    }
                }
                break;
            case 1:
                break;
            case 2:
                break;
        }
        break;
    case 1:

        break;
    case 2:
        break;
    case 3:
        break;
    default:
        //block
        break;
}
switch (dist[0])
{
    case 0:
        if (dist[1] == 1)
        {
            switch (turns)
            {
                case 1:
                    for (byte i = 0; i < 3; i++)
                    {
                        for (byte j = 0; j < 3; j++)
                        {
                            if (new int[] { 0, 2 }.SequenceEqual(RelPos(i, j, first_move[0], first_move[1])) 
                                && new int[] { 1, 2 }.SequenceEqual(RelPos(i, j, row, col)))
                            {
                                table[i, j] = 2;
                                ButtonId(i, j).Text = "O";
                                break;
                            }
                        }
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        } else if (dist[1] == 2)
        {
            //
        }
        break;
    case 1:
        if (dist[1] == 1)
        {
            //
        } else
        {
            //
        }
        break;
    case 2:
        break;
}*/
