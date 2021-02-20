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
    public partial class Viga : ContentPage
    {
        public Viga()
        {
            InitializeComponent();
        }

        //select Maximun value L1 and L2
        public void L1_Completed(object sender, EventArgs e)
        {
            //don´t know why do not recognize dot as 0.
            string cond = L1.Text;
            if (cond == ".")
            {
                L1.Text.Replace(".", "0.");
            }
            else
            {
                if (!string.IsNullOrEmpty(L1.Text))
                {
                    double ll1 = Convert.ToDouble(L1.Text);
                    L1.Text = string.Format("{0:F2}", ll1);
                    Bcol_Completed(sender, e);
                }
            }

        }
        // calculated Lv 
        private void Bcol_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(L1.Text) &&
                !string.IsNullOrEmpty(Bcol.Text))
            {
                double l1 = double.Parse(L1.Text);
                double valcol = double.Parse(Bcol.Text);
                Bcol.Text = string.Format("{0:F2}", valcol);
                lv.Text = string.Format("{0:F2}", l1 - valcol);
                if (valcol < 0.3)
                {
                    DisplayAlert("Recomendación", "La normativa recomienda una dimension minima de columna de 0.30cm", "Ok");
                }
            }
        }

        //evaluar si numero de pisos es entero
        private void Np_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Np.Text))
            {
                double a;
                a = double.Parse(Np.Text);

                int y = Convert.ToInt32(a);

                double b = a - y;

                if (b != 0)
                {
                    DisplayAlert("Atención", "Ingrese un Valores Entero", "Aceptar");
                }
            }
        }
        //switch Evalua si las dimensiones de la columna seran calculadas o ingresadas
        private void Col_OnChanged(object sender, ToggledEventArgs e)
        {
            if (string.IsNullOrEmpty(Np.Text))
            {
                Col.On = false;
                DisplayAlert("Atención", "Para proceder con el calculo, Ingrese Número de pisos", "Aceptar");

            }
            else
            {
                if (Col.On)
                {
                    if (!string.IsNullOrEmpty(L1.Text))
                    {
                        Bcol.Label = "Ancho Aproximado bcol (m)";
                        double valcol;
                        double l1 = double.Parse(L1.Text);
                        double npisos = double.Parse(Np.Text);
                        valcol = Math.Round(0.095 * npisos, 2);
                        if (valcol < 0.30)
                        {
                            Bcol.Text = 0.30.ToString();
                            lv.Text = string.Format("{0:F2}", l1 - valcol);
                        }
                        else
                        {
                            Bcol.Text = string.Empty;
                            Bcol.Text = string.Format("{0:F2}", valcol);
                            lv.Text = string.Format("{0:F2}", l1 - valcol);
                            Bcol.IsEnabled = false;
                        }
                    }
                    else
                    {
                        DisplayAlert("Atención", "Ingresar datos de longitudes", "Aceptar");
                        Col.On = false;
                    }
                }
                else
                {
                    Bcol.Label = "Ancho de columna bcol (m)";
                    Bcol.Text = string.Empty;
                    lv.Text = string.Empty;
                    Bcol.Placeholder = "Ingrese Ancho de columna";
                    Bcol.IsEnabled = true;
                }
            }
        }

        //calculated Material and percentage of steel
        private void Fy_Completed(object sender, EventArgs e)
        {
            Sism_SelectedIndexChanged(sender, e);
        }
        private void Fc_Completed(object sender, EventArgs e)
        {
            Sism_SelectedIndexChanged(sender, e);
        }
        //calculated the maximun percentage of steel
        private void Sism_SelectedIndexChanged(object sender, EventArgs e)
        {
            pb.Text = string.Empty;
            pm.Text = string.Empty;
            if (!string.IsNullOrEmpty(Fc.Text) &&
                !string.IsNullOrEmpty(Fy.Text) &&
                Sism.SelectedIndex != -1)
            {
                var item = Sism.ItemsSource[Sism.SelectedIndex] as string;

                if (item == "Alta (Costa - Sierra)")
                {
                    double fc = double.Parse(Fc.Text.Replace(".", "0."));
                    double fy = double.Parse(Fy.Text.Replace(".", "0."));
                    double cuantiab = Math.Round(0.85 * 0.85 * fc / fy * (6100 / (6100 + fy)) * 100, 2);
                    pb.Text = cuantiab.ToString();
                    double pmAlta = Math.Round(0.5 * cuantiab, 2);
                    pm.Text = pmAlta.ToString();
                }
                else if (item == "Intermedia (Oriente)")
                {
                    double fc = double.Parse(Fc.Text.Replace(".", "0."));
                    double fy = double.Parse(Fy.Text.Replace(".", "0."));
                    double cuantiab = Math.Round(0.85 * 0.85 * fc / fy * (6100 / (6100 + fy)) * 100, 2);
                    pb.Text = cuantiab.ToString();
                    double pmIntermedia = Math.Round(0.75 * cuantiab, 2);
                    pm.Text = pmIntermedia.ToString();
                }
            }
            else
            {
                //step 1: re-select index to -1
                Sism.SelectedIndex = -1;
                //step 2: clear all items
                //Sism.ItemsSource.Clear();
            }

        }
        //selection of the dead load
        private void SelectCM_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = SelectCM.ItemsSource[SelectCM.SelectedIndex] as string;

            if (item == "Losa con Mamposteria liviana")
            {
                CM.IsEnabled = false;
                CM.Text = string.Format("{0:F2}", 0.57);
            }
            else if (item == "Losa con Mamposteria de bloque")
            {
                CM.IsEnabled = false;
                CM.Text = string.Format("{0:F2}", 0.65);
            }
            else if (item == "Losa con mamposteria de ladrillo")
            {
                CM.IsEnabled = false;
                CM.Text = string.Format("{0:F2}", 0.67);
            }
            else if (item == "Losa sin mamposteria")
            {
                CM.IsEnabled = false;
                CM.Text = string.Format("{0:F2}", 0.55);
            }
            else if (item == "Ingresar")
            {
                CM.Placeholder = "Ingrese CM";
                CM.Text = string.Empty;
                CM.IsEnabled = true;
            }
            if (!string.IsNullOrEmpty(CV.Text))
            {
                CV_Completed(sender, e);
            }
        }

        //selection of the live load
        private void SelectCV_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = SelectCV.ItemsSource[SelectCV.SelectedIndex] as string;

            if (item == "Vivienda")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.20);
            }
            else if (item == "Almacenes")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.48);
            }
            else if (item == "Bibliotecas")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.72);
            }
            else if (item == "Oficinas")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.25);
            }
            else if (item == "Graderios")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.48);
            }
            else if (item == "Aulas")
            {
                CV.IsEnabled = false;
                CV.Text = string.Format("{0:F2}", 0.20);
            }
            else if (item == "Ingresar")
            {
                CV.Text = string.Empty;
                CV.Placeholder = "Ingrese CV";
                CV.IsEnabled = true;
            }
            if (!string.IsNullOrEmpty(CM.Text))
            {
                CV_Completed(sender, e);
            }

        }

        //calculated the factored load
        private void CV_Completed(object sender, EventArgs e)
        {
            CU.Text = string.Empty;
            if (!string.IsNullOrEmpty(CM.Text) &&
                !string.IsNullOrEmpty(CV.Text))
            {
                double cm = Convert.ToDouble(CM.Text.Replace(".", "0."));
                double cv = Convert.ToDouble(CV.Text.Replace(".", "0."));
                double cargaU = Math.Round((1.2 * cm) + (1.6 * cv), 2);
                CU.Text = string.Format("{0:F2}", cargaU);
                CM.Text = string.Format("{0:F2}", cm);
                CV.Text = string.Format("{0:F2}", cv);
            }
        }

        private void CM_Completed(object sender, EventArgs e)
        {
            CV_Completed(sender, e);
        }

        private void Hv_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Bv.Text))
            {
                Beamh.IsToggled = false;
                Beamh.IsToggled = true;
            }
            if (!string.IsNullOrEmpty(Dv.Text))
            {
                Ascom_SelectedIndexChanged(sender, e);
            }
        }
        private void Rec_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Md.Text))
            { Bv_Completed(sender, e); }
        }
        private void Bv_Completed(object sender, EventArgs e)
        {
            //si existe valor de md, fc, rec entonces calcula una altura 
            //cuando se cambie el valor del ancho de viga, calcula una altura
            // y nuevamente ejecuta el switch de calcular altura para 
            //ajustar el valor del momento incluyendo el peso de la  viga
            if (!string.IsNullOrEmpty(Rec.Text) &&
                !string.IsNullOrEmpty(Fc.Text) &&
                !string.IsNullOrEmpty(Md.Text))
            {
                double md = double.Parse(Md.Text);
                double b = double.Parse(Bv.Text);
                double rec = double.Parse(Rec.Text);
                double fc = double.Parse(Fc.Text);
                Hv.Text = Math.Round(Math.Sqrt(md * 100000 / 0.145 / fc / b) + rec, 0).ToString();
                Beamh.IsToggled = false;
                Beamh.IsToggled = true;

            }
            if (!string.IsNullOrEmpty(Dv.Text))
            {
                Ascom_SelectedIndexChanged(sender, e);
            }
        }

        private void Beamh_Toggled(object sender, ToggledEventArgs e)
        {
            if (Beamh.IsToggled == true)
            {
                if (!string.IsNullOrEmpty(Lt1.Text) &&
                    !string.IsNullOrEmpty(Lt2.Text) &&
                    !string.IsNullOrEmpty(lv.Text) &&
                    !string.IsNullOrEmpty(CU.Text) &&
                    !string.IsNullOrEmpty(Bv.Text) &&
                    !string.IsNullOrEmpty(Fc.Text) &&
                    !string.IsNullOrEmpty(Rec.Text) &&
                    Sism.SelectedIndex != -1)
                {
                    if (Sism.SelectedIndex == 0)
                    { Fspp.Text = 1.20.ToString(); }
                    else if (Sism.SelectedIndex == 1)
                    { Fspp.Text = 1.15.ToString(); }
                    double f = double.Parse(Fspp.Text);
                    double b = double.Parse(Bv.Text);
                    double lt1 = double.Parse(Lt1.Text);
                    double lt2 = double.Parse(Lt2.Text);
                    double lvmax = double.Parse(lv.Text);
                    double cu = double.Parse(CU.Text);
                    double rec = double.Parse(Rec.Text);
                    double fc = double.Parse(Fc.Text);

                    if (string.IsNullOrEmpty(Hv.Text))
                    {
                        Hv.Text = 0.0.ToString();
                        double h = double.Parse(Hv.Text);
                        //primer calculo lo hace con un valor de h = 0.00, para obtener un primer valor de momento de diseño 
                        //y devuelve una altura de viga h todavia sin considerar el peso propio
                        double me = Math.Round(((cu * ((lt1 + lt2) / 2) + (b / 100 * h / 100 * 2.4)) * lvmax * lvmax) / 8, 2);
                        double md = Math.Round(me * f * .65 * .85, 2);
                        double hbeam = Math.Round(Math.Sqrt(md * 100000 / 0.145 / fc / b) + rec, 0);
                        Me.Text = string.Format("{0:F2}", me);
                        Md.Text = string.Format("{0:F2}", md);
                        Hv.Text = string.Format("{0:0}", hbeam);
                        //chequeo h/b
                        double HB = h / b;
                        if (HB > 1.1 && HB < 1.6)
                        {
                            Chkbeam.Text = "Relacion h/b ok, Viga eficiente";
                            Chkbeam.TextColor = Color.Green;
                            hb.Text = string.Format("{0:F2}", HB);
                        }
                        else
                        {
                            Chkbeam.Text = "Relacion h/b ok, Viga eficiente";
                            Chkbeam.TextColor = Color.Red;
                            hb.Text = string.Format("{0:F2}", HB);
                        }


                        Hv.IsEnabled = false;
                    }
                    else
                    {
                        double h = double.Parse(Hv.Text);
                        double me = Math.Round(((cu * ((lt1 + lt2) / 2) + (b / 100 * h / 100 * 2.4)) * lvmax * lvmax) / 8, 2);
                        double md = Math.Round(me * f * .65 * .85, 2);
                        Me.Text = string.Format("{0:F2}", me);
                        Md.Text = string.Format("{0:F2}", md);
                        Hv.Text = string.Format("{0:0}", h);

                        Hv.IsEnabled = false;
                        //chequeo h/b
                        double HB = h / b;
                        if (HB > 1.1 && HB < 1.6)
                        {
                            Chkbeam.Text = "Relacion h/b ok, Viga eficiente";
                            Chkbeam.TextColor = Color.Green;
                            hb.Text = string.Format("{0:F2}", HB);
                        }
                        else
                        {
                            Chkbeam.Text = "Relacion h/b No Cumple, Aumente Ancho de Viga";
                            Chkbeam.TextColor = Color.Red;
                            hb.Text = string.Format("{0:F2}", HB);
                        }
                    }
                }
            }
            else
            {
                Hv.IsEnabled = true;
            }

        }

        public void Ascom_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(Fy.Text) &&
                !string.IsNullOrEmpty(Md.Text) &&
                AsEst.SelectedIndex != -1)
            {
                double b = double.Parse(Bv.Text);
                double h = double.Parse(Hv.Text);
                double rec = double.Parse(Rec.Text);
                double romax = double.Parse(pm.Text);
                double fy = double.Parse(Fy.Text); // resistencia del acero
                double fc = double.Parse(Fc.Text);
                double md = double.Parse(Md.Text); //momento de diseño
                                                   //seleccion de diametro de estribo
                if (AsEst.SelectedIndex == 0)
                {
                    double est = 10;
                    AuxEst.Text = est.ToString();
                }
                else if (AsEst.SelectedIndex == 1)
                {
                    double est = 12;
                    AuxEst.Text = est.ToString();
                }
                else if (AsEst.SelectedIndex == 2)
                {
                    double est = 14;
                    AuxEst.Text = est.ToString();
                }
                // seleccion de diametro de refuerzo
                if (Ascom.SelectedIndex == 0)
                {
                    double reb = 12;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 1)
                {
                    double reb = 14;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 2)
                {
                    double reb = 16;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 3)
                {
                    double reb = 18;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 4)
                {
                    double reb = 20;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 5)
                {
                    double reb = 22;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 6)
                {
                    double reb = 25;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 7)
                {
                    double reb = 28;
                    AuxRebar.Text = reb.ToString();
                }
                else if (Ascom.SelectedIndex == 8)
                {
                    double reb = 32;
                    AuxRebar.Text = reb.ToString();
                }
                //peralte
                double rebar = double.Parse(AuxRebar.Text);
                double estribo = double.Parse(AuxEst.Text);
                double d = h - rec - estribo / 10 - rebar / 20;
                Dv.Text = d.ToString("F2");
                //acero minimo y total
                double asmin = Math.Round(14 * b * d / fy, 2);
                double ast = Math.Round(30 * md / d, 2);
                Asmin.Text = string.Format("{0:F2}", asmin);
                Ast.Text = string.Format("{0:F2}", ast);

                Dv.Text = string.Format("{0:F2}", d);
                // cantidad de acero minimo colocado
                double nvarmin = Math.Max(Math.Round(asmin / (0.00785 * rebar * rebar), 0), 2);
                Nvarmin.Text = string.Concat(nvarmin, " Barras de ", rebar, "mm", " Para Acero mínimo As(min)");
                //cantidad de acero de refuerzo colocado
                double refuerzo = Math.Round((ast - (nvarmin * 0.00785 * rebar * rebar)) / (0.00785 * rebar * rebar) + 1, 0);
                Refuerzo.Text = string.Concat(refuerzo, " Barras de ", rebar, "mm", " Para Refuerzo As(-)");
                //cantidad de acero de refuerzo positivo 
                double rpositivo = Math.Round(0.75 * ast / (0.00785 * rebar * rebar), 0);
                Rpositivo.Text = string.Concat(rpositivo, " Barras de ", rebar, "mm", " Para Refuerzo As(+)");
                //area de acero colocada
                double ascomt = Math.Round((nvarmin + refuerzo) * 0.00785 * rebar * rebar, 2);
                AscomT.Text = string.Concat("Area total de acero colocado = ", ascomt, " cm2");
                // separacion entre aceros 
                double sep = Math.Round((b - 2 * rec - estribo / 5 - (nvarmin + refuerzo) * rebar / 10) / (nvarmin + refuerzo - 1), 2);
                Sep.Text = string.Concat("Separación Entre Aceros ", sep.ToString("F2"), " cm");
                //Separacion minima
                double sepmin = Math.Max(rebar / 10, 2.54);
                Sepmin.Text = string.Concat("Separación mimina ", sepmin.ToString("F2"), "cm");
                //chequeo de separación
                if (sep > sepmin)
                {
                    ChkSep.Text = ("Separación Cumple el minimo de la normativa");
                    ChkSep.TextColor = Color.Green;
                }
                else
                {
                    ChkSep.Text = ("Separación no cumple, Aumentar diametro de refuerzo");
                    ChkSep.TextColor = Color.Red;
                }
                //control de fisura 
                double dc = rec + estribo / 10 + rebar / 20;
                Dc.Text = string.Concat("Distancia al centro de la varilla (dc) ", dc.ToString("F2"), "cm");
                double Z = 0.6 * fy * Math.Pow((2 * dc * dc * b) / (nvarmin + refuerzo), 0.33333);
                //chequeo de fisuras 
                if (Z < 23000)
                {
                    Fisura.Text = string.Concat("Viga apta para interiores y exteriores", " Z = ", Z.ToString("F2"));
                    Fisura.TextColor = Color.Green;
                }
                else if (Z >= 23000 && Z <= 31000)
                {
                    Fisura.Text = string.Concat("Viga apta solo para interiores", " Z = ", Z.ToString("F2"));
                    Fisura.TextColor = Color.YellowGreen;
                }
                else if (Z > 31000)
                {
                    Fisura.Text = string.Concat("Fisuración no apta Aumente acero", " Z = ", Z.ToString("F2"));
                    Fisura.TextColor = Color.Red;
                }
                //cuantia de acero 
                double ro = Math.Round(ascomt / h / b * 100, 2);
                Ro.Text = string.Concat("Cuantia colocada ", ro.ToString("F2"), "%");
                Romax.Text = string.Concat("Cuantia Maxima Permitida ", romax.ToString("F2"), "%");
                if (ro < romax)
                {
                    ChkRo.Text = ("Cuantia de acero OK");
                    ChkRo.TextColor = Color.Green;
                }
                else
                {
                    ChkRo.Text = ("Cuantia de acero No Cumple, Aumente Acero");
                    ChkRo.TextColor = Color.Red;
                }
                //Momento resistente y demanda capacidad
                double a = ascomt * fy / (0.85 * fc * b);
                double Mr = (0.90 * ascomt * fy * (d - a / 2)) / 100000;
                MR.Text = string.Concat("Momento Resistente = ", Mr.ToString("F2"), "t-m");
                double d7c = md / Mr;
                DC.Text = string.Concat("Demanda - Capacidad D/C = ", d7c.ToString("F2"));
            }

        }

        private void AsEst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Ascom.SelectedIndex != 0)
            { Ascom_SelectedIndexChanged(sender, e); }

        }
    }
}