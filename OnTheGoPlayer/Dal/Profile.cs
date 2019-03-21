using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    internal class Profile
    {
        #region Public Properties

        public Guid InterfaceID { get; set; }

        public virtual JToken ProfileData { get; set; }

        public string SubTitle { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        #endregion Public Properties
    }

    internal class Profile<TProfileData> : Profile
    {
        #region Private Fields

        private TProfileData profileData;

        #endregion Private Fields

        #region Public Properties

        public new TProfileData ProfileData
        {
            get => profileData;
            set
            {
                profileData = value;
                base.ProfileData = JToken.FromObject(value);
            }
        }

        #endregion Public Properties
    }
}