// Cyclope.cpp: ���������� ���������������� ������� ��� ���������� DLL.
//

#include "stdafx.h"
#include "Cyclope.h"


// ������ ���������������� ����������
CYCLOPE_API int nCyclope=0;

// ������ ���������������� �������.
CYCLOPE_API int fnCyclope(void)
{
    return 42;
}

// ����������� ��� ����������������� ������.
// ��. ����������� ������ � Cyclope.h
CCyclope::CCyclope()
{
    return;
}
