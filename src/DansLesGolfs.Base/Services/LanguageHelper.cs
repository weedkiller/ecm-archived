using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base.Services
{
    public class LanguageHelper
    {
        private ResourceManager rm;

        public LanguageHelper(Type resourceType)
        {
            rm = new ResourceManager(resourceType);
        }

        public string GetString(string name)
        {
            return rm.GetString(name, CultureHelper.GetCurrentCultureInfo());
        }
    }
}
