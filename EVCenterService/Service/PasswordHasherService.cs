using System.Security.Cryptography;

namespace EVCenterService.Service
{
    public class PasswordHasherService
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public string Hash(string password)
        {
            // Tạo salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Băm mật khẩu
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Kết hợp salt và hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Chuyển thành chuỗi Base64 để lưu vào database
            return Convert.ToBase64String(hashBytes);
        }

        public bool Verify(string password, string hashedPassword)
        {
            // Lấy hashBytes từ chuỗi Base64
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Lấy salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Băm mật khẩu được cung cấp
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // So sánh kết quả
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
