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
    public partial class Analiza : Form
    {
        public Analiza()
        {
            InitializeComponent();
        }
        public void UpdateAnalize()
        {
            List<Element> allElements = Form1.elements;
            if (allElements.Count > 0)
            {
                double sumaMasa = 0;
                List<Element> filterElements = new List<Element>();
                foreach (var element in allElements)
                {
                    if (setFilter(element))
                    {
                        filterElements.Add(new Element(element.oblik, element.boja, element.masa));
                        sumaMasa += element.masa;
                    }
                    
                }

                lbSet.Text = "Broj elemenata u traženom uzorku: " + filterElements.Count;
                lbUkupnaMasa.Text = "Ukupna masa elemenata u traženom uzorku: " + sumaMasa.ToString("0.00");
                lbProsjecnaMasa.Text = "Prosječna masa elemenata u traženom uzorku: " + (sumaMasa / filterElements.Count).ToString("0.00");
                if (filterElements.Count > 0)
                {
                    int elementCount = 0;
                    foreach (var element in filterElements)
                    {
                        if (elementFilter(element)) elementCount++;
                    }
                    lbElement.Text = "Broj elemenata koji se podudaraju iz traženog uzorka: " + elementCount.ToString();

                    lbCalc.Text = "Postotak: " + ((float)(((float)elementCount/(float)filterElements.Count)*100)).ToString() + " %";
                }
                else
                {
                    lbElement.Text = "";
                    lbCalc.Text = "";
                }
            }
        }

        private bool elementFilter(Element element)
        {
            return filterElementBoja(element.boja) && filterElementOblika(element.oblik);
        }

        private bool filterSetOblika(byte oblik)
        {
            switch (oblik)
            {
                case 1:
                    return cbSetPiramida.Checked;
                case 2:
                    return cbSetKocka.Checked;
                case 4:
                    return cbSetKvadar.Checked;
                case 8:
                    return cbSetKugla.Checked;
                case 16:
                    return cbSetNepoznatoOblici.Checked;
                default:
                    return false;
            }
        }

        private bool filterSetBoja(byte boja)
        {
            switch (boja)
            {
                case 1:
                    return cbSetPlava.Checked;
                case 2:
                    return cbSetBijela.Checked;
                case 4:
                    return cbSetCrvena.Checked;
                case 8:
                    return cbSetZuta.Checked;
                case 16:
                    return cbSetNepoznatoBoje.Checked;
                default:
                    return false;
            }
        }

        private bool setFilter(Element element)
        {
            return filterSetBoja(element.boja) && filterSetOblika(element.oblik);
        }

        private bool filterElementOblika(byte oblik)
        {
            switch (oblik)
            {
                case 1:
                    return cbElementPiramida.Checked;
                case 2:
                    return cbElementKocka.Checked;
                case 4:
                    return cbElementKvadar.Checked;
                case 8:
                    return cbElementKugla.Checked;
                case 16:
                    return cbElementNepoznatoOblici.Checked;
                default:
                    return false;
            }
        }

        private bool filterElementBoja(byte boja)
        {
            switch (boja)
            {
                case 1:
                    return cbElementPlava.Checked;
                case 2:
                    return cbElementBijela.Checked;
                case 4:
                    return cbElementCrvena.Checked;
                case 8:
                    return cbElementZuta.Checked;
                case 16:
                    return cbElementNepoznatoBoje.Checked;
                default:
                    return false;
            }
        }

        #region cbChanged
        private void cbSetPiramida_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }
        
        private void cbSetKocka_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetKvadar_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetKugla_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetNepoznatoOblici_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetPlava_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetBijela_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetCrvena_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetZuta_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbSetNepoznatoBoje_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementPiramida_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementKocka_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementKvadar_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementKugla_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementNepoznatoOblici_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementPlava_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementBijela_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementCrvena_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementZuta_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }

        private void cbElementNepoznatoBoje_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAnalize();
        }
        #endregion

        private void btnSetShapeAll_Click(object sender, EventArgs e)
        {
            if (cbSetPiramida.Checked &&
               cbSetKocka.Checked &&
               cbSetKvadar.Checked &&
               cbSetKugla.Checked &&
               cbSetNepoznatoOblici.Checked)
            {
                cbSetPiramida.Checked = false;
                cbSetKocka.Checked = false;
                cbSetKvadar.Checked = false;
                cbSetKugla.Checked = false;
                cbSetNepoznatoOblici.Checked = false;
            }
            else
            {
                cbSetPiramida.Checked = true;
                cbSetKocka.Checked = true;
                cbSetKvadar.Checked = true;
                cbSetKugla.Checked = true;
                cbSetNepoznatoOblici.Checked = true;
            }
        }

        private void btnSetColorAll_Click(object sender, EventArgs e)
        {
            if (cbSetPlava.Checked &&
               cbSetBijela.Checked &&
               cbSetCrvena.Checked &&
               cbSetZuta.Checked &&
               cbSetNepoznatoBoje.Checked)
            {
                cbSetPlava.Checked = false;
                cbSetBijela.Checked = false;
                cbSetCrvena.Checked = false;
                cbSetZuta.Checked = false;
                cbSetNepoznatoBoje.Checked = false;
            }
            else
            {
                cbSetPlava.Checked = true;
                cbSetBijela.Checked = true;
                cbSetCrvena.Checked = true;
                cbSetZuta.Checked = true;
                cbSetNepoznatoBoje.Checked = true;
            }
        }

        private void btnElementShapeAll_Click(object sender, EventArgs e)
        {
            if (cbElementPiramida.Checked &&
               cbElementKocka.Checked &&
               cbElementKvadar.Checked &&
               cbElementKugla.Checked &&
               cbElementNepoznatoOblici.Checked)
            {
                cbElementPiramida.Checked = false;
                cbElementKocka.Checked = false;
                cbElementKvadar.Checked = false;
                cbElementKugla.Checked = false;
                cbElementNepoznatoOblici.Checked = false;
            }
            else
            {
                cbElementPiramida.Checked = true;
                cbElementKocka.Checked = true;
                cbElementKvadar.Checked = true;
                cbElementKugla.Checked = true;
                cbElementNepoznatoOblici.Checked = true;
            }
        }

        private void btnElementColorAll_Click(object sender, EventArgs e)
        {
            if (cbElementPlava.Checked &&
               cbElementBijela.Checked &&
               cbElementCrvena.Checked &&
               cbElementZuta.Checked &&
               cbElementNepoznatoBoje.Checked)
            {
                cbElementPlava.Checked = false;
                cbElementBijela.Checked = false;
                cbElementCrvena.Checked = false;
                cbElementZuta.Checked = false;
                cbElementNepoznatoBoje.Checked = false;
            }
            else
            {
                cbElementPlava.Checked = true;
                cbElementBijela.Checked = true;
                cbElementCrvena.Checked = true;
                cbElementZuta.Checked = true;
                cbElementNepoznatoBoje.Checked = true;
            }
        }

        private void Analiza_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Jeste li sigurni da želite zatvoriti prozor analize?", "Upozorenje", MessageBoxButtons.OKCancel);
            e.Cancel = true;
            if (res == DialogResult.OK)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    Hide();
                }
            }
        }
    }
}
