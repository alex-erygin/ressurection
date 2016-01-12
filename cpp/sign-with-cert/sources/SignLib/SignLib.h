// SignLib.h

#pragma once

namespace SignLib 
{
     ///<summary>
     /// ����������� ����� � ��������� ������� �����.
     ///</summary>
     public ref class Signer
     {
     public:
          ///<summary>
          /// ��������� ����.
          ///</summary>
          ///<param name="cert">���������� ��� �������.</param>
          ///<param name="sourceileName">���� � �����, ������� ���������� ���������.</param>
          ///<param name="sourceileName">���� � ��������� �����.</param>
          void Sign(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ sourceileName, System::String^ targetFileName);

          ///<summary>
          /// ��������� �������.
          ///</summary>
          ///<param name="cert">���������� ��� �������.</param>
          ///<param name="sourceileName">���� � �����, ������� ���������� ���������.</param>
          ///<param name="sourceileName">���� � ��������� �����.</param>
          void Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName);
     };
}