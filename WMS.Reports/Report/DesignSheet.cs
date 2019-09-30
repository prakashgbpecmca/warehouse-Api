using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Globalization;
using System.Reflection;
using System.Linq;
using DevExpress.XtraRichEdit;

namespace WMS.Reports.Report
{
    public partial class DesignSheet : DevExpress.XtraReports.UI.XtraReport
    {
        public DesignSheet()
        {
            InitializeComponent();
        }

        internal System.Drawing.Color GetBackColor(XRTableCell obj)
        {
            if (obj.Text.ToUpper().Contains("RED"))
                return Color.Red;
            else if (obj.Text.ToUpper().Contains("WHITE"))
                return Color.White;
            else if (obj.Text.ToUpper().Contains("BLACK"))
                return Color.Black;
            else if (obj.Text.ToUpper().Contains("VILET"))
                return Color.Violet;
            else if (obj.Text.ToUpper().Contains("YELLOW"))
                return Color.Yellow;
            else if (obj.Text.ToUpper().Contains("DARKGRAY"))
                return Color.DarkGray;
            else if (obj.Text.ToUpper().Contains("BLUE"))
                return Color.White;
            else if (obj.Text.ToUpper().Contains("ORANGE"))
                return Color.Orange;
            else if (obj.Text.ToUpper().Contains("PINK"))
                return Color.Pink;
            else if (obj.Text.ToUpper().Contains("OLIVE"))
                return Color.Olive;
            else if (obj.Text.ToUpper().Contains("BROWN"))
                return Color.Brown;
            else if (obj.Text.ToUpper().Contains("GREEN"))
                return Color.Green;
            else if (obj.Text.ToUpper().Contains("LIGHT CORAL"))
                return Color.LightCoral;
            else if (obj.Text.ToUpper().Contains("MAROON"))
                return Color.Maroon;
            else if (Name.Contains("PURPLE"))
                return Color.White;
            else if (Name.Contains("NAVY"))
                return Color.White;
            else
                return Color.Transparent;
        }

        internal System.Drawing.Color GetForeColor(string Name)
        {
            if (Name.Contains("RED"))
                return Color.Black;
            else if (Name.Contains("WHITE"))
                return Color.Black;
            else if (Name.Contains("BLACK"))
                return Color.White;
            else if (Name.Contains("VILET"))
                return Color.Black;
            else if (Name.Contains("YELLOW"))
                return Color.Black;
            else if (Name.Contains("DARKGRAY"))
                return Color.Black;
            else if (Name.Contains("BLUE"))
                return Color.White;
            else if (Name.Contains("ORANGE"))
                return Color.Black;
            else if (Name.Contains("PINK"))
                return Color.Black;
            else if (Name.Contains("OLIVE"))
                return Color.Black;
            else if (Name.Contains("BROWN"))
                return Color.Black;
            else if (Name.Contains("GREEN"))
                return Color.Black;
            else if (Name.Contains("LIGHT CORAL"))
                return Color.Black;
            else if (Name.Contains("MAROON"))
                return Color.Black;
            else if (Name.Contains("CYAN"))
                return Color.Black;
            else if (Name.Contains("PURPLE"))
                return Color.White;
            else if (Name.Contains("NAVY"))
                return Color.White;
            else
                return Color.Transparent;

        }

        internal string GetRGBString(string cmykcolor)
        {
            string[] cc = cmykcolor.Replace("%", "").Split(',');
            int red = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[0]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            int green = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[1]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            int blue = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[2]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            return red + "," + green + "," + blue;
        }

        internal Tuple<int, int, int> GetRGBValues(string cmykcolor)
        {
            string[] cc = cmykcolor.Replace("%", "").Split(',');
            int red = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[0]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            int green = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[1]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            int blue = Convert.ToInt32(255 * (1 - (Convert.ToDecimal(cc[2]) / 100)) * (1 - (Convert.ToDecimal(cc[3]) / 100)));
            return Tuple.Create(red, green, blue);
        }

        private void xrTableCell68_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
               
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if(obj.Text != "")
            {
                xrTableCell68.Visible = true;
            }
            else if(obj.Text == "")
            {
                xrTableCell68.Visible = false;
            }
          
        }

