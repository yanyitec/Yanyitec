using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    public class InstanceFactoryGenerator
    {
        public InstanceFactoryGenerator(Injection injection)
        {
            this.Injection = injection;
        }
        public System.Threading.ReaderWriterLockSlim AsyncLocker { get { return this.Injection.AsyncLocker; } }
        public Injection Injection { get; private set; }

        Injection InternalFindDependence(string name)
        {
            return this.Injection.FindDepedence(name);
        }
        Injection InternalFindDependence(Type type)
        {
            return this.Injection.FindDepedence(type.FullName);
        }


        public InjectionKinds CreationKind
        {
            get { return this.Injection.Kind; }
        }

        public Type InjectionType { get { return this.Injection.InjectionType; } }

        
        public Func<object> Generate()
        {
            return CreateInstanceFactory();
        }

        

        private readonly static Type InitializableInterfaceType = typeof(IInitializable);

        private static readonly MethodInfo InitializableInitializeMethodInfo = InitializableInterfaceType.GetMethod("Initialize");

        private Func<object> CreateInstanceFactory()
        {
            var locals = new List<ParameterExpression>();
            var typedVars = new Dictionary<int, ParameterExpression>();
            var namedVars = new Dictionary<string, ParameterExpression>();
            var creations = new List<Expression>();
            var resultExpr = this.InitExpression(this.InjectionType, locals, typedVars, namedVars, creations);


            var labelTarget = Expression.Label(typeof(object));
            var returnValueExpr = Expression.Convert(resultExpr, typeof(object));
            var retExpr = Expression.Return(labelTarget, returnValueExpr);
            var labelExpr = Expression.Label(labelTarget, resultExpr);
            creations.Add(retExpr);
            creations.Add(labelExpr);

            Expression block = Expression.Block(locals, creations);
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Func<object>>(block);

            var result = lamda.Compile();
            return result;
            
        }


        private ParameterExpression InitExpression(
            Type type,
            List<ParameterExpression> locals,
            Dictionary<int, ParameterExpression> typedVars,
            Dictionary<string, ParameterExpression> namedVars,
            List<Expression> creations)
        {

            var ctor = GetConstructor(type);
            if (ctor == null)
            {
                throw new ArgumentException("Cannot find the constructor for create instance in Type[" + type.FullName + "]");
            }
            var newExpr = this.NewExpression(ctor, locals, typedVars, namedVars, creations);
            var instanceExpr = Expression.Variable(ctor.DeclaringType);
            locals.Add(instanceExpr);
            creations.Add(Expression.Assign(instanceExpr, newExpr));
            var members = type.GetMembers();
            foreach (var memberInfo in members)
            {
                bool isProp = (memberInfo as PropertyInfo) != null;
                bool isField = false;
                if (!isProp)
                {
                    isField = (memberInfo as FieldInfo) != null;
                    if (!isField) continue;
                }


                this.MemberExpression(memberInfo, instanceExpr, locals, typedVars, namedVars, creations);
            }

            if (InitializableInterfaceType.IsAssignableFrom(this.InjectionType))
            {
                var callInitialize = Expression.Call(instanceExpr, InitializableInitializeMethodInfo);
                creations.Add(callInitialize);
            }

            return instanceExpr;
        }
        private Expression NewExpression(
            ConstructorInfo ctor,
            List<ParameterExpression> locals,
            Dictionary<int, ParameterExpression> typedVars,
            Dictionary<string, ParameterExpression> namedVars,
            List<Expression> creations)
        {
            var parameters = ctor.GetParameters();
            var paramExpressions = new List<Expression>();
            foreach (var parameterInfo in parameters)
            {
                var item = this.InternalFindDependence(parameterInfo.Name) ?? this.InternalFindDependence(parameterInfo.ParameterType.FullName);
                if (item == null)
                {
                    if (parameterInfo.HasDefaultValue)
                    {
                        paramExpressions.Add(Expression.Constant(parameterInfo.DefaultValue, parameterInfo.ParameterType));
                    }
                    else
                    {
                        throw new Exception("The parameter [" + parameterInfo.Name + "] of [" + ctor.DeclaringType.FullName + "] was not defined default value and can not be found in inject context.");
                    }
                }
                else
                {
                    var itemGenerator = new InstanceFactoryGenerator(item);
                    var instanceExpr = itemGenerator.ValueExpression(locals, typedVars, namedVars, creations,parameterInfo.ParameterType);
                    paramExpressions.Add(instanceExpr);
                    item.Changed += (sender, e) =>  this.Injection.ResetInstanceFactory();
                    
                }

            }
            return Expression.New(ctor, paramExpressions);

        }

        private void MemberExpression(
            MemberInfo memberInfo,
            Expression instanceExpr,
            List<ParameterExpression> locals,
            Dictionary<int, ParameterExpression> typedVars,
            Dictionary<string, ParameterExpression> namedVars,
            List<Expression> creations)
        {
            var fieldInfo = memberInfo as FieldInfo;
            var propInfo = memberInfo as PropertyInfo;
            if (fieldInfo != null)
            {
                var item = this.InternalFindDependence(fieldInfo.FieldType) ?? this.InternalFindDependence(fieldInfo.Name);
                if (item == null)
                {
                    return;
                }
                var itemGenerator = new InstanceFactoryGenerator(item);
                var valueExpr = itemGenerator.ValueExpression(locals, typedVars, namedVars, creations);
                if (valueExpr != null) {
                    var assignExpr = Expression.Assign(Expression.Field(instanceExpr, fieldInfo), valueExpr);
                    creations.Add(assignExpr);
                    item.Changed += (sender, e) => this.Injection.ResetInstanceFactory();
                }
                
            }
            if (propInfo != null && propInfo.CanWrite)
            {
                var item = this.InternalFindDependence(propInfo.PropertyType) ?? this.InternalFindDependence(propInfo.Name);
                if (item == null)
                {
                    return;
                }
                var itemGenerator = new InstanceFactoryGenerator(item);
                var valueExpr = itemGenerator.ValueExpression(locals, typedVars, namedVars, creations,propInfo.PropertyType);
                if (valueExpr != null) {
                    var assignExpr = Expression.Assign(Expression.Property(instanceExpr, propInfo), valueExpr);
                    creations.Add(assignExpr);
                    item.Changed += (sender, e) => this.Injection.ResetInstanceFactory();
                }
                
            }

        }

        /// <summary>
        /// 该函数会被其他的Generator调用
        /// </summary>
        /// <param name="locals"></param>
        /// <param name="typedVars"></param>
        /// <param name="namedVars"></param>
        /// <param name="creations"></param>
        /// <returns></returns>
        public Expression ValueExpression(
            List<ParameterExpression> locals,
            Dictionary<int, ParameterExpression> typedVars,
            Dictionary<string, ParameterExpression> namedVars,
            List<Expression> creations,Type expectValueType =null)
        {
            if (this.CreationKind == InjectionKinds.AlwaysNew)
            {
                return Expression.Call(Expression.Constant(this.Injection), CreateInstanceMethodInfo);
            }
            else if (this.CreationKind == InjectionKinds.Constant)
            {
                var constValue = this.Injection.GetOrCreateInstanceFunc(false,this.AsyncLocker)();
                if (expectValueType != null && constValue!=null && !expectValueType.IsAssignableFrom(constValue.GetType())) {
                    var valueStr = constValue.ToString();
                    constValue = StringConvertExtension.ConvertTo(valueStr, expectValueType);
                    if (constValue == null) throw new ArgumentException("Cannot convert \"" + valueStr + "\" to type[" + expectValueType.FullName + "]");
                    //var parsedValue = valueStr.ConvertTo(expectValueType) as Nullable;
                    
                }
                return Expression.Constant(constValue,expectValueType);
            }

            ParameterExpression varExpr = null;
            var name = this.Injection.Name ?? this.Injection.InjectionType.FullName;
            if (!namedVars.TryGetValue(name, out varExpr))
            {
                varExpr = this.InitExpression(
                    this.InjectionType,
                    locals,
                    typedVars,
                    namedVars,
                    creations);
                namedVars.Add(name, varExpr);
            }

            return varExpr;
        }



        private static readonly MethodInfo CreateInstanceMethodInfo = typeof(Injection).GetMethod("CreateInstance");
        private static ConstructorInfo GetConstructor(Type type)
        {
            var ctors = type.GetConstructors();
            ConstructorInfo injectCtor = null;
            ConstructorInfo defaultCtor = null;
            foreach (var ctor in ctors)
            {
                if (ctor.GetParameters().Length == 0)
                {
                    defaultCtor = ctor;
                }

                var injectAttr = ctor.GetCustomAttribute<InjectableAttribute>();
                if (injectAttr != null)
                {
                    injectCtor = ctor;
                }
            }

            return injectCtor ?? defaultCtor;
        }

    }
}
