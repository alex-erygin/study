// SignLib.h

#pragma once

namespace SignLib 
{
     ///<summary>
     /// Подписывает файлы и проверяет подпись файла.
     ///</summary>
     public ref class Signer
     {
     public:
          ///<summary>
          /// Подписать файл.
          ///</summary>
          ///<param name="cert">Сертификат для подписи.</param>
          ///<param name="sourceileName">Путь к файлу, который необходимо подписать.</param>
          ///<param name="sourceileName">Путь к выходному файлу.</param>
          void Sign(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ sourceileName, System::String^ targetFileName);

          ///<summary>
          /// Проверить подпись.
          ///</summary>
          ///<param name="cert">Сертификат для подписи.</param>
          ///<param name="sourceileName">Путь к файлу, который необходимо подписать.</param>
          ///<param name="sourceileName">Путь к выходному файлу.</param>
          void Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName);
     };
}