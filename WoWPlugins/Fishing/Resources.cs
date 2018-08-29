using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing
{
    internal static class Resources
    {
        internal static readonly Image Plugin_Fishing;
        private const string plugin_fishing = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAIAAACQkWg2AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAACxIAAAsSAdLdfvwAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAArJJREFUOE8tkmlPE1EUhufu9860Mx2m+5QyHdvSylJKKTptsVAKLUEEF/YCilCUgFGDIQQjuCcm+kXjV+N/8Ad6XZKT3OTmfU7Oe86rhKwgBAAqACjK34JAwRpFFye7h7s7xdEiAAArCClIKqh8pRoBKIH/BTCCPJvJ1GtjtVplZnqGAAIBUSABkCgAyZaSlABGCsWAESQo1mNRp3ztetnzgv4ghyqElABMAIV/mP9qRqBGscGIKbClEithp7sHRwHaw6j80TwnO5EfEnKIf2qKfByZKon4mG2ypMX6krHcYff4aqrEWMTWgqezS78+fQkLIT1QAlWOTR+JBphjcTfCMzE+6NqFBzv76XhZZVeGo/2fFztft/dMxhQMBcMBQUKJYK7PKowkvNvXVvu08axdvN/pxs1Ckuf3xpo/do9WylUmPVDs5zjkZ71L7U6utzJslRYHF119MmMXz44vXCP/qDj3c/3h06lmiBoIcQnoKonp3Nnfet7fW6n21tvubCrQdO3R96dvz6v3vt/dvzVUidOEwAGINIVDXbC4yVNbywez1YWF7HwhXC+EvFFn4OPZ5fboUjlajbKsSmyGTQSEgrGu45imOZ3Vg8lcpeFMe+HKnbw34uQuTy7SLBNRh0MkrZGItIolwLDOeVhn8dXW+ly+0Up624XJx+Otftd9f/Lq1siCIXI9tE8jFkOGvKzCoE8jPfVstT3S6DbvHs1urBWnn9Ua97zpd6v7pze7ukj5aVxgg0CfTINCsFqy0rUrY0ndPl/uTg6Uxt2Bg1LrQ3vzW3vzxY0Vg9ty6Qz5EeAyl/LOeKvULATdWuzqk9bG/NTC8uJGPV/eKzVfT600UyWBewg2KBAQYkUGQ+ZpIpF/2Vh70+jkDUflMZ+w/TQSEpGwFtaISf6MLmRnqQYA/QZ4Z2mMSRpPiwAAAABJRU5ErkJggg==";
        
        static Resources()
        {
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(plugin_fishing)))
            {
                Plugin_Fishing = Image.FromStream(memoryStream);
            }
        }

    }
}
