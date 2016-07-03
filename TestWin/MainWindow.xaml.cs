﻿using Gigasoft.ProEssentials.Enums;
using Microsoft.Win32;
using SortierAlgorithmen;
using Statistik;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWin
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd;
        bool set = false;
        List<double> flo;
        List<DateTime> tt;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rnd = new Random(DateTime.Now.Millisecond);


        }

        private void Foo_RunCalculationReady(object sender, CalculationReadyEventArgs e)
        {
            var res = e.Result;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < res.Length; i++)
            {
                sb.Append(res[i] + " ");
            }
            re.Text += e.Sender + ": " + sb.ToString() + Environment.NewLine;

            if (!set)
            {
                set = true;
                // Enable MouseWheel Zooming
                Pesgo.PeUserInterface.Scrollbar.MouseWheelFunction = MouseWheelFunction.HorizontalVerticalZoom;
                Pesgo.PeUserInterface.Scrollbar.MouseDraggingX = true;  // note that pan gestures require MouseDragging to be enabled 
                Pesgo.PeUserInterface.Scrollbar.MouseDraggingY = true;  // also, pan gestures should have ScrollingHorzZooom enabled

                // Enable MouseWheel Zoom Smoothness
                Pesgo.PeUserInterface.Scrollbar.MouseWheelZoomSmoothness = 5;
                Pesgo.PeUserInterface.Scrollbar.PinchZoomSmoothness = 2;

                // Allow zooming and panning //
                Pesgo.PeUserInterface.Allow.Zooming = AllowZooming.HorzAndVert;
                Pesgo.PeUserInterface.Scrollbar.ScrollingHorzZoom = true;
                Pesgo.PeUserInterface.Scrollbar.ScrollingVertZoom = true;

                // Enable ZoomWindow //
                Pesgo.PePlot.ZoomWindow.Show = true;


                Pesgo.PeString.MainTitle = "";
                Pesgo.PeString.SubTitle = "";
                Pesgo.PeString.XAxisLabel = "Zeit";
                Pesgo.PeString.YAxisLabel = "Puls";
                Pesgo.PeColor.QuickStyle = QuickStyle.DarkNoBorder;
                Pesgo.PePlot.Option.BarGlassEffect = true;
                Pesgo.PePlot.Method = SGraphPlottingMethod.PointsPlusLine;

                Pesgo.PePlot.Option.GradientBars = 8;
                Pesgo.PePlot.Option.LineShadows = true;

                // Enable Plotting style gradient and bevel features //
                Pesgo.PePlot.Option.AreaGradientStyle = PlotGradientStyle.VerticalAscentInverse;
                Pesgo.PePlot.Option.AreaBevelStyle = BevelStyle.MediumSmooth;
                Pesgo.PePlot.Option.SplineGradientStyle = PlotGradientStyle.VerticalAscentInverse;
                Pesgo.PePlot.Option.SplineBevelStyle = SplineBevelStyle.MediumSmooth;

                // v7.2 new features //
                Pesgo.PePlot.Option.PointGradientStyle = PlotGradientStyle.VerticalAscentInverse;
                Pesgo.PeColor.PointBorderColor = Color.FromArgb(100, 0, 20, 0);
               // Pesgo.PePlot.Option.LineSymbolThickness = 3;
                Pesgo.PePlot.Option.AreaBorder = 1;
                Pesgo.PeData.Points = flo.Count;
                Pesgo.PeData.Subsets = 1;
                Pesgo.PeConfigure.PrepareImages = true;

                Pesgo.PeColor.BitmapGradientMode = true;
                // subset colors //
                Pesgo.PeColor.SubsetColors[0] = Color.FromArgb(128, 198, 0, 0);
                Pesgo.PeLegend.SubsetPointTypes[0] = PointType.DotSolid;

                // v7.2 new features //
                Pesgo.PePlot.Option.PointGradientStyle = PlotGradientStyle.VerticalAscentInverse;
                Pesgo.PeColor.PointBorderColor = Color.FromArgb(100, 0, 0, 0);
                Pesgo.PePlot.Option.LineSymbolThickness = 3;
                Pesgo.PePlot.Option.AreaBorder = 1;
                Pesgo.PeUserInterface.Dialog.AllowSvgExport = true;
                Pesgo.PeData.UsingXDataii = true;
  
                Pesgo.PeData.StartTime = tt[0].ToOADate();
                for (int i = 0; i < flo.Count; i++)
                {
                    Pesgo.PeData.Y[0, i] = (float)flo[i];
                    Pesgo.PeData.Xii[0, i] = tt[i].ToOADate();
                    //Pesgo.PeData.Y[1, i] = (float)flo[i];
                }
                Pesgo.PeConfigure.RenderEngine = RenderEngine.Direct2D;
                Pesgo.PeConfigure.AntiAliasGraphics = true;
                Pesgo.PeConfigure.AntiAliasText = true;

                Pesgo.PeGrid.Option.YearMonthDayPrompt = YearMonthDayPrompt.InsideTop;

                // Optional related properties ...
                // PeGrid.Option.TimeLabelType
                // PeGrid.Option.DayLabelType
                // PeGrid.Option.MonthLabelType
                // PeGrid.Option.YearLabelType

                // Enable DateTimeMode //
                Pesgo.PeData.DateTimeMode = true;
                Pesgo.PeFunction.ReinitializeResetImage();
            }
            Pesgo.Invalidate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            set = false;
            var ofd = new OpenFileDialog();
            ofd.Filter = "*.csv|*.csv";
            ofd.ShowDialog();
            var path = ofd.FileName;
            double dd;
            DateTime oo;
            flo = new List<double>();
            tt = new List<DateTime>();

            var lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                var spl = line.Split(',');
                var str = spl[2].Replace("\"","");
                if (double.TryParse(str, out dd))
                    flo.Add(dd);
                str = spl[1].Replace("\"", "");
                if (DateTime.TryParse(str, out oo))
                    tt.Add(oo);
            }

            var foo = new Statistik.Statistik();
            foo.RunCalculationReady -= Foo_RunCalculationReady;
            foo.RunCalculationReady += Foo_RunCalculationReady;


            //for (int i = 0; i < 500; i++)
            //{
            //    flo.Add(rnd.NextDouble());
            //}

            double[] test = { 3, 3, 6, 7, 7, 10, 10, 10, 11, 13, 30 };

            test = flo.ToArray();

            var qs = new QuickSort(test);
            var bar = qs.SortIt();

           // flo = bar.ToList();

            foo.Stats = new Median(test);
            foo.SetSortedData(bar);
            foo.RunCalculationAsync();

            foo.Stats = new Spannweite(test);
            foo.SetSortedData(bar);
            foo.RunCalculationAsync();



            foo.Stats = new InterQuartilsAbstand(test);
            foo.SetSortedData(bar);
            foo.RunCalculationAsync();

            foo.Stats = new ArithmetischesMittel(test);
            foo.SetSortedData(bar);
            foo.RunCalculationAsync();

        }
    }
}
