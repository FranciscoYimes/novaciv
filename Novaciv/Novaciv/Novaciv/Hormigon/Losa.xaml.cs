using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Novaciv.Hormigon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Losa : ContentPage
    {
        public Losa()
        {
            InitializeComponent();
        }

        private void Aliv_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Altura del alivianamiento
            var item = Aliv.ItemsSource[Aliv.SelectedIndex] as string;
            int altb1 = 10;
            int altb2 = 15;
            int altb3 = 20;
            if (item == "40x20x10 (cm)")
            {
                hb.Text = altb1.ToString();

            }
            else if (item == "40x20x15 (cm)")
            {
                hb.Text = altb2.ToString();
            }
            else if (item == "40x20x20 (cm)")
            {
                hb.Text = altb3.ToString();
            }

            if (!string.IsNullOrEmpty(Lns.Text) &&
                !string.IsNullOrEmpty(Lnc.Text) &&
                !string.IsNullOrEmpty(Bn.Text) &&
                !string.IsNullOrEmpty(Tc.Text))
            {
                //altura de loseta y altura de losa alivianda
                double loseta = double.Parse(Tc.Text);
                var hbloq = double.Parse(hb.Text);
                double ht;
                ht = loseta + hbloq;
                h.Text = ht.ToString("F2");

                // longitud de analisis
                double bbloq = double.Parse(bb.Text);
                var bnerv = double.Parse(Bn.Text);
                double lmax = 2 * bbloq + 2 * bnerv;
                lcalc.Text = lmax.ToString();
                //longitud critica 
                var lonl = double.Parse(Lns.Text);
                var longc = double.Parse(Lnc.Text);

                // inercia y altura de losa maciza
                double hmin = Math.Round(lonl * 0.03 * 100, 2);
                hl.Text = hmin.ToString("F2");
                double Imaz = lmax * Math.Pow(hmin, 3) / 12;
                inerl.Text = Imaz.ToString("F2");

                // inercia de losa alivianada
                //area de nervio
                double hnerv = double.Parse(hb.Text);
                double A1 = hnerv * bnerv;

                // centro de gravedad del nervio
                double y1 = hnerv / 2;
                //inercia propia nervio
                double Inerv = bnerv * Math.Pow(hnerv, 3) / 12;

                //area de la loseta 
                double A2 = loseta * lmax;
                double y2 = loseta / 2 + hnerv;
                // inercia propia loseta
                double Iloseta = lmax * Math.Pow(loseta, 3) / 12;

                //suumatoria de areas
                double EA = A1 + A1 + A2;
                // Sumatoria de Areas * Centroides
                double EAy = A1 * y1 + A1 * y1 + A2 * y2;
                // cendoride de losa alivianada
                double yr = Math.Round(EAy / EA, 2);

                // distancias al cuadrado 
                double dnerv = Math.Pow(y1 - yr, 2);
                double dloseta = Math.Pow(y2 - yr, 2);

                //inercia de losa alivianada
                double Ialiv = Math.Round(2 * (Inerv + A1 * dnerv) + Iloseta + A2 * dloseta, 2);
                ila.Text = Ialiv.ToString();

                //Altura equivalente
                double hq = Math.Round(Math.Pow(12 * Ialiv / lmax, 0.3333), 2);
                heq.Text = hq.ToString();
                // chequeos de diseño 
                // chequeo longitudes de losa
                if (lonl < longc)
                {
                    // chequeo longitudes de losa
                    DisplayAlert("Datos No Ingresados", "Ls debe ser mayor que Lc", "OK");

                }
                //chequeo de dimensiones del nervio
                if (bnerv < 10)
                {

                    chknerv.Text = string.Concat("Dimenciones del Nervio No Cumple, bn >= 10cm");
                    chknerv.TextColor = Color.Red;
                }
                else
                {
                    chknerv.Text = "Dimensiones del Nervio Cumple";
                    chknerv.TextColor = Color.Green;
                }
                //chequeo de loseta
                double maxval = Math.Max(1 / 12 * bbloq, 5);
                if (maxval > loseta)
                {
                    chklos.Text = string.Concat("Dimension de loseta No cumple ", "Tc >= ", maxval.ToString());
                    chklos.TextColor = Color.Red;
                }
                else
                {
                    chklos.Text = "Dimension de loseta Cumple";
                    chklos.TextColor = Color.Green;
                }
                // chequeo de inercias
                if (Imaz > Ialiv)
                {
                    chkiner.Text = "inercia No Cumple, Aumentar haliv y/o bn y/o loseta";
                    chkiner.TextColor = Color.Red;
                }
                else
                {
                    // chequeo de inercias

                    chkiner.Text = "inercia Cumple";
                    chkiner.TextColor = Color.Green;
                }


                //peso de losa
                double Vlosa = ht * lmax * lmax / 1000000;
                double Vbloq = 0.2 * bbloq / 100 * hbloq / 100 * 8;
                double Vhormigon = Vlosa - Vbloq;
                double Whormigon = Vhormigon * 2.4;
                double Wbloq = Vbloq * 0.75;
                double wlosa = 0.08 + Wbloq + Whormigon;
                Wlosa.Text = wlosa.ToString("F2");


            }
            else
            {
                DisplayAlert("Datos No Ingresados", "llenar información requerida para el calculo", "OK");
            }

            Mampost_SelectedIndexChanged(sender, e);
            if (Mampost.SelectedIndex == 4 &&
                !string.IsNullOrEmpty(Wlosa.Text))
            {
                WMampost_Completed(sender, e);
            }
        }

        private void Bn_Completed(object sender, EventArgs e)
        {
            Aliv_SelectedIndexChanged(sender, e);
        }

        private void Tc_Completed(object sender, EventArgs e)
        {
            Aliv_SelectedIndexChanged(sender, e);
        }

        private void Mampost_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Mampost.SelectedIndex == 0)
            { WMampost.Text = 0.20.ToString("F2"); }
            if (Mampost.SelectedIndex == 1)
            { WMampost.Text = 0.18.ToString("F2"); }
            if (Mampost.SelectedIndex == 2)
            { WMampost.Text = 0.10.ToString("F2"); }
            if (Mampost.SelectedIndex == 3)
            { WMampost.Text = 0.00.ToString("F2"); }
            if (Mampost.SelectedIndex == 4)
            { WMampost.IsEnabled = true; }

            if (Mampost.SelectedIndex !=-1 &&
                Mampost.SelectedIndex !=4 &&
                !string.IsNullOrEmpty(Wlosa.Text))
            {
                double wmampost = double.Parse(WMampost.Text);
                double wlosa = double.Parse(Wlosa.Text);
                double wtotal = wmampost + wlosa;
                Wtotal.Text = wtotal.ToString("F2");
            }

        }

        private void WMampost_Completed(object sender, EventArgs e)
        {
            double wmampost = double.Parse(WMampost.Text);
            double wlosa = double.Parse(Wlosa.Text);
            double wtotal = wmampost + wlosa;
            Wtotal.Text = wtotal.ToString("F2");
        }
    }
}