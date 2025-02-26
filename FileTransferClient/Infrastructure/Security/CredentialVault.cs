using System;
using System.Security;
using System.Security.Cryptography; // Add this using directive
using System.Text;

namespace FileTransferClient.Infrastructure.Security
{
    public static class CredentialVault
    {
        /// <summary>
        /// Encrypts a password using DPAPI.
        /// </summary>
        public static string EncryptPassword(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentNullException(nameof(plainText));

            byte[] encryptedData = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(plainText),
                null,
                DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts a password using DPAPI.
        /// </summary>
        public static string DecryptPassword(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                throw new ArgumentNullException(nameof(encryptedText));

            byte[] decryptedData = ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedText),
                null,
                DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// Converts a secure string to an encrypted string.
        /// </summary>
        public static string SecureStringToEncryptedString(SecureString secureString)
        {
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
            try
            {
                return EncryptPassword(System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
        }
        public static SecureString EncryptedStringToSecureString(string encryptedString)
        {
            var plainText = DecryptPassword(encryptedString);
            var secureString = new SecureString();
            foreach (char c in plainText)
                secureString.AppendChar(c);

            secureString.MakeReadOnly();
            return secureString;
        }
    }
}
