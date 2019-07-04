using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace UgRS_app
{
    public partial class Form1 : Form
    {
        #region konstante
        const string BrojCasa = "6";
        const int brCasa = 6;
        const int buffSizeSl = 15;
        const int buffSizePr = 4;
        #endregion

        #region enumi
        enum Shapes {Piramida = 1, Kocka = 2, Kvadar = 4, Kugla = 8, Nepoznato = 16};
        enum Colors {Plava = 1, Bijela = 2, Crvena = 4, Žuta = 8, Nepoznato = 16 };
        #endregion

        #region globalne varijable
        internal static string oblik = "", boja = "", masaMin = "", masaMax = "";
        static bool novi_podatak=false;

        internal static List<Element> elements = new List<Element>();
        Analiza analiza = new Analiza();

        Filter[] filteriSlanje = new Filter[brCasa - 1];   //5*24 bita = 120 bit = 15 B; 6. casa je poznata u stm32-zapisano (nerazvrstano-neprepoznato)
        byte[] bufferSlanje = new byte[buffSizeSl];
        byte[] bufferPrimanje = new byte[buffSizePr];

        int brElemenata = 0;
        #endregion

        #region globalne varijable UART
        private SerialPort myPort = new SerialPort();
        private string[] ports;
        private string comPort = "";
        private ushort[] dataBuffer = new ushort[8];
        #endregion

        public Form1()
        {
            InitializeComponent();

            #region dataGrid postavke
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            #endregion
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {            
            if (!(dataGridView1["Rbr", dataGridView1.Rows.Count - 1].Value.ToString() == BrojCasa))
            {
                oblik = "";
                boja = "";
                masaMin = "0";
                masaMax = "127";

                Dodaj form = new Dodaj();
                form.ShowDialog();
                form.Dispose();
                if (novi_podatak)
                {
                    int last = int.Parse(dataGridView1["Rbr", dataGridView1.Rows.Count - 1].Value.ToString());
                    dataGridView1["Rbr", last-1].Value = last + 1;
                    dataGridView1.Rows.Insert(last-1, last.ToString(), oblik, boja, masaMin, masaMax, "Uredi", "Ukloni");
                    novi_podatak = false;
                }
            }
            else
            {
                MessageBox.Show("Nije moguće dodati više boxova");
            }
        }

        internal static void spremiVrijednosti(string oblici, string boje, string masaOd, string masaDo, bool novi)
        {
            oblik = oblici;
            boja = boje;
            masaMin = masaOd;
            masaMax = masaDo;
            novi_podatak = novi;
        }

        #region UART

        private void punjenjeBuffera()
        {
            int i, j;
            if (rbSort.Checked)
            {               
                for (i = 0; i < brCasa - 1; i++)
                {
                    filteriSlanje[i] = new Filter(0, 0, 0, 0);
                }
                i = 0;
                foreach (var row in dataGridView1.Rows)
                {
                    oblik = dataGridView1[1, i].Value.ToString();
                    boja = dataGridView1[2, i].Value.ToString();
                    masaMin = dataGridView1[3, i].Value.ToString();
                    masaMax = dataGridView1[4, i].Value.ToString();
                    if (oblik == "Sve")
                    {
                        break;
                    }
                    if (oblik.Contains(Enum.GetName(typeof(Shapes), 1))) filteriSlanje[i].oblici += 1;
                    if (oblik.Contains(Enum.GetName(typeof(Shapes), 2))) filteriSlanje[i].oblici += 2;
                    if (oblik.Contains(Enum.GetName(typeof(Shapes), 4))) filteriSlanje[i].oblici += 4;
                    if (oblik.Contains(Enum.GetName(typeof(Shapes), 8))) filteriSlanje[i].oblici += 8;
                    if (oblik.Contains(Enum.GetName(typeof(Shapes), 16))) filteriSlanje[i].oblici += 16;

                    if (boja.Contains(Enum.GetName(typeof(Colors), 1))) filteriSlanje[i].boje += 1;
                    if (boja.Contains(Enum.GetName(typeof(Colors), 2))) filteriSlanje[i].boje += 2;
                    if (boja.Contains(Enum.GetName(typeof(Colors), 4))) filteriSlanje[i].boje += 4;
                    if (boja.Contains(Enum.GetName(typeof(Colors), 8))) filteriSlanje[i].boje += 8;
                    if (boja.Contains(Enum.GetName(typeof(Colors), 16))) filteriSlanje[i].boje += 16;

                    filteriSlanje[i].masaMin = byte.Parse(masaMin);
                    filteriSlanje[i].masaMax = byte.Parse(masaMax);
                    i++;
                }
                for (i = 0; i < buffSizeSl; i++)
                {
                    bufferSlanje[i] = 0;
                }

                for (i = 0, j = 0; i < brCasa - 1; i++, j += 3)
                {
                    bufferSlanje[j] |= Convert.ToByte(filteriSlanje[i].oblici & maskaF(5));
                    bufferSlanje[j] |= Convert.ToByte(Convert.ToByte(filteriSlanje[i].boje & maskaF(3)) << 5);
                    bufferSlanje[j + 1] |= Convert.ToByte(Convert.ToByte(filteriSlanje[i].boje & 0x18) >> 3);
                    bufferSlanje[j + 1] |= Convert.ToByte(Convert.ToByte(filteriSlanje[i].masaMin & maskaF(6)) << 2);
                    bufferSlanje[j + 2] |= Convert.ToByte(Convert.ToByte(filteriSlanje[i].masaMin & 0x40) >> 6);
                    bufferSlanje[j + 2] |= Convert.ToByte(Convert.ToByte(filteriSlanje[i].masaMax & maskaF(7)) << 1);
                }
            }
            else
            {
                for (i = 0; i < buffSizeSl; i++)
                {
                    bufferSlanje[i] = 0xFF;
                }
            }
            
        }

        private byte maskaF(int brBit)
        {
            int i;
            byte maska = 0;
            for (i = 0; i < brBit; i++)
            {
                maska <<= 1;
                maska |= 1;
            }
            return maska;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pretrazi(); // Pretražuje dostupne COM portove
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (myPort.IsOpen)
            {
                myPort.Close();
            }
            else
            {
                try
                {
                    myPort.PortName = comPort;
                    myPort.Open();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ne postoji niti jedan spojen Com Port");
                }
            }
            if (myPort.IsOpen)
            {
                Console.WriteLine(myPort.PortName);
                comboBox1.Enabled = false;
                button1.Enabled = false;
                button2.Text = "Odspoji";
                btnStart.Enabled = true;
                rbSort.Enabled = true;
                rbAnalize.Enabled = true;
                btnStop.Enabled = true;
                lbAnaliza.Text = "";
            }
            else
            {
                comboBox1.Enabled = true;
                button1.Enabled = true;
                button2.Text = "Spoji";
                btnStart.Enabled = false;
                rbSort.Enabled = false;
                rbAnalize.Enabled = false;
                btnStop.Enabled = false;
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (myPort.IsOpen)
            {
                punjenjeBuffera();
                analiza.UpdateAnalize();
                if (rbAnalize.Checked)
                {
                    analiza.Show();
                    elements.Clear();
                }
                myPort.Write(bufferSlanje, 0, buffSizeSl);
                lbAnaliza.Text = "";
                btnStart.Enabled = false;
                rbSort.Enabled = false;
                rbAnalize.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                comPort = comboBox1.SelectedItem.ToString();
            }
        }

        private void Pretrazi()
        {
            ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
        }
        
        private void myPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            myPort.Read(bufferPrimanje, 0, buffSizePr);
            this.BeginInvoke(new LineReceivedEvent(LineReceived));
        }
        private delegate void LineReceivedEvent();
        private void LineReceived()
        {
            if (bufferPrimanje[0]==255)
            {
                MessageBox.Show("Ukupno je bilo: " + brElemenata + " elemenata");
                lbAnaliza.Text += "\n\nUkupno je bilo: " + brElemenata + " elemenata";
                brElemenata = 0;
                btnStart.Enabled = true;
                rbSort.Enabled = true;
                rbAnalize.Enabled = true;
            }
            else if (bufferPrimanje[0] != 0)
            {
                lbAnaliza.Text += "Primljeni element br: " + bufferPrimanje[0] + "., masa: " + bufferPrimanje[3] + ", oblik: " + Enum.GetName(typeof(Shapes), bufferPrimanje[1]) + ", boja: " + Enum.GetName(typeof(Colors), bufferPrimanje[2]) + "\n";
                elements.Add(new Element(bufferPrimanje[1], bufferPrimanje[2], bufferPrimanje[3]));
                brElemenata = bufferPrimanje[0];
                analiza.UpdateAnalize();
            }
        }       

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                EventArgs eventa = new EventArgs();
                button3_Click(sender, eventa);
            }
        }
        #endregion
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (myPort.IsOpen)
            {
                int i;
                for (i = 0; i < buffSizeSl; i++)
                {
                    bufferSlanje[i] = 0;
                }
                myPort.Write(bufferSlanje, 0, buffSizeSl);
                btnStart.Enabled = true;
                rbSort.Enabled = true;
                rbAnalize.Enabled = true;
            }
        }

        private void btn_statistika_Click(object sender, EventArgs e)
        {
            analiza.Show();
            analiza.UpdateAnalize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add("1", "Sve", "Sve", "0", "127", "Uredi", "Ukloni");
            #region UART
            // Pretraživanje COM portova
            Pretrazi();

            // postavke serijske komunikacije
            myPort.DtrEnable = true;
            myPort.RtsEnable = true;
            myPort.BaudRate = 115200;
            myPort.Parity = Parity.None;
            myPort.DataBits = 8;
            myPort.StopBits = StopBits.One;
            //myPort.PortName = comPort;
            myPort.DataReceived += myPort_DataReceived;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            rbSort.Enabled = false;
            rbAnalize.Enabled = false;
            #endregion
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                int last = int.Parse(dataGridView1["Rbr", dataGridView1.Rows.Count - 1].Value.ToString());
                if (e.RowIndex == last-1)   //zadnji
                {
                    MessageBox.Show("Nije moguće uređivati zadnji red");
                }   //zadnji
                else if (e.ColumnIndex==5)  //uredi
                {
                    oblik = dataGridView1[1, e.RowIndex].Value.ToString();
                    boja = dataGridView1[2, e.RowIndex].Value.ToString();
                    masaMin = dataGridView1[3, e.RowIndex].Value.ToString();
                    masaMax = dataGridView1[4, e.RowIndex].Value.ToString();

                    Dodaj form = new Dodaj();
                    form.ShowDialog();
                    form.Dispose();
                    if (novi_podatak)
                    {
                        dataGridView1.Rows.RemoveAt(e.RowIndex);
                        dataGridView1.Rows.Insert(e.RowIndex, e.RowIndex+1, oblik, boja, masaMin, masaMax, "Uredi", "Ukloni");
                        novi_podatak = false;
                    }
                }   //uredi
                else if (e.ColumnIndex == 6)    //ukloni
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    correctRedniBroj();
                }   //ukloni
            }
        }

        private void correctRedniBroj()
        {
            int i = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Rbr"].Value = i.ToString();
                i++;
            }
        }
    }
}
