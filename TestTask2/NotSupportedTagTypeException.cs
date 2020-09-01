using System ;

namespace TestTask2
{
    internal class NotSupportedTagTypeException : Exception
	{
		public NotSupportedTagTypeException() { }
		public NotSupportedTagTypeException(string message) : base(message) { }
		public NotSupportedTagTypeException(string message, Exception inner) : base(message, inner) { }
	}
}
