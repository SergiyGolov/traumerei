using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

        private double[] RValues;
        private double[] GValues;
        private double[] BValues;

        private float animationFactorR;
        private float animationFactorG;
        private float animationFactorB;

        private int RXoffset;
        private int RYoffset;
        private int GXoffset;
        private int GYoffset;
        private int BXoffset;
        private int BYoffset;

        private int sizeBinaryTimes;

        private bool animationAnchor;

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


            animationAnchor = false;

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

        public new void SetDimensions(int width, int height)
        {
            Width = width;
            Height = height;
            imgBitmap = new SKBitmap(Width, Height);
            RValues = new double[width * height];
            GValues = new double[width * height];
            BValues = new double[width * height];

            sizeBinaryTimes = width - 1;

        }

        //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
            (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);

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

            RXoffset = 0;
            RYoffset = 0;
            GXoffset = 0;
            GYoffset = 0;
            BXoffset = 0;
            BYoffset = 0;

            _random = new Random();

            animationFactorR = 5 + _random.Next(15);
            animationFactorG = 5 + _random.Next(15);
            animationFactorB = 5 + _random.Next(15);

            Array combinationValues = Enum.GetValues(typeof(Combination));

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth) + 3; i++)
            {
                funcListR.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListR.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListR.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth) + 3; i++)
            {
                funcListG.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListG.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListG.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            maxDepth = _random.Next(5) + 3;

            for (int i = 0; i < _random.Next(maxDepth) + 3; i++)
            {
                funcListB.Add(avalaibleFuncs[_random.Next(avalaibleFuncs.Count)]);
                atomicFuncListB.Add(avalaibleAtomicFuncs[_random.Next(avalaibleAtomicFuncs.Count)]);
                combinationListB.Add((Combination)combinationValues.GetValue(_random.Next(combinationValues.Length)));
            }

            int threadNb = Environment.ProcessorCount;
            Thread[] threads = new Thread[threadNb];

            //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
            IntPtr pixelsAddr = imgBitmap.GetPixels();

            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId] = new Thread((i) =>
                {
                    int s = (int)i;
                    int wh = Width * Height;

                    unsafe
                    {
                        uint* ptr = (uint*)pixelsAddr.ToPointer();

                        ptr += s;

                        while (s < wh)
                        {
                            int y = (s / Width);
                            int x = s - (y * Width);

                            RValues[s] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListR, funcListR, combinationListR);
                            GValues[s] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListG, funcListG, combinationListG);
                            BValues[s] = Width / 2 * applyFuncsFromList(transformX(x), transformY(y), atomicFuncListB, funcListB, combinationListB);

                            *ptr = MakePixel((byte)RValues[s], (byte)GValues[s], (byte)BValues[s], 0xFF);

                            s += threadNb;
                            ptr += threadNb;
                        }
                    }
                });
                threads[threadId].Start(threadId);
            }

            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId].Join();
            }

            return imgBitmap;
        }


        public void toggleAnimationAnchor()
        {
            animationAnchor = !animationAnchor;
        }


        public new SKBitmap Step(float deltaX, float deltaY, float deltaZ)
        {

            int local_RXoffset = (int)(deltaX * animationFactorR);
            int local_RYoffset = (int)(deltaY * animationFactorR);

            int local_GXoffset = (int)(deltaY * animationFactorG);
            int local_GYoffset = (int)(deltaX * -1 * animationFactorG);

            int local_BXoffset = (int)(deltaX * -1 * animationFactorB);
            int local_BYoffset = (int)(deltaY * -1 * animationFactorB);

            if (!animationAnchor)
            {
                RXoffset += local_RXoffset;
                RYoffset += local_RYoffset;

                GXoffset += local_GXoffset;
                GYoffset += local_GYoffset;

                BXoffset += local_BXoffset;
                BYoffset += local_BYoffset;
            }
            else
            {
                RXoffset = local_RXoffset;
                RYoffset = local_RYoffset;

                GXoffset = local_GXoffset;
                GYoffset = local_GYoffset;

                BXoffset = local_BXoffset;
                BYoffset = local_BYoffset;
            }


            int threadNb = Environment.ProcessorCount;
            Thread[] threads = new Thread[threadNb];

            IntPtr pixelsAddr = imgBitmap.GetPixels();

            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId] = new Thread((i) =>
                {
                    int s = (int)i;
                    int wh = Width * Height;

                    unsafe
                    {
                        uint* ptr = (uint*)pixelsAddr.ToPointer();

                        ptr += s;

                        while (s < wh)
                        {
                            int y = (s / Width);
                            int x = s - (y * Width);

                            //source for bitwise and modulo trick: https://jacksondunstan.com/articles/1946
                            int yR = (y + RYoffset) & sizeBinaryTimes;
                            int xR = (x + RXoffset) & sizeBinaryTimes;

                            int yG = (y + GYoffset) & sizeBinaryTimes;
                            int xG = (x + GXoffset) & sizeBinaryTimes;

                            int yB = (y + BYoffset) & sizeBinaryTimes;
                            int xB = (x + BXoffset) & sizeBinaryTimes;

                            int indexR = xR + yR * Width;
                            int indexG = xG + yG * Width;
                            int indexB = xB + yB * Width;

                            *ptr = MakePixel((byte)RValues[indexR], (byte)GValues[indexG], (byte)BValues[indexB], 0xFF);

                            s += threadNb;
                            ptr += threadNb;
                        }
                    }
                });
                threads[threadId].Start(threadId);
            }

            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId].Join();
            }

            return imgBitmap;
        }

        public void load(SKBitmap loaded)
        {
            imgBitmap = loaded;
            SKColor[] pixels = loaded.Pixels;

            int threadNb = Environment.ProcessorCount;
            Thread[] threads = new Thread[threadNb];
           
            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId] = new Thread((i) =>
                {
                    int s = (int)i;
                    int wh = Width * Height;

                    while (s < wh)
                    {
                        RValues[s] = pixels[s].Red;
                        GValues[s] = pixels[s].Green;
                        BValues[s] = pixels[s].Blue;

                        s += threadNb;
                    }
                });
                threads[threadId].Start(threadId);
            }

            for (int threadId = 0; threadId < threadNb; threadId++)
            {
                threads[threadId].Join();
            }

            RXoffset = 0;
            RYoffset = 0;
            GXoffset = 0;
            GYoffset = 0;
            BXoffset = 0;
            BYoffset = 0;

            _random = new Random();

            animationFactorR = 5 + _random.Next(15);
            animationFactorG = 5 + _random.Next(15);
            animationFactorB = 5 + _random.Next(15);

        }
    }
}


