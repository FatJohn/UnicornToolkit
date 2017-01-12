// Copyright (c) 2016 John Shu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Unicorn
{
    public class CryptographyService : ICryptographyService
    {
        public string HashMD5(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var algorithm = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(source, BinaryStringEncoding.Utf8);
            var hashedBuffer = algorithm.HashData(buffer);
            var result = CryptographicBuffer.EncodeToHexString(hashedBuffer);
            return result;
        }

        public byte[] HashMD5(byte[] source)
        {
            if (source == null)
            {
                return null;
            }

            var algorithm = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var buffer = CryptographicBuffer.CreateFromByteArray(source);
            var hashedBuffer = algorithm.HashData(buffer);
            byte[] result = null;
            CryptographicBuffer.CopyToByteArray(hashedBuffer, out result);
            return result;
        }

        public string EncryptAES(string plainText, string key, string salt)
        {
            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);
            IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            // using salt and 1000 iterations
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(keyBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(keyBuffer);

            // derive buffer to be used for encryption salt from derived password key
            IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            // display the buffers – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
            string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            // create symmetric key from derived password key
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            // encrypt data buffer using symmetric key and derived salt material
            IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, saltMaterial);
            var result = CryptographicBuffer.EncodeToBase64String(resultBuffer);

            return result;
        }

        public string DecryptAES(string encryptedData, string key, string salt)
        {
            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);
            IBuffer cipherBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedData);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            // using salt and 1000 iterations
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(keyBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(keyBuffer);

            // derive buffer to be used for encryption salt from derived password key
            IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            // display the keys – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
            string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            // create symmetric key from derived password material
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            // encrypt data buffer using symmetric key and derived salt material
            IBuffer resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, saltMaterial);
            string result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16LE, resultBuffer);
            return result;
        }

        public string HashSHA1(string source)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(source, BinaryStringEncoding.Utf8);
            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hashBuffer = hashAlgorithm.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hashBuffer);
        }
    }
}
