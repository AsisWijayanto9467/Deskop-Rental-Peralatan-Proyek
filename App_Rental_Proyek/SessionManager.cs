using App_Rental_Proyek.Model;
using System;
using System.Collections.Generic;
using System.Text;
using App_Rental_Proyek.Model;

namespace App_Rental_Proyek
{
    public static class SessionManager
    {
        public static UserModel CurrentUser { get; private set; }

        public static void SetCurrentUser(UserModel user)
        {
            CurrentUser = user;
        }

        public static bool IsLoggedIn => CurrentUser != null;

        public static ulong GetCurrentUserId()
        {
            return CurrentUser?.Id ?? 0;
        }

        public static string GetCurrentUserRole()
        {
            return CurrentUser?.Role ?? "guest";
        }

        public static void ClearSession()
        {
            CurrentUser = null;
        }
    }
}
