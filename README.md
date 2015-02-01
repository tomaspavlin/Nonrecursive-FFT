# Nonrecursive-FFT
Program implements recursive and nonrecursive form of Fast Fourier Transform and mul operation of polynoms.

###Class FFT
The core class of the program is FFT. It consists of three methods:
```
Polynom RecursiveFFT(Polynom p, Complex omega)
Polynom NonrecursiveFFT(Polynom p)
Polynom NonrecursiveFFT(Polynom p, bool reversed)
```
The description of them is in *.cs* file.

###Class Polynom
Class representing polynom. The core method of this class is:
```
Polynom operator *(Polynom p, Polynom q)
```
The description of the method is in *.cs* file.

###Examples
There are 5 examples in *Program.cs* file how the mul operator works.
