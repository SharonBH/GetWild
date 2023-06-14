using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels
{
    public class UsersSummaryModel
    {
        public ParticipantType UserType { get; set; }

        public string TypeName { get { return UserType.GetDisplayName(); } }

        public int Frozen { get; set; }

        public int Total { get; set; }

        public int Active { get; set; }
        public int ActiveTickets { get; set; }

        public int ActiveSubscription { get { return Active - ActiveTickets; } }

        public int Paying { get; set; }

        public int NonPaying { get { return Active - Paying; } }

        public int InActive { get { return Total - Active; } }

        public string CssActive { get { return Utils.GetColorforTypeActive(Enum.GetName(typeof(ParticipantType), UserType), true); } }

        public string CssInActive { get { return Utils.GetColorforTypeActive(Enum.GetName(typeof(ParticipantType), UserType), false); } }

        public string CssFrozen { get { return Utils.GetColorforTypeActive("Frozen", false); } }

    }
}
