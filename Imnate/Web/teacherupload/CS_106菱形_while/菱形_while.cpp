#include<iostream>
using namespace std;

int main()
{
	int count = 0;
	cout << "Please input an integer for listing a rhombus with n asterisks in each side"<<endl;
	cout <<": ";
	cin >> count;

	for (int i = count;i>0;i--)
	{
		for (int s = 0; s < i - 1; s++)
		{
			cout << " ";
		}
		int str=count - i;
		for (int j= 1;j<=str*2+1;j++)
		{
			cout << "*";
		}
		cout << endl;
	}

	for (int i = 1; i<count; i++)
	{
		for (int s = 0; s <= i - 1; s++)
		{
			cout << " ";
		}
		int str = count - i;
		for (int j = 1; j <= str * 2 -1; j++)
		{
			cout << "*";
		}
		cout << endl;
	}
	system("pause");
	return 0;
}