using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceLock
{
    class QuickSort
    {
        public static void partition(int[] ar)
        {
            int low = 0;
            int high = ar.Length - 1;
            QSort(low, high, ar);

        }
        public static void QSort(int low, int high, int[] ar)
        {
            int i = low+1;
            int j = high;
            int piviot = ar[low];
            while (i < j)
            {
                while (ar[i] < piviot && i< high)
                {
                    i++;
                }
                while (ar[j] > piviot && j>low)
                {
                    j--;
                }
                if (i < j)
                {
                    int temp = ar[i];
                    ar[i] = ar[j];
                    ar[j] = temp;
                    i++;
                    j--;
                }
                if (i == j) {
                    if (ar[j] > piviot)
                        j--;
                }
            }
            if (piviot > ar[j])
            {
                ar[low] = ar[j];
                ar[j] = piviot;
            }
            if (j - 1 > low)
            QSort(low, j - 1, ar);
            if (j + 1 < high)
            QSort(j + 1, high, ar);

        }
    }
}
