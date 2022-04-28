using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FieldEntity<T> : FieldManager<T>
    {
        private Dictionary<string, EntityProperty> EntityBody;

        public List<string> Fields => EntityBody.Keys.ToList();

        internal FieldEntity(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            EntityBody = new Dictionary<string, EntityProperty>();
            Type type = typeof(T);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                this.EntityBody.Add(prop.Name, new EntityProperty(prop));
            }
        }

        public LocalBuilder this[string Name]
        {
            get
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                LocalBuilder item = generator.DeclareLocal(EntityBody[Name].type);
                Output();
                Emit(OpCodes.Callvirt, EntityBody[Name].get);
                Emit(OpCodes.Stloc_S, item);
                return item;
            }

            set
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                Output();
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Callvirt, EntityBody[Name].set);
            }
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder GetValue(string FieldName)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            LocalBuilder item = generator.DeclareLocal(EntityBody[FieldName].type);
            Output();
            Emit(OpCodes.Callvirt, EntityBody[FieldName].get);
            Emit(OpCodes.Stloc_S, item);
            return item;
        }

        public void SetValue(string FieldName, LocalBuilder value)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            Output();
            Emit(OpCodes.Ldloc_S, value);
            Emit(OpCodes.Callvirt, EntityBody[FieldName].set);
        }

        private bool ContanisKey(string Name)
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

    public class FieldEntity : FieldManager
    {
        private Dictionary<string, EntityProperty> EntityBody;

        public List<string> Fields => EntityBody.Keys.ToList();

        internal FieldEntity(LocalBuilder stack, ILGenerator generator) : base(stack, generator)
        {
            EntityBody = new Dictionary<string, EntityProperty>();
            Type type = stack.LocalType;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                this.EntityBody.Add(prop.Name, new EntityProperty(prop));
            }
        }

        public LocalBuilder this[string Name]
        {
            get
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                LocalBuilder item = generator.DeclareLocal(EntityBody[Name].type);
                Output();
                Emit(OpCodes.Callvirt, EntityBody[Name].get);
                Emit(OpCodes.Stloc_S, item);
                return item;
            }

            set
            {
                if (!ContanisKey(Name)) ManagerGX.ShowEx("Entity prop is null;");
                Output();
                Emit(OpCodes.Ldloc_S, value);
                Emit(OpCodes.Callvirt, EntityBody[Name].set);
            }
        }

        public FieldBoolean IsNull()
        {
            return this.IsNull(this);
        }

        public LocalBuilder GetValue(string FieldName)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            LocalBuilder item = generator.DeclareLocal(EntityBody[FieldName].type);
            Output();
            Emit(OpCodes.Callvirt, EntityBody[FieldName].get);
            Emit(OpCodes.Stloc_S, item);
            return item;
        }

        public void SetValue(string FieldName, LocalBuilder value)
        {
            if (!ContanisKey(FieldName)) ManagerGX.ShowEx("Entity property is null;");
            Output();
            Emit(OpCodes.Ldloc_S, value);
            Emit(OpCodes.Callvirt, EntityBody[FieldName].set);
        }

        private bool ContanisKey(string Name)
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