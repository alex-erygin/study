// Cyclope.cpp: определяет экспортированные функции для приложения DLL.
//

#include "stdafx.h"
#include "Cyclope.h"


// Пример экспортированной переменной
CYCLOPE_API int nCyclope=0;

// Пример экспортированной функции.
CYCLOPE_API int fnCyclope(void)
{
    return 42;
}

// Конструктор для экспортированного класса.
// см. определение класса в Cyclope.h
CCyclope::CCyclope()
{
    return;
}
