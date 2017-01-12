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
using System.IO;
using System.Reflection;
using System.Text;

namespace Unicorn.ServiceModel
{
    public static class HttpParameterPacker
    {
        public static HttpParameterPackResult CreatePackedParameterResult(object parameter)
        {
            var packResult = new HttpParameterPackResult();

            var propertyArray = parameter.GetType().GetRuntimeProperties();
            var getParameters = new StringBuilder();

            foreach (var property in propertyArray)
            {
                if (CheckIsIgnoreProperty(property))
                {
                    continue;
                }

                var normalAttribute = property.GetCustomAttribute<HttpPropertyAttribute>();
                if (normalAttribute != null)
                {
                    CreateNormalParameter(parameter, property, normalAttribute, packResult, getParameters);
                    continue;
                }

                var rawStringAttribute = property.GetCustomAttribute<HttpRawStringPropertyAttribute>();
                if (rawStringAttribute != null)
                {
                    CreateRawStringParameter(parameter, property, rawStringAttribute, packResult, getParameters);
                    continue;
                }

                var rawByteArrayAttribute = property.GetCustomAttribute<HttpRawByteArrayPropertyAttribute>();
                if (rawByteArrayAttribute != null)
                {
                    CreateRawByteArrayParameter(parameter, property, rawStringAttribute, packResult, getParameters);
                    continue;
                }

                var multiPartAttribute = property.GetCustomAttribute<HttpMultiPartPropertyAttribute>();
                if (multiPartAttribute != null)
                {
                    CreateMultiPartParameter(parameter, property, multiPartAttribute, packResult);
                    continue;
                }
            }

            if (getParameters.Length > 0)
            {
                getParameters.Remove(0, 1);
            }
            packResult.GetCombindedString = getParameters.ToString();

            return packResult;
        }

        private static bool CheckIsIgnoreProperty(PropertyInfo property)
        {
            var ignoreAttribute = property.GetCustomAttribute<HttpIgnoreAttribute>();
            return ignoreAttribute != null;
        }

        #region 產生一般的參數

        private static void CreateNormalParameter(object parameter, PropertyInfo property, HttpPropertyAttribute attribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            string propertyName;
            string propertyValue;
            GetPropertyNameAndValue(parameter, property, attribute, out propertyName, out propertyValue);

            if ((string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue)) && attribute.IgnoreNullEmpty)
            {
                return;
            }

            switch (attribute.For)
            {
                case HttpPropertyFor.GET:
                    var urlEncodedValue = System.Net.WebUtility.UrlEncode(propertyValue);
                    getParameters.Append($"&{propertyName}={urlEncodedValue}");
                    break;
                case HttpPropertyFor.POST:
                    packResult.PostParameterMap.Add(propertyName, propertyValue);
                    break;
                case HttpPropertyFor.HEADER:
                    packResult.HeaderParameterMap.Add(propertyName, propertyValue);
                    break;
            }
        }

        private static void GetPropertyNameAndValue(object parameter, PropertyInfo property, HttpPropertyAttribute attribute, out string propertyName, out string propertyValue)
        {
            propertyName = null;
            propertyValue = null;

            if (attribute == null)
            {
                propertyName = property.Name;
                propertyValue = string.Format("{0}", property.GetValue(parameter, null));
                return;
            }

            propertyName = attribute.Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (attribute.ConveterType == null)
            {
                propertyValue = string.Format("{0}", property.GetValue(parameter, null));
                return;
            }

            var converter = Activator.CreateInstance(attribute.ConveterType) as IServiceParameterConveter;
            propertyValue = converter.Convert(property.GetValue(parameter, null));
        }

        #endregion

        #region 產生 Raw String 的參數

        private static void CreateRawStringParameter(object parameter, PropertyInfo property, HttpRawStringPropertyAttribute rawStringAttribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            var value = property.GetValue(parameter);
            if (value == null)
            {
                return;
            }

            packResult.PostRawStringList.Add(value.ToString());
        }

        #endregion

        #region 產生 Raw Byte Array 的參數

        private static void CreateRawByteArrayParameter(object parameter, PropertyInfo property, HttpRawStringPropertyAttribute rawStringAttribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            var value = property.GetValue(parameter);
            if (value == null)
            {
                return;
            }

            packResult.PostRawData = (byte[])value;
        }

        #endregion

        #region 建立 Multi-Part 參數

        private static void CreateMultiPartParameter(object parameter, PropertyInfo property, HttpMultiPartPropertyAttribute attribute, HttpParameterPackResult packResult)
        {
            switch (attribute.ContentType)
            {
                case MultipartContentType.File:
                    AddFileMultiPartItem(parameter, property, attribute, packResult);
                    break;
                case MultipartContentType.Json:
                    AddJsonMultiPartItem(parameter, property, attribute, packResult);
                    break;
            }
        }

        private static void AddFileMultiPartItem(object parameter, PropertyInfo property, HttpMultiPartPropertyAttribute attribute, HttpParameterPackResult packResult)
        {
            var fileContent = property.GetValue(parameter, null) as byte[];
            if (fileContent == null || fileContent.Length == 0)
            {
                return;
            }

            string contentType = "application/octet-stream";
            if (!string.IsNullOrEmpty(attribute.FileName))
            {
                var fileExtension = Path.GetExtension(attribute.FileName);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    fileExtension = fileExtension.Replace(".", string.Empty);
                    contentType = ApacheMimeTypes.MimeTypes[fileExtension];
                }
            }

            var multiPartItem = new HttpMutiPartPackItem
            {
                FileName = attribute.FileName,
                Content = fileContent,
                ContentType = contentType,
            };
            packResult.MutliPartParameterMap.Add(attribute.Name, multiPartItem);
        }

        private static void AddJsonMultiPartItem(object parameter, PropertyInfo property, HttpMultiPartPropertyAttribute attribute, HttpParameterPackResult packResult)
        {
            var jsonString = property.GetValue(parameter, null) as string;
            if (string.IsNullOrEmpty(jsonString))
            {
                return;
            }

            var jsonContent = Encoding.UTF8.GetBytes(jsonString);
            const string contentType = "application/json";

            var multiPartItem = new HttpMutiPartPackItem
            {
                ContentType = contentType,
                Content = jsonContent,
            };
            packResult.MutliPartParameterMap.Add(attribute.Name, multiPartItem);
        }

        #endregion
    }
}
