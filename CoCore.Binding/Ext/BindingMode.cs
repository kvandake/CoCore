namespace CoCore.Binding
{
    /// <summary>
    /// The mode of the <see cref="Binding"/>.
    /// </summary>
    ////[ClassInfo(typeof(Binding))]
    public enum BindingMode
    {
        /// <summary>
        /// A default binding is a one way binding.
        /// </summary>
        Default = 0,

        /// <summary>
        /// A one way binding, where the changes to the source
        /// property will update the target property, but changes to the
        /// target property don't affect the source property.
        /// </summary>
        OneWay = 1,

        /// <summary>
        /// A two way binding, where the changes to the source
        /// property will update the target property, and vice versa.
        /// </summary>
        TwoWay = 2
    }
}

