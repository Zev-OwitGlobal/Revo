using System;

namespace Revo.MongoDB.Attributes
{
    /// <summary>
    /// Represents an attribute which allows you to specify of the name of the collection.
    /// The attribute takes precedence over anything else, and if not present the 
    /// Framework will fall back to the Pluralized method.
    /// </summary>
	[AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the collection in which your documents are stored.
        /// </summary>
		public string Name { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">
        /// The name of the collection.
        /// </param>
		public CollectionNameAttribute(string name)
        {
            Name = name;
        }

        public CollectionNameAttribute()
        {
        }
    }
}
