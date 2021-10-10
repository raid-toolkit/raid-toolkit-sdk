using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Raid.Model;
using SharedModel.Meta.Account;

namespace Raid.Service.DataModel
{
    public class Account
    {
        public string Id;
        public UserAvatarId Avatar;
        public string Name;
        public int Level;
        public int Power;
    }
}