using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Traumerei.Algorithme
{
    class ImageGenerator_RandomFunctions : ImageGenerator_Random
    {
        private List<Func<Func<double, double, double>, double, double, double>> funcListR;
        private List<Func<Func<double, double, double>, double, double, double>> funcListG;
        private List<Func<Func<double, double, double>, double, double, double>> funcListB;

        private List<Func<double, double, double>> atomicFuncListR;
        private List<Func<double, double, double>> atomicFuncListG;
        private List<Func<double, double, double>> atomicFuncListB;

        private List<Combination> combinationListR;
        private List<Combination> combinationListG;
        private List<Combination> combinationListB;

        private int maxDepth;

        private List<Func<Func<double, double, double>, double, double, double>> avalaibleFuncs;
        private List<Func<double, double, double>> avalaibleAtomicFuncs;

        private Func<Double, Double> transformX;
        private Func<Double, Double> transformY;

        enum Combination { ReplaceX, ReplaceY, Imbricate };

        public ImageGenerator_RandomFunctions() : base()
        {

            funcListR = new List<Func<Func<double, double, double>, double, double, double>>();
            funcListG = new List<Func<Func<double, double, double>, double, double, double>>();
            funcListB = new List<Func<Func<double, double, double>, double, double, double>>();

            atomicFuncListR = new List<Func<double, double, double>>();
            atomicFuncListG = new List<Func<double, double, double>>();
            atomicFuncListB = new List<Func<double, double, double>>();

            combinationListR = new List<Combination>();
            combinationListG = new List<Combination>();
            combinationListB = new List<Combination>();

            avalaibleFuncs = new List<Func<Func<double, double, double>, double, double, double>>()
            {
                (f,x,y)=>Math.Sin(Math.PI*f(x,y)),
                (f,x,y)=>Math.Cos(Math.PI*f(x,y)),
                (f,x,y)=>x*f(x,y),
                (f,x,y)=>y*f(x,y),
                (f,x,y)=>x/f(x,y),
                (f,x,y)=>y/f(x,y),
                (f,x,y)=>x+f(x,y),
                (f,x,y)=>y+f(x,y),
                (f,x,y)=>x-f(x,y),
                (f,x,y)=>y-f(x,y),
                (f,x,y)=>f(x,y),
                (f,x,y)=>Math.Abs(f(x,y)),
                (f,x,y)=>Math.Cosh(Math.PI*f(x,y)),
                (f,x,y)=>Math.Sinh(Math.PI*f(x,y)),
                (f,x,y)=>Math.Log(f(x,y)),
                (f,x,y)=>Math.Exp(f(x,y)),
            };

            avalaibleAtomicFuncs = new List<Func<double, double, double>>()
            {
                (x,y)=>x,
                (x,y)=>y,
                (x,y)=>x*y,
                (x,y)=>x/y,
                (x,y)=>y/x,
                (x,y)=>x+y,
                (x,y)=>x-y,
                (x,y)=>y-x,
                (x,y)=>Math.Sin(Math.PI*x),
                (x,y)=>Math.Sin(Math.PI*y),
                (x,y)=>Math.Cos(Math.PI*x),
                (x,y)=>Math.Cos(Math.PI*y),
                (x,y)=>Math.Sinh(Math.PI*x),
                (x,y)=>Math.Sinh(Math.PI*y),
                (x,y)=>Math.Cosh(Math.PI*x),
                (x,y)=>Math.Cosh(Math.PI*y),
                (x,y)=>Math.Exp(x),
                (x,y)=>Math.Exp(y),
                (x,y)=>Math.Log(x),
                (x,y)=>Math.Log(y),
            };

            //Adapt "image" coordinate to "mathematic" coordinate
            transformX = (x => (x - Width / 2) / (Width / 2));
            transformY = (y => (-y + Height / 2) / (Height / 2));

        }

        private double applyFuncsFromList(double x, double y,
            List<Func<double, double, double>> atomicFuncList,
            List<Func<Func<double, double, double>, double, double, double>> funcList,
            List<Combination> combList)
        {
            double z = 1;
            double localX = x;
            double localY = y;

            for (int i = 0; i < funcList.Count; i++)
            {
                Func<Func<double, double, double>, double, double, double> f = funcList[i];
                Func<double, double, double> af = atomicFuncList[i];
                Combination comb = combList[i];
                switch (comb)
                {
                    case Combination.Imbricate:
                        break;
                    case Combination.ReplaceX:
                        localX = f(af, localX, localY);
                        break;
                    case Combination.ReplaceY:
                        localY = f(af, localX, localY);
                        break;
                }
                z *= f(af, localX, localY);
            }

            return z;
        }


        public new SKBitmap Generate()
        {
            funcListR.Clear();
            funcListG.Clear();
            funcListB.Clear();

            atomicFuncListR.Clear();
            atomicFuncListG.Clear();
            atomicFuncListB.Clear();

            combinationListR.Clear();
            combinationListG.Clear();
            combinationListB.Clear();

            _random = new Random();

            Array combinationValues = Enum.GetValues(typeof(Combination));

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListR.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListR.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListR.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListG.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListG.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListG.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth); i++)
            {
                funcListB.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListB.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListB.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            SKColor color = new SKColor(0, 0, 0);
            double[] RGBvalues = { 0, 0, 0 };

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    RGBvalues[0] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListR, funcListR, combinationListR);
                    RGBvalues[1] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListG, funcListG, combinationListG);
                    RGBvalues[2] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListB, funcListB, combinationListB);
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
