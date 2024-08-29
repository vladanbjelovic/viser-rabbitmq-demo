using NC3.Comm.Core.Layer.Helpers;
using System;

namespace Microservice_B.Remote
{
    public partial class RemoteFunctions
    {
        public double Add(string valueA, string valueB)
        {
			try
			{
				double a = Convert.ToDouble(valueA);
				double b = Convert.ToDouble(valueB);

				var res = a+ b;

				return res;
            }
			catch (Exception e)
			{
                throw new RPCException("Unable to calculate. Error: " + e.Message, RPCExceptionType.Custom);
			}
        }

        public double Multiply(string valueA, string valueB)
        {
            try
            {
                double a = Convert.ToDouble(valueA);
                double b = Convert.ToDouble(valueB);

                var res = a * b;

                return res;
            }
            catch (Exception e)
            {
                throw new RPCException("Unable to calculate. Error: " + e.Message, RPCExceptionType.Custom);
            }
        }

        public double Substract(string valueA, string valueB)
        {
            try
            {
                double a = Convert.ToDouble(valueA);
                double b = Convert.ToDouble(valueB);

                var res = a - b;

                return res;
            }
            catch (Exception e)
            {
                throw new RPCException("Unable to calculate. Error: " + e.Message, RPCExceptionType.Custom);
            }
        }

        public double Divide(string valueA, string valueB)
        {
            try
            {
                double a = Convert.ToDouble(valueA);
                double b = Convert.ToDouble(valueB);

                var res = a / b;

                return res;
            }
            catch (Exception e)
            {
                throw new RPCException("Unable to calculate. Error: " + e.Message, RPCExceptionType.Custom);
            }
        }
    }
}
