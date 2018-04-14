using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public static class Diagnostic
    {

        /// <summary>
        /// usage Debug.WriteLine(System.Diagnostic.Tracer(r));
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Tracer(params object[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod(); //System.Reflection.MethodBase.GetCurrentMethod().Name;
            sb.Append(methodBase.Name + "(");

            ParameterInfo[] methodParameters = methodBase.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                if(i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(methodParameters[i].Name + "=" + parameters[i].ToString());
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}