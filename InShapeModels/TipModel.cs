using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class TipModel
    {
        public int Id { get; set; }

        public string Tip { get; set; }

        public bool IsDeleted { get; set; }

        public int StudioId { get; set; }

    }
}
