using System;
using System.Collections.Immutable;
using System.IO;
using Discord;
using Newtonsoft.Json;
using Yuki.Bot.Misc;

namespace Yuki.Bot.Entities
{
    public class BotCredentials
    {
        public string Token { get; }
        public string CatApiKey { get; }
        public string FirebaseKey { get; }
        public string EncryptionKey { get; }

        public ImmutableArray<ulong> OwnerIds { get; }

        private readonly string _credsFileName;

        public BotCredentials()
        {
            _credsFileName = FileDirectories.AppDataDirectory + "credentials.json";
            File.WriteAllText(FileDirectories.AppDataDirectory + "credentials_example.json", JsonConvert.SerializeObject(new CredentialsModel(), Formatting.Indented));

            try
            {
                //Deserialize the JSON credentials file
                CredentialsModel credentials = JsonConvert.DeserializeObject<CredentialsModel>(File.ReadAllText(_credsFileName));
                
                Token = credentials.Token;
                CatApiKey = credentials.CatApiKey;
                FirebaseKey = credentials.FirebaseKey;
                EncryptionKey = credentials.EncryptionKey;

                OwnerIds = credentials.OwnerIds.ToImmutableArray();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private class CredentialsModel
        {
            public string Token { get; set; } = "";
            public string CatApiKey { get; set; } = "API_KEY";
            public string FirebaseKey { get; set; } = "FIREBASE_KEY";
            public string EncryptionKey { get; set; } = "ENCRYPT_KEY";
            public ulong[] OwnerIds { get; set; } = new ulong[1];
        }

        public bool IsOwner(IUser u) => OwnerIds.Contains(u.Id);
    }
}
