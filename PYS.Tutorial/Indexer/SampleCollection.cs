using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PYS.Tutorial.Indexer
{
    class SampleCollection<T>
    {
        // Declcare an array to store the data elements
        private T[] arr = new T[100];

        // Define the indexer, which will allow client code to use [] notion on the class intance itself. (sele line 2code in Main below)
        public T this[int i]
        {
            get
            {
                // This indexer is very simple, and just returns or sets the corresponding element from the internal array.
                return arr[i];
            }
            set
            {
                arr[i] = value;
            }

        }

    }
}


/*
 
 索引器允许类或结构的实例就像数组一样进行索引。 索引器类似于属性，不同之处在于索引器的取值函数采用参数。

 */