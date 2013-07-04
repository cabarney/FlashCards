using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.Model
{
    public class MathFact
    {
        public int Operand1 { get; set; }
        public int Operand2 { get; set; }
        public string Operation { get; set; }
        public int Resultant { get; set; }

        public int Priority { get; set; }

        public override string ToString()
        {
            return Operand1 + " " + Operation + " " + Operand2 + " = " + Resultant;
        }

        public string Summary { get { return this.ToString(); } }

        public static int CalculateResult(int op1, int op2, string op)
        {
            if (op == ((char)45).ToString()) op = "-";
            if (op == ((char)215).ToString()) op = "*";
            if (op == ((char)247).ToString()) op = "/";

            if (op == "+")
                return op1 + op2;
            if (op == "-")
                return op1 - op2;
            if (op == "*")
                return op1 * op2;
            if (op == "/")
                return op1 / op2;
            return 0;
        }
    }
}
