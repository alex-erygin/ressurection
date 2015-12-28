// SignLib.h

#pragma once

namespace SignLib {

	public ref class Signer
	{
		// TODO: ����� ������� �������� ���� ������ ��� ����� ������.
	public:
          void Sign(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ signatureFileName);
		void Verify(System::Security::Cryptography::X509Certificates::X509Certificate2^ cert, System::String^ dataFileName);

     private:
          void Sign();
	};
}