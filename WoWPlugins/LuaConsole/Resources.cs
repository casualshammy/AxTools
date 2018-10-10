using System;
using System.Drawing;
using System.IO;

namespace LuaConsole
{
    internal class Resources
    {
        internal static readonly Image PluginImage;
        internal static readonly Icon PluginIcon;
        private const string pluginImage = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAE/UlEQVRYR61XW0xcVRS9A1ggQJV+aGgrpVWsJUYqUFEC8QFNG4gfmkLBtHwYE38cDGkoyuveGYaiwdEAGpjG0oBibKWxCamG9AuLH0Zb6gei1qi1rW1salK1H9bKdu1z98x9zAwM4krWnMfdZ6997zlnnzPaMuHx+/35XNF1vQosA5MMw9jb3d29AfXVwWAwXVlGQc8E6zTNeAHlg9KZGOD8HjjP6+joyEdZJN1R6O3tzfb5fLWoemBXwKV6oOklEL4MknABHLSexwGcIGpVlqFY3NgFBL3RDDZ0G8R+tInbuVfMoxEIBHLgoF6aK4BeHkM4zEkxcgLCj0jVBT0NfAYDh8DPUP8B5XmUZ1EeQfkiuFaMBXoVnsUSZ54UIwsQzwPvk6aAhY1XwKu2wfH4N+zfBzfJ2NXou+6yCbPVtBFghT8kVRt4xRrzroGJ8AaIFc8wGkAEZn+uz2has7VjiMiDFVwjTYHvCRj/4RwYn2lpAdq58z3atesorVsXDPf3mb46ClEPQngEfB5cZfYL8NnvkKpAvXnC4tu2HaQLF67jPUzcvHmL9u2bCj/H9C0C3ud4+0ppAmrOE/7s6ekBunjRErejsnKUbW7BJ29lhdra2mS88LPS1LT+/v5UFLZ9rhZclFA8VlePi1w0Dh+eDdudwURHNNrb2zeqCmx47itUQ0G9fSKrPcK6ug9NtRiYmJiz2erVIsJTngJu4sqdIKdNge9pu/NEuH79G5jzf0TSCa/3Y5ut7wMRUYBurESnkoxtUGLcv/+kSFqYmfmZUlMDdrtrEHCmdESxR6oCznDmgORkH5048Z3dwaLcvn2MRkfP0rFjX1NT0yducWH33SLE2lXq0JC2QKVXZZyS4qeFhQWXg5VSf1iEOIAy/nHlbs7tpnG8AHJyXqfduyci7R073qUtW95S9dLSd2ho6Au1+hsaLBuLvkdFSG0Araury3UkqoNFGccLoLg4RJOT30baAwOfq+yXmXmALl36nWpqxqmiYoTm56/S1q3DjrHwf78Imdper5dzgA18qpnGyw2A53zz5kFKSvJRbu6bdOrUeSSiMfvYvzTN0sPXz+OfXOc08JFqDkg0gMFBM4CsrANqAU5P/0SHDp2h06d/cQfwqYgoIBlt0Jqbm9NdiQjBmCdXvACKikI0NfV9pM11DqCx8SMlHO7nHeQKwCsiWktLSxZ0kXMAtRgcMMZ5AAfAOH78mwh5ca1Z8xpdufIn+f3TNDz8Jc3OXlYB8IF07tw1dQhxIHNzv9LY2Fcirv8G4m7ggKmLKXjSuR35MmHc8HgMKik56GBh4ZByyNmvvn4C7WG1K7KzX1X9BQVvq9Wfnz9AGRk9VF4+IgEYL4lzhc7OTsfR7+E7oNQFXbhMRD7dSomrl54kjrXW1tbb8dL3StMEOjJB15WbLxMxHS6H2Na92eJQ7Tro5ErTCSyKx6Rqg/4ynOA8j+l8KeLNLXEGNJ7q6+vLkGY0EN0DoRDf5e3gy4SO8zymSAyqBdcERj47o62tzTXNMSBT8bg0beCdwue5jiOVT7UoYSQZ3ue81aJWu7rwgsXSXBq8HkC5WkcBwfCpxgcL53ZOr+6MakLmHH/N/gMwsABcBeKWtHzw3Q9zXrronC8F/reLABo5CM5e0h0XsEvhxIbyuZ6enruk+/8BnBaFdwrqe+QP6Fr+e86fmt8WCcZ2u14KmvYvC3B9W7+ymJwAAAAASUVORK5CYII=";

