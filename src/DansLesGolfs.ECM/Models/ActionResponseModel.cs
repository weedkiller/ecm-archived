using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DansLesGolfs.ECM.Models
{
    public class ActionResponseModel
    {
        public string Status;
        public long Source_id;
        public long Target_id;

        public ActionResponseModel(string status, long source_id, long target_id)
        {
            Status = status;
            Source_id = source_id;
            Target_id = target_id;
        }
    }
}