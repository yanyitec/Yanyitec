using Yanyitec.Mvc.ParameterBinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public class Command : ICommand
    {
        public Command(MethodInfo actionInfo,IParameterBinders binders) {
            this.command = GenCommand(actionInfo,binders);
        }

        Func<object, IInputData, object> command;

        public object Execute(object controller, IInputData inputData) {
            return command(controller,inputData);
        }

        public HttpMethods AcceptMethods { get; set; }

        readonly static Type IInputDataType = typeof(IInputData);
        readonly static MethodInfo InputDataGetContextValueMethodInfo = IInputDataType.GetMethod("GetContextValue");
        readonly static MethodInfo ParameterBinderGetValueMethodInfo = typeof(IParameterBinder).GetMethod("GetValue");

        static Func<object, IInputData, object> GenCommand(MethodInfo methodInfo, IParameterBinders binders)
        {
            var parameters = methodInfo.GetParameters();
            var controllerExpr = Expression.Parameter(typeof(object), "controller");
            var inputDataExpr = Expression.Parameter(typeof(IInputData), "inputData");
            List<ParameterExpression> varExprs = new List<ParameterExpression>();

            IList<ParameterExpression> argExprs = new List<ParameterExpression>();
            List<Expression> codeExprs = new List<Expression>();
            #region handler parameters
            foreach (var paraInfo in parameters)
            {
                var paramTypeInfo = paraInfo.ParameterType.GetTypeInfo();
                var binder = binders.GetOrCreateBinder(paraInfo.ParameterType);
                //var User user = null;
                var paramValueExpr = Expression.Parameter(paraInfo.ParameterType, paraInfo.Name);
                varExprs.Add(paramValueExpr);
                argExprs.Add(paramValueExpr);

                if (paramTypeInfo.IsClass)
                {
                    // user = inputData.GetContextValue(typeof(User)) as User;
                    var getContextValueExpr = Expression.Convert(Expression.Call(controllerExpr, InputDataGetContextValueMethodInfo, Expression.Constant(paraInfo.ParameterType)), paraInfo.ParameterType);
                    var assignExpr = Expression.Assign(paramValueExpr, getContextValueExpr);
                    codeExprs.Add(assignExpr);
                    // if(user==null) user = binder.GetValue("user",inputData) as User;
                    var getBindValueExpr = Expression.Convert(
                        Expression.Call(Expression.Constant(binder), ParameterBinderGetValueMethodInfo, Expression.Constant(paraInfo.Name), inputDataExpr)
                        , paraInfo.ParameterType
                    );
                    var checkExpr = Expression.IfThen(
                        Expression.Equal(paramValueExpr, Expression.Constant(null))
                        , Expression.Assign(paramValueExpr, getBindValueExpr)
                    );
                    codeExprs.Add(checkExpr);
                }
                else
                {
                    //var object userObj = null;
                    var paramValueObjExpr = Expression.Parameter(typeof(object), paraInfo.Name + "Obj");
                    varExprs.Add(paramValueObjExpr);
                    var getBindValueExpr = Expression.Call(Expression.Constant(binder), ParameterBinderGetValueMethodInfo, Expression.Constant(paraInfo.Name), inputDataExpr);
                    // userObj = binder.GetValue("user",inputData);
                    var assignExpr = Expression.Assign(paramValueObjExpr, getBindValueExpr);
                    codeExprs.Add(assignExpr);
                    if (paraInfo.HasDefaultValue)
                    {
                        //if(userObj==null) user = "1" else user = (string)userObj;
                        var checkDefault = Expression.IfThenElse(
                            Expression.Equal(paramValueObjExpr, Expression.Constant(null))
                            , Expression.Assign(paramValueExpr, Expression.Constant(paraInfo.DefaultValue))
                            , Expression.Assign(paramValueExpr, Expression.Convert(paramValueObjExpr, paraInfo.ParameterType))
                        );
                        codeExprs.Add(checkDefault);
                    }
                }
            }
            #endregion

            #region call and return
            Expression resultExpr = null;
            if (methodInfo.ReturnType != null && methodInfo.ReturnType != typeof(void))
            {
                resultExpr = Expression.Call(controllerExpr, methodInfo, argExprs);

            }
            else
            {
                resultExpr = Expression.Constant(null, typeof(object));


            }
            var labelTarget = Expression.Label(typeof(object));
            var returnValueExpr = Expression.Convert(resultExpr, typeof(object));
            var retExpr = Expression.Return(labelTarget, returnValueExpr);
            var labelExpr = Expression.Label(labelTarget, resultExpr);
            codeExprs.Add(retExpr);
            codeExprs.Add(labelExpr);
            #endregion

            Expression block = Expression.Block(varExprs, codeExprs);
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Func<object, IInputData, object>>(block, argExprs);
            return lamda.Compile();
        }
    }
}
