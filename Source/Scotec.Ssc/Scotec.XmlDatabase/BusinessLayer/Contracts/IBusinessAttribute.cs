namespace Scotec.XMLDatabase
{
    public interface IBusinessAttribute
    {
        /// <summary>
        ///   The name of the attribute
        /// </summary>
        string ObjectName { get; }

        /// <summary>
        ///   The attribute value.
        ///   Derived interfaces should override this property with an typesafe object.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        ///   The attribute default value.
        ///   Derived interfaces should override this property with an typesafe object.
        /// </summary>
        object DefaultValue { get; }


        /// <summary>
        ///   The owner of the attribute.
        /// </summary>
        IBusinessObject Owner { get; }

        /// <summary>
        ///   Checks whether the attribute contains valid data. Typically data are invalid
        ///   after the attribute has been deleted but there are still references to the attribute.
        /// </summary>
        bool DataAvailable { get; }


        /// <summary>
        ///   Getter for the owner document.
        /// </summary>
        IBusinessSession Session { get; }


        /// <summary>
        ///   Validates the given value. This method can be called to validate a value
        ///   before assigning it to the attribute.
        ///   Derived interfaces should override this property with an typesafe object.
        /// </summary>
        /// <param name = "value">The object to test.</param>
        /// <returns>Returns true if the value can be assigned. Otherwise false.</returns>
        bool Validate( object value );


        /// <summary>
        ///   Compares the business object with another instance.
        /// </summary>
        /// <param name = "obj">The business object to compare with.</param>
        /// <returns>Returns true if both objects are equal, otherwise false.</returns>
        bool IsSameAs( IBusinessObject obj );
    }
}
