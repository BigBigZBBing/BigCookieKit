using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit.Field
{
    public class FieldEntity<T> : FieldManager<T>
    {
        private Dictionary<String, EntityProperty> EntityBody;

        public List<String> Fields => EntityBody.Keys.ToList();

        internal FieldEntity(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            EntityBody = new Dictionary<String, EntityProperty>();
            Type type = typeof(T);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                this.EntityBody.Add(prop.Name, new EntityProperty(prop));
            }
        }

        public LocalBuilder this[String Name]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                LocalBuilder item = generator.DeclareLocal(EntityBody[Name].type);
                Output();
                Emit(OpCodes.Callvirt, EntityBody[Name].get);
                Emit(OpCodes.Stloc_S, item);
                return item;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                Output();
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Callvirt, EntityBody[Name].set);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalBuilder GetValue(String FieldName)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            LocalBuilder item = generator.DeclareLocal(EntityBody[FieldName].type);
            Output();
            Emit(OpCodes.Callvirt, EntityBody[FieldName].get);
            Emit(OpCodes.Stloc_S, item);
            return item;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(String FieldName, LocalBuilder value)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            Output();
            Emit(OpCodes.Ldloc_S, value);
            Emit(OpCodes.Callvirt, EntityBody[FieldName].set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Boolean ContanisKey(String Name)
        {
            return EntityBody.ContainsKey(Name);
        }

        private struct EntityProperty
        {
            public EntityProperty(PropertyInfo property)
            {
                this.property = property;
            }

            public PropertyInfo property { get; set; }
            public Type type => property.PropertyType;
            public MethodInfo get => property.GetGetMethod();
            public MethodInfo set => property.GetSetMethod();
        }
    }
}
