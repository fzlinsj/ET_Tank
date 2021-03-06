using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ETModel_Tank_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.Tank);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("set_Position", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Position_0);

            field = type.GetField("m_coefficient", flag);
            app.RegisterCLRFieldGetter(field, get_m_coefficient_0);
            app.RegisterCLRFieldSetter(field, set_m_coefficient_0);
            field = type.GetField("m_hpChange", flag);
            app.RegisterCLRFieldGetter(field, get_m_hpChange_1);
            app.RegisterCLRFieldSetter(field, set_m_hpChange_1);


        }


        static StackObject* set_Position_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @value = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ETModel.Tank instance_of_this_method = (ETModel.Tank)typeof(ETModel.Tank).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Position = value;

            return __ret;
        }


        static object get_m_coefficient_0(ref object o)
        {
            return ETModel.Tank.m_coefficient;
        }
        static void set_m_coefficient_0(ref object o, object v)
        {
            ETModel.Tank.m_coefficient = (System.Int32)v;
        }
        static object get_m_hpChange_1(ref object o)
        {
            return ETModel.Tank.m_hpChange;
        }
        static void set_m_hpChange_1(ref object o, object v)
        {
            ETModel.Tank.m_hpChange = (System.Action<System.Int32, System.Int32>)v;
        }


    }
}
