using System;
namespace BobGreenhands.Utils
{
    /// <summary>
    /// [Class name might be subject to change]
    /// Simple class with an array and an int that points to an element in the array.
    /// This pointer can be incremented and decremented on demand, including wrapping around.
    /// <summary/>
    public class SelectableArray<T>
    {
        public T[]? Arr
        {
            get;
            private set;
        }

        public int Pointer
        {
            get;
            private set;
        }

        public bool WrapAround;

        public SelectableArray(T[] arr, int pointer, bool wrapAround)
        {
            Arr = arr;
            Pointer = pointer;
            WrapAround = wrapAround;
        }

        /// <summary>
        /// Returns the value that's currently being pointed at.
        /// <summary/>
        public T Get()
        {
            return Arr[Pointer];
        }

        /// <summary>
        /// Adds amount to the current pointer and wraps around, if WrapAround is true.
        /// <summary/>
        public T ModifyPointer(int amount)
        {
            if(WrapAround)
            {
                Pointer = (Pointer + amount) % Arr.Length;
                if(Pointer < 0)
                {
                    Pointer = Arr.Length + Pointer;
                }
            }
            else
            {
                Pointer = Math.Clamp(Pointer + amount, 0, Arr.Length - 1);
            }
            return Arr[Pointer];
        }

        /// <summary>
        /// Sets the Pointer to value.
        /// <summary/>
        public T SetPointer(int value)
        {
            Pointer = value % Arr.Length;
            return Arr[Pointer];
        }

    }
}