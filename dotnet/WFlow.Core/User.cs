using WFlow.Repositories;
using System;
using System.Collections.Generic;

using WFlow.Entities;

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

        public readonly static User Anyone = new User(string.Empty,"[Anyone]");

        public static User Creator(ActivityEntity entity) {
            
            return string.IsNullOrEmpty(entity.CreatorId)?null:new User(entity.CreatorId, entity.CreatorName);
        }

        public static IUser Creator(ActivityEntity entity,IUser value)
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                entity.CreatorId = null;
                entity.CreatorName = null;
                return null;
            }
            else {
                entity.CreatorId = value.Id;
                entity.CreatorName = value.Name;
                return value;
            }
            
        }

        public static User Owner(ActivityEntity entity)
        {
            return string.IsNullOrEmpty(entity.OwnerId) ? null : new User(entity.OwnerId, entity.OwnerName);
        }

        public static IUser Owner(ActivityEntity entity, IUser value)
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                entity.OwnerId = null;
                entity.OwnerName = null;
                return null;
            }
            else
            {
                entity.OwnerId = value.Id;
                entity.OwnerName = value.Name;
                return value;
            }
        }


        public static User Dealer(ActivityEntity entity)
        {
            return string.IsNullOrEmpty(entity.DealerId) ? null : new User(entity.DealerId,entity.DealerName);
        }

        public static IUser Dealer(ActivityEntity entity,IUser value)
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                entity.DealerId = null;
                entity.DealerName = null;
                return null;
            }
            else
            {
                entity.DealerId = value.Id;
                entity.DealerName = value.Name;
                return value;
            }
        }

        public static User Closer(ActivityEntity entity)
        {
            return string.IsNullOrEmpty(entity.CloserId) ? null : new User(entity.CloserId, entity.CloserId);
        }

        public static IUser Closer(ActivityEntity entity, IUser value)
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                entity.CloserId = null;
                entity.CloserName = null;
                return null;
            }
            else
            {
                entity.CloserId = value.Id;
                entity.CloserName = value.Name;
                return value;
            }
        }


    }
}
