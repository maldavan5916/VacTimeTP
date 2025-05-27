using DatabaseManager;
using System.Windows;

namespace VacTrack.Tools
{
    public static class AppSession
    {
        private const string CurrentUserKey = "CurrentUser";

        public static Users CurrentUser
        {
            get
            {
                if (Application.Current.Properties.TryGet(CurrentUserKey, out Users user))
                {
                    return user;
                }
                else
#if DEBUG
                {
                    System.Diagnostics.Debug.WriteLine(">>> Invalid or missing `CurrentUser` in application properties.");
                    return new Users { Login = "TestUsr", Password = string.Empty, Access = "rwa" };
                }
#else
                throw new ArgumentException("Invalid or missing `CurrentUser` in application properties.");
#endif
            }
            set => Application.Current.Properties[CurrentUserKey] = value;
        }

        public static bool CurrentUserIsReadOnly => CurrentUser.Access == "r";
    }
}
