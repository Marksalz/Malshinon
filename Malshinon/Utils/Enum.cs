using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon
{
    /// <summary>
    /// Contains enumerations used throughout the Malshinon system.
    /// </summary>
    public class Enum
    {
        /// <summary>
        /// Represents the status of a person in the system.
        /// </summary>
        public enum Status
        {      
            Reporter = 1,            
            Target,           
            Both,
            PotentialAgent
        }
    }
}