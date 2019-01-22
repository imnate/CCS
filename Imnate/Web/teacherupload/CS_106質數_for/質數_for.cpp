#include<iostream>

using namespace std;

bool prime(int prime);

int main()
{
	int n;
	bool s = true;

	cout<< "Please input an integer for listing all the primes less than the integer: ";
	cin>> n;
	cout<<"Primes less than "<< n <<" are ";
	for(int i = 2;i < n;i++){
		s = prime(i);
		if(s == true)
			cout<< i <<" ";
	}
	cout<<endl;
	system("pause");
	return 0;
}
bool prime(int prime)
{
	for(int i = 2;i <= prime;i++){
		if(prime == i)
			return true;
		 else if(prime%i == 0){
			return false;
			break;
		 }
	}
}