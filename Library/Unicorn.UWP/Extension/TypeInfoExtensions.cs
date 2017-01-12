// Copyright (c) 2016 John Shu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unicorn
{
    public static class TypeInfoExtensions
    {
        public static MethodInfo[] GetMethods(this TypeInfo type)
        {
            var methods = new List<MethodInfo>();

            while (true)
            {
                methods.AddRange(type.DeclaredMethods);

                Type type2 = type.BaseType;

                if (type2 == null)
                {
                    break;
                }

                type = type2.GetTypeInfo();
            }

            return methods.ToArray();
        }

        public static MethodInfo GetMethod(this TypeInfo type, string methodName)
        {
            var methods = new List<MethodInfo>();

            while (true)
            {
                methods.AddRange(type.DeclaredMethods);

                Type type2 = type.BaseType;

                if (type2 == null)
                {
                    break;
                }

                type = type2.GetTypeInfo();
            }

            return methods.FirstOrDefault(m => m.Name == methodName);
        }

        /// <summary>
        /// 找出 PropertyInfo (包含 BaseType)
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(this TypeInfo targetType, string targetName)
        {
            var result = targetType.GetDeclaredProperty(targetName);

            if (result == null)
            {
                var baseType = targetType.BaseType?.GetTypeInfo();
                if (baseType != null)
                {
                    result = baseType.GetPropertyInfo(targetName);
                }
            }

            return result;
        }
    }
}
