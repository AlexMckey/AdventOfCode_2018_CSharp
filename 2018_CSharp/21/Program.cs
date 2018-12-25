using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _21
{
    public struct Oper
    {
        public string opcode;
        public int A;
        public int B;
        public int C;

        public static Oper Parse(string str)
        {
            var arr = str.Split(' ');
            return new Oper { opcode = arr[0], A = int.Parse(arr[1]), B = int.Parse(arr[2]), C = int.Parse(arr[3]) };
        }
    }

    public class Regs
    {
        public event EventHandler<int> OnReadReg0;

        internal int[] regs;

        public Regs(int cnt = 6)
        {
            regs = new int[cnt];
        }

        public Regs(IEnumerable<int> regsInit)
        {
            regs = regsInit.ToArray();
        }

        public int this[int i]
        {
            get {
                if (i == 0)
                    OnReadReg0(this, regs[5]);
                return regs[i];
            }
            set => regs[i] = value;
        }

        public void Clear()
        {
            Array.Clear(regs, 0, 6);
        }

        public override string ToString()
        {
            return "regs[" + string.Join(",", regs) + "]";
        }

    }

    public class CPU
    {
        public Dictionary<string, Action<Regs, int, int, int>> actions = new Dictionary<string, Action<Regs, int, int, int>>()
        {
            //Addition:
            ["addr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] + regs[B],
            //(add register) stores into register C the result of adding register A and register B.
            ["addi"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] + B,
            //(add immediate) stores into register C the result of adding register A and value B.
            //Multiplication:
            ["mulr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] * regs[B],
            //(multiply register) stores into register C the result of multiplying register A and register B.
            ["muli"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] * B,
            //(multiply immediate) stores into register C the result of multiplying register A and value B.
            //Bitwise AND:
            ["banr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] & regs[B],
            //(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
            ["bani"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] & B,
            //(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
            //Bitwise OR:
            ["borr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] | regs[B],
            //(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
            ["bori"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] | B,
            //(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
            //Assignment:
            ["setr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A],
            //(set register) copies the contents of register A into register C. (Input B is ignored.)
            ["seti"] = (Regs regs, int A, int B, int C) => regs[C] = A,
            //(set immediate) stores value A into register C. (Input B is ignored.)
            //Greater-than testing:
            ["gtir"] = (Regs regs, int A, int B, int C) => regs[C] = A > regs[B] ? 1 : 0,
            //(greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
            ["gtri"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] > B ? 1 : 0,
            //(greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
            ["gtrr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] > regs[B] ? 1 : 0,
            //(greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
            //Equality testing:
            ["eqir"] = (Regs regs, int A, int B, int C) => regs[C] = A == regs[B] ? 1 : 0,
            //(equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
            ["eqri"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] == B ? 1 : 0,
            //(equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
            ["eqrr"] = (Regs regs, int A, int B, int C) => regs[C] = regs[A] == regs[B] ? 1 : 0,
            //(equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
        };
        private Regs regs = new Regs(6);
        public Regs Regs => regs;
        public List<int> RegsValue => regs.regs.Select(r => r).ToList();
        private int ip_reg = 0;
        private int step = 0;
        public int Steps => step;
        private List<(string action, int A, int B, int C)> instructions;

        public CPU() { }

        public void Reset()
        {
            regs.Clear();
            step = 0;
        }

        public void LoadProgram(IEnumerable<string> program)
        {
            ip_reg = int.Parse(program.First().Split(' ')[1]);
            instructions = program.Skip(1)
                .Select(Oper.Parse)
                .Select(oper => (oper.opcode, oper.A, oper.B, oper.C))
                .ToList();
        }

        public void DoProgram(int reg0Init = 0, bool logExecution = false)
        {
            Reset();
            regs[0] = reg0Init;
            var ip = regs[ip_reg];
            if (logExecution) Console.WriteLine($"Initial State: step = {step}, ip_reg = {ip_reg}");
            do
            {
                regs[ip_reg] = ip;
                //if (logExecution) Console.Write($"(step={step}) ip={ip} {Regs.ToString()} ");
                if (logExecution) Console.Write($"(step={step}) ip={ip} ");
                var instr = instructions[ip];
                if (logExecution) Console.Write($"{instr.action} {instr.A} {instr.B} {instr.C} ");
                actions[instr.action](regs, instr.A, instr.B, instr.C);
                ip = regs[ip_reg];
                ip++;
                step++;
                if (logExecution) Console.WriteLine($"{Regs.ToString()}");
                if (logExecution) Console.ReadKey();
            } while (ip >= 0 && ip < instructions.Count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');
            //var inputstr = "#ip 0;seti 5 0 1;seti 6 0 2;addi 0 1 0;addr 1 2 3;setr 1 0 0;seti 8 0 4;seti 9 0 5";
            //var input = inputstr.Split(';');

            var cpu = new CPU();
            cpu.LoadProgram(input);
            var seen = new HashSet<int>();
            var last = -1;
            cpu.Regs.OnReadReg0 += (e, val) =>
            {
                if (last == -1)
                {
                    Console.WriteLine($"Part 1: {val}");
                }
                if (seen.Contains(val))
                {
                    Console.WriteLine($"Part 2: {last}");
                    (e as Regs)[0] = (e as Regs)[5];
                    Environment.Exit(0);
                }
                last = val;
                seen.Add(last);
            };
            cpu.DoProgram();
        }
    }
}
