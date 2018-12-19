using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _16
{
    public class CPU
    {
        public static Dictionary<string, Action<int, int, int>> behavior = new Dictionary<string, Action<int, int, int>>()
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
        public Action<int, int, int>[] instructions { get; private set; } = new Action<int, int, int>[16];
        public Dictionary<int, HashSet<string>> ProbablyOpCodes { get; private set; }
        public int icnt { get; private set; } = 0;
        public int testSamples { get; private set; } = 0;
        public int sampleWithThreeOrMoreOpcode { get; private set; } = 0;
        public static int[] regs { get; private set; } = new int[4] { 0, 0, 0, 0 }; // reg0, reg1, reg2, reg3

        private void DoOper(int[] instr)
        {
            instructions[instr[0]](instr[1], instr[2], instr[3]);
            icnt++;
        }

        private bool CheckRegs(int[] current, int[] after)
        {
            return (current[0] == after[0]
                 && current[1] == after[1]
                 && current[2] == after[2]
                 && current[3] == after[3]);
        }

        private bool TestOper(string name, int[] before, int[] after, int[] instr)
        {
            var oper = behavior[name];
            before.CopyTo(regs,0);
            oper(instr[1], instr[2], instr[3]);
            icnt++;
            return CheckRegs(regs, after);
        }

        private void TestOpcodeBehavior(int[] before, int[] after, int[] instr, bool part1)
        {
            var testSet = ProbablyOpCodes[instr[0]];
            var newSet = testSet.Where((name, oper) => TestOper(name, before, after, instr));
            var correct = newSet.Count();
            if (correct >= 3)
                sampleWithThreeOrMoreOpcode++;
            if (!part1) ProbablyOpCodes[instr[0]] = newSet.ToHashSet();
        }

        public CPU()
        {
            ProbablyOpCodes = Enumerable.Range(0, 16).ToDictionary(i => i, _ => behavior.Keys.ToHashSet());
        }

        public void EducateCPU(IEnumerable<TestCPUInstr> tests, bool part1 = true)
        {
            foreach (var t in tests)
            {
                TestOpcodeBehavior(t.regBefore, t.regAfter, t.instr, part1);
                testSamples++;
            }
        }

        public void OptimizeOpCode()
        {
            var onlyOneOpCode = ProbablyOpCodes.Where(kvp => kvp.Value.Count() == 1);
            while (onlyOneOpCode.Count() > 0)
            { 
                foreach (var opcode in onlyOneOpCode)
                {
                    var optimazedProPablyOpCodes = new Dictionary<int, HashSet<string>>();
                    var id = opcode.Key;
                    var name = opcode.Value.First();
                    instructions[id] = behavior[name];
                    foreach (var kvp in ProbablyOpCodes)
                    {
                        var hs = kvp.Value;
                        hs.Remove(name);
                        if (hs.Count > 0)
                            optimazedProPablyOpCodes.Add(kvp.Key, hs);
                    }
                    ProbablyOpCodes = optimazedProPablyOpCodes;
                }
                onlyOneOpCode = ProbablyOpCodes.Where(kvp => kvp.Value.Count() == 1);
            }
        }

        public void DoProgram(IEnumerable<int[]> instrs)
        {
            regs = new int[4] { 0, 0, 0, 0};
            icnt = 0;
            foreach (var i in instrs)
                DoOper(i);
        }
    }

    public struct TestCPUInstr
    {
        public int[] regBefore;
        public int[] regAfter;
        public int[] instr;
        public int oper => instr[0];
    }

    public static class WindowFunction
    {
        public static IEnumerable<TestCPUInstr> SplitToEducationInstr(this IEnumerable<string> strs)
        {
            var e = strs.GetEnumerator();
            while (e.MoveNext())
            {
                var before = e.Current
                    .Replace("Before: [", "")
                    .Replace("]", "").Split(',')
                    .Select(c => int.Parse(c));
                e.MoveNext();
                var instr = e.Current
                    .Split(' ')
                    .Select(c => int.Parse(c));
                e.MoveNext();
                var after = e.Current
                    .Replace("After:  [", "")
                    .Replace("]", "").Split(',')
                    .Select(c => int.Parse(c));
                e.MoveNext();
                yield return new TestCPUInstr()
                {
                    regBefore = before.ToArray(),
                    regAfter = after.ToArray(),
                    instr = instr.ToArray(),
                };
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');

            var education = input.Take(3104)
                .SplitToEducationInstr();

            var program = input.Skip(3106)
                .Select(s => s.Split(' ')
                    .Select(c => int.Parse(c))
                    .ToArray());

            var cpu = new CPU();
            cpu.EducateCPU(education);
            var res1 = cpu.sampleWithThreeOrMoreOpcode;

            Console.WriteLine($"{res1}"); //588

            cpu = new CPU();
            cpu.EducateCPU(education, false);
            cpu.OptimizeOpCode();

            if (cpu.instructions.Count(i => i == null) > 0) Console.WriteLine($"CPU not optimazed");

            cpu.DoProgram(program);

            var res2 = CPU.regs[0];

            Console.WriteLine($"{res2}"); //627
        }
    }
}
