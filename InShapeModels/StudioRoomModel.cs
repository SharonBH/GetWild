using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class StudioRoomModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int MaxParticipants { get; set; }

        public string Picture { get; set; }

        public int StudioId { get; set; }

    }
}
