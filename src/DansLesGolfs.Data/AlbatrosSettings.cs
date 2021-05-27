using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DansLesGolfs.Data
{
    public class AlbatrosSettings
    {
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Protocol { get; set; }
    }

}
