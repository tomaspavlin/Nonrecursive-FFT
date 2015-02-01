using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FFT {
    public class FFT {
        public static Polynom NonrecursiveFFT(Polynom p){
            return NonrecursiveFFT(p.ComplementWithNulls(),false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">Polynom. Must have pow of 2 member. Maybe you can use .ComplementWithNulls()</param>
        /// <param name="reversed">If true, the method will make reversed FFT (interpolation)</param>
        /// <returns></returns>
        public static Polynom NonrecursiveFFT(Polynom p,bool reversed) {
            int layersCount = (int) Math.Ceiling(Math.Log(p.Count,2));
            if (Math.Pow(2, layersCount) != p.Count) throw new ArgumentException("Polynom must have pow of 2 members. Maybe you want to use .ComplementWithNulls() methdo.");

            //Permuts coefficients of p so they would suit the coefficients in the last layer of FFT
            Polynom new_p = new Polynom(p.Count);
            for (int i = 0; i < p.Count; i++) {
                int pom_i = i;
                int new_i = 0;
                int s = p.Count / 2;
                for (int l = 0; l < layersCount; l++) {
                    if (pom_i % 2 == 1) {
                        pom_i--;
                        new_i += s;
                    }
                    pom_i /= 2;
                    s /= 2;
                }
                new_p[new_i] = p[i];
            }
            p = new_p;

            for (int layer = 1; layer <= layersCount; layer++) { //
                int nodeSize = (int)Math.Round(Math.Pow(2, layer));
                int nodeCount = p.Count / nodeSize; //division is integer

                Complex omega = ComplexUtils.GetSqrtOfOne(nodeSize);
                if (reversed) omega = 1 / omega;

                new_p = new Polynom(p.Count);
                for (int nodeI = 0; nodeI < nodeCount; nodeI++) {
                    Complex x = 1;
                    for (int i = 0; i < nodeSize / 2; i++) {
                        Complex c1 = p[nodeI * nodeSize + i];
                        Complex c2 = p[nodeI * nodeSize + i + nodeSize / 2];

                        new_p[nodeI * nodeSize + i] = c1 + x * c2;
                        new_p[nodeI * nodeSize + i + nodeSize / 2] = c1 - x * c2;

                        x *= omega;
                    }
                }
                p = new_p;

                //Uncomment to see how the algorithm works
                //Console.WriteLine("Layer {0}:", layer);
                //Console.WriteLine(p);
                //Console.WriteLine();
            }
            return p;
        }
        /// <summary>
        /// Make recursive FFT
        /// </summary>
        /// <param name="a">Polynom. Number of items must be pow of two</param>
        /// <param name="omega">Proper sqrt of 1</param>
        /// <returns></returns>
        public static Polynom RecursiveFFT(Polynom a, Complex omega) {
            if (ComplexUtils.IsAlmostEqual(omega,1)) return a;
            Polynom a_s = new Polynom();
            Polynom a_l = new Polynom();
            for (int i = 0; i < a.Count / 2; i++) {
                a_s.Add(a[2 * i]);
                a_l.Add(a[2 * i + 1]);
            }

            Polynom s = RecursiveFFT(a_s, omega * omega);
            Polynom l = RecursiveFFT(a_l, omega * omega);
            
            Complex x = 1;
            Polynom ret = new Polynom(a.Count);
            for (int i = 0; i < a.Count / 2; i++) {
                ret[i] = s[i] + x * l[i];
                ret[i + a.Count/2] = s[i] - x * l[i];
                x *= omega;
            }
            return ret;
        }

    }
    public class Polynom : List<Complex> {
        // Remove null members at the end
        public void Trim() {
            while (this.Count > 0 && ComplexUtils.IsAlmostEqual(this[this.Count - 1], 0)) {
                this.RemoveAt(this.Count - 1);
            }
        }
        public Polynom() { }
        /// <summary>
        /// Create polynom with complex coeficients
        /// </summary>
        /// <param name="arr">Coeficients. For example {a,b,c} creates polynom a+bx+cx^2</param>
        public Polynom(params Complex[] arr) : base(arr){}
        public Polynom(int count) {
            for (int i = 0; i < count; i++ )
                this.Add(0);
        }
        public override string ToString() {
            StringBuilder ret = new StringBuilder();
            int i = 0;
            foreach (Complex c in this) {
                ret.Append(i + ": ");
                ret.AppendLine(ComplexUtils.ToNiceString(c));
                i++;
            }
            return ret.ToString();
        }
        public static Polynom operator /(Polynom p, int n){
            for (int i = 0; i < p.Count; i++)
                p[i] /= n;
            return p;
        }
        public Polynom Clone() {
            return new Polynom(this.ToArray());
        }
        /// <summary>
        /// Complements polynom with nulls so rank would be pow of 2
        /// </summary>
        /// <returns>New polynom</returns>
        public Polynom ComplementWithNulls() {
            Polynom ret = this.Clone();
            int l = (int)Math.Ceiling(Math.Log(ret.Count, 2));
            while (ret.Count < Math.Pow(2,l)) {
                ret.Add(0);
            }
            return ret;
        }
        /// <summary>
        /// Return multiplication of two polynoms using nonrecursive FFT
        /// </summary>
        /// <param name="p">First polynom</param>
        /// <param name="q">Second polynom</param>
        /// <returns>p * q</returns>
        public static Polynom operator *(Polynom p, Polynom q) {
            p = p.Clone();
            q = q.Clone();
            p.Trim();
            q.Trim();

            //set rank of mul
            int count = p.Count + q.Count - 1;
            if (p.Count == 0 || q.Count == 0) throw new ArgumentException("Polynom must have at least one member.");

            // So FFT returns polynom with bigger rank
            while (p.Count < count) {
                p.Add(0);
                p = p.ComplementWithNulls();
            }
            while (q.Count < count) {
                q.Add(0);
                q = q.ComplementWithNulls();
            }

            // Nonrecursive FFT
            Polynom p_vals = FFT.NonrecursiveFFT(p);
            Polynom q_vals = FFT.NonrecursiveFFT(q);

            Polynom r_vals = new Polynom(count).ComplementWithNulls();
            
            count = r_vals.Count;
            for (int i = 0; i < count; i++ ) {
                Complex a = p_vals[i];
                Complex b = q_vals[i];

                r_vals[i] = a * b;
            }

            Polynom ret = FFT.NonrecursiveFFT(r_vals, true)/count;
            ret.Trim();
            return ret;
        }
    }
    public static class ComplexUtils {
        public static Complex GetSqrtOfOne(int n) {
            return Complex.Exp((2 * Math.PI * Complex.ImaginaryOne) / n);
        }
        public static bool IsAlmostEqual(Complex c, Complex c2) {//HACK
            return RoundComplex(c) == RoundComplex(c2);
        }
        public static Complex RoundComplex(Complex c) {
            double r = Math.Round(c.Real, 10);
            double im = Math.Round(c.Imaginary, 10);
            return new Complex(r, im);
        }
        public static string ToNiceString(Complex c) {
            Complex cr = ComplexUtils.RoundComplex(c);
            double r = cr.Real;
            double i = cr.Imaginary;
            if (i == 0)
                return string.Format("{0}", r, i);
            else if (r == 0)
                return string.Format("{1}i", r, i);
            else if (i < 0)
                return string.Format("{0} - {1}i", r, -i);
            else
                return string.Format("{0} + {1}i", r, i);
        }
    }
}
