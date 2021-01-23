﻿using Flow.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flow
{
    public class NextInfo
    {
        public NextInfo(Navigation nav,Node toNode, IReadOnlyDictionary<string, string> results) {
            this.Results = results;
            this.Association = nav.Association;
            
            this.FromActivity = nav.FromActivity;
            this.ToNode = toNode;
            
            
            string userName = null;
            if (!results.TryGetValue("dealerId", out string userId))
            {
                string userIdKey = this.Association.UserIdKey ?? this.Association.Name + "_DealerId";
                string userNameKey = this.Association.UserNameKey ?? this.Association.Name + "_DealerName";

                if (!results.TryGetValue(userIdKey, out userId))
                {
                    userId = this.FromActivity[userIdKey];
                    if (userId != null) userName = this.FromActivity[userNameKey];
                }
                else
                {
                    results.TryGetValue(userNameKey, out userName);
                }
            }
            else {
                results.TryGetValue("dealerName", out userName);
            }
            if (userId != null) {
                this.Dealer = new User() { Id = userId, Name = userName };
            }



        }
        public IReadOnlyDictionary<string, string> Results { get; set; }
       
        public IUser Dealer { get; set; }


        public Activity Activity {
            get;set;
        }
        public Node ToNode { get; set; }
        public Association Association { get; set; }

        public Activity FromActivity { get; }
    }
}
