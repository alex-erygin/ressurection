// SignLib.h

#pragma once

namespace SignLib {

	public ref class Signer
	{
		// TODO: ����� ������� �������� ���� ������ ��� ����� ������.
	public:
          void Sign(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ sourceileName, System::String^ targetFileName);
		void Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName);

     private:
          void Sign();
	};
}