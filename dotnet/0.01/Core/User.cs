using WFlow.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class User:IUser
    {
        public User(string id, string name) {
            this.Id = id;this.Name = name;
        }
        public string Id { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return this.Name + "#" + this.Id;
        }

        public static User GetCreator(ActivityEntity entity) {
            return new User(entity.CreatorId, entity.CreatorName);
        }

        public static User GetDealer(ActivityEntity entity)
        {
            return new User(entity.DealerId,entity.DealerName);
        }

        public static User GetCloser(ActivityEntity entity)
        {
            return new User(entity.CloserId, entity.CloserId);
        }

        
    }
}
