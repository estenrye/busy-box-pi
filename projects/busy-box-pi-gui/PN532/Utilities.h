#include "pch.h"

#pragma once
#ifndef UTILS
#define UTILS
class Utilities
{
public:
	Utilities();
	void print(char* text);
	void println(char* text);
	void print(byte* body, char* formatString);
	void print(byte body, char* formatString);
	void println();
	void print(char text);
	void println(byte text);
	void println(byte text, char* formatString);
	~Utilities();
};

#endif