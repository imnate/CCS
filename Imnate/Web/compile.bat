@echo off
cd /d C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC
call vcvarsall
C:
cd C:\Users\Imnate\Imnate\Web\Upload\%1
cl /EHsc %2
exit