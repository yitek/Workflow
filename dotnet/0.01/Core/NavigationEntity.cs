using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class NavigationEntity
    {
        public Guid FromActionId { get; set; }
        public Guid ToActionId { get; set; }

        

        public Guid OwnActionId { get; set; }

        public Guid FlowId { get; set; }

        public string Graph { get; set; }
        public string Name { get; set; }

        public string NavigationType { get; set; }
        


        public string NextDealerId { get; set; }

        public string NextDealerName { get; set; }

        public string NextInputs { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public ActivityEntity FromActivityEntity { get; set; }

        public ActivityEntity ToActivityEntity { get; set; }
    }
}
