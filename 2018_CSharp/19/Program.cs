using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _19
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

    public class CPU
    {
        public static Dictionary<string, Action<int, int, int>> actions = new Dictionary<string, Action<int, int, int>>()
        {
            //Addition:
            ["addr"] = (int A, int B, int C) => regs[C] = regs[A] + regs[B],
            //(add register) stores into register C the result of adding register A and register B.
            ["addi"] = (int A, int B, int C) => regs[C] = regs[A] + B,
            //(add immediate) stores into register C the result of adding register A and value B.
            //Multiplication:
            ["mulr"] = (int A, int B, int C) => regs[C] = regs[A] * regs[B],
            //(multiply register) stores into register C the result of multiplying register A and register B.
            ["muli"] = (int A, int B, int C) => regs[C] = regs[A] * B,
            //(multiply immediate) stores into register C the result of multiplying register A and value B.
            //Bitwise AND:
            ["banr"] = (int A, int B, int C) => regs[C] = regs[A] & regs[B],
            //(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
            ["bani"] = (int A, int B, int C) => regs[C] = regs[A] & B,
            //(bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
            //Bitwise OR:
            ["borr"] = (int A, int B, int C) => regs[C] = regs[A] | regs[B],
            //(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
            ["bori"] = (int A, int B, int C) => regs[C] = regs[A] | B,
            //(bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
            //Assignment:
            ["setr"] = (int A, int B, int C) => regs[C] = regs[A],
            //(set register) copies the contents of register A into register C. (Input B is ignored.)
            ["seti"] = (int A, int B, int C) => regs[C] = A,
            //(set immediate) stores value A into register C. (Input B is ignored.)
            //Greater-than testing:
            ["gtir"] = (int A, int B, int C) => regs[C] = A > regs[B] ? 1 : 0,
            //(greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
            ["gtri"] = (int A, int B, int C) => regs[C] = regs[A] > B ? 1 : 0,
            //(greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
            ["gtrr"] = (int A, int B, int C) => regs[C] = regs[A] > regs[B] ? 1 : 0,
            //(greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
            //Equality testing:
            ["eqir"] = (int A, int B, int C) => regs[C] = A == regs[B] ? 1 : 0,
            //(equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
            ["eqri"] = (int A, int B, int C) => regs[C] = regs[A] == B ? 1 : 0,
            //(equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
            ["eqrr"] = (int A, int B, int C) => regs[C] = regs[A] == regs[B] ? 1 : 0,
            //(equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
        };
        private static int[] regs { get; set; } = new int[6] { 0, 0, 0, 0, 0, 0 }; // reg0, reg1, reg2, reg3, reg4, reg5
        public int Regs(int id) => regs[id];
        private int ip_reg = 0;
        private int step = 0;
        public int Steps => step;
        private List<(string action, int A, int B, int C)> instructions;

        public CPU() { }

        public void Reset()
        {
            Array.Clear(regs, 0, 6);
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

        public void DoProgram(bool bug = false, bool logExecution = false)
        {
            Reset();
            if (bug) regs[0] = 1;
            var ip = regs[ip_reg];
            if (logExecution) Console.WriteLine($"Initial State: step = {step}, ip_reg = {ip_reg}");
            do
            {
                regs[ip_reg] = ip;
                if (logExecution) Console.Write($"(step={step}) ip={ip} [{regs[0]}, {regs[1]}, {regs[2]}, {regs[3]}, {regs[4]}, {regs[5]}] ");
                var instr = instructions[ip];
                if (logExecution) Console.Write($"{instr.action} {instr.A} {instr.B} {instr.C} ");
                actions[instr.action](instr.A, instr.B, instr.C);
                ip = regs[ip_reg];
                ip++;
                step++;
                if (logExecution) Console.WriteLine($"[{regs[0]}, {regs[1]}, {regs[2]}, {regs[3]}, {regs[4]}, {regs[5]}]");
                //if (logExecution) Console.ReadKey();
                if (bug && ip == 1) break;
            } while (ip >= 0 && ip < instructions.Count);
            if (bug)
            {
                var target = regs[5];
                var res = Enumerable.Range(1, target + 1)
                    .Where(num => target % num == 0)
                    .Sum();
                regs[0] = res;
            }
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
            //cpu.DoProgram(logExecution: true);
            cpu.DoProgram();

            var res1 = cpu.Regs(0);
            Console.WriteLine($"{res1}");

            cpu.Reset();
            cpu.DoProgram(bug: true);
            //cpu.DoProgram(bug: true, logExecution: true);

            var res2 = cpu.Regs(0);
            Console.WriteLine($"{res2}");
        }
    }
}