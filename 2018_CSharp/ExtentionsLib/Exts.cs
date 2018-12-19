using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class Extensions
{
    public static void Dump(this object o, string label = null)
    {
        if (label != null) Console.WriteLine($"{label}: ");
        if (o.GetType().IsValueType)
        {
            Console.Write(o);
        }
        else if (o is string)
        {
            Console.Write(o);
        }
        else if (o is IEnumerable<IGrouping<object, object>>)
        {
            var d = o as IEnumerable<IGrouping<object, object>>;
            bool first = true;
            foreach (IGrouping<object, object> g in d)
            {
                if (!first)
                {
                    Console.Write(",");
                    Console.WriteLine();
                }
                g.Key.Dump();
                Console.Write(": ");
                g.Dump();
                first = false;
            }
        }
        else if (o is IDictionary)
        {
            var d = o as IDictionary;
            bool first = true;
            foreach (object k in d.Keys)
            {
                if (!first)
                {
                    Console.Write(",");
                    Console.WriteLine();
                }
                k.Dump();
                Console.Write(": ");
                d[k].Dump();
                first = false;
            }
        }
        else if (o is IEnumerable)
        {
            bool first = true;
            foreach (object c in o as IEnumerable)
            {
                if (!first)
                {
                    Console.Write(",");
                }
                c.Dump();
                first = false;
            }
        }
        else Console.Write(o.ToString());
        if (label != null) Console.WriteLine();
    }
}