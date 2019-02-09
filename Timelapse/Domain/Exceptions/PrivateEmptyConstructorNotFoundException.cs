using System;

namespace Timelapse.Domain.Exceptions
{
    public class PrivateEmptyConstructorNotFoundException<T> : Exception
    {
        public PrivateEmptyConstructorNotFoundException() : base($"Unable to find any private constructor with 0 argument in type {typeof(T)}"){}
    }
}