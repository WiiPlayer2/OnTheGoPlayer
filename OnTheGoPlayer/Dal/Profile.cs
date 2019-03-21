using Newtonsoft.Json.Linq;
using NullGuard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Dal
{
    [DebuggerDisplay("{GetHashCode()}")]
    public class Profile
    {
        #region Public Properties

        public Guid InterfaceID { get; set; }

        [AllowNull]
        public JToken ProfileData { get; set; }

        public string SubTitle { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is Profile profile))
                return false;

            return InterfaceID == profile.InterfaceID
                && JToken.DeepEquals(ProfileData ?? JValue.CreateNull(), profile.ProfileData ?? JValue.CreateNull());
        }

        public override int GetHashCode()
        {
            return InterfaceID.GetHashCode() ^ (ProfileData?.GetHashCode() ?? 0);
        }

        #endregion Public Methods
    }

    internal class Profile<TProfileData> : Profile
    {
        #region Private Fields

        private TProfileData profileData;

        #endregion Private Fields

        #region Public Properties

        [AllowNull]
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