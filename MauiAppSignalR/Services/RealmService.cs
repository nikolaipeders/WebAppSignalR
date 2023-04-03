using Realms.Exceptions;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppSignalR.Services
{
    public class RealmService
    {
        private readonly string pathToDb = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder
                .LocalApplicationData), "TicTacToeDB");


        public RealmService()
        {
            var config = new RealmConfiguration(pathToDb + "my.realm");

            config.SchemaVersion = 2;

            Realm localRealm;
            try
            {
                localRealm = Realm.GetInstance(config);
            }
            catch (RealmFileAccessErrorException ex)
            {
                Console.WriteLine($@"Error creating or opening the
                realm file. {ex.Message}");
            }
        }
    }

}
