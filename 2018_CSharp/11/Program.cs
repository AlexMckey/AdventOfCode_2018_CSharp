using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _11
{
    class Program
    {
        const int SizeFuelCells = 300;

        static int CalcPower(int x, int y, int serialNum)
        {
            var rackID = x + 10;
            var startPower = rackID * y;
            var multAddPower = (startPower + serialNum) * rackID;
            var handredNum = (int)(multAddPower % 1000 / 100);
            return handredNum - 5;
        }

        static (int x, int y, int power) CalcMaxPowerInBoxes(int[,] powers, int boxSize)
        {
            //var fuelPowerBoxes = new int[SizeFuelCells - boxSize + 1, SizeFuelCells - boxSize + 1];

            (int x, int y, int p) maxPowerCell = (0, 0, int.MinValue);

            var curPowerInBox = 0;
            var tempPowerInBox = 0;
            for (var y = 0; y <= SizeFuelCells - boxSize; y++)
            {
                for (var x = 0; x <= SizeFuelCells - boxSize; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        for (var i = 0; i < boxSize; i++)
                            for (var j = 0; j < boxSize; j++)
                                curPowerInBox += powers[x + i, y + j];
                        tempPowerInBox = curPowerInBox;
                    }
                    else if (x == 0 && y != 0)
                    {
                        foreach (var i in Enumerable.Range(0, boxSize))
                        {
                            tempPowerInBox -= powers[x + i, y - 1];
                            tempPowerInBox += powers[x + i, y + boxSize - 1];
                            curPowerInBox = tempPowerInBox;
                        }
                    }
                    else
                    {
                        foreach (var i in Enumerable.Range(0, boxSize))
                        {
                            curPowerInBox -= powers[x - 1, y + i];
                            curPowerInBox += powers[x + boxSize - 1, y + i];
                        }
                    }
                    if (curPowerInBox > maxPowerCell.p)
                        maxPowerCell = (x, y, curPowerInBox);
                    //fuelPowerBoxes[x, y] = curPowerInBox;
                }
            }
            maxPowerCell = (maxPowerCell.x + 1, maxPowerCell.y + 1, maxPowerCell.p);
            return maxPowerCell;
        }

        static void Main(string[] args)
        {
            var input = 7803;
            //var input = 18;

            var fuelCells = new int[SizeFuelCells, SizeFuelCells];
            
            for (var i = 0; i < SizeFuelCells; i++)
                for (var j = 0; j < SizeFuelCells; j++)
                    fuelCells[i, j] = CalcPower(i+1, j+1, input);

            // First Solution
            //var pws = Enumerable.Range(1, SizeFuelCells+1)
            //    .SelectMany(x => Enumerable.Range(1, SizeFuelCells+1)
            //        .Select(y => (x: x, y: y, p: CalcPower(x,y,input))));

            //var pws3 = Enumerable.Range(1, SizeFuelCells + 1 - 3)
            //    .SelectMany(x => Enumerable.Range(1, SizeFuelCells + 1 - 3)
            //        .Select(y => (x: x, y: y, ps: Enumerable.Range(0, 4)
            //            .SelectMany(i => Enumerable.Range(0, 4)
            //                .Select(j => (xs: x + i, ys: y + j))))));

            //var MaxP = pws
            //    .Where(pw => pw.x > 0 && pw.x <= SizeFuelCells && pw.y > 0 && pw.y <= SizeFuelCells)
            //    .GroupBy(pw => (x: pw.x, y: pw.y), pw => pw.p)
            //    .Select(g => (pt: g.Key, spw: g.Sum()));


            Debug.Assert(CalcPower(3, 5, 8) == 4); // 4
            Debug.Assert(CalcPower(122, 79, 57) == -5); //-5
            Debug.Assert(CalcPower(217, 196, 39) == 0); //0
            Debug.Assert(CalcPower(101, 153, 71) == 4); //4
            Debug.Assert(CalcPower(1, 3, 18) == 0);
           
            var boxSize = 3;
            var MaxP = CalcMaxPowerInBoxes(fuelCells, boxSize);
            var res1 = $"{MaxP.x},{MaxP.y}";

            //Console.WriteLine(MaxP);

            Console.WriteLine(res1); //20,51

            var boxes = Enumerable.Range(2, SizeFuelCells + 1);
            var maxBoxesPowers = boxes.Select(bs => (bs: bs, ps: CalcMaxPowerInBoxes(fuelCells, bs)));

            var MaxBoxP = maxBoxesPowers.OrderByDescending(p => p.ps.power).First();

            var res2 = $"{MaxBoxP.ps.x},{MaxBoxP.ps.y},{MaxBoxP.bs}";

            //Console.WriteLine(MaxBoxP);

            Console.WriteLine(res2); //230,272
        }
    }
}
