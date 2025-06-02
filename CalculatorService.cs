using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary
{
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2) {
            double result = n1 + n2;
            Console.WriteLine("Received Add({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double n1, double n2) {
            double result = n1 - n2;
            Console.WriteLine("Received Subtract({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double n1, double n2) {
            double result = n1 * n2;
            Console.WriteLine("Received Multiply({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double n1, double n2) {
            double result = n1 / n2;
            Console.WriteLine("Received Divide({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Power(double baseNumber, double exponent)
        {
            double result = Math.Pow(baseNumber, exponent);
            Console.WriteLine("Received Power({0},{1})", baseNumber, exponent);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double SquareRoot(double number)
        {
            double result = Math.Sqrt(number);
            Console.WriteLine("Received SquareRoot({0})", number);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Modulo(double dividend, double divisor)
        {
            double result = dividend % divisor;
            Console.WriteLine("Received Modulo({0},{1})", dividend, divisor);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public string GetData(int value) {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite) {
            if (composite == null) {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue) {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