        internal static readonly Image OpenFile;
        private const string openFile = "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAABVUlEQVRoQ+2Yzw7BQBDGW44IXgKP4OwuIt6FK0fv4iDO4swbiIcgEm7+fZto0q5N2+1M26yMZE52vp3fzLdtur7n+M93vH5PAMqeoExAJkDswN9aqIvGLBEDRMOySQusn1vmZF5umkAHagdEK7Oq5xUGYQJYofAJofggtRAIE8AZFbQZAJRE7hAmgAc2rjIBcMjcIbJDTBFHXdAE8MSiCsfOzBpX6PV1CJcAVD82iFG4Ma4B3PTHehqAsl92b82KkXoEgPmgmuSsJ/CCSngyzllIAJhtJRaSM0C0lLWFYhOIxWRJFwDnzoBYKIvRY3LkDMgZIFpKLCQWKtpC6sO5Rtw0r/RUH/Xq6mKYVwVE3TXyx0nXKj0s2COaxM240y/fi61TEoD6X0EE1+t17kos9ZRttogZIlK80in7CWPJ8rtcAMgtJArIBIgNJKfLBMgtJAo4P4EPa2dGMSGVYO4AAAAASUVORK5CYII=";

        internal static readonly Image SaveFile;
        private const string saveFile = "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAB60lEQVRoQ+2ZPUsEMRCG7xo/ULHXP2ChIqhgbacgYq+1WBwiFrbaaSNiIf4A7UUQG3+AH2ih2NiKtSAiWuk74MIRdrOTTJJLIAcvd7c3k7zPTDYcm2Yj8Vczcf+NDNDpDuYOxN6BERjchWahAc9mdzD+tukcuiVE5q+hQdNBBfHGEDqAMxhZFJixTTWC0AF8BFg2VZBsCB3ArzK66x1LHV+FYUHEDEBAtRCxA9RCpACghUgFgCBKvWYA280eeXW7kDp07oCg2E5SxR1w4kIwSAYQFM9Jau6AkzIKBskdEBTPSWqwDnzD7hF0Cj3/Wx/F+zK0BnVb4gQBeIW5uTbjqtcxXLiEhi0gvANQ5ac05gvP4/hwB3UZQngH2IehTaapA8StM2OLMO8Ak5jpgWmKOkVdMHl5B+iBmx+mo17EfTFjg3UgeQBaFvfMqk4j7pYZG6wDdGNuME0dIq7FjA0GQOufKvtUY2wCv99A0W2j5PsNmoceKyDI/AU0ZFh9Cve+CxWeqBPH0AnU/ldiBd9XLSpvvYQ+kdlnUSkfKeSl9HxC91zoHEkLPtxYjEmP+pfK8mI74Cjz+I6LM9CLKQDF0ynNHkRHTP0WlZOk0LK5graqzFfe2ZJZQ+e6PrQI7T+f1AevuDphXkKdbsEfAZhbMfflGqAAAAAASUVORK5CYII=";

        static Resources()
        {
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(pluginImage)))
            {
                PluginImage = Image.FromStream(memoryStream);
                using (Bitmap bitmap = new Bitmap(PluginImage))
                    PluginIcon = Icon.FromHandle(bitmap.GetHicon());
            }
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(openFile)))
            {
                OpenFile = Image.FromStream(memoryStream);
            }
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(saveFile)))
            {
                SaveFile = Image.FromStream(memoryStream);
            }
        }
    }
}
