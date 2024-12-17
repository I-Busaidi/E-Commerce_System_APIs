using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
namespace E_Commerce_System
{
    public class PasswordHasher
    {
        public static string GetHashedPassword(string password)
        {
            return HashPassword(password);
        }

        public static bool GetPasswordVerification(string enteredPassword, string storedHashedPassword)
        {
            return VerifyPassword(enteredPassword, storedHashedPassword);
        }

        private static string HashPassword(string password)
        {
            // Converting password to bytes.
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Setting parameters for Argon2 hashing.
            int iterations = 3;
            int memoryCost = 65536;
            int parallelism = 4;

            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.Iterations = iterations;
                argon2.MemorySize = memoryCost;
                argon2.DegreeOfParallelism = parallelism;
                argon2.Salt = GenerateRandomSalt(16);

                byte[] hash = argon2.GetBytes(32);

                byte[] hashWithSalt = new byte[argon2.Salt.Length + hash.Length];
                Buffer.BlockCopy(argon2.Salt, 0, hashWithSalt, 0, argon2.Salt.Length);
                Buffer.BlockCopy(hash, 0, hashWithSalt, argon2.Salt.Length, hash.Length);

                return Convert.ToBase64String(hashWithSalt);
            }
        }

        private static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] storedHashWithSalt = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Buffer.BlockCopy(storedHashWithSalt, 0, salt, 0, 16);

            byte[] storedHashBytes = new byte[storedHashWithSalt.Length - 16];
            Buffer.BlockCopy(storedHashWithSalt, 16, storedHashBytes, 0, storedHashBytes.Length);

            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            using (var argon2 = new Argon2id(enteredPasswordBytes))
            {
                argon2.Iterations = 3;
                argon2.MemorySize = 65536;
                argon2.DegreeOfParallelism = 4;
                argon2.Salt = salt;

                byte[] enteredHashBytes = argon2.GetBytes(32);  // Generate the hash for the entered password

                // Compare the newly generated hash with the stored hash
                return CompareHashes(enteredHashBytes, storedHashBytes);
            }
        }

        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            for (int i = 0; i < hash1.Length; i++)
            {
                if(hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] GenerateRandomSalt(int length)
        {
            byte[] salt = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