        private void xrTableCell70_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell70.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell70.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell70.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell70.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell70.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell70.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell70.Visible = false;
            }
        }

        private void xrTableCell83_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell83.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell83.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell83.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell83.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell83.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell83.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell83.Visible = false;
            }
        }

        private void xrTableCell119_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell119.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell119.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell119.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell119.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell119.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            
            }
            if (obj.Text != "")
            {
                xrTableCell119.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell119.Visible = false;
            }
        }

        private void xrTableCell121_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell121.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell121.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell121.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell121.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell121.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
           
            }
            if (obj.Text != "")
            {
                xrTableCell121.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell121.Visible = false;
            }
        }

        private void xrTableCell123_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell123.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell123.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell123.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell123.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell123.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell123.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell123.Visible = false;
            }
        }

        private void xrTableCell129_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell129.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell129.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell129.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell129.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell129.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            
            }
            if (obj.Text != "")
            {
                xrTableCell129.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell129.Visible = false;
            }
        }

        private void xrTableCell131_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell131.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell131.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell131.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell131.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell131.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            
            }
            if (obj.Text != "")
            {
                xrTableCell131.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell131.Visible = false;
            }
        }

        private void xrTableCell36_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell36.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell36.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell36.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell36.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell36.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell36.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell36.Visible = false;
            }
        }

        private void xrTableCell94_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell94.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell94.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell94.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell94.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell94.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell94.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell94.Visible = false;
            }
        }

        private void xrTableCell96_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell96.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell96.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell96.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell96.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell96.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            }
            if (obj.Text != "")
            {
                xrTableCell96.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell96.Visible = false;
            }
        }

        private void xrTableCell139_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell139.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell139.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell139.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell139.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell139.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell139.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell139.Visible = false;
            }
        }

        private void xrTableCell141_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell141.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell141.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell141.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell141.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell141.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell141.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell141.Visible = false;
            }
        }

        private void xrTableCell143_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell143.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell143.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell143.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell143.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell143.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell143.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell143.Visible = false;
            }
        }

        private void xrTableCell149_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell149.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell149.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell149.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell149.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell149.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell149.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell149.Visible = false;
            }
        }

        private void xrTableCell151_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell151.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell151.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell151.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell151.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell151.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell151.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell151.Visible = false;
            }
        }

        private void xrTableCell155_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell155.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell155.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell155.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell155.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell155.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
          
            }
            if (obj.Text != "")
            {
                xrTableCell155.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell155.Visible = false;
            }
        }

        private void xrTableCell157_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell157.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell157.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell157.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell157.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell157.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            
            }
            if (obj.Text != "")
            {
                xrTableCell157.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell157.Visible = false;
            }
        }

        private void xrTableCell185_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell185.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell185.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell185.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell185.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell185.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell185.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell185.Visible = false;
            }
        }

        private void xrTableCell191_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell191.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell191.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell191.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell191.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell191.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell191.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell191.Visible = false;
            }
        }

        private void xrTableCell193_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell193.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell193.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell193.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell193.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell193.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell193.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell193.Visible = false;
            }
        }

        private void xrTableCell195_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell195.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell195.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell195.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell195.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell195.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell195.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell195.Visible = false;
            }
        }

        private void xrTableCell201_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell201.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell201.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell201.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell201.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell201.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell201.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell201.Visible = false;
            }
        }

        private void xrTableCell203_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell203.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell203.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell203.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell203.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell203.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell203.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell203.Visible = false;
            }
        }

        private void xrTableCell159_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell159.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell159.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell159.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell159.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell159.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell159.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell159.Visible = false;
            }
        }

        private void xrTableCell161_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell161.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell161.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell161.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell161.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell161.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell161.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell161.Visible = false;
            }
        }

        private void xrTableCell163_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell163.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell163.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell163.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell163.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell163.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell163.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell163.Visible = false;
            }
        }

        private void xrTableCell169_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell169.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell169.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell169.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell169.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell169.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell169.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell169.Visible = false;
            }
        }

        private void xrTableCell171_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell171.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell171.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell171.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell171.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell171.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell171.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell171.Visible = false;
            }
        }

        private void xrTableCell173_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell173.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell173.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell173.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell173.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell173.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell173.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell173.Visible = false;
            }
        }

        private void xrTableCell179_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell179.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell179.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell179.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell179.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell179.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
            
            }
            if (obj.Text != "")
            {
                xrTableCell179.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell179.Visible = false;
            }
        }

        private void xrTableCell181_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell181.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell181.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell181.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell181.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell181.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                
            }
            if (obj.Text != "")
            {
                xrTableCell181.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell181.Visible = false;
            }
        }

        private void xrTableCell133_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell133.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell133.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell133.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell133.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell133.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell133.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell133.Visible = false;
            }
        }

        private void xrTableCell18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell18.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell18.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell18.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell18.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell18.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell18.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell18.Visible = false;
            }
        }

        private void xrTableCell22_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell22.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell22.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell22.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell22.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell22.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
           
            }
            if (obj.Text != "")
            {
                xrTableCell22.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell22.Visible = false;
            }
        }

        private void xrTableCell43_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell43.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell43.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell43.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell43.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell43.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell43.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell43.Visible = false;
            }
        }

        private void xrTableCell44_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell44.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell44.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell44.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell44.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell44.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                
            }
            if (obj.Text != "")
            {
                xrTableCell44.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell44.Visible = false;
            }
        }

        private void xrTableCell46_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell46.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell46.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell46.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell46.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell46.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell46.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell46.Visible = false;
            }
        }

        private void xrTableCell153_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell153.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell153.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell153.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell153.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell153.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell153.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell153.Visible = false;
            }
        }

        private void xrTableCell60_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell60.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell60.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell60.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell60.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell60.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
 
            }
            if (obj.Text != "")
            {
                xrTableCell60.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell60.Visible = false;
            }
        }

        private void xrTableCell62_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell62.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell62.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell62.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell62.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell62.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell62.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell62.Visible = false;
            }
        }

        private void xrTableCell64_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell64.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell64.Visible = false;
            }
        }

        private void xrTableCell74_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell74.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell74.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell74.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell74.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell74.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell74.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell74.Visible = false;
            }
        }

        private void xrTableCell76_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell76.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell76.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell76.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell76.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell76.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell76.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell76.Visible = false;
            }
        }

        private void xrTableCell205_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell205.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell205.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell205.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell205.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell205.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell205.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell205.Visible = false;
            }
        }

        private void xrTableCell86_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell86.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell86.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell86.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell86.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell86.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell86.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell86.Visible = false;
            }
        }

        private void xrTableCell88_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell88.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell88.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell88.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell88.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell88.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell88.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell88.Visible = false;
            }
        }

        private void xrTableCell90_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell90.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell90.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell90.Visible = false;
            }
        }

        private void xrTableCell101_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell101.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell101.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell101.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell101.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell101.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell101.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell101.Visible = false;
            }
        }

        private void xrTableCell103_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];
                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell103.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell103.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell103.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell103.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell103.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell103.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell103.Visible = false;
            }
        }

        private void xrTableCell183_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell183.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell183.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell183.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell183.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell183.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell183.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell183.Visible = false;
            }
        }

        private void xrTableCell111_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell111.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell111.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell111.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell111.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell111.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
             
            }
            if (obj.Text != "")
            {
                xrTableCell111.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell111.Visible = false;
            }
        }

        private void xrTableCell113_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell113.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell113.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell113.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell113.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell113.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell113.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell113.Visible = false;
            }
        }

        private void xrTableCell206_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell206.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell206.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell206.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell206.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell206.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell206.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell206.Visible = false;
            }
        }

        private void xrTableCell212_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell212.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell212.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell212.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell212.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell212.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
              
            }
            if (obj.Text != "")
            {
                xrTableCell212.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell212.Visible = false;
            }
        }

        private void xrTableCell214_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell obj = (XRTableCell)sender;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrTableCell214.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell214.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell214.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrTableCell214.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text != "")
                {
                    xrTableCell214.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
               
            }
            if (obj.Text != "")
            {
                xrTableCell214.Visible = true;
            }
            else if (obj.Text == "")
            {
                xrTableCell214.Visible = false;
            }
        }

        private void xrLabel6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel9;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name== "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }
                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }
        
        private void xrLabel15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel3;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);

             
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }

        private void xrLabel16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel4;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);

             
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }

        private void xrLabel19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel13;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);

            
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel19.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel19.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel19.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel19.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel19.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }

        private void xrLabel20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel14;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }

        private void xrLabel2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel obj = (XRLabel)sender;
            //XRLabel objColorHex = (XRLabel)xrLabel14;

            if (obj.Text.Contains("~"))
            {
                string[] s = obj.Text.Split('~');
                obj.Text = s[1];

                obj.BackColor = ColorTranslator.FromHtml(s[0]);
                if (obj.BackColor.Name == "ffffffff" || obj.BackColor.Name == "fff6fcfa" || obj.BackColor.Name == "ffcedcdb" || obj.BackColor.Name == "fffffef0" || obj.BackColor.Name == "fff2d6bf" || obj.BackColor.Name == "ffa9a8ab" || obj.BackColor.Name == "fffefefe" || obj.BackColor.Name == "ffccc9b5" || obj.BackColor.Name == "ffc6d0b4")
                {
                    obj.ForeColor = GetForeColor("White".ToUpper());
                }
                else
                {

                    obj.ForeColor = GetForeColor("Black".ToUpper());
                }

                obj.BorderColor = ColorTranslator.FromHtml("Black");

                xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                if (obj.Text.Contains("%"))
                {
                    //Convert cmyk to rgb
                    string rgb = GetRGBString(obj.Text);
                    Tuple<int, int, int> tuple = GetRGBValues(obj.Text);

                    obj.BackColor = ColorTranslator.FromHtml(rgb);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains("#"))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else if (obj.Text.Contains(","))
                {
                    obj.BackColor = ColorTranslator.FromHtml(obj.Text);

                    obj.ForeColor = GetForeColor("Black".ToUpper());

                    obj.BorderColor = ColorTranslator.FromHtml("Black");

                    xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                }
                else
                {
                    if (obj.Text != "")
                    {
                        obj.BackColor = ColorTranslator.FromHtml("White");

                        obj.ForeColor = GetForeColor("White".ToUpper());

                        obj.BorderColor = ColorTranslator.FromHtml("Black");

                        xrLabel20.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                    }
                }
            }
        }       
    }
}
