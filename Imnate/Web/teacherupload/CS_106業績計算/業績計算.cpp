//1011357_¹ù«aµ{_HW5·~ÁZ­pºâ
#include<iostream>
#include<iomanip>
using namespace std;

int main(){
	int sales_person = 4;
	int product = 5;
	double sales_Amount = 0;
	double product_Amount = 0;
	double sales[4][5] = {0};
	double value;
		cout << "Enter the salesperson (1 - 4), product number (1 - 5), and total sales."<<endl;
		cout << "Enter -1 for the salesperson to end input."<<endl;
		cin >> sales_person;
		while (sales_person != -1)
		{
			if (sales_person > 4 || sales_person < 0)
			{
				cin >> product >> value;
				cin >> sales_person;
			}
			else{
				cin >> product >> value;
				sales[sales_person - 1][product - 1] = value;
				cin >> sales_person;
			}
		}

		cout <<endl<<"The total sales for each salesperson are displayed at the end of each row,"<<endl<<"and the total sales for each product are displayed at the bottom of each"<<endl<<"column."<<endl;
		cout << setw(13) << "1";
		for (int i = 1; i < 6; i++)
		{
			if (i < 5)
				cout << setw(12) << i + 1;
			else if (i >= 5)
				cout << setw(12) << "Total"<<endl;
		}
		for (int i = 0; i < 4; i++)
		{
			if (i < 4)
			cout << i + 1;

			for (int j = 0; j < 5; j++)
			{
					cout << setw(12) << fixed << setprecision(2) << sales[i][j];
					sales_Amount += sales[i][j];
				if (j == 4)
				{
					cout << setw(12) << fixed << setprecision(2) << sales_Amount;
				}
			}
			cout << endl;
			sales_Amount = 0;
		}

		cout <<endl<<"Total";
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				product_Amount += sales[j][i];
			}
			if (i==0)
				cout << setw(8) << fixed << setprecision(2) << product_Amount;
			else
				cout << setw(12) << fixed << setprecision(2) << product_Amount;
			product_Amount = 0;
		}
		cout << endl;
		system("pause");
}