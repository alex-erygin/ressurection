// SignLib.h

#pragma once

#define CERT_PERSONAL_STORE_NAME  L�My�
#define CERT_OTHER_PEOPLE_STORE_NAME L�AddressBook�
#define MY_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)
#define BUFSIZE 1024

using namespace System;

namespace SignLib {

	public ref class Signer
	{
		// TODO: ����� ������� �������� ���� ������ ��� ����� ������.
	public:
          void Sign(System::String^ signerName, System::String^ fileName, System::String^ signatureFileName);
		void Verify(System::String^ signerName, System::String^ signatureFileName, System::String^ dataFileName);
	};
}