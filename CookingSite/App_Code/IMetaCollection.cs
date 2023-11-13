using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public enum GenerateOption
    {
        /// <summary>
        /// Do not generate additional sets, typical implementation.
        /// </summary>
        WithoutRepetition,
        /// <summary>
        /// Generate additional sets even if repetition is required.
        /// </summary>
        WithRepetition
    }

    interface IMetaCollection<T> : IEnumerable<IList<T>>
    {
        /// <summary>
        /// The count of items in the collection.  This is not inherited from
        /// ICollection since this meta-collection cannot be extended by users.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// The type of the meta-collection, determining how the collections are 
        /// determined from the inputs.
        /// </summary>
        GenerateOption Type { get; }

        /// <summary>
        /// The upper index of the meta-collection, which is the size of the input collection.
        /// </summary>
        int UpperIndex { get; }

        /// <summary>
        /// The lower index of the meta-collection, which is the size of each output collection.
        /// </summary>
        int LowerIndex { get; }
    }
}