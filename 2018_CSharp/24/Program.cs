using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _24
{
    using static WarSideType;
    using static ImmunitiesAndWeaknesses;

    public enum WarSideType { ImmuneSystem, Infection }
    public enum ImmunitiesAndWeaknesses { None, Fire, Cold, Radiation, Slashing, Bludgeoning }

    public static class Helper
    {
        public static ImmunitiesAndWeaknesses toImmunitiesAndWeaknesses(this string str)
        {
            switch (str)
            {
                case "fire": return Fire;
                case "cold": return Cold;
                case "radiation": return Radiation;
                case "slashing": return Slashing;
                case "bludgeoning": return Bludgeoning;
                default: return None;
            }
        }
    }

    public class Unit
    {
        public readonly WarSideType WarSide;
        public int Count { get; private set; }
        public readonly int HPEach;
        public readonly List<ImmunitiesAndWeaknesses> Immune;
        public readonly List<ImmunitiesAndWeaknesses> Weak;
        public readonly int APEach;
        public readonly ImmunitiesAndWeaknesses AttackType;
        public readonly int Initiative;

        public bool Destroyed => Count <= 0;

        public Unit(WarSideType ws, int cnt, int hp, List<ImmunitiesAndWeaknesses> ilst, List<ImmunitiesAndWeaknesses> wlst, int ap, ImmunitiesAndWeaknesses at, int iv)
        {
            WarSide = ws;
            Count = cnt;
            HPEach = hp;
            Immune = ilst;
            Weak = wlst;
            APEach = ap;
            AttackType = at;
            Initiative = iv;
        }
        
        public int RecivedDamageFromEnemy(Unit enemy)
        {
            var eEP = enemy.EffPower;
            var eAT = enemy.AttackType;
            var damageRecive = eEP;
            if (Weak.Contains(eAT)) damageRecive *= 2;
            if (Immune.Contains(eAT)) damageRecive *= 0;
            return damageRecive;
        }

        public void TakeDamageFromEnemy(Unit enemy)
        {
            var damageDealt = RecivedDamageFromEnemy(enemy);
            Count = (SumHP - damageDealt) / HPEach;
        }

        public int SumHP => Count * HPEach;
        public int EffPower => Count * APEach;

        static Regex rUnit = new Regex(@"^(?<cnt>\d+) .+with (?<hp>\d+).+ points[ ]?(?>\((?<IW>.+)\))? with .+does (?<damage>\d+) (?<damageType>.+) damage.+ (?<initiative>\d+)$", RegexOptions.Compiled);
        static Regex rIW = new Regex(@"^(?>(?>; )?immune to (?<immune1>\w+)(?>, (?<immune2>\w+))?)?(?>(?>; )?weak to (?<weak1>\w+)(?>, (?<weak2>\w+))?)?$", RegexOptions.Compiled);
        public static Unit Parse(WarSideType ws, string str)
        {
            var mu = rUnit.Matches(str).First();
            var cnt = int.Parse(mu.Groups["cnt"].Value);
            var hp = int.Parse(mu.Groups["hp"].Value);
            var ap = int.Parse(mu.Groups["damage"].Value);
            var intve = int.Parse(mu.Groups["initiative"].Value);
            var at = mu.Groups["damageType"].Value.toImmunitiesAndWeaknesses();
            var iw = mu.Groups["IW"].Value;
            var miw = rIW.Matches(iw).First();
            var ilst = new List<ImmunitiesAndWeaknesses>();
            var wlst = new List<ImmunitiesAndWeaknesses>();
            if (miw.Groups["immune1"].Success)
                ilst.Add(miw.Groups["immune1"].Value.toImmunitiesAndWeaknesses());
            if (miw.Groups["immune2"].Success)
                ilst.Add(miw.Groups["immune2"].Value.toImmunitiesAndWeaknesses());
            if (miw.Groups["weak1"].Success)
                ilst.Add(miw.Groups["weak1"].Value.toImmunitiesAndWeaknesses());
            if (miw.Groups["weak2"].Success)
                ilst.Add(miw.Groups["weak2"].Value.toImmunitiesAndWeaknesses());
            return new Unit(ws, cnt, hp, ilst, wlst, ap ,at, intve);
        }
    }

    public class Army
    {
        public readonly WarSideType WarSide;

        public List<Unit> Units { get; private set; }

        public Army(WarSideType ws, IEnumerable<Unit> units)
        {
            WarSide = ws;
            Units = units.ToList();
        }

        public int CountUnit => Units.Count(u => !u.Destroyed);

        public IEnumerable<Unit> LiveUnits => Units.Where(u => !u.Destroyed);

        public bool Defeated => CountUnit == 0;

        public IEnumerable<Unit> ByEffPower()
        {
            return Units.OrderByDescending(u => u.EffPower).ThenByDescending(u => u.Initiative);
        }

        public IEnumerable<Unit> ByIniative()
        {
            return Units.OrderByDescending(u => u.Initiative);
        }

        public Unit FindUnitForEnemy(Unit enemy)
        {
            if (Units.Select(u => u.RecivedDamageFromEnemy(enemy)).All(d => d <= 0))
                return null;
            else
                return Units.OrderByDescending(u => u.RecivedDamageFromEnemy(enemy))
                    .ThenByDescending(u => u.EffPower)
                    .ThenByDescending(u => u.Initiative).First();
        }

        public static Army Parse(string str)
        {
            var strs = str.Split('\n');
            var wsStr = strs.First();
            var ws = Infection;
            if (wsStr.StartsWith("Immune System:"))
                ws = ImmuneSystem;
            return new Army(ws, strs.Skip(1).Select(s => Unit.Parse(ws, s)));
        }
    }

    public class Battle
    {
        public Army ISArmy;
        public Army InfArmy;
        public int Turn { get; private set; }

        public Battle(string str)
        {
            var strs = str.Split("\n\n");
            ISArmy = Army.Parse(strs[0]);
            InfArmy = Army.Parse(strs[1]);
            Turn = 0;
        }


    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var battle = new Battle(input);

            Console.WriteLine(battle.ISArmy.WarSide);
            Console.WriteLine(battle.InfArmy.CountUnit);
            Console.WriteLine(battle.InfArmy.LiveUnits.Count());

            Console.WriteLine("Hello World!");
        }
    }
}