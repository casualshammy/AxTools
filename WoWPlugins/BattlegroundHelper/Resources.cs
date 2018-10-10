using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlegroundHelper
{
    internal static class Resources
    {
        internal static readonly Image Plugin_Bg;
        private const string plugin_bg = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAMAUExURT8xLhYzViM6XTc3RRVDZCtGayNTdUcFAE8DAl0MBF8KDlkVBlshC1UoH2YEAmASBWEVD3YMBHQMBX4LAHAaB2cuF2s0FHEhB3cgD3AtGXozHGoyJ3owIH00J388KUI+RkRBNn5AIXhBL3JHPkNFTltOUFJTWVFVbFJiaFJtc1lofHxSQXJaUGJydndrcXVycXN4f0N/kW2YfBydnTCelyyhmj2hkjCzoTaztD/HrkuHpWSAmXWJhX+MiHeOkXuMmnmOmn6RlXiQnm6npmqrp3q8skXHumbMxXTY0IERCIgSAZ0RA4EiCYEkDoIuDoUoDYktC4o3HpMtDZskCJozB5o0B5A1FpA1HJQxHJE7G5g+HoQ+IKMPB6QNAqQPBqcUDK4aCLgfBqEjEa4vG6E3DaM7Cac9D6Q/DaU0Hq0/GLskCr8iC7EuEbUuE7YzD7g6F6A+IJZFHZ5THYZBJohDIYlGJolGK4BLMYVJM5BBJZVMKptGIpdZJ5dUMp5cPqZCF6NLHahAEaxFEaxFGqlIF65KF61KGLJJGLJLHrpIFLtKFL1PGr5TGr1SHK1DIKtPJahYM6xfNbxNIbpQKbpSLLpaJLRUObFbNrxYOqBnMKdrOopUTZdaS5xvUZBxV5Z4XJ9zVZp1XZl3Xp13WZ14W558XIFnY4FpaI56bod9dYt/dZd4Z512YJd+cqZsQKVtTKhwQKZ4Xax7V7VlQ7htQad8ZcwqCMMtEMAxDsA2FNE8CMFOF8FMGsVRHMhVGMtdHNZEGsNZIcJaIcNePdliD8xsLdBhINNrIdV5N+tQE+ZhDuJvFu9pF+psH7mEPJGGdpuJequRfbOLbbqMbL6KaLKRdvqGH+aCONeHRMCVe9GUZdOcat2tbdCmddineOSoWOq5V+i1YfPIXeTCdurIfvPKZ4GEgoCIiIKQm4WTnJ2hkJigmJ6jmKeYiaSdjrukg6atrp+9z6zNvZHX2Z7n27Lc4MOoldKvhdOtkd64hdy7lcfGveLGjObKksnm2tLl4ppTOFsAAAAJcEhZcwAACxIAAAsSAdLdfvwAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAARtJREFUKFMBEAHv/gClpCwNrdLb96ovKCctP+hAAKCh0B0r2d75+tEkMCrpQT4Asay1aHbU3fz9pi77PD3n5gCio5i4s7Dc4+R90/DsqairAJ+ywLrDfOHl3474OkPuna4Ar5fJym20zuLYluo0OET2mgCQU8zWYWN04IDaKTdGM0LVAI9vy8S3XWKZfqcGNTlIMe0AbLm+zV5cyHqRnAU2R/TzzwBgam62X0nXWXGVmzJF/vWeAEtrEkoOTcUcWoeE7/L/8SUAERMKBwhwcnOLT4V46zsEAQAYCRALD5QXiMdWVyImAiAAAFUUTkxQkwzCxr1Ydx8DW3UAgVKDZI15IcG/u2keIxuSFgBlZ1RmghV7jImKvFEaGX+GSdZ/gTVY0EQAAAAASUVORK5CYII=";
        
        static Resources()
        {
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(plugin_bg)))
            {
                Plugin_Bg = Image.FromStream(memoryStream);
            }
        }

    }
}
