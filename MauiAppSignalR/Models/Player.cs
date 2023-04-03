using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppSignalR.Models
{
    public partial class Player : IRealmObject
    {
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

    }
}
