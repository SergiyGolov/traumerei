using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Traumerei.Algorithme
{
    class ImageGenerator_RandomFunctions : ImageGenerator_Random
    {
        private List<Func<Func<double, double, double>, double, double, double>> funcListR;
        private List<Func<Func<double, double, double>, double, double, double>> funcListG;
        private List<Func<Func<double, double, double>, double, double, double>> funcListB;

        private int maxDepth;

        private List<Func<Func<double, double, double>, double, double, double>> avalaibleFuncs;

        private List<Func<double, double, double>> avalaibleAtomicFuncs;

        public ImageGenerator_RandomFunctions() : base()
        {
            maxDepth = 3;

            funcListR = new List<Func<Func<double, double, double>, double, double, double>>();
            funcListG = new List<Func<Func<double, double, double>, double, double, double>>();
            funcListB = new List<Func<Func<double, double, double>, double, double, double>>();

            avalaibleFuncs = new List<Func<Func<double, double, double>,double, double,double>>()
            {
                (f,x,y)=>Math.Sin(Math.PI*f(x,y)),
                (f,x,y)=>Math.Cos(Math.PI*f(x,y))
            };

            avalaibleAtomicFuncs = new List<Func<double, double, double>>()
            {
                (x,y)=>x,
                (x,y)=>y,
                (x,y)=>x*y,
                (x,y)=>x/y,
                (x,y)=>y/x
            };
        }

        private double applyFuncsFromList(double x, double y,Func<double,double,double> af, List<Func<Func<double, double, double>, double, double, double>> funcList)
        {
            double z = 1;

            foreach (Func<Func<double, double, double>, double, double, double> f in funcList)
            {
                z *= f(af, x, y);
            }

            return z;
        }

        public new SKBitmap Generate()
        {
            funcListR.Clear();
            funcListG.Clear();
            funcListB.Clear();

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListR.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
            }

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListG.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
            }

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListB.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
            }

            Func<double, double, double> afR = avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)];
            Func<double, double, double> afG = avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)];
            Func<double, double, double> afB = avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)];

            SKColor color = new SKColor(0, 0, 0);
            double[] RGBvalues = { 0, 0, 0 };

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    RGBvalues[0] = applyFuncsFromList(x, y,afR, funcListR);
                    RGBvalues[1] = applyFuncsFromList(x, y,afG, funcListG);
                    RGBvalues[2] = applyFuncsFromList(x, y,afB, funcListB);

                    imgBitmap.SetPixel(x, y, color
                       .WithRed((byte)(RGBvalues[0] * 255))
                       .WithGreen((byte)(RGBvalues[1] * 255))
                       .WithBlue((byte)(RGBvalues[2] * 255))
                   );
                }
            }
            return imgBitmap;
        }
    }
}
