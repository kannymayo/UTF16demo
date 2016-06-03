// ConsoleApplication2.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>

int main()
{
	auto* x = L"汉𠜎𠜱𠝹𠱓";
	int cnt = 0;
	intptr_t startIndex = (intptr_t)&x[0];
	while (x[cnt] != NULL) {
		cnt++;
	}
	intptr_t endIndex = (intptr_t)&x[cnt];

	std::cout << startIndex << std::endl;
	std::cout << endIndex - startIndex << std::endl;

	std::cout << sizeof(wchar_t) << std::endl;

	scanf_s("%d");

    return 0;
}

