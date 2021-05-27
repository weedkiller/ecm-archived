using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base
{
    public class StringHelper
    {
        public static string RandomString(int length = 5, bool toUpper = true)
        {
            string randomString = Guid.NewGuid().ToString().Replace("-", "");
            randomString = randomString.Substring(0, length > 0 && length <= randomString.Length ? length : randomString.Length);
            if(toUpper)
            {
                randomString = randomString.ToUpper();
            }
            return randomString;
        }
    }
}
