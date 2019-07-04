using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UgRS_app
{
    public partial class Dodaj : Form
    {
        string oblici = "", boje = "", masaOd = "", masaDo = "";
        public Dodaj()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const int maxMDo = 127;
            const int minMOd = 0;
            int mOd, mDo;
            if (tbMasaDo.Text == "")
            {
                tbMasaDo.Text = maxMDo.ToString();
            }
            if (tbMasaOd.Text == "")
            {
                tbMasaOd.Text = minMOd.ToString();
            }
            if (int.TryParse(tbMasaOd.Text, out mOd) && int.TryParse(tbMasaDo.Text, out mDo))
            {
                if (mDo <= mOd)
                {
                    MessageBox.Show("Minimalna vrijednost mase mora biti manja od maksimalne vrijednosti.");
                }
                else if (mDo > maxMDo || mDo < minMOd)
                {
                    MessageBox.Show("Masa do " + maxMDo.ToString());
                    tbMasaDo.Text = maxMDo.ToString();
                }
                else if (mOd < minMOd || mOd > maxMDo)
                {
                    MessageBox.Show("Masa od " + minMOd.ToString());
                    tbMasaOd.Text = minMOd.ToString();
                }
                else if (mOd==minMOd && mDo==maxMDo && 
                    checkBox1.Checked &&
                    checkBox2.Checked &&
                    checkBox3.Checked &&
                    checkBox4.Checked &&
                    checkBox5.Checked &&
                    checkBox6.Checked &&
                    checkBox7.Checked &&
                    checkBox8.Checked &&
                    checkBox9.Checked &&
                    checkBox10.Checked)
                {
                    MessageBox.Show("Ovo je uvjet nesortiranih elemenata");
                }
                else if (
                    (!checkBox1.Checked &&
                    !checkBox2.Checked &&
                    !checkBox3.Checked &&
                    !checkBox4.Checked &&
                    !checkBox5.Checked) ||
                    (!checkBox6.Checked &&
                    !checkBox7.Checked &&
                    !checkBox8.Checked &&
                    !checkBox9.Checked &&
                    !checkBox10.Checked))
                {
                    MessageBox.Show("Mora biti odabrana najmanje jedna boja i oblik");
                }
                else
                {

                    if(checkBox1.Checked) oblici += checkBox1.Text + ", ";
                    if(checkBox2.Checked) oblici += checkBox2.Text + ", ";
                    if(checkBox3.Checked) oblici += checkBox3.Text + ", ";
                    if(checkBox4.Checked) oblici += checkBox4.Text + ", ";
                    if(checkBox5.Checked) oblici += checkBox5.Text + ", ";
                    if(checkBox6.Checked) boje += checkBox6.Text + ", ";
                    if(checkBox7.Checked) boje += checkBox7.Text + ", ";
                    if(checkBox8.Checked) boje += checkBox8.Text + ", ";
                    if(checkBox9.Checked) boje += checkBox9.Text + ", ";
                    if(checkBox10.Checked) boje += checkBox10.Text + ", ";

                    oblici = oblici.Remove(oblici.Length - 2);
                    boje = boje.Remove(boje.Length - 2);

                    masaOd = tbMasaOd.Text;
                    masaDo = tbMasaDo.Text;

                    Form1.spremiVrijednosti(oblici, boje, masaOd, masaDo, true);
                    this.Close();
                }
            }
            else
            {
                tbMasaDo.Text = maxMDo.ToString();
                tbMasaOd.Text = minMOd.ToString();
                MessageBox.Show("Moraju biti unešeni brojevi");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked &&
               checkBox2.Checked &&
               checkBox3.Checked &&
               checkBox4.Checked &&
               checkBox5.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
                checkBox2.Checked = true;
                checkBox3.Checked = true;
                checkBox4.Checked = true;
                checkBox5.Checked = true;
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox6.Checked &&
                checkBox7.Checked &&
                checkBox8.Checked &&
                checkBox9.Checked &&
                checkBox10.Checked)
            {
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
                checkBox10.Checked = false;
            }
            else
            {
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox8.Checked = true;
                checkBox9.Checked = true;
                checkBox10.Checked = true;
            }
        }

        private void Dodaj_Load(object sender, EventArgs e)
        {
            oblici = Form1.oblik;
            boje = Form1.boja;
            masaOd = Form1.masaMin;
            masaDo = Form1.masaMax;

            if (oblici.Contains(checkBox1.Text)) checkBox1.Checked = true;
            if (oblici.Contains(checkBox2.Text)) checkBox2.Checked = true;
            if (oblici.Contains(checkBox3.Text)) checkBox3.Checked = true;
            if (oblici.Contains(checkBox4.Text)) checkBox4.Checked = true;
            if (oblici.Contains(checkBox5.Text)) checkBox5.Checked = true;

            if (boje.Contains(checkBox6.Text)) checkBox6.Checked = true;
            if (boje.Contains(checkBox7.Text)) checkBox7.Checked = true;
            if (boje.Contains(checkBox8.Text)) checkBox8.Checked = true;
            if (boje.Contains(checkBox9.Text)) checkBox9.Checked = true;
            if (boje.Contains(checkBox10.Text)) checkBox10.Checked = true;

            tbMasaOd.Text = masaOd;
            tbMasaDo.Text = masaDo;

            if(oblici == "" && boje == "")
            {
                button1.Text = "Dodaj";
            }
            else
            {
                button1.Text = "Uredi";
            }

            oblici = "";
            boje = "";
            masaOd = "";
            masaDo = "";

        }
    }
}
