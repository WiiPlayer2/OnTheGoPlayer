using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnTheGoPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    internal class ProfileRepository
    {
        #region Private Fields

        private const string PROFILE_FILE = "profiles.json";

        private static readonly string profileFile = Constants.GetDataPath(PROFILE_FILE);

        private readonly List<Profile> profiles;

        #endregion Private Fields

        #region Private Constructors

        private ProfileRepository()
        {
            if (File.Exists(profileFile))
                profiles = JToken.Parse(File.ReadAllText(profileFile)).ToObject<List<Profile>>();
            else
                profiles = new List<Profile>();
        }

        #endregion Private Constructors

        #region Public Properties

        public static ProfileRepository Instance { get; } = new ProfileRepository();

        #endregion Public Properties

        #region Public Methods

        public void Add(Profile profile)
        {
            profiles.Add(profile);
            Commit();
        }

        public IEnumerable<Profile> GetAll() => profiles;

        public void Remove(Profile profile)
        {
            profiles.Remove(profile);
            Commit();
        }

        #endregion Public Methods

        #region Private Methods

        private void Commit() => File.WriteAllText(profileFile, JToken.FromObject(profiles).ToString(Formatting.Indented));

        #endregion Private Methods
    }
}