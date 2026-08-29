using App_Rental_Proyek.Model;

namespace App_Rental_Proyek.Helper
{
    public static class Session
    {
        // =========================================================
        // USER YANG SEDANG LOGIN
        // =========================================================

        private static UserModel _currentUser;

        public static UserModel CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public static bool IsLoggedIn => _currentUser != null;

        public static void Clear()
        {
            _currentUser = null;
        }
    }
}
