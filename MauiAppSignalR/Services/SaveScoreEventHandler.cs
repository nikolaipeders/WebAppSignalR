using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppSignalR.Services
{
    public class SaveScoreEventHandler
    {
        public event EventHandler PlayerScoreUpdated;

        private static SaveScoreEventHandler _instance;

        public static SaveScoreEventHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveScoreEventHandler();
                }

                return _instance;
            }
        }

        private SaveScoreEventHandler()
        {
        }

        public void OnPlayerScoreUpdated()
        {
            PlayerScoreUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

}
