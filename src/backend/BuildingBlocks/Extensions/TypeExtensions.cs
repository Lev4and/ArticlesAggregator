namespace Extensions;

public static class TypeExtensions
{
    extension(Type type)
    {
        public bool HasInterface(Type typeInterface)
        {
            return type.GetInterfaces().Any(item => 
                item.IsGenericType
                    ? item.GetGenericTypeDefinition() == typeInterface
                    : item == typeInterface);
        }

        public IEnumerable<Type> GetGenericInterfaces(Type typeGenericInterface)
        {
            return type.GetInterfaces()
                .Where(interfaceType => interfaceType.GetGenericTypeDefinition() == typeGenericInterface);
        }
    }
}