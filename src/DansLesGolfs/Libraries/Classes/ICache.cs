using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DansLesGolfs
{
    /// <summary>
    /// Implement this interface as an abstraction of a Cache implementation
    /// </summary>
    public interface ICache<K, V>
    {
        /// <summary>    
        /// Get the object from the Cache    
        /// </summary>    
        /// <param name="key"></param>    
        /// <returns></returns>    
        V Get(K key);

        /// <summary>    
        /// Put the object in the cache    
        /// </summary>    
        /// <param name="key"></param>    
        /// <param name="value"></param>    
        void Put(K key, V value);

        /// <summary>    
        /// Remove an item from the Cache.    
        /// </summary>    
        /// <param name="key">The Key of the Item in the Cache to remove.</param>    
        void Remove(K key);

        /// <summary>    
        /// Clear the Cache    
        /// </summary>    
        void Clear();
    }
}
