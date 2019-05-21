using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// A YouTube ASH.
    /// </summary>
    public class Yash
    {
        /// <summary>
        /// The sums of this yash.
        /// </summary>
        public List<float> Sums;

        /// <summary>
        /// The duration of the song this yash belongs to.
        /// </summary>
        public float Duration; // in seconds

        public Yash()
        {

        }

        public Yash(List<float> sums, float duration)
        {
            Sums = sums;
            Duration = duration;
        } 
    
    }
}
