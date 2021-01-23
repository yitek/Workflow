using Flow.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class User:IUser
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name + "#" + this.Id;
        }

        public static User GetCreator(ActivityEntity entity) {
            return new User()
            {
                Id= entity.CreatorId,
                Name = entity.CreatorName
            };
        }

        public static User GetDealer(ActivityEntity entity)
        {
            return new User()
            {
                Id = entity.DealerId,
                Name = entity.DealerName
            };
        }
    }
}
