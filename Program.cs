using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FFT {
    class Program {
        static void Main(string[] args) {
            Polynom p, q, r;

            // Example 1
            p = new Polynom((Complex)(-2)); // -2
            q = new Polynom(15,0,0); // 15 + 0x + 0x^2
            r = p * q;
            Console.WriteLine("Example {0}:\np =\n{1}\nq =\n{2}\np*q =\n{3}", 1, p, q, r);

            // Example 2
            p = new Polynom(Complex.ImaginaryOne); // i
            q = new Polynom((Complex)3); // 3
            r = p * q;
            Console.WriteLine("Example {0}:\np =\n{1}\nq =\n{2}\np*q =\n{3}", 2, p, q, r);

            // Example 3
            p = new Polynom(1, 3); // 1 + 3x
            q = new Polynom(-2, 1); // -2 + x
            r = p * q;
            Console.WriteLine("Example {0}:\np =\n{1}\nq =\n{2}\np*q =\n{3}", 3, p, q, r);

            // Example 4
            // The same result in WolframAlpha:
            // http://www.wolframalpha.com/input/?i=%28%282%2B4i%29+%2B+%282%2B8i%29x+%2B+%282%2B3i%29x%5E2%29+*+%282+%2B+%287+%2B+3i%29x%29
            p = new Polynom(new Complex(2,4),new Complex(2,8),new Complex(2,3));
            q = new Polynom(new Complex(2,0),new Complex(7,3));
            r = p * q;
            Console.WriteLine("Example {0}:\np =\n{1}\nq =\n{2}\np*q =\n{3}", 4, p, q, r);

            // Example 5
            // In this case the rank of the result is not pow of 2
            p = new Polynom(1,2);
            q = new Polynom(1,2,3,4,5);
            r = p * q;
            Console.WriteLine("Example {0}:\np =\n{1}\nq =\n{2}\np*q =\n{3}", 4, p, q, r);
        }

    }
}
