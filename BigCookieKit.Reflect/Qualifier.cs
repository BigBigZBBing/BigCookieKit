using System.Reflection;

namespace BigCookieKit.Reflect
{
    public enum ClassQualifier
    {
        Public = TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
        PublicOrAbstract = Public | TypeAttributes.Abstract,
        PublicOrStatic = Public | TypeAttributes.Abstract | TypeAttributes.Sealed,
        PublicOrInterface = TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.AutoClass | TypeAttributes.AnsiClass,
        PublicOrSealed = Public | TypeAttributes.Sealed,

        Internal = TypeAttributes.NotPublic | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
        InternalOrAbstract = Internal | TypeAttributes.Abstract,
        InternalOrStatic = Internal | TypeAttributes.Abstract | TypeAttributes.Sealed,
        InternalOrInterface = TypeAttributes.NotPublic | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.AutoClass | TypeAttributes.AnsiClass,
        InternalOrSealed = Internal | TypeAttributes.Sealed,

        PublicStruct = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
        InternalStruct = TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit
    }

    public enum FieldQualifier
    {
        Public = FieldAttributes.Public,
        PublicOrStatic = Public | FieldAttributes.Static,

        Private = FieldAttributes.Private,
        PrivateOrStatic = Private | FieldAttributes.Static,

        Internal = FieldAttributes.Assembly,
        InternalOrStatic = Internal | MethodAttributes.Static,

        Protected = MethodAttributes.Family,
        ProtectedOrStatic = Protected | MethodAttributes.Static,
    }

    public enum PropertyQualifier
    {
        Public = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
        PublicOrStatic = Public | MethodAttributes.Static,
        PublicOrOverride = Public | MethodAttributes.Virtual,
        PublicOrVirtual = PublicOrOverride | MethodAttributes.NewSlot,

        Private = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
        PrivateOrStatic = Private | MethodAttributes.Static,

        Internal = MethodAttributes.Assembly | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
        InternalOrStatic = Internal | MethodAttributes.Static,
        InternalOrOverride = Internal | MethodAttributes.Virtual,
        InternalOrVirtual = InternalOrOverride | MethodAttributes.NewSlot,

        Protected = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
        ProtectedOrStatic = Protected | MethodAttributes.Static,
        ProtectedOrOverride = Protected | MethodAttributes.Virtual,
        ProtectedOrVirtual = ProtectedOrOverride | MethodAttributes.NewSlot,

        Interface = MethodAttributes.Abstract,
        Inherit = MethodAttributes.Final,
    }

    public enum MethodQualifier
    {
        Public = MethodAttributes.Public | MethodAttributes.HideBySig,
        PublicOrStatic = Public | MethodAttributes.Static,
        PublicOrVirtual = Public | MethodAttributes.NewSlot | MethodAttributes.Virtual,
        PublicOrOverride = PublicOrVirtual | MethodAttributes.Final,

        Private = MethodAttributes.Private | MethodAttributes.HideBySig,
        PrivateOrStatic = Private | MethodAttributes.Static,

        Internal = MethodAttributes.Assembly | MethodAttributes.HideBySig,
        InternalOrStatic = Internal | MethodAttributes.Static,
        InternalOrVirtual = Internal | MethodAttributes.NewSlot | MethodAttributes.Virtual,
        InternalOrOverride = InternalOrVirtual | MethodAttributes.Final,

        Protected = MethodAttributes.Family | MethodAttributes.HideBySig,
        ProtectedOrStatic = Protected | MethodAttributes.Static,
        ProtectedOrVirtual = Protected | MethodAttributes.NewSlot | MethodAttributes.Virtual,
        ProtectedOrOverride = ProtectedOrVirtual | MethodAttributes.Final,

        Interface = MethodAttributes.Abstract,
        Inherit = MethodAttributes.Final,

        Property = MethodAttributes.SpecialName
    }
}