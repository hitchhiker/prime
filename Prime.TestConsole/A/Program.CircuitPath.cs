using System;
using System.Linq;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public sealed class CircuitPaths
        {
            public readonly RoutePath PositivePath;
            public readonly RoutePath NegativePath;
            public readonly decimal Percentage;

            public CircuitPaths(RoutePath positivePath, RoutePath negativePath)
            {
                PositivePath = positivePath;
                NegativePath = negativePath;
                Percentage = positivePath.GetCompoundPercent() - negativePath.GetCompoundPercent();
            }

            public string Explain()
            {
               /* var s = Environment.NewLine;
                s += "-----CIRCUIT " + Math.Round(Percentage, 2) + "------";
                s += Environment.NewLine;

                s += PositivePath.Explain(false, 1);
                s += NegativePath.Explain(true, PositivePath.Count + 1);

                s += Environment.NewLine;
                s += "-------------------------------------------";
                return s;*/
                return "*";
            }

            public RoutePath MergeRoutes()
            {
                var ppi = -1;
                var npi = 0;
                var found = false;
                foreach (var ppit in PositivePath)
                {
                    ppi++;
                    npi = 0;
                    foreach (var npit in NegativePath)
                    {
                        npi++;
                        if (npit.DestinationHash == ppit.DestinationReversedHash)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                if (!found)
                    return null;

                var pp = new RoutePath(PositivePath.StartNetwork, PositivePath.StartAsset, ppi);
                foreach (var st in PositivePath.Take(ppi))
                    pp.Add(st);

                foreach (var st in NegativePath.Take(npi).Reversed())
                    pp.Add(st);

                return pp;
            }
        }
    }
}